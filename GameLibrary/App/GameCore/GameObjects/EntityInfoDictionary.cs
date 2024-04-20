using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary
{
    public class EntityInfoList : List<EntityInfo>
    {
        public EntityInfoList() { }
        public EntityInfoList(IEnumerable<EntityInfo> coll) : base(coll) { }
    }
    public class IntEntityDict : Dictionary<int, EntityInfo>
    {
        public IntEntityDict() { }
        public IntEntityDict(IDictionary<int, EntityInfo> dict) : base(dict) { }
    }
    public class StrEntityDict : Dictionary<string, EntityInfo>
    {
        public StrEntityDict() { }
        public StrEntityDict(IDictionary<string, EntityInfo> dict) : base(dict) { }
    }
    public class IntEntityViewDict : Dictionary<int, EntityViewModel>
    {
        public IntEntityViewDict() { }
        public IntEntityViewDict(IDictionary<int, EntityViewModel> dict) : base(dict) { }
    }
    public class StrEntityViewDict : Dictionary<string, EntityViewModel>
    {
        public StrEntityViewDict() { }
        public StrEntityViewDict(IDictionary<string, EntityViewModel> dict) : base(dict) { }
    }
    public sealed class EntityInfoDictionary
    {
        public bool Contains(int id)
        {
            return m_LinkNodeDictionary.ContainsKey(id);
        }
        ///Duplication is not considered here, it is guaranteed when called from the outside
        ///(performance considerations)
        public void AddFirst(int id, EntityInfo obj)
        {
            LinkedListNode<EntityInfo> linkNode = m_Objects.AddFirst(obj);
            if (null != linkNode) {
                try {
                    m_LinkNodeDictionary.Add(id, linkNode);
                } catch (Exception ex) {
                    m_Objects.RemoveFirst();
                    LogSystem.Error("EntityInfoDictionary.AddFirst throw Exception:{0}\n{1}\n Add id:{2}", ex.Message, ex.StackTrace, id);
                }
            }
        }
        public void AddLast(int id, EntityInfo obj)
        {
            LinkedListNode<EntityInfo> linkNode = m_Objects.AddLast(obj);
            if (null != linkNode) {
                try {
                    m_LinkNodeDictionary.Add(id, linkNode);
                } catch (Exception ex) {
                    m_Objects.RemoveLast();
                    LogSystem.Error("EntityInfoDictionary.AddLast throw Exception:{0}\n{1}\n Add id:{2}", ex.Message, ex.StackTrace, id);
                }
            }
        }
        public void Remove(int id)
        {
            LinkedListNode<EntityInfo> linkNode;
            if (m_LinkNodeDictionary.TryGetValue(id, out linkNode)) {
                m_LinkNodeDictionary.Remove(id);
                try {
                    m_Objects.Remove(linkNode);
                } catch (Exception ex) {
                    LogSystem.Error("EntityInfoDictionary.Remove throw Exception:{0}\n{1}\n Remove id:{2}", ex.Message, ex.StackTrace, id);
                }
            }
        }
        public void Clear()
        {
            m_LinkNodeDictionary.Clear();
            m_Objects.Clear();
        }
        public bool TryGetValue(int id, out EntityInfo value)
        {
            LinkedListNode<EntityInfo> linkNode;
            bool ret = m_LinkNodeDictionary.TryGetValue(id, out linkNode);
            if (ret) {
                value = linkNode.Value;
            } else {
                value = default(EntityInfo);
            }
            return ret;
        }
        public void VisitValues(MyAction<EntityInfo> visitor)
        {
            for (LinkedListNode<EntityInfo> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                visitor(linkNode.Value);
            }
        }
        public void VisitValues(MyFunc<EntityInfo, bool> visitor)
        {
            for (LinkedListNode<EntityInfo> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                if (!visitor(linkNode.Value))
                    break;
            }
        }
        public EntityInfo FindValue(MyFunc<EntityInfo, bool> visitor)
        {
            for (LinkedListNode<EntityInfo> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                if (visitor(linkNode.Value)) {
                    return linkNode.Value;
                }
            }
            return default(EntityInfo);
        }
        public int Count
        {
            get
            {
                return m_LinkNodeDictionary.Count;
            }
        }
        public EntityInfo this[int id]
        {
            get
            {
                LinkedListNode<EntityInfo> ret = null;
                if (m_LinkNodeDictionary.TryGetValue(id, out ret) == true) {
                    return ret.Value;
                } else {
                    return default(EntityInfo);
                }
            }
            set
            {
                LinkedListNode<EntityInfo> linkNode;
                if (m_LinkNodeDictionary.TryGetValue(id, out linkNode)) {
                    linkNode.Value = value;
                } else {
                    AddLast(id, value);
                }
            }
        }
        public LinkedListNode<EntityInfo> FirstNode
        {
            get
            {
                return m_Objects.First;
            }
        }
        public LinkedListNode<EntityInfo> LastNode
        {
            get
            {
                return m_Objects.Last;
            }
        }
        public void CopyValuesTo(EntityInfoList list)
        {
            list.AddRange(m_Objects);
        }
        public IEnumerable<EntityInfo> Values
        {
            get
            {
                return m_Objects;
            }
        }

        private Dictionary<int, LinkedListNode<EntityInfo>> m_LinkNodeDictionary = new Dictionary<int, LinkedListNode<EntityInfo>>();
        private LinkedList<EntityInfo> m_Objects = new LinkedList<EntityInfo>();
    }
    public sealed class EntityInfoByGuidDictionary
    {
        public bool Contains(ulong guid)
        {
            return m_LinkNodeDictionary.ContainsKey(guid);
        }
        ///Duplication is not considered here, it is guaranteed when called from the outside
        ///(performance considerations)
        public void AddFirst(ulong guid, EntityInfo obj)
        {
            LinkedListNode<EntityInfo> linkNode = m_Objects.AddFirst(obj);
            if (null != linkNode) {
                try {
                    m_LinkNodeDictionary.Add(guid, linkNode);
                } catch (Exception ex) {
                    m_Objects.RemoveFirst();
                    LogSystem.Error("EntityInfoByGuidDictionary.AddFirst throw Exception:{0}\n{1}\n Add guid:{2}", ex.Message, ex.StackTrace, guid);
                }
            }
        }
        public void AddLast(ulong guid, EntityInfo obj)
        {
            LinkedListNode<EntityInfo> linkNode = m_Objects.AddLast(obj);
            if (null != linkNode) {
                try {
                    m_LinkNodeDictionary.Add(guid, linkNode);
                } catch (Exception ex) {
                    m_Objects.RemoveLast();
                    LogSystem.Error("EntityInfoByGuidDictionary.AddLast throw Exception:{0}\n{1}\n Add guid:{2}", ex.Message, ex.StackTrace, guid);
                }
            }
        }
        public void Remove(ulong guid)
        {
            LinkedListNode<EntityInfo> linkNode;
            if (m_LinkNodeDictionary.TryGetValue(guid, out linkNode)) {
                m_LinkNodeDictionary.Remove(guid);
                try {
                    m_Objects.Remove(linkNode);
                } catch (Exception ex) {
                    LogSystem.Error("EntityInfoByGuidDictionary.Remove throw Exception:{0}\n{1}\n Remove guid:{2}", ex.Message, ex.StackTrace, guid);
                }
            }
        }
        public void Clear()
        {
            m_LinkNodeDictionary.Clear();
            m_Objects.Clear();
        }
        public bool TryGetValue(ulong guid, out EntityInfo value)
        {
            LinkedListNode<EntityInfo> linkNode;
            bool ret = m_LinkNodeDictionary.TryGetValue(guid, out linkNode);
            if (ret) {
                value = linkNode.Value;
            } else {
                value = default(EntityInfo);
            }
            return ret;
        }
        public void VisitValues(MyAction<EntityInfo> visitor)
        {
            for (LinkedListNode<EntityInfo> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                visitor(linkNode.Value);
            }
        }
        public void VisitValues(MyFunc<EntityInfo, bool> visitor)
        {
            for (LinkedListNode<EntityInfo> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                if (!visitor(linkNode.Value))
                    break;
            }
        }
        public EntityInfo FindValue(MyFunc<EntityInfo, bool> visitor)
        {
            for (LinkedListNode<EntityInfo> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                if (visitor(linkNode.Value)) {
                    return linkNode.Value;
                }
            }
            return default(EntityInfo);
        }
        public int Count
        {
            get
            {
                return m_LinkNodeDictionary.Count;
            }
        }
        public EntityInfo this[ulong guid]
        {
            get
            {
                LinkedListNode<EntityInfo> ret = null;
                if (m_LinkNodeDictionary.TryGetValue(guid, out ret) == true) {
                    return ret.Value;
                } else {
                    return default(EntityInfo);
                }
            }
            set
            {
                LinkedListNode<EntityInfo> linkNode;
                if (m_LinkNodeDictionary.TryGetValue(guid, out linkNode)) {
                    linkNode.Value = value;
                } else {
                    AddLast(guid, value);
                }
            }
        }
        public LinkedListNode<EntityInfo> FirstNode
        {
            get
            {
                return m_Objects.First;
            }
        }
        public LinkedListNode<EntityInfo> LastNode
        {
            get
            {
                return m_Objects.Last;
            }
        }
        public void CopyValuesTo(EntityInfoList list)
        {
            list.AddRange(m_Objects);
        }
        public IEnumerable<EntityInfo> Values
        {
            get
            {
                return m_Objects;
            }
        }

        private Dictionary<ulong, LinkedListNode<EntityInfo>> m_LinkNodeDictionary = new Dictionary<ulong, LinkedListNode<EntityInfo>>();
        private LinkedList<EntityInfo> m_Objects = new LinkedList<EntityInfo>();
    }
}
