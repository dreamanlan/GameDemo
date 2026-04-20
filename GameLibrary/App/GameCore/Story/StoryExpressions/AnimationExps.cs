using System;
using System.Collections.Generic;
using UnityEngine;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// gameobjectanimation(obj, anim_name) - play an animation on a GameObject
    /// </summary>
    internal sealed class GameObjectAnimationExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: gameobjectanimation(obj, anim_name)");

            GameObject obj = AnimationExpHelpers.GetGameObject(operands[0]);
            string anim = operands[1].ToString();
            if (null != obj)
            {
                EntityViewModel view = SceneSystem.Instance.GetEntityView(obj);
                if (null != view)
                {
                    view.PlayAnimation(anim);
                }
                else
                {
                    var animators = obj.GetComponentsInChildren<Animator>();
                    if (null != animators)
                    {
                        for (int i = 0; i < animators.Length; ++i)
                        {
                            animators[i].Play(anim);
                        }
                    }
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// setanimatorparam(obj, type, name, value) - set an animator parameter
    /// Types: int, float, bool, trigger
    /// </summary>
    internal sealed class SetAnimatorParamExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 4)
                throw new Exception("Expected: setanimatorparam(obj, type, name, value)");

            GameObject obj = AnimationExpHelpers.GetGameObject(operands[0]);
            string type = operands[1].ToString();
            string key = operands[2].ToString();
            if (null == obj) return BoxedValue.NullObject;

            Animator animator = obj.GetComponentInChildren<Animator>();
            if (null != animator)
            {
                if (type == "int")
                {
                    int v = operands[3].GetInt();
                    animator.SetInteger(key, v);
                }
                else if (type == "float")
                {
                    float v = operands[3].GetFloat();
                    animator.SetFloat(key, v);
                }
                else if (type == "bool")
                {
                    bool v = operands[3].GetBool();
                    animator.SetBool(key, v);
                }
                else if (type == "trigger")
                {
                    string v = operands[3].ToString();
                    if (v == "false")
                    {
                        animator.ResetTrigger(key);
                    }
                    else
                    {
                        animator.SetTrigger(key);
                    }
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npcanimationparam(unit_id) { int(key, value); float(key, value); bool(key, value); trigger(key, value); }
    /// - set animator parameters on an NPC
    /// </summary>
    internal sealed class NpcAnimationParamExp : AbstractExpression
    {
        public override bool IsAsync { get { return false; } }

        protected override BoxedValue DoCalc()
        {
            int unitId = m_UnitIdExp.Calc().GetInt();
            GameObject obj = SceneSystem.Instance.GetGameObjectByUnitId(unitId);
            if (null != obj)
            {
                Animator animator = obj.GetComponentInChildren<Animator>();
                if (null != animator)
                {
                    ApplyParams(animator);
                }
            }
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            // Parse: npcanimationparam(unit_id) { ... }
            Dsl.FunctionData funcData = statementData.First.AsFunction;
            if (null != funcData)
            {
                Dsl.FunctionData callData = funcData;
                if (funcData.IsHighOrder)
                    callData = funcData.LowerOrderFunction;

                if (null != callData && callData.HaveParam())
                {
                    if (callData.GetParamNum() > 0)
                    {
                        m_UnitIdExp = Calculator.Load(callData.GetParam(0));
                    }
                }
            }

            // Parse block content
            for (int i = 1; i < statementData.GetFunctionNum(); ++i)
            {
                Dsl.FunctionData callData = statementData.Functions[i].AsFunction;
                if (null != callData)
                {
                    LoadParam(callData);
                }
            }

            return true;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            // Handle: npcanimationparam(unit_id) without block
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
                callData = funcData.LowerOrderFunction;

            if (null != callData && callData.HaveParam())
            {
                if (callData.GetParamNum() > 0)
                {
                    m_UnitIdExp = Calculator.Load(callData.GetParam(0));
                }
            }

            // Also parse block content if present
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData callData2 = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != callData2)
                    {
                        LoadParam(callData2);
                    }
                }
            }

            return true;
        }

        private void LoadParam(Dsl.FunctionData callData)
        {
            string type = callData.GetId();
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                ParamInfo info = new ParamInfo();
                info.Type = type;
                info.KeyExp = Calculator.Load(callData.GetParam(0));
                info.ValueExp = Calculator.Load(callData.GetParam(1));
                m_Params.Add(info);
            }
        }

        private void ApplyParams(Animator animator)
        {
            for (int i = 0; i < m_Params.Count; ++i)
            {
                var param = m_Params[i];
                string type = param.Type;
                string key = param.KeyExp.Calc().ToString();
                var val = param.ValueExp.Calc();

                if (type == "int")
                {
                    int v = val.GetInt();
                    animator.SetInteger(key, v);
                }
                else if (type == "float")
                {
                    float v = val.GetFloat();
                    animator.SetFloat(key, v);
                }
                else if (type == "bool")
                {
                    bool v = val.GetBool();
                    animator.SetBool(key, v);
                }
                else if (type == "trigger")
                {
                    string v = val.ToString();
                    if (v == "false")
                    {
                        animator.ResetTrigger(key);
                    }
                    else
                    {
                        animator.SetTrigger(key);
                    }
                }
            }
        }

        private class ParamInfo
        {
            internal string Type;
            internal IExpression KeyExp;
            internal IExpression ValueExp;
        }

        private IExpression m_UnitIdExp;
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }

    /// <summary>
    /// objanimationparam(obj_id) { int(key, value); float(key, value); bool(key, value); trigger(key, value); }
    /// - set animator parameters on a GameObject
    /// </summary>
    internal sealed class ObjAnimationParamExp : AbstractExpression
    {
        public override bool IsAsync { get { return false; } }

        protected override BoxedValue DoCalc()
        {
            int objId = m_ObjIdExp.Calc().GetInt();
            GameObject obj = SceneSystem.Instance.GetGameObject(objId);
            if (null != obj)
            {
                Animator animator = obj.GetComponentInChildren<Animator>();
                if (null != animator)
                {
                    ApplyParams(animator);
                }
            }
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            // Parse: objanimationparam(obj_id) { ... }
            Dsl.FunctionData funcData = statementData.First.AsFunction;
            if (null != funcData)
            {
                Dsl.FunctionData callData = funcData;
                if (funcData.IsHighOrder)
                    callData = funcData.LowerOrderFunction;

                if (null != callData && callData.HaveParam())
                {
                    if (callData.GetParamNum() > 0)
                    {
                        m_ObjIdExp = Calculator.Load(callData.GetParam(0));
                    }
                }
            }

            // Parse block content
            for (int i = 1; i < statementData.GetFunctionNum(); ++i)
            {
                Dsl.FunctionData callData = statementData.Functions[i].AsFunction;
                if (null != callData)
                {
                    LoadParam(callData);
                }
            }

            return true;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            // Handle: objanimationparam(obj_id) without block
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
                callData = funcData.LowerOrderFunction;

            if (null != callData && callData.HaveParam())
            {
                if (callData.GetParamNum() > 0)
                {
                    m_ObjIdExp = Calculator.Load(callData.GetParam(0));
                }
            }

            // Also parse block content if present
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData callData2 = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != callData2)
                    {
                        LoadParam(callData2);
                    }
                }
            }

            return true;
        }

        private void LoadParam(Dsl.FunctionData callData)
        {
            string type = callData.GetId();
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                ParamInfo info = new ParamInfo();
                info.Type = type;
                info.KeyExp = Calculator.Load(callData.GetParam(0));
                info.ValueExp = Calculator.Load(callData.GetParam(1));
                m_Params.Add(info);
            }
        }

        private void ApplyParams(Animator animator)
        {
            for (int i = 0; i < m_Params.Count; ++i)
            {
                var param = m_Params[i];
                string type = param.Type;
                string key = param.KeyExp.Calc().ToString();
                var val = param.ValueExp.Calc();

                if (type == "int")
                {
                    int v = val.GetInt();
                    animator.SetInteger(key, v);
                }
                else if (type == "float")
                {
                    float v = val.GetFloat();
                    animator.SetFloat(key, v);
                }
                else if (type == "bool")
                {
                    bool v = val.GetBool();
                    animator.SetBool(key, v);
                }
                else if (type == "trigger")
                {
                    string v = val.ToString();
                    if (v == "false")
                    {
                        animator.ResetTrigger(key);
                    }
                    else
                    {
                        animator.SetTrigger(key);
                    }
                }
            }
        }

        private class ParamInfo
        {
            internal string Type;
            internal IExpression KeyExp;
            internal IExpression ValueExp;
        }

        private IExpression m_ObjIdExp;
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }

    internal static class AnimationExpHelpers
    {
        internal static GameObject GetGameObject(BoxedValue val)
        {
            string path = val.IsString ? val.StringVal : null;
            if (null != path)
            {
                return GameObject.Find(path);
            }
            else
            {
                GameObject obj = val.ObjectVal as GameObject;
                if (null == obj)
                {
                    try
                    {
                        int id = val.GetInt();
                        obj = SceneSystem.Instance.GetGameObject(id);
                    }
                    catch { }
                }
                return obj;
            }
        }
    }
}
