using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GameLibrary
{
    public class ClientConcurrentActionProcessor
    {
        public int CurActionNum
        {
            get
            {
                return m_Actions.Count;
            }
        }

        public void QueueAction(MyAction action)
        {
            m_Actions.Enqueue(action);
        }

        public void QueueFunc(MyFunc action)
        {
            bool needUseLambda = true;
            ClientConcurrentObjectPool<ClientConcurrentPoolAllocatedFunc> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                ClientConcurrentPoolAllocatedFunc helper = pool.Alloc();
                if (null != helper) {
                    helper.Init(action);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(); });
                LogSystem.Warn("QueueAction {0}() use lambda expression, maybe out of memory.", action.Method.ToString());
            }
        }

        public void QueueFunc<R>(MyFunc<R> action)
        {
            bool needUseLambda = true;
            ClientConcurrentObjectPool<ClientConcurrentPoolAllocatedFunc<R>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                ClientConcurrentPoolAllocatedFunc<R> helper = pool.Alloc();
                if (null != helper) {
                    helper.Init(action);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(); });
                LogSystem.Warn("QueueAction {0}() use lambda expression, maybe out of memory.", action.Method.ToString());
            }
        }

        public MyAction DequeueAction()
        {
            MyAction action = null;
            m_Actions.TryDequeue(out action);
            return action;
        }

        public void HandleActions(int maxCount)
        {
            try {
                for (int i = 0; i < maxCount; ++i) {
                    if (m_Actions.Count > 0) {
                        MyAction action = null;
                        m_Actions.TryDequeue(out action);
                        if (null != action) {
                            try {
                                action();
                            } catch (Exception ex) {
                                LogSystem.Error("ClientConcurrentActionProcessor action() throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
                            }
                        }
                    } else {
                        break;
                    }
                }
            } catch (Exception ex) {
                LogSystem.Error("ClientConcurrentActionProcessor.HandleActions throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public void Reset()
        {
            m_Actions.Clear();
            m_ActionPools.Clear();
        }

        public void ClearPool(int clearOnSize)
        {
            m_ActionPools.Visit((object type, object pool) => {
                Type t = type as Type;
                if (null != t) {
                    object ret = t.InvokeMember("Count", System.Reflection.BindingFlags.GetProperty, null, pool, null);
                    if (null != ret && ret is int) {
                        int ct = (int)ret;
                        if (ct > clearOnSize) {
                            t.InvokeMember("Clear", System.Reflection.BindingFlags.InvokeMethod, null, pool, null);
                        }
                    }
                }
            });
        }

        public void DebugPoolCount(MyAction<string> output)
        {
            m_ActionPools.Visit((object type, object pool) => {
                Type t = type as Type;
                if (null != t) {
                    object ret = t.InvokeMember("Count", System.Reflection.BindingFlags.GetProperty, null, pool, null);
                    if (null != ret) {
                        output(string.Format("ActionPool {0} buffered {1} objects", pool.GetType().ToString(), (int)ret));
                    }
                } else {
                    output(string.Format("ActionPool contain a null pool ({0}) ..", pool.GetType().ToString()));
                }
            });
        }

        public ClientConcurrentActionProcessor()
        {
        }

        private ClientConcurrentQueue<MyAction> m_Actions = new ClientConcurrentQueue<MyAction>();
        private ClientConcurrentTypedDataCollection m_ActionPools = new ClientConcurrentTypedDataCollection();
    }
    public class ClientAsyncActionProcessor : ClientConcurrentActionProcessor
    {
    }
    public class ClientDelayActionProcessor : ClientConcurrentActionProcessor
    {
    }
}