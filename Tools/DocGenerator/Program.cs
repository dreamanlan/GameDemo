using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsLibrary;
using Dsl;

namespace DocGenerator
{
    internal class SnippetsInfo
    {
        internal string m_Key = string.Empty;
        internal string m_Prefix = string.Empty;
        internal List<string> m_Bodies = new List<string>();
        internal List<string> m_Descriptions = new List<string>();
    }
    internal class DocItemInfo
    {
        internal SnippetsInfo m_Snippets;
        internal List<string> m_Docs = new List<string>();
        internal int m_Order = 0;
    }
    internal class DocDslParser
    {
        internal string DslFile
        {
            get { return m_DslFile; }
        }
        public List<DocItemInfo> DocInfo
        {
            get { return m_DocInfo; }
        }
        internal bool Init(string dslFile)
        {
            m_DslFile = dslFile;
            try {
                DslFile file = new DslFile();
                if (!file.Load(dslFile, LogSystem.Log)) {
                    LogSystem.Error("DSL {0} load failed !", dslFile);
                    return false;
                }
                bool haveError = false;
                foreach (DslInfo info in file.DslInfos) {
                    if (info.GetId() == "@include") {
                        if (info.Functions.Count == 1) {
                            FunctionData funcData = info.First;
                            CallData call = funcData.Call;
                            if (null != call && call.GetParamNum() >= 1) {
                                string commentFile = call.GetParamId(0);
                                string path = Path.Combine(Path.GetDirectoryName(m_DslFile), commentFile);
                                try {
                                    string txt = File.ReadAllText(path);
                                    DocItemInfo item = new DocItemInfo();
                                    item.m_Docs.Add(txt);
                                    m_DocInfo.Add(item);
                                } catch (Exception fileEx) {
                                    LogSystem.Error("File.ReadAllLines({0}) throw exception {1}, line {2} file {3}", fileEx.Message, path, info.GetLine(), dslFile);
                                    haveError = true;
                                }
                            } else {
                                LogSystem.Error("@include must have param file , line {0} file {1}", info.GetLine(), dslFile);
                                haveError = true;
                            }
                        } else {
                            LogSystem.Error("@include must end with ';' , line {0} file {1}", info.GetLine(), dslFile);
                            haveError = true;
                        }
                    } else if (info.GetId() == "@doc") {
                        if (info.Functions.Count == 1) {
                            FunctionData funcData = info.First;
                            if (funcData.HaveExternScript()) {
                                string txt = funcData.GetExternScript();
                                DocItemInfo item = new DocItemInfo();
                                item.m_Docs.Add(RemoveFirstAndLastEmptyLine(txt));
                                m_DocInfo.Add(item);
                            } else {
                                LogSystem.Error("@doc must have externscript , line {0} file {1}", info.GetLine(), dslFile);
                                haveError = true;
                            }
                        } else {
                            LogSystem.Error("@doc must end with ';' , line {0} file {1}", info.GetLine(), dslFile);
                            haveError = true;
                        }
                    } else if (info.GetId() == "@order") {
                        if (info.Functions.Count == 1) {
                            FunctionData funcData = info.First;
                            CallData callData = funcData.Call;
                            if (null != callData && callData.GetParamNum() >= 1) {
                                DocItemInfo item = new DocItemInfo();
                                item.m_Order = int.Parse(callData.GetParamId(0));
                                m_DocInfo.Add(item);
                            } else {
                                LogSystem.Error("@order must have 1 param , line {0} file {1}", info.GetLine(), dslFile);
                                haveError = true;
                            }
                        } else {
                            LogSystem.Error("@order must end with ';' , line {0} file {1}", info.GetLine(), dslFile);
                            haveError = true;
                        }
                    } else if (info.GetId() == "@snippets") {
                        FunctionData funcData = info.First;
                        CallData callData = funcData.Call;
                        if (null != callData && callData.GetParamNum() >= 1) {
                            List<string> args = new List<string>();
                            for (int i = 0; i < callData.GetParamNum(); ++i) {
                                string v = callData.GetParamId(i);
                                args.Add(v);
                            }

                            DocItemInfo item = new DocItemInfo();
                            m_DocInfo.Add(item);

                            item.m_Snippets = new SnippetsInfo();
                            item.m_Snippets.m_Prefix = args[0];
                            string key = string.Join(" ", args.ToArray());
                            item.m_Snippets.m_Key = key;

                            if (m_SnippetsKeys.Contains(key)) {
                                LogSystem.Error("@snippets {0} key already exist ! line {1} file {2}", callData.ToScriptString(false), callData.GetLine(), dslFile);
                                haveError = true;
                            } else {
                                m_SnippetsKeys.Add(key);

                                foreach (FunctionData doc in info.Functions) {
                                    if (doc.HaveExternScript()) {
                                        string scp = doc.GetExternScript();
                                        scp = RemoveFirstAndLastEmptyLine(scp);
                                        if (doc.GetId() == "@snippets") {
                                            item.m_Snippets.m_Bodies.Add(scp);
                                        } else if (doc.GetId() == "@description") {
                                            item.m_Snippets.m_Descriptions.Add(scp);
                                        } else if (doc.GetId() == "@doc") {
                                            item.m_Docs.Add(scp);
                                        } else {
                                            LogSystem.Error("@snippets {0} function must be @snippets or @description or @doc ! line {1} file {2}", doc.ToScriptString(false), doc.GetLine(), dslFile);
                                            haveError = true;
                                        }
                                    } else {
                                        LogSystem.Error("@snippets {0} function must contains code ! line {1} file {2}", doc.ToScriptString(false), doc.GetLine(), dslFile);
                                        haveError = true;
                                    }
                                }
                            }
                        } else {
                            LogSystem.Error("@snippets {0} must have params ! line {1} file {2}", funcData.ToScriptString(false), funcData.GetLine(), dslFile);
                            haveError = true;
                        }
                    } else {
                        LogSystem.Error("Unknown part {0}, line {1} file {2}", info.GetId(), info.GetLine(), dslFile);
                        haveError = true;
                    }
                }
                return !haveError;
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return false;
        }
        internal void GenDocAndSnippets(string docFile)
        {
            using (StreamWriter sw = new StreamWriter(docFile)) {
                sw.WriteLine("//========================================================================================================================================================================================================");
                sw.WriteLine("//此文档由DocGenerator根据{0}生成，请勿手动修改！！！", DslFile);
                sw.WriteLine("//========================================================================================================================================================================================================");
                sw.WriteLine();
                int order = 1;
                foreach (DocItemInfo item in m_DocInfo) {
                    if (null != item.m_Snippets) {
                        int ct = item.m_Snippets.m_Bodies.Count;
                        for (int i = 0; i < ct; ++i) {
                            if (i == 0) {
                                sw.WriteLine("{0}.{1}", order++, item.m_Snippets.m_Bodies[i]);
                            } else {
                                sw.WriteLine("{0}", IndentDoc("  ", item.m_Snippets.m_Bodies[i]));
                            }
                        }
                        sw.WriteLine();
                    } else if (item.m_Order > 0) {
                        order = item.m_Order;
                    } else {
                        order = 1;
                    }
                    if (item.m_Docs.Count > 0) {
                        for (int i = 0; i < item.m_Docs.Count; ++i) {
                            sw.WriteLine("{0}", item.m_Docs[i]);
                        }
                        sw.WriteLine();
                    }
                }
            }
        }
        internal void GenVscodeSnippets(string jsonFile, string postfixKey)
        {
            using (StreamWriter sw = new StreamWriter(jsonFile)) {
                foreach (DocItemInfo item in m_DocInfo) {
                    if (null != item.m_Snippets) {
                        int ct1 = item.m_Snippets.m_Bodies.Count;
                        int ct2 = item.m_Snippets.m_Descriptions.Count;
                        for (int i = 0; i < ct1 || i < ct2; ++i) {
                            if (i < ct1) {
                                string key = item.m_Snippets.m_Key;
                                if (ct1 > 1)
                                    key = string.Format("{0} {1}", key, i + 1);
                                if (!string.IsNullOrEmpty(postfixKey)) {
                                    key += " " + postfixKey;
                                }
                                string prefix = item.m_Snippets.m_Prefix;
                                string body = item.m_Snippets.m_Bodies[i];
                                sw.WriteLine("\t\"{0}\": {{",key);
                                sw.WriteLine("\t\t\"prefix\": \"{0}\",",prefix);
		                        sw.WriteLine("\t\t\"body\": [");
                                sw.WriteLine("{0}", IndentCode("\t\t\t", QuoteString(body)));
		                        sw.WriteLine("\t\t],");
                                if (i < ct2) {
                                    sw.WriteLine("\t\t\"description\": \"{0}\"", QuoteString(item.m_Snippets.m_Descriptions[i]));
                                } else {
                                    sw.WriteLine("\t\t\"description\": \"{0}\"", key);
                                }
                                sw.WriteLine("\t\t},");
                                sw.WriteLine();
                            }
                        }
                    }
                }
            }
        }
        internal void GenCompositeDoc(string file)
        {
            using (StreamWriter sw = new StreamWriter(file)) {
                foreach (DocItemInfo item in m_DocInfo) {
                    if (item.m_Docs.Count > 0) {
                        for (int i = 0; i < item.m_Docs.Count; ++i) {
                            sw.WriteLine("{0}", item.m_Docs[i]);
                        }
                    }
                    if (null != item.m_Snippets) {
                        Console.WriteLine("Composite document can't include @snippets, please check {0} !", m_DslFile);
                    }
                }
            }
        }
        internal string GetIdVscodeColorizers()
        {
            StringBuilder sb = new StringBuilder();
            string prefix = "";
            foreach (DocItemInfo item in m_DocInfo) {
                if (null != item.m_Snippets && Array.IndexOf(s_Keywords, item.m_Snippets.m_Prefix) < 0 && IsId(item.m_Snippets.m_Prefix)) {
                    sb.AppendFormat("{0}{1}", prefix, item.m_Snippets.m_Prefix);
                    prefix = "|";
                }
            }
            return sb.ToString();
        }
        internal string GetIdUltraEditColorizers()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DocItemInfo item in m_DocInfo) {
                if (null != item.m_Snippets && Array.IndexOf(s_Keywords, item.m_Snippets.m_Prefix) < 0 && IsId(item.m_Snippets.m_Prefix)) {
                    sb.AppendFormat("{0}", item.m_Snippets.m_Prefix);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    
        private string m_DslFile = string.Empty;
        private List<DocItemInfo> m_DocInfo = new List<DocItemInfo>();
        private HashSet<string> m_SnippetsKeys = new HashSet<string>();

        internal static string[] Keywords
        {
            get { return s_Keywords; }
            set { s_Keywords = value; }
        }

        internal static string GetKeywordVscodeColorizers()
        {
            StringBuilder sb = new StringBuilder();
            string prefix = "";
            foreach (string s in s_Keywords) {
                sb.AppendFormat("{0}{1}", prefix, s);
                prefix = "|";
            }
            return sb.ToString();
        }
        internal static string GetKeywordUltraEditColorizers()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in s_Keywords) {
                sb.AppendFormat("{0}", s);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static bool IsId(string id)
        {
            return null != id && id.Length > 0 && char.IsLetter(id[0]);
        }
        private static string QuoteString(string code)
        {
            return code.Replace("\"", "\\\"");
        }
        private static string IndentDoc(string indent, string doc)
        {
            string[] lines = doc.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int ix = 0; ix < lines.Length; ++ix) {
                lines[ix] = string.Format("{0}{1}", indent, lines[ix]);
            }
            return string.Join("\r\n", lines);
        }
        private static string IndentCode(string indent, string code)
        {
            string[] lines = code.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int ix = 0; ix < lines.Length; ++ix) {
                string postfix = string.Empty;
                if (ix < lines.Length - 1) {
                    postfix = ",";
                }
                lines[ix] = string.Format("{0}\"{1}\"{2}", indent, lines[ix], postfix);
            }
            return string.Join("\r\n", lines);
        }
        private static string RemoveFirstAndLastEmptyLine(string str)
        {
            if (string.IsNullOrEmpty(str)) {
                return string.Empty;
            }
            int start = 0;
            while (start < str.Length && IsWhiteSpace(str[start]) && str[start] != '\n')
                ++start;
            if (str[start] == '\n')
                ++start;
            int end = str.Length - 1;
            for (int i = 0; i < 2; ++i) {
                while (end > 0 && IsWhiteSpace(str[end]) && str[end] != '\n')
                    --end;
                if (end > 0 && str[end] == '\n')
                    --end;
                if (end > 0 && str[end] == '\r')
                    --end;
            }
            if (end >= start)
                return str.Substring(start, end - start + 1);
            else
                return string.Empty;
        }
        private static bool IsWhiteSpace(char c)
        {
            if (0 == c)
                return false;
            else
                return c_WhiteSpaces.IndexOf(c) >= 0;
        }
        
