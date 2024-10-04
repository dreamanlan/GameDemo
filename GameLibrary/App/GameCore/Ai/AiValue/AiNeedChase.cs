using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StoryScript;

internal class AiNeedChase : SimpleStoryFunctionBase<AiNeedChase, StoryFunctionParam<int, float>>
{
    protected override void UpdateValue(StoryInstance instance, StoryFunctionParam<int, float> _params, StoryFunctionResult result)
    {
        int objId = _params.Param1Value;
        float skillDist = _params.Param2Value;
        EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
        if (null != npc) {
            int targetId = npc.GetAiStateInfo().Target;
            if (targetId > 0) {
                EntityInfo target = SceneSystem.Instance.GetEntityById(targetId);
                if (null != target) {
                    float distSqr = Geometry.DistanceSquare(npc.GetMovementStateInfo().GetPosition3D(), target.GetMovementStateInfo().GetPosition3D());
                    if (distSqr > skillDist * skillDist) {
                        result.Value = 1;
                        return;
                    }
                }
            }
        }
        result.Value = 0;
    }
}
