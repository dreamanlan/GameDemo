﻿using System;
using System.Collections.Generic;
using GameLibrary;
namespace StorySystem
{
    /// <summary>
    /// story(1)
    /// {
    ///   onmessage(start)
    ///   {
    ///     dialog(1);
    ///   };
    ///   onmessage(enterarea, 1)
    ///   {
    ///     dialog(2);
    ///   };
    ///   onmessage(enddialog, 2)
    ///   {
    ///     createnpc(10,11,12);
    ///     movenpc(10,vector2(10,20));
    ///     aienable(10,11,12);
    ///   };
    ///   onmessage(killnpc,12)
    ///   {
    ///     missioncomplete();
    ///   };
    /// };
    /// </summary>
    public sealed class StoryRuntime
    {
        public object Iterator
        {
            get { return m_Iterator; }
            set { m_Iterator = value; }
        }
        public object[] Arguments
        {
            get { return m_Arguments; }
            set { m_Arguments = value; }
        }
        public Queue<IStoryCommand> CommandQueue
        {
            get { return m_CommandQueue; }
        }
        public void Reset()
        {
            foreach (IStoryCommand cmd in m_CommandQueue) {
                cmd.Reset();
            }
            m_CommandQueue.Clear();
        }
        public void Tick(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            while (m_CommandQueue.Count > 0) {
                IStoryCommand cmd = m_CommandQueue.Peek();
                if (cmd.Execute(instance, handler, delta, m_Iterator, m_Arguments)) {
                    break;
                } else {
                    cmd.Reset();
                    m_CommandQueue.Dequeue();
                }
            }
        }

