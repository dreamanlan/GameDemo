using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace GameLibrary
{
    public sealed class Logger : IDisposable
    {
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }
        public void Log(string format, params object[] args)
        {
            if (!m_Enabled)
                return;
            string msg = DateTime.Now.ToString("HH-mm-ss-fff:") + string.Format(format, args);
#if USE_DISK_LOG
        m_LogStream.WriteLine(msg);
        m_LogStream.Flush();
#else
            m_LogQueue.Enqueue(msg);
            if (m_LogQueue.Count >= c_FlushCount) {
                m_LastFlushTime = StoryScript.TimeUtility.GetLocalMilliseconds();

                RequestFlush();
            }
#endif
        }
        public void Init(string logPath, string suffix)
        {
            string logFile = string.Format("{0}/Game{1}.log", logPath, suffix);
            m_LogStream = new StreamWriter(logFile, false);
#if !USE_DISK_LOG
            m_Thread.OnQuitEvent = OnThreadQuit;
            m_Thread.Start();
#endif
            Log("======GameLog Start ({0}, {1})======", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
        }
        public void Dispose()
        {
            Release();
        }
        public void Tick()
        {
#if !USE_DISK_LOG
            long curTime = StoryScript.TimeUtility.GetLocalMilliseconds();
            if (m_LastFlushTime + 10000 < curTime) {
                m_LastFlushTime = curTime;

                RequestFlush();
            }
#endif
        }
        private void Release()
        {
#if !USE_DISK_LOG
            m_Thread.Stop();
#endif
            m_LogStream.Close();
            m_LogStream.Dispose();
        }
#if !USE_DISK_LOG
        private void RequestFlush()
        {
            m_Thread.QueueAction(FlushToFile);
        }
        private void OnThreadQuit()
        {
            while (m_LogQueue.Count>0) {
                string msg;
                if (m_LogQueue.TryDequeue(out msg)) {
                    m_LogStream.WriteLine(msg);
                }
            }
            m_LogStream.Flush();
        }
        private void FlushToFile()
        {
            int ct = m_LogQueue.Count;
            for (int ix = ct; ix > 0; --ix) {
                string msg;
                if (m_LogQueue.TryDequeue(out msg)) {
                    m_LogStream.WriteLine(msg);
                }
            }
            m_LogStream.Flush();
        }

        private MyClientThread m_Thread = new MyClientThread();
        private ClientConcurrentQueue<string> m_LogQueue = new ClientConcurrentQueue<string>();

        private long m_LastFlushTime = 0;
        private const int c_FlushCount = 4096;
#endif
        private StreamWriter m_LogStream;
        private bool m_Enabled = true;
    }
}
