using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StoryScript;

internal class AiGetTarget : SimpleStoryFunctionBase<AiGetTarget, StoryFunctionParam<int>>
{
    protected override void UpdateValue(StoryInstance instance, StoryFunctionParam<int> _params, StoryFunctionResult result)
    {
        int objId = _params.Param1Value;
        EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
        if (null != npc) {
            int targetId = npc.GetAiStateInfo().Target;
            if (targetId > 0) {
                EntityInfo entity = SceneSystem.Instance.GetEntityById(targetId);
                if (null != entity && !entity.IsDead()) {
                    result.Value = BoxedValue.FromObject(entity);
                } else {
                    result.Value = BoxedValue.NullObject;
                }
            } else {
                result.Value = BoxedValue.NullObject;
            }
        }
    }
}
