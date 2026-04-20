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
    /// ai_keep_away(objId, skillDist, ratio) - make NPC keep away from target
    /// This is an async expression that yields until the action is complete
    /// </summary>
    internal sealed class AiKeepAwayExp : SimpleAsyncExpressionBase
    {
        protected override IEnumerator OnCalc(IList<BoxedValue> operands, AsyncCalcResult result)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: ai_keep_away(objId, skillDist, ratio)");

            int objId = operands[0].GetInt();
            float skillDist = operands[1].GetFloat();
            float ratio = operands[2].GetFloat();

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                AiStateInfo info = npc.GetAiStateInfo();

                while (!npc.IsDead())
                {
                    EntityInfo target = SceneSystem.Instance.GetEntityById(info.Target);
                    if (null == target || target.IsDead())
                        break;

                    UnityEngine.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
                    UnityEngine.Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
                    float distSqr = Geometry.DistanceSquare(srcPos, targetPos);

                    if (distSqr >= ratio * ratio * skillDist * skillDist)
                        break;  // Far enough away, keep_away complete

                    // Move away from target
                    UnityEngine.Vector3 dir = (srcPos - targetPos).normalized;
                    UnityEngine.Vector3 awayPos = targetPos + dir * skillDist;
                    AiCommand.AiPursue(npc, awayPos);

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
