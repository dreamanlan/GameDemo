using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary
{
    /// <summary>
    /// 这里放客户端与服务器存在差异的变量值，供各公共模块使用（如果是各模块所需的逻辑数据，则不要放在这里，独立写读表器）。
    /// </summary>
    public class GlobalVariables
    {
        public bool IsDebug
        {
            get { return m_IsDebug; }
            set { m_IsDebug = value; }
        }
        public bool LoggerEnabled
        {
            get { return m_LoggerEnabled; }
            set { m_LoggerEnabled = value; }
        }
        public bool IsEditor {
            get { return m_IsEditor; }
            set { m_IsEditor = value; }
        }
        public bool IsDevelopment
        {
            get { return m_IsDevelopment; }
            set { m_IsDevelopment = value; }
        }
        public bool IsDevice
        {
            get { return m_IsDevice; }
            set { m_IsDevice = value; }
        }
        public bool IsIphone6
        {
            get { return m_IsIphone6; }
            set { m_IsIphone6 = value; }
        }
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set { m_IsPaused = value; }
        }
        public bool IsStorySkipped
        {
            get { return m_IsStorySkipped; }
            set { m_IsStorySkipped = value; }
        }

        public Dictionary<string, string> EncodeTable
        {
            get { return m_EncodeTable; }
        }
        public Dictionary<string, string> DecodeTable
        {
            get { return m_DecodeTable; }
        }

        private static void AddCrypto(string s, string d, Dictionary<string, string> encodeTable, Dictionary<string, string> decodeTable)
        {
            encodeTable.Add(s, d);
            decodeTable.Add(d, s);
        }

        private GlobalVariables()
        {
        }

        private bool m_IsDebug = false;
        private bool m_LoggerEnabled = true;
        private bool m_IsEditor = false;
        private bool m_IsDevelopment = false;
        private bool m_IsDevice = false;
        private bool m_IsIphone6 = false;

		private Dictionary<string, string> m_EncodeTable = new Dictionary<string, string>();
        private Dictionary<string, string> m_DecodeTable = new Dictionary<string, string>();

        private bool m_IsPaused = false;
        private bool m_IsStorySkipped = false;
                
        public static GlobalVariables Instance
        {
            get { return s_Instance; }
        }
        private static GlobalVariables s_Instance = new GlobalVariables();
    }
}
