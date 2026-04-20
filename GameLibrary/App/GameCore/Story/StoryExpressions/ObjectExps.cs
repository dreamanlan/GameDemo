using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// objface(obj_id, dir[, immediately]) - set object face direction
    /// </summary>
    internal sealed class ObjFaceExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objface(obj_id, dir[, immediately])");

            int objId = operands[0].GetInt();
            float dir = operands[1].GetFloat();

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj)
            {
                obj.GetMovementStateInfo().SetFaceDir(dir);
                var uobj = obj.View.Actor;
                if (null != uobj)
                {
                    var e = uobj.transform.eulerAngles;
                    uobj.transform.eulerAngles = new UnityEngine.Vector3(e.x, Geometry.RadianToDegree(dir), e.z);
                }
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objstop(obj_id) - stop object movement
    /// </summary>
    internal sealed class ObjStopExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: objstop(obj_id)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj)
            {
                AiStateInfo aiInfo = obj.GetAiStateInfo();
                if (aiInfo.CurState == (int)PredefinedAiStateId.MoveCommand)
                {
                    aiInfo.Time = 0;
                    aiInfo.Target = 0;
                }
                obj.GetMovementStateInfo().IsMoving = false;
                if (aiInfo.CurState > (int)PredefinedAiStateId.Invalid)
                    aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// sethp(objid, value) - set entity HP
    /// </summary>
    internal sealed class SetHpExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: sethp(objid, value)");

            int objId = operands[0].GetInt();
            int value = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.Hp = value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// setmaxhp(objid, value) - set entity max HP
    /// </summary>
    internal sealed class SetMaxHpExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setmaxhp(objid, value)");

            int objId = operands[0].GetInt();
            int value = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.HpMax = value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// gethp(objid) - get entity HP
    /// </summary>
    internal sealed class GetHpExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: gethp(objid)");

            int objId = operands[0].GetInt();
            int hp = 0;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                hp = charObj.Hp;
            }
            return hp;
        }
    }

    /// <summary>
    /// getmaxhp(objid) - get entity max HP
    /// </summary>
    internal sealed class GetMaxHpExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getmaxhp(objid)");

            int objId = operands[0].GetInt();
            int maxHp = 0;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                maxHp = charObj.HpMax;
            }
            return maxHp;
        }
    }

    /// <summary>
    /// setlevel(objid, value) - set entity level
    /// </summary>
    internal sealed class SetLevelExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setlevel(objid, value)");

            int objId = operands[0].GetInt();
            int value = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.Level = value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// getlevel(objid) - get entity level
    /// </summary>
    internal sealed class GetLevelExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getlevel(objid)");

            int objId = operands[0].GetInt();
            int level = 0;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                level = charObj.Level;
            }
            return level;
        }
    }

    /// <summary>
    /// setunitid(obj_id, unit_id) - set entity unit ID
    /// </summary>
    internal sealed class SetUnitIdExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setunitid(obj_id, unit_id)");

            int objId = operands[0].GetInt();
            int unitId = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj)
            {
                obj.SetUnitId(unitId);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objsetcamp(objid, camp_id) - set entity camp
    /// </summary>
    internal sealed class ObjSetCampExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objsetcamp(objid, camp_id)");

            int objId = operands[0].GetInt();
            int campId = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj)
            {
                obj.SetCampId(campId);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objmove(obj_id, vector3(x,y,z)[, event]) - move object to position
    /// </summary>
    internal sealed class ObjMoveExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objmove(obj_id, pos[, event])");

            int objId = operands[0].GetInt();
            Vector3Obj posObj = operands[1];
            Vector3 pos = posObj;
            string eventName = operands.Count > 2 ? operands[2].ToString() : "";

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj)
            {
                Vector3List waypoints = new Vector3List();
                waypoints.Add(pos);
                AiStateInfo aiInfo = obj.GetAiStateInfo();
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
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objmovewithwaypoints(obj_id, waypoints[, event]) - move object along waypoints
    /// </summary>
    internal sealed class ObjMoveWithWaypointsExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objmovewithwaypoints(obj_id, waypoints[, event])");

            int objId = operands[0].GetInt();
            var poses = operands[1].GetObject() as List<object>;
            string eventName = operands.Count > 2 ? operands[2].ToString() : "";

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj && null != poses && poses.Count > 0)
            {
                Vector3List waypoints = new Vector3List();
                waypoints.Add(obj.GetMovementStateInfo().GetPosition3D());
                for (int i = 0; i < poses.Count; ++i)
                {
                    Vector3 pt = (Vector3)poses[i];
                    waypoints.Add(pt);
                }
                AiStateInfo aiInfo = obj.GetAiStateInfo();
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
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objattack(obj_id[, target_obj_id]) - command object to attack target
    /// </summary>
    internal sealed class ObjAttackExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: objattack(obj_id[, target_obj_id])");

            int objId = operands[0].GetInt();
            int targetObjId = operands.Count > 1 ? operands[1].GetInt() : 0;

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            EntityInfo target = SceneSystem.Instance.GetEntityById(targetObjId);
            if (null != obj && null != target)
            {
                AiStateInfo aiInfo = obj.GetAiStateInfo();
                aiInfo.Target = target.GetId();
                aiInfo.LastChangeTargetTime = TimeUtility.GetLocalMilliseconds();
                aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objenableai(obj_id, enable) - enable/disable object AI
    /// </summary>
    internal sealed class ObjEnableAiExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objenableai(obj_id, enable)");

            int objId = operands[0].GetInt();
            int enable = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (null != obj)
            {
                obj.SetAIEnable(enable != 0);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objsetai(obj_id, ai_logic, ai_params) - set object AI logic and parameters
    /// </summary>
    internal sealed class ObjSetAiExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: objsetai(obj_id, ai_logic, ai_params)");

            int objId = operands[0].GetInt();
            string aiLogic = operands[1].ToString();
            IEnumerable aiParams = operands[2].GetObject() as IEnumerable;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
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
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objsetaitarget(obj_id, target_id) - set object AI target
    /// </summary>
    internal sealed class ObjSetAiTargetExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objsetaitarget(obj_id, target_id)");

            int objId = operands[0].GetInt();
            int targetId = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.GetAiStateInfo().Target = targetId;
                charObj.GetAiStateInfo().HateTarget = targetId;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// objanimation(obj_id, anim[, normalized_time]) - play object animation
    /// </summary>
    internal sealed class ObjAnimationExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objanimation(obj_id, anim[, normalized_time])");

            int objId = operands[0].GetInt();
            string anim = operands[1].ToString();

            var view = SceneSystem.Instance.GetEntityViewById(objId);
            if (null != view)
            {
                view.PlayAnimation(anim);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// setenergy(objid, value) - set entity energy
    /// </summary>
    internal sealed class SetEnergyExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setenergy(objid, value)");

            int objId = operands[0].GetInt();
            int value = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.Energy = value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// getenergy(objid) - get entity energy
    /// </summary>
    internal sealed class GetEnergyExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getenergy(objid)");

            int objId = operands[0].GetInt();
            int energy = 0;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                energy = charObj.Energy;
            }
            return energy;
        }
    }

    /// <summary>
    /// setmaxenergy(objid, value) - set entity max energy
    /// </summary>
    internal sealed class SetMaxEnergyExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setmaxenergy(objid, value)");

            int objId = operands[0].GetInt();
            int value = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.EnergyMax = value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// getmaxenergy(objid) - get entity max energy
    /// </summary>
    internal sealed class GetMaxEnergyExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getmaxenergy(objid)");

            int objId = operands[0].GetInt();
            int maxEnergy = 0;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                maxEnergy = charObj.EnergyMax;
            }
            return maxEnergy;
        }
    }

    /// <summary>
    /// setspeed(objid, value) - set entity speed
    /// </summary>
    internal sealed class SetSpeedExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setspeed(objid, value)");

            int objId = operands[0].GetInt();
            float value = operands[1].GetFloat();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.Speed = value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// getspeed(objid) - get entity speed
    /// </summary>
    internal sealed class GetSpeedExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getspeed(objid)");

            int objId = operands[0].GetInt();
            float speed = 0;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                speed = charObj.Speed;
            }
            return speed;
        }
    }

    /// <summary>
    /// setexp(objid, value) - set entity exp
    /// </summary>
    internal sealed class SetExpExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setexp(objid, value)");

            int objId = operands[0].GetInt();
            int value = operands[1].GetInt();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.Exp = value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// getexp(objid) - get entity exp
    /// </summary>
    internal sealed class GetExpExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getexp(objid)");

            int objId = operands[0].GetInt();
            int exp = 0;

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                exp = charObj.Exp;
            }
            return exp;
        }
    }

    /// <summary>
    /// setattr(objid, attrid, value) - set entity attribute
    /// </summary>
    internal sealed class SetAttrExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: setattr(objid, attrid, value)");

            int objId = operands[0].GetInt();
            int attrId = operands[1].GetInt();
            long val = operands[2].GetLong();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                try
                {
                    charObj.Property.SetLong((CharacterPropertyEnum)attrId, val);
                    return true;
                }
                catch (Exception ex)
                {
                    LogSystem.Error("setattr throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
            return false;
        }
    }

    /// <summary>
    /// markcontrolbystory(objid, true_or_false) - mark entity as controlled by story
    /// </summary>
    internal sealed class MarkControlByStoryExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: markcontrolbystory(objid, true_or_false)");

            int objId = operands[0].GetInt();
            string value = operands[1].ToString();

            EntityInfo charObj = SceneSystem.Instance.GetEntityById(objId);
            if (null != charObj)
            {
                charObj.IsControlByStory = (0 == value.CompareTo("true"));
                return true;
            }
            return false;
        }
    }

    // ========== Get Position/Rotation/Scale ==========

    /// <summary>
    /// getposition(obj) - get position of object
    /// </summary>
    internal sealed class GetPositionExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getposition(obj)");

            UnityEngine.Vector3 pos = UnityEngine.Vector3.zero;
            var objVal = operands[0];

            GameObject obj = null;
            if (objVal.IsObject)
                obj = objVal.ObjectVal as GameObject;
            else if (objVal.IsInteger)
            {
                int objId = objVal.GetInt();
                obj = SceneSystem.Instance.GetGameObject(objId);
            }

            if (obj != null)
                pos = obj.transform.position;

            Vector3Obj vecObj = new Vector3Obj { Value = pos };
            return BoxedValue.FromObject(vecObj);
        }
    }

    /// <summary>
    /// findallobjids(position, radius) - find all object IDs within radius
    /// </summary>
    internal sealed class FindAllObjIdsExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: findallobjids(position, radius)");

            Vector3Obj posObj = operands[0];
            UnityEngine.Vector3 pos = posObj;
            float radius = operands[1].GetFloat();

            List<EntityInfo> list = new List<EntityInfo>();
            if (SceneSystem.Instance.FindAllEntitiesByRange(pos.x, pos.z, radius, list)) {
                List<int> ids = new List<int>();
                foreach (var info in list) {
                    ids.Add(info.GetId());
                }
                return BoxedValue.FromObject(ids);
            }
            return BoxedValue.FromObject(new List<int>());
        }
    }

    /// <summary>
    /// getpositionx(obj_id[, local]) - get entity X position
    /// </summary>
    internal sealed class GetPositionXExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getpositionx(obj_id[, local])");

            int objId = operands[0].GetInt();
            bool local = operands.Count > 1 && operands[1].GetBool();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return local ? uobj.transform.localPosition.x : uobj.transform.position.x;
                return obj.GetMovementStateInfo().GetPosition3D().x;
            }
            return 0f;
        }
    }

    /// <summary>
    /// getpositiony(obj_id[, local]) - get entity Y position
    /// </summary>
    internal sealed class GetPositionYExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getpositiony(obj_id[, local])");

            int objId = operands[0].GetInt();
            bool local = operands.Count > 1 && operands[1].GetBool();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return local ? uobj.transform.localPosition.y : uobj.transform.position.y;
                return obj.GetMovementStateInfo().GetPosition3D().y;
            }
            return 0f;
        }
    }

    /// <summary>
    /// getpositionz(obj_id[, local]) - get entity Z position
    /// </summary>
    internal sealed class GetPositionZExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getpositionz(obj_id[, local])");

            int objId = operands[0].GetInt();
            bool local = operands.Count > 1 && operands[1].GetBool();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return local ? uobj.transform.localPosition.z : uobj.transform.position.z;
                return obj.GetMovementStateInfo().GetPosition3D().z;
            }
            return 0f;
        }
    }

    /// <summary>
    /// getrotation(obj_id[, local]) - get entity rotation as Vector3
    /// </summary>
    internal sealed class GetRotationExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getrotation(obj_id[, local])");

            int objId = operands[0].GetInt();
            bool local = operands.Count > 1 && operands[1].GetBool();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                {
                    Vector3 e = local ? uobj.transform.localEulerAngles : uobj.transform.eulerAngles;
                    Vector3Obj vecObj = new Vector3Obj { Value = e };
                    return BoxedValue.FromObject(vecObj);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// getrotationx(obj_id[, local]) - get entity rotation X
    /// </summary>
    internal sealed class GetRotationXExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getrotationx(obj_id[, local])");

            int objId = operands[0].GetInt();
            bool local = operands.Count > 1 && operands[1].GetBool();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return local ? uobj.transform.localEulerAngles.x : uobj.transform.eulerAngles.x;
            }
            return 0f;
        }
    }

    /// <summary>
    /// getrotationy(obj_id[, local]) - get entity rotation Y
    /// </summary>
    internal sealed class GetRotationYExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getrotationy(obj_id[, local])");

            int objId = operands[0].GetInt();
            bool local = operands.Count > 1 && operands[1].GetBool();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return local ? uobj.transform.localEulerAngles.y : uobj.transform.eulerAngles.y;
            }
            return 0f;
        }
    }

    /// <summary>
    /// getrotationz(obj_id[, local]) - get entity rotation Z
    /// </summary>
    internal sealed class GetRotationZExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getrotationz(obj_id[, local])");

            int objId = operands[0].GetInt();
            bool local = operands.Count > 1 && operands[1].GetBool();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return local ? uobj.transform.localEulerAngles.z : uobj.transform.eulerAngles.z;
            }
            return 0f;
        }
    }

    /// <summary>
    /// getscale(obj_id) - get entity scale as Vector3
    /// </summary>
    internal sealed class GetScaleExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getscale(obj_id)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                {
                    Vector3 s = uobj.transform.localScale;
                    Vector3Obj vecObj = new Vector3Obj { Value = s };
                    return BoxedValue.FromObject(vecObj);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// getscalex(obj_id) - get entity scale X
    /// </summary>
    internal sealed class GetScaleXExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getscalex(obj_id)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return uobj.transform.localScale.x;
            }
            return 1f;
        }
    }

    /// <summary>
    /// getscaley(obj_id) - get entity scale Y
    /// </summary>
    internal sealed class GetScaleYExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getscaley(obj_id)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return uobj.transform.localScale.y;
            }
            return 1f;
        }
    }

    /// <summary>
    /// getscalez(obj_id) - get entity scale Z
    /// </summary>
    internal sealed class GetScaleZExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getscalez(obj_id)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                var uobj = obj.View?.Actor;
                if (uobj != null)
                    return uobj.transform.localScale.z;
            }
            return 1f;
        }
    }

    /// <summary>
    /// position(x, y, z) - create a Vector3
    /// </summary>
    internal sealed class PositionExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: position(x, y, z)");

            float x = operands[0].GetFloat();
            float y = operands[1].GetFloat();
            float z = operands[2].GetFloat();
            Vector3Obj vecObj = new Vector3Obj { Value = new Vector3(x, y, z) };
            return BoxedValue.FromObject(vecObj);
        }
    }

    /// <summary>
    /// rotation(x, y, z) - create a rotation Vector3
    /// </summary>
    internal sealed class RotationExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: rotation(x, y, z)");

            float x = operands[0].GetFloat();
            float y = operands[1].GetFloat();
            float z = operands[2].GetFloat();
            Vector3Obj vecObj = new Vector3Obj { Value = new Vector3(x, y, z) };
            return BoxedValue.FromObject(vecObj);
        }
    }

    /// <summary>
    /// scale(x, y, z) - create a scale Vector3
    /// </summary>
    internal sealed class ScaleExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: scale(x, y, z)");

            float x = operands[0].GetFloat();
            float y = operands[1].GetFloat();
            float z = operands[2].GetFloat();
            Vector3Obj vecObj = new Vector3Obj { Value = new Vector3(x, y, z) };
            return BoxedValue.FromObject(vecObj);
        }
    }

    /// <summary>
    /// getcamp(objid) - get entity camp ID
    /// </summary>
    internal sealed class GetCampExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: getcamp(objid)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
                return obj.GetCampId();
            return 0;
        }
    }

    /// <summary>
    /// getattr(objid, attrid) - get entity attribute value
    /// </summary>
    internal sealed class GetAttrExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: getattr(objid, attrid)");

            int objId = operands[0].GetInt();
            int attrId = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
            {
                try
                {
                    return obj.Property.GetLong((CharacterPropertyEnum)attrId);
                }
                catch (Exception ex)
                {
                    LogSystem.Error("getattr throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
            return 0L;
        }
    }

    /// <summary>
    /// calcoffset(obj_id, target_obj_id, offset) - calculate offset position relative to target
    /// </summary>
    internal sealed class CalcOffsetExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: calcoffset(obj_id, target_obj_id, offset)");

            int objId = operands[0].GetInt();
            int targetObjId = operands[1].GetInt();
            float offset = operands[2].GetFloat();

            EntityInfo srcObj = SceneSystem.Instance.GetEntityById(objId);
            EntityInfo targetObj = SceneSystem.Instance.GetEntityById(targetObjId);
            if (srcObj != null && targetObj != null)
            {
                Vector3 srcPos = srcObj.GetMovementStateInfo().GetPosition3D();
                Vector3 targetPos = targetObj.GetMovementStateInfo().GetPosition3D();
                float dir = Geometry.GetYRadian(srcPos, targetPos);
                float rad = Geometry.DegreeToRadian(offset);
                float x = targetPos.x + (float)Math.Cos(dir + rad) * offset;
                float z = targetPos.z + (float)Math.Sin(dir + rad) * offset;
                Vector3Obj vecObj = new Vector3Obj { Value = new Vector3(x, targetPos.y, z) };
                return BoxedValue.FromObject(vecObj);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// calcdir(obj_id, target_obj_id) - calculate facing direction from obj to target
    /// </summary>
    internal sealed class CalcDirExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: calcdir(obj_id, target_obj_id)");

            int objId = operands[0].GetInt();
            int targetObjId = operands[1].GetInt();

            EntityInfo srcObj = SceneSystem.Instance.GetEntityById(objId);
            EntityInfo targetObj = SceneSystem.Instance.GetEntityById(targetObjId);
            if (srcObj != null && targetObj != null)
            {
                Vector3 srcPos = srcObj.GetMovementStateInfo().GetPosition3D();
                Vector3 targetPos = targetObj.GetMovementStateInfo().GetPosition3D();
                return Geometry.GetYRadian(srcPos, targetPos);
            }
            return 0f;
        }
    }

    /// <summary>
    /// isenemy(camp1, camp2) - check if camp1 is enemy of camp2
    /// </summary>
    internal sealed class IsEnemyExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: isenemy(camp1, camp2)");

            int camp1 = operands[0].GetInt();
            int camp2 = operands[1].GetInt();
            return EntityInfo.GetRelation(camp1, camp2) == CharacterRelation.RELATION_ENEMY ? 1 : 0;
        }
    }

    /// <summary>
    /// isfriend(camp1, camp2) - check if camp1 is friend of camp2
    /// </summary>
    internal sealed class IsFriendExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: isfriend(camp1, camp2)");

            int camp1 = operands[0].GetInt();
            int camp2 = operands[1].GetInt();
            return EntityInfo.GetRelation(camp1, camp2) == CharacterRelation.RELATION_FRIEND ? 1 : 0;
        }
    }

    /// <summary>
    /// objgetnpctype(objid) - get entity type by object ID
    /// </summary>
    internal sealed class ObjGetNpcTypeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: objgetnpctype(objid)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
                return obj.EntityType;
            return 0;
        }
    }

    /// <summary>
    /// objgetaiparam(objid, index) - get entity AI parameter
    /// </summary>
    internal sealed class ObjGetAiParamExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: objgetaiparam(objid, index)");

            int objId = operands[0].GetInt();
            int index = operands[1].GetInt();

            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
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
