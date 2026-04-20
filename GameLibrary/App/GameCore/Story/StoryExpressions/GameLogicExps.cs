using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;
using UnityEngine;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// blackboardclear() - clear the scene blackboard variables
    /// </summary>
    internal sealed class BlackboardClearExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            SceneSystem.Instance.BlackBoard.ClearVariables();
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// blackboardset(name, value) - set a variable on the scene blackboard
    /// </summary>
    internal sealed class BlackboardSetExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: blackboardset(name, value)");
            string name = operands[0].ToString();
            object value = operands[1].GetObject();
            SceneSystem.Instance.BlackBoard.SetVariable(name, value);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// bornfinish(objid) - finish the born state of an entity
    /// </summary>
    internal sealed class BornFinishExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: bornfinish(objid)");
            int objId = operands[0].GetInt();
            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                npc.IsBorning = false;
                npc.BornTime = 0;
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// deadfinish(objid) - finish the dead state of an entity
    /// </summary>
    internal sealed class DeadFinishExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: deadfinish(objid)");
            int objId = operands[0].GetInt();
            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                npc.DeadTime = 0;
                npc.NeedDelete = true;
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// setplayerid([objid,] leaderid) - set player ID or entity leader ID
    /// </summary>
    internal sealed class SetPlayerIdExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: setplayerid([objid,] leaderid)");

            if (operands.Count > 1)
            {
                // With objid parameter
                int objId = operands[0].GetInt();
                int leaderId = operands[1].GetInt();
                EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
                if (null != npc)
                {
                    npc.GetAiStateInfo().LeaderID = leaderId;
                }
            }
            else
            {
                // Without objid parameter - set player ID directly
                int leaderId = operands[0].GetInt();
                SceneSystem.Instance.PlayerId = leaderId;
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// setstoryskipped(0_or_1) - set story skipped state
    /// </summary>
    internal sealed class SetStorySkippedExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: setstoryskipped(0_or_1)");

            int state = operands[0].GetInt();
            StoryConfigManager.Instance.IsStorySkipped = state != 0;
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// setstoryspeedup(0_or_1) - set story speedup state
    /// </summary>
    internal sealed class SetStorySpeedupExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: setstoryspeedup(0_or_1)");

            int state = operands[0].GetInt();
            StoryConfigManager.Instance.IsStorySpeedup = state != 0;
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// blackboardget(name[, default_value]) - get a blackboard variable
    /// </summary>
    internal sealed class BlackboardGetExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: blackboardget(name[, default_value])");

            string name = operands[0].ToString();
            if (SceneSystem.Instance.BlackBoard.TryGetVariable(name, out object v))
                return BoxedValue.FromObject(v);
            if (operands.Count > 1)
                return operands[1];
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// getplayerid() - get player entity object ID
    /// </summary>
    internal sealed class GetPlayerIdExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            int playerId = SceneSystem.Instance.PlayerId;
            return playerId;
        }
    }

    /// <summary>
    /// unitid2objid(unitid) - convert unit ID to object ID
    /// </summary>
    internal sealed class UnitId2ObjIdExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: unitid2objid(unitid)");

            int unitId = operands[0].GetInt();
            int objId = 0;

            // Find entity by unit ID
            EntityInfo entity = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != entity)
            {
                objId = entity.GetId();
            }
            return objId;
        }
    }

    /// <summary>
    /// objid2unitid(objid) - convert object ID to unit ID
    /// </summary>
    internal sealed class ObjId2UnitIdExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: objid2unitid(objid)");

            int objId = operands[0].GetInt();
            EntityInfo obj = SceneSystem.Instance.GetEntityById(objId);
            if (obj != null)
                return obj.GetUnitId();
            return 0;
        }
    }

    /// <summary>
    /// findobjid(type, position, range) - find nearest entity by type within range
    /// </summary>
    internal sealed class FindObjIdExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: findobjid(type, position, range)");

            EntityTypeEnum type = (EntityTypeEnum)operands[0].GetInt();
            Vector3Obj posObj = operands[1];
            Vector3 pos = posObj;
            float range = operands[2].GetFloat();

            var entity = SceneSystem.Instance.FindEntityByRange(type, pos.x, pos.z, range);
            if (entity != null)
                return entity.GetId();
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// findobjids(type, position, range) - find all entity IDs by type within range
    /// </summary>
    internal sealed class FindObjIdsExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: findobjids(type, position, range)");

            EntityTypeEnum type = (EntityTypeEnum)operands[0].GetInt();
            Vector3Obj posObj = operands[1];
            Vector3 pos = posObj;
            float range = operands[2].GetFloat();

            List<EntityInfo> list = new List<EntityInfo>();
            if (SceneSystem.Instance.FindEntitiesByRange(type, pos.x, pos.z, range, list))
            {
                List<int> ids = new List<int>();
                foreach (var info in list)
                    ids.Add(info.GetId());
                return BoxedValue.FromObject(ids);
            }
            return BoxedValue.FromObject(new List<int>());
        }
    }

    /// <summary>
    /// countdyingnpc(campId[, relation]) - count dying NPCs by camp or relation
    /// </summary>
    internal sealed class CountDyingNpcExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: countdyingnpc(campId[, relation])");

            int campId = operands[0].GetInt();
            if (operands.Count > 1)
            {
                CharacterRelation relation = (CharacterRelation)operands[1].GetInt();
                return SceneSystem.Instance.GetDyingNpcCountByRelationWithCamp(campId, relation);
            }
            return SceneSystem.Instance.GetDyingNpcCountByCamp(campId);
        }
    }
}
