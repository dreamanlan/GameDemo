﻿using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using UnityEngine;
using GameLibrary;

namespace GameLibrary.Story.Commands
{
    /// <summary>
    /// createnpc(npc_unit_id,vector3(x,y,z),dir,model,type[,ai,stringlist("param1 param2 param3 ...")])[objid("@objid")];
    /// </summary>
    internal class CreateNpcCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            if (m_ParamNum >= 5)
            {
                m_UnitId.Evaluate(instance, handler, iterator, args);
                m_Pos.Evaluate(instance, handler, iterator, args);
                m_Dir.Evaluate(instance, handler, iterator, args);
                m_Model.Evaluate(instance, handler, iterator, args);
                m_Type.Evaluate(instance, handler, iterator, args);
                if (m_ParamNum > 6)
                {
                    m_AiLogic.Evaluate(instance, handler, iterator, args);
                    m_AiParams.Evaluate(instance, handler, iterator, args);
                }
            }
            if (m_HaveObjId)
            {
                m_ObjIdVarName.Evaluate(instance, handler, iterator, args);
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
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
                    if (aiParams is List<BoxedValue> bvAiParams) {
                        foreach (var aiParam in bvAiParams) {
                            ais.Add(aiParam.GetString());
                        }
                    }
                    else {
                        foreach (var aiParam in aiParams) {
                            ais.Add(aiParam as string);
                        }
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
        protected override bool Load(Dsl.FunctionData callData)
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
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2)
            {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                if (null != first && null != second)
                {
                    Load(first);
                    LoadVarName(second);
                }
            }
            return true;
        }
        private void LoadVarName(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "objid" && callData.GetParamNum() == 1)
            {
                m_ObjIdVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObjId = true;
            }
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private int m_ParamNum = 0;
        private IStoryFunction<Vector3> m_Pos = new StoryFunction<Vector3>();
        private IStoryFunction<float> m_Dir = new StoryFunction<float>();
        private IStoryFunction<string> m_Model = new StoryFunction<string>();
        private IStoryFunction<int> m_Type = new StoryFunction<int>();
        private IStoryFunction<string> m_AiLogic = new StoryFunction<string>();
        private IStoryFunction<IEnumerable> m_AiParams = new StoryFunction<IEnumerable>();
        private bool m_HaveObjId = false;
        private IStoryFunction<string> m_ObjIdVarName = new StoryFunction<string>();
    }
    /// <summary>
    /// destroynpc(npc_unit_id[,immediately]);
    /// </summary>
    internal class DestroyNpcCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            DestroyNpcCommand cmd = new DestroyNpcCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Immediately = m_Immediately.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_Immediately.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
            if (num > 1) {
                m_Immediately.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<bool> m_Immediately = new StoryFunction<bool>();
    }
    /// <summary>
    /// destroynpcwithobjid(npc_obj_id[,immediately]);
    /// </summary>
    internal class DestroyNpcWithObjIdCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            DestroyNpcWithObjIdCommand cmd = new DestroyNpcWithObjIdCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Immediately = m_Immediately.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Immediately.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
            if (num > 1) {
                m_Immediately.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryFunction<int>();
        private IStoryFunction<bool> m_Immediately = new StoryFunction<bool>();
    }
    /// <summary>
    /// npcface(npc_unit_id, dir[, immediately]);
    /// </summary>
    internal class NpcFaceCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_Dir.Evaluate(instance, handler, iterator, args);
            m_Immediately.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Dir.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Immediately.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<float> m_Dir = new StoryFunction<float>();
        private IStoryFunction<int> m_Immediately = new StoryFunction<int>();
    }
    /// <summary>
    /// npcmove(npc_unit_id,vector3(x,y,z)[,event]);
    /// </summary>
    internal class NpcMoveCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_Pos.Evaluate(instance, handler, iterator, args);
            m_Event.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
                aiInfo.Time = 1000;//Movement is triggered on the next frame
                aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Pos.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Event.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<Vector3> m_Pos = new StoryFunction<Vector3>();
        private IStoryFunction<string> m_Event = new StoryFunction<string>();
    }
    /// <summary>
    /// npcmovewithwaypoints(npc_unit_id,vector3list("1 2 3 4 5 6")[,event]);
    /// </summary>
    internal class NpcMoveWithWaypointsCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_WayPoints.Evaluate(instance, handler, iterator, args);
            m_Event.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
                aiInfo.Time = 1000;//Movement is triggered on the next frame
                aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_WayPoints.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Event.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<List<object>> m_WayPoints = new StoryFunction<List<object>>();
        private IStoryFunction<string> m_Event = new StoryFunction<string>();
    }
    /// <summary>
    /// npcstop(npc_unit_id);
    /// </summary>
    internal class NpcStopCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            NpcStopCommand cmd = new NpcStopCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
    }
    /// <summary>
    /// npcattack(npc_unit_id[,target_unit_id]);
    /// </summary>
    internal class NpcAttackCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_TargetUnitId.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
        protected override bool Load(Dsl.FunctionData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_TargetUnitId.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private int m_ParamNum = 0;
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<int> m_TargetUnitId = new StoryFunction<int>();
    }
    /// <summary>
    /// npcenableai(npc_unit_id,1_or_0);
    /// </summary>
    internal class NpcEnableAiCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            NpcEnableAiCommand cmd = new NpcEnableAiCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Enable = m_Enable.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_Enable.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(m_UnitId.Value);
            if (null != obj)
            {
                obj.SetAIEnable(m_Enable.Value != 0);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Enable.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<int> m_Enable = new StoryFunction<int>();
    }
    /// <summary>
    /// npcsetai(unitid,ai_logic_id,stringlist("param1 param2 param3 ..."));
    /// </summary>
    internal class NpcSetAiCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            NpcSetAiCommand cmd = new NpcSetAiCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_AiLogic = m_AiLogic.Clone();
            cmd.m_AiParams = m_AiParams.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_AiLogic.Evaluate(instance, handler, iterator, args);
            m_AiParams.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_AiLogic.InitFromDsl(callData.GetParam(1));
                m_AiParams.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<string> m_AiLogic = new StoryFunction<string>();
        private IStoryFunction<IEnumerable> m_AiParams = new StoryFunction<IEnumerable>();
    }
    /// <summary>
    /// npcsetaitarget(unitid,targetId);
    /// </summary>
    internal class NpcSetAiTargetCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            NpcSetAiTargetCommand cmd = new NpcSetAiTargetCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_TargetId = m_TargetId.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_TargetId.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<int> m_TargetId = new StoryFunction<int>();
    }
    /// <summary>
    /// npcanimation(unit_id, anim[, normalized_time]);
    /// </summary>
    internal class NpcAnimationCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_Anim.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 2)
            {
                m_Time.Evaluate(instance, handler, iterator, args);
            }

            if (m_ParamNum > 2)
            {
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int unitId = m_UnitId.Value;
            string anim = m_Anim.Value;
            EntityViewModel view = SceneSystem.Instance.GetEntityViewByUnitId(unitId);
            if (null != view) {
                view.PlayAnimation(anim);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
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
            return true;
        }
        private int m_ParamNum = 0;
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<string> m_Anim = new StoryFunction<string>();
        private IStoryFunction<float> m_Time = new StoryFunction<float>();
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
        protected override IStoryCommand CloneCommand()
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Params.Count; ++i)
            {
                var pair = m_Params[i];
                pair.Key.Evaluate(instance, handler, iterator, args);
                pair.Value.Evaluate(instance, handler, iterator, args);
            }

            for (int i = 0; i < m_Params.Count; ++i)
            {
                var pair = m_Params[i];
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
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
                        var val = param.Value.Value;
                        if (type == "int")
                        {
                            int v = val.GetInt();
                            animator.SetInteger(key, v);
                        }
                        else if (type == "float")
                        {
                            float v = val.GetFloat();
                            animator.SetFloat(key, v);
                        }
                        else if (type == "bool")
                        {
                            bool v = val.GetBool();
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
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                LoadCall(funcData.LowerOrderFunction);
            }
            else if (funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.ISyntaxComponent statement = funcData.GetParam(i);
                    Dsl.FunctionData stCall = statement as Dsl.FunctionData;
                    if (null != stCall && stCall.GetParamNum() >= 2)
                    {
                        string id = stCall.GetId();
                        ParamInfo param = new ParamInfo(id, stCall.GetParam(0), stCall.GetParam(1));
                        m_Params.Add(param);
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }
        private class ParamInfo
        {
            internal string Type;
            internal IStoryFunction<string> Key;
            internal IStoryFunction Value;
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
                Key = new StoryFunction<string>();
                Value = new StoryFunction();
            }
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }
    /// <summary>
    /// setcamp(npc_unit_id,camp_id);
    /// </summary>
    internal class NpcSetCampCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            NpcSetCampCommand cmd = new NpcSetCampCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_CampId = m_CampId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_UnitId.Evaluate(instance, handler, iterator, args);
            m_CampId.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(m_UnitId.Value);
            if (null != obj)
            {
                int campId = m_CampId.Value;
                obj.SetCampId(campId);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1)
            {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_CampId.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_UnitId = new StoryFunction<int>();
        private IStoryFunction<int> m_CampId = new StoryFunction<int>();
    }
}
