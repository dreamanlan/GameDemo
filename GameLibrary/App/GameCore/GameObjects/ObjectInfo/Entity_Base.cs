using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLibrary
{
    public partial class EntityInfo
    {
        public void InitBase(int id)
        {
            m_Id = id;
            m_UnitId = 0;
            m_AIEnable = true;
            m_Property = new CharacterProperty();
        }
        public int GetId()
        {
            return m_Id;
        }
        public int GetUnitId()
        {
            return m_UnitId;
        }
        public void SetUnitId(int id)
        {
            int old = m_UnitId;
            m_UnitId = id;
            m_EntityManager.UpdateUnitId(this, old);
        }
        public void SetName(string name)
        {
            m_Name = name;
        }
        public string GetName()
        {
            return m_Name;
        }
        public bool IsControlByStory
        {
            get { return m_IsControlByStory; }
            set { m_IsControlByStory = value; }
        }
        public bool IsControlByManual
        {
            get { return m_IsControlByManual; }
            set { m_IsControlByManual = value; }
        }
        public long DeadTime
        {
            get
            {
                return m_DeadTime;
            }
            set
            {
                m_DeadTime = value;
            }
        }
        public long DeadTimeout
        {
            get { return m_DeadTimeout; }
        }
        public int Level
        {
            get
            {
                return Property.GetInt(CharacterPropertyEnum.x1006_等级);
            }
            set
            {
                Property.SetInt(CharacterPropertyEnum.x1006_等级, value);
            }
        }
        public int Exp
        {
            get
            {
                return Property.GetInt(CharacterPropertyEnum.x1007_当前经验);
            }
            set
            {
                Property.SetInt(CharacterPropertyEnum.x1007_当前经验, value);
            }
        }
        public int Hp
        {
            get 
            {
                return Property.GetInt(CharacterPropertyEnum.x1002_当前生命值); 
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > HpMax)
                    value = HpMax;
                Property.SetInt(CharacterPropertyEnum.x1002_当前生命值, value);
            }
        }
        public int HpMax
        {
            get
            {
                return Property.GetInt(CharacterPropertyEnum.x1001_最大生命值);
            }
            set
            {
                if (value < 0)
                    value = 0;
                Property.SetInt(CharacterPropertyEnum.x1001_最大生命值, value);
            }
        }
        public int Energy
        {
            get
            {
                return Property.GetInt(CharacterPropertyEnum.x1004_当前法力值);
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > EnergyMax)
                    value = EnergyMax;
                Property.SetInt(CharacterPropertyEnum.x1004_当前法力值, value);
            }
        }
        public int EnergyMax
        {
            get
            {
                return Property.GetInt(CharacterPropertyEnum.x1003_最大法力值);
            }
            set
            {
                if (value < 0)
                    value = 0;
                Property.SetInt(CharacterPropertyEnum.x1003_最大法力值, value);
            }
        }
        public float Speed
        {
            get
            {
                return Property.GetFloat(CharacterPropertyEnum.x1005_移动速度);
            }
            set
            {
                Property.SetFloat(CharacterPropertyEnum.x1005_移动速度, value);
            }
        }
        public string GetModel()
        {
            return m_Model;
        }
        public void SetModel(string model)
        {
            m_Model = model;
        }
        public bool GetAIEnable()
        {
            return m_AIEnable;
        }
        public void SetAIEnable(bool enable)
        {
            m_AIEnable = enable;
        }
        public int GetCampId()
        {
            return m_CampId;
        }
        public void SetCampId(int val)
        {
            m_CampId = val;
        }
        public bool IsDead()
        {
            return Hp <= 0;
        }
        public float GetRadius()
        {
            return m_Radius;
        }
        public CharacterProperty Property
        {
            get
            {
                return m_Property;
            }
        }
        public MovementStateInfo GetMovementStateInfo()
        {
            return m_MovementStateInfo;
        }
        internal EntityManager EntityManager
        {
            get { return m_EntityManager; }
            set { m_EntityManager = value; }
        }

        private void ResetBaseInfo()
        {
            SetAIEnable(true);
            m_DeadTime = 0;

            m_IsControlByStory = false;
            m_IsControlByManual = false;
            m_CampId = 0;

            Hp = 0;
            Energy = 0;

            GetMovementStateInfo().Reset();
        }

        private int m_Id = 0;
        private float m_Radius = 1.0f;
        private int m_UnitId = 0;
        private string m_Name = "";
        private string m_Model = "";
        private bool m_AIEnable = true;
        private bool m_IsControlByStory = false;
        private bool m_IsControlByManual = false;
        private int m_CampId = 0;

        private long m_DeadTime = 0;
        private long m_DeadTimeout = 5000;

        private CharacterProperty m_Property;
        private MovementStateInfo m_MovementStateInfo = new MovementStateInfo();
        private EntityManager m_EntityManager = null;

        //Camp:Friendly、Hostile、Blue、Red
        //Friendly: friendly to everyone
        //Hostile: hostile to everyone (but friendly to same camp)
        //Blue and Hostile: hostile to Red
        //Red and Hostile: hostile to Blue
        public static CharacterRelation GetRelation(EntityInfo pObj_A, EntityInfo pObj_B)
        {
            if (pObj_A == null || pObj_B == null) {
                return CharacterRelation.RELATION_INVALID;
            }

            if (pObj_A == pObj_B) {
                return CharacterRelation.RELATION_FRIEND;
            }

            int campA = pObj_A.GetCampId();
            int campB = pObj_B.GetCampId();
            CharacterRelation relation = GetRelation(campA, campB);
            return relation;
        }
        public static CharacterRelation GetRelation(int campA, int campB)
        {
            CharacterRelation relation = CharacterRelation.RELATION_INVALID;
            if ((int)CampIdEnum.Unkown != campA && (int)CampIdEnum.Unkown != campB) {
                if (campA == campB)
                    relation = CharacterRelation.RELATION_FRIEND;
                else if ((int)CampIdEnum.Friendly == campA || (int)CampIdEnum.Friendly == campB)
                    relation = CharacterRelation.RELATION_FRIEND;
                else if ((int)CampIdEnum.Hostile == campA || (int)CampIdEnum.Hostile == campB)
                    relation = CharacterRelation.RELATION_ENEMY;
                else
                    relation = CharacterRelation.RELATION_ENEMY;
            }
            return relation;
        }

        public float DistanceSquareBetween(EntityInfo other)
        {
            var moveInfo1 = GetMovementStateInfo().GetPosition3D();
            var moveInfo2 = other.GetMovementStateInfo().GetPosition3D();

            return Geometry.DistanceSquare(moveInfo1,moveInfo2);
        }
    }
}
