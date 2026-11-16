using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;
using UnityEngine;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// create_npc(unit_id, vector_3(x,y,z), dir, model, type[, ai, string_list("param1 param2...")])[objid("varname")] - create an NPC
    /// create_npc(unit_id, vector_3(x,y,z), dir, model, type[, ai, string_list("param1 param2...")]) {
    ///     objid("varname");
    /// };
    /// </summary>
    internal sealed class CreateNpcExp : AbstractExpression
    {
        protected override BoxedValue DoCalc(DslCalculator calculator)
        {
            int unitId = m_UnitId.Calc(calculator).GetInt();
            Vector3Obj posObj = m_Pos.Calc(calculator);
            Vector3 pos = posObj;
            float dir = m_Dir.Calc(calculator).GetFloat();
            string model = m_Model.Calc(calculator).ToString();
            int entityType = m_Type.Calc(calculator).GetInt();

            // Destroy existing entity with same unitId
            var entityInfo = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != entityInfo)
            {
                SceneSystem.Instance.DestroyEntity(entityInfo);
            }

            int objId = 0;
            if (m_HaveAi)
            {
                // With AI
                string aiLogic = m_AiLogic.Calc(calculator).ToString();
                IEnumerable aiParams = m_AiParams.Calc(calculator).GetObject() as IEnumerable;
                List<string> ais = new List<string>();
                if (aiParams != null)
                {
                    foreach (var aiParam in aiParams)
                    {
                        ais.Add(aiParam.ToString());
                    }
                }
                objId = SceneSystem.Instance.CreateEntityWithAi(unitId, pos.x, pos.y, pos.z, dir, model, (EntityTypeEnum)entityType, aiLogic, ais.ToArray());
            }
            else
            {
                objId = SceneSystem.Instance.CreateEntity(unitId, pos.x, pos.y, pos.z, dir, model, (EntityTypeEnum)entityType);
            }

            if (m_HaveObjId && objId != 0)
            {
                string varName = m_ObjIdVarName.Calc(calculator).ToString();
                var storyInst = calculator.GetFuncContext<StoryInstance>();
                if (null != storyInst)
                {
                    storyInst.SetVariable(varName, objId);
                }
            }

            return BoxedValue.FromObject(objId);
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
            {
                callData = funcData.LowerOrderFunction;
                LoadObjId(funcData.LowerOrderFunction);
            }
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum >= 5)
            {
                m_UnitId = CodeStructure.Load(callData.GetParam(0));
                m_Pos = CodeStructure.Load(callData.GetParam(1));
                m_Dir = CodeStructure.Load(callData.GetParam(2));
                m_Model = CodeStructure.Load(callData.GetParam(3));
                m_Type = CodeStructure.Load(callData.GetParam(4));
                if (m_ParamNum > 6)
                {
                    m_AiLogic = CodeStructure.Load(callData.GetParam(5));
                    m_AiParams = CodeStructure.Load(callData.GetParam(6));
                    m_HaveAi = true;
                }
            }
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd)
                    {
                        LoadObjId(cd);
                    }
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
                    LoadObjId(second);
                }
            }
            return true;
        }

        private void LoadObjId(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "objid" && callData.GetParamNum() == 1)
            {
                m_ObjIdVarName = CodeStructure.Load(callData.GetParam(0));
                m_HaveObjId = true;
            }
        }

        private IExpression m_UnitId;
        private IExpression m_Pos;
        private IExpression m_Dir;
        private IExpression m_Model;
        private IExpression m_Type;
        private IExpression m_AiLogic;
        private IExpression m_AiParams;
        private bool m_HaveAi = false;
        private int m_ParamNum = 0;
        private bool m_HaveObjId = false;
        private IExpression m_ObjIdVarName;
    }

    /// <summary>
    /// destroy_npc(unit_id[, immediately]) - destroy an NPC by unit ID
    /// </summary>
    internal sealed class DestroyNpcExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: destroy_npc(unit_id[, immediately])");

            int unitId = operands[0].GetInt();
            bool immediately = operands.Count > 1 ? operands[1].GetBool() : false;

            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != npc)
            {
                if (immediately)
                {
                    SceneSystem.Instance.DestroyEntity(npc);
                }
                else
                {
                    npc.NeedDelete = true;
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_face(unit_id, dir[, immediately]) - set NPC face direction
    /// </summary>
    internal sealed class NpcFaceExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_face(unit_id, dir[, immediately])");

            int unitId = operands[0].GetInt();
            float dir = operands[1].GetFloat();

            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != npc)
            {
                npc.GetMovementStateInfo().SetFaceDir(dir);
                var uobj = npc.View.Actor;
                if (null != uobj)
                {
                    var e = uobj.transform.eulerAngles;
                    uobj.transform.eulerAngles = new UnityEngine.Vector3(e.x, Geometry.RadianToDegree(dir), e.z);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_stop(unit_id) - stop NPC movement
    /// </summary>
    internal sealed class NpcStopExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: npc_stop(unit_id)");

            int unitId = operands[0].GetInt();
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
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_move(unit_id, vector_3(x,y,z)[, event]) - move NPC to position
    /// </summary>
    internal sealed class NpcMoveExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_move(unit_id, pos[, event])");

            int unitId = operands[0].GetInt();
            Vector3Obj posObj = operands[1];
            Vector3 pos = posObj;
            string eventName = operands.Count > 2 ? operands[2].ToString() : "";

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
                aiInfo.Time = 1000; // Movement is triggered on the next frame
                aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_move_with_waypoints(unit_id, waypoints[, event]) - move NPC along waypoints
    /// </summary>
    internal sealed class NpcMoveWithWaypointsExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_move_with_waypoints(unit_id, waypoints[, event])");

            int unitId = operands[0].GetInt();
            var poses = operands[1].GetObject() as List<object>;
            string eventName = operands.Count > 2 ? operands[2].ToString() : "";

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
                aiInfo.Time = 1000; // Movement is triggered on the next frame
                aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_attack(unit_id[, target_unit_id]) - command NPC to attack target
    /// </summary>
    internal sealed class NpcAttackExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: npc_attack(unit_id[, target_unit_id])");

            int unitId = operands[0].GetInt();
            int targetUnitId = operands.Count > 1 ? operands[1].GetInt() : 0;

            EntityInfo npc = SceneSystem.Instance.GetEntityByUnitId(unitId);
            EntityInfo target = SceneSystem.Instance.GetEntityByUnitId(targetUnitId);
            if (null != npc && null != target)
            {
                AiStateInfo aiInfo = npc.GetAiStateInfo();
                aiInfo.Target = target.GetId();
                aiInfo.LastChangeTargetTime = TimeUtility.GetLocalMilliseconds();
                aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_enable_ai(unit_id, enable) - enable/disable NPC AI
    /// </summary>
    internal sealed class NpcEnableAiExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_enable_ai(unit_id, enable)");

            int unitId = operands[0].GetInt();
            int enable = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != obj)
            {
                obj.SetAIEnable(enable != 0);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_set_ai(unit_id, ai_logic, ai_params) - set NPC AI logic and parameters
    /// </summary>
    internal sealed class NpcSetAiExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: npc_set_ai(unit_id, ai_logic, ai_params)");

            int unitId = operands[0].GetInt();
            string aiLogic = operands[1].ToString();
            IEnumerable aiParams = operands[2].GetObject() as IEnumerable;

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
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_set_ai_target(unit_id, target_id) - set NPC AI target
    /// </summary>
    internal sealed class NpcSetAiTargetExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_set_ai_target(unit_id, target_id)");

            int unitId = operands[0].GetInt();
            int targetId = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != charObj)
            {
                charObj.GetAiStateInfo().Target = targetId;
                charObj.GetAiStateInfo().HateTarget = targetId;
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_set_camp(unit_id, camp_id) - set NPC camp
    /// </summary>
    internal sealed class NpcSetCampExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_set_camp(unit_id, camp_id)");

            int unitId = operands[0].GetInt();
            int campId = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != obj)
            {
                obj.SetCampId(campId);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// destroy_npc_with_obj_id(objid[, immediately]) - destroy an NPC by object ID
    /// </summary>
    internal sealed class DestroyNpcWithObjIdExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: destroy_npc_with_obj_id(objid[, immediately])");

            int objId = operands[0].GetInt();
            bool immediately = operands.Count > 1 ? operands[1].GetBool() : false;

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                if (immediately)
                {
                    SceneSystem.Instance.DestroyEntity(npc);
                }
                else
                {
                    npc.NeedDelete = true;
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_animation(unit_id, anim[, normalized_time]) - play NPC animation
    /// </summary>
    internal sealed class NpcAnimationExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_animation(unit_id, anim[, normalized_time])");

            int unitId = operands[0].GetInt();
            string anim = operands[1].ToString();

            var view = SceneSystem.Instance.GetEntityViewByUnitId(unitId);
            if (null != view)
            {
                view.PlayAnimation(anim);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// npc_get_npc_type(unitid) - get NPC entity type
    /// </summary>
    internal sealed class NpcGetNpcTypeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: npc_get_npc_type(unitid)");

            int unitId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (obj != null)
                return obj.EntityType;
            return 0;
        }
    }

    /// <summary>
    /// npc_get_ai_param(unitid, index) - get NPC AI parameter
    /// </summary>
    internal sealed class NpcGetAiParamExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: npc_get_ai_param(unitid, index)");

            int unitId = operands[0].GetInt();
            int index = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (obj != null)
            {
                var aiState = obj.GetAiStateInfo();
                if (index >= 0 && index < aiState.AiParam.Length)
                    return aiState.AiParam[index];
            }
            return BoxedValue.NullObject;
        }
    }
}
