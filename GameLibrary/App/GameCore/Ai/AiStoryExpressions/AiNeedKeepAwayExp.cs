using UnityEngine;
using System;
using System.Collections.Generic;
using GameLibrary;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// ai_need_keep_away(objId, skillDist, ratio) - check if NPC needs to keep away from target
    /// Returns 1 if need to keep away, 0 otherwise
    /// </summary>
    internal sealed class AiNeedKeepAwayExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: ai_need_keep_away(objId, skillDist, ratio)");

            int objId = operands[0].GetInt();
            float skillDist = operands[1].GetFloat();
            float ratio = operands[2].GetFloat();

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                int targetId = npc.GetAiStateInfo().Target;
                if (targetId > 0)
                {
                    EntityInfo target = SceneSystem.Instance.GetEntityById(targetId);
                    if (null != target)
                    {
                        float distSqr = Geometry.DistanceSquare(
                            npc.GetMovementStateInfo().GetPosition3D(),
                            target.GetMovementStateInfo().GetPosition3D());
                        if (distSqr < ratio * ratio * skillDist * skillDist)
                        {
                            return BoxedValue.FromObject(1);
                        }
                    }
                }
            }
            return BoxedValue.FromObject(0);
        }
    }
}
