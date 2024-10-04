using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StoryScript;

internal class AiChase : SimpleStoryCommandBase<AiChase, StoryFunctionParam<int, float>>
{
    protected override void ResetState()
    {
        m_ChaseStarted = false;
    }

    protected override bool ExecCommand(StoryInstance instance, StoryFunctionParam<int, float> _params, long delta)
    {
        if (!m_ChaseStarted) {
            m_ChaseStarted = true;

            m_ObjId = _params.Param1Value;
            m_SkillDistance = _params.Param2Value;
        }
        EntityInfo npc = SceneSystem.Instance.GetEntityById(m_ObjId);
        if (null != npc) {
            AiStateInfo info = npc.GetAiStateInfo();
            if (npc.IsDead() && npc.GetMovementStateInfo().IsMoving) {
                npc.GetMovementStateInfo().IsMoving = false;
                AiCommand.AiStopPursue(npc);
                info.ChangeToState((int)PredefinedAiStateId.Idle);
                return false;
            }
            EntityInfo target = SceneSystem.Instance.GetEntityById(info.Target);
            if (null != target) {
                info.Time += delta;
                if (info.Time > 100) {
                    info.Time = 0;
                } else {
                    return true;
                }
                UnityEngine.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
                UnityEngine.Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
                float distSqr = Geometry.DistanceSquare(srcPos, targetPos);
                if (distSqr > m_SkillDistance * m_SkillDistance) {
                    AiCommand.AiPursue(npc, targetPos);
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    private int m_ObjId = 0;
    private float m_SkillDistance = 0;
    private bool m_ChaseStarted = false;
}
