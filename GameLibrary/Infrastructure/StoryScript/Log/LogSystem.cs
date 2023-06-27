
using System;

namespace StoryScript
{
    public enum Log_Type
    {
        LT_Debug,
        LT_Info,
        LT_Warn,
        LT_Error,
        LT_Assert,
    }
    public delegate void LogSystemOutputDelegation(Log_Type type, string msg);

    public class LogSystem
    {
        public static LogSystemOutputDelegation OnOutput;
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Debug(string format, params object[] args)
        {
#if DEBUG
            if (!StoryConfigManager.Instance.LoggerEnabled)
                return;
            if (!StoryConfigManager.Instance.IsDevice || StoryConfigManager.Instance.IsDebug) {
                format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                string str = string.Format("[Debug]:" + format, args);
                Output(Log_Type.LT_Debug, str);
            }
#endif
        }
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Info(string format, params object[] args)
        {
            if (!StoryConfigManager.Instance.LoggerEnabled)
                return;
            if (!StoryConfigManager.Instance.IsDevice || StoryConfigManager.Instance.IsDevelopment || StoryConfigManager.Instance.IsDebug) {
                string str = string.Format("[Info]:" + format, args);
                Output(Log_Type.LT_Info, str);
            }
        }
        [System.Diagnostics.Conditional("ENABLE_ERROR_LOG")]
        public static void Warn(string format, params object[] args)
        {
            if (!StoryConfigManager.Instance.LoggerEnabled)
                return;
            if (!StoryConfigManager.Instance.IsDevice || StoryConfigManager.Instance.IsDevelopment || StoryConfigManager.Instance.IsDebug) {
                format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                string str = string.Format("[Warn]:" + format, args);
                Output(Log_Type.LT_Warn, str);
            }
        }
        [System.Diagnostics.Conditional("ENABLE_ERROR_LOG")]
        public static void Error(string format, params object[] args)
        {
            if (!StoryConfigManager.Instance.LoggerEnabled)
                return;
            if (!StoryConfigManager.Instance.IsDevice || StoryConfigManager.Instance.IsDevelopment || StoryConfigManager.Instance.IsDebug) {
                format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                string str = string.Format("[Error]:" + format, args);
                Output(Log_Type.LT_Error, str);
            }
        }
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Assert(bool check, string format, params object[] args)
        {
            if (!StoryConfigManager.Instance.LoggerEnabled)
                return;
            if (!StoryConfigManager.Instance.IsDevice || StoryConfigManager.Instance.IsDevelopment || StoryConfigManager.Instance.IsDebug) {
                if (!check) {
                    format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                    string str = string.Format("[Assert]:" + format, args);
                    Output(Log_Type.LT_Assert, str);
                }
            }
        }
        public static void Log(string msg)
        {
            if (!StoryConfigManager.Instance.LoggerEnabled)
                return;
            if (!StoryConfigManager.Instance.IsDevice || StoryConfigManager.Instance.IsDevelopment || StoryConfigManager.Instance.IsDebug) {
                Output(Log_Type.LT_Info, msg);
            }
        }

        private static void Output(Log_Type type, string msg)
        {
            if (null != OnOutput) {
                OnOutput(type, msg);
            }
        }
    }
}
