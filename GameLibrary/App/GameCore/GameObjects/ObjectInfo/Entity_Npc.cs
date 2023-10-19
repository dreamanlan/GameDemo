using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLibrary
{
    public enum EntityTypeEnum
    {
        Player = 0,
        Npc,
        MaxTypeNum
    }

    public partial class EntityInfo
    {
        public EntityViewModel View
        {
            get { return m_View; }
            set { m_View = value; }
        }
        public object CustomData
        {
            get { return m_CustomData; }
            set { m_CustomData = value; }
        }
        public int EntityType
        {
            get { return m_EntityType; }
            set { m_EntityType = value; }
        }
        public float Scale
        {
            get { return m_Scale; }
        }
        public int CreatorId
        {
            get { return m_CreatorId; }
            set { m_CreatorId = value; }
        }
        public bool IsBorning
        {
            get { return m_IsBorning; }
            set { m_IsBorning = value; }
        }
        public long BornTime
        {
            get { return m_BornTime; }
            set { m_BornTime = value; }
        }
        public long BornTimeout
        {
            get { return m_BornTimeout; }
        }
        public bool NeedDelete
        {
            get { return m_NeedDelete; }
            set { m_NeedDelete = value; }
        }
        public float ViewRange
        {
            get { return m_ViewRange; }
            set { m_ViewRange = value; }
        }
        public float GohomeRange
        {
            get { return m_GohomeRange; }
            set { m_GohomeRange = value; }
        }
        public EntityInfo(int id)
        {
            InitBase(id);
        }
        public void InitId(int id)
        {
            m_Id = id;
        }
        public void Reset()
        {
            m_IsBorning = false;
            m_NeedDelete = false;
            m_BornTime = 0;
            m_CreatorId = 0;

            m_EntityType = 0;
            m_Scale = 1.0f;

            ResetBaseInfo();
            GetAiStateInfo().Reset();
        }

        public void LoadData(int unitId, string model, EntityTypeEnum entityType, string ai, params string[] aiParams)
        {
            SetUnitId(unitId);
            SetModel(model);
            m_EntityType = (int)entityType;
            GetAiStateInfo().AiLogic = ai;
            for (int i = 0; i < aiParams.Length && i < AiStateInfo.c_MaxAiParamNum; ++i) {
                GetAiStateInfo().AiParam[i] = aiParams[i];
            }
            Property.Owner = this;
            HpMax = 10000;
            Hp = 10000;
            EnergyMax = 1000;
            Energy = 1000;
            Speed = 8;
            Level = 1;
            Exp = 0;
        }

        public AiStateInfo GetAiStateInfo()
        {
            return m_AiStateInfo;
        }
        
        private object m_CustomData = null;
        private int m_EntityType = 0;
        private float m_Scale = 1.0f;

        private bool m_IsBorning = false;
        private long m_BornTime = 0;
        private long m_BornTimeout = 5000;
        private bool m_NeedDelete = false;

        private float m_ViewRange = 10.0f;
        private float m_GohomeRange = 20.0f;
        private int m_CreatorId = 0;

        private AiStateInfo m_AiStateInfo = new AiStateInfo();
        private EntityViewModel m_View = null;

        public const int c_StartUserUnitId = 100000000;
    }
}
