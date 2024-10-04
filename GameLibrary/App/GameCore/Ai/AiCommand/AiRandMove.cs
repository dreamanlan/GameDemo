using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary;
using StoryScript;

internal class AiRandMove : SimpleStoryCommandBase<AiRandMove, StoryFunctionParam<int, int, int>>
{
    protected override void ResetState()
    {
        m_ParamReaded = false;
    }

    protected override bool ExecCommand(StoryInstance instance, StoryFunctionParam<int, int, int> _params, long delta)
    {
        if (!m_ParamReaded) {
            m_ParamReaded = true;

            m_ObjId = _params.Param1Value;
            m_Time = _params.Param2Value;
            m_Radius = _params.Param3Value;

            EntityInfo npc = SceneSystem.Instance.GetEntityById(m_ObjId);
            if (null != npc) {
                SelectTargetPos(npc);
                return true;
            }
        } else {
            EntityInfo npc = SceneSystem.Instance.GetEntityById(m_ObjId);
            if (null != npc) {
                AiStateInfo info = npc.GetAiStateInfo();
                if (npc.IsDead() && npc.GetMovementStateInfo().IsMoving) {
                    npc.GetMovementStateInfo().IsMoving = false;
                    AiCommand.AiStopPursue(npc);
                    info.ChangeToState((int)PredefinedAiStateId.Idle);
                    return false;
                }
                return RandMoveHandler(npc, info, delta);
            }
        }
        return false;
    }

    private bool RandMoveHandler(EntityInfo npc, AiStateInfo info, long deltaTime)
    {
        info.Time += deltaTime;
        if (info.Time > m_Time) {
            info.Time = 0;
            npc.GetMovementStateInfo().IsMoving = false;
            AiCommand.AiStopPursue(npc);
            info.ChangeToState((int)PredefinedAiStateId.Idle);
            EntityInfo target = SceneSystem.Instance.GetEntityById(info.Target);
            if (null != target) {
                float dir = Geometry.GetYRadian(npc.GetMovementStateInfo().GetPosition3D(), target.GetMovementStateInfo().GetPosition3D());
                npc.GetMovementStateInfo().SetWantedFaceDir(dir);
            }
            return false;
        }

        AiData_General data = AiLogicUtility.GetAiData(npc);
        if (null != data) {
            UnityEngine.Vector3 targetPos = info.TargetPosition;
            UnityEngine.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
            float distSqr = Geometry.DistanceSquare(srcPos, targetPos);
            if (distSqr <= 1) {
                if (npc.GetMovementStateInfo().IsMoving) {
                    npc.GetMovementStateInfo().IsMoving = false;
                    AiCommand.AiStopPursue(npc);
                    info.ChangeToState((int)PredefinedAiStateId.Idle);
                }
            } else {
                npc.GetMovementStateInfo().IsMoving = true;
                AiCommand.AiPursue(npc, targetPos);
            }
        }
        return true;
    }

    private void SelectTargetPos(EntityInfo npc)
    {
        UnityEngine.Vector3 pos = npc.GetMovementStateInfo().GetPosition3D();
        float dx = Helper.Random.Next(m_Radius) - m_Radius / 2;
        float dz = Helper.Random.Next(m_Radius) - m_Radius / 2;
        pos.x += dx;
        pos.z += dz;
        npc.GetAiStateInfo().TargetPosition = AiCommand.AiGetValidPosition(npc, pos, m_Radius);
    }

    private int m_ObjId = 0;
    private long m_Time = 0;
    private int m_Radius = 0;
    private bool m_ParamReaded = false;
}
