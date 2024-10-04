using System;
using System.Collections.Generic;
using StoryScript;
using GameLibrary;
using UnityEngine;
namespace GameLibrary.Story.Functions
{
    internal sealed class NpcGetNpcTypeFunction : IStoryFunction
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
            NpcGetNpcTypeFunction val = new NpcGetNpcTypeFunction();
            val.m_UnitId = m_UnitId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_UnitId.Evaluate(instance, handler, iterator, args);
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
            if (m_UnitId.HaveValue) {
                int unitId = m_UnitId.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
                if (null != obj) {
                    m_Value = obj.EntityType;
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class NpcGetAiParamFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 2) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Index.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryFunction Clone()
        {
            NpcGetAiParamFunction val = new NpcGetAiParamFunction();
            val.m_UnitId = m_UnitId.Clone();
            val.m_Index = m_Index.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_Index.Evaluate(instance, handler, iterator, args);
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
            if (m_UnitId.HaveValue) {
                int unitId = m_UnitId.Value;
                int index = m_Index.Value;
                m_HaveValue = true;
                EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
                if (null != obj) {
                    var aiState = obj.GetAiStateInfo();
                    if (index >= 0 && index < aiState.AiParam.Length) {
                        m_Value = aiState.AiParam[index];
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else {
                    m_Value = BoxedValue.NullObject;
                }
            }
        }

        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<int> m_Index = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
}
