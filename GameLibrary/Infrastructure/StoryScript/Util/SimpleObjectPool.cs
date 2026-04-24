using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace StoryScript
{
    public class SimpleObjectPool<T> where T : new()
    {
        public SimpleObjectPool()
        {
            m_UnusedObjects = new ConcurrentQueue<T>();
        }
        public SimpleObjectPool(int initPoolSize)
        {
            m_UnusedObjects = new ConcurrentQueue<T>();
            Init(initPoolSize);
        }
        public void Init(int initPoolSize)
        {
            for (int i = 0; i < initPoolSize; ++i) {
                T t = new T();
                Recycle(t);
            }
        }
        public T Alloc()
        {
            if (m_UnusedObjects.TryDequeue(out var t)) {
                m_HashCodes.TryRemove(t.GetHashCode(), out _);
                return t;
            }
            else {
                T n = new T();
                return n;
            }
        }
        public void Recycle(T t)
        {
            if (null != t && m_UnusedObjects.Count < m_PoolSize) {
                int hashCode = t.GetHashCode();
                if (m_HashCodes.TryAdd(hashCode, 0)) {
                    m_UnusedObjects.Enqueue(t);
                }
            }
        }
        public void Clear()
        {
            m_HashCodes.Clear();
            while (m_UnusedObjects.TryDequeue(out _)) { }
        }
        public int Count
        {
            get {
                return m_UnusedObjects.Count;
            }
        }

        private ConcurrentDictionary<int, byte> m_HashCodes = new ConcurrentDictionary<int, byte>();
        private ConcurrentQueue<T> m_UnusedObjects = new ConcurrentQueue<T>();
        private int m_PoolSize = 4096;
    }
    public class SimpleObjectPoolEx<T>
    {
        public SimpleObjectPoolEx(Func<T> creater, Action<T> destroyer)
        {
            m_UnusedObjects = new ConcurrentQueue<T>();
            m_Creater = creater;
            m_Destroyer = destroyer;
        }
        public SimpleObjectPoolEx(int initPoolSize, Func<T> creater, Action<T> destroyer)
        {
            m_UnusedObjects = new ConcurrentQueue<T>();
            Init(initPoolSize, creater, destroyer);
        }
        public void Init(int initPoolSize, Func<T> creater, Action<T> destroyer)
        {
            m_Creater = creater;
            m_Destroyer = destroyer;
            for (int i = 0; i < initPoolSize; ++i) {
                T t = creater();
                Recycle(t);
            }
        }
        public T Alloc()
        {
            if (m_UnusedObjects.TryDequeue(out var t)) {
                m_HashCodes.TryRemove(t.GetHashCode(), out _);
                return t;
            }
            else {
                T n = m_Creater();
                return n;
            }
        }
        public void Recycle(T t)
        {
            if (null != t && m_UnusedObjects.Count < m_PoolSize) {
                int hashCode = t.GetHashCode();
                if (m_HashCodes.TryAdd(hashCode, 0)) {
                    m_UnusedObjects.Enqueue(t);
                }
            }
        }
        public void Clear()
        {
            if (null != m_Destroyer) {
                foreach (var item in m_UnusedObjects) {
                    m_Destroyer(item);
                }
            }
            m_HashCodes.Clear();
            while (m_UnusedObjects.TryDequeue(out _)) { }
        }
        public int Count
        {
            get {
                return m_UnusedObjects.Count;
            }
        }

        private ConcurrentDictionary<int, byte> m_HashCodes = new ConcurrentDictionary<int, byte>();
        private ConcurrentQueue<T> m_UnusedObjects = null;
        private Func<T> m_Creater = null;
        private Action<T> m_Destroyer = null;
        private int m_PoolSize = 4096;
    }
}
