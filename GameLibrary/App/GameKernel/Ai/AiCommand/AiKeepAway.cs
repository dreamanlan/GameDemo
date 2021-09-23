using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StoryScript;

internal class AiKeepAway : SimpleStoryCommandBase<AiKeepAway, StoryValueParam<int, float, float>>
{
    protected override void ResetState()
    {
        m_KeepAwayStarted = false;
    }

    protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int, float, float> _params, long delta)
    {
        if (!m_KeepAwayStarted) {
            m_KeepAwayStarted = true;

            m_ObjId = _params.Param1Value;
            m_SkillDistance = _params.Param2Value;
            m_Ratio = _params.Param3Value;
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
                UnityEngine.Vector3 dir = srcPos - targetPos;
                dir.Normalize();
                targetPos = targetPos + dir * m_SkillDistance;
                if (distSqr < m_Ratio * m_Ratio * m_SkillDistance * m_SkillDistance) {
                    AiCommand.AiPursue(npc, targetPos);
                    return true;
                }
            }
        }
        return false;
    }

    private int m_ObjId = 0;
    private float m_SkillDistance = 0;
    private float m_Ratio = 1.0f;
    private bool m_KeepAwayStarted = false;
}
