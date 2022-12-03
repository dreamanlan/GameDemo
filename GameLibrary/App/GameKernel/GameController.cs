//#define USE_DISK_LOG

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GameLibrary;
using GameLibrary.GmCommands;

namespace GameLibrary
{
    public sealed class GameControler
    {
        //----------------------------------------------------------------------
        // 标准接口
        //----------------------------------------------------------------------
        public bool IsInited
        {
            get { return m_IsInited; }
        }
        public void InitLog(string logPath, string suffix)
        {
            m_IsInited = true;
            m_Logger.Init(logPath, suffix);
            m_MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            GlobalVariables.Instance.IsDebug = false;

            LogSystem.OnOutput = (Log_Type type, string msg) => {
#if DEBUG
                if (System.Threading.Thread.CurrentThread.ManagedThreadId == m_MainThreadId) {
                    if (type == Log_Type.LT_Warn) {
                        Utility.GfxLog("{0}", msg);
                    } else if (type == Log_Type.LT_Error) {
                        Utility.GfxErrorLog("{0}", msg);
                    }
                }
#endif
                m_Logger.Log("{0}", msg);
            };
        }
        public void InitGame()
        {
            Utility.GfxLog("GameControler.InitGame");
            SceneSystem.Instance.Init();
        }
        public void PauseGame(bool isPause)
        {
            GlobalVariables.Instance.IsPaused = isPause;
        }
        public void Release()
        {
            Utility.GfxLog("GameControler.Release");
            SceneSystem.Instance.Release();
            m_Logger.Dispose();
        }
        public void TickGame()
        {
            try {
                UnityEngine.Profiling.Profiler.BeginSample("GameController.TickGame");
                TimeUtility.UpdateGfxTime(UnityEngine.Time.time, UnityEngine.Time.realtimeSinceStartup, UnityEngine.Time.timeScale);
                if (!GlobalVariables.Instance.IsPaused) {
                    SceneSystem.Instance.Tick();
                }
                SceneSystem.Instance.GmTick();
                m_Logger.Tick();
            } finally {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        public Logger LoggerInstance
        {
            get
            {
                if (null == m_Logger) {
                    m_Logger = new Logger();
                }
                return m_Logger;
            }
        }

        private Logger m_Logger = new Logger();
        private int m_MainThreadId = 0;
        private bool m_IsInited = false;

        public static GameControler Instance
        {
            get { return s_Instance; }
        }
        private static GameControler s_Instance = new GameControler();
    }
}