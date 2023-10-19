using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary
{
    internal sealed class EntityManager
    {
        internal EntityManager(int poolSize)
        {
            m_EntityPoolSize = poolSize;
            for (int i = 0; i < m_TypedEntities.Length; ++i) {
                m_TypedEntities[i] = new EntityInfoDictionary();
            }
        }

        internal long MaxTimeInPool
        {
            get { return m_MaxTimeInPool; }
            set { m_MaxTimeInPool = value; }
        }

        internal EntityInfoDictionary Entities
        {
            get { return m_Entities; }
        }

        internal EntityInfoDictionary GetEntitiesByType(EntityTypeEnum type)
        {
            EntityInfoDictionary dict = null;
            if (type >= 0 && type < EntityTypeEnum.MaxTypeNum) {
                dict = m_TypedEntities[(int)type];
            }
            return dict;
        }

        internal EntityInfo GetEntityInfo(int id)
        {
            EntityInfo npc;
            m_Entities.TryGetValue(id, out npc);
            return npc;
        }

        internal EntityInfo GetEntityInfoByUnitId(int unitId)
        {
            EntityInfo npc;
            m_MarkedEntities.TryGetValue(unitId, out npc);
            return npc;
        }

        internal bool CheckEntityRange(EntityInfo info, float x, float y, float range)
        {
            var pos = new UnityEngine.Vector2(info.GetMovementStateInfo().PositionX, info.GetMovementStateInfo().PositionZ);
            float distSqr = Geometry.DistanceSquare(pos.x, pos.y, x, y);
            return distSqr < range * range;
        }
        
        internal EntityInfo FindEntityByRange(EntityTypeEnum type, float x, float y, float range)
        {
            EntityInfo npc = null;
            if (type >= 0 && type < EntityTypeEnum.MaxTypeNum) {
                var entities = m_TypedEntities[(int)type];
                for (LinkedListNode<EntityInfo> linkNode = entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                    EntityInfo info = linkNode.Value;
                    if (CheckEntityRange(info, x, y, range)) {
                        npc = info;
                        break;
                    }
                }
            }
            return npc;
        }

        internal bool FindEntitiesByRange(EntityTypeEnum type, float x, float y, float range, List<EntityInfo> list)
        {
            bool ret = false;
            if (type >= 0 && type < EntityTypeEnum.MaxTypeNum)
            {
                var entities = m_TypedEntities[(int)type];
                for (LinkedListNode<EntityInfo> linkNode = entities.FirstNode; null != linkNode; linkNode = linkNode.Next)
                {
                    EntityInfo info = linkNode.Value;
                    if (CheckEntityRange(info, x, y, range))
                    {
                        list.Add(info);
                        ret = true;
                    }
                }
            }
            return ret;
        }

        internal EntityInfo AddEntity(int unitId, string model, EntityTypeEnum entityType, string ai, params string[] aiParams)
        {
            EntityInfo npc = NewEntityInfo();
            npc.EntityManager = this;
            npc.LoadData(unitId, model, entityType, ai, aiParams);
            // born
            npc.IsBorning = true;
            npc.BornTime = 0;
            npc.SetAIEnable(false);

            AddEntity(npc);
            return npc;
        }

        internal EntityInfo AddEntityWithId(int id, ulong guid, int unitId, string model, EntityTypeEnum entityType, string ai, params string[] aiParams)
        {
            if (m_Entities.Contains(id)) {
                LogSystem.Warn("duplicate entity {0} !!!", id);
                return null;
            }
            EntityInfo npc = NewEntityInfo(id, guid);
            npc.EntityManager = this;
            npc.LoadData(unitId, model, entityType, ai, aiParams);
            npc.IsBorning = true;
            npc.BornTime = 0;
            npc.SetAIEnable(false);

            AddEntity(npc);
            return npc;
        }

        internal void RemoveEntity(int id)
        {
            EntityInfo npc = GetEntityInfo(id);
            if (null != npc) {
                RecycleEntity(npc);
            }
        }

        internal void UpdateId(EntityInfo npc, int oldId)
        {
            if (oldId >= 0) {
                m_Entities.Remove(oldId);
            }
            int id = npc.GetId();
            if (id >= 0) {
                m_Entities.AddLast(id, npc);
            }
        }
        internal void UpdateUnitId(EntityInfo npc, int oldUnitId)
        {
            if (oldUnitId > 0) {
                m_MarkedEntities.Remove(oldUnitId);
            }
            int unitId = npc.GetUnitId();
            if (unitId > 0) {
                EntityInfo old;
                if (!m_MarkedEntities.TryGetValue(unitId, out old)) {
                    m_MarkedEntities.AddLast(unitId, npc);
                } else {
                    LogSystem.Error("entity unitid {0} duplicated !!!", unitId);
                }
            }
        }

        internal void Reset()
        {
            m_Entities.Clear();
            m_NextInfoId = c_StartId;
            m_UnusedIds.Clear();
            m_UnusedClientIds.Clear();

            for (int i = 0; i < (int)EntityTypeEnum.MaxTypeNum; ++i) {
                m_TypedEntities[i].Clear();
            }
            m_MarkedEntities.Clear();
        }

        private void AddEntity(EntityInfo npc)
        {
            int id = npc.GetId();
            if (!m_Entities.Contains(id)) {
                m_Entities.AddLast(id, npc);
            }
            int type = npc.EntityType;
            if (type >= 0 && type < (int)EntityTypeEnum.MaxTypeNum) {
                var entities = m_TypedEntities[type];
                if (!entities.Contains(id)) {
                    entities.AddLast(id, npc);
                }
            }
        }
        private void RecycleEntity(EntityInfo npc)
        {
            int id = npc.GetId();
            int type = npc.EntityType;
            if (type >= 0 && type < (int)EntityTypeEnum.MaxTypeNum) {
                var entities = m_TypedEntities[type];
                entities.Remove(id);
            }
            m_MarkedEntities.Remove(npc.GetUnitId());

            m_Entities.Remove(id);
            RecycleEntityInfo(npc);
        }

        private EntityInfo NewEntityInfo()
        {
            EntityInfo npc = null;
            int id = GenNextId();
            npc = new EntityInfo(id);
            return npc;
        }

        private EntityInfo NewEntityInfo(int id, ulong guid)
        {
            EntityInfo npc = null;
            if (m_UnusedEntities.Count > 0) {
                npc = m_UnusedEntities.Dequeue();

                npc.Reset();
                npc.InitId(id);
            } else {
                npc = new EntityInfo(id);
            }
            return npc;
        }

        private void RecycleEntityInfo(EntityInfo npcInfo)
        {
            if (null != npcInfo) {
                int id = npcInfo.GetId();
                if (id >= c_StartId && id < c_StartId + c_MaxIdNum) {
                    m_UnusedIds.Push(id);
                }
                if (id >= c_StartId_Client && id < c_StartId_Client + c_MaxIdNum) {
                    m_UnusedClientIds.Push(id);
                }
                npcInfo.Reset();
                m_UnusedEntities.Enqueue(npcInfo);
            }
        }

        private int GenNextId()
        {
            int id = 0;
            int startId = 0;
            startId = c_StartId_Client;
            while (m_UnusedClientIds.Count > 100) {
                int t = m_UnusedClientIds.Pop();
                if (!m_Entities.Contains(t)) {
                    id = t;
                    break;
                }
            }
            /*
            bool isClient = true;
            if (isClient) {
                startId = c_StartId_Client;
                while (m_UnusedClientIds.Count > 100) {
                    int t = m_UnusedClientIds.Pop();
                    if (!m_Entities.Contains(t)) {
                        id = t;
                        break;
                    }
                }
            } else {
                startId = c_StartId;
                while (m_UnusedIds.Count > 100) {
                    int t = m_UnusedIds.Pop();
                    if (!m_Entities.Contains(t)) {
                        id = t;
                        break;
                    }
                }
            }
            */
            if (id <= 0) {
                for (int i = 0; i < c_MaxIdNum; ++i) {
                    int t = (m_NextInfoId + i - startId) % c_MaxIdNum + startId;
                    if (!m_Entities.Contains(t)) {
                        id = t;
                        break;
                    }
                }
                if (id > 0) {
                    m_NextInfoId = (id + 1 - startId) % c_MaxIdNum + startId;
                }
            }
            return id;
        }

        private EntityInfoDictionary m_Entities = new EntityInfoDictionary();
        private Queue<EntityInfo> m_UnusedEntities = new Queue<EntityInfo>();
        private int m_EntityPoolSize = 512;
        private long m_MaxTimeInPool = 60000;

        private Heap<int> m_UnusedIds = new Heap<int>(new DefaultReverseComparer<int>());
        private Heap<int> m_UnusedClientIds = new Heap<int>(new DefaultReverseComparer<int>());

        private const int c_StartId = 100;
        private const int c_MaxIdNum = 1000;
        private const int c_StartId_Client = 2000;
        private int m_NextInfoId = c_StartId;

        //额外列表，提高查找性能
        private EntityInfoDictionary[] m_TypedEntities = new EntityInfoDictionary[(int)EntityTypeEnum.MaxTypeNum];
        private EntityInfoDictionary m_MarkedEntities = new EntityInfoDictionary();
    }
}
