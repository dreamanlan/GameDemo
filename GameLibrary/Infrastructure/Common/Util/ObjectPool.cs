using System;
using System.Collections.Generic;

namespace GameLibrary
{
    public interface IPoolAllocatedObject<T> where T : IPoolAllocatedObject<T>, new()
    {
        void InitPool(ObjectPool<T> pool);
        T Downcast();
    }

    public class ObjectPool<T> where T : IPoolAllocatedObject<T>, new()
    {
        public void Init(int initPoolSize)
        {
            for (int i = 0; i < initPoolSize; ++i) {
                T t = new T();
                t.InitPool(this);
                m_UnusedObjects.Enqueue(t);
            }
        }
        public T Alloc()
        {
            if (m_UnusedObjects.Count > 0)
                return m_UnusedObjects.Dequeue();
            else {
                T t = new T();
                if (null != t) {
                    t.InitPool(this);
                }
                return t;
            }
        }
        public void Recycle(IPoolAllocatedObject<T> t)
        {
            if (null != t) {
                m_UnusedObjects.Enqueue(t.Downcast());
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

    public interface IPoolAllocatedObjectEx<T> where T : IPoolAllocatedObjectEx<T>
    {
        void InitPool(ObjectPoolEx<T> pool);
        T Downcast();
    }

    public class ObjectPoolEx<T> where T : IPoolAllocatedObjectEx<T>
    {
        public void Init(int initPoolSize, Func<T> factory)
        {
            for (int i = 0; i < initPoolSize; ++i) {
                T t = factory();
                t.InitPool(this);
                m_UnusedObjects.Enqueue(t);
            }
        }
        public T Alloc(Func<T> factory)
        {
            if (m_UnusedObjects.Count > 0)
                return m_UnusedObjects.Dequeue();
            else {
                T t = factory();
                if (null != t) {
                    t.InitPool(this);
                }
                return t;
            }
        }
        public void Recycle(IPoolAllocatedObjectEx<T> t)
        {
            if (null != t) {
                m_UnusedObjects.Enqueue(t.Downcast());
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
