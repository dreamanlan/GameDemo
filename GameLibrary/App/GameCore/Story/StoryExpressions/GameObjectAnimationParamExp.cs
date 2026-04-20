using System;
using System.Collections.Generic;
using UnityEngine;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// gameobjectanimationparam(obj) { int(key, value); float(key, value); bool(key, value); trigger(key, value); ... }
    /// - set animator parameters on a GameObject
    /// </summary>
    internal sealed class GameObjectAnimationParamExp : AbstractExpression
    {
        public override bool IsAsync { get { return false; } }

        protected override BoxedValue DoCalc()
        {
            if (m_ObjExp == null)
                return BoxedValue.NullObject;

            GameObject obj = GetGameObject(m_ObjExp.Calc());
            if (null == obj)
                return BoxedValue.NullObject;

            UnityEngine.Animator animator = obj.GetComponentInChildren<UnityEngine.Animator>();
            if (null != animator)
            {
                for (int i = 0; i < m_Params.Count; ++i)
                {
                    var param = m_Params[i];
                    string type = param.Type;
                    string key = param.Key.Calc().ToString();
                    var val = param.Value.Calc();

                    if (type == "int")
                    {
                        int v = val.GetInt();
                        animator.SetInteger(key, v);
                    }
                    else if (type == "float")
                    {
                        float v = (float)val.GetFloat();
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

            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            // Parse: gameobjectanimationparam(obj) { ... }
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
                        m_ObjExp = Calculator.Load(callData.GetParam(0));
                    }
                }
            }

            // Parse block content
            for (int i = 1; i < statementData.GetFunctionNum(); ++i)
            {
                Dsl.FunctionData callData = statementData.Functions[i].AsFunction;
                if (null != callData)
                {
                    string id = callData.GetId();
                    if (id == "int" || id == "float" || id == "bool" || id == "trigger")
                    {
                        LoadParam(id, callData);
                    }
                }
            }

            return true;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            // Handle: gameobjectanimationparam(obj) without block or with block in FunctionData
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
                callData = funcData.LowerOrderFunction;

            if (null != callData && callData.HaveParam())
            {
                if (callData.GetParamNum() > 0)
                {
                    m_ObjExp = Calculator.Load(callData.GetParam(0));
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
                        string id = callData2.GetId();
                        if (id == "int" || id == "float" || id == "bool" || id == "trigger")
                        {
                            LoadParam(id, callData2);
                        }
                    }
                }
            }

            return true;
        }

        private void LoadParam(string type, Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                ParamInfo info = new ParamInfo();
                info.Type = type;
                info.Key = Calculator.Load(callData.GetParam(0));
                info.Value = Calculator.Load(callData.GetParam(1));
                m_Params.Add(info);
            }
        }

        private GameObject GetGameObject(BoxedValue val)
        {
            string path = val.IsString ? val.StringVal : null;
            if (null != path)
            {
                return UnityEngine.GameObject.Find(path);
            }
            else
            {
                GameObject obj = val.ObjectVal as GameObject;
                if (null == obj)
                {
                    try
                    {
                        int objId = val.GetInt();
                        obj = SceneSystem.Instance.GetGameObject(objId);
                    }
                    catch
                    {
                        obj = null;
                    }
                }
                return obj;
            }
        }

        private class ParamInfo
        {
            internal string Type;
            internal IExpression Key;
            internal IExpression Value;
        }

        private IExpression m_ObjExp = null;
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }
}
