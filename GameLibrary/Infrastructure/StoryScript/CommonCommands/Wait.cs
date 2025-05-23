﻿using System;
using System.Collections.Generic;
namespace StoryScript.CommonCommands
{
    /// <summary>
    /// sleep(milliseconds);
    /// </summary>
    public sealed class SleepCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SleepCommand cmd = new SleepCommand();
            cmd.m_Time = m_Time.Clone();
            cmd.m_Condition = m_Condition.Clone();
            cmd.m_HaveCondition = m_HaveCondition;
            return cmd;
        }
        protected override void ResetState()
        {
            m_CurTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Time.Evaluate(instance, handler, iterator, args);
            if (m_HaveCondition)
                m_Condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_HaveCondition && m_Condition.HaveValue && m_Condition.Value == 0) {
                return false;
            }
            int curTime = m_CurTime;
            m_CurTime += (int)delta;
            int val = m_Time.Value;
            if (curTime <= val && curTime <= StoryFunctionHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
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
                    LoadCondition(second);
                }
            }
            return true;
        }
        private void LoadCondition(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryFunction<int> m_Time = new StoryFunction<int>();
        private IStoryFunction<int> m_Condition = new StoryFunction<int>();
        private bool m_HaveCondition = false;
        private int m_CurTime = 0;
    }
    /// <summary>
    /// realsleep(milliseconds);
    /// </summary>
    public sealed class RealTimeSleepCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            RealTimeSleepCommand cmd = new RealTimeSleepCommand();
            cmd.m_Time = m_Time.Clone();
            cmd.m_Condition = m_Condition.Clone();
            cmd.m_HaveCondition = m_HaveCondition;
            return cmd;
        }
        protected override void ResetState()
        {
            m_RealStartTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Time.Evaluate(instance, handler, iterator, args);
            if (m_HaveCondition)
                m_Condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_HaveCondition && m_Condition.HaveValue && m_Condition.Value == 0) {
                return false;
            }
            if (m_RealStartTime <= 0) {
                m_RealStartTime = (int)TimeUtility.GetLocalRealMilliseconds();
            }
            int curTime = (int)TimeUtility.GetLocalRealMilliseconds();
            int val = m_Time.Value;
            if (curTime <= m_RealStartTime + val && curTime <= m_RealStartTime + StoryFunctionHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
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
                    LoadCondition(second);
                }
            }
            return true;
        }
        private void LoadCondition(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryFunction<int> m_Time = new StoryFunction<int>();
        private IStoryFunction<int> m_Condition = new StoryFunction<int>();
        private bool m_HaveCondition = false;
        private int m_RealStartTime = 0;
    }
    /// <summary>
    /// storysleep(milliseconds);
    /// </summary>
    public sealed class StorySleepCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            StorySleepCommand cmd = new StorySleepCommand();
            cmd.m_Time = m_Time.Clone();
            cmd.m_Condition = m_Condition.Clone();
            cmd.m_HaveCondition = m_HaveCondition;
            return cmd;
        }
        protected override void ResetState()
        {
            m_CurTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Time.Evaluate(instance, handler, iterator, args);
            if (m_HaveCondition)
                m_Condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_HaveCondition && m_Condition.HaveValue && m_Condition.Value == 0) {
                return false;
            }
            if (StoryConfigManager.Instance.IsStorySkipped) {
                return false;
            }
            if (StoryConfigManager.Instance.IsStorySpeedup) {
                return false;
            }
            int curTime = m_CurTime;
            m_CurTime += (int)delta;
            int val = m_Time.Value;
            if (curTime <= val && curTime <= StoryFunctionHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
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
                    LoadCondition(second);
                }
            }
            return true;
        }
        private void LoadCondition(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryFunction<int> m_Time = new StoryFunction<int>();
        private IStoryFunction<int> m_Condition = new StoryFunction<int>();
        private bool m_HaveCondition = false;
        private int m_CurTime = 0;
    }
    /// <summary>
    /// storyrealsleep(milliseconds);
    /// </summary>
    public sealed class StoryRealTimeSleepCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            StoryRealTimeSleepCommand cmd = new StoryRealTimeSleepCommand();
            cmd.m_Time = m_Time.Clone();
            cmd.m_Condition = m_Condition.Clone();
            cmd.m_HaveCondition = m_HaveCondition;
            return cmd;
        }
        protected override void ResetState()
        {
            m_RealStartTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Time.Evaluate(instance, handler, iterator, args);
            if (m_HaveCondition)
                m_Condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_HaveCondition && m_Condition.HaveValue && m_Condition.Value == 0) {
                return false;
            }
            if (StoryConfigManager.Instance.IsStorySkipped) {
                return false;
            }
            if (StoryConfigManager.Instance.IsStorySpeedup) {
                return false;
            }
            if (m_RealStartTime <= 0) {
                m_RealStartTime = (int)TimeUtility.GetLocalRealMilliseconds();
            }
            int curTime = (int)TimeUtility.GetLocalRealMilliseconds();
            int val = m_Time.Value;
            if (curTime <= m_RealStartTime + val && curTime <= m_RealStartTime + StoryFunctionHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
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
                    LoadCondition(second);
                }
            }
            return true;
        }
        private void LoadCondition(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryFunction<int> m_Time = new StoryFunction<int>();
        private IStoryFunction<int> m_Condition = new StoryFunction<int>();
        private bool m_HaveCondition = false;
        private int m_RealStartTime = 0;
    }
    public sealed class StoryBreakCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            StoryBreakCommand cmd = new StoryBreakCommand();
            cmd.m_Condition = m_Condition.Clone();
            cmd.m_HaveCondition = m_HaveCondition;
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            if (m_HaveCondition)
                m_Condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_HaveCondition && m_Condition.HaveValue && m_Condition.Value == 0) {
                return false;
            }
            if (StoryConfigManager.Instance.IsStorySkipped) {
                return false;
            }
            if (StoryConfigManager.Instance.IsStorySpeedup) {
                return false;
            }
            if (StoryConfigManager.Instance.StoryEditorOpen && StoryConfigManager.Instance.StoryEditorContinue) {
                StoryConfigManager.Instance.StoryEditorContinue = false;
                return false;
            }
            return true;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }

        private IStoryFunction<int> m_Condition = new StoryFunction<int>();
        private bool m_HaveCondition = false;
    }
}