        private static string[] s_Keywords = null;
        
        private const string c_WhiteSpaces = " \t\r\n";
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            LogSystem.OnOutput = (Log_Type type, string msg) => {
                Console.WriteLine("{0}", msg);
            };
            if (args.Length == 1 && args[0] == "conv") {
                ConvertToUtf8(".");
            } else {
                DocDslParser.Keywords = File.ReadAllLines("keywords.dat");
                GenStoryDoc();
                GenVscodeSnippets();
                GenVscodeColorizers();
                GenUltraEditColorizers();
            }
        }
        private static void GenStoryDoc()
        {
            DocDslParser parser = new DocDslParser();
            parser.Init("storydoc.dsl");
            parser.GenDocAndSnippets("StoryDsl.txt");
            parser.GenVscodeSnippets("story.json", "(story)");
        }
        private static void GenVscodeSnippets()
        {
            DocDslParser parser = new DocDslParser();
            parser.Init("snippets.dsl");
            parser.GenCompositeDoc("dsl.json");
        }
        private static void GenVscodeColorizers()
        {
            using (StreamWriter sw = new StreamWriter("keywords.xml")) {
                sw.Write("\t\t\t\t{0}{1}{2}", "<string>(?&lt;!\\.)\\b(", DocDslParser.GetKeywordVscodeColorizers(), ")\\b</string>");
            }
            using (StreamWriter sw = new StreamWriter("ids.xml")) {
                DocDslParser parser1 = new DocDslParser();
                parser1.Init("storydoc.dsl");
                sw.Write("\t\t\t\t{0}{1}{2}", "<string>(?&lt;!\\.)\\b(", parser1.GetIdVscodeColorizers(), ")\\b</string>");
            }
            DocDslParser parser = new DocDslParser();
            parser.Init("textmate.dsl");
            parser.GenCompositeDoc("dsl.tmLanguage");
        }
        private static void GenUltraEditColorizers()
        {
            using (StreamWriter sw = new StreamWriter("uew.txt")) {
                sw.WriteLine("{0}","/C7\"Keywords\" STYLE_KEYWORD");
                sw.WriteLine(";");
                sw.WriteLine("=");
                sw.WriteLine("{0}", DocDslParser.GetKeywordUltraEditColorizers());
                sw.WriteLine("{0}", "/C8\"Functions、Commands and Triggers\" STYLE_FUNCTION");
                DocDslParser parser1 = new DocDslParser();
                parser1.Init("storydoc.dsl");
                sw.Write("{0}", parser1.GetIdUltraEditColorizers());
            }
            DocDslParser parser = new DocDslParser();
            parser.Init("uew.dsl");
            parser.GenCompositeDoc("dsl.uew");
            ConvertToChinese("dsl.uew", "dsl.uew");
        }

        private static void ConvertToUtf8(string dir)
        {
            // 子文件夹
            foreach (string sub in Directory.GetDirectories(dir)) {
                ConvertToUtf8(sub);
            }
            // 文件
            foreach (string file in Directory.GetFiles(dir, "*.dsl")) {
                ConvertToUtf8(file, file);
                Console.WriteLine("gb2312->utf8 {0}", file);
            }
            foreach (string file in Directory.GetFiles(dir, "*.txt")) {
                ConvertToUtf8(file, file);
                Console.WriteLine("gb2312->utf8 {0}", file);
            }
        }
        private static void ConvertToUtf8(string file, string destfile)
        {
            File.WriteAllText(destfile, File.ReadAllText(file, s_ChineseEncoding), Encoding.UTF8);
        }
        private static void ConvertToChinese(string file, string destfile)
        {
            File.WriteAllText(destfile, File.ReadAllText(file, Encoding.UTF8), s_ChineseEncoding);
        }

        private static Encoding s_ChineseEncoding = Encoding.GetEncoding(936);
    }
}
