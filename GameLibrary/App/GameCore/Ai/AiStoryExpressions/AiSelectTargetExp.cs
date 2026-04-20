using UnityEngine;
using System;
using System.Collections.Generic;
using GameLibrary;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// ai_select_target(objId, dist) - select nearest enemy target within distance
    /// Sets the target on the NPC's AiStateInfo and returns the target entity
    /// </summary>
    internal sealed class AiSelectTargetExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: ai_select_target(objId, dist)");

            int objId = operands[0].GetInt();
            float dist = operands[1].GetFloat();

            EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
            if (null != npc)
            {
                EntityInfo entity;
                if (dist < Geometry.c_FloatPrecision)
                {
                    entity = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY);
                    if (null != entity)
                    {
                        npc.GetAiStateInfo().Target = entity.GetId();
                    }
                }
                else
                {
                    entity = AiLogicUtility.GetNearstTargetHelper(npc, dist, CharacterRelation.RELATION_ENEMY);
                    if (null != entity)
                    {
                        npc.GetAiStateInfo().Target = entity.GetId();
                    }
                }
                return BoxedValue.FromObject(entity);
            }
            return BoxedValue.NullObject;
        }
    }
}
