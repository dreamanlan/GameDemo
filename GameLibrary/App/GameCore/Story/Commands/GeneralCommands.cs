﻿using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using GameLibrary;

namespace GameLibrary.Story.Commands
{
    /// <summary>
    /// preload(objresid1,objresid2,...);
    /// </summary>
    internal class PreloadCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            PreloadCommand cmd = new PreloadCommand();
            for (int i = 0; i < m_DslFiles.Count; i++) {
                cmd.m_DslFiles.Add(m_DslFiles[i].Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_DslFiles.Count; i++) {
                m_DslFiles[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            List<string> dslFiles = new List<string>();
            for (int i = 0; i < m_DslFiles.Count; ++i) {
                var dslFile = m_DslFiles[i].Value;
                dslFiles.Add(dslFile);
            }
            ClientStorySystem.Instance.LoadSceneStories(dslFiles.ToArray());
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_DslFiles.Add(val);
            }
            return true;
        }
        private List<IStoryValue<string>> m_DslFiles = new List<IStoryValue<string>>();
    }
    /// <summary>
    /// startstory(story_id);
    /// </summary>
    internal class StartStoryCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            StartStoryCommand cmd = new StartStoryCommand();
            cmd.m_StoryId = m_StoryId.Clone();
            cmd.m_Multiple = m_Multiple.Clone();
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_StoryId.Evaluate(instance, handler, iterator, args);
            m_Multiple.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var storyId = m_StoryId.Value;
            var multiple = m_Multiple.Value;
            SceneSystem.Instance.QueueAction(() => {
                if (multiple == 0)
                    ClientStorySystem.Instance.StartStory(storyId);
                else
                    ClientStorySystem.Instance.StartStories(storyId);
            });
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_StoryId.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.GetFunctionNum() == 2) {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (!first.HaveStatement() && !second.HaveStatement()) {
                    Load(first);
                    var call = second;
                    if (call.GetId() == "multiple" && call.GetParamNum() > 0) {
                        m_Multiple.InitFromDsl(call.GetParam(0));
                    }
                }
            }
            return true;
        }
        private IStoryValue<string> m_StoryId = new StoryValue<string>();
        private IStoryValue<int> m_Multiple = new StoryValue<int>();
    }
    /// <summary>
    /// stopstory(story_id);
    /// </summary>
    internal class StopStoryCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            StopStoryCommand cmd = new StopStoryCommand();
            cmd.m_StoryId = m_StoryId.Clone();
            cmd.m_Multiple = m_Multiple.Clone();
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_StoryId.Evaluate(instance, handler, iterator, args);
            m_Multiple.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var multiple = m_Multiple.Value;
            if (multiple == 0)
                ClientStorySystem.Instance.MarkStoryTerminated(m_StoryId.Value);
            else
                ClientStorySystem.Instance.MarkStoriesTerminated(m_StoryId.Value);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_StoryId.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.GetFunctionNum() == 2) {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (!first.HaveStatement() && !second.HaveStatement()) {
                    Load(first);
                    var call = second;
                    if (call.GetId() == "multiple" && call.GetParamNum() > 0) {
                        m_Multiple.InitFromDsl(call.GetParam(0));
                    }
                }
            }
            return true;
        }
        private IStoryValue<string> m_StoryId = new StoryValue<string>();
        private IStoryValue<int> m_Multiple = new StoryValue<int>();
    }
    /// <summary>
    /// waitstory(storyid1,storyid2,...)[set(var,val)timeoutset(timeout,var,val)];
    /// </summary>
    internal class WaitStoryCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            WaitStoryCommand cmd = new WaitStoryCommand();
            for (int i = 0; i < m_StoryIds.Count; i++) {
                cmd.m_StoryIds.Add(m_StoryIds[i].Clone());
            }
            cmd.m_SetVar = m_SetVar.Clone();
            cmd.m_SetVal = m_SetVal.Clone();
            cmd.m_TimeoutVal = m_TimeoutVal.Clone();
            cmd.m_TimeoutSetVar = m_TimeoutSetVar.Clone();
            cmd.m_TimeoutSetVal = m_TimeoutSetVal.Clone();
            cmd.m_Multiple = m_Multiple.Clone();
            cmd.m_HaveSet = m_HaveSet;
            cmd.m_HaveMultiple = m_HaveMultiple;
            return cmd;
        }
        protected override void ResetState()
        {
            m_CurTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_StoryIds.Count; i++) {
                m_StoryIds[i].Evaluate(instance, handler, iterator, args);
            }
            if (m_HaveSet) {
                m_SetVar.Evaluate(instance, handler, iterator, args);
                m_SetVal.Evaluate(instance, handler, iterator, args);
                m_TimeoutVal.Evaluate(instance, handler, iterator, args);
                m_TimeoutSetVar.Evaluate(instance, handler, iterator, args);
                m_TimeoutSetVal.Evaluate(instance, handler, iterator, args);
            }
            if (m_HaveMultiple) {
                m_Multiple.Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int ct = 0;
            for (int i = 0; i < m_StoryIds.Count; i++) {
                ct += ClientStorySystem.Instance.CountStory(m_StoryIds[i].Value);
            }
            bool ret = false;
            int multiple = m_Multiple.Value;
            if (ct <= 0) {
                if (m_HaveSet) {
                    string varName = m_SetVar.Value;
                    var varVal = m_SetVal.Value;
                    instance.SetVariable(varName, varVal);
                }
            } else {
                int timeout = m_TimeoutVal.Value;
                int curTime = m_CurTime;
                m_CurTime += (int)delta;
                if (timeout <= 0 || curTime <= timeout) {
                    ret = true;
                } else if (m_HaveSet) {
                    string varName = m_TimeoutSetVar.Value;
                    var varVal = m_TimeoutSetVal.Value;
                    instance.SetVariable(varName, varVal);
                }
            }
            return ret;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_StoryIds.Add(val);
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            int ct = statementData.Functions.Count;
            if (statementData.Functions.Count >= 2) {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (ct == 2) {
                    m_HaveMultiple = true;
                    LoadMultiple(second);
                } else if (ct == 3) {
                    var third = statementData.Third.AsFunction;
                    if (null != first && null != second && null != third) {
                        m_HaveSet = true;
                        Load(first);
                        LoadSet(second);
                        LoadTimeoutSet(third);
                    }
                } else if (ct == 4) {
                    var third = statementData.Third.AsFunction;
                    var last = statementData.Last.AsFunction;
                    if (null != first && null != second && null != third && null != last) {
                        m_HaveSet = true;
                        Load(first);
                        LoadSet(second);
                        LoadTimeoutSet(third);
                        m_HaveMultiple = true;
                        LoadMultiple(last);
                    }
                }
            }
            return true;
        }
        private void LoadMultiple(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 1 && callData.GetId() == "multiple") {
                m_Multiple.InitFromDsl(callData.GetParam(0));
            }
        }
        private void LoadSet(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2 && callData.GetId() == "set") {
                m_SetVar.InitFromDsl(callData.GetParam(0));
                m_SetVal.InitFromDsl(callData.GetParam(1));
            }
        }
        private void LoadTimeoutSet(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 3 && callData.GetId() == "timeoutset") {
                m_TimeoutVal.InitFromDsl(callData.GetParam(0));
                m_TimeoutSetVar.InitFromDsl(callData.GetParam(1));
                m_TimeoutSetVal.InitFromDsl(callData.GetParam(2));
            }
        }
        private List<IStoryValue<string>> m_StoryIds = new List<IStoryValue<string>>();
        private IStoryValue<string> m_SetVar = new StoryValue<string>();
        private IStoryValue m_SetVal = new StoryValue();
        private IStoryValue<int> m_TimeoutVal = new StoryValue<int>();
        private IStoryValue<string> m_TimeoutSetVar = new StoryValue<string>();
        private IStoryValue m_TimeoutSetVal = new StoryValue();
        private IStoryValue<int> m_Multiple = new StoryValue<int>();
        private bool m_HaveMultiple = false;
        private bool m_HaveSet = false;
        private int m_CurTime = 0;
    }
    /// <summary>
    /// pausestory(storyid1,storyid2,...);
    /// </summary>
    internal class PauseStoryCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            PauseStoryCommand cmd = new PauseStoryCommand();
            for (int i = 0; i < m_StoryIds.Count; i++) {
                cmd.m_StoryIds.Add(m_StoryIds[i].Clone());
            }
            cmd.m_Multiple = m_Multiple.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_StoryIds.Count; i++) {
                m_StoryIds[i].Evaluate(instance, handler, iterator, args);
            }
            m_Multiple.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var multiple = m_Multiple.Value;
            if (multiple == 0) {
                for (int i = 0; i < m_StoryIds.Count; i++) {
                    ClientStorySystem.Instance.PauseStory(m_StoryIds[i].Value, true);
                }
            } else {
                for (int i = 0; i < m_StoryIds.Count; i++) {
                    ClientStorySystem.Instance.PauseStories(m_StoryIds[i].Value, true);
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_StoryIds.Add(val);
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.GetFunctionNum() == 2) {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (!first.HaveStatement() && !second.HaveStatement()) {
                    Load(first);
                    var call = second;
                    if (call.GetId() == "multiple" && call.GetParamNum() > 0) {
                        m_Multiple.InitFromDsl(call.GetParam(0));
                    }
                }
            }
            return true;
        }
        private List<IStoryValue<string>> m_StoryIds = new List<IStoryValue<string>>();
        private IStoryValue<int> m_Multiple = new StoryValue<int>();
    }
    /// <summary>
    /// resumestory(storyid1,storyid2,...);
    /// </summary>
    internal class ResumeStoryCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ResumeStoryCommand cmd = new ResumeStoryCommand();
            for (int i = 0; i < m_StoryIds.Count; i++) {
                cmd.m_StoryIds.Add(m_StoryIds[i].Clone());
            }
            cmd.m_Multiple = m_Multiple.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_StoryIds.Count; i++) {
                m_StoryIds[i].Evaluate(instance, handler, iterator, args);
            }
            m_Multiple.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var multiple = m_Multiple.Value;
            if (multiple == 0) {
                for (int i = 0; i < m_StoryIds.Count; i++) {
                    ClientStorySystem.Instance.PauseStory(m_StoryIds[i].Value, false);
                }
            } else {
                for (int i = 0; i < m_StoryIds.Count; i++) {
                    ClientStorySystem.Instance.PauseStories(m_StoryIds[i].Value, false);
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_StoryIds.Add(val);
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.GetFunctionNum() == 2) {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (!first.HaveStatement() && !second.HaveStatement()) {
                    Load(first);
                    var call = second;
                    if (call.GetId() == "multiple" && call.GetParamNum() > 0) {
                        m_Multiple.InitFromDsl(call.GetParam(0));
                    }
                }
            }
            return true;
        }
        private List<IStoryValue<string>> m_StoryIds = new List<IStoryValue<string>>();
        private IStoryValue<int> m_Multiple = new StoryValue<int>();
    }
    /// <summary>
    /// firemessage(msgid,arg1,arg2,...);
    /// </summary>
    internal class FireMessageCommand : AbstractStoryCommand
    {
        public FireMessageCommand(bool isConcurrent)
        {
            m_IsConcurrent = isConcurrent;
        }
        protected override IStoryCommand CloneCommand()
        {
            FireMessageCommand cmd = new FireMessageCommand(m_IsConcurrent);
            cmd.m_MsgId = m_MsgId.Clone();
            for (int i = 0; i < m_MsgArgs.Count; ++i) {
                IStoryValue val = m_MsgArgs[i];
                cmd.m_MsgArgs.Add(val.Clone());
            }
            return cmd;
        }

        protected override void ResetState()
        { }

        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_MsgId.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_MsgArgs.Count; ++i) {
                IStoryValue val = m_MsgArgs[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string msgId = m_MsgId.Value;
            BoxedValueList args = ClientStorySystem.Instance.NewBoxedValueList();
            for (int i = 0; i < m_MsgArgs.Count; ++i) {
                IStoryValue val = m_MsgArgs[i];
                args.Add(val.Value);
            }
            if(m_IsConcurrent)
                ClientStorySystem.Instance.SendConcurrentMessage(msgId, args);
            else
                ClientStorySystem.Instance.SendMessage(msgId, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_MsgId.InitFromDsl(callData.GetParam(0));
            }
            for (int i = 1; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_MsgArgs.Add(val);
            }
            return true;
        }

        private IStoryValue<string> m_MsgId = new StoryValue<string>();
        private List<IStoryValue> m_MsgArgs = new List<IStoryValue>();
        private bool m_IsConcurrent = false;
    }
    internal sealed class FireMessageCommandFactory : IStoryCommandFactory
    {
        public IStoryCommand Create()
        {
            return new FireMessageCommand(false);
        }
    }
    internal sealed class FireConcurrentMessageCommandFactory : IStoryCommandFactory
    {
        public IStoryCommand Create()
        {
            return new FireMessageCommand(true);
        }
    }
    /// <summary>
    /// waitallmessage(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];
    /// </summary>
    internal class WaitAllMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            WaitAllMessageCommand cmd = new WaitAllMessageCommand();
            for (int i = 0; i < m_MsgIds.Count; i++) {
                cmd.m_MsgIds.Add(m_MsgIds[i].Clone());
            }
            cmd.m_SetVar = m_SetVar.Clone();
            cmd.m_SetVal = m_SetVal.Clone();
            cmd.m_TimeoutVal = m_TimeoutVal.Clone();
            cmd.m_TimeoutSetVar = m_TimeoutSetVar.Clone();
            cmd.m_TimeoutSetVal = m_TimeoutSetVal.Clone();
            cmd.m_HaveSet = m_HaveSet;
            return cmd;
        }
        protected override void ResetState()
        {
            m_CurTime = 0;
            m_StartTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_MsgIds.Count; i++) {
                m_MsgIds[i].Evaluate(instance, handler, iterator, args);
            }
            if (m_HaveSet) {
                m_SetVar.Evaluate(instance, handler, iterator, args);
                m_SetVal.Evaluate(instance, handler, iterator, args);
                m_TimeoutVal.Evaluate(instance, handler, iterator, args);
                m_TimeoutSetVar.Evaluate(instance, handler, iterator, args);
                m_TimeoutSetVal.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_StartTime <= 0) {
                long startTime = GameLibrary.TimeUtility.GetLocalMilliseconds();
                m_StartTime = startTime;
            }
            bool triggered = false;
            for (int i = 0; i < m_MsgIds.Count; i++) {
                long time = instance.GetMessageTriggerTime(m_MsgIds[i].Value);
                if (time > m_StartTime) {
                    triggered = true;
                    break;
                }
            }
            bool ret = false;
            if (triggered) {
                string varName = m_SetVar.Value;
                var varVal = m_SetVal.Value;
                instance.SetVariable(varName, varVal);
            } else {
                int timeout = m_TimeoutVal.Value;
                int curTime = m_CurTime;
                m_CurTime += (int)delta;
                if (timeout <= 0 || curTime <= timeout) {
                    ret = true;
                } else {
                    string varName = m_TimeoutSetVar.Value;
                    var varVal = m_TimeoutSetVal.Value;
                    instance.SetVariable(varName, varVal);
                }
            }
            return ret;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_MsgIds.Add(val);
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count >= 3) {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                Dsl.FunctionData third = statementData.Third.AsFunction;
                if (null != first && null != second && null != third) {
                    m_HaveSet = true;
                    Load(first);
                    LoadSet(second);
                    LoadTimeoutSet(third);
                }
            }
            return true;
        }
        private void LoadSet(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2) {
                m_SetVar.InitFromDsl(callData.GetParam(0));
                m_SetVal.InitFromDsl(callData.GetParam(1));
            }
        }
        private void LoadTimeoutSet(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 3) {
                m_TimeoutVal.InitFromDsl(callData.GetParam(0));
                m_TimeoutSetVar.InitFromDsl(callData.GetParam(1));
                m_TimeoutSetVal.InitFromDsl(callData.GetParam(2));
            }
        }
        private List<IStoryValue<string>> m_MsgIds = new List<IStoryValue<string>>();
        private IStoryValue<string> m_SetVar = new StoryValue<string>();
        private IStoryValue m_SetVal = new StoryValue();
        private IStoryValue<int> m_TimeoutVal = new StoryValue<int>();
        private IStoryValue<string> m_TimeoutSetVar = new StoryValue<string>();
        private IStoryValue m_TimeoutSetVal = new StoryValue();
        private bool m_HaveSet = false;
        private int m_CurTime = 0;
        private long m_StartTime = 0;
    }
    /// <summary>
    /// waitallmessagehandler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];
    /// </summary>
    internal class WaitAllMessageHandlerCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            WaitAllMessageHandlerCommand cmd = new WaitAllMessageHandlerCommand();
            for (int i = 0; i < m_MsgIds.Count; i++) {
                cmd.m_MsgIds.Add(m_MsgIds[i].Clone());
            }
            cmd.m_SetVar = m_SetVar.Clone();
            cmd.m_SetVal = m_SetVal.Clone();
            cmd.m_TimeoutVal = m_TimeoutVal.Clone();
            cmd.m_TimeoutSetVar = m_TimeoutSetVar.Clone();
            cmd.m_TimeoutSetVal = m_TimeoutSetVal.Clone();
            cmd.m_HaveSet = m_HaveSet;
            return cmd;
        }
        protected override void ResetState()
        {
            m_CurTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_MsgIds.Count; i++) {
                m_MsgIds[i].Evaluate(instance, handler, iterator, args);
            }
            if (m_HaveSet) {
                m_SetVar.Evaluate(instance, handler, iterator, args);
                m_SetVal.Evaluate(instance, handler, iterator, args);
                m_TimeoutVal.Evaluate(instance, handler, iterator, args);
                m_TimeoutSetVar.Evaluate(instance, handler, iterator, args);
                m_TimeoutSetVal.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int ct = 0;
            for (int i = 0; i < m_MsgIds.Count; i++) {
                ct += ClientStorySystem.Instance.CountMessage(m_MsgIds[i].Value);
            }
            bool ret = false;
            if (ct <= 0) {
                string varName = m_SetVar.Value;
                var varVal = m_SetVal.Value;
                instance.SetVariable(varName, varVal);
            } else {
                int timeout = m_TimeoutVal.Value;
                int curTime = m_CurTime;
                m_CurTime += (int)delta;
                if (timeout <= 0 || curTime <= timeout) {
                    ret = true;
                } else {
                    string varName = m_TimeoutSetVar.Value;
                    var varVal = m_TimeoutSetVal.Value;
                    instance.SetVariable(varName, varVal);
                }
            }
            return ret;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_MsgIds.Add(val);
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count >= 3) {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                Dsl.FunctionData third = statementData.Third.AsFunction;
                if (null != first && null != second && null != third) {
                    m_HaveSet = true;
                    Load(first);
                    LoadSet(second);
                    LoadTimeoutSet(third);
                }
            }
            return true;
        }
        private void LoadSet(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2) {
                m_SetVar.InitFromDsl(callData.GetParam(0));
                m_SetVal.InitFromDsl(callData.GetParam(1));
            }
        }
        private void LoadTimeoutSet(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 3) {
                m_TimeoutVal.InitFromDsl(callData.GetParam(0));
                m_TimeoutSetVar.InitFromDsl(callData.GetParam(1));
                m_TimeoutSetVal.InitFromDsl(callData.GetParam(2));
            }
        }
        private List<IStoryValue<string>> m_MsgIds = new List<IStoryValue<string>>();
        private IStoryValue<string> m_SetVar = new StoryValue<string>();
        private IStoryValue m_SetVal = new StoryValue();
        private IStoryValue<int> m_TimeoutVal = new StoryValue<int>();
        private IStoryValue<string> m_TimeoutSetVar = new StoryValue<string>();
        private IStoryValue m_TimeoutSetVal = new StoryValue();
        private bool m_HaveSet = false;
        private int m_CurTime = 0;
    }
    /// <summary>
    /// suspendallmessagehandler(msgid1,msgid2,...);
    /// </summary>
    internal class SuspendAllMessageHandlerCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SuspendAllMessageHandlerCommand cmd = new SuspendAllMessageHandlerCommand();
            for (int i = 0; i < m_MsgIds.Count; i++) {
                cmd.m_MsgIds.Add(m_MsgIds[i].Clone());
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_MsgIds.Count; i++) {
                m_MsgIds[i].Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            for (int i = 0; i < m_MsgIds.Count; i++) {
                ClientStorySystem.Instance.SuspendMessageHandler(m_MsgIds[i].Value, true);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_MsgIds.Add(val);
            }
            return true;
        }
        private List<IStoryValue<string>> m_MsgIds = new List<IStoryValue<string>>();
    }
    /// <summary>
    /// resumeallmessagehandler(msgid1,msgid2,...);
    /// </summary>
    internal class ResumeAllMessageHandlerCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ResumeAllMessageHandlerCommand cmd = new ResumeAllMessageHandlerCommand();
            for (int i = 0; i < m_MsgIds.Count; i++) {
                cmd.m_MsgIds.Add(m_MsgIds[i].Clone());
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_MsgIds.Count; i++) {
                m_MsgIds[i].Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            for (int i = 0; i < m_MsgIds.Count; i++) {
                ClientStorySystem.Instance.SuspendMessageHandler(m_MsgIds[i].Value, false);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> val = new StoryValue<string>();
                val.InitFromDsl(callData.GetParam(i));
                m_MsgIds.Add(val);
            }
            return true;
        }
        private List<IStoryValue<string>> m_MsgIds = new List<IStoryValue<string>>();
    }
    /// <summary>
    /// sendaimessage(objid,msg,arg1,arg2,...);
    /// </summary>
    internal class SendAiMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendAiMessageCommand cmd = new SendAiMessageCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            string msg = m_Msg.Value;
            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity) {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 0; i < m_Args.Count; ++i) {
                    IStoryValue val = m_Args[i];
                    args.Add(val.Value);
                }
                entity.GetAiStateInfo().SendMessage(msg, args);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// sendaiconcurrentmessage(objid,msg,arg1,arg2,...);
    /// </summary>
    internal class SendAiConcurrentMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendAiConcurrentMessageCommand cmd = new SendAiConcurrentMessageCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            string msg = m_Msg.Value;
            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity) {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 0; i < m_Args.Count; ++i) {
                    IStoryValue val = m_Args[i];
                    args.Add(val.Value);
                }
                entity.GetAiStateInfo().SendConcurrentMessage(msg, args);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// sendainamespacedmessage(objid,msg,arg1,arg2,...);
    /// </summary>
    internal class SendAiNamespacedMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendAiNamespacedMessageCommand cmd = new SendAiNamespacedMessageCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            string msg = m_Msg.Value;
            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity) {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 0; i < m_Args.Count; ++i) {
                    IStoryValue val = m_Args[i];
                    args.Add(val.Value);
                }
                entity.GetAiStateInfo().SendNamespacedMessage(msg, args);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// sendaiconcurrentnamespacedmessage(objid,msg,arg1,arg2,...);
    /// </summary>
    internal class SendAiConcurrentNamespacedMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendAiConcurrentNamespacedMessageCommand cmd = new SendAiConcurrentNamespacedMessageCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            string msg = m_Msg.Value;
            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity) {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 0; i < m_Args.Count; ++i) {
                    IStoryValue val = m_Args[i];
                    args.Add(val.Value);
                }
                entity.GetAiStateInfo().SendConcurrentNamespacedMessage(msg, args);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// publishevent(ev_name,group,arg1,arg2,...);
    /// </summary>
    internal class PublishEventCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            PublishEventCommand cmd = new PublishEventCommand();
            cmd.m_EventName = m_EventName.Clone();
            cmd.m_Group = m_Group.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_EventName.Evaluate(instance, handler, iterator, args);
            m_Group.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string evname = m_EventName.Value;
            string group = m_Group.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            Utility.EventSystem.Publish(evname, group, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_EventName.InitFromDsl(callData.GetParam(0));
                m_Group.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue<string> m_EventName = new StoryValue<string>();
        private IStoryValue<string> m_Group = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// sendmessage(objname,msg,arg1,arg2,...);
    /// </summary>
    internal class SendMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendMessageCommand cmd = new SendMessageCommand();
            cmd.m_ObjName = m_ObjName.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjName.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string objname = m_ObjName.Value;
            string msg = m_Msg.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (args.Length == 0)
                Utility.SendMessage(objname, msg, null);
            else if (args.Length == 1)
                Utility.SendMessage(objname, msg, args[0]);
            else
                Utility.SendMessage(objname, msg, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjName.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue<string> m_ObjName = new StoryValue<string>();
        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// sendmessagewithtag(tagname,msg,arg1,arg2,...);
    /// </summary>
    internal class SendMessageWithTagCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendMessageWithTagCommand cmd = new SendMessageWithTagCommand();
            cmd.m_ObjTag = m_ObjTag.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjTag.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string objtag = m_ObjTag.Value;
            string msg = m_Msg.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (args.Length == 0)
                Utility.SendMessageWithTag(objtag, msg, null);
            else if (args.Length == 1)
                Utility.SendMessageWithTag(objtag, msg, args[0]);
            else
                Utility.SendMessageWithTag(objtag, msg, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjTag.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue<string> m_ObjTag = new StoryValue<string>();
        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// sendmessagewithgameobject(gameobject,msg,arg1,arg2,...);
    /// </summary>
    internal class SendMessageWithGameObjectCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendMessageWithGameObjectCommand cmd = new SendMessageWithGameObjectCommand();
            cmd.m_Object = m_Object.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_Object.Value;
            UnityEngine.GameObject uobj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
            if (null == uobj) {
                try {
                    int objId = objVal.IsInteger ? objVal.GetInt() : -1;
                    uobj = SceneSystem.Instance.GetGameObject(objId);
                } catch {
                    uobj = null;
                }
            }
            if (null != uobj) {
                string msg = m_Msg.Value;
                ArrayList arglist = new ArrayList();
                for (int i = 0; i < m_Args.Count; ++i) {
                    IStoryValue val = m_Args[i];
                    arglist.Add(val.Value.GetObject());
                }
                object[] args = arglist.ToArray();
                if (args.Length == 0)
                    uobj.SendMessage(msg, UnityEngine.SendMessageOptions.DontRequireReceiver);
                else if (args.Length == 1)
                    uobj.SendMessage(msg, args[0], UnityEngine.SendMessageOptions.DontRequireReceiver);
                else
                    uobj.SendMessage(msg, args, UnityEngine.SendMessageOptions.DontRequireReceiver);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryValue m_Object = new StoryValue();
        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// sendscriptmessage(msg,arg1,arg2,...);
    /// </summary>
    internal class SendScriptMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendScriptMessageCommand cmd = new SendScriptMessageCommand();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string msg = m_Msg.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (args.Length == 0)
                Utility.SendScriptMessage(msg, null);
            else if (args.Length == 1)
                Utility.SendScriptMessage(msg, args[0]);
            else
                Utility.SendScriptMessage(msg, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Msg.InitFromDsl(callData.GetParam(0));
            }
            for (int i = 1; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }

        private IStoryValue<string> m_Msg = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// creategameobject(name, prefab[, parent])[obj("varname")]{
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    ///     loadtimeout(1000);
    ///     disable("typename", "typename", ...);
    ///     remove("typename", "typename", ...);
    /// };
    /// </summary>
    internal class CreateGameObjectCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            CreateGameObjectCommand cmd = new CreateGameObjectCommand();
            cmd.m_Name = m_Name.Clone();
            cmd.m_Prefab = m_Prefab.Clone();
            cmd.m_HaveParent = m_HaveParent;
            cmd.m_Parent = m_Parent.Clone();
            cmd.m_HaveObj = m_HaveObj;
            cmd.m_ObjVarName = m_ObjVarName.Clone();
            cmd.m_Position = m_Position.Clone();
            cmd.m_Rotation = m_Rotation.Clone();
            cmd.m_Scale = m_Scale.Clone();
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                cmd.m_DisableComponents.Add(m_DisableComponents[i].Clone());
            }
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                cmd.m_RemoveComponents.Add(m_RemoveComponents[i].Clone());
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Name.Evaluate(instance, handler, iterator, args);
            m_Prefab.Evaluate(instance, handler, iterator, args);
            if (m_HaveParent) {
                m_Parent.Evaluate(instance, handler, iterator, args);
            }
            if (m_HaveObj) {
                m_ObjVarName.Evaluate(instance, handler, iterator, args);
            }
            m_Position.Evaluate(instance, handler, iterator, args);
            m_Rotation.Evaluate(instance, handler, iterator, args);
            m_Scale.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                m_DisableComponents[i].Evaluate(instance, handler, iterator, args);
            }
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                m_RemoveComponents[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string name = m_Name.Value;
            string prefab = m_Prefab.Value;
            List<string> disables = new List<string>();
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                disables.Add(m_DisableComponents[i].Value);
            }
            List<string> removes = new List<string>();
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                removes.Add(m_RemoveComponents[i].Value);
            }
            var o = ResourceSystem.Instance.NewObject(prefab);
            AfterLoad(instance, name, o, disables, removes);

            return false;
        }
        private void AfterLoad(StoryInstance instance, string name, UnityEngine.Object o, List<string> disables, List<string> removes)
        {
            var obj = o as UnityEngine.GameObject;
            if (null != obj) {
                foreach (string disable in disables) {
                    var type = Utility.GetType(disable);
                    if (null != type) {
                        var comps = obj.GetComponentsInChildren(type);
                        for (int i = 0; i < comps.Length; ++i) {
                            var t = comps[i].GetType();
                            t.InvokeMember("enabled", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, comps[i], new object[] { false });
                        }
                    }
                }
                foreach (string remove in removes) {
                    var type = Utility.GetType(remove);
                    if (null != type) {
                        var comps = obj.GetComponentsInChildren(type);
                        for (int i = 0; i < comps.Length; ++i) {
                            Utility.DestroyObject(comps[i]);
                        }
                    }
                }

                obj.name = name;
                if (m_HaveParent) {
                    var parentVal = m_Parent.Value;
                    string path = parentVal.IsString ? parentVal.StringVal : null;
                    if (null != path) {
                        var pobj = UnityEngine.GameObject.Find(path);
                        if (null != pobj) {
                            obj.transform.SetParent(pobj.transform, false);
                        }
                    } else {
                        UnityEngine.GameObject pobj = parentVal.IsObject ? parentVal.ObjectVal as UnityEngine.GameObject : null;
                        if (null != pobj) {
                            obj.transform.SetParent(pobj.transform, false);
                        }
                    }
                }
                if (m_Position.HaveValue) {
                    var v = m_Position.Value;
                    obj.transform.localPosition = new UnityEngine.Vector3(v.x, v.y, v.z);
                }
                if (m_Rotation.HaveValue) {
                    var v = m_Rotation.Value;
                    obj.transform.localEulerAngles = new UnityEngine.Vector3(v.x, v.y, v.z);
                }
                if (m_Scale.HaveValue) {
                    var v = m_Scale.Value;
                    obj.transform.localScale = new UnityEngine.Vector3(v.x, v.y, v.z);
                }
                if (m_HaveObj) {
                    string varName = m_ObjVarName.Value;
                    instance.SetVariable(varName, obj);
                }
            }
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                var callData = funcData.LowerOrderFunction;
                LoadCall(callData);
            }
            else if(funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                foreach (var comp in funcData.Params) {
                    var cd = comp as Dsl.FunctionData;
                    if (null != cd) {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                if (null != first && null != second) {
                    Load(first);
                    if (second.IsHighOrder) {
                        LoadVarName(second.LowerOrderFunction);
                    }
                    else if (second.HaveParam()) {
                        LoadVarName(second);
                    }
                }
                if (null != second && second.HaveStatement()) {
                    foreach (var comp in second.Params) {
                        var cd = comp as Dsl.FunctionData;
                        if (null != cd) {
                            LoadOptional(cd);
                        }
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Name.InitFromDsl(callData.GetParam(0));
                m_Prefab.InitFromDsl(callData.GetParam(1));
                if (num > 2) {
                    m_HaveParent = true;
                    m_Parent.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        private void LoadVarName(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "obj" && callData.GetParamNum() == 1) {
                m_ObjVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObj = true;
            }
        }
        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            if (id == "position") {
                m_Position.InitFromDsl(callData.GetParam(0));
            } else if (id == "rotation") {
                m_Rotation.InitFromDsl(callData.GetParam(0));
            } else if (id == "scale") {
                m_Scale.InitFromDsl(callData.GetParam(0));
            } else if (id == "disable") {
                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    var p = new StoryValue<string>();
                    p.InitFromDsl(callData.GetParam(i));
                    m_DisableComponents.Add(p);
                }
            } else if (id == "remove") {
                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    var p = new StoryValue<string>();
                    p.InitFromDsl(callData.GetParam(i));
                    m_RemoveComponents.Add(p);
                }
            }
        }

        private IStoryValue<string> m_Name = new StoryValue<string>();
        private IStoryValue<string> m_Prefab = new StoryValue<string>();
        private IStoryValue m_Parent = new StoryValue();
        private bool m_HaveParent = false;
        private bool m_HaveObj = false;
        private IStoryValue<string> m_ObjVarName = new StoryValue<string>();
        private IStoryValue<UnityEngine.Vector3> m_Position = new StoryValue<UnityEngine.Vector3>();
        private IStoryValue<UnityEngine.Vector3> m_Rotation = new StoryValue<UnityEngine.Vector3>();
        private IStoryValue<UnityEngine.Vector3> m_Scale = new StoryValue<UnityEngine.Vector3>();
        private List<IStoryValue<string>> m_DisableComponents = new List<IStoryValue<string>>();
        private List<IStoryValue<string>> m_RemoveComponents = new List<IStoryValue<string>>();
    }
    /// <summary>
    /// settransform(name, world_or_local){
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    /// };
    /// </summary>
    internal class SetTransformCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetTransformCommand cmd = new SetTransformCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_LocalOrWorld = m_LocalOrWorld.Clone();
            cmd.m_Position = m_Position.Clone();
            cmd.m_Rotation = m_Rotation.Clone();
            cmd.m_Scale = m_Scale.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);
            m_Position.Evaluate(instance, handler, iterator, args);
            m_Rotation.Evaluate(instance, handler, iterator, args);
            m_Scale.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int worldOrLocal = m_LocalOrWorld.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    m_Object = obj;
                    var entityInfo = SceneSystem.Instance.GetEntityByGameObject(obj);
                    if (m_Position.HaveValue) {
                        var v = m_Position.Value;
                        if (0 == worldOrLocal) {
                            obj.transform.localPosition = new UnityEngine.Vector3(v.x, v.y, v.z);
                        }
                        else {
                            if (null != entityInfo) {
                                entityInfo.GetMovementStateInfo().SetPosition(new UnityEngine.Vector3(v.x, v.y, v.z));
                            }
                            else {
                                obj.transform.position = new UnityEngine.Vector3(v.x, v.y, v.z);
                            }
                        }
                    }
                    if (m_Rotation.HaveValue) {
                        var v = m_Rotation.Value;
                        if (0 == worldOrLocal)
                            obj.transform.localEulerAngles = new UnityEngine.Vector3(v.x, v.y, v.z);
                        else
                            obj.transform.eulerAngles = new UnityEngine.Vector3(v.x, v.y, v.z);
                    }
                    if (m_Scale.HaveValue) {
                        var v = m_Scale.Value;
                        obj.transform.localScale = new UnityEngine.Vector3(v.x, v.y, v.z);
                    }
                    return true;
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                var callData = funcData.LowerOrderFunction;
                LoadCall(callData);
            }
            else if(funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                foreach (var comp in funcData.Params) {
                    var cd = comp as Dsl.FunctionData;
                    if (null != cd) {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            if (id == "position") {
                m_Position.InitFromDsl(callData.GetParam(0));
            } else if (id == "rotation") {
                m_Rotation.InitFromDsl(callData.GetParam(0));
            } else if (id == "scale") {
                m_Scale.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private IStoryValue<UnityEngine.Vector3> m_Position = new StoryValue<UnityEngine.Vector3>();
        private IStoryValue<UnityEngine.Vector3> m_Rotation = new StoryValue<UnityEngine.Vector3>();
        private IStoryValue<UnityEngine.Vector3> m_Scale = new StoryValue<UnityEngine.Vector3>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
    }
    /// <summary>
    /// destroygameobject(path);
    /// </summary>
    internal class DestroyGameObjectCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            DestroyGameObjectCommand cmd = new DestroyGameObjectCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var pathVal = m_ObjPath.Value;
            string path = pathVal.IsString ? pathVal.StringVal : null;
            if (null != path) {
                var obj = UnityEngine.GameObject.Find(path);
                if (null != obj) {
                    obj.transform.SetParent(null);
                    if (!ResourceSystem.Instance.RecycleObject(obj)) {
                        SceneSystem.Instance.GameObjectsFromDsl.Remove(obj);
                        Utility.DestroyObject(obj);
                    }
                }
            } else {
                var obj = pathVal.IsObject ? pathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null != obj) {
                    obj.transform.SetParent(null);
                    if (!ResourceSystem.Instance.RecycleObject(obj)) {
                        SceneSystem.Instance.GameObjectsFromDsl.Remove(obj);
                        Utility.DestroyObject(obj);
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }

        private IStoryValue m_ObjPath = new StoryValue();
    }
    /// <summary>
    /// autorecycle(obj, 1_or_0);
    /// </summary>
    internal class AutoRecycleCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            AutoRecycleCommand cmd = new AutoRecycleCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Enable = m_Enable.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Enable.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int enable = m_Enable.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            UnityEngine.GameObject obj = null;
            if (null != objPath) {
                obj = UnityEngine.GameObject.Find(objPath);
            } else {
                obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    try {
                        int id = objVal.GetInt();
                        obj = SceneSystem.Instance.GetGameObject(id);
                    } catch {
                        obj = null;
                    }
                }
            }
            if (null != obj) {
                if (enable != 0) {
                    SceneSystem.Instance.GameObjectsFromDsl.Add(obj);
                } else {
                    SceneSystem.Instance.GameObjectsFromDsl.Remove(obj);
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Enable.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue<int> m_Enable = new StoryValue<int>();
    }
    /// <summary>
    /// setparent(objpath,parent,stay_world_pos);
    /// </summary>
    internal class SetParentCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetParentCommand cmd = new SetParentCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Parent = m_Parent.Clone();
            cmd.m_StayWorldPos = m_StayWorldPos.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
            m_ParentObject = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Parent.Evaluate(instance, handler, iterator, args);
            m_StayWorldPos.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            var parentVal = m_Parent.Value;
            int stayWorldPos = m_StayWorldPos.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    m_Object = obj;
                    string parentPath = parentVal.IsString ? parentVal.StringVal : null;
                    if (null != parentPath) {
                        if (string.IsNullOrEmpty(parentPath)) {
                            obj.transform.SetParent(null, stayWorldPos != 0);
                        } else {
                            var pobj = UnityEngine.GameObject.Find(parentPath);
                            if (null != pobj) {
                                m_ParentObject = pobj;
                                obj.transform.SetParent(pobj.transform, stayWorldPos != 0);
                            }
                        }
                    } else {
                        UnityEngine.GameObject pobj = parentVal.IsObject ? parentVal.ObjectVal as UnityEngine.GameObject : null;
                        if (null != pobj) {
                            m_ParentObject = pobj;
                            obj.transform.SetParent(pobj.transform, stayWorldPos != 0);
                        } else {
                            try {
                                int id = parentVal.GetInt();
                                if (id < 0) {
                                    m_ParentObject = null;
                                    obj.transform.SetParent(null, stayWorldPos != 0);
                                } else {
                                    pobj = SceneSystem.Instance.GetGameObject(id);
                                    if (null != pobj) {
                                        m_ParentObject = pobj;
                                        obj.transform.SetParent(pobj.transform, stayWorldPos != 0);
                                    }
                                }
                            } catch {
                            }
                        }
                    }
                    return true;
                }
            } else if (null != m_Object) {
                if (null == m_Object.transform.parent && null == m_ParentObject) {
                    return false;
                } else if (null != m_Object.transform.parent && null != m_ParentObject && m_Object.transform.parent.gameObject == m_ParentObject) {
                    return false;
                } else if (null == m_ParentObject) {
                    m_Object.transform.SetParent(null, stayWorldPos != 0);
                    return true;
                } else {
                    m_Object.transform.SetParent(m_ParentObject.transform, stayWorldPos != 0);
                    return true;
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Parent.InitFromDsl(callData.GetParam(1));
                m_StayWorldPos.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_Parent = new StoryValue();
        private IStoryValue<int> m_StayWorldPos = new StoryValue<int>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
        private UnityEngine.GameObject m_ParentObject = null;
    }
    /// <summary>
    /// setactive(objpath,1_or_0);
    /// </summary>
    internal class SetActiveCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetActiveCommand cmd = new SetActiveCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Active = m_Active.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Active.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int active = m_Active.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            var view = SceneSystem.Instance.GetEntityViewById(id);
                            if (null != view) {
                                view.Active = active != 0;
                            }
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    obj.SetActive(active != 0);
                    m_Object = obj;
                    return true;
                }
            } else if (null != m_Object) {
                if (active != 0 && m_Object.activeSelf || active == 0 && !m_Object.activeSelf) {
                    return false;
                } else {
                    m_Object.SetActive(active != 0);
                    return false;
                }
            }        
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Active.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue<int> m_Active = new StoryValue<int>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
    }
    /// <summary>
    /// setvisible(objpath,1_or_0);
    /// </summary>
    internal class SetVisibleCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetVisibleCommand cmd = new SetVisibleCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Visible = m_Visible.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Visible.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int visible = m_Visible.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            var view = SceneSystem.Instance.GetEntityViewById(id);
                            if (null != view) {
                                view.Visible = visible != 0;
                            }
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    m_Object = obj;
                    var renderers = obj.GetComponentsInChildren<UnityEngine.Renderer>();
                    if (null != renderers) {
                        for (int i = 0; i < renderers.Length; ++i) {
                            renderers[i].enabled = visible != 0;
                        }
                    }
                    return true;
                }
            } else if (null != m_Object) {
                var renderer = m_Object.GetComponentInChildren<UnityEngine.Renderer>();
                if (null != renderer) {
                    if (visible != 0 && renderer.isVisible || visible == 0 && !renderer.isVisible) {
                        return false;
                    } else {
                        renderer.enabled = visible != 0;
                        return false;
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Visible.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue<int> m_Visible = new StoryValue<int>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
    }
    /// <summary>
    /// addcomponent(objpath,type)[obj("varname")];
    /// </summary>
    internal class AddComponentCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            AddComponentCommand cmd = new AddComponentCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_ComponentType = m_ComponentType.Clone();
            cmd.m_HaveObj = m_HaveObj;
            cmd.m_ObjVarName = m_ObjVarName.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            if (m_HaveObj) {
                m_ObjVarName.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objPathVal = m_ObjPath.Value;
            var componentType = m_ComponentType.Value;
            UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
            if (null == obj) {
                string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                        obj = SceneSystem.Instance.GetGameObject(id);
                    } catch {
                        obj = null;
                    }
                }
            }
            if (null != obj) {
                UnityEngine.Component component = null;
                Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                if (null != t) {
                    component = obj.AddComponent(t);
                } else {
                    string name = componentType.IsString ? componentType.StringVal : null;
                    if (null != name) {
                        t = Type.GetType(name);
                        component = obj.AddComponent(t);
                    }
                }
                if (m_HaveObj) {
                    string varName = m_ObjVarName.Value;
                    instance.SetVariable(varName, component);
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_ComponentType.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                if (null != first && null != second) {
                    Load(first);
                    LoadVarName(second);
                }
            }
            return true;
        }
        private void LoadVarName(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "obj" && callData.GetParamNum() == 1) {
                m_ObjVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObj = true;
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
        private bool m_HaveObj = false;
        private IStoryValue<string> m_ObjVarName = new StoryValue<string>();
    }
    /// <summary>
    /// removecomponent(objpath,type);
    /// </summary>
    internal class RemoveComponentCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            RemoveComponentCommand cmd = new RemoveComponentCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_ComponentType = m_ComponentType.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objPathVal = m_ObjPath.Value;
            var componentType = m_ComponentType.Value;
            UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
            if (null == obj) {
                string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                        obj = SceneSystem.Instance.GetGameObject(id);
                    } catch {
                        obj = null;
                    }
                }
            }
            if (null != obj) {
                //UnityEngine.Component component = null;
                Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                if (null != t) {
                    var comp = obj.GetComponent(t);
                    Utility.DestroyObject(comp);
                } else {
                    string name = componentType.IsString ? componentType.StringVal : null;
                    if (null != name) {
                        t = Type.GetType(name);
                        var comp = obj.GetComponent(t);
                        Utility.DestroyObject(comp);
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_ComponentType.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
    }
    /// <summary>
    /// loadui(name, prefab, dslfile);
    /// </summary>
    internal class LoadUiCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            LoadUiCommand cmd = new LoadUiCommand();
            cmd.m_Name = m_Name.Clone();
            cmd.m_Prefab = m_Prefab.Clone();
            cmd.m_DslFile = m_DslFile.Clone();
            cmd.m_DontDestroyOld = m_DontDestroyOld.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Name.Evaluate(instance, handler, iterator, args);
            m_Prefab.Evaluate(instance, handler, iterator, args);
            m_DslFile.Evaluate(instance, handler, iterator, args);
            m_DontDestroyOld.Evaluate(instance, handler, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string name = m_Name.Value;
            string prefab = m_Prefab.Value;
            string dslfile = m_DslFile.Value;
            int dontDestoryOld = m_DontDestroyOld.Value;
            ClientStorySystem.Instance.LoadStoryFromFile(name, dslfile);
			UnityEngine.GameObject asset = ResourceSystem.Instance.GetSharedResource(prefab) as UnityEngine.GameObject;
            if (null != asset) {
                UnityEngine.GameObject uiObj = null;
                if (dontDestoryOld <= 0) {
                    var old = UnityEngine.GameObject.Find(name);
                    if (null != old) {
                        old.transform.SetParent(null);
                        Utility.DestroyObject(old);
                    }
                }
                var rootUi = UnityEngine.GameObject.Find("Canvas");
                uiObj = Utility.AttachUiAsset(rootUi, asset, name);
                SceneSystem.Instance.LoadedUiPrefabs.Add(prefab);
                if (null != uiObj) {
                    if (!string.IsNullOrEmpty(dslfile)) {
                        GameLibrary.Story.UiStoryInitializer initer = uiObj.GetComponent<GameLibrary.Story.UiStoryInitializer>();
                        if (null == initer) {
                            initer = uiObj.AddComponent<GameLibrary.Story.UiStoryInitializer>();
                        }
                        if (null != initer) {
                            initer.WindowName = name;
                            initer.Init();
                        }
                    }
                }
            }
            return false;
        }

        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2) {
                m_Name.InitFromDsl(callData.GetParam(0));
                m_Prefab.InitFromDsl(callData.GetParam(1));
                m_DslFile.InitFromDsl(callData.GetParam(2));
            }
            if (num > 3) {
                m_DontDestroyOld.InitFromDsl(callData.GetParam(3));
            }
            return true;
        }

        private IStoryValue<string> m_Name = new StoryValue<string>();
        private IStoryValue<string> m_Prefab = new StoryValue<string>();
        private IStoryValue<string> m_DslFile = new StoryValue<string>();
        private IStoryValue<int> m_DontDestroyOld = new StoryValue<int>();
    }
    /// <summary>
    /// bindui(gameobject){
    ///     var("@varname","Panel/Input");
    ///     inputs("",...);
    ///     toggles("",...);
    ///     sliders("",...);
    ///     dropdowns("",...);
    ///     onevent("button","eventtag","Panel/Button");
    /// };
    /// </summary>
    internal class BindUiCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            BindUiCommand cmd = new BindUiCommand();
            cmd.m_Obj = m_Obj.Clone();
            for (int i = 0; i < m_VarInfos.Count; ++i) {
                cmd.m_VarInfos.Add(new VarInfo(m_VarInfos[i]));
            }
            for (int i = 0; i < m_EventInfos.Count; ++i) {
                cmd.m_EventInfos.Add(new EventInfo(m_EventInfos[i]));
            }
            for (int i = 0; i < m_Inputs.Count; ++i) {
                cmd.m_Inputs.Add(m_Inputs[i].Clone());
            }
            for (int i = 0; i < m_Toggles.Count; ++i) {
                cmd.m_Toggles.Add(m_Toggles[i].Clone());
            }
            for (int i = 0; i < m_Sliders.Count; ++i) {
                cmd.m_Sliders.Add(m_Sliders[i].Clone());
            }
            for (int i = 0; i < m_DropDowns.Count; ++i) {
                cmd.m_DropDowns.Add(m_DropDowns[i].Clone());
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Obj.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_VarInfos.Count; ++i) {
                m_VarInfos[i].m_VarName.Evaluate(instance, handler, iterator, args);
                m_VarInfos[i].m_ControlPath.Evaluate(instance, handler, iterator, args);
            }
            for (int i = 0; i < m_EventInfos.Count; ++i) {
                m_EventInfos[i].m_Tag.Evaluate(instance, handler, iterator, args);
                m_EventInfos[i].m_Path.Evaluate(instance, handler, iterator, args);
            }
            var list = m_Inputs;
            for (int k = 0; k < list.Count; ++k) {
                list[k].Evaluate(instance, handler, iterator, args);
            }
            list = m_Toggles;
            for (int k = 0; k < list.Count; ++k) {
                list[k].Evaluate(instance, handler, iterator, args);
            }
            list = m_Sliders;
            for (int k = 0; k < list.Count; ++k) {
                list[k].Evaluate(instance, handler, iterator, args);
            }
            list = m_DropDowns;
            for (int k = 0; k < list.Count; ++k) {
                list[k].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            UnityEngine.GameObject obj = m_Obj.Value.ObjectVal as UnityEngine.GameObject;
            if (null != obj) {
                UiStoryInitializer initer = obj.GetComponent<UiStoryInitializer>();
                if (null != initer) {
                    ClientStorySystem.Instance.AddBindedStory(obj, instance);

                    UiStoryEventHandler handler0 = obj.GetComponent<UiStoryEventHandler>();
                    if (null == handler0) {
                        handler0 = obj.AddComponent<UiStoryEventHandler>();
                    }
                    if (null != handler0) {
                        int ct = m_VarInfos.Count;
                        for (int ix = 0; ix < ct; ++ix) {
                            string name = m_VarInfos[ix].m_VarName.Value;
                            string path = m_VarInfos[ix].m_ControlPath.Value;
                            UnityEngine.GameObject ctrl = Utility.FindChildObjectByPath(obj, path);
                            if (null != ctrl)
                                AddVariable(instance, name, ctrl);
                        }
                        handler0.WindowName = initer.WindowName;
                        var list = m_Inputs;
                        for (int k = 0; k < list.Count; ++k) {
                            string path = list[k].Value;
                            var comp = Utility.FindComponentInChildren<UnityEngine.UI.InputField>(obj, path);
                            if (null != comp)
                                handler0.InputLabels.Add(comp);
                        }
                        list = m_Toggles;
                        for (int k = 0; k < list.Count; ++k) {
                            string path = list[k].Value;
                            var comp = Utility.FindComponentInChildren<UnityEngine.UI.Toggle>(obj, path);
                            if (null != comp)
                                handler0.InputToggles.Add(comp);
                        }
                        list = m_Sliders;
                        for (int k = 0; k < list.Count; ++k) {
                            string path = list[k].Value;
                            var comp = Utility.FindComponentInChildren<UnityEngine.UI.Slider>(obj, path);
                            if (null != comp)
                                handler0.InputSliders.Add(comp);
                        }
                        list = m_DropDowns;
                        for (int k = 0; k < list.Count; ++k) {
                            string path = list[k].Value;
                            var comp = Utility.FindComponentInChildren<UnityEngine.UI.Dropdown>(obj, path);
                            if (null != comp)
                                handler0.InputDropdowns.Add(comp);
                        }
                        ct = m_EventInfos.Count;
                        for (int ix = 0; ix < ct; ++ix) {
                            string evt = m_EventInfos[ix].m_Event.Value;
                            string tag = m_EventInfos[ix].m_Tag.Value;
                            string path = m_EventInfos[ix].m_Path.Value;
                            if (evt == "button") {
                                UnityEngine.UI.Button button = Utility.FindComponentInChildren<UnityEngine.UI.Button>(obj, path);
                                if (null != button)
                                    button.onClick.AddListener(() => { handler0.OnClickHandler(tag); });
                            } else if (evt == "toggle") {
                                UnityEngine.UI.Toggle toggle = Utility.FindComponentInChildren<UnityEngine.UI.Toggle>(obj, path);
                                if (null != toggle)
                                    toggle.onValueChanged.AddListener((bool val) => { handler0.OnToggleHandler(tag, val); });
                            } else if (evt == "dropdown") {
                                UnityEngine.UI.Dropdown dropdown = Utility.FindComponentInChildren<UnityEngine.UI.Dropdown>(obj, path);
                                if (null != dropdown)
                                    dropdown.onValueChanged.AddListener((int val) => { handler0.OnDropdownHandler(tag, val); });
                            } else if (evt == "slider") {
                                UnityEngine.UI.Slider slider = Utility.FindComponentInChildren<UnityEngine.UI.Slider>(obj, path);
                                if (null != slider)
                                    slider.onValueChanged.AddListener((float val) => { handler0.OnSliderHandler(tag, val); });
                            } else if (evt == "input") {
                                UnityEngine.UI.InputField input = Utility.FindComponentInChildren<UnityEngine.UI.InputField>(obj, path);
                                if (null != input)
                                    input.onEndEdit.AddListener((string val) => { handler0.OnInputHandler(tag, val); });
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                LoadCall(funcData.LowerOrderFunction);
            }
            else if(funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                for (int i = 0; i < funcData.GetParamNum(); ++i) {
                    Dsl.FunctionData callData = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != callData) {
                        string id = callData.GetId();
                        if (id == "var") {
                            LoadVar(callData);
                        }
                        else if (id == "onevent") {
                            LoadEvent(callData);
                        }
                        else if (id == "inputs") {
                            LoadPaths(m_Inputs, callData);
                        }
                        else if (id == "toggles") {
                            LoadPaths(m_Toggles, callData);
                        }
                        else if (id == "sliders") {
                            LoadPaths(m_Sliders, callData);
                        }
                        else if (id == "dropdowns") {
                            LoadPaths(m_DropDowns, callData);
                        }
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Obj.InitFromDsl(callData.GetParam(0));
            }
        }
        private void LoadVar(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2) {
                VarInfo info = new VarInfo();
                info.m_VarName.InitFromDsl(callData.GetParam(0));
                info.m_ControlPath.InitFromDsl(callData.GetParam(1));
                m_VarInfos.Add(info);
            }
        }
        private void LoadEvent(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 3) {
                EventInfo info = new EventInfo();
                info.m_Event.InitFromDsl(callData.GetParam(0));
                info.m_Tag.InitFromDsl(callData.GetParam(1));
                info.m_Path.InitFromDsl(callData.GetParam(2));
                m_EventInfos.Add(info);
            }
        }
        private class VarInfo
        {
            internal IStoryValue<string> m_VarName = null;
            internal IStoryValue<string> m_ControlPath = null;
            internal VarInfo()
            {
                m_VarName = new StoryValue<string>();
                m_ControlPath = new StoryValue<string>();
            }
            internal VarInfo(VarInfo other)
            {
                m_VarName = other.m_VarName.Clone();
                m_ControlPath = other.m_ControlPath.Clone();
            }
        }
        private class EventInfo
        {
            internal IStoryValue<string> m_Event = null;
            internal IStoryValue<string> m_Tag = null;
            internal IStoryValue<string> m_Path = null;
            internal EventInfo()
            {
                m_Event = new StoryValue<string>();
                m_Tag = new StoryValue<string>();
                m_Path = new StoryValue<string>();
            }
            internal EventInfo(EventInfo other)
            {
                m_Event = other.m_Event.Clone();
                m_Tag = other.m_Tag.Clone();
                m_Path = other.m_Path.Clone();
            }
        }
        private IStoryValue m_Obj = new StoryValue();
        private List<VarInfo> m_VarInfos = new List<VarInfo>();
        internal List<IStoryValue<string>> m_Inputs = new List<IStoryValue<string>>();
        internal List<IStoryValue<string>> m_Toggles = new List<IStoryValue<string>>();
        internal List<IStoryValue<string>> m_Sliders = new List<IStoryValue<string>>();
        internal List<IStoryValue<string>> m_DropDowns = new List<IStoryValue<string>>();
        private List<EventInfo> m_EventInfos = new List<EventInfo>();

        private static void LoadPaths(List<IStoryValue<string>> List, Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i) {
                IStoryValue<string> path = new StoryValue<string>();
                path.InitFromDsl(callData.GetParam(i));
                List.Add(path);
            }
        }
        private static void AddVariable(StoryInstance instance, string name, UnityEngine.GameObject control)
        {
            instance.SetVariable(name, control);
            UnityEngine.UI.Text text = control.GetComponent<UnityEngine.UI.Text>();
            if (null != text) {
                instance.SetVariable(string.Format("{0}_Text", name), BoxedValue.FromObject(text));
            }
            UnityEngine.UI.Image image = control.GetComponent<UnityEngine.UI.Image>();
            if (null != image) {
                instance.SetVariable(string.Format("{0}_Image", name), BoxedValue.FromObject(image));
            }
            UnityEngine.UI.RawImage rawImage = control.GetComponent<UnityEngine.UI.RawImage>();
            if (null != rawImage) {
                instance.SetVariable(string.Format("{0}_RawImage", name), BoxedValue.FromObject(rawImage));
            }
            UnityEngine.UI.Button button = control.GetComponent<UnityEngine.UI.Button>();
            if (null != button) {
                instance.SetVariable(string.Format("{0}_Button", name), BoxedValue.FromObject(button));
            }
            UnityEngine.UI.Dropdown dropdown = control.GetComponent<UnityEngine.UI.Dropdown>();
            if (null != dropdown) {
                instance.SetVariable(string.Format("{0}_Dropdown", name), BoxedValue.FromObject(dropdown));
            }
            UnityEngine.UI.InputField inputField = control.GetComponent<UnityEngine.UI.InputField>();
            if (null != inputField) {
                instance.SetVariable(string.Format("{0}_Input", name), BoxedValue.FromObject(inputField));
            }
            UnityEngine.UI.Slider slider = control.GetComponent<UnityEngine.UI.Slider>();
            if (null != slider) {
                instance.SetVariable(string.Format("{0}_Slider", name), BoxedValue.FromObject(slider));
            }
            UnityEngine.UI.Toggle toggle = control.GetComponent<UnityEngine.UI.Toggle>();
            if (null != toggle) {
                instance.SetVariable(string.Format("{0}_Toggle", name), BoxedValue.FromObject(toggle));
            }
            UnityEngine.UI.ToggleGroup toggleGroup = control.GetComponent<UnityEngine.UI.ToggleGroup>();
            if (null != toggleGroup) {
                instance.SetVariable(string.Format("{0}_ToggleGroup", name), BoxedValue.FromObject(toggleGroup));
            }
            UnityEngine.UI.Scrollbar scrollbar = control.GetComponent<UnityEngine.UI.Scrollbar>();
            if (null != scrollbar) {
                instance.SetVariable(string.Format("{0}_Scrollbar", name), BoxedValue.FromObject(scrollbar));
            }
        }
    }
    /// <summary>
    /// openurl(url);
    /// </summary>
    internal class OpenUrlCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            OpenUrlCommand cmd = new OpenUrlCommand();
            cmd.m_Url = m_Url.Clone();
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Url.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            UnityEngine.Application.OpenURL(m_Url.Value);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Url.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        private IStoryValue<string> m_Url = new StoryValue<string>();
    }
    /// <summary>
    /// quit();
    /// </summary>
    internal class QuitCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            QuitCommand cmd = new QuitCommand();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            UnityEngine.Application.Quit();
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            return true;
        }
    }
    /// <summary>
    /// changescene(target_scene_id);
    /// </summary>
    internal class ChangeSceneCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ChangeSceneCommand cmd = new ChangeSceneCommand();
            cmd.m_TargetScene = m_TargetScene.Clone();
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_TargetScene.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string targetScene = m_TargetScene.Value;
            SceneSystem.Instance.ChangeScene(targetScene);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_TargetScene.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        private IStoryValue<string> m_TargetScene = new StoryValue<string>();
    }
    /// <summary>
    /// highlightprompt(info);
    /// </summary>
    internal class HighlightPromptCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            HighlightPromptCommand cmd = new HighlightPromptCommand();
            cmd.m_Info = m_Info.Clone();
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Info.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string info = m_Info.Value;
            SceneSystem.Instance.HighlightPrompt(info);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Info.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        private IStoryValue<string> m_Info = new StoryValue<string>();
    }
    /// <summary>
    /// setactorscale(name,value);
    /// </summary>
    internal class SetActorScaleCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetActorScaleCommand cmd = new SetActorScaleCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            var value = m_Value.Value;
            UnityEngine.GameObject obj = SceneSystem.Instance.GetGameObject(objId);
            if (null != obj) {
                UnityEngine.Vector3 scale = value;                
                obj.transform.localScale = new UnityEngine.Vector3(scale.x, scale.y, scale.z);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue m_Value = new StoryValue();
    }
    /// <summary>
    /// gameobjectanimation(obj, anim[, normalized_time]);
    /// </summary>
    internal class GameObjectAnimationCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            GameObjectAnimationCommand cmd = new GameObjectAnimationCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Anim = m_Anim.Clone();
            cmd.m_Time = m_Time.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Anim.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 2) {
                m_Time.Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var o = m_ObjPath.Value;
            string objPath = o.IsString ? o.StringVal : null;
            UnityEngine.GameObject uobj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
            if (null == uobj) {
                if (null != objPath) {
                    uobj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int objId = o.GetInt();
                        uobj = SceneSystem.Instance.GetGameObject(objId);
                    } catch {
                        uobj = null;
                    }
                }
            }
            if (null != uobj) {
                string anim = m_Anim.Value;
                EntityViewModel view = SceneSystem.Instance.GetEntityView(uobj);
                if (null != view) {
                    view.PlayAnimation(anim);
                } else {
                    var animators = uobj.GetComponentsInChildren<UnityEngine.Animator>();
                    if (null != animators) {
                        for (int i = 0; i < animators.Length; ++i) {
                            animators[i].Play(anim);
                        }
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Anim.InitFromDsl(callData.GetParam(1));
            }
            if (m_ParamNum > 2) {
                m_Time.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }
        private int m_ParamNum = 0;
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue<string> m_Anim = new StoryValue<string>();
        private IStoryValue<float> m_Time = new StoryValue<float>();
    }
    /// <summary>
    /// gameobjectanimationparam(obj)
    /// {
    ///     float(name,val);
    ///     int(name,val);
    ///     bool(name,val);
    ///     trigger(name,val);
    /// };
    /// </summary>
    internal class GameObjectAnimationParamCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            GameObjectAnimationParamCommand cmd = new GameObjectAnimationParamCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            for (int i = 0; i < m_Params.Count; ++i) {
                ParamInfo param = new ParamInfo();
                param.CopyFrom(m_Params[i]);
                cmd.m_Params.Add(param);
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Params.Count; ++i) {
                var pair = m_Params[i];
                pair.Key.Evaluate(instance, handler, iterator, args);
                pair.Value.Evaluate(instance, handler, iterator, args);
            }

            for (int i = 0; i < m_Params.Count; ++i) {
                var pair = m_Params[i];
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var o = m_ObjPath.Value;
            string objPath = o.IsString ? o.StringVal : null;
            UnityEngine.GameObject obj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
            if (null == obj) {
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int objId = o.GetInt();
                        obj = SceneSystem.Instance.GetGameObject(objId);
                    } catch {
                        obj = null;
                    }
                }
            }
            if (null != obj) {
                UnityEngine.Animator animator = obj.GetComponentInChildren<UnityEngine.Animator>();
                if (null != animator) {
                    for (int i = 0; i < m_Params.Count; ++i) {
                        var param = m_Params[i];
                        string type = param.Type;
                        string key = param.Key.Value;
                        var val = param.Value.Value;
                        if (type == "int") {
                            int v = val.GetInt();
                            animator.SetInteger(key, v);
                        } else if (type == "float") {
                            float v = val.GetFloat();
                            animator.SetFloat(key, v);
                        } else if (type == "bool") {
                            bool v = val.GetBool();
                            animator.SetBool(key, v);
                        } else if (type == "trigger") {
                            string v = val.ToString();
                            if (v == "false") {
                                animator.ResetTrigger(key);
                            } else {
                                animator.SetTrigger(key);
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                LoadCall(funcData.LowerOrderFunction);
            }
            else if (funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                for (int i = 0; i < funcData.GetParamNum(); ++i) {
                    Dsl.ISyntaxComponent statement = funcData.GetParam(i);
                    Dsl.FunctionData stCall = statement as Dsl.FunctionData;
                    if (null != stCall && stCall.GetParamNum() >= 2) {
                        string id = stCall.GetId();
                        ParamInfo param = new ParamInfo(id, stCall.GetParam(0), stCall.GetParam(1));
                        m_Params.Add(param);
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
            }
        }
        private class ParamInfo
        {
            internal string Type;
            internal IStoryValue<string> Key;
            internal IStoryValue Value;
            internal ParamInfo()
            {
                Init();
            }
            internal ParamInfo(string type, Dsl.ISyntaxComponent keyDsl, Dsl.ISyntaxComponent valDsl)
                : this()
            {
                Type = type;
                Key.InitFromDsl(keyDsl);
                Value.InitFromDsl(valDsl);
            }
            internal void CopyFrom(ParamInfo other)
            {
                Type = other.Type;
                Key = other.Key.Clone();
                Value = other.Value.Clone();
            }
            private void Init()
            {
                Type = string.Empty;
                Key = new StoryValue<string>();
                Value = new StoryValue();
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }
    /// <summary>
    /// setstoryvariable(storyId[, namespace], varname, val);
    /// </summary>
    internal class SetStoryVariableCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetStoryVariableCommand cmd = new SetStoryVariableCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_StoryId = m_StoryId.Clone();
            cmd.m_Namespace = m_Namespace.Clone();
            cmd.m_Name = m_Name.Clone();
            cmd.m_Val = m_Val.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_StoryId.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 3) {
                m_Namespace.Evaluate(instance, handler, iterator, args);
            }
            m_Name.Evaluate(instance, handler, iterator, args);
            m_Val.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string storyId = m_StoryId.Value;
            string ns = string.Empty;
            if (m_ParamNum > 3) {
                ns = m_Namespace.Value;
            }
            string name = m_Name.Value;
            var val = m_Val.Value;
            var storyInstance = ClientStorySystem.Instance.GetStory(storyId, ns);
            if (null != storyInstance) {
                storyInstance.SetVariable(name, val);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 2) {
                m_StoryId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 3) {
                    m_Namespace.InitFromDsl(callData.GetParam(1));
                    m_Name.InitFromDsl(callData.GetParam(2));
                    m_Val.InitFromDsl(callData.GetParam(3));
                } else {
                    m_Name.InitFromDsl(callData.GetParam(1));
                    m_Val.InitFromDsl(callData.GetParam(2));
                }
            }
            return true;
        }
        private int m_ParamNum = 0;
        private IStoryValue<string> m_StoryId = new StoryValue<string>();
        private IStoryValue<string> m_Namespace = new StoryValue<string>();
        private IStoryValue<string> m_Name = new StoryValue<string>();
        private IStoryValue m_Val = new StoryValue();
    }
}