﻿//#define USE_DISK_LOG

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GameLibrary;
using GameLibrary.GmCommands;
using StoryScript;

namespace GameLibrary
{
    public sealed class GameControler
    {
        //----------------------------------------------------------------------
        // standard interface
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
            StoryConfigManager.Instance.IsDebug = false;

            Action<GameLogType, string> onLog = (GameLogType type, string msg) => {
#if DEBUG
                if (System.Threading.Thread.CurrentThread.ManagedThreadId == m_MainThreadId) {
                    if (type == GameLogType.Warn) {
                        Utility.GfxLog("{0}", msg);
                    } else if (type == GameLogType.Error) {
                        Utility.GfxErrorLog("{0}", msg);
                    }
                }
#endif
                m_Logger.Log("{0}", msg);
            };
            LogSystem.OnOutput = (GameLogType type, string msg) => onLog(type, msg);
            StoryScript.LogSystem.OnOutput = (StoryScript.StoryLogType type, string msg) => onLog((GameLogType)type, msg);
        }
        public void InitGame()
        {
            Utility.GfxLog("GameControler.InitGame");
            SceneSystem.Instance.Init();
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
                StoryScript.TimeUtility.UpdateGfxTime(UnityEngine.Time.time, UnityEngine.Time.realtimeSinceStartup, UnityEngine.Time.timeScale);
                SceneSystem.Instance.Tick();
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