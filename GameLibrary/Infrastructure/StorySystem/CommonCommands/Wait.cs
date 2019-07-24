﻿using System;
using System.Collections.Generic;
namespace StorySystem.CommonCommands
{
    /// <summary>
    /// sleep(milliseconds);
    /// </summary>
    internal sealed class SleepCommand : AbstractStoryCommand
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
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
            if (curTime <= val && curTime <= StoryValueHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
            }
        }
        protected override void Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First;
                Dsl.FunctionData second = statementData.Second;
                if (null != first && null != first.Call && null != second && null != second.Call) {
                    Load(first.Call);
                    LoadCondition(second.Call);
                }
            }
        }
        private void LoadCondition(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryValue<int> m_Time = new StoryValue<int>();
        private IStoryValue<int> m_Condition = new StoryValue<int>();
        private bool m_HaveCondition = false;
        private int m_CurTime = 0;
    }
    /// <summary>
    /// realsleep(milliseconds);
    /// </summary>
    internal sealed class RealTimeSleepCommand : AbstractStoryCommand
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
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
                m_RealStartTime = (int)GameLibrary.TimeUtility.GetLocalRealMilliseconds();
            }
            int curTime = (int)GameLibrary.TimeUtility.GetLocalRealMilliseconds();
            int val = m_Time.Value;
            if (curTime <= m_RealStartTime + val && curTime <= m_RealStartTime + StoryValueHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
            }
        }
        protected override void Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First;
                Dsl.FunctionData second = statementData.Second;
                if (null != first && null != first.Call && null != second && null != second.Call) {
                    Load(first.Call);
                    LoadCondition(second.Call);
                }
            }
        }
        private void LoadCondition(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryValue<int> m_Time = new StoryValue<int>();
        private IStoryValue<int> m_Condition = new StoryValue<int>();
        private bool m_HaveCondition = false;
        private int m_RealStartTime = 0;
    }
    /// <summary>
    /// storysleep(milliseconds);
    /// </summary>
    internal sealed class StorySleepCommand : AbstractStoryCommand
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
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
            if (GameLibrary.GlobalVariables.Instance.IsStorySkipped) {
                return false;
            }
            if (GameLibrary.GlobalVariables.Instance.IsStorySpeedup) {
                return false;
            }
            int curTime = m_CurTime;
            m_CurTime += (int)delta;
            int val = m_Time.Value;
            if (curTime <= val && curTime <= StoryValueHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
            }
        }
        protected override void Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First;
                Dsl.FunctionData second = statementData.Second;
                if (null != first && null != first.Call && null != second && null != second.Call) {
                    Load(first.Call);
                    LoadCondition(second.Call);
                }
            }
        }
        private void LoadCondition(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryValue<int> m_Time = new StoryValue<int>();
        private IStoryValue<int> m_Condition = new StoryValue<int>();
        private bool m_HaveCondition = false;
        private int m_CurTime = 0;
    }
    /// <summary>
    /// storyrealsleep(milliseconds);
    /// </summary>
    internal sealed class StoryRealTimeSleepCommand : AbstractStoryCommand
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
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
            if (GameLibrary.GlobalVariables.Instance.IsStorySkipped) {
                return false;
            }
            if (GameLibrary.GlobalVariables.Instance.IsStorySpeedup) {
                return false;
            }
            if (m_RealStartTime <= 0) {
                m_RealStartTime = (int)GameLibrary.TimeUtility.GetLocalRealMilliseconds();
            }
            int curTime = (int)GameLibrary.TimeUtility.GetLocalRealMilliseconds();
            int val = m_Time.Value;
            if (curTime <= m_RealStartTime + val && curTime <= m_RealStartTime + StoryValueHelper.c_MaxWaitCommandTime)
                return true;
            else
                return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Time.InitFromDsl(callData.GetParam(0));
            }
        }
        protected override void Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First;
                Dsl.FunctionData second = statementData.Second;
                if (null != first && null != first.Call && null != second && null != second.Call) {
                    Load(first.Call);
                    LoadCondition(second.Call);
                }
            }
        }
        private void LoadCondition(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryValue<int> m_Time = new StoryValue<int>();
        private IStoryValue<int> m_Condition = new StoryValue<int>();
        private bool m_HaveCondition = false;
        private int m_RealStartTime = 0;
    }
    internal sealed class StoryBreakCommand : AbstractStoryCommand
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            if (m_HaveCondition)
                m_Condition.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_HaveCondition && m_Condition.HaveValue && m_Condition.Value == 0) {
                return false;
            }
            if (GameLibrary.GlobalVariables.Instance.IsStorySkipped) {
                return false;
            }
            if (GameLibrary.GlobalVariables.Instance.IsStorySpeedup) {
                return false;
            }
            if (GameLibrary.GlobalVariables.Instance.StoryEditorOpen && GameLibrary.GlobalVariables.Instance.StoryEditorContinue) {
                GameLibrary.GlobalVariables.Instance.StoryEditorContinue = false;
                return false;
            }
            return true;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_HaveCondition = true;
                m_Condition.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryValue<int> m_Condition = new StoryValue<int>();
        private bool m_HaveCondition = false;
    }
}
