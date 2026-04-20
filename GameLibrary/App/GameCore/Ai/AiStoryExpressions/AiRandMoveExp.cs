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
    /// ai_rand_move(objId, time, radius) - make NPC move randomly for specified time
    /// This is an async expression that yields until the time expires
    /// </summary>
    internal sealed class AiRandMoveExp : SimpleAsyncExpressionBase
    {
        protected override IEnumerator OnCalc(IList<BoxedValue> operands, AsyncCalcResult result)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: ai_rand_move(objId, time, radius)");

            int objId = operands[0].GetInt();
            long moveTime = operands[1].GetLong();
            int radius = operands[2].GetInt();

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                long startTime = TimeUtility.GetLocalMilliseconds();
                SelectTargetPos(npc, radius);

                while (!npc.IsDead())
                {
                    long currentTime = TimeUtility.GetLocalMilliseconds();
                    if (currentTime - startTime >= moveTime)
                        break;  // Time expired

                    AiData_General data = AiLogicUtility.GetAiData(npc);
                    if (null != data)
                    {
                        UnityEngine.Vector3 targetPos = npc.GetAiStateInfo().TargetPosition;
                        UnityEngine.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
                        float distSqr = Geometry.DistanceSquare(srcPos, targetPos);

                        if (distSqr <= 1)
                        {
                            // Reached target position, select new one
                            npc.GetMovementStateInfo().IsMoving = false;
                            AiCommand.AiStopPursue(npc);
                            SelectTargetPos(npc, radius);
                        }
                        else
                        {
                            npc.GetMovementStateInfo().IsMoving = true;
                            AiCommand.AiPursue(npc, targetPos);
                        }
                    }

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

        private void SelectTargetPos(EntityInfo npc, int radius)
        {
            UnityEngine.Vector3 pos = npc.GetMovementStateInfo().GetPosition3D();
            float dx = Helper.Random.Next(radius) - radius / 2;
            float dz = Helper.Random.Next(radius) - radius / 2;
            pos.x += dx;
            pos.z += dz;
            npc.GetAiStateInfo().TargetPosition = AiCommand.AiGetValidPosition(npc, pos, radius);
        }
    }
}
