﻿using System;
using System.Collections;
using System.Collections.Generic;
using StorySystem;
using UnityEngine;
namespace StorySystem.CommonValues
{
    /// <summary>
    /// dummy值，用于注册没有对应实现的函数（为了解析需要注册）。
    /// </summary>
    public sealed class DummyValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            DummyValue val = new DummyValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            TryUpdateValue(instance);
        }
        public void Analyze(StoryInstance instance)
        {
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = 0;
        }

        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class NamespaceValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            NamespaceValue val = new NamespaceValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = instance.Namespace;
        }
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class PropGetValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 0) {
                    m_VarName.InitFromDsl(callData.GetParam(0));
                }
                if (m_ParamNum > 1) {
                    m_DefaultValue.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            PropGetValue val = new PropGetValue();
            val.m_ParamNum = m_ParamNum;
            val.m_VarName = m_VarName.Clone();
            val.m_DefaultValue = m_DefaultValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            if (m_ParamNum > 0)
                m_VarName.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 1)
                m_DefaultValue.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance, handler, iterator, args);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            if (m_VarName.HaveValue) {
                m_HaveValue = true;
                string varName = m_VarName.Value;
                if (varName.StartsWith("@") && !varName.StartsWith("@@")) {
                    object val;
                    if (instance.LocalVariables.TryGetValue(varName, out val)) {
                        m_Value = val;
                    } else if (m_ParamNum > 1) {
                        m_Value = m_DefaultValue.Value;
                    } else {
                        m_Value = null;
                    }
                } else if (varName.StartsWith("$")) {
                    if (varName.StartsWith("$$")) {
                        m_Value = iterator;
                    } else if (null != args) {
                        string realName = varName.Substring(1);
                        try {
                            if (char.IsDigit(realName, 0)) {
                                int index = int.Parse(realName);
                                if (index >= 0 && index < args.Length) {
                                    m_Value = args[index];
                                } else if (m_ParamNum > 1) {
                                    m_Value = m_DefaultValue.Value;
                                } else {
                                    m_Value = null;
                                }
                            } else {
                                object val;
                                if (instance.StackVariables.TryGetValue(varName, out val)) {
                                    m_Value = val;
                                } else if (m_ParamNum > 1) {
                                    m_Value = m_DefaultValue.Value;
                                } else {
                                    m_Value = null;
                                }
                            }
                        } catch {
                            if (m_ParamNum > 1) {
                                m_Value = m_DefaultValue.Value;
                            } else {
                                m_Value = null;
                            }
                        }
                    }
                } else {
                    object val;
                    if (null != instance.GlobalVariables && instance.GlobalVariables.TryGetValue(varName, out val)) {
                        m_Value = val;
                    } else if (m_ParamNum > 1) {
                        m_Value = m_DefaultValue.Value;
                    } else {
                        m_Value = null;
                    }
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<string> m_VarName = new StoryValue<string>();
        private IStoryValue m_DefaultValue = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class RandomIntValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Min.InitFromDsl(callData.GetParam(0));
                m_Max.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            RandomIntValue val = new RandomIntValue();
            val.m_Min = m_Min.Clone();
            val.m_Max = m_Max.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Min.Evaluate(instance, handler, iterator, args);
            m_Max.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_Min.HaveValue && m_Max.HaveValue) {
                m_HaveValue = true;
                int min = m_Min.Value;
                int max = m_Max.Value;
                m_Value = GameLibrary.Helper.Random.Next(min, max);
            }
        }
        private IStoryValue<int> m_Min = new StoryValue<int>();
        private IStoryValue<int> m_Max = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class RandomFloatValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            RandomFloatValue val = new RandomFloatValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;

            TryUpdateValue(instance);
        }
        public void Analyze(StoryInstance instance)
        { }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = GameLibrary.Helper.Random.NextFloat();
        }
        private IStoryValue<int> m_Min = new StoryValue<int>();
        private IStoryValue<int> m_Max = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class Vector2Value : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_X.InitFromDsl(callData.GetParam(0));
                m_Y.InitFromDsl(callData.GetParam(1));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector2Value val = new Vector2Value();
            val.m_X = m_X.Clone();
            val.m_Y = m_Y.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;

            m_X.Evaluate(instance, handler, iterator, args);
            m_Y.Evaluate(instance, handler, iterator, args);

            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_X.HaveValue && m_Y.HaveValue) {
                m_HaveValue = true;
                m_Value = new Vector2(m_X.Value, m_Y.Value);
            }
        }
        private IStoryValue<float> m_X = new StoryValue<float>();
        private IStoryValue<float> m_Y = new StoryValue<float>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class Vector3Value : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 3) {
                m_X.InitFromDsl(callData.GetParam(0));
                m_Y.InitFromDsl(callData.GetParam(1));
                m_Z.InitFromDsl(callData.GetParam(2));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector3Value val = new Vector3Value();
            val.m_X = m_X.Clone();
            val.m_Y = m_Y.Clone();
            val.m_Z = m_Z.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_X.Evaluate(instance, handler, iterator, args);
            m_Y.Evaluate(instance, handler, iterator, args);
            m_Z.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_X.HaveValue && m_Y.HaveValue && m_Z.HaveValue) {
                m_HaveValue = true;
                m_Value = new Vector3(m_X.Value, m_Y.Value, m_Z.Value);
            }
        }
        private IStoryValue<float> m_X = new StoryValue<float>();
        private IStoryValue<float> m_Y = new StoryValue<float>();
        private IStoryValue<float> m_Z = new StoryValue<float>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class Vector4Value : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 4) {
                m_X.InitFromDsl(callData.GetParam(0));
                m_Y.InitFromDsl(callData.GetParam(1));
                m_Z.InitFromDsl(callData.GetParam(2));
                m_W.InitFromDsl(callData.GetParam(3));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector4Value val = new Vector4Value();
            val.m_X = m_X.Clone();
            val.m_Y = m_Y.Clone();
            val.m_Z = m_Z.Clone();
            val.m_W = m_W.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_X.Evaluate(instance, handler, iterator, args);
            m_Y.Evaluate(instance, handler, iterator, args);
            m_Z.Evaluate(instance, handler, iterator, args);
            m_W.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_X.HaveValue && m_Y.HaveValue && m_Z.HaveValue && m_W.HaveValue) {
                m_HaveValue = true;
                m_Value = new Vector4(m_X.Value, m_Y.Value, m_Z.Value, m_W.Value);
            }
        }
        private IStoryValue<float> m_X = new StoryValue<float>();
        private IStoryValue<float> m_Y = new StoryValue<float>();
        private IStoryValue<float> m_Z = new StoryValue<float>();
        private IStoryValue<float> m_W = new StoryValue<float>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class QuaternionValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 4) {
                m_X.InitFromDsl(callData.GetParam(0));
                m_Y.InitFromDsl(callData.GetParam(1));
                m_Z.InitFromDsl(callData.GetParam(2));
                m_W.InitFromDsl(callData.GetParam(3));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            QuaternionValue val = new QuaternionValue();
            val.m_X = m_X.Clone();
            val.m_Y = m_Y.Clone();
            val.m_Z = m_Z.Clone();
            val.m_W = m_W.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_X.Evaluate(instance, handler, iterator, args);
            m_Y.Evaluate(instance, handler, iterator, args);
            m_Z.Evaluate(instance, handler, iterator, args);
            m_W.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_X.HaveValue && m_Y.HaveValue && m_Z.HaveValue && m_W.HaveValue) {
                m_HaveValue = true;
                m_Value = new Quaternion(m_X.Value, m_Y.Value, m_Z.Value, m_W.Value);
            }
        }
        private IStoryValue<float> m_X = new StoryValue<float>();
        private IStoryValue<float> m_Y = new StoryValue<float>();
        private IStoryValue<float> m_Z = new StoryValue<float>();
        private IStoryValue<float> m_W = new StoryValue<float>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class EularValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 3) {
                m_X.InitFromDsl(callData.GetParam(0));
                m_Y.InitFromDsl(callData.GetParam(1));
                m_Z.InitFromDsl(callData.GetParam(2));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            EularValue val = new EularValue();
            val.m_X = m_X.Clone();
            val.m_Y = m_Y.Clone();
            val.m_Z = m_Z.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_X.Evaluate(instance, handler, iterator, args);
            m_Y.Evaluate(instance, handler, iterator, args);
            m_Z.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_X.HaveValue && m_Y.HaveValue && m_Z.HaveValue) {
                m_HaveValue = true;
                m_Value = Quaternion.Euler(m_X.Value, m_Y.Value, m_Z.Value);
            }
        }
        private IStoryValue<float> m_X = new StoryValue<float>();
        private IStoryValue<float> m_Y = new StoryValue<float>();
        private IStoryValue<float> m_Z = new StoryValue<float>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class Vector2DistanceValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Pt1.InitFromDsl(callData.GetParam(0));
                m_Pt2.InitFromDsl(callData.GetParam(1));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector2DistanceValue val = new Vector2DistanceValue();
            val.m_Pt1 = m_Pt1.Clone();
            val.m_Pt2 = m_Pt2.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Pt1.Evaluate(instance, handler, iterator, args);
            m_Pt2.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_Pt1.HaveValue && m_Pt2.HaveValue) {
                m_HaveValue = true;
                m_Value = (m_Pt1.Value - m_Pt2.Value).magnitude;
            }
        }
        private IStoryValue<Vector2> m_Pt1 = new StoryValue<Vector2>();
        private IStoryValue<Vector2> m_Pt2 = new StoryValue<Vector2>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class Vector3DistanceValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Pt1.InitFromDsl(callData.GetParam(0));
                m_Pt2.InitFromDsl(callData.GetParam(1));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector3DistanceValue val = new Vector3DistanceValue();
            val.m_Pt1 = m_Pt1.Clone();
            val.m_Pt2 = m_Pt2.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Pt1.Evaluate(instance, handler, iterator, args);
            m_Pt2.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_Pt1.HaveValue && m_Pt2.HaveValue) {
                m_HaveValue = true;
                m_Value = GameLibrary.Geometry.Distance(m_Pt1.Value, m_Pt2.Value);
            }
        }
        private IStoryValue<Vector3> m_Pt1 = new StoryValue<Vector3>();
        private IStoryValue<Vector3> m_Pt2 = new StoryValue<Vector3>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class Vector2To3Value : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_Pt.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector2To3Value val = new Vector2To3Value();
            val.m_Pt = m_Pt.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Pt.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_Pt.HaveValue) {
                m_HaveValue = true;
                m_Value = new Vector3(m_Pt.Value.x, 0, m_Pt.Value.y);
            }
        }
        private IStoryValue<Vector2> m_Pt = new StoryValue<Vector2>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class Vector3To2Value : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_Pt.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector3To2Value val = new Vector3To2Value();
            val.m_Pt = m_Pt.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Pt.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_Pt.HaveValue) {
                m_HaveValue = true;
                m_Value = new Vector2(m_Pt.Value.x, m_Pt.Value.z);
            }
        }
        private IStoryValue<Vector3> m_Pt = new StoryValue<Vector3>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class StringListValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ListString.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            StringListValue val = new StringListValue();
            val.m_ListString = m_ListString.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ListString.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListString.HaveValue) {
                m_HaveValue = true;
                List<string> list = GameLibrary.Converter.ConvertStringList(m_ListString.Value);
                m_Value = new ObjList();
                for (int i = 0; i < list.Count; ++i) {
                    m_Value.Add(list[i]);
                }
            }
        }
        private IStoryValue<string> m_ListString = new StoryValue<string>();
        private bool m_HaveValue;
        private ObjList m_Value;
    }
    internal sealed class IntListValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ListString.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            IntListValue val = new IntListValue();
            val.m_ListString = m_ListString.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ListString.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListString.HaveValue) {
                m_HaveValue = true;
                List<int> list = GameLibrary.Converter.ConvertNumericList<int>(m_ListString.Value);
                m_Value = new ObjList();
                for (int i = 0; i < list.Count; ++i) {
                    m_Value.Add(list[i]);
                }
            }
        }
        private IStoryValue<string> m_ListString = new StoryValue<string>();
        private bool m_HaveValue;
        private ObjList m_Value;
    }
    internal sealed class FloatListValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ListString.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            FloatListValue val = new FloatListValue();
            val.m_ListString = m_ListString.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ListString.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListString.HaveValue) {
                m_HaveValue = true;
                List<float> list = GameLibrary.Converter.ConvertNumericList<float>(m_ListString.Value);
                m_Value = new ObjList();
                for (int i = 0; i < list.Count; ++i) {
                    m_Value.Add(list[i]);
                }
            }
        }
        private IStoryValue<string> m_ListString = new StoryValue<string>();
        private bool m_HaveValue;
        private ObjList m_Value;
    }
    internal sealed class Vector2ListValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ListString.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector2ListValue val = new Vector2ListValue();
            val.m_ListString = m_ListString.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ListString.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListString.HaveValue) {
                m_HaveValue = true;
                List<Vector2> list = GameLibrary.Converter.ConvertVector2DList(m_ListString.Value);
                m_Value = new ObjList();
                for (int i = 0; i < list.Count; ++i) {
                    m_Value.Add(list[i]);
                }
            }
        }
        private IStoryValue<string> m_ListString = new StoryValue<string>();
        private bool m_HaveValue;
        private ObjList m_Value;
    }
    internal sealed class Vector3ListValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ListString.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            Vector3ListValue val = new Vector3ListValue();
            val.m_ListString = m_ListString.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ListString.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListString.HaveValue) {
                m_HaveValue = true;
                List<Vector3> list = GameLibrary.Converter.ConvertVector3DList(m_ListString.Value);
                m_Value = new ObjList();
                for (int i = 0; i < list.Count; ++i) {
                    m_Value.Add(list[i]);
                }
            }
        }
        private IStoryValue<string> m_ListString = new StoryValue<string>();
        private bool m_HaveValue;
        private ObjList m_Value;
    }
    internal sealed class ListValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {

                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    Dsl.ISyntaxComponent arg = callData.GetParam(i);
                    StoryValue val = new StoryValue();
                    val.InitFromDsl(arg);
                    m_List.Add(val);
                }
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            ListValue val = new ListValue();
            for (int i = 0; i < m_List.Count; i++) {
                val.m_List.Add(m_List[i]);
            }
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            for (int i = 0; i < m_List.Count; i++) {
                m_List[i].Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            bool canCalc = true;
            for (int i = 0; i < m_List.Count; i++) {
                if (!m_List[i].HaveValue) {
                    canCalc = false;
                    break;
                }
            }
            if (canCalc) {
                m_HaveValue = true;
                m_Value = new ObjList();
                for (int i = 0; i < m_List.Count; i++) {
                    m_Value.Add(m_List[i].Value);
                }
            }
        }
        private List<IStoryValue> m_List = new List<IStoryValue>();
        private bool m_HaveValue;
        private ObjList m_Value;
    }
    internal sealed class RandomFromListValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 0) {
                    m_ListValue.InitFromDsl(callData.GetParam(0));
                }
                if (m_ParamNum > 1) {
                    m_DefaultValue.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            RandomFromListValue val = new RandomFromListValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ListValue = m_ListValue.Clone();
            val.m_DefaultValue = m_DefaultValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            if (m_ParamNum > 0)
                m_ListValue.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 1)
                m_DefaultValue.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListValue.HaveValue) {
                m_HaveValue = true;
                IList listValue = m_ListValue.Value;
                int ct = listValue.Count;
                int ix = GameLibrary.Helper.Random.Next(ct);
                if (ix >= 0 && ix < ct) {
                    m_Value = listValue[ix];
                } else if (ct > 0) {
                    m_Value = listValue[0];
                } else if (m_ParamNum > 1) {
                    m_Value = m_DefaultValue.Value;
                } else {
                    m_Value = null;
                }
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<IList> m_ListValue = new StoryValue<IList>();
        private IStoryValue m_DefaultValue = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class ListGetValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {

                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 1) {
                    m_ListValue.InitFromDsl(callData.GetParam(0));
                    m_IndexValue.InitFromDsl(callData.GetParam(1));
                    if (m_ParamNum > 2) {
                        m_DefaultValue.InitFromDsl(callData.GetParam(2));
                    }
                    TryUpdateValue();
                }
            }
        }
        public IStoryValue Clone()
        {
            ListGetValue val = new ListGetValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ListValue = m_ListValue.Clone();
            val.m_IndexValue = m_IndexValue.Clone();
            val.m_DefaultValue = m_DefaultValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            if (m_ParamNum > 1) {
                m_ListValue.Evaluate(instance, handler, iterator, args);
                m_IndexValue.Evaluate(instance, handler, iterator, args);
            }
            if (m_ParamNum > 2) {
                m_DefaultValue.Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListValue.HaveValue && m_IndexValue.HaveValue) {
                m_HaveValue = true;
                IList listValue = m_ListValue.Value;
                int ix = m_IndexValue.Value;
                int ct = listValue.Count;
                if (ix >= 0 && ix < ct) {
                    m_Value = listValue[ix];
                } else if (ct > 0) {
                    m_Value = listValue[ct - 1];
                } else if (m_ParamNum > 2) {
                    m_Value = m_DefaultValue.Value;
                } else {
                    m_Value = null;
                }
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<IList> m_ListValue = new StoryValue<IList>();
        private IStoryValue<int> m_IndexValue = new StoryValue<int>();
        private IStoryValue m_DefaultValue = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class ListSizeValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ListValue.InitFromDsl(callData.GetParam(0));
                TryUpdateValue();
            }
        }
        public IStoryValue Clone()
        {
            ListSizeValue val = new ListSizeValue();
            val.m_ListValue = m_ListValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ListValue.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListValue.HaveValue) {
                m_HaveValue = true;
                IList listValue = m_ListValue.Value;
                int ct = listValue.Count;
                m_Value = ct;
            }
        }
        private IStoryValue<IList> m_ListValue = new StoryValue<IList>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class ListIndexOfValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {

                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 1) {
                    m_ListValue.InitFromDsl(callData.GetParam(0));
                    m_IndexOfValue.InitFromDsl(callData.GetParam(1));
                    TryUpdateValue();
                }
            }
        }
        public IStoryValue Clone()
        {
            ListIndexOfValue val = new ListIndexOfValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ListValue = m_ListValue.Clone();
            val.m_IndexOfValue = m_IndexOfValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            if (m_ParamNum > 1) {
                m_ListValue.Evaluate(instance, handler, iterator, args);
                m_IndexOfValue.Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_ListValue.HaveValue && m_IndexOfValue.HaveValue) {
                m_HaveValue = true;
                IList listValue = m_ListValue.Value;
                object val = m_IndexOfValue.Value;
                m_Value = listValue.IndexOf(val);
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<IList> m_ListValue = new StoryValue<IList>();
        private IStoryValue m_IndexOfValue = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class RandVector3Value : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Pt.InitFromDsl(callData.GetParam(0));
                m_Radius.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            RandVector3Value val = new RandVector3Value();
            val.m_Pt = m_Pt.Clone();
            val.m_Radius = m_Radius.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Pt.Evaluate(instance, handler, iterator, args);
            m_Radius.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_Pt.HaveValue) {
                m_HaveValue = true;
                float r = m_Radius.Value;
                Vector3 pt = m_Pt.Value;
                float deltaX = (GameLibrary.Helper.Random.NextFloat() - 0.5f) * r;
                float deltaZ = (GameLibrary.Helper.Random.NextFloat() - 0.5f) * r;
                m_Value = new Vector3(pt.x + deltaX, pt.y, pt.z + deltaZ);
            }
        }
        private IStoryValue<Vector3> m_Pt = new StoryValue<Vector3>();
        private IStoryValue<float> m_Radius = new StoryValue<float>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class RandVector2Value : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Pt.InitFromDsl(callData.GetParam(0));
                m_Radius.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            RandVector2Value val = new RandVector2Value();
            val.m_Pt = m_Pt.Clone();
            val.m_Radius = m_Radius.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Pt.Evaluate(instance, handler, iterator, args);
            m_Radius.Evaluate(instance, handler, iterator, args);
            TryUpdateValue();
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private void TryUpdateValue()
        {
            if (m_Pt.HaveValue) {
                m_HaveValue = true;
                float r = m_Radius.Value;
                Vector2 pt = m_Pt.Value;
                float deltaX = (GameLibrary.Helper.Random.NextFloat() - 0.5f) * r;
                float deltaZ = (GameLibrary.Helper.Random.NextFloat() - 0.5f) * r;
                m_Value = new Vector2(pt.x + deltaX, pt.y + deltaZ);
            }
        }
        private IStoryValue<Vector2> m_Pt = new StoryValue<Vector2>();
        private IStoryValue<float> m_Radius = new StoryValue<float>();
        private bool m_HaveValue;
        private object m_Value;
    }
}
