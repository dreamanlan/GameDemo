
using System;

namespace StoryScript
{
    public enum StoryLogType
    {
        Debug,
        Info,
        Warn,
        Error,
        Assert,
    }
    public delegate void LogSystemOutputDelegation(StoryLogType type, string msg);

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
                Output(StoryLogType.Debug, str);
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
                Output(StoryLogType.Info, str);
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
                Output(StoryLogType.Warn, str);
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
                Output(StoryLogType.Error, str);
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
                    Output(StoryLogType.Assert, str);
                }
            }
        }
        public static void Log(string msg)
        {
            if (!StoryConfigManager.Instance.LoggerEnabled)
                return;
            if (!StoryConfigManager.Instance.IsDevice || StoryConfigManager.Instance.IsDevelopment || StoryConfigManager.Instance.IsDebug) {
                Output(StoryLogType.Info, msg);
            }
        }

        private static void Output(StoryLogType type, string msg)
        {
            if (null != OnOutput) {
                OnOutput(type, msg);
            }
        }
    }
}
