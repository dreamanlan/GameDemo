using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// waitstory(story_id1, story_id2, ...)[set(var,val)timeoutset(timeout,var,val)] - wait for stories to finish
    /// waitstory(story_id1, story_id2, ...){
    ///     set(var, val);
    ///     timeoutset(timeout, var, val);
    /// };
    /// </summary>
    internal sealed class WaitStoryExp : AbstractExpression
    {
        public override bool IsAsync { get { return true; } }

        protected override IEnumerator DoCalc(AsyncCalcResult result)
        {
            if (Calculator.IsInSyncCalculation) {
                yield break;
            }
            List<string> storyIds = new List<string>();
            for (int i = 0; i < m_StoryIds.Count; ++i)
            {
                storyIds.Add(m_StoryIds[i].Calc().ToString());
            }

            var storyInst = Calculator.GetFuncContext<StoryInstance>();
            if (storyInst == null)
            {
                result.Value = BoxedValue.NullObject;
                yield break;
            }

            int timeout = m_HaveSet ? m_TimeoutVal.Calc().GetInt() : 0;
            int curTime = 0;

            // Wait until all specified stories have finished
            while (true)
            {
                int ct = 0;
                for (int i = 0; i < storyIds.Count; ++i)
                {
                    ct += ClientStorySystem.Instance.CountStory(storyIds[i]);
                }
                if (ct <= 0)
                {
                    if (m_HaveSet)
                    {
                        string varName = m_SetVar.Calc().ToString();
                        var varVal = m_SetVal.Calc();
                        storyInst.SetVariable(varName, varVal);
                    }
                    break;
                }
                if (timeout > 0 && curTime > timeout)
                {
                    if (m_HaveSet)
                    {
                        string varName = m_TimeoutSetVar.Calc().ToString();
                        var varVal = m_TimeoutSetVal.Calc();
                        storyInst.SetVariable(varName, varVal);
                    }
                    break;
                }
                if (StoryConfigManager.Instance.IsStorySkipped) {
                    break;
                }
                yield return null;
                curTime += 1; // Approximate frame time
            }

            result.Value = BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            int num = funcData.GetParamNum();
            for (int i = 0; i < num; ++i)
            {
                m_StoryIds.Add(Calculator.Load(funcData.GetParam(i)));
            }
            if (funcData.IsHighOrder)
            {
                LoadHighOrder(funcData.LowerOrderFunction);
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            int ct = statementData.Functions.Count;
            if (ct >= 2)
            {
                var first = statementData.First.AsFunction;
                if (ct == 2)
                {
                    Load(first);
                    var second = statementData.Second.AsFunction;
                    LoadSetOrTimeoutSet(second);
                }
                else if (ct >= 3)
                {
                    var second = statementData.Second.AsFunction;
                    var third = statementData.Third.AsFunction;
                    Load(first);
                    LoadSet(second);
                    LoadTimeoutSet(third);
                }
            }
            return true;
        }

        private void LoadHighOrder(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "set")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 2)
                {
                    m_SetVar = Calculator.Load(callData.GetParam(0));
                    m_SetVal = Calculator.Load(callData.GetParam(1));
                }
            }
            else if (callData.GetId() == "timeoutset")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 3)
                {
                    m_TimeoutVal = Calculator.Load(callData.GetParam(0));
                    m_TimeoutSetVar = Calculator.Load(callData.GetParam(1));
                    m_TimeoutSetVal = Calculator.Load(callData.GetParam(2));
                }
            }
        }

        private void LoadSetOrTimeoutSet(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "set")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 2)
                {
                    m_SetVar = Calculator.Load(callData.GetParam(0));
                    m_SetVal = Calculator.Load(callData.GetParam(1));
                }
            }
            else if (callData.GetId() == "timeoutset")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 3)
                {
                    m_TimeoutVal = Calculator.Load(callData.GetParam(0));
                    m_TimeoutSetVar = Calculator.Load(callData.GetParam(1));
                    m_TimeoutSetVal = Calculator.Load(callData.GetParam(2));
                }
            }
        }

        private void LoadSet(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "set")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 2)
                {
                    m_SetVar = Calculator.Load(callData.GetParam(0));
                    m_SetVal = Calculator.Load(callData.GetParam(1));
                }
            }
        }

        private void LoadTimeoutSet(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "timeoutset")
            {
                int num = callData.GetParamNum();
                if (num >= 3)
                {
                    m_TimeoutVal = Calculator.Load(callData.GetParam(0));
                    m_TimeoutSetVar = Calculator.Load(callData.GetParam(1));
                    m_TimeoutSetVal = Calculator.Load(callData.GetParam(2));
                }
            }
        }

        private List<IExpression> m_StoryIds = new List<IExpression>();
        private bool m_HaveSet = false;
        private IExpression m_SetVar;
        private IExpression m_SetVal;
        private IExpression m_TimeoutVal;
        private IExpression m_TimeoutSetVar;
        private IExpression m_TimeoutSetVal;
    }

    /// <summary>
    /// waitallmessage(msgid1, msgid2, ...)[set(var,val)timeoutset(timeout,var,val)] - wait for all specified messages to be received
    /// waitallmessage(msgid1, msgid2, ...){
    ///     set(var, val);
    ///     timeoutset(timeout, var, val);
    /// };
    /// </summary>
    internal sealed class WaitAllMessageExp : AbstractExpression
    {
        public override bool IsAsync { get { return true; } }

        protected override IEnumerator DoCalc(AsyncCalcResult result)
        {
            if (Calculator.IsInSyncCalculation) {
                yield break;
            }
            List<string> msgIds = new List<string>();
            for (int i = 0; i < m_MsgIds.Count; ++i)
            {
                msgIds.Add(m_MsgIds[i].Calc().ToString());
            }

            var storyInst = Calculator.GetFuncContext<StoryInstance>();
            if (storyInst == null)
            {
                result.Value = BoxedValue.NullObject;
                yield break;
            }

            long startTime = TimeUtility.GetLocalMilliseconds();
            int timeout = m_HaveSet ? m_TimeoutVal.Calc().GetInt() : 0;
            int curTime = 0;

            // Wait until all specified messages have been received after start time
            while (true)
            {
                bool triggered = false;
                for (int i = 0; i < msgIds.Count; ++i)
                {
                    long time = storyInst.GetMessageTriggerTime(msgIds[i]);
                    if (time > startTime)
                    {
                        triggered = true;
                        break;
                    }
                }
                if (triggered)
                {
                    if (m_HaveSet)
                    {
                        string varName = m_SetVar.Calc().ToString();
                        var varVal = m_SetVal.Calc();
                        storyInst.SetVariable(varName, varVal);
                    }
                    break;
                }
                if (timeout > 0 && curTime > timeout)
                {
                    if (m_HaveSet)
                    {
                        string varName = m_TimeoutSetVar.Calc().ToString();
                        var varVal = m_TimeoutSetVal.Calc();
                        storyInst.SetVariable(varName, varVal);
                    }
                    break;
                }
                if (StoryConfigManager.Instance.IsStorySkipped) {
                    break;
                }
                yield return null;
                curTime += 1; // Approximate frame time
            }

            result.Value = BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            int num = funcData.GetParamNum();
            for (int i = 0; i < num; ++i)
            {
                m_MsgIds.Add(Calculator.Load(funcData.GetParam(i)));
            }
            if (funcData.IsHighOrder)
            {
                LoadHighOrder(funcData.LowerOrderFunction);
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count >= 3)
            {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                var third = statementData.Third.AsFunction;
                if (null != first && null != second && null != third)
                {
                    m_HaveSet = true;
                    Load(first);
                    LoadSet(second);
                    LoadTimeoutSet(third);
                }
            }
            return true;
        }

        private void LoadHighOrder(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "set")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 2)
                {
                    m_SetVar = Calculator.Load(callData.GetParam(0));
                    m_SetVal = Calculator.Load(callData.GetParam(1));
                }
            }
            else if (callData.GetId() == "timeoutset")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 3)
                {
                    m_TimeoutVal = Calculator.Load(callData.GetParam(0));
                    m_TimeoutSetVar = Calculator.Load(callData.GetParam(1));
                    m_TimeoutSetVal = Calculator.Load(callData.GetParam(2));
                }
            }
        }

        private void LoadSet(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "set")
            {
                int num = callData.GetParamNum();
                if (num >= 2)
                {
                    m_SetVar = Calculator.Load(callData.GetParam(0));
                    m_SetVal = Calculator.Load(callData.GetParam(1));
                }
            }
        }

        private void LoadTimeoutSet(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "timeoutset")
            {
                int num = callData.GetParamNum();
                if (num >= 3)
                {
                    m_TimeoutVal = Calculator.Load(callData.GetParam(0));
                    m_TimeoutSetVar = Calculator.Load(callData.GetParam(1));
                    m_TimeoutSetVal = Calculator.Load(callData.GetParam(2));
                }
            }
        }

        private List<IExpression> m_MsgIds = new List<IExpression>();
        private bool m_HaveSet = false;
        private IExpression m_SetVar;
        private IExpression m_SetVal;
        private IExpression m_TimeoutVal;
        private IExpression m_TimeoutSetVar;
        private IExpression m_TimeoutSetVal;
    }

    /// <summary>
    /// waitallmessagehandler(msgid1, msgid2, ...)[set(var,val)timeoutset(timeout,var,val)] - wait for all message handlers to finish
    /// waitallmessagehandler(msgid1, msgid2, ...){
    ///     set(var, val);
    ///     timeoutset(timeout, var, val);
    /// };
    /// </summary>
    internal sealed class WaitAllMessageHandlerExp : AbstractExpression
    {
        public override bool IsAsync { get { return true; } }

        protected override IEnumerator DoCalc(AsyncCalcResult result)
        {
            if (Calculator.IsInSyncCalculation) {
                yield break;
            }
            List<string> msgIds = new List<string>();
            for (int i = 0; i < m_MsgIds.Count; ++i)
            {
                msgIds.Add(m_MsgIds[i].Calc().ToString());
            }

            var storyInst = Calculator.GetFuncContext<StoryInstance>();
            if (storyInst == null)
            {
                result.Value = BoxedValue.NullObject;
                yield break;
            }

            int timeout = m_HaveSet ? m_TimeoutVal.Calc().GetInt() : 0;
            int curTime = 0;

            // Wait until all specified message handlers have finished
            while (true)
            {
                int ct = 0;
                for (int i = 0; i < msgIds.Count; ++i)
                {
                    ct += ClientStorySystem.Instance.CountMessage(msgIds[i]);
                }
                if (ct <= 0)
                {
                    if (m_HaveSet)
                    {
                        string varName = m_SetVar.Calc().ToString();
                        var varVal = m_SetVal.Calc();
                        storyInst.SetVariable(varName, varVal);
                    }
                    break;
                }
                if (timeout > 0 && curTime > timeout)
                {
                    if (m_HaveSet)
                    {
                        string varName = m_TimeoutSetVar.Calc().ToString();
                        var varVal = m_TimeoutSetVal.Calc();
                        storyInst.SetVariable(varName, varVal);
                    }
                    break;
                }
                if (StoryConfigManager.Instance.IsStorySkipped) {
                    break;
                }
                yield return null;
                curTime += 1; // Approximate frame time
            }

            result.Value = BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            int num = funcData.GetParamNum();
            for (int i = 0; i < num; ++i)
            {
                m_MsgIds.Add(Calculator.Load(funcData.GetParam(i)));
            }
            if (funcData.IsHighOrder)
            {
                LoadHighOrder(funcData.LowerOrderFunction);
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count >= 3)
            {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                var third = statementData.Third.AsFunction;
                if (null != first && null != second && null != third)
                {
                    m_HaveSet = true;
                    Load(first);
                    LoadSet(second);
                    LoadTimeoutSet(third);
                }
            }
            return true;
        }

        private void LoadHighOrder(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "set")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 2)
                {
                    m_SetVar = Calculator.Load(callData.GetParam(0));
                    m_SetVal = Calculator.Load(callData.GetParam(1));
                }
            }
            else if (callData.GetId() == "timeoutset")
            {
                m_HaveSet = true;
                int num = callData.GetParamNum();
                if (num >= 3)
                {
                    m_TimeoutVal = Calculator.Load(callData.GetParam(0));
                    m_TimeoutSetVar = Calculator.Load(callData.GetParam(1));
                    m_TimeoutSetVal = Calculator.Load(callData.GetParam(2));
                }
            }
        }

        private void LoadSet(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "set")
            {
                int num = callData.GetParamNum();
                if (num >= 2)
                {
                    m_SetVar = Calculator.Load(callData.GetParam(0));
                    m_SetVal = Calculator.Load(callData.GetParam(1));
                }
            }
        }

        private void LoadTimeoutSet(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "timeoutset")
            {
                int num = callData.GetParamNum();
                if (num >= 3)
                {
                    m_TimeoutVal = Calculator.Load(callData.GetParam(0));
                    m_TimeoutSetVar = Calculator.Load(callData.GetParam(1));
                    m_TimeoutSetVal = Calculator.Load(callData.GetParam(2));
                }
            }
        }

        private List<IExpression> m_MsgIds = new List<IExpression>();
        private bool m_HaveSet = false;
        private IExpression m_SetVar;
        private IExpression m_SetVal;
        private IExpression m_TimeoutVal;
        private IExpression m_TimeoutSetVar;
        private IExpression m_TimeoutSetVal;
    }
}
