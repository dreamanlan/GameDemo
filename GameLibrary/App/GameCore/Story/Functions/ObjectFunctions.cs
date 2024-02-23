using System;
using System.Collections.Generic;
using StoryScript;
using GameLibrary;
using UnityEngine;
namespace GameLibrary.Story.Functions
{
    internal sealed class UnitId2ObjIdFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            UnitId2ObjIdFunction val = new UnitId2ObjIdFunction();
            val.m_UnitId = m_UnitId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_UnitId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
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
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_UnitId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class ObjId2UnitIdFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            ObjId2UnitIdFunction val = new ObjId2UnitIdFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                    m_Value = obj.GetUnitId();
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetPositionFunction val = new GetPositionFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = Vector3.zero;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionXFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetPositionXFunction val = new GetPositionXFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionYFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetPositionYFunction val = new GetPositionYFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionZFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetPositionZFunction val = new GetPositionZFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetRotationFunction val = new GetRotationFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = Vector3.zero;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationXFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetRotationXFunction val = new GetRotationXFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationYFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetRotationYFunction val = new GetRotationYFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationZFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            GetRotationZFunction val = new GetRotationZFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
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
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjId = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleFunction val = new GetScaleFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt;
                }
                else {
                    m_Value = new Vector3(1, 1, 1);
                }
            }
        }

        private IStoryFunction m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleXFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleXFunction val = new GetScaleXFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.x;
                }
                else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryFunction m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleYFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleYFunction val = new GetScaleYFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.y;
                }
                else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryFunction m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleZFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleZFunction val = new GetScaleZFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = SceneSystem.Instance.GetGameObject(id);
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.z;
                }
                else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryFunction m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetCampFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetCampFunction val = new GetCampFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                    m_Value = obj.GetCampId();
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsEnemyFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Camp1.InitFromDsl(callData.GetParam(0));
                m_Camp2.InitFromDsl(callData.GetParam(1));
                TryUpdateValue(null);
            }
        }
        public IStoryFunction Clone()
        {
            IsEnemyFunction val = new IsEnemyFunction();
            val.m_Camp1 = m_Camp1.Clone();
            val.m_Camp2 = m_Camp2.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_Camp1.Evaluate(instance, handler, iterator, args);
                m_Camp2.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
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
        private IStoryFunction<int> m_Camp1 = new StoryValue<int>();
        private IStoryFunction<int> m_Camp2 = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsFriendFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_Camp1.InitFromDsl(callData.GetParam(0));
                m_Camp2.InitFromDsl(callData.GetParam(1));
                TryUpdateValue(null);
            }
        }
        public IStoryFunction Clone()
        {
            IsFriendFunction val = new IsFriendFunction();
            val.m_Camp1 = m_Camp1.Clone();
            val.m_Camp2 = m_Camp2.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_Camp1.Evaluate(instance, handler, iterator, args);
                m_Camp2.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
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
        private IStoryFunction<int> m_Camp1 = new StoryValue<int>();
        private IStoryFunction<int> m_Camp2 = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetHpFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetHpFunction val = new GetHpFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                    m_Value = obj.Hp;
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetMaxHpFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetMaxHpFunction val = new GetMaxHpFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                    m_Value = obj.HpMax;
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetEnergyFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetEnergyFunction val = new GetEnergyFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetMaxEnergyFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetMaxEnergyFunction val = new GetMaxEnergyFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetSpeedFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetSpeedFunction val = new GetSpeedFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetLevelFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetLevelFunction val = new GetLevelFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetExpFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetExpFunction val = new GetExpFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetAttrFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
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
        public IStoryFunction Clone()
        {
            GetAttrFunction val = new GetAttrFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_AttrId = m_AttrId.Clone();
            val.m_DefaultValue = m_DefaultValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                if (m_ParamNum > 1) {
                    m_ObjId.Evaluate(instance, handler, iterator, args);
                    m_AttrId.Evaluate(instance, handler, iterator, args);
                }
                if (m_ParamNum > 2) {
                    m_DefaultValue.Evaluate(instance, handler, iterator, args);
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
        public BoxedValue Value
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
                }
                else if (m_ParamNum > 2) {
                    m_Value = m_DefaultValue.Value;
                }
                else {
                    m_Value = BoxedValue.NullObject;
                }
            }
        }
        private int m_ParamNum = 0;
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_AttrId = new StoryValue<int>();
        private IStoryFunction m_DefaultValue = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CalcOffsetFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 3) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
                m_Offset.InitFromDsl(callData.GetParam(2));
            }
        }
        public IStoryFunction Clone()
        {
            CalcOffsetFunction val = new CalcOffsetFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_TargetId = m_TargetId.Clone();
            val.m_Offset = m_Offset.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
                m_TargetId.Evaluate(instance, handler, iterator, args);
                m_Offset.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
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
                var gobj = SceneSystem.Instance.GetGameObject(objId);
                if (null != obj && null != target) {
                    Vector2 srcPos = new Vector2(obj.GetMovementStateInfo().PositionX, obj.GetMovementStateInfo().PositionZ);
                    float y = obj.GetMovementStateInfo().PositionY;
                    Vector2 targetPos = new Vector2(target.GetMovementStateInfo().PositionX, target.GetMovementStateInfo().PositionZ);
                    float radian = Geometry.GetYRadian(srcPos, targetPos);
                    Vector2 newPos = srcPos + Geometry.GetRotate(new Vector2(offset.x, offset.z), radian);
                    m_Value = new Vector3(newPos.x, y + offset.y, newPos.y);
                }
                else if (null != obj && null != gobj) {
                    Vector2 srcPos = new Vector2(obj.GetMovementStateInfo().PositionX, obj.GetMovementStateInfo().PositionZ);
                    float y = obj.GetMovementStateInfo().PositionY;
                    float radian = Geometry.DegreeToRadian(gobj.transform.localEulerAngles.y);
                    Vector2 newPos = srcPos + Geometry.GetRotate(new Vector2(offset.x, offset.z), radian);
                    m_Value = new Vector3(newPos.x, y + offset.y, newPos.y);
                }
                else {
                    m_Value = Vector3.zero;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_TargetId = new StoryValue<int>();
        private IStoryFunction<Vector3> m_Offset = new StoryValue<Vector3>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CalcDirFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            CalcDirFunction val = new CalcDirFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_TargetId = m_TargetId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
                m_TargetId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
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
                var gobj = SceneSystem.Instance.GetGameObject(objId);
                if (null != obj && null != target) {
                    Vector2 srcPos = new Vector2(obj.GetMovementStateInfo().PositionX, obj.GetMovementStateInfo().PositionZ);
                    Vector2 targetPos = new Vector2(target.GetMovementStateInfo().PositionX, target.GetMovementStateInfo().PositionZ);
                    m_Value = Geometry.GetYRadian(srcPos, targetPos);
                }
                else if (null != gobj) {
                    m_Value = Geometry.DegreeToRadian(gobj.transform.localEulerAngles.y);
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_TargetId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class ObjGetNpcTypeFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            ObjGetNpcTypeFunction val = new ObjGetNpcTypeFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            {
                m_ObjId.Evaluate(instance, handler, iterator, args);
            }

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
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
                    m_Value = obj.EntityType;
                }
                else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class ObjGetAiParamFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Index.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            ObjGetAiParamFunction val = new ObjGetAiParamFunction();
            val.m_ObjId = m_ObjId.Clone();
            val.m_Index = m_Index.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Index.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
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
                    }
                    else {
                        m_Value = BoxedValue.NullObject;
                    }
                }
                else {
                    m_Value = BoxedValue.NullObject;
                }
            }
        }

        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Index = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
}
