using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameLibrary
{
    public sealed class TimeUtility
    {
        public static long AverageRoundtripTime
        {
            get { return s_AverageRoundtripTime; }
            set { s_AverageRoundtripTime = value; }
        }
        public static long GetTimeUsAtFrameStart()
        {
            return s_TimeUsAtFrameStart;
        }
        public static float GetFloatTime()
        {
            return s_GfxFloatTime;
        }
        public static float GetTimeScale()
        {
            return s_GfxTimeScale;
        }
        public static long GetLocalMilliseconds()
        {
            return s_GfxTime;
        }
        public static long GetLocalRealMilliseconds()
        {
            return s_GfxRealTime;
        }

        public static int GetLocalRealSeconds()
        {
            long seconds = s_GfxRealTime / 1000;
            return (int)seconds;
        }

        public static long GetElapsedTimeUs()
        {
            return (long)(Stopwatch.GetTimestamp() / s_TickPerUs);
        }
        public static void UpdateGfxTime(float time, float realTime, float timeScale)
        {
            s_GfxFloatTime = time;
            s_GfxTime = (long)(time * 1000);
            s_GfxRealTime = (long)(realTime * 1000);
            s_GfxTimeScale = timeScale;
            s_TimeUsAtFrameStart = GetElapsedTimeUs();
        }
        public static string CountDownString(int seconds) {
            if (seconds > 3600) {
                return string.Format("{0}:{1:D2}:{2:D2}",seconds / 3600, (seconds % 3600) / 60, seconds % 60);
            }else if(seconds > 60) {
                return string.Format("{0}:{1:D2}", seconds / 60, seconds % 60);
            } else {
                return seconds.ToString();
            }
        }

        public static DateTime GetDateTimeBySecond(int timeStamp)// timestamp单位秒
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = ((long)timeStamp * 10000000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }


        private static long s_AverageRoundtripTime = 0;
        private static long s_GfxTime = 0;
        private static long s_GfxRealTime = 0;
        private static float s_GfxTimeScale = 0;
        private static float s_GfxFloatTime = 0;
        private static long s_TimeUsAtFrameStart = 0;
        private static double s_TickPerUs = Stopwatch.Frequency / 1000000.0;
    }

    public sealed class TimeSnapshot
    {
        public static void Start()
        {
            Instance.Start_();
        }
        public static long End()
        {
            return Instance.End_();
        }
        public static long DoCheckPoint()
        {
            return Instance.DoCheckPoint_();
        }

        private void Start_()
        {
            m_LastSnapshotTime = TimeUtility.GetElapsedTimeUs();
            m_StartTime = m_LastSnapshotTime;
        }
        private long End_()
        {
            m_EndTime = TimeUtility.GetElapsedTimeUs();
            return m_EndTime - m_StartTime;
        }
        private long DoCheckPoint_()
        {
            long curTime = TimeUtility.GetElapsedTimeUs();
            long ret = curTime - m_LastSnapshotTime;
            m_LastSnapshotTime = curTime;
            return ret;
        }

        private long m_StartTime = 0;
        private long m_LastSnapshotTime = 0;
        private long m_EndTime = 0;

        private static TimeSnapshot Instance
        {
            get
            {
                if (null == s_Instance) {
                    s_Instance = new TimeSnapshot();
                }
                return s_Instance;
            }
        }

        [ThreadStatic]
        private static TimeSnapshot s_Instance = null;
    }
}
