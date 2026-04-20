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
    /// ai_go_home(objId) - make NPC go back to home position
    /// This is an async expression that yields until the NPC reaches home
    /// </summary>
    internal sealed class AiGohomeExp : SimpleAsyncExpressionBase
    {
        protected override IEnumerator OnCalc(IList<BoxedValue> operands, AsyncCalcResult result)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: ai_go_home(objId)");

            int objId = operands[0].GetInt();

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                AiStateInfo info = npc.GetAiStateInfo();
                UnityEngine.Vector3 homePos = info.HomePos;

                while (!npc.IsDead())
                {
                    UnityEngine.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
                    float distSqr = Geometry.DistanceSquare(srcPos, homePos);

                    if (distSqr <= 1)
                        break;  // Reached home

                    // Move towards home
                    npc.GetMovementStateInfo().IsMoving = true;
                    info.TargetPosition = homePos;
                    AiCommand.AiPursue(npc, homePos);

                    yield return null;  // Wait one frame
                }

                // Stop when done
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
