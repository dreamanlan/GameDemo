using System;
using System.Collections.Generic;
namespace StorySystem.CommonCommands
{
    /// <summary>
    /// while($val<10)
    /// {
    ///   createnpc($$);
    ///   wait(100);
    /// };
    /// </summary>
    internal sealed class WhileCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            WhileCommand retCmd = new WhileCommand();
            retCmd.m_Condition = m_Condition.Clone();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
            }
            retCmd.IsCompositeCommand = true;
            return retCmd;
        }
        protected override void ResetState()
        {
            m_CurCount = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_Condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta, object iterator, object[] args)
        {
            bool ret = true;
            while (ret) {
                Evaluate(instance, handler, iterator, args);
                if (m_Condition.Value != 0) {
                    Prepare(handler.RuntimeStack);
                    m_Runtime.Iterator = iterator;
                    m_Runtime.Arguments = args;
                    ++m_CurCount;
                    ret = true;
                    //没有wait之类命令直接执行
                    m_Runtime.Tick(instance, handler, delta);
                    if (m_Runtime.CommandQueue.Count == 0) {
                        handler.RuntimeStack.Pop();
                    } else {
                        //遇到wait命令，跳出执行，之后直接在StoryMessageHandler里执行栈顶的命令队列（降低开销）
                        break;
                    }
                } else {
                    ret = false;
                }
            }
            return ret;
        }
        protected override void Load(Dsl.FunctionData functionData)
        {
            Dsl.CallData callData = functionData.Call;
            if (null != callData) {
                if (callData.GetParamNum() > 0) {
                    Dsl.ISyntaxComponent param = callData.GetParam(0);
                    m_Condition.InitFromDsl(param);
                }
                for (int i = 0; i < functionData.Statements.Count; i++) {
                    IStoryCommand cmd = StoryCommandManager.Instance.CreateCommand(functionData.Statements[i]);
                    if (null != cmd)
                        m_LoadedCommands.Add(cmd);
                }
            }
            IsCompositeCommand = true;
        }
        private void Prepare(StoryRuntimeStack runtimeStack)
        {
            runtimeStack.Push(m_Runtime);
            var queue = m_Runtime.CommandQueue;
            foreach (IStoryCommand cmd in queue) {
                cmd.Reset();
            }
            queue.Clear();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                IStoryCommand cmd = m_LoadedCommands[i];
                if (null != cmd.LeadCommand)
                    queue.Enqueue(cmd.LeadCommand);
                queue.Enqueue(cmd);
            }
        }

        private IStoryValue<int> m_Condition = new StoryValue<int>();
        private StoryRuntime m_Runtime = new StoryRuntime();
        private List<IStoryCommand> m_LoadedCommands = new List<IStoryCommand>();
        private int m_CurCount = 0;
    }
}
