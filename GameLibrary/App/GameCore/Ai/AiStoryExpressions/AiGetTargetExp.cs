using UnityEngine;
using System;
using System.Collections.Generic;
using GameLibrary;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// ai_get_target(objId) - get the current target entity of an NPC
    /// Returns the target entity or null if no target
    /// </summary>
    internal sealed class AiGetTargetExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: ai_get_target(objId)");

            int objId = operands[0].GetInt();

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                int targetId = npc.GetAiStateInfo().Target;
                if (targetId > 0)
                {
                    EntityInfo entity = SceneSystem.Instance.GetEntityById(targetId);
                    if (null != entity && !entity.IsDead())
                    {
                        return BoxedValue.FromObject(entity);
                    }
                }
            }
            return BoxedValue.NullObject;
        }
    }
}
