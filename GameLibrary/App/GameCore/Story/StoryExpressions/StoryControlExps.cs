using System;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// startstory(story_id) { multiple(val); } - start a story
    /// startstory(story_id){
    ///     multiple(val);
    /// };
    /// </summary>
    internal sealed class StartStoryExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            string storyId = m_StoryId.Calc().ToString();
            int multiple = m_Multiple != null ? m_Multiple.Calc().GetInt() : 0;
            SceneSystem.Instance.QueueAction(() =>
            {
                if (multiple == 0)
                    ClientStorySystem.Instance.StartStory(storyId);
                else
                    ClientStorySystem.Instance.StartStories(storyId);
            });
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            int num = funcData.GetParamNum();
            if (num > 0)
            {
                m_StoryId = Calculator.Load(funcData.GetParam(0));
            }
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd && cd.GetId() == "multiple" && cd.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(cd.GetParam(0));
                    }
                }
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2)
            {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (null != first && null != second)
                {
                    Load(first);
                    if (second.GetId() == "multiple" && second.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(second.GetParam(0));
                    }
                }
            }
            return true;
        }

        private IExpression m_StoryId;
        private IExpression m_Multiple;
    }

    /// <summary>
    /// stopstory(story_id) { multiple(val); } - stop a story
    /// stopstory(story_id){
    ///     multiple(val);
    /// };
    /// </summary>
    internal sealed class StopStoryExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            string storyId = m_StoryId.Calc().ToString();
            int multiple = m_Multiple != null ? m_Multiple.Calc().GetInt() : 0;
            if (multiple == 0)
                ClientStorySystem.Instance.MarkStoryTerminated(storyId);
            else
                ClientStorySystem.Instance.MarkStoriesTerminated(storyId);
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            int num = funcData.GetParamNum();
            if (num > 0)
            {
                m_StoryId = Calculator.Load(funcData.GetParam(0));
            }
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd && cd.GetId() == "multiple" && cd.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(cd.GetParam(0));
                    }
                }
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2)
            {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (null != first && null != second)
                {
                    Load(first);
                    if (second.GetId() == "multiple" && second.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(second.GetParam(0));
                    }
                }
            }
            return true;
        }

        private IExpression m_StoryId;
        private IExpression m_Multiple;
    }

    /// <summary>
    /// pausestory(story_id1, story_id2, ...) { multiple(val); } - pause stories
    /// pausestory(story_id1, story_id2, ...){
    ///     multiple(val);
    /// };
    /// </summary>
    internal sealed class PauseStoryExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            int multiple = m_Multiple != null ? m_Multiple.Calc().GetInt() : 0;
            if (multiple == 0)
            {
                for (int i = 0; i < m_StoryIds.Count; i++)
                {
                    string storyId = m_StoryIds[i].Calc().ToString();
                    ClientStorySystem.Instance.PauseStory(storyId, true);
                }
            }
            else
            {
                for (int i = 0; i < m_StoryIds.Count; i++)
                {
                    string storyId = m_StoryIds[i].Calc().ToString();
                    ClientStorySystem.Instance.PauseStories(storyId, true);
                }
            }
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            int num = funcData.GetParamNum();
            for (int i = 0; i < num; ++i)
            {
                m_StoryIds.Add(Calculator.Load(funcData.GetParam(i)));
            }
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd && cd.GetId() == "multiple" && cd.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(cd.GetParam(0));
                    }
                }
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2)
            {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (null != first && null != second)
                {
                    Load(first);
                    if (second.GetId() == "multiple" && second.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(second.GetParam(0));
                    }
                }
            }
            return true;
        }

        private List<IExpression> m_StoryIds = new List<IExpression>();
        private IExpression m_Multiple;
    }

    /// <summary>
    /// resumestory(story_id1, story_id2, ...) { multiple(val); } - resume paused stories
    /// resumestory(story_id1, story_id2, ...){
    ///     multiple(val);
    /// };
    /// </summary>
    internal sealed class ResumeStoryExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            int multiple = m_Multiple != null ? m_Multiple.Calc().GetInt() : 0;
            if (multiple == 0)
            {
                for (int i = 0; i < m_StoryIds.Count; i++)
                {
                    string storyId = m_StoryIds[i].Calc().ToString();
                    ClientStorySystem.Instance.PauseStory(storyId, false);
                }
            }
            else
            {
                for (int i = 0; i < m_StoryIds.Count; i++)
                {
                    string storyId = m_StoryIds[i].Calc().ToString();
                    ClientStorySystem.Instance.PauseStories(storyId, false);
                }
            }
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            int num = funcData.GetParamNum();
            for (int i = 0; i < num; ++i)
            {
                m_StoryIds.Add(Calculator.Load(funcData.GetParam(i)));
            }
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd && cd.GetId() == "multiple" && cd.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(cd.GetParam(0));
                    }
                }
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2)
            {
                var first = statementData.First.AsFunction;
                var second = statementData.Second.AsFunction;
                if (null != first && null != second)
                {
                    Load(first);
                    if (second.GetId() == "multiple" && second.GetParamNum() > 0)
                    {
                        m_Multiple = Calculator.Load(second.GetParam(0));
                    }
                }
            }
            return true;
        }

        private List<IExpression> m_StoryIds = new List<IExpression>();
        private IExpression m_Multiple;
    }

    /// <summary>
    /// setstoryvariable(story_id[, namespace], name, value) - set a variable on another story instance
    /// </summary>
    internal sealed class SetStoryVariableExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: setstoryvariable(story_id[, namespace], name, value)");

            string storyId;
            string ns = string.Empty;
            string name;
            BoxedValue val;

            if (operands.Count > 3)
            {
                // With namespace
                storyId = operands[0].ToString();
                ns = operands[1].ToString();
                name = operands[2].ToString();
                val = operands[3];
            }
            else
            {
                // Without namespace
                storyId = operands[0].ToString();
                name = operands[1].ToString();
                val = operands[2];
            }

            var storyInstance = ClientStorySystem.Instance.GetStory(storyId, ns);
            if (null != storyInstance)
            {
                storyInstance.SetVariable(name, val);
            }
            return BoxedValue.NullObject;
        }
    }
}
