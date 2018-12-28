using System;
using System.Collections;
using System.Collections.Generic;
using StorySystem;
using UnityEngine;
using GameLibrary;

namespace GameLibrary.Story.Commands
{
    /// <summary>
    /// createnpc(npc_unit_id,vector3(x,y,z),dir,model,type[,ai,stringlist("param1 param2 param3 ...")])[objid("@objid")];
    /// </summary>
    internal class CreateNpcCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            CreateNpcCommand cmd = new CreateNpcCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Pos = m_Pos.Clone();
            cmd.m_Dir = m_Dir.Clone();
            cmd.m_Model = m_Model.Clone();
            cmd.m_Type = m_Type.Clone();
            cmd.m_AiLogic = m_AiLogic.Clone();
            cmd.m_AiParams = m_AiParams.Clone();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_HaveObjId = m_HaveObjId;
            cmd.m_ObjIdVarName = m_ObjIdVarName.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            if (m_ParamNum >= 5)
            {
                m_UnitId.Evaluate(instance, iterator, args);
                m_Pos.Evaluate(instance, iterator, args);
                m_Dir.Evaluate(instance, iterator, args);
                m_Model.Evaluate(instance, iterator, args);
                m_Type.Evaluate(instance, iterator, args);
                if (m_ParamNum > 6)
                {
                    m_AiLogic.Evaluate(instance, iterator, args);
                    m_AiParams.Evaluate(instance, iterator, args);
                }
            }
            if (m_HaveObjId)
            {
                m_ObjIdVarName.Evaluate(instance, iterator, args);
            }

