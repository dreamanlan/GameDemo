using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using StorySystem;

namespace GameLibrary
{
    public enum PredefinedAiStateId : int
    {
        Invalid = 0,
        Idle,
        MoveCommand,
        WaitCommand,
        MaxValue = 100
    }
    public class AiStoryInstanceInfo
    {
        public StoryInstance m_StoryInstance;
        public bool m_IsUsed;

        public void Recycle()
        {
            m_StoryInstance.Reset(false);
            m_IsUsed = false;
        }

		public void SetObjID(int id)
		{
			if(m_StoryInstance != null)
				m_StoryInstance.SetVariable("@objid", id);
		}
    }
    public class AiStateInfo
    {
        public int CurState
        {
            get
            {
                int state = (int)PredefinedAiStateId.Invalid;
                if (m_StateStack.Count > 0)
                    state = m_StateStack.Peek();
                return state;
            }
        }
		public void SetObjID(int id)
		{
			if (m_AiStoryInstanceInfo != null)
				m_AiStoryInstanceInfo.SetObjID(id);
		}
        public void PushState(int state)
        {
            m_StateStack.Push(state);
        }
        public int PopState()
        {
            int ret = (int)PredefinedAiStateId.Invalid;
            if (m_StateStack.Count > 0)
                ret = m_StateStack.Pop();
            return ret;
        }
        public void ChangeToState(int state)
        {
            if (m_StateStack.Count > 0)
                m_StateStack.Pop();
            m_StateStack.Push(state);
        }
        public void CloneAiStates(IEnumerable<int> states)
        {
            m_StateStack = new Stack<int>(states);
        }
        public int[] CloneAiStates()
        {
            return m_StateStack.ToArray();
        }

		public bool CheckIfNeedReset(string logic, string param)
		{
			return this.AiLogic != logic || this.AiParam[0] != param || AiStoryInstanceInfo == null || AiStoryInstanceInfo.m_StoryInstance == null;
		}

        public void Reset()
        {
            m_StateStack.Clear();
            m_AiDatas.Clear();
            if (null != m_AiStoryInstanceInfo) {
                m_AiStoryInstanceInfo.Recycle();
                m_AiStoryInstanceInfo = null;
            }
            m_IsInited = false;

            m_AiLogic = string.Empty;
            m_AiParam = new string[c_MaxAiParamNum];
            m_AiStoryInstanceInfo = null;
            m_Time = 0;
            m_IsInited = false;
            m_LeaderID = -1;
            m_HomePos = Vector3.zero;
            m_Target = 0;
            m_HateTarget = 0;
            m_IsExternalTarget = false;
            m_LastChangeTargetTime = 0;
        }
        public string AiLogic
        {
            get { return m_AiLogic; }
            set { m_AiLogic = value; }
        }
        public int LeaderID
        {
            get { return m_LeaderID; }
            set { m_LeaderID = value; }
        }
        public string[] AiParam
        {
            get { return m_AiParam; }
        }
        public CustomDataCollection AiDatas
        {
            get { return m_AiDatas; }
        }
        public AiStoryInstanceInfo AiStoryInstanceInfo
        {
            get { return m_AiStoryInstanceInfo; }
            set { m_AiStoryInstanceInfo = value; }
        }
        public bool IsInited
        {
            get { return m_IsInited; }
            set { m_IsInited = value; }
        }
        public long Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        public UnityEngine.Vector3 HomePos
        {
            get { return m_HomePos; }
            set { m_HomePos = value; }
        }
        public UnityEngine.Vector3 TargetPosition
        {
            get { return m_TargetPosition; }
            set { m_TargetPosition = value; }
        }
        public int Target
        {
            get { return m_Target; }
            set
            {
                m_Target = value;
                m_IsExternalTarget = false;
            }
        }
        public int HateTarget
        {
            get { return m_HateTarget; }
            set { m_HateTarget = value; }
        }
        public bool IsExternalTarget
        {
            get { return m_IsExternalTarget; }
        }
        public long LastChangeTargetTime
        {
            get { return m_LastChangeTargetTime; }
            set { m_LastChangeTargetTime = value; }
        }
        public void SetExternalTarget(int target)
        {
            m_Target = target;
            m_IsExternalTarget = true;
        }
        public BoxedValueList NewBoxedValueList()
        {
            if (null != m_AiStoryInstanceInfo && null != m_AiStoryInstanceInfo.m_StoryInstance) {
                return m_AiStoryInstanceInfo.m_StoryInstance.NewBoxedValueList();
            }
            return null;
        }
        public void SendMessage(string msgId, BoxedValueList args)
        {
            if (null != m_AiStoryInstanceInfo && null != m_AiStoryInstanceInfo.m_StoryInstance) {
                m_AiStoryInstanceInfo.m_StoryInstance.SendMessage(msgId, args);
            }
        }
        public void SendConcurrentMessage(string msgId, BoxedValueList args)
        {
            if (null != m_AiStoryInstanceInfo && null != m_AiStoryInstanceInfo.m_StoryInstance) {
                m_AiStoryInstanceInfo.m_StoryInstance.SendConcurrentMessage(msgId, args);
            }
        }
        public void SendNamespacedMessage(string msgId, BoxedValueList args)
        {
            if (null != m_AiStoryInstanceInfo && null != m_AiStoryInstanceInfo.m_StoryInstance) {
                var storyInst = m_AiStoryInstanceInfo.m_StoryInstance;
                if (!string.IsNullOrEmpty(storyInst.Namespace)) {
                    msgId = string.Format("{0}:{1}", storyInst.Namespace, msgId);
                }
                m_AiStoryInstanceInfo.m_StoryInstance.SendMessage(msgId, args);
            }
        }
        public void SendConcurrentNamespacedMessage(string msgId, BoxedValueList args)
        {
            if (null != m_AiStoryInstanceInfo && null != m_AiStoryInstanceInfo.m_StoryInstance) {
                var storyInst = m_AiStoryInstanceInfo.m_StoryInstance;
                if (!string.IsNullOrEmpty(storyInst.Namespace)) {
                    msgId = string.Format("{0}:{1}", storyInst.Namespace, msgId);
                }
                m_AiStoryInstanceInfo.m_StoryInstance.SendConcurrentMessage(msgId, args);
            }
        }

        private Stack<int> m_StateStack = new Stack<int>();
        private string m_AiLogic = string.Empty;
        private string[] m_AiParam = new string[c_MaxAiParamNum];
        private CustomDataCollection m_AiDatas = new CustomDataCollection();
        private AiStoryInstanceInfo m_AiStoryInstanceInfo = null;
        private long m_Time = 0;
        private bool m_IsInited = false;
        private int m_LeaderID = -1;
        private UnityEngine.Vector3 m_HomePos = Vector3.zero;
        private UnityEngine.Vector3 m_TargetPosition = UnityEngine.Vector3.zero;
        private int m_Target = 0;
        private int m_HateTarget = 0;
        private bool m_IsExternalTarget = false;
        private long m_LastChangeTargetTime = 0;

        public const int c_MaxAiParamNum = 8;
    }
}
