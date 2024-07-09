using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary;
using GameLibrary.Story;

internal static class AiCommand
{
    public static void NotifyAiDeath(EntityInfo npc)
    {
        EntityViewModel view = SceneSystem.Instance.EntityViewManager.GetEntityViewById(npc.GetId());
        if (null != view)
            view.Death();
    }
    public static void AiFace(EntityInfo npc)
    {
        if (null != npc) {
            float dir = npc.GetMovementStateInfo().GetFaceDir();
            GameObject actor = SceneSystem.Instance.EntityViewManager.GetGameObject(npc.GetId());
            if (null != actor) {
                actor.transform.localRotation = Quaternion.Euler(0, Geometry.RadianToDegree(dir), 0);
            }
        }
    }
    public static UnityEngine.Vector3 AiGetValidPosition(EntityInfo npc, UnityEngine.Vector3 target, float maxDistance)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, maxDistance, NavMesh.AllAreas)) {
            return hit.position;
        } else {
            return npc.GetAiStateInfo().HomePos;
        }
    }
    public static void AiPursue(EntityInfo npc, UnityEngine.Vector3 target)
    {
        EntityViewModel npcView = SceneSystem.Instance.EntityViewManager.GetEntityViewById(npc.GetId());
        if (null != npcView)
            npcView.MoveTo(target.x, target.y, target.z);
    }
    public static void AiStopPursue(EntityInfo npc)
    {
        EntityViewModel npcView = SceneSystem.Instance.EntityViewManager.GetEntityViewById(npc.GetId());
        if (null != npcView)
            npcView.StopMove();
    }
    public static void AiSendStoryMessage(EntityInfo npc, string msgId, StoryScript.BoxedValueList args)
    {
        ClientStorySystem.Instance.SendMessage(msgId, args);
    }
}