            if (m_ParamNum >= 5)
            {
                if (m_ParamNum > 6)
                {
                }
            }
            if (m_HaveObjId)
            {
            }
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int objId = 0;
            if (m_ParamNum >= 5)
            {
                Vector3 pos = m_Pos.Value;
                float dir = m_Dir.Value;
                string model = m_Model.Value;
                int entityType = m_Type.Value;
                int unitId = m_UnitId.Value;
                if (unitId > 0) {
                    var entityInfo = SceneSystem.Instance.GetEntityByUnitId(unitId);
                    if (null != entityInfo) {
                        SceneSystem.Instance.DestroyEntity(entityInfo);
                    }
                }
                if (m_ParamNum > 6) {
                    List<string> ais = new List<string>();
                    IEnumerable aiParams = m_AiParams.Value;
                    foreach (string aiParam in aiParams)
                    {
                        ais.Add(aiParam);
                    }
                    objId = SceneSystem.Instance.CreateEntityWithAi(unitId, pos.x, pos.y, pos.z, dir, model, (EntityTypeEnum)entityType, m_AiLogic.Value, ais.ToArray());
                } else {
                    objId = SceneSystem.Instance.CreateEntity(unitId, pos.x, pos.y, pos.z, dir, model, (EntityTypeEnum)entityType);
                }
            }
            if (m_HaveObjId)
            {
                string varName = m_ObjIdVarName.Value;
                instance.SetVariable(varName, objId);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum >= 5)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Pos.InitFromDsl(callData.GetParam(1));
                m_Dir.InitFromDsl(callData.GetParam(2));
                m_Model.InitFromDsl(callData.GetParam(3));
                m_Type.InitFromDsl(callData.GetParam(4));
                if (m_ParamNum > 6)
                {
                    m_AiLogic.InitFromDsl(callData.GetParam(5));
                    m_AiParams.InitFromDsl(callData.GetParam(6));
                }
            }
        }
        protected override void Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2)
            {
                Dsl.FunctionData first = statementData.First;
                Dsl.FunctionData second = statementData.Second;
                if (null != first && null != first.Call && null != second && null != second.Call)
                {
                    Load(first.Call);
                    LoadVarName(second.Call);
                }
            }
        }
        private void LoadVarName(Dsl.CallData callData)
        {
            if (callData.GetId() == "objid" && callData.GetParamNum() == 1)
            {
                m_ObjIdVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObjId = true;
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private int m_ParamNum = 0;
        private IStoryValue<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryValue<float> m_Dir = new StoryValue<float>();
        private IStoryValue<string> m_Model = new StoryValue<string>();
        private IStoryValue<int> m_Type = new StoryValue<int>();
        private IStoryValue<string> m_AiLogic = new StoryValue<string>();
        private IStoryValue<IEnumerable> m_AiParams = new StoryValue<IEnumerable>();
        private bool m_HaveObjId = false;
        private IStoryValue<string> m_ObjIdVarName = new StoryValue<string>();
    }
    /// <summary>
    /// destroynpc(npc_unit_id[,immediately]);
    /// </summary>
    internal class DestroyNpcCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            DestroyNpcCommand cmd = new DestroyNpcCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Immediately = m_Immediately.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Immediately.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            bool immediately = m_Immediately.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != npc)
            {
                if (immediately) {
                    SceneSystem.Instance.DestroyEntity(npc);
                } else {
                    npc.NeedDelete = true;
                }
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
            if (num > 1) {
                m_Immediately.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<bool> m_Immediately = new StoryValue<bool>();
    }
    /// <summary>
    /// destroynpcwithobjid(npc_obj_id[,immediately]);
    /// </summary>
    internal class DestroyNpcWithObjIdCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            DestroyNpcWithObjIdCommand cmd = new DestroyNpcWithObjIdCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Immediately = m_Immediately.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_ObjId.Evaluate(instance, iterator, args);
            m_Immediately.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int objid = m_ObjId.Value;
            bool immediately = m_Immediately.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityById(objid);
            if (null != npc)
            {
                if (immediately) {
                    SceneSystem.Instance.DestroyEntity(npc);
                } else {
                    npc.NeedDelete = true;
                }
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
            if (num > 1) {
                m_Immediately.InitFromDsl(callData.GetParam(1));
            }
        }
        private IStoryValue<int> m_ObjId = new StoryValue<int>();
        private IStoryValue<bool> m_Immediately = new StoryValue<bool>();
    }
    /// <summary>
    /// npcface(npc_unit_id, dir[, immediately]);
    /// </summary>
    internal class NpcFaceCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcFaceCommand cmd = new NpcFaceCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Dir = m_Dir.Clone();
            cmd.m_Immediately = m_Immediately.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Dir.Evaluate(instance, iterator, args);
            m_Immediately.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            float dir = m_Dir.Value;
            int im = 0;
            if (m_Immediately.HaveValue)
                im = m_Immediately.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != npc)
            {
                npc.GetMovementStateInfo().SetFaceDir(dir);
                var uobj = npc.View.Actor;
                if (null != uobj) {
                    var e = uobj.transform.eulerAngles;
                    uobj.transform.eulerAngles = new UnityEngine.Vector3(e.x, Geometry.RadianToDegree(dir), e.z);
                }
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Dir.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Immediately.InitFromDsl(callData.GetParam(2));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<float> m_Dir = new StoryValue<float>();
        private IStoryValue<int> m_Immediately = new StoryValue<int>();
    }
    /// <summary>
    /// npcmove(npc_unit_id,vector3(x,y,z)[,event]);
    /// </summary>
    internal class NpcMoveCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcMoveCommand cmd = new NpcMoveCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Pos = m_Pos.Clone();
            cmd.m_Event = m_Event.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Pos.Evaluate(instance, iterator, args);
            m_Event.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            Vector3 pos = m_Pos.Value;
            string eventName = m_Event.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != npc)
            {
                Vector3List waypoints = new Vector3List();
                waypoints.Add(pos);
                AiStateInfo aiInfo = npc.GetAiStateInfo();
                AiData_ForMoveCommand data = aiInfo.AiDatas.GetData<AiData_ForMoveCommand>();
                if (null == data)
                {
                    data = new AiData_ForMoveCommand(waypoints);
                    aiInfo.AiDatas.AddData(data);
                }
                data.WayPoints = waypoints;
                data.Index = 0;
                data.IsFinish = false;
                data.Event = eventName;
                aiInfo.TargetPosition = pos;
                aiInfo.Time = 1000;//下一帧即触发移动
                aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Pos.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Event.InitFromDsl(callData.GetParam(2));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryValue<string> m_Event = new StoryValue<string>();
    }
    /// <summary>
    /// npcmovewithwaypoints(npc_unit_id,vector3list("1 2 3 4 5 6")[,event]);
    /// </summary>
    internal class NpcMoveWithWaypointsCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcMoveWithWaypointsCommand cmd = new NpcMoveWithWaypointsCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_WayPoints = m_WayPoints.Clone();
            cmd.m_Event = m_Event.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_WayPoints.Evaluate(instance, iterator, args);
            m_Event.Evaluate(instance, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            List<object> poses = m_WayPoints.Value;
            string eventName = m_Event.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != npc && null != poses && poses.Count > 0)
            {
                Vector3List waypoints = new Vector3List();
                waypoints.Add(npc.GetMovementStateInfo().GetPosition3D());
                for (int i = 0; i < poses.Count; ++i)
                {
                    Vector3 pt = (Vector3)poses[i];
                    waypoints.Add(pt);
                }
                AiStateInfo aiInfo = npc.GetAiStateInfo();
                AiData_ForMoveCommand data = aiInfo.AiDatas.GetData<AiData_ForMoveCommand>();
                if (null == data)
                {
                    data = new AiData_ForMoveCommand(waypoints);
                    aiInfo.AiDatas.AddData(data);
                }
                data.WayPoints = waypoints;
                data.Index = 0;
                data.IsFinish = false;
                data.Event = eventName;
                aiInfo.TargetPosition = waypoints[0];
                aiInfo.Time = 1000;//下一帧即触发移动
                aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_WayPoints.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Event.InitFromDsl(callData.GetParam(2));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<List<object>> m_WayPoints = new StoryValue<List<object>>();
        private IStoryValue<string> m_Event = new StoryValue<string>();
    }
    /// <summary>
    /// npcstop(npc_unit_id);
    /// </summary>
    internal class NpcStopCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcStopCommand cmd = new NpcStopCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != npc)
            {
                AiStateInfo aiInfo = npc.GetAiStateInfo();
                if (aiInfo.CurState == (int)PredefinedAiStateId.MoveCommand)
                {
                    aiInfo.Time = 0;
                    aiInfo.Target = 0;
                }
                npc.GetMovementStateInfo().IsMoving = false;
                if (aiInfo.CurState > (int)PredefinedAiStateId.Invalid)
                    aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
    }
    /// <summary>
    /// npcattack(npc_unit_id[,target_unit_id]);
    /// </summary>
    internal class NpcAttackCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcAttackCommand cmd = new NpcAttackCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_TargetUnitId = m_TargetUnitId.Clone();
            cmd.m_ParamNum = m_ParamNum;
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_TargetUnitId.Evaluate(instance, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            EntityInfo target = null;
            int targetUnitId = m_TargetUnitId.Value;
            target = SceneSystem.Instance.GetEntityByUnitId(targetUnitId);
            if (null != npc && null != target)
            {
                AiStateInfo aiInfo = npc.GetAiStateInfo();
                aiInfo.Target = target.GetId();
                aiInfo.LastChangeTargetTime = TimeUtility.GetLocalMilliseconds();
                aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_TargetUnitId.InitFromDsl(callData.GetParam(1));
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_TargetUnitId = new StoryValue<int>();
    }
    /// <summary>
    /// npcenableai(npc_unit_id,true_or_false);
    /// </summary>
    internal class NpcEnableAiCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcEnableAiCommand cmd = new NpcEnableAiCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Enable = m_Enable.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Enable.Evaluate(instance, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(m_UnitId.Value);
            if (null != obj)
            {
                obj.SetAIEnable(m_Enable.Value != "false");
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Enable.InitFromDsl(callData.GetParam(1));
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<string> m_Enable = new StoryValue<string>();
    }
    /// <summary>
    /// npcsetai(unitid,ai_logic_id,stringlist("param1 param2 param3 ..."));
    /// </summary>
    internal class NpcSetAiCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetAiCommand cmd = new NpcSetAiCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_AiLogic = m_AiLogic.Clone();
            cmd.m_AiParams = m_AiParams.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_AiLogic.Evaluate(instance, iterator, args);
            m_AiParams.Evaluate(instance, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            string aiLogic = m_AiLogic.Value;
            IEnumerable aiParams = m_AiParams.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != charObj)
            {
                charObj.GetAiStateInfo().Reset();
                charObj.GetAiStateInfo().AiLogic = aiLogic;
                int ix = 0;
                foreach (string aiParam in aiParams)
                {
                    if (ix < AiStateInfo.c_MaxAiParamNum)
                    {
                        charObj.GetAiStateInfo().AiParam[ix] = aiParam;
                        ++ix;
                    }
                    else
                    {
                        break;
                    }
                }
                SceneSystem.Instance.UpdateAiLogic(charObj);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_AiLogic.InitFromDsl(callData.GetParam(1));
                m_AiParams.InitFromDsl(callData.GetParam(2));
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<string> m_AiLogic = new StoryValue<string>();
        private IStoryValue<IEnumerable> m_AiParams = new StoryValue<IEnumerable>();
    }
    /// <summary>
    /// npcsetaitarget(unitid,targetId);
    /// </summary>
    internal class NpcSetAiTargetCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetAiTargetCommand cmd = new NpcSetAiTargetCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_TargetId = m_TargetId.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_TargetId.Evaluate(instance, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            int targetId = m_TargetId.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != charObj)
            {
                charObj.GetAiStateInfo().Target = targetId;
                charObj.GetAiStateInfo().HateTarget = targetId;
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_TargetId = new StoryValue<int>();
    }
    /// <summary>
    /// npcanimation(unit_id, anim[, normalized_time]);
    /// </summary>
    internal class NpcAnimationCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcAnimationCommand cmd = new NpcAnimationCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Anim = m_Anim.Clone();
            cmd.m_Time = m_Time.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Anim.Evaluate(instance, iterator, args);
            if (m_ParamNum > 2)
            {
                m_Time.Evaluate(instance, iterator, args);
            }

            if (m_ParamNum > 2)
            {
            }
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            string anim = m_Anim.Value;
            EntityViewModel view = SceneSystem.Instance.GetEntityViewByUnitId(unitId);
            if (null != view) {
                view.PlayAnimation(anim);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Anim.InitFromDsl(callData.GetParam(1));
            }
            if (m_ParamNum > 2)
            {
                m_Time.InitFromDsl(callData.GetParam(2));
            }
        }
        private int m_ParamNum = 0;
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<string> m_Anim = new StoryValue<string>();
        private IStoryValue<float> m_Time = new StoryValue<float>();
    }
    /// <summary>
    /// npcanimationparam(unit_id)
    /// {
    ///     float(name,val);
    ///     int(name,val);
    ///     bool(name,val);
    ///     trigger(name,val);
    /// };
    /// </summary>
    internal class NpcAnimationParamCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcAnimationParamCommand cmd = new NpcAnimationParamCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            for (int i = 0; i < m_Params.Count; ++i)
            {
                ParamInfo param = new ParamInfo();
                param.CopyFrom(m_Params[i]);
                cmd.m_Params.Add(param);
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            for (int i = 0; i < m_Params.Count; ++i)
            {
                var pair = m_Params[i];
                pair.Key.Evaluate(instance, iterator, args);
                pair.Value.Evaluate(instance, iterator, args);
            }

            for (int i = 0; i < m_Params.Count; ++i)
            {
                var pair = m_Params[i];
            }
        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            int unitId = m_UnitId.Value;
            UnityEngine.GameObject obj = SceneSystem.Instance.GetGameObjectByUnitId(unitId);
            if (null != obj)
            {
                UnityEngine.Animator animator = obj.GetComponentInChildren<UnityEngine.Animator>();
                if (null != animator)
                {
                    for (int i = 0; i < m_Params.Count; ++i)
                    {
                        var param = m_Params[i];
                        string type = param.Type;
                        string key = param.Key.Value;
                        object val = param.Value.Value;
                        if (type == "int")
                        {
                            int v = (int)Convert.ChangeType(val, typeof(int));
                            animator.SetInteger(key, v);
                        }
                        else if (type == "float")
                        {
                            float v = (float)Convert.ChangeType(val, typeof(float));
                            animator.SetFloat(key, v);
                        }
                        else if (type == "bool")
                        {
                            bool v = (bool)Convert.ChangeType(val, typeof(bool));
                            animator.SetBool(key, v);
                        }
                        else if (type == "trigger")
                        {
                            string v = val.ToString();
                            if (v == "false")
                            {
                                animator.ResetTrigger(key);
                            }
                            else
                            {
                                animator.SetTrigger(key);
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }
        protected override void Load(Dsl.FunctionData funcData)
        {
            Dsl.CallData callData = funcData.Call;
            if (null != callData)
            {
                Load(callData);
                for (int i = 0; i < funcData.Statements.Count; ++i)
                {
                    Dsl.ISyntaxComponent statement = funcData.Statements[i];
                    Dsl.CallData stCall = statement as Dsl.CallData;
                    if (null != stCall && stCall.GetParamNum() >= 2)
                    {
                        string id = stCall.GetId();
                        ParamInfo param = new ParamInfo(id, stCall.GetParam(0), stCall.GetParam(1));
                        m_Params.Add(param);
                    }
                }
            }
        }
        private class ParamInfo
        {
            internal string Type;
            internal IStoryValue<string> Key;
            internal IStoryValue<object> Value;
            internal ParamInfo()
            {
                Init();
            }
            internal ParamInfo(string type, Dsl.ISyntaxComponent keyDsl, Dsl.ISyntaxComponent valDsl) : this()
            {
                Type = type;
                Key.InitFromDsl(keyDsl);
                Value.InitFromDsl(valDsl);
            }
            internal void CopyFrom(ParamInfo other)
            {
                Type = other.Type;
                Key = other.Key.Clone();
                Value = other.Value.Clone();
            }
            private void Init()
            {
                Type = string.Empty;
                Key = new StoryValue<string>();
                Value = new StoryValue();
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }
    /// <summary>
    /// setcamp(npc_unit_id,camp_id);
    /// </summary>
    internal class NpcSetCampCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetCampCommand cmd = new NpcSetCampCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_CampId = m_CampId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_CampId.Evaluate(instance, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(m_UnitId.Value);
            if (null != obj)
            {
                int campId = m_CampId.Value;
                obj.SetCampId(campId);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_CampId.InitFromDsl(callData.GetParam(1));
            }
        }
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_CampId = new StoryValue<int>();
    }
}
