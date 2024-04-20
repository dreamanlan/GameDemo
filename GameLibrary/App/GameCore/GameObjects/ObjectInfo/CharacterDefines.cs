using System;
using System.Collections.Generic;

namespace GameLibrary
{
    public enum CampIdEnum : int
    {
        Unkown = 0,
        Friendly,
        Hostile,
        Blue,
        Red,
    }

    // relationship
    public enum CharacterRelation : int
    {
        RELATION_INVALID = 0,
        RELATION_ENEMY,				// hostility
        RELATION_FRIEND,			// friendly
    }

    public enum CharacterPropertyEnum : int
    {
        x1001_最大生命值 = 1001,
        x1002_当前生命值 = 1002,
        x1003_最大法力值 = 1003,
        x1004_当前法力值 = 1004,
        x1005_移动速度 = 1005,
        x1006_等级 = 1006,
        x1007_当前经验 = 1007,
    }

    public class AttackerInfo
    {
        public ulong m_AttackGuid;
        public long m_AttackTime;
        public int m_HpDamage;
        public int m_NpDamage;
    }
}

