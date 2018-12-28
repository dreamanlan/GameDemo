using System;
using System.Collections.Generic;
using System.Threading;

namespace GameLibrary
{
    public delegate void MyClientThreadEventDelegate();
    public class MyClientThread
    {
        public MyClientThread()
        {
            InitThread(new ClientAsyncActionProcessor());
        }
        public MyClientThread(int tickSleepTime)
        {
            m_TickSleepTime = tickSleepTime;
            InitThread(new ClientAsyncActionProcessor());
        }
        public MyClientThread(int tickSleepTime, int actionNumPerTick)
        {
            m_TickSleepTime = tickSleepTime;
            m_ActionNumPerTick = actionNumPerTick;
            InitThread(new ClientAsyncActionProcessor());
        }
        public MyClientThread(ClientAsyncActionProcessor asyncActionQueue)
        {
            InitThread(asyncActionQueue);
        }
        public MyClientThread(int tickSleepTime, ClientAsyncActionProcessor asyncActionQueue)
        {
            m_TickSleepTime = tickSleepTime;
            InitThread(asyncActionQueue);
        }
        public MyClientThread(int tickSleepTime, int actionNumPerTick, ClientAsyncActionProcessor asyncActionQueue)
        {
            m_TickSleepTime = tickSleepTime;
            m_ActionNumPerTick = actionNumPerTick;
            InitThread(asyncActionQueue);
        }

        public int TickSleepTime
        {
            get { return m_TickSleepTime; }
            set { m_TickSleepTime = value; }
        }

        public int ActionNumPerTick
        {
            get
            {
                return m_ActionNumPerTick;
            }
            set
            {
                m_ActionNumPerTick = value;
            }
        }

        public int CurActionNum
        {
            get
            {
                return m_ActionQueue.CurActionNum;
            }
        }

        public void DebugPoolCount(MyAction<string> output)
        {
            m_ActionQueue.DebugPoolCount(output);
        }

        public void ClearPool(int clearOnSize)
        {
            m_ActionQueue.ClearPool(clearOnSize);
        }

        public void Start()
        {
            m_IsRun = true;
            m_Thread.Start();
        }

        public void Stop()
        {
            m_IsRun = false;
            m_Thread.Join();
        }

        public bool IsCurrentThread
        {
            get
            {
                return m_Thread == Thread.CurrentThread;
            }
        }

        public Thread Thread
        {
            get
            {
                return m_Thread;
            }
        }

        public void QueueAction(MyAction action)
        {
            m_ActionQueue.QueueAction(action);
        }

        public void QueueFunc(MyFunc action)
        {
            m_ActionQueue.QueueFunc(action);
        }

        public void QueueFunc<R>(MyFunc<R> action)
        {
            m_ActionQueue.QueueFunc(action);
        }

        public MyClientThreadEventDelegate OnStartEvent;
        public MyClientThreadEventDelegate OnTickEvent;
        public MyClientThreadEventDelegate OnQuitEvent;

        protected virtual void OnStart()
        {
        }
        protected virtual void OnTick()
        {
        }
        protected virtual void OnQuit()
        {
        }

        private void InitThread(ClientAsyncActionProcessor asyncActionQueue)
        {
            m_Thread = new Thread(this.Loop);
            m_ActionQueue = asyncActionQueue;
        }

        private void Loop()
        {
            try {
                if (OnStartEvent != null)
                    OnStartEvent();
                else
                    OnStart();
                while (m_IsRun) {
                    try {
                        if (OnTickEvent != null)
                            OnTickEvent();
                        else
                            OnTick();
                    } catch (Exception ex) {
                        LogSystem.Error("ClientThread.Tick throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
                    }
                    m_ActionQueue.HandleActions(m_ActionNumPerTick);
                    Thread.Sleep(m_TickSleepTime);
                }
                if (OnQuitEvent != null)
                    OnQuitEvent();
                else
                    OnQuit();
            } catch (Exception ex) {
                LogSystem.Error("ClientThread.Loop throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private Thread m_Thread = null;
        private bool m_IsRun = true;

        private int m_TickSleepTime = 10;
        private int m_ActionNumPerTick = 1024;

        private ClientAsyncActionProcessor m_ActionQueue = null;
    }
}