        private object m_Iterator;
        private object[] m_Arguments;
        private Queue<IStoryCommand> m_CommandQueue = new Queue<IStoryCommand>();
    }
    public class StoryRuntimeStack : Stack<StoryRuntime>
    {
        public StoryRuntimeStack() { }
        public StoryRuntimeStack(int capacity) : base(capacity) { }
        public StoryRuntimeStack(IEnumerable<StoryRuntime> coll) : base(coll) { }
    }
    public sealed class StoryMessageHandler
    {
        public string MessageId
        {
            get { return m_MessageId; }
            set { m_MessageId = value; }
        }
        public bool IsTriggered
        {
            get { return m_IsTriggered; }
            set { m_IsTriggered = value; }
        }
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set { m_IsPaused = value; }
        }
        public bool IsInTick
        {
            get { return m_IsInTick; }
        }
        public StrObjDict StackVariables
        {
            get { return m_StackVariables; }
        }
        public StoryRuntimeStack RuntimeStack
        {
            get { return m_RuntimeStack; }
        }
        public StoryMessageHandler Clone()
        {
            StoryMessageHandler handler = new StoryMessageHandler();
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                handler.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
            }
            handler.m_MessageId = m_MessageId;
            handler.m_ArgumentNames = m_ArgumentNames;
            return handler;
        }
        public void Load(Dsl.FunctionData messageHandlerData)
        {
            Dsl.CallData callData = messageHandlerData.Call;
            if (null != callData && callData.HaveParam()) {
                int paramNum = callData.GetParamNum();
                string[] args = new string[paramNum];
                for (int i = 0; i < paramNum; ++i) {
                    args[i] = callData.GetParamId(i);
                }
                m_MessageId = string.Join(":", args);
            }
            RefreshCommands(messageHandlerData);
        }
        public void Load(Dsl.StatementData messageHandlerData)
        {
            Dsl.CallData first = messageHandlerData.First.Call;
            Dsl.FunctionData func = messageHandlerData.Second;
            Dsl.CallData second = func.Call;
            if (null != first && first.HaveParam()) {
                int paramNum = first.GetParamNum();
                string[] args = new string[paramNum];
                for (int i = 0; i < paramNum; ++i) {
                    args[i] = first.GetParamId(i);
                }
                m_MessageId = string.Join(":", args);
            }
            if (null != second && second.GetId() == "args" && second.HaveParam()) {
                int paramNum = second.GetParamNum();
                if (paramNum > 0) {
                    m_ArgumentNames = new string[paramNum];
                    for (int i = 0; i < paramNum; ++i) {
                        m_ArgumentNames[i] = second.GetParamId(i);
                    }
                }
            }
            RefreshCommands(func);
        }
        public void Reset()
        {
            if (m_IsTriggered) {
                LogSystem.Warn("Reset a running message handler !");
            }
            m_IsTriggered = false;
            m_IsPaused = false;
            while (RuntimeStack.Count > 0) {
                var runtime = RuntimeStack.Peek();
                runtime.Reset();
                RuntimeStack.Pop();
            }
            m_StackVariables.Clear();
        }
        public void Prepare()
        {
            Reset();
            RuntimeStack.Push(m_Runtime);
            for (int i = 0; i < m_LoadedCommands.Count; i++) {
                IStoryCommand cmd = m_LoadedCommands[i];
                if (null != cmd.LeadCommand)
                    m_Runtime.CommandQueue.Enqueue(cmd.LeadCommand);
                m_Runtime.CommandQueue.Enqueue(cmd);
            }
        }
        public void Tick(StoryInstance instance, long delta)
        {
            if (m_IsPaused) {
                return;
            }
            try {
                instance.StackVariables = StackVariables;
                m_IsInTick = true;
                var runtime = RuntimeStack.Peek();
                runtime.Tick(instance, this, delta);
                if (runtime.CommandQueue.Count == 0) {
                    RuntimeStack.Pop();
                }
                if (RuntimeStack.Count <= 0) {
                    m_IsTriggered = false;
                }
            } finally {
                m_IsInTick = false;
            }
        }
        public void Trigger(StoryInstance instance, object[] args)
        {
            Prepare();
            instance.StackVariables = StackVariables;
            m_IsTriggered = true;
            m_Arguments = args;
            if (null != m_ArgumentNames) {
                for (int i = 0; i < m_ArgumentNames.Length; ++i) {
                    if (i < args.Length)
                        instance.SetVariable(m_ArgumentNames[i], args[i]);
                    else
                        instance.SetVariable(m_ArgumentNames[i], null);
                }
            }
            m_Runtime.Iterator = null;
            m_Runtime.Arguments = m_Arguments;
        }
        private void RefreshCommands(Dsl.FunctionData handlerData)
        {
            m_LoadedCommands.Clear();
            for (int i = 0; i < handlerData.Statements.Count; i++) {
                IStoryCommand cmd = StoryCommandManager.Instance.CreateCommand(handlerData.Statements[i]);
                if (null != cmd) {
                    m_LoadedCommands.Add(cmd);
                }
            }
        }
        private string m_MessageId = "";
        private bool m_IsTriggered = false;
        private bool m_IsPaused = false;
        private bool m_IsInTick = false;
        private StoryRuntime m_Runtime = new StoryRuntime();
        private string[] m_ArgumentNames = null;
        private object[] m_Arguments = null;
        private StoryRuntimeStack m_RuntimeStack = new StoryRuntimeStack();
        private List<IStoryCommand> m_LoadedCommands = new List<IStoryCommand>();
        private StrObjDict m_StackVariables = new StrObjDict();
    }
    public sealed class StoryInstance
    {
        public string StoryId
        {
            get { return m_StoryId; }
            set { m_StoryId = value; }
        }
        public string Namespace
        {
            get { return m_Namespace; }
            set { m_Namespace = value; }
        }
        public bool IsDebug
        {
            get { return m_IsDebug; }
            set { m_IsDebug = value; }
        }
        public bool IsTerminated
        {
            get { return m_IsTerminated; }
            set { m_IsTerminated = value; }
        }
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set { m_IsPaused = value; }
        }
        public bool IsInTick
        {
            get { return m_IsInTick; }
        }
        public object Context
        {
            get { return m_Context; }
            set { m_Context = value; }
        }
        public StrObjDict LocalVariables
        {
            get { return m_LocalVariables; }
        }
        public StrObjDict GlobalVariables
        {
            get { return m_GlobalVariables; }
            set { m_GlobalVariables = value; }
        }
        public StrObjDict StackVariables
        {
            get { return m_StackVariables; }
            set { m_StackVariables = value; }
        }
        public void SetVariable(string varName, object varValue)
        {
            if (varName.StartsWith("$")) {
                if (null != m_StackVariables) {
                    if (m_StackVariables.ContainsKey(varName)) {
                        m_StackVariables[varName] = varValue;
                    } else {
                        m_StackVariables.Add(varName, varValue);
                    }
                }
            } else if (varName.StartsWith("@") && !varName.StartsWith("@@")) {
                if (m_LocalVariables.ContainsKey(varName)) {
                    m_LocalVariables[varName] = varValue;
                } else {
                    m_LocalVariables.Add(varName, varValue);
                }
            } else {
                if (null != m_GlobalVariables) {
                    if (m_GlobalVariables.ContainsKey(varName)) {
                        m_GlobalVariables[varName] = varValue;
                    } else {
                        m_GlobalVariables.Add(varName, varValue);
                    }
                }
            }
        }
        public bool TryGetVariable(string varName, out object val)
        {
            bool ret = false;
            val = null;
            if (varName.StartsWith("$")) {
                if (null != m_StackVariables) {
                    ret = m_StackVariables.TryGetValue(varName, out val);
                }
            } else if (varName.StartsWith("@") && !varName.StartsWith("@@")) {
                ret = m_LocalVariables.TryGetValue(varName, out val);
            } else {
                if (null != m_GlobalVariables) {
                    ret = m_GlobalVariables.TryGetValue(varName, out val);
                }
            }
            return ret;
        }
        public bool RemoveVariable(string varName)
        {
            bool ret = false;
            if (varName.StartsWith("$")) {
                if (null != m_StackVariables) {
                    ret = m_StackVariables.Remove(varName);
                }
            } else if (varName.StartsWith("@") && !varName.StartsWith("@@")) {
                ret = m_LocalVariables.Remove(varName);
            } else {
                if (null != m_GlobalVariables) {
                    ret = m_GlobalVariables.Remove(varName);
                }
            }
            return ret;
        }
        public StoryInstance Clone()
        {
            StoryInstance instance = new StoryInstance();
            foreach (var pair in m_PreInitedLocalVariables) {
                instance.m_PreInitedLocalVariables.Add(pair.Key, pair.Value);
            }
            foreach (var pair in m_LoadedMessageHandlers) {
                instance.m_LoadedMessageHandlers.Add(pair.Key, pair.Value);
            }
            for (int i = 0; i < m_MessageHandlers.Count; i++) {
                instance.m_MessageHandlers.Add(m_MessageHandlers[i].Clone());
                string msgId = m_MessageHandlers[i].MessageId;
                if (!instance.m_MessageQueues.ContainsKey(msgId)) {
                    instance.m_MessageQueues.Add(msgId, new Queue<MessageInfo>());
                }
                if (!instance.m_ConcurrentMessageQueues.ContainsKey(msgId)) {
                    instance.m_ConcurrentMessageQueues.Add(msgId, new Queue<MessageInfo>());
                }
                if (!instance.m_ConcurrentMessageHandlerPool.ContainsKey(msgId)) {
                    instance.m_ConcurrentMessageHandlerPool.Add(msgId, new Queue<StoryMessageHandler>());
                }
            }
            instance.m_StoryId = m_StoryId;
            instance.m_Namespace = m_Namespace;
            return instance;
        }
        public bool Init(Dsl.DslInfo config)
        {
            if (null == config || null == config.First)
                return false;
            bool ret = false;
            Dsl.FunctionData story = config.First;
            if (story.GetId() == "story" || story.GetId() == "script") {
                ret = true;
                Dsl.CallData callData = story.Call;
                if (null != callData && callData.HaveParam()) {
                    m_StoryId = callData.GetParamId(0);
                }
                for (int i = 0; i < story.Statements.Count; i++) {
                    if (story.Statements[i].GetId() == "local") {
                        Dsl.FunctionData sectionData = story.Statements[i] as Dsl.FunctionData;
                        if (null != sectionData) {
                            for (int j = 0; j < sectionData.Statements.Count; j++) {
                                Dsl.CallData defData = sectionData.Statements[j] as Dsl.CallData;
                                if (null != defData && defData.HaveId() && defData.HaveParam()) {
                                    string id = defData.GetId();
                                    if (id.StartsWith("@") && !id.StartsWith("@@")) {
                                        StoryValue val = new StoryValue();
                                        val.InitFromDsl(defData.GetParam(0));
                                        if (!m_PreInitedLocalVariables.ContainsKey(id)) {
                                            m_PreInitedLocalVariables.Add(id, val.Value);
                                        } else {
                                            m_PreInitedLocalVariables[id] = val.Value;
                                        }
                                    }
                                }
                            }
                        } else {
#if DEBUG
                            string err = string.Format("Story {0} DSL, local must be a function ! line:{1} local:{2}", m_StoryId, story.Statements[i].GetLine(), story.Statements[i].ToScriptString(false));
                            throw new Exception(err);
#else
                            LogSystem.Error("Story {0} DSL, local must be a function !", m_StoryId);
#endif
                        }
                    } else if (story.Statements[i].GetId() == "onmessage" || story.Statements[i].GetId() == "onnamespacedmessage") {
                        StoryMessageHandler handler = null;
                        Dsl.StatementData msgData = story.Statements[i] as Dsl.StatementData;
                        if (null != msgData) {
                            handler = new StoryMessageHandler();
                            handler.Load(msgData);
                        } else {
                            Dsl.FunctionData sectionData = story.Statements[i] as Dsl.FunctionData;
                            if (null != sectionData) {
                                handler = new StoryMessageHandler();
                                handler.Load(sectionData);
                            }
                        }
                        if (null != handler) {
                            string msgId;
                            if (!string.IsNullOrEmpty(m_Namespace) && story.Statements[i].GetId() == "onnamespacedmessage") {
                                msgId = string.Format("{0}:{1}", m_Namespace, handler.MessageId);
                                handler.MessageId = msgId;
                            } else {
                                msgId = handler.MessageId;
                            }
                            if (!m_LoadedMessageHandlers.ContainsKey(msgId)) {
                                m_LoadedMessageHandlers.Add(msgId, handler);
                                m_MessageHandlers.Add(handler.Clone());
                                m_MessageQueues.Add(msgId, new Queue<MessageInfo>());
                                m_ConcurrentMessageQueues.Add(msgId, new Queue<MessageInfo>());
                                m_ConcurrentMessageHandlerPool.Add(msgId, new Queue<StoryMessageHandler>());
                            } else {
#if DEBUG
                                string err = string.Format("Story {0} DSL, onmessage or onnamespacedmessage {1} duplicate, discard it ! line:{2}", m_StoryId, msgId, story.Statements[i].GetLine());
                                throw new Exception(err);
#else
                                LogSystem.Error("Story {0} DSL, onmessage {1} duplicate, discard it !", m_StoryId, msgId);
#endif
                            }
                        } else {
#if DEBUG
                            string err = string.Format("Story {0} DSL, onmessage must be a function or statement ! line:{1} onmessage:{2}", m_StoryId, story.Statements[i].GetLine(), story.Statements[i].ToScriptString(false));
                            throw new Exception(err);
#else
                            LogSystem.Error("Story {0} DSL, onmessage must be a function !", m_StoryId);
#endif
                        }
                    } else {
#if DEBUG
                        string err = string.Format("StoryInstance::Init, Story {0} unknown part {1}, line:{2} section:{3}", m_StoryId, story.Statements[i].GetId(), story.Statements[i].GetLine(), story.Statements[i].ToScriptString(false));
                        throw new Exception(err);
#else
                        LogSystem.Error("StoryInstance::Init, Story {0} unknown part {1}", m_StoryId, story.Statements[i].GetId());
#endif
                    }
                }
            } else {
#if DEBUG
                string err = string.Format("StoryInstance::Init, isn't story DSL, line:{0} story:{1}", story.GetLine(), story.ToScriptString(false));
                throw new Exception(err);
#else
                LogSystem.Error("StoryInstance::Init, isn't story DSL");
#endif
            }
            LogSystem.Debug("StoryInstance.Init message handler num:{0} {1}", m_MessageHandlers.Count, ret);
            return ret;
        }
        public void Reset()
        {
            m_IsTerminated = false;
            m_IsPaused = false;
            int ct = m_MessageHandlers.Count;
            for (int i = ct - 1; i >= 0; --i) {
                StoryMessageHandler handler = m_MessageHandlers[i];
                handler.Reset();
            }
            ct = m_ConcurrentMessageHandlers.Count;
            for (int i = ct - 1; i >= 0; --i) {
                StoryMessageHandler handler = m_ConcurrentMessageHandlers[i];
                RecycleConcurrentMessageHandler(handler);
            }
            foreach (KeyValuePair<string, Queue<MessageInfo>> pair in m_MessageQueues) {
                Queue<MessageInfo> queue = pair.Value;
                if (null != queue)
                    queue.Clear();
            }
            foreach (KeyValuePair<string, Queue<MessageInfo>> pair in m_ConcurrentMessageQueues) {
                Queue<MessageInfo> queue = pair.Value;
                if (null != queue)
                    queue.Clear();
            }
            m_ConcurrentMessageHandlers.Clear();
            m_LocalVariables.Clear();
            m_Message2TriggerTimes.Clear();
            m_MessageCount = 0;
            m_ConcurrentMessageCount = 0;
            m_TriggeredHandlerCount = 0;
        }
        public void Start()
        {
            m_LastTickTime = 0;
            m_CurTime = 0;
            try {
                foreach (KeyValuePair<string, object> pair in m_PreInitedLocalVariables) {
                    m_LocalVariables.Add(pair.Key, pair.Value);
                }
            } catch (Exception ex) {
                LogSystem.Error("Story {0} local variable duplicate! (for AI, @objid is a system predefined variable; for UI, @window is a system predefined variable), Exception:{1}\n{2}", m_StoryId, ex.Message, ex.StackTrace);
            }
            SendMessage("start");
        }
        public void SendMessage(string msgId, params object[] args)
        {
            MessageInfo msgInfo = new MessageInfo();
            msgInfo.m_MsgId = msgId;
            msgInfo.m_Args = args;
            Queue<MessageInfo> queue;
            if (m_MessageQueues.TryGetValue(msgId, out queue)) {
                if (msgId != "start" && msgId != "PlayerStart" && !msgId.StartsWith("common_")) {
                    LogSystem.Warn("StoryInstance queue message {0}", msgId);
                }
                queue.Enqueue(msgInfo);
                ++m_MessageCount;
            } else {
                //忽略没有处理的消息
                //LogSystem.Info("StoryInstance ignore message {0}", msgId);
            }
        }
        public void SendConcurrentMessage(string msgId, params object[] args)
        {
            MessageInfo msgInfo = new MessageInfo();
            msgInfo.m_MsgId = msgId;
            msgInfo.m_Args = args;
            Queue<MessageInfo> queue;
            if (m_ConcurrentMessageQueues.TryGetValue(msgId, out queue)) {
                if (msgId != "start" && msgId != "PlayerStart" && !msgId.StartsWith("common_")) {
                    LogSystem.Warn("StoryInstance queue concurrent message {0}", msgId);
                }
                queue.Enqueue(msgInfo);
                ++m_ConcurrentMessageCount;
            } else {
                //忽略没有处理的消息
                //LogSystem.Info("StoryInstance ignore concurrent message {0}", msgId);
            }
        }
        public int CountMessage(string msgId)
        {
            int ct = 0;
            Queue<MessageInfo> queue;
            if (m_MessageQueues.TryGetValue(msgId, out queue)) {
                ct = queue.Count;
                for (int i = 0; i < m_MessageHandlers.Count; ++i) {
                    StoryMessageHandler handler = m_MessageHandlers[i];
                    if (handler.IsTriggered && !handler.IsInTick && handler.MessageId == msgId) {
                        ++ct;
                        break;
                    }
                }
            }
            if (m_ConcurrentMessageQueues.TryGetValue(msgId, out queue)) {
                ct += queue.Count;
                for (int i = 0; i < m_ConcurrentMessageHandlers.Count; ++i) {
                    StoryMessageHandler handler = m_ConcurrentMessageHandlers[i];
                    if (handler.IsTriggered && !handler.IsInTick && handler.MessageId == msgId) {
                        ++ct;
                    }
                }
            }
            return ct;
        }
        public void ClearMessage(params string[] msgIds)
        {
            int len = msgIds.Length;
            if (len <= 0) {
                foreach (KeyValuePair<string, Queue<MessageInfo>> pair in m_MessageQueues) {
                    Queue<MessageInfo> queue = pair.Value;
                    if (null != queue)
                        queue.Clear();
                }
                foreach (KeyValuePair<string, Queue<MessageInfo>> pair in m_ConcurrentMessageQueues) {
                    Queue<MessageInfo> queue = pair.Value;
                    if (null != queue)
                        queue.Clear();
                }
                m_MessageCount = 0;
                m_ConcurrentMessageCount = 0;
            } else {
                for (int i = 0; i < len; ++i) {
                    string msgId = msgIds[i];
                    Queue<MessageInfo> queue;
                    if (m_MessageQueues.TryGetValue(msgId, out queue)) {
                        m_MessageCount -= queue.Count;
                        queue.Clear();
                    }
                    if (m_ConcurrentMessageQueues.TryGetValue(msgId, out queue)) {
                        m_ConcurrentMessageCount -= queue.Count;
                        queue.Clear();
                    }
                }
            }
        }
        public void PauseMessageHandler(string msgId, bool pause)
        {
            for (int i = 0; i < m_MessageHandlers.Count; ++i) {
                StoryMessageHandler handler = m_MessageHandlers[i];
                if (handler.IsTriggered && !handler.IsInTick && handler.MessageId == msgId) {
                    handler.IsPaused = pause;
                    break;
                }
            }
            for (int i = 0; i < m_ConcurrentMessageHandlers.Count; ++i) {
                StoryMessageHandler handler = m_ConcurrentMessageHandlers[i];
                if (handler.IsTriggered && !handler.IsInTick && handler.MessageId == msgId) {
                    handler.IsPaused = pause;
                    break;
                }
            }
        }
        public bool CanSleep()
        {
            return m_IsPaused || m_MessageCount + m_ConcurrentMessageCount + m_TriggeredHandlerCount <= 0;
        }
        public void Tick(long curTime)
        {
            if (m_IsPaused || m_MessageCount + m_ConcurrentMessageCount + m_TriggeredHandlerCount <= 0) {
                m_LastTickTime = curTime;
                return;
            }
            try {
                m_IsInTick = true;
                const int c_MaxMsgCountPerTick = 64;
                const int c_MaxConcurrentMsgCountPerTick = 16;
                long delta = 0;
                if (m_LastTickTime == 0) {
                    m_LastTickTime = curTime;
                } else {
                    delta = curTime - m_LastTickTime;
                    m_LastTickTime = curTime;
                    m_CurTime += delta;
                }
                int curTriggerdCount = 0;
                int ct = m_MessageHandlers.Count;
                for (int ix = ct - 1; ix >= 0; --ix) {
                    long dt = delta;
                    StoryMessageHandler handler = m_MessageHandlers[ix];
                    if (handler.IsTriggered) {
                        handler.Tick(this, dt);
                        dt = 0;
                    }
                    if (!handler.IsTriggered && m_MessageCount > 0) {
                        string msgId = handler.MessageId;
                        Queue<MessageInfo> queue;
                        if (m_MessageQueues.TryGetValue(msgId, out queue)) {
                            for (int msgCt = 0; msgCt < c_MaxMsgCountPerTick && queue.Count > 0 && !handler.IsTriggered; ++msgCt) {
                                MessageInfo info = queue.Dequeue();
                                --m_MessageCount;
                                UpdateMessageTriggerTime(info.m_MsgId, curTime);
                                handler.Trigger(this, info.m_Args);
                                handler.Tick(this, 0);
                            }
                        }
                    }
                    if (handler.IsTriggered) {
                        ++curTriggerdCount;
                    }
                }
                ct = m_ConcurrentMessageHandlers.Count;
                if (m_ConcurrentMessageCount > 0) {
                    foreach (var pair in m_ConcurrentMessageQueues) {
                        Queue<MessageInfo> queue = pair.Value;
                        for (int concurrentMsgCt = 0; concurrentMsgCt < c_MaxConcurrentMsgCountPerTick && queue.Count > 0; ++concurrentMsgCt) {
                            MessageInfo info = queue.Dequeue();
                            --m_ConcurrentMessageCount;
                            StoryMessageHandler handler = NewConcurrentMessageHandler(info.m_MsgId);
                            if (null != handler) {
                                UpdateMessageTriggerTime(info.m_MsgId, curTime);
                                handler.Trigger(this, info.m_Args);
                                handler.Tick(this, 0);
                                if (handler.IsTriggered) {
                                    m_ConcurrentMessageHandlers.Add(handler);
                                } else {
                                    RecycleConcurrentMessageHandler(handler);
                                }
                            }
                        }
                    }
                }
                for (int ix = ct - 1; ix >= 0; --ix) {
                    long dt = delta;
                    StoryMessageHandler handler = m_ConcurrentMessageHandlers[ix];
                    if (handler.IsTriggered) {
                        handler.Tick(this, dt);
                        dt = 0;
                    }
                    if (!handler.IsTriggered) {
                        m_ConcurrentMessageHandlers.RemoveAt(ix);
                        RecycleConcurrentMessageHandler(handler);
                    }
                }
                m_TriggeredHandlerCount = curTriggerdCount + m_ConcurrentMessageHandlers.Count;
            } finally {
                m_IsInTick = false;
            }
        }
        public StoryMessageHandler GetMessageHandler(string msgId)
        {
            StoryMessageHandler ret = null;
            for(int i=0;i<m_MessageHandlers.Count;++i) {
                var handler = m_MessageHandlers[i];
                if (handler.MessageId == msgId) {
                    ret = handler;
                    break;
                }
            }
            return ret;
        }
        public List<StoryMessageHandler> GetConcurrentMessageHandler(string msgId)
        {
            List<StoryMessageHandler> ret = new List<StoryMessageHandler>();
            for (int i = 0; i < m_ConcurrentMessageHandlers.Count; ++i) {
                var handler = m_ConcurrentMessageHandlers[i];
                if (handler.MessageId == msgId) {
                    ret.Add(handler);
                    break;
                }
            }
            return ret;
        }
        public long GetMessageTriggerTime(string msgId)
        {
            long time;
            m_Message2TriggerTimes.TryGetValue(msgId, out time);
            return time;
        }
        private void UpdateMessageTriggerTime(string msgId, long time)
        {
            if (m_Message2TriggerTimes.ContainsKey(msgId)) {
                m_Message2TriggerTimes[msgId] = time;
            } else {
                m_Message2TriggerTimes.Add(msgId, time);
            }
        }

        private StoryMessageHandler NewConcurrentMessageHandler(string msgId)
        {
            Queue<StoryMessageHandler> queue;
            if (m_ConcurrentMessageHandlerPool.TryGetValue(msgId, out queue)) {
                if (queue.Count > 0) {
                    StoryMessageHandler handler = queue.Dequeue();
                    return handler;
                }
            }
            return NewMessageHandler(msgId);
        }
        private void RecycleConcurrentMessageHandler(StoryMessageHandler handler)
        {
            string msgId = handler.MessageId;
            Queue<StoryMessageHandler> queue;
            if (m_ConcurrentMessageHandlerPool.TryGetValue(msgId, out queue)) {
                handler.Reset();
                queue.Enqueue(handler);
            }
        }
        private StoryMessageHandler NewMessageHandler(string msgId)
        {
            StoryMessageHandler handler;
            if (m_LoadedMessageHandlers.TryGetValue(msgId, out handler)) {
                handler = handler.Clone();
            }
            return handler;
        }

        private class MessageInfo
        {
            public string m_MsgId = null;
            public object[] m_Args = null;
        }
        private long m_CurTime = 0;
        private long m_LastTickTime = 0;
        private StrObjDict m_LocalVariables = new StrObjDict();
        private StrObjDict m_GlobalVariables = null;
        private StrObjDict m_StackVariables = null;
        private bool m_IsDebug = false;
        private string m_StoryId = string.Empty;
        private string m_Namespace = string.Empty;
        private bool m_IsTerminated = false;
        private bool m_IsPaused = false;
        private bool m_IsInTick = false;
        private object m_Context = null;
        private int m_MessageCount = 0;
        private int m_ConcurrentMessageCount = 0;
        private int m_TriggeredHandlerCount = 0;
        private Dictionary<string, Queue<MessageInfo>> m_MessageQueues = new Dictionary<string, Queue<MessageInfo>>();
        private List<StoryMessageHandler> m_MessageHandlers = new List<StoryMessageHandler>();
        private Dictionary<string, Queue<MessageInfo>> m_ConcurrentMessageQueues = new Dictionary<string, Queue<MessageInfo>>();
        private List<StoryMessageHandler> m_ConcurrentMessageHandlers = new List<StoryMessageHandler>();
        private Dictionary<string, long> m_Message2TriggerTimes = new Dictionary<string, long>();

        private Dictionary<string, Queue<StoryMessageHandler>> m_ConcurrentMessageHandlerPool = new Dictionary<string, Queue<StoryMessageHandler>>();
        private StrObjDict m_PreInitedLocalVariables = new StrObjDict();
        private Dictionary<string, StoryMessageHandler> m_LoadedMessageHandlers = new Dictionary<string, StoryMessageHandler>();
    }
}
