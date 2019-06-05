using System;
using System.Collections.Generic;
namespace StorySystem.CommonCommands
{
    /// <summary>
    /// if(@val>0)
    /// {
    ///   createnpc(123);
    /// };
    /// 
    /// or
    /// 
    /// if(@val>0)
    /// {
    ///   createnpc(123);
    /// }
    /// else
    /// {
    ///   missioncomplete();
    /// };
    /// </summary>
    internal sealed class IfElseCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            IfElseCommand retCmd = new IfElseCommand();
            retCmd.m_Conditions = new List<IStoryValue<int>>();
            for (int i = 0; i < m_Conditions.Count; ++i) {
                retCmd.m_Conditions.Add(m_Conditions[i].Clone());
            }
            for (int i = 0; i < m_LoadedIfCommands.Count; i++) {
                List<IStoryCommand> cmds = new List<IStoryCommand>();
                for (int j = 0; j < m_LoadedIfCommands[i].Count; ++j) {
                    cmds.Add(m_LoadedIfCommands[i][j].Clone());
                }
                retCmd.m_LoadedIfCommands.Add(cmds);
            }
            for (int i = 0; i < m_LoadedElseCommands.Count; i++) {
                retCmd.m_LoadedElseCommands.Add(m_LoadedElseCommands[i].Clone());
            }
            retCmd.IsCompositeCommand = true;
            return retCmd;
        }
        protected override void ResetState()
        {
            m_AlreadyExecute = false;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            if (!m_AlreadyExecute) {
                for (int i = 0; i < m_Conditions.Count; ++i) {
                    m_Conditions[i].Evaluate(instance, handler, iterator, args);
                }
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta, object iterator, object[] args)
        {
            bool ret = false;
            if (!m_AlreadyExecute) {
                Evaluate(instance, handler, iterator, args);
                bool isElse = true;
                for (int i = 0; i < m_Conditions.Count; ++i) {
                    if (m_Conditions[i].Value != 0) {
                        PrepareIf(i, handler.RuntimeStack);
                        m_Runtime.Iterator = iterator;
                        m_Runtime.Arguments = args;
                        isElse = false;
                        break;
                    }
                }
                if (isElse) {
                    PrepareElse(handler.RuntimeStack);
                    m_Runtime.Iterator = iterator;
                    m_Runtime.Arguments = args;
                }
                m_AlreadyExecute = true;
                //没有wait之类命令直接执行
                m_Runtime.Tick(instance, handler, delta);
                if (m_Runtime.CommandQueue.Count == 0) {
                    handler.RuntimeStack.Pop();
                    ret = false;
                } else {
                    //遇到wait命令，跳出执行，之后直接在StoryMessageHandler里执行栈顶的命令队列（降低开销）
                    ret = true;
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
                    StoryValue<int> cond = new StoryValue<int>();
                    cond.InitFromDsl(param);
                    m_Conditions.Add(cond);
                }
                List<IStoryCommand> cmds = new List<IStoryCommand>();
                for (int i = 0; i < functionData.Statements.Count; i++) {
                    IStoryCommand cmd = StoryCommandManager.Instance.CreateCommand(functionData.Statements[i]);
                    if (null != cmd)
                        cmds.Add(cmd);
                }
                m_LoadedIfCommands.Add(cmds);
            }
            IsCompositeCommand = true;
        }
        protected override void Load(Dsl.StatementData statementData)
        {
            Load(statementData.First);
            int ct = statementData.Functions.Count;
            for (int stIx = 1; stIx < ct; ++stIx) {
                Dsl.FunctionData functionData = statementData.Functions[stIx];
                if (null != functionData) {
                    string funcId = functionData.GetId();
                    if (funcId == "elseif") {
                        Load(functionData);
                    } else if (funcId == "else") {
                        if (stIx == ct - 1) {
                            for (int i = 0; i < functionData.Statements.Count; i++) {
                                IStoryCommand cmd = StoryCommandManager.Instance.CreateCommand(functionData.Statements[i]);
                                if (null != cmd)
                                    m_LoadedElseCommands.Add(cmd);
                            }
                        } else {
#if DEBUG
                            string err = string.Format("[StoryDsl] else must be the last function !!! line:{0}", functionData.GetLine());
                            throw new Exception(err);
#else
              GameLibrary.LogSystem.Error("[StoryDsl] else must be the last function !!!");
#endif
                        }
                    }
                }
            }
        }
        private void PrepareIf(int ix, StoryRuntimeStack runtimeStack)
        {
            runtimeStack.Push(m_Runtime);
            var queue = runtimeStack.Peek().CommandQueue;
            foreach (IStoryCommand cmd in queue) {
                cmd.Reset();
            }
            queue.Clear();
            List<IStoryCommand> cmds = m_LoadedIfCommands[ix];
            for (int i = 0; i < cmds.Count; ++i) {
                IStoryCommand cmd = cmds[i];
                if (null != cmd.LeadCommand)
                    queue.Enqueue(cmd.LeadCommand);
                queue.Enqueue(cmd);
            }
        }
        private void PrepareElse(StoryRuntimeStack runtimeStack)
        {
            runtimeStack.Push(m_Runtime);
            var queue = runtimeStack.Peek().CommandQueue;
            foreach (IStoryCommand cmd in queue) {
                cmd.Reset();
            }
            queue.Clear();
            for (int i = 0; i < m_LoadedElseCommands.Count; ++i) {
                IStoryCommand cmd = m_LoadedElseCommands[i];
                if (null != cmd.LeadCommand)
                    queue.Enqueue(cmd.LeadCommand);
                queue.Enqueue(cmd);
            }
        }

        private List<IStoryValue<int>> m_Conditions = new List<IStoryValue<int>>();
        private StoryRuntime m_Runtime = new StoryRuntime();
        private List<List<IStoryCommand>> m_LoadedIfCommands = new List<List<IStoryCommand>>();
        private List<IStoryCommand> m_LoadedElseCommands = new List<IStoryCommand>();
        private bool m_AlreadyExecute = false;
    }
}
