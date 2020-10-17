using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StorySystem;

internal class AiGetTarget : SimpleStoryValueBase<AiGetTarget, StoryValueParam<int>>
{
    protected override void UpdateValue(StoryInstance instance, StoryValueParam<int> _params, StoryValueResult result)
    {
        int objId = _params.Param1Value;
        EntityInfo npc = SceneSystem.Instance.GetEntityById(objId);
        if (null != npc) {
            int targetId = npc.GetAiStateInfo().Target;
            if (targetId > 0) {
                EntityInfo entity = SceneSystem.Instance.GetEntityById(targetId);
                if (null != entity && !entity.IsDead()) {
                    result.Value = new BoxedValue(entity);
                } else {
                    result.Value = BoxedValue.NullObject;
                }
            } else {
                result.Value = BoxedValue.NullObject;
            }
        }
    }
}
