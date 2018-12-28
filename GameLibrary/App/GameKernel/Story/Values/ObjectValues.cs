using System;
using System.Collections.Generic;
using StorySystem;
using GameLibrary;
using UnityEngine;
namespace GameLibrary.Story.Values
{
    internal sealed class UnitId2ObjIdValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            UnitId2ObjIdValue val = new UnitId2ObjIdValue();
            val.m_UnitId = m_UnitId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_UnitId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_UnitId.HaveValue) {
                int unitId = m_UnitId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
                if (null != obj) {
                    m_Value = obj.GetId();
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class ObjId2UnitIdValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            ObjId2UnitIdValue val = new ObjId2UnitIdValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.GetUnitId();
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetPositionValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetPositionValue val = new GetPositionValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);
        
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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt;
                } else {
                    m_Value = Vector3.zero;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetPositionXValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetPositionXValue val = new GetPositionXValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);
        
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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt.x;
                } else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetPositionYValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetPositionYValue val = new GetPositionYValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);
        
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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt.y;
                } else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetPositionZValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetPositionZValue val = new GetPositionZValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);
        
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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt.z;
                } else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetRotationValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetRotationValue val = new GetRotationValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt;
                } else {
                    m_Value = Vector3.zero;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetRotationXValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetRotationXValue val = new GetRotationXValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt.x;
                } else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetRotationYValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetRotationYValue val = new GetRotationYValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt.y;
                } else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetRotationZValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetRotationZValue val = new GetRotationZValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_LocalOrWorld.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt.z;
                } else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<object> m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetScaleValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetScaleValue val = new GetScaleValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt;
                } else {
                    m_Value = new Vector3(1, 1, 1);
                }
            }
        }

        private IStoryValue<object> m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetScaleXValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetScaleXValue val = new GetScaleXValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.x;
                } else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryValue<object> m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetScaleYValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetScaleYValue val = new GetScaleYValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.y;
                } else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryValue<object> m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetScaleZValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetScaleZValue val = new GetScaleZValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);

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
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objVal = m_ObjId.Value;
                string objPath = objVal as string;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal as UnityEngine.GameObject;
                    if (null == obj) {
                        try {
                            int id = (int)objVal;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.z;
                } else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryValue<object> m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetCampValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetCampValue val = new GetCampValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.GetCampId();
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class IsEnemyValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Camp1.InitFromDsl(callData.GetParam(0));
                m_Camp2.InitFromDsl(callData.GetParam(1));
                TryUpdateValue(null);
            }
        }
        public IStoryValue<object> Clone()
        {
            IsEnemyValue val = new IsEnemyValue();
            val.m_Camp1 = m_Camp1.Clone();
            val.m_Camp2 = m_Camp2.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_Camp1.Evaluate(instance, iterator, args);
                m_Camp2.Evaluate(instance, iterator, args);
            }
        
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
            if (m_Camp1.HaveValue && m_Camp2.HaveValue) {
                int camp1 = m_Camp1.Value;
                int camp2 = m_Camp2.Value;
                m_HaveValue = true;
                m_Value = (EntityInfo.GetRelation(camp1, camp2) == CharacterRelation.RELATION_ENEMY ? 1 : 0);
            }
        }
        private IStoryValue<int> m_Camp1 = new StoryValue<int>();
        private IStoryValue<int> m_Camp2 = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class IsFriendValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Camp1.InitFromDsl(callData.GetParam(0));
                m_Camp2.InitFromDsl(callData.GetParam(1));
                TryUpdateValue(null);
            }
        }
        public IStoryValue<object> Clone()
        {
            IsFriendValue val = new IsFriendValue();
            val.m_Camp1 = m_Camp1.Clone();
            val.m_Camp2 = m_Camp2.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_Camp1.Evaluate(instance, iterator, args);
                m_Camp2.Evaluate(instance, iterator, args);
            }
        
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
            if (m_Camp1.HaveValue && m_Camp2.HaveValue) {
                int camp1 = m_Camp1.Value;
                int camp2 = m_Camp2.Value;
                m_HaveValue = true;
                m_Value = (EntityInfo.GetRelation(camp1, camp2) == CharacterRelation.RELATION_FRIEND ? 1 : 0);
            }
        }
        private IStoryValue<int> m_Camp1 = new StoryValue<int>();
        private IStoryValue<int> m_Camp2 = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetHpValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetHpValue val = new GetHpValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.Hp;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetMaxHpValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetMaxHpValue val = new GetMaxHpValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.HpMax;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetEnergyValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetEnergyValue val = new GetEnergyValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.Energy;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetMaxEnergyValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetMaxEnergyValue val = new GetMaxEnergyValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.EnergyMax;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetSpeedValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetSpeedValue val = new GetSpeedValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.Speed;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetLevelValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetLevelValue val = new GetLevelValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.Level;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetExpValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            GetExpValue val = new GetExpValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.Exp;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetAttrValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 1) {
                    m_ObjId.InitFromDsl(callData.GetParam(0));
                    m_AttrId.InitFromDsl(callData.GetParam(1));
                }
                if (m_ParamNum > 2) {
                    m_DefaultValue.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryValue<object> Clone()
        {
            GetAttrValue val = new GetAttrValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_AttrId = m_AttrId.Clone();
            val.m_DefaultValue = m_DefaultValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                if (m_ParamNum > 1) {
                    m_ObjId.Evaluate(instance, iterator, args);
                    m_AttrId.Evaluate(instance, iterator, args);
                }
                if (m_ParamNum > 2) {
                    m_DefaultValue.Evaluate(instance, iterator, args);
                }
            }

            if (m_ParamNum > 1) {
            }
            if (m_ParamNum > 2) {
            }
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public object Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue && m_AttrId.HaveValue) {
                int objId = m_ObjId.Value;
                int attrId = m_AttrId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.Property.GetLong((CharacterPropertyEnum)attrId);
                } else if (m_ParamNum > 2) {
                    m_Value = m_DefaultValue.Value;
                } else {
                    m_Value = null;
                }
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<int> m_AttrId = new StoryValue<int>();
        private IStoryValue<object> m_DefaultValue = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class CalcOffsetValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 3) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
                m_Offset.InitFromDsl(callData.GetParam(2));
            }
        }
        public IStoryValue<object> Clone()
        {
            CalcOffsetValue val = new CalcOffsetValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_TargetId = m_TargetId.Clone();
            val.m_Offset = m_Offset.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
                m_TargetId.Evaluate(instance, iterator, args);
                m_Offset.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue && m_TargetId.HaveValue) {
                int objId = m_ObjId.Value;
                int targetId = m_TargetId.Value;
                Vector3 offset = m_Offset.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                EntityInfo target = SceneSystem.Instance.GetEntityById(targetId);
                if (null != obj && null != target) {
                    Vector2 srcPos = new Vector2(obj.GetMovementStateInfo().PositionX, obj.GetMovementStateInfo().PositionZ);
                    float y = obj.GetMovementStateInfo().PositionY;
                    Vector2 targetPos = new Vector2(target.GetMovementStateInfo().PositionX, target.GetMovementStateInfo().PositionZ);
                    float radian = Geometry.GetYRadian(srcPos, targetPos);
                    Vector2 newPos = srcPos + Geometry.GetRotate(new Vector2(offset.x, offset.z), radian);
                    m_Value = new Vector3(newPos.x, y + offset.y, newPos.y);
                } else if (null != obj) {
                    Vector2 srcPos = new Vector2(obj.GetMovementStateInfo().PositionX, obj.GetMovementStateInfo().PositionZ);
                    float y = obj.GetMovementStateInfo().PositionY;
                    float radian = Geometry.DegreeToRadian(obj.View.Actor.transform.localEulerAngles.y);
                    Vector2 newPos = srcPos + Geometry.GetRotate(new Vector2(offset.x, offset.z), radian);
                    m_Value = new Vector3(newPos.x, y + offset.y, newPos.y);
                } else {
                    m_Value = Vector3.zero;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<int> m_TargetId = new StoryValue<int>();
        private IStoryValue<Vector3> m_Offset = new StoryValue<Vector3>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class CalcDirValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            CalcDirValue val = new CalcDirValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_TargetId = m_TargetId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
                m_TargetId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue && m_TargetId.HaveValue) {
                int objId = m_ObjId.Value;
                int targetId = m_TargetId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                EntityInfo target = SceneSystem.Instance.GetEntityById(targetId);
                if (null != obj && null != target) {
                    Vector2 srcPos = new Vector2(obj.GetMovementStateInfo().PositionX, obj.GetMovementStateInfo().PositionZ);
                    Vector2 targetPos = new Vector2(target.GetMovementStateInfo().PositionX, target.GetMovementStateInfo().PositionZ);
                    m_Value = Geometry.GetYRadian(srcPos, targetPos);
                } else if (null != obj) {
                    m_Value = Geometry.DegreeToRadian(obj.View.Actor.transform.localEulerAngles.y);
                } else {
                    m_Value = 0.0f;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<int> m_TargetId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class IsControlByStoryValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            IsControlByStoryValue val = new IsControlByStoryValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.IsControlByStory;
                } else {
                    m_Value = false;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class ObjGetNpcTypeValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue<object> Clone()
        {
            ObjGetNpcTypeValue val = new ObjGetNpcTypeValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, iterator, args);
            }
        
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
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    m_Value = obj.EntityType;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class ObjGetAiParamValue : IStoryValue<object>
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Index.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue<object> Clone()
        {
            ObjGetAiParamValue val = new ObjGetAiParamValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_Index = m_Index.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, iterator, args);
            m_Index.Evaluate(instance, iterator, args);
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
            if (m_ObjId.HaveValue) {
                int objId = m_ObjId.Value;
                int index = m_Index.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
                if (null != obj) {
                    var aiState = obj.GetAiStateInfo();
                    if (index >= 0 && index < aiState.AiParam.Length) {
                        m_Value = aiState.AiParam[index];
                    } else {
                        m_Value = null;
                    }
                } else {
                    m_Value = null;
                }
            }
        }

        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<int> m_Index = new StoryValue<int>();
        private bool m_HaveValue;
        private object m_Value;
    }
}
