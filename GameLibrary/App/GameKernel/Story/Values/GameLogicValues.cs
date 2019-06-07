using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using StorySystem;
using GameLibrary;

namespace GameLibrary.Story.Values
{
    internal sealed class BlackboardGetValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 0) {
                    m_AttrName.InitFromDsl(callData.GetParam(0));
                }
                if (m_ParamNum > 1) {
                    m_DefaultValue.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            BlackboardGetValue val = new BlackboardGetValue();
            val.m_ParamNum = m_ParamNum;
            val.m_AttrName = m_AttrName.Clone();
            val.m_DefaultValue = m_DefaultValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            if (m_ParamNum > 0) {
                m_AttrName.Evaluate(instance, handler, iterator, args);
            }
            if (m_ParamNum > 1) {
                m_DefaultValue.Evaluate(instance, handler, iterator, args);
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
            if (m_AttrName.HaveValue) {
                string name = m_AttrName.Value;
                m_HaveValue = true;
                if (!SceneSystem.Instance.BlackBoard.TryGetVariable(name, out m_Value)) {
                    if (m_ParamNum > 1) {
                        m_Value = m_DefaultValue.Value;
                    }
                }
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<string> m_AttrName = new StoryValue<string>();
        private IStoryValue m_DefaultValue = new StoryValue();
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class IsStorySkippedValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            IsStorySkippedValue val = new IsStorySkippedValue();
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
            m_HaveValue = true;
            m_Value = GlobalVariables.Instance.IsStorySkipped ? 1 : 0;
        }
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetPlayerIdValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetPlayerIdValue val = new GetPlayerIdValue();
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
            m_Value = SceneSystem.Instance.PlayerId;
        }
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class FindObjIdValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 3) {
                    m_Type.InitFromDsl(callData.GetParam(0));
                    m_Pos.InitFromDsl(callData.GetParam(1));
                    m_Range.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryValue Clone()
        {
            FindObjIdValue val = new FindObjIdValue();
            val.m_Type = m_Type.Clone();
            val.m_Pos = m_Pos.Clone();
            val.m_Range = m_Range.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Type.Evaluate(instance, handler, iterator, args);
            m_Pos.Evaluate(instance, handler, iterator, args);
            m_Range.Evaluate(instance, handler, iterator, args);
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
            m_Value = null;
            EntityTypeEnum type = (EntityTypeEnum)m_Type.Value;
            var pos = m_Pos.Value;
            var range = m_Range.Value;
            var entity = SceneSystem.Instance.FindEntityByRange(type, pos.x, pos.z, range);
            if (null != entity) {
                m_Value = entity.GetId();
            }
        }

        private IStoryValue<int> m_Type = new StoryValue<int>();
        private IStoryValue<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryValue<float> m_Range = new StoryValue<float>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class FindObjIdsValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 3) {
                    m_Type.InitFromDsl(callData.GetParam(0));
                    m_Pos.InitFromDsl(callData.GetParam(1));
                    m_Range.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryValue Clone()
        {
            FindObjIdsValue val = new FindObjIdsValue();
            val.m_Type = m_Type.Clone();
            val.m_Pos = m_Pos.Clone();
            val.m_Range = m_Range.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Type.Evaluate(instance, handler, iterator, args);
            m_Pos.Evaluate(instance, handler, iterator, args);
            m_Range.Evaluate(instance, handler, iterator, args);
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
            m_Value = null;
            EntityTypeEnum type = (EntityTypeEnum)m_Type.Value;
            var pos = m_Pos.Value;
            var range = m_Range.Value;
            List<EntityInfo> list = new List<EntityInfo>();
            if (SceneSystem.Instance.FindEntitiesByRange(type, pos.x, pos.z, range, list)) {
                List<int> ids = new List<int>();
                foreach (var info in list) {
                    ids.Add(info.GetId());
                }
                m_Value = ids;
            }
        }

        private IStoryValue<int> m_Type = new StoryValue<int>();
        private IStoryValue<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryValue<float> m_Range = new StoryValue<float>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class FindAllObjIdsValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 2) {
                    m_Pos.InitFromDsl(callData.GetParam(0));
                    m_Range.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            FindAllObjIdsValue val = new FindAllObjIdsValue();
            val.m_Pos = m_Pos.Clone();
            val.m_Range = m_Range.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Pos.Evaluate(instance, handler, iterator, args);
            m_Range.Evaluate(instance, handler, iterator, args);
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
            m_Value = null;
            var pos = m_Pos.Value;
            var range = m_Range.Value;
            List<EntityInfo> list = new List<EntityInfo>();
            if (SceneSystem.Instance.FindAllEntitiesByRange(pos.x, pos.z, range, list)) {
                List<int> ids = new List<int>();
                foreach (var info in list) {
                    ids.Add(info.GetId());
                }
                m_Value = ids;
            }
        }

        private IStoryValue<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryValue<float> m_Range = new StoryValue<float>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class CountNpcValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum >= 1) {
                    m_Camp.InitFromDsl(callData.GetParam(0));
                }
                if (m_ParamNum == 2) {
                    m_Relation.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            CountNpcValue val = new CountNpcValue();
            val.m_Camp = m_Camp.Clone();
            val.m_Relation = m_Relation.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Camp.Evaluate(instance, handler, iterator, args);
            m_Relation.Evaluate(instance, handler, iterator, args);
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
            m_HaveValue = true;
            int campId = m_Camp.Value;
            if (m_ParamNum == 1) {
                m_Value = SceneSystem.Instance.GetNpcCountByCamp(campId);
            } else {
                CharacterRelation relation = (CharacterRelation)m_Relation.Value;
                m_Value = SceneSystem.Instance.GetNpcCountByRelationWithCamp(campId, relation);
            }
        }

        private IStoryValue<int> m_Camp = new StoryValue<int>();
        private IStoryValue<int> m_Relation = new StoryValue<int>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class CountDyingNpcValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum >= 1) {
                    m_Camp.InitFromDsl(callData.GetParam(0));
                }
                if (m_ParamNum == 2) {
                    m_Relation.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            CountDyingNpcValue val = new CountDyingNpcValue();
            val.m_Camp = m_Camp.Clone();
            val.m_Relation = m_Relation.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Camp.Evaluate(instance, handler, iterator, args);
            m_Relation.Evaluate(instance, handler, iterator, args);
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
            m_HaveValue = true;
            int campId = m_Camp.Value;
            if (m_ParamNum == 1) {
                m_Value = SceneSystem.Instance.GetDyingNpcCountByCamp(campId);
            } else {
                CharacterRelation relation = (CharacterRelation)m_Relation.Value;
                m_Value = SceneSystem.Instance.GetDyingNpcCountByRelationWithCamp(campId, relation);
            }
        }

        private IStoryValue<int> m_Camp = new StoryValue<int>();
        private IStoryValue<int> m_Relation = new StoryValue<int>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetFileNameValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetFileNameValue val = new GetFileNameValue();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Path.Evaluate(instance, handler, iterator, args);
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
            var path = m_Path.Value;
            m_Value = Path.GetFileName(path);
        }

        private IStoryValue<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetDirectoryNameValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetDirectoryNameValue val = new GetDirectoryNameValue();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Path.Evaluate(instance, handler, iterator, args);
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
            var path = m_Path.Value;
            m_Value = Path.GetDirectoryName(path);
        }

        private IStoryValue<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetExtensionValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetExtensionValue val = new GetExtensionValue();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Path.Evaluate(instance, handler, iterator, args);
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
            var path = m_Path.Value;
            m_Value = Path.GetExtension(path);
        }

        private IStoryValue<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class CombinePathValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 2) {
                    m_Path1.InitFromDsl(callData.GetParam(0));
                    m_Path2.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            CombinePathValue val = new CombinePathValue();
            val.m_Path1 = m_Path1.Clone();
            val.m_Path2 = m_Path2.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_HaveValue = false;
            m_Path1.Evaluate(instance, handler, iterator, args);
            m_Path2.Evaluate(instance, handler, iterator, args);
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
            var path1 = m_Path1.Value;
            var path2 = m_Path2.Value;
            m_Value = Path.Combine(path1, path2);
        }

        private IStoryValue<string> m_Path1 = new StoryValue<string>();
        private IStoryValue<string> m_Path2 = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetStreamingAssetsValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetStreamingAssetsValue val = new GetStreamingAssetsValue();
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
            m_Value = UnityEngine.Application.streamingAssetsPath;
        }
        private bool m_HaveValue;
        private object m_Value;
    }
    internal sealed class GetPersistentPathValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.CallData callData = param as Dsl.CallData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetPersistentPathValue val = new GetPersistentPathValue();
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
            m_Value = UnityEngine.Application.persistentDataPath;
        }
        private bool m_HaveValue;
        private object m_Value;
    }
}
