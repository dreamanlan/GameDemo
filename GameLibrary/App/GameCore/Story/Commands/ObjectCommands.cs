using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using UnityEngine;
using GameLibrary;

namespace GameLibrary.Story.Commands
{
    /// <summary>
    /// objface(obj_id, dir[, immediately]);
    /// </summary>
    internal class ObjFaceCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjFaceCommand cmd = new ObjFaceCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Dir = m_Dir.Clone();
            cmd.m_Immediately = m_Immediately.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Dir.Evaluate(instance, handler, iterator, args);
            m_Immediately.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            float dir = m_Dir.Value;
            int im = 0;
            if(m_Immediately.HaveValue)
                im = m_Immediately.Value;
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj) {
                obj.GetMovementStateInfo().SetFaceDir(dir);
                var uobj = obj.View.Actor;
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
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Dir.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Immediately.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<float> m_Dir = new StoryValue<float>();
        private IStoryFunction<int> m_Immediately = new StoryValue<int>();
    }
    /// <summary>
    /// objmove(obj_id, vector3(x,y,z)[, event]);
    /// </summary>
    internal class ObjMoveCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjMoveCommand cmd = new ObjMoveCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Pos = m_Pos.Clone();
            cmd.m_Event = m_Event.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Pos.Evaluate(instance, handler, iterator, args);
            m_Event.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            Vector3 pos = m_Pos.Value;
            string eventName = m_Event.Value;
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj) {
                Vector3List waypoints = new Vector3List();
                waypoints.Add(pos);
                AiStateInfo aiInfo = obj.GetAiStateInfo();
                AiData_ForMoveCommand data = aiInfo.AiDatas.GetData<AiData_ForMoveCommand>();
                if (null == data) {
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
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Pos.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Event.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryFunction<string> m_Event = new StoryValue<string>();
    }
    /// <summary>
    /// objmovewithwaypoints(obj_id, vector3list("1 2 3 4 5 6")[, event]);
    /// </summary>
    internal class ObjMoveWithWaypointsCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjMoveWithWaypointsCommand cmd = new ObjMoveWithWaypointsCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_WayPoints = m_WayPoints.Clone();
            cmd.m_Event = m_Event.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_WayPoints.Evaluate(instance, handler, iterator, args);
            m_Event.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            List<object> poses = m_WayPoints.Value;
            string eventName = m_Event.Value;
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj && null != poses && poses.Count > 0) {
                Vector3List waypoints = new Vector3List();
                waypoints.Add(obj.GetMovementStateInfo().GetPosition3D());
                for (int i = 0; i < poses.Count; ++i) {
                    Vector3 pt = (Vector3)poses[i];
                    waypoints.Add(pt);
                }
                AiStateInfo aiInfo = obj.GetAiStateInfo();
                AiData_ForMoveCommand data = aiInfo.AiDatas.GetData<AiData_ForMoveCommand>();
                if (null == data) {
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
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_WayPoints.InitFromDsl(callData.GetParam(1));
                if (num > 2)
                    m_Event.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<List<object>> m_WayPoints = new StoryValue<List<object>>();
        private IStoryFunction<string> m_Event = new StoryValue<string>();
    }
    /// <summary>
    /// objstop(obj_id);
    /// </summary>
    internal class ObjStopCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjStopCommand cmd = new ObjStopCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj) {
                AiStateInfo aiInfo = obj.GetAiStateInfo();
                if (aiInfo.CurState == (int)PredefinedAiStateId.MoveCommand) {
                    aiInfo.Time = 0;
                    aiInfo.Target = 0;
                }
                obj.GetMovementStateInfo().IsMoving = false;
                if (aiInfo.CurState > (int)PredefinedAiStateId.Invalid)
                    aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
    }
    /// <summary>
    /// objattack(npc_obj_id[,target_obj_id]);
    /// </summary>
    internal class ObjAttackCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjAttackCommand cmd = new ObjAttackCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_TargetObjId = m_TargetObjId.Clone();
            cmd.m_ParamNum = m_ParamNum;
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_TargetObjId.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            int targetObjId = m_TargetObjId.Value;
            if (null != npc) {
                AiStateInfo aiInfo = npc.GetAiStateInfo();
                aiInfo.Target = targetObjId;
                aiInfo.LastChangeTargetTime = TimeUtility.GetLocalMilliseconds();
                aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_TargetObjId.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private int m_ParamNum = 0;
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_TargetObjId = new StoryValue<int>();
    }
    /// <summary>
    /// objenableai(obj_id, 1_or_0);
    /// </summary>
    internal class ObjEnableAiCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjEnableAiCommand cmd = new ObjEnableAiCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Enable = m_Enable.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Enable.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj) {
                obj.SetAIEnable(m_Enable.Value != 0);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Enable.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Enable = new StoryValue<int>();
    }
    /// <summary>
    /// objsetai(objid,ai_logic_id,stringlist("param1 param2 param3 ..."));
    /// </summary>
    internal class ObjSetAiCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjSetAiCommand cmd = new ObjSetAiCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_AiLogic = m_AiLogic.Clone();
            cmd.m_AiParams = m_AiParams.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_AiLogic.Evaluate(instance, handler, iterator, args);
            m_AiParams.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            string aiLogic = m_AiLogic.Value;
            IEnumerable aiParams = m_AiParams.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.GetAiStateInfo().Reset();
                charObj.GetAiStateInfo().AiLogic = aiLogic;
                int ix = 0;
                foreach (string aiParam in aiParams) {
                    if (ix < AiStateInfo.c_MaxAiParamNum) {
                        charObj.GetAiStateInfo().AiParam[ix] = aiParam;
                        ++ix;
                    } else {
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
            if (num > 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_AiLogic.InitFromDsl(callData.GetParam(1));
                m_AiParams.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<string> m_AiLogic = new StoryValue<string>();
        private IStoryFunction<IEnumerable> m_AiParams = new StoryValue<IEnumerable>();
    }
    /// <summary>
    /// objsetaitarget(objid,targetid);
    /// </summary>
    internal class ObjSetAiTargetCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjSetAiTargetCommand cmd = new ObjSetAiTargetCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_TargetId = m_TargetId.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_TargetId.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int targetId = m_TargetId.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.GetAiStateInfo().Target = targetId;
                charObj.GetAiStateInfo().HateTarget = targetId;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_TargetId = new StoryValue<int>();
    }
    /// <summary>
    /// objanimation(obj_id, anim[, normalized_time]);
    /// </summary>
    internal class ObjAnimationCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjAnimationCommand cmd = new ObjAnimationCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Anim = m_Anim.Clone();
            cmd.m_Time = m_Time.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Anim.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 2) {
                m_Time.Evaluate(instance, handler, iterator, args);
            }
        
            if (m_ParamNum > 2) {
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            string anim = m_Anim.Value;
            var view = SceneSystem.Instance.GetEntityViewById(objId);
            if (null != view) {
                view.PlayAnimation(anim);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Anim.InitFromDsl(callData.GetParam(1));
            }
            if (m_ParamNum > 2) {
                m_Time.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }
        private int m_ParamNum = 0;
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<string> m_Anim = new StoryValue<string>();
        private IStoryFunction<float> m_Time = new StoryValue<float>();
    }
    /// <summary>
    /// objanimationparam(obj_id)
    /// {
    ///     float(name,val);
    ///     int(name,val);
    ///     bool(name,val);
    ///     trigger(name,val);
    /// };
    /// </summary>
    internal class ObjAnimationParamCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjAnimationParamCommand cmd = new ObjAnimationParamCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            for (int i = 0; i < m_Params.Count; ++i) {
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
            m_ObjId.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Params.Count; ++i) {
                var pair = m_Params[i];
                pair.Key.Evaluate(instance, handler, iterator, args);
                pair.Value.Evaluate(instance, handler, iterator, args);
            }
        
            for (int i = 0; i < m_Params.Count; ++i) {
                var pair = m_Params[i];
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            UnityEngine.GameObject obj = SceneSystem.Instance.GetGameObject(objId);
            if (null != obj) {
                UnityEngine.Animator animator = obj.GetComponentInChildren<UnityEngine.Animator>();
                if (null != animator) {
                    for (int i = 0; i < m_Params.Count; ++i) {
                        var param = m_Params[i];
                        string type = param.Type;
                        string key = param.Key.Value;
                        var val = param.Value.Value;
                        if (type == "int") {
                            int v = val.GetInt();
                            animator.SetInteger(key, v);
                        } else if (type == "float") {
                            float v = val.GetFloat();
                            animator.SetFloat(key, v);
                        } else if (type == "bool") {
                            bool v = val.GetBool();
                            animator.SetBool(key, v);
                        } else if (type == "trigger") {
                            string v = val.ToString();
                            if (v == "false") {
                                animator.ResetTrigger(key);
                            } else {
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
                for (int i = 0; i < funcData.GetParamNum(); ++i) {
                    Dsl.ISyntaxComponent statement = funcData.GetParam(i);
                    Dsl.FunctionData stCall = statement as Dsl.FunctionData;
                    if (null != stCall && stCall.GetParamNum() >= 2) {
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
                m_ObjId.InitFromDsl(callData.GetParam(0));
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
            internal ParamInfo(string type, Dsl.ISyntaxComponent keyDsl, Dsl.ISyntaxComponent valDsl)
                : this()
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
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }
    /// <summary>
    /// sethp(objid,value);
    /// </summary>
    internal class SetHpCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetHpCommand cmd = new SetHpCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.Hp = value;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Value = new StoryValue<int>();
    }
    /// <summary>
    /// setmaxhp(objid,value);
    /// </summary>
    internal class SetMaxHpCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetMaxHpCommand cmd = new SetMaxHpCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.HpMax = value;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Value = new StoryValue<int>();
    }
    /// <summary>
    /// setenergy(objid,value);
    /// </summary>
    internal class SetEnergyCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetEnergyCommand cmd = new SetEnergyCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.Energy = value;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Value = new StoryValue<int>();
    }
    /// <summary>
    /// setmaxenergy(objid,value);
    /// </summary>
    internal class SetMaxEnergyCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetMaxEnergyCommand cmd = new SetMaxEnergyCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.EnergyMax = value;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Value = new StoryValue<int>();
    }
    /// <summary>
    /// setspeed(objid,value);
    /// </summary>
    internal class SetSpeedCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetSpeedCommand cmd = new SetSpeedCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            float value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.Speed = value;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<float> m_Value = new StoryValue<float>();
    }
    /// <summary>
    /// setlevel(objid,value);
    /// </summary>
    internal class SetLevelCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetLevelCommand cmd = new SetLevelCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.Level = value;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Value = new StoryValue<int>();
    }
    /// <summary>
    /// setexp(objid,value);
    /// </summary>
    internal class SetExpCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetExpCommand cmd = new SetExpCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);

        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.Exp = value;
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_Value = new StoryValue<int>();
    }
    /// <summary>
    /// setattr(objid,attrid,value);
    /// </summary>
    internal class SetAttrCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetAttrCommand cmd = new SetAttrCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_AttrId = m_AttrId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_AttrId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int attrId = m_AttrId.Value;
            long val = m_Value.Value.GetLong();
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                try {
                    charObj.Property.SetLong((CharacterPropertyEnum)attrId, val);
                } catch (Exception ex) {
                    LogSystem.Error("setattr throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_AttrId.InitFromDsl(callData.GetParam(1));
                m_Value.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_AttrId = new StoryValue<int>();
        private IStoryFunction m_Value = new StoryValue();
    }
    /// <summary>
    /// markcontrolbystory(objid,true_or_false);
    /// </summary>
    internal class MarkControlByStoryCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            MarkControlByStoryCommand cmd = new MarkControlByStoryCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_Value = m_Value.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_Value.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            string value = m_Value.Value;
            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj) {
                charObj.IsControlByStory = (0 == value.CompareTo("true"));
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_Value.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<string> m_Value = new StoryValue<string>();
    }
    /// <summary>
    /// setunitid(obj_id, dir);
    /// </summary>
    internal class SetUnitIdCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetUnitIdCommand cmd = new SetUnitIdCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_UnitId = m_UnitId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_UnitId.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            int objId = m_ObjId.Value;
            int unitId = m_UnitId.Value;
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj) {
                obj.SetUnitId(unitId);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_UnitId.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_UnitId = new StoryValue<int>();
    }
    /// <summary>
    /// objsetcamp(objid,camp_id);
    /// </summary>
    internal class ObjSetCampCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            ObjSetCampCommand cmd = new ObjSetCampCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            cmd.m_CampId = m_CampId.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_CampId.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            EntityInfo obj = SceneSystem.Instance.GetEntityById(m_ObjId.Value);
            if (null != obj) {
                int campId = m_CampId.Value;
                obj.SetCampId(campId);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
                m_CampId.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        private IStoryFunction<int> m_ObjId = new StoryValue<int>();
        private IStoryFunction<int> m_CampId = new StoryValue<int>();
    }
}
