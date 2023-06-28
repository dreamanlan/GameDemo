
using System;

namespace GameLibrary
{
    public enum GameLogType
    {
        Debug,
        Info,
        Warn,
        Error,
        Assert,
    }
    public delegate void LogSystemOutputDelegation(GameLogType type, string msg);

    public class LogSystem
    {
        public static LogSystemOutputDelegation OnOutput;
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Debug(string format, params object[] args)
        {
#if DEBUG
            if (!GlobalVariables.Instance.LoggerEnabled)
                return;
            if (!GlobalVariables.Instance.IsDevice || GlobalVariables.Instance.IsDebug) {
                format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                string str = string.Format("[Debug]:" + format, args);
                Output(GameLogType.Debug, str);
            }
#endif
        }
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Info(string format, params object[] args)
        {
            if (!GlobalVariables.Instance.LoggerEnabled)
                return;
            if (!GlobalVariables.Instance.IsDevice || GlobalVariables.Instance.IsDevelopment || GlobalVariables.Instance.IsDebug) {
                string str = string.Format("[Info]:" + format, args);
                Output(GameLogType.Info, str);
            }
        }
        [System.Diagnostics.Conditional("ENABLE_ERROR_LOG")]
        public static void Warn(string format, params object[] args)
        {
            if (!GlobalVariables.Instance.LoggerEnabled)
                return;
            if (!GlobalVariables.Instance.IsDevice || GlobalVariables.Instance.IsDevelopment || GlobalVariables.Instance.IsDebug) {
                format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                string str = string.Format("[Warn]:" + format, args);
                Output(GameLogType.Warn, str);
            }
        }
        [System.Diagnostics.Conditional("ENABLE_ERROR_LOG")]
        public static void Error(string format, params object[] args)
        {
            if (!GlobalVariables.Instance.LoggerEnabled)
                return;
            if (!GlobalVariables.Instance.IsDevice || GlobalVariables.Instance.IsDevelopment || GlobalVariables.Instance.IsDebug) {
                format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                string str = string.Format("[Error]:" + format, args);
                Output(GameLogType.Error, str);
            }
        }
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Assert(bool check, string format, params object[] args)
        {
            if (!GlobalVariables.Instance.LoggerEnabled)
                return;
            if (!GlobalVariables.Instance.IsDevice || GlobalVariables.Instance.IsDevelopment || GlobalVariables.Instance.IsDebug) {
                if (!check) {
                    format = "Time:" + DateTime.Now.ToString("HH-mm-ss-fff:") + format;
                    string str = string.Format("[Assert]:" + format, args);
                    Output(GameLogType.Assert, str);
                }
            }
        }
        public static void Log(string msg)
        {
            if (!GlobalVariables.Instance.LoggerEnabled)
                return;
            if (!GlobalVariables.Instance.IsDevice || GlobalVariables.Instance.IsDevelopment || GlobalVariables.Instance.IsDebug) {
                Output(GameLogType.Info, msg);
            }
        }

        private static void Output(GameLogType type, string msg)
        {
            if (null != OnOutput) {
                OnOutput(type, msg);
            }
        }
    }
}
