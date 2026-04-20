using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameLibrary;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// ai_chase(objId, skillDist) - make NPC chase target until within skill distance
    /// This is an async expression that yields until the chase is complete
    /// </summary>
    internal sealed class AiChaseExp : SimpleAsyncExpressionBase
    {
        protected override IEnumerator OnCalc(IList<BoxedValue> operands, AsyncCalcResult result)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: ai_chase(objId, skillDist)");

            int objId = operands[0].GetInt();
            float skillDist = operands[1].GetFloat();

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                AiStateInfo info = npc.GetAiStateInfo();

                while (!npc.IsDead())
                {
                    EntityInfo target = SceneSystem.Instance.GetEntityById(info.Target);
                    if (null == target || target.IsDead())
                        break;

                    float distSqr = Geometry.DistanceSquare(
                        npc.GetMovementStateInfo().GetPosition3D(),
                        target.GetMovementStateInfo().GetPosition3D());

                    if (distSqr <= skillDist * skillDist)
                        break;  // Within range, chase complete

                    // Continue chasing
                    UnityEngine.Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
                    AiCommand.AiPursue(npc, targetPos);

                    yield return null;  // Wait one frame
                }

                // Stop pursuing when done
                if (!npc.IsDead())
                {
                    npc.GetMovementStateInfo().IsMoving = false;
                    AiCommand.AiStopPursue(npc);
                }
            }

            result.Value = BoxedValue.NullObject;
        }
    }
}
