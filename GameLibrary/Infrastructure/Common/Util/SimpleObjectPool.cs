using System;
using System.Collections.Generic;

namespace GameLibrary
{
    public class SimpleObjectPool<T> where T : new()
    {
        public void Init(int initPoolSize)
        {
            for (int i = 0; i < initPoolSize; ++i) {
                T t = new T();
                m_UnusedObjects.Enqueue(t);
            }
        }
        public T Alloc()
        {
            if (m_UnusedObjects.Count > 0)
                return m_UnusedObjects.Dequeue();
            else {
                T t = new T();
                return t;
            }
        }
        public void Recycle(T t)
        {
            if (null != t) {
                m_UnusedObjects.Enqueue(t);
            }
        }
        public void Clear()
        {
            m_UnusedObjects.Clear();
        }
        public int Count
        {
            get
            {
                return m_UnusedObjects.Count;
            }
        }
        private Queue<T> m_UnusedObjects = new Queue<T>();
    }
    public class SimpleObjectPoolEx<T>
    {
        public void Init(int initPoolSize, Func<T> factory)
        {
            for (int i = 0; i < initPoolSize; ++i) {
                T t = factory();
                m_UnusedObjects.Enqueue(t);
            }
        }
        public T Alloc(Func<T> factory)
        {
            if (m_UnusedObjects.Count > 0)
                return m_UnusedObjects.Dequeue();
            else {
                T t = factory();
                return t;
            }
        }
        public void Recycle(T t)
        {
            if (null != t) {
                m_UnusedObjects.Enqueue(t);
            }
        }
        public void Clear()
        {
            Clear(null);
        }
        public void Clear(Action<T> destroyFunc)
        {
            if (null != destroyFunc) {
                foreach (var item in m_UnusedObjects) {
                    destroyFunc(item);
                }
            }
            m_UnusedObjects.Clear();
        }
        public int Count
        {
            get
            {
                return m_UnusedObjects.Count;
            }
        }
        private Queue<T> m_UnusedObjects = new Queue<T>();
    }
}
