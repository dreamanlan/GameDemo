using System;
using System.Collections;
using System.Collections.Generic;
using StorySystem;
using UnityEngine;
using GameLibrary;

namespace GameLibrary.Story.Commands
{
    /// <summary>
    /// blackboardclear();
    /// </summary>
    internal class BlackboardClearCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            BlackboardClearCommand cmd = new BlackboardClearCommand();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
        
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            SceneSystem.Instance.BlackBoard.ClearVariables();
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
        }
    }
    /// <summary>
    /// blackboardset(name,value);
    /// </summary>
    internal class BlackboardSetCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            BlackboardSetCommand cmd = new BlackboardSetCommand();
            cmd.m_AttrName = m_AttrName.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_AttrName.Evaluate(instance, iterator, args);
            m_Value.Evaluate(instance, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            string name = m_AttrName.Value;
            object value = m_Value.Value;
            SceneSystem.Instance.BlackBoard.SetVariable(name, value);
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_AttrName.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
        }
        private IStoryValue<string> m_AttrName = new StoryValue<string>();
        private IStoryValue<object> m_Value = new StoryValue();
    }
    /// <summary>
    /// setstoryskipped(0_or_1[, delay_time]);
    /// </summary>
    internal class SetStorySkippedCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            SetStorySkippedCommand cmd = new SetStorySkippedCommand();
            cmd.m_StorySkipped = m_StorySkipped.Clone();
            cmd.m_DelayedTime = m_DelayedTime.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_CurTime = 0;
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_StorySkipped.Evaluate(instance, iterator, args);
            m_DelayedTime.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int state = m_StorySkipped.Value;
            int curTime = m_CurTime;
            m_CurTime += (int)delta;
            int val = m_DelayedTime.Value;
            if (curTime <= val && val <= StoryValueHelper.c_MaxWaitCommandTime) {
                return true;
            } else {
                GlobalVariables.Instance.IsStorySkipped = state != 0;
                return false;
            }
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_StorySkipped.InitFromDsl(callData.GetParam(0));
            }
            if (num > 1) {
                m_DelayedTime.InitFromDsl(callData.GetParam(1));
            }
        }
        private IStoryValue<int> m_StorySkipped = new StoryValue<int>();
        private IStoryValue<int> m_DelayedTime = new StoryValue<int>();
        private int m_CurTime = 0;
    }
    /// <summary>
    /// setplayerid([objid,]leaderid);
    /// </summary>
    internal class SetPlayerIdCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            SetPlayerIdCommand cmd = new SetPlayerIdCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_LeaderId = m_LeaderId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            if (m_ParamNum > 1) {
                m_ObjId.Evaluate(instance, iterator, args);
            }
            m_LeaderId.Evaluate(instance, iterator, args);

            if (m_ParamNum > 1) {
            }
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int leaderId = m_LeaderId.Value;
            if (m_ParamNum > 1) {
                int objId = m_ObjId.Value;
                EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
                if (null != npc) {
                    npc.GetAiStateInfo().LeaderID = leaderId;
                }
            } else {
                SceneSystem.Instance.PlayerId = leaderId;
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_LeaderId.InitFromDsl(callData.GetParam(1));
            } else if (m_ParamNum > 0) {
                m_LeaderId.InitFromDsl(callData.GetParam(0));
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<int> m_LeaderId = new StoryValue<int>();
    }
    /// <summary>
    /// bornfinish(objid);
    /// </summary>
    internal class BornFinishCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            BornFinishCommand cmd = new BornFinishCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_ObjId = m_ObjId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_ObjId.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int objId = m_ObjId.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc) {
                npc.IsBorning = false;
                npc.BornTime = 0;
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 0) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
    }
    /// <summary>
    /// deadfinish(objid);
    /// </summary>
    internal class DeadFinishCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            DeadFinishCommand cmd = new DeadFinishCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_ObjId = m_ObjId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_ObjId.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int objId = m_ObjId.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc) {
                npc.DeadTime = 0;
                npc.NeedDelete = true;
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 0) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
    }


}
