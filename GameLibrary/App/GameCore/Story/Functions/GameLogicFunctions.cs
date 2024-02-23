using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using StoryScript;
using GameLibrary;

namespace GameLibrary.Story.Functions
{
    internal sealed class BlackboardGetFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
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
        public IStoryFunction Clone()
        {
            BlackboardGetFunction val = new BlackboardGetFunction();
            val.m_ParamNum = m_ParamNum;
            val.m_AttrName = m_AttrName.Clone();
            val.m_DefaultValue = m_DefaultValue.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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
                object v;
                if (SceneSystem.Instance.BlackBoard.TryGetVariable(name, out v)) {
                    m_Value = BoxedValue.FromObject(v);
                }
                else {
                    if (m_ParamNum > 1) {
                        m_Value = m_DefaultValue.Value;
                    }
                }
            }
        }
        private int m_ParamNum = 0;
        private IStoryFunction<string> m_AttrName = new StoryValue<string>();
        private IStoryFunction m_DefaultValue = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsStorySkippedFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            IsStorySkippedFunction val = new IsStorySkippedFunction();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = StoryConfigManager.Instance.IsStorySkipped ? 1 : 0;
        }
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPlayerIdFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            GetPlayerIdFunction val = new GetPlayerIdFunction();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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
        private BoxedValue m_Value;
    }
    internal sealed class FindObjIdFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 3) {
                    m_Type.InitFromDsl(callData.GetParam(0));
                    m_Pos.InitFromDsl(callData.GetParam(1));
                    m_Range.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryFunction Clone()
        {
            FindObjIdFunction val = new FindObjIdFunction();
            val.m_Type = m_Type.Clone();
            val.m_Pos = m_Pos.Clone();
            val.m_Range = m_Range.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = BoxedValue.NullObject;
            EntityTypeEnum type = (EntityTypeEnum)m_Type.Value;
            var pos = m_Pos.Value;
            var range = m_Range.Value;
            var entity = SceneSystem.Instance.FindEntityByRange(type, pos.x, pos.z, range);
            if (null != entity) {
                m_Value = entity.GetId();
            }
        }

        private IStoryFunction<int> m_Type = new StoryValue<int>();
        private IStoryFunction<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryFunction<float> m_Range = new StoryValue<float>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class FindObjIdsFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 3) {
                    m_Type.InitFromDsl(callData.GetParam(0));
                    m_Pos.InitFromDsl(callData.GetParam(1));
                    m_Range.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryFunction Clone()
        {
            FindObjIdsFunction val = new FindObjIdsFunction();
            val.m_Type = m_Type.Clone();
            val.m_Pos = m_Pos.Clone();
            val.m_Range = m_Range.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = BoxedValue.NullObject;
            EntityTypeEnum type = (EntityTypeEnum)m_Type.Value;
            var pos = m_Pos.Value;
            var range = m_Range.Value;
            List<EntityInfo> list = new List<EntityInfo>();
            if (SceneSystem.Instance.FindEntitiesByRange(type, pos.x, pos.z, range, list)) {
                List<int> ids = new List<int>();
                foreach (var info in list) {
                    ids.Add(info.GetId());
                }
                m_Value = BoxedValue.FromObject(ids);
            }
        }

        private IStoryFunction<int> m_Type = new StoryValue<int>();
        private IStoryFunction<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryFunction<float> m_Range = new StoryValue<float>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class FindAllObjIdsFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 2) {
                    m_Pos.InitFromDsl(callData.GetParam(0));
                    m_Range.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryFunction Clone()
        {
            FindAllObjIdsFunction val = new FindAllObjIdsFunction();
            val.m_Pos = m_Pos.Clone();
            val.m_Range = m_Range.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = BoxedValue.NullObject;
            var pos = m_Pos.Value;
            var range = m_Range.Value;
            List<EntityInfo> list = new List<EntityInfo>();
            if (SceneSystem.Instance.FindAllEntitiesByRange(pos.x, pos.z, range, list)) {
                List<int> ids = new List<int>();
                foreach (var info in list) {
                    ids.Add(info.GetId());
                }
                m_Value = BoxedValue.FromObject(ids);
            }
        }

        private IStoryFunction<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryFunction<float> m_Range = new StoryValue<float>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CountNpcFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
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
        public IStoryFunction Clone()
        {
            CountNpcFunction val = new CountNpcFunction();
            val.m_Camp = m_Camp.Clone();
            val.m_Relation = m_Relation.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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

        private IStoryFunction<int> m_Camp = new StoryValue<int>();
        private IStoryFunction<int> m_Relation = new StoryValue<int>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CountDyingNpcFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
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
        public IStoryFunction Clone()
        {
            CountDyingNpcFunction val = new CountDyingNpcFunction();
            val.m_Camp = m_Camp.Clone();
            val.m_Relation = m_Relation.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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

        private IStoryFunction<int> m_Camp = new StoryValue<int>();
        private IStoryFunction<int> m_Relation = new StoryValue<int>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetFileNameFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryFunction Clone()
        {
            GetFileNameFunction val = new GetFileNameFunction();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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

        private IStoryFunction<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetDirectoryNameFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryFunction Clone()
        {
            GetDirectoryNameFunction val = new GetDirectoryNameFunction();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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

        private IStoryFunction<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetExtensionFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryFunction Clone()
        {
            GetExtensionFunction val = new GetExtensionFunction();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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

        private IStoryFunction<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CombinePathFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 2) {
                    m_Path1.InitFromDsl(callData.GetParam(0));
                    m_Path2.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryFunction Clone()
        {
            CombinePathFunction val = new CombinePathFunction();
            val.m_Path1 = m_Path1.Clone();
            val.m_Path2 = m_Path2.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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

        private IStoryFunction<string> m_Path1 = new StoryValue<string>();
        private IStoryFunction<string> m_Path2 = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetStreamingAssetsFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            GetStreamingAssetsFunction val = new GetStreamingAssetsFunction();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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
        private BoxedValue m_Value;
    }
    internal sealed class GetPersistentPathFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            GetPersistentPathFunction val = new GetPersistentPathFunction();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
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
        public BoxedValue Value
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
        private BoxedValue m_Value;
    }
}
