using System;
using System.Collections;
using System.Collections.Generic;
namespace StorySystem.CommonCommands
{
    /// <summary>
    /// foreach(v1,v2,v3)
    /// {
    ///   createnpc($$);
    ///   wait(100);
    /// };
    /// </summary>
    internal sealed class ForeachCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            ForeachCommand retCmd = new ForeachCommand();
            for (int i = 0; i < m_LoadedIterators.Count; i++) {
                retCmd.m_LoadedIterators.Add(m_LoadedIterators[i].Clone());
            }
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
            }
            retCmd.IsCompositeCommand = true;
            return retCmd;
        }
        protected override void ResetState()
        {
            m_AlreadyExecute = false;
            m_CurIterator = null;
            m_Iterators.Clear();
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            if (m_AlreadyExecute)
                return;
            if (m_Iterators.Count <= 0 && m_LoadedIterators.Count > 0) {
                for (int i = 0; i < m_LoadedIterators.Count; i++) {
                    m_LoadedIterators[i].Evaluate(instance, handler, iterator, args);
                }
                for (int i = 0; i < m_LoadedIterators.Count; i++) {
                    m_Iterators.Enqueue(m_LoadedIterators[i].Value);
                }
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta, object iterator, object[] args)
        {
            Evaluate(instance, handler, iterator, args);
            bool ret = true;
            while (ret) {
                if (m_Iterators.Count > 0) {
                    Prepare(handler.RuntimeStack);
                    m_CurIterator = m_Iterators.Dequeue();
                    m_Runtime.Iterator = m_CurIterator;
                    m_Runtime.Arguments = args;
                    m_AlreadyExecute = true;
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
                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    Dsl.ISyntaxComponent param = callData.GetParam(i);
                    StoryValue val = new StoryValue();
                    val.InitFromDsl(param);
                    m_LoadedIterators.Add(val);
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

        private object m_CurIterator = null; 
        private Queue<object> m_Iterators = new Queue<object>();
        private StoryRuntime m_Runtime = new StoryRuntime();
        private List<IStoryValue<object>> m_LoadedIterators = new List<IStoryValue<object>>();
        private List<IStoryCommand> m_LoadedCommands = new List<IStoryCommand>();
        private bool m_AlreadyExecute = false;
    }
    /// <summary>
    /// looplist(list)
    /// {
    ///   createnpc($$);
    ///   wait(100);
    /// };
    /// </summary>
    internal sealed class LoopListCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            LoopListCommand retCmd = new LoopListCommand();
            retCmd.m_LoadedList = m_LoadedList.Clone();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
            }
            retCmd.IsCompositeCommand = true;
            return retCmd;
        }
        protected override void ResetState()
        {
            m_AlreadyExecute = false;
            m_CurIterator = null;
            m_Iterators.Clear();
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            if (m_AlreadyExecute)
                return;
            if (m_Iterators.Count <= 0) {
                m_LoadedList.Evaluate(instance, handler, iterator, args);
                foreach (object obj in m_LoadedList.Value) {
                    m_Iterators.Enqueue(obj);
                }
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta, object iterator, object[] args)
        {
            Evaluate(instance, handler, iterator, args);
            bool ret = true;
            while (ret) {
                if (m_Iterators.Count > 0) {
                    Prepare(handler.RuntimeStack);
                    m_CurIterator = m_Iterators.Dequeue();
                    m_Runtime.Iterator = m_CurIterator;
                    m_Runtime.Arguments = args;
                    m_AlreadyExecute = true;
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
                    m_LoadedList.InitFromDsl(callData.GetParam(0));
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
        
        private object m_CurIterator = null;
        private Queue<object> m_Iterators = new Queue<object>();
        private StoryRuntime m_Runtime = new StoryRuntime();
        private IStoryValue<IEnumerable> m_LoadedList = new StoryValue<IEnumerable>();
        private List<IStoryCommand> m_LoadedCommands = new List<IStoryCommand>();
        private bool m_AlreadyExecute = false;
    }
    /// <summary>
    /// loop(count)
    /// {
    ///   createnpc($$);
    ///   wait(100);
    /// };
    /// </summary>
    internal sealed class LoopCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            LoopCommand retCmd = new LoopCommand();
            retCmd.m_Count = m_Count.Clone();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
            }
            retCmd.IsCompositeCommand = true;
            return retCmd;
        }
        protected override void ResetState()
        {
            m_AlreadyExecute = false;
            m_CurCount = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            if (m_AlreadyExecute)
                return;
            m_Count.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta, object iterator, object[] args)
        {
            Evaluate(instance, handler, iterator, args);
            bool ret = true;
            while (ret) {
                if (m_CurCount < m_Count.Value) {
                    Prepare(handler.RuntimeStack);
                    m_Runtime.Iterator = m_CurCount;
                    m_Runtime.Arguments = args;
                    ++m_CurCount;
                    m_AlreadyExecute = true;
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
                    m_Count.InitFromDsl(param);
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

        private IStoryValue<int> m_Count = new StoryValue<int>();
        private StoryRuntime m_Runtime = new StoryRuntime();
        private List<IStoryCommand> m_LoadedCommands = new List<IStoryCommand>();
        private int m_CurCount = 0;
        private bool m_AlreadyExecute = false;
    }
}
