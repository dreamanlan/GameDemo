using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StoryScript;

internal class AiGohome : SimpleStoryCommandBase<AiGohome, StoryValueParam<int>>
{
    protected override void ResetState()
    {
        m_ParamReaded = false;
    }

    protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int> _params, long delta)
    {
        if (!m_ParamReaded) {
            m_ParamReaded = true;
            m_ObjId = _params.Param1Value;
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
            return GohomeHandler(npc, info, delta);
        }
        return false;
    }

    private bool GohomeHandler(EntityInfo npc, AiStateInfo info, long deltaTime)
    {
        info.Time += deltaTime;
        if (info.Time > 100) {
            info.Time = 0;
        } else {
            return true;
        }
        
        AiData_General data = AiLogicUtility.GetAiData(npc);
        if (null != data) {
            UnityEngine.Vector3 targetPos = info.HomePos;
            UnityEngine.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
            float distSqr = Geometry.DistanceSquare(srcPos, info.HomePos);
            if (distSqr <= 1) {
                npc.GetMovementStateInfo().IsMoving = false;
                AiCommand.AiStopPursue(npc);
                info.ChangeToState((int)PredefinedAiStateId.Idle);
                return false;
            } else {
                npc.GetMovementStateInfo().IsMoving = true;
                info.TargetPosition = targetPos;
                AiCommand.AiPursue(npc, targetPos);
            }
        }
        return true;
    }

    private int m_ObjId = 0;
    private bool m_ParamReaded = false;
}
