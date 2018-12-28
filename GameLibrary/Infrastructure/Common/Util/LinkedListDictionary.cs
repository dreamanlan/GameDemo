﻿using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
    public sealed class IntComparer : IEqualityComparer<int>
    {
        public int GetHashCode(int obj)
        {
            return obj;
        }
        public bool Equals(int x, int y)
        {
            return x == y;
        }
        public static object Instance
        {
            get
            {
                return s_Instance;
            }
        }

        private static IntComparer s_Instance = new IntComparer();
    }
    public sealed class UintComparer : IEqualityComparer<uint>
    {
        public int GetHashCode(uint obj)
        {
            return (int)obj;
        }
        public bool Equals(uint x, uint y)
        {
            return x == y;
        }
        public static object Instance
        {
            get
            {
                return s_Instance;
            }
        }

        private static UintComparer s_Instance = new UintComparer();
    }
    public sealed class LongComparer : IEqualityComparer<long>
    {
        public int GetHashCode(long obj)
        {
            return (int)obj;
        }
        public bool Equals(long x, long y)
        {
            return x == y;
        }
        public static object Instance
        {
            get
            {
                return s_Instance;
            }
        }

        private static LongComparer s_Instance = new LongComparer();
    }
    public sealed class UlongComparer : IEqualityComparer<ulong>
    {
        public int GetHashCode(ulong obj)
        {
            return (int)obj;
        }
        public bool Equals(ulong x, ulong y)
        {
            return x == y;
        }
        public static object Instance
        {
            get
            {
                return s_Instance;
            }
        }

        private static UlongComparer s_Instance = new UlongComparer();
    }
    public sealed class MyDefaultComparer<T> : IEqualityComparer<T>
    {
        public int GetHashCode(T obj)
        {
            if (obj == null) {
                return 0;
            }
            return obj.GetHashCode();
        }
        public bool Equals(T x, T y)
        {
            if (x == null) {
                return y == null;
            }
            return x.Equals(y);
        }
        public static MyDefaultComparer<T> Instance
        {
            get
            {
                return s_Instance;
            }
        }

        private static MyDefaultComparer<T> s_Instance = new MyDefaultComparer<T>();
    }
}

namespace GameLibrary
{
    /// <summary>
    /// 我们频繁用到增加与删除少，但根据id查询对象与遍历对象都很多的情形。这个类提供对这种应用场景具有较好性能的容器实现。
    /// </summary>
    /// <typeparam name="KeyT"></typeparam>
    /// <typeparam name="ValueT"></typeparam>
    /// <remarks>
    /// 频繁使用时需要注意foreach语句的开销，本类不期望用于foreach语句遍历(保留是为了兼容现有代码，逐渐会替换掉)。
    /// 另，本类不提供id遍历，所以使用时KeyT应可由ValueT推导出来。
    /// </remarks>
    public sealed class LinkedListDictionary<TKey, TValue>
    {
        public bool Contains(TKey id)
        {
            return m_LinkNodeDictionary.ContainsKey(id);
        }
        ///这里不考虑重复，外界调用时保证（性能考虑）
        public void AddFirst(TKey id, TValue obj)
        {
            LinkedListNode<TValue> linkNode = m_Objects.AddFirst(obj);
            if (null != linkNode) {
                try {
                    m_LinkNodeDictionary.Add(id, linkNode);
                } catch (Exception ex) {
                    m_Objects.RemoveFirst();
                    LogSystem.Error("LinkedListDictionary.AddFirst throw Exception:{0}\n{1}\n Add id:{2}", ex.Message, ex.StackTrace, id);
                }
            }
        }
        public void AddLast(TKey id, TValue obj)
        {
            LinkedListNode<TValue> linkNode = m_Objects.AddLast(obj);
            if (null != linkNode) {
                try {
                    m_LinkNodeDictionary.Add(id, linkNode);
                } catch (Exception ex) {
                    m_Objects.RemoveLast();
                    LogSystem.Error("LinkedListDictionary.AddLast throw Exception:{0}\n{1}\n Add id:{2}", ex.Message, ex.StackTrace, id);
                }
            }
        }
        public void Remove(TKey id)
        {
            LinkedListNode<TValue> linkNode;
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
        public bool TryGetValue(TKey id, out TValue value)
        {
            LinkedListNode<TValue> linkNode;
            bool ret = m_LinkNodeDictionary.TryGetValue(id, out linkNode);
            if (ret) {
                value = linkNode.Value;
            } else {
                value = default(TValue);
            }
            return ret;
        }
        public void VisitValues(MyAction<TValue> visitor)
        {
            for (LinkedListNode<TValue> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                visitor(linkNode.Value);
            }
        }
        public void VisitValues(MyFunc<TValue, bool> visitor)
        {
            for (LinkedListNode<TValue> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                if (!visitor(linkNode.Value))
                    break;
            }
        }
        public TValue FindValue(MyFunc<TValue, bool> visitor)
        {
            for (LinkedListNode<TValue> linkNode = m_Objects.First; null != linkNode && m_Objects.Count > 0; linkNode = linkNode.Next) {
                if (visitor(linkNode.Value)) {
                    return linkNode.Value;
                }
            }
            return default(TValue);
        }
        public int Count
        {
            get
            {
                return m_LinkNodeDictionary.Count;
            }
        }
        public TValue this[TKey id]
        {
            get
            {
                LinkedListNode<TValue> ret = null;
                if (m_LinkNodeDictionary.TryGetValue(id, out ret) == true) {
                    return ret.Value;
                } else {
                    return default(TValue);
                }
            }
            set
            {
                LinkedListNode<TValue> linkNode;
                if (m_LinkNodeDictionary.TryGetValue(id, out linkNode)) {
                    linkNode.Value = value;
                } else {
                    AddLast(id, value);
                }
            }
        }
        public LinkedListNode<TValue> FirstNode
        {
            get
            {
                return m_Objects.First;
            }
        }
        public LinkedListNode<TValue> LastNode
        {
            get
            {
                return m_Objects.Last;
            }
        }
        public void CopyValuesTo(TValue[] array, int index)
        {
            m_Objects.CopyTo(array, index);
        }
        public IEnumerable<TValue> Values
        {
            get
            {
                return m_Objects;
            }
        }

        private Dictionary<TKey, LinkedListNode<TValue>> m_LinkNodeDictionary = new Dictionary<TKey, LinkedListNode<TValue>>();
        private LinkedList<TValue> m_Objects = new LinkedList<TValue>();
    }
}
