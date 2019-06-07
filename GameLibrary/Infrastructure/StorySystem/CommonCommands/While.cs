﻿using System;
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
        protected override IStoryCommand CloneCommand()
        {
            WhileCommand retCmd = new WhileCommand();
            retCmd.m_LocalInfoIndex = m_LocalInfoIndex;
            retCmd.m_LoadedCondition = m_LoadedCondition.Clone();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
            }
            retCmd.IsCompositeCommand = true;
            return retCmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            var localInfos = handler.LocalInfoStack.Peek();
            var condition = localInfos.GetLocalInfo(m_LocalInfoIndex) as IStoryValue<int>;
            condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta, object iterator, object[] args)
        {
            bool ret = true;
            var localInfos = handler.LocalInfoStack.Peek();
            var condition = localInfos.GetLocalInfo(m_LocalInfoIndex) as IStoryValue<int>;
            if (null == condition) {
                condition = m_LoadedCondition.Clone();
                localInfos.SetLocalInfo(m_LocalInfoIndex, condition);
            }
            while (ret) {
                Evaluate(instance, handler, iterator, args);
                if (condition.Value != 0) {
                    Prepare(handler.RuntimeStack);
                    var runtime = handler.PeekRuntime();
                    runtime.Iterator = iterator;
                    runtime.Arguments = args;
                    ret = true;
                    //没有wait之类命令直接执行
                    runtime.Tick(instance, handler, delta);
                    if (runtime.CommandQueue.Count == 0) {
                        handler.PopRuntime();
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
            m_LocalInfoIndex = StoryCommandManager.Instance.AllocLocalInfoIndex();
            Dsl.CallData callData = functionData.Call;
            if (null != callData) {
                if (callData.GetParamNum() > 0) {
                    Dsl.ISyntaxComponent param = callData.GetParam(0);
                    m_LoadedCondition.InitFromDsl(param);
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
            var runtime = StoryRuntime.New();
            runtimeStack.Push(runtime);
            var queue = runtime.CommandQueue;
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

        private int m_LocalInfoIndex;
        private IStoryValue<int> m_LoadedCondition = new StoryValue<int>();
        private List<IStoryCommand> m_LoadedCommands = new List<IStoryCommand>();
    }
}
