using System;
using System.Collections;
using System.Collections.Generic;
using GameLibrary;

internal enum AiStateId : int
{
    Combat = PredefinedAiStateId.MaxValue + 1,
    Pursue,
    Gohome,
}
    
internal enum AiTargetType : int
{
    NPC = 0,
    Player,
    ALL,
}

internal class AiQueryComparer : IComparer
{
    public int Compare(object x, object y)
    {
        var ax = x as ArrayList;
        var ay = y as ArrayList;

        for (int i = 1; i <= m_Count; ++i) {
            int v = CompareObject(ax[i], ay[i]);
            if (v != 0) {
                return v;
            }
        }
        return 0;
    }
    public AiQueryComparer(bool desc, int count)
    {
        m_Desc = desc;
        m_Count = count;
    }
    private int CompareObject(object x, object y)
    {
        if (x is string && y is string) {
            var xs = x as string;
            var ys = y as string;
            if (m_Desc) {
                return -xs.CompareTo(ys);
            } else {
                return xs.CompareTo(ys);
            }
        } else {
            var fx = (float)System.Convert.ChangeType(x, typeof(float));
            var fy = (float)System.Convert.ChangeType(y, typeof(float));
            if (m_Desc) {
                return -fx.CompareTo(fy);
            } else {
                return fx.CompareTo(fy);
            }
        }
    }

    private bool m_Desc = false;
    private int m_Count = 0;
}

internal sealed class AiLogicUtility
{
    internal const long c_MaxComboInterval = 6000;
    internal const int c_MaxViewRange = 30;
    internal const int c_MaxViewRangeSqr = c_MaxViewRange * c_MaxViewRange;
    
    internal static EntityInfo GetNearstTargetHelper(EntityInfo srcObj, CharacterRelation relation)
    {
        return GetNearstTargetHelper(srcObj, relation, AiTargetType.ALL);
    }

    internal static EntityInfo GetNearstTargetHelper(EntityInfo srcObj, float range, CharacterRelation relation)
    {
        return GetNearstTargetHelper(srcObj, range, relation, AiTargetType.ALL);
    }

    internal static EntityInfo GetNearstTargetHelper(EntityInfo srcObj, CharacterRelation relation, AiTargetType type)
    {
        return GetNearstTargetHelper(srcObj, srcObj.ViewRange, relation, type);
    }

    internal static EntityInfo GetNearstTargetHelper(EntityInfo srcObj, float range, CharacterRelation relation, AiTargetType type)
    {
        EntityInfo nearstTarget = null;
        var lastTarget = SceneSystem.Instance.GetEntityById(srcObj.GetAiStateInfo().Target);
        if (null != lastTarget && !lastTarget.IsDead() && relation == EntityInfo.GetRelation(srcObj, lastTarget)) {
            float dSqr = Geometry.DistanceSquare(srcObj.GetMovementStateInfo().GetPosition3D(), lastTarget.GetMovementStateInfo().GetPosition3D());
            if (dSqr < range * range) {
                return lastTarget;
            }
        }
        float minDistSqr = 999999;
        SceneSystem.Instance.EntityKdTree.QueryWithAction(srcObj, range, (float distSqr, EntityKdTree.KdTreeData kdTreeObj) => {
            StepCalcNearstTarget(srcObj, relation, type, distSqr, kdTreeObj.Object, ref minDistSqr, ref nearstTarget);
        });
        return nearstTarget;
    }

    internal static EntityInfo GetLivingCharacterInfoHelper(EntityInfo srcObj, int id)
    {
        EntityInfo target = srcObj.EntityManager.GetEntityInfo(id);
        if (null != target) {
            if (target.IsDead())
                target = null;
        }
        return target;
    }

    internal static EntityInfo GetSeeingLivingCharacterInfoHelper(EntityInfo srcObj, int id)
    {
        EntityInfo target = srcObj.EntityManager.GetEntityInfo(id);
        if (null != target) {
            if (target.IsDead())
                target = null;
        }
        return target;
    }

    private static void StepCalcNearstTarget(EntityInfo srcObj, CharacterRelation relation, AiTargetType type, float distSqr, EntityInfo obj, ref float minDistSqr, ref EntityInfo nearstTarget)
    {
        EntityInfo target = GetSeeingLivingCharacterInfoHelper(srcObj, obj.GetId());
        if (null != target && !target.IsDead()) {
            if (type == AiTargetType.Player && target.IsPlayerType) {
                return;
            }
            if (type == AiTargetType.NPC && !target.IsNpcType) {
                return;
            }

            if (relation == EntityInfo.GetRelation(srcObj, target)) {
                if (srcObj.EntityType == (int)EntityTypeEnum.Player) {
                    if (distSqr < minDistSqr) {
                        nearstTarget = target;
                        minDistSqr = distSqr;
                    }
                }
            }
        }
    }
    
    internal static AiData_General GetAiData(EntityInfo npc)
    {
        AiData_General data = npc.GetAiStateInfo().AiDatas.GetData<AiData_General>();
        if (null == data) {
            data = new AiData_General();
            npc.GetAiStateInfo().AiDatas.AddData(data);
        }
        return data;
    }
}