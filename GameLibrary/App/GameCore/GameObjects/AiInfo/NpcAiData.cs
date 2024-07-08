using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameLibrary
{
    internal class AiData_General
    {
        internal int LockTargetId
        {
            get { return m_LockTargetId; }
            set { m_LockTargetId = value; }
        }
        internal EntityInfo LockTarget
        {
            get { return m_LockTarget; }
            set { m_LockTarget = value; }
        }
        internal bool IsNormalAttackStopped
        {
            get { return m_IsNormalAttackStopped; }
            set { m_IsNormalAttackStopped = value; }
        }
        internal int ManualSkillId
        {
            get { return m_ManualSkillId; }
            set { m_ManualSkillId = value; }
        }
        internal bool DontPursue
        {
            get { return m_DontPursue; }
            set { m_DontPursue = value; }
        }
        internal long LastManualOperationTime
        {
            get { return m_LastManualOperationTime; }
            set { m_LastManualOperationTime = value; }
        }
        internal long LastUseSkillTime
        {
            get { return m_LastUseSkillTime; }
            set { m_LastUseSkillTime = value; }
        }
        internal int LastSkillId
        {
            get { return m_LastSkillId; }
            set { m_LastSkillId = value; }
        }
        internal int ComboCount
        {
            get { return m_ComboCount; }
            set { m_ComboCount = value; }
        }
        internal IntList ComboSkills
        {
            get { return m_ComboSkills; }
            set { m_ComboSkills = value; }
        }

        internal long WaitManualOperationTime
        {
            get { return m_WaitManualOperationTime; }
            set { m_WaitManualOperationTime = value; }
        }
        internal bool WaitManualOperation()
        {
            return m_LastManualOperationTime + m_WaitManualOperationTime > StoryScript.TimeUtility.GetLocalMilliseconds();
        }

        private int m_LockTargetId = 0;
        private EntityInfo m_LockTarget = null;
        private int m_LastSkillId = 0;
        private int m_ComboCount = 0;
        private IntList m_ComboSkills = new IntList();
        private bool m_IsNormalAttackStopped = true;
        private int m_ManualSkillId = 0;
        private bool m_DontPursue = false;
        private long m_LastUseSkillTime = 0;
        private long m_LastManualOperationTime = 0;
        private long m_WaitManualOperationTime = 5000;
    }
    internal class AiData_ForMoveCommand
    {
        internal List<Vector3> WayPoints { get; set; }
        internal int Index { get; set; }
        internal bool IsFinish { get; set; }
        internal string Event { get; set; }

        internal AiData_ForMoveCommand(List<Vector3> way_points)
        {
            WayPoints = way_points;
            Index = 0;
            IsFinish = false;
        }
    }
}
