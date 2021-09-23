using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StoryScript;

internal class AiSelectTarget : SimpleStoryValueBase<AiSelectTarget, StoryValueParam<int, float>>
{
    protected override void UpdateValue(StoryInstance instance, StoryValueParam<int, float> _params, StoryValueResult result)
    {
        int objId = _params.Param1Value;
        float dist = _params.Param2Value;
        EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
        if (null != npc) {
            EntityInfo entity;
            if (dist < Geometry.c_FloatPrecision) {
                entity = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY);
                if (null != entity) {
                    npc.GetAiStateInfo().Target = entity.GetId();
                }
            } else {
                entity = AiLogicUtility.GetNearstTargetHelper(npc, dist, CharacterRelation.RELATION_ENEMY);
                if (null != entity) {
                    npc.GetAiStateInfo().Target = entity.GetId();
                }
            }
            result.Value = BoxedValue.From(entity);
        }
    }
}
