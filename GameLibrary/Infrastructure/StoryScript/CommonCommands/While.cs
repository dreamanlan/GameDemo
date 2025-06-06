﻿using System;
using System.Collections.Generic;
namespace StoryScript.CommonCommands
{
    /// <summary>
    /// while($val<10)
    /// {
    ///   createnpc(1000+$val);
    ///   inc($val);
    ///   wait(100);
    /// };
    /// </summary>
    public sealed class WhileCommand : AbstractStoryCommand
    {
        public override bool IsCompositeCommand { get { return true; } }
        protected override IStoryCommand CloneCommand()
        {
            WhileCommand retCmd = new WhileCommand();
            retCmd.m_LocalInfoIndex = m_LocalInfoIndex;
            retCmd.m_LoadedCondition = m_LoadedCondition.Clone();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
            }
            return retCmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            var localInfos = handler.LocalInfoStack.Peek();
            var condition = localInfos.GetLocalInfo(m_LocalInfoIndex) as IStoryFunction<int>;
            condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta, BoxedValue iterator, BoxedValueList args)
        {
            var runtime = handler.PeekRuntime();
            if (runtime.TryBreakLoop()) {
                return false;
            }
            bool ret = true;
            var localInfos = handler.LocalInfoStack.Peek();
            var condition = localInfos.GetLocalInfo(m_LocalInfoIndex) as IStoryFunction<int>;
            if (null == condition) {
                condition = m_LoadedCondition.Clone();
                localInfos.SetLocalInfo(m_LocalInfoIndex, condition);
            }
            while (ret) {
                Evaluate(instance, handler, iterator, args);
                if (condition.Value != 0) {
                    Prepare(handler);
                    runtime = handler.PeekRuntime();
                    runtime.Iterator = iterator;
                    runtime.Arguments = args;
                    ret = true;
                    //Execute directly without commands such as wait
                    runtime.Tick(instance, handler, delta);
                    if (runtime.CommandQueue.Count == 0) {
                        handler.PopRuntime(instance);
                        if (runtime.TryBreakLoop()) {
                            ret = false;
                            break;
                        }
                    } else {
                        //When encountering the wait command, jump out of execution, and then directly
                        //execute the command queue on the top of the stack in StoryMessageHandler
                        //(reducing overhead)
                        break;
                    }
                } else {
                    ret = false;
                }
            }
            return ret;
        }
        protected override bool Load(Dsl.FunctionData functionData)
        {
            if (functionData.IsHighOrder) {
                m_LocalInfoIndex = StoryCommandManager.Instance.AllocLocalInfoIndex();
                Dsl.FunctionData callData = functionData.LowerOrderFunction;
                if (null != callData) {
                    if (callData.GetParamNum() > 0) {
                        Dsl.ISyntaxComponent param = callData.GetParam(0);
                        m_LoadedCondition.InitFromDsl(param);
                    }
                    for (int i = 0; i < functionData.GetParamNum(); i++) {
                        IStoryCommand cmd = StoryCommandManager.Instance.CreateCommand(functionData.GetParam(i));
                        if (null != cmd)
                            m_LoadedCommands.Add(cmd);
                    }
                }
            }
            return true;
        }
        private void Prepare(StoryMessageHandler handler)
        {
            var runtime = StoryRuntime.New();
            handler.PushRuntime(runtime);
            var queue = runtime.CommandQueue;
            foreach (IStoryCommand cmd in queue) {
                cmd.Reset();
            }
            queue.Clear();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                IStoryCommand cmd = m_LoadedCommands[i];
                if (null != cmd.PrologueCommand)
                    queue.Enqueue(cmd.PrologueCommand);
                queue.Enqueue(cmd);
                if (null != cmd.EpilogueCommand)
                    queue.Enqueue(cmd.EpilogueCommand);
            }
        }

        private int m_LocalInfoIndex;
        private IStoryFunction<int> m_LoadedCondition = new StoryFunction<int>();
        private List<IStoryCommand> m_LoadedCommands = new List<IStoryCommand>();
    }
}
