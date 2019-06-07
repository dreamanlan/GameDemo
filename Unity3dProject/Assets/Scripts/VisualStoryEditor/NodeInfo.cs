using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualStoryTool
{
    public interface IParamDrawer
    {
        void SetValue(string val);
        string GetValue();
        bool Inspect();
        bool Update();
    }
    public sealed class ParamInfo
    {
        public string Value
        {
            get {
                if (null == Drawer)
                    return InitValue;
                else
                    return Drawer.GetValue();
            }
            set {
                if (null == Drawer)
                    InitValue = value;
                else
                    Drawer.SetValue(value);
            }
        }

        public string Caption;
        public string InitValue;
        public Dsl.ISyntaxComponent DslInitValue;
        public Dsl.ISyntaxComponent[] SemanticOptions;
        public bool Used;
        public string Name;
        public string Type;
        public bool IsOptional;
        public string Semantic;
        public IParamDrawer Drawer;
        public GameObject SceneObject;

        public ParamInfo Clone()
        {
            var newObj = new ParamInfo();
            newObj.Caption = Caption;
            newObj.InitValue = InitValue;
            newObj.DslInitValue = DslInitValue;
            newObj.Used = Used;
            newObj.Name = Name;
            newObj.Type = Type;
            newObj.IsOptional = IsOptional;
            newObj.Semantic = Semantic;
            newObj.SemanticOptions = SemanticOptions;
            return newObj;
        }
        public void Export(StringBuilder sb)
        {
            if (Type == "string") {
                sb.Append('"');
                sb.Append(Value);
                sb.Append('"');
            } else {
                sb.Append(Value);
            }
        }
        public static string DslToString(Dsl.ISyntaxComponent comp)
        {
            StringBuilder sb = new StringBuilder();
            DslToStringBuilder(comp, sb);
            return sb.ToString();
        }
        public static Dsl.ISyntaxComponent StringToDsl(string val)
        {
            if(s_DslFile.LoadFromString(val, string.Empty, msg => Debug.LogWarningFormat("{0}", msg))) {
                var cd = s_DslFile.DslInfos[0].First.Call;
                if (!cd.HaveParam()) {
                    return cd.Name;
                } else {
                    return cd;
                }
            }
            return null;
        }
        private static void DslToStringBuilder(Dsl.ISyntaxComponent comp, StringBuilder sb)
        {
            var vd = comp as Dsl.ValueData;
            if (null != vd) {
                sb.Append(vd.GetId());
            } else {
                var cd = comp as Dsl.CallData;
                if (null != cd) {
                    sb.Append(cd.GetId());
                    sb.Append("(");
                    for(int i = 0; i < cd.GetParamNum(); ++i) {
                        if (i > 0)
                            sb.Append(",");
                        var pcomp = cd.GetParam(i);
                        DslToStringBuilder(pcomp, sb);
                    }
                    sb.Append(")");
                } else {
                    Debug.LogError("[Error] Param must be Value or Call !");
                }
            }
        }
        private static Dsl.DslFile s_DslFile = new Dsl.DslFile();
    }
    public sealed class CommandTupleInfo
    {
        public string Caption;
        public string Name;
        public ParamInfo[] Params;
        public bool Used;
        public int OptionalParamStart;
        public bool NoParams;
        public bool IsOptional;

        public CommandTupleInfo Clone()
        {
            var newObj = new CommandTupleInfo();
            newObj.Name = Name;
            newObj.Caption = Caption;
            newObj.Params = new ParamInfo[Params.Length];
            for (int i = 0; i < Params.Length; ++i) {
                newObj.Params[i] = Params[i].Clone();
            }
            newObj.Used = Used;
            newObj.OptionalParamStart = OptionalParamStart;
            newObj.NoParams = NoParams;
            newObj.IsOptional = IsOptional;
            return newObj;
        }
        public void Export(StringBuilder sb)
        {
            sb.Append(Name);
            if (!NoParams) {
                sb.Append('(');
                for (int i = 0; i < OptionalParamStart && i < Params.Length; ++i) {
                    if (i > 0)
                        sb.Append(',');
                    Params[i].Export(sb);
                }
                sb.Append(')');
            }
            if (OptionalParamStart < Params.Length) {
                sb.AppendLine(" {");
                for (int i = OptionalParamStart; i < Params.Length; ++i) {
                    var param = Params[i];
                    if (param.Used) {
                        sb.Append("\t\t\t");
                        sb.Append(param.Name);
                        sb.Append("(");
                        sb.Append(param.Value);
                        sb.AppendLine(");");
                    }
                }
                sb.Append("\t\t}");
            }
        }
    }
    public sealed class CommandInfo
    {
        public string Caption;
        public string Name;
        public CommandTupleInfo[] Tuples;
        public List<GameObject> SceneObjects;

        public CommandInfo Clone()
        {
            var newObj = new CommandInfo();
            newObj.Caption = Caption;
            newObj.Name = Name;
            newObj.Tuples = new CommandTupleInfo[Tuples.Length];
            for (int i = 0; i < Tuples.Length; ++i) {
                newObj.Tuples[i] = Tuples[i].Clone();
            }
            return newObj;
        }
        public void Export(StringBuilder sb)
        {
            sb.Append("\t\t");
            bool first = true;
            foreach(var tuple in Tuples) {
                if (tuple.Used) {
                    if (!first)
                        sb.Append(" ");
                    tuple.Export(sb);
                    first = false;
                }
            }
            sb.AppendLine(";");
        }
    }
    public sealed class OutputInfo
    {
        public CommandInfo Command;
        public ParamInfo Param;
        public NodeInfo Node;
    }
    [Serializable]
    public sealed class NodeInfo
    {
        public string Message = string.Empty;
        [NonSerialized]
        public List<CommandInfo> Commands = new List<CommandInfo>();
        [NonSerialized]
        public List<OutputInfo> Outputs = new List<OutputInfo>();
        [NonSerialized]
        public string[] CommandCaptions;
        [NonSerialized]
        public int SelectedIndex;
        [NonSerialized]
        public Queue<Action> DeferredActions = new Queue<Action>();
        [NonSerialized]
        public NodeContainer Container;

        public NodeInfo Clone()
        {
            var newObj = new NodeInfo();
            newObj.Message = Message;
            for(int i = 0; i < Commands.Count; ++i) {
                newObj.Commands.Add(Commands[i].Clone());
            }
            CommandCaptions = null;
            SelectedIndex = 0;
            return newObj;
        }
        public void RefreshOutputs(IList<NodeContainer> nodeContainers)
        {
            var list = Outputs;
            list.Clear();
            foreach(var cmd in Commands) {
                foreach (var tuple in cmd.Tuples) {
                    foreach (var param in tuple.Params) {
                        if (param.Semantic == "message") {
                            bool find = false;
                            foreach (var c in nodeContainers) {
                                if (c.Node.Message == param.Value) {
                                    find = true;
                                    list.Add(new OutputInfo { Command = cmd, Param = param, Node = c.Node });
                                }
                            }
                            if (!find) {
                                list.Add(new OutputInfo { Command = cmd, Param = param, Node = null });
                            }
                        }
                    }
                }
            }
        }
        public void Export(StringBuilder sb)
        {
            sb.Append("\tonmessage(\"");
            sb.Append(Message);
            sb.AppendLine("\")");
            sb.AppendLine("\t{");
            for (int i = 0; i < Commands.Count; ++i) {
                Commands[i].Export(sb);
            }
            sb.AppendLine("\t};");
        }
    }

    public sealed class ApiManager
    {
        public string[] Captions
        {
            get {
                return m_CaptionArray;
            }
        }
        public void Init(string apiFile)
        {
            Dsl.DslFile apiDefines = new Dsl.DslFile();
            if(apiDefines.Load(apiFile, msg => Debug.LogWarningFormat("{0}", msg))) {
                Clear();
                foreach(var dslInfo in apiDefines.DslInfos) {
                    var cmdInfo = new CommandInfo();
                    var tuples = new List<CommandTupleInfo>();
                    foreach (var funcData in dslInfo.Functions) {
                        var id = funcData.GetId();
                        if (id == "defapi" || id == "tuple") {
                            var callData = funcData.Call;

                            var name = callData.GetParamId(0);
                            var caption = callData.GetParamId(1);

                            var tuple = new CommandTupleInfo();
                            tuple.Name = name;
                            tuple.Caption = caption;

                            if (id == "defapi") {
                                cmdInfo.Name = name;
                                cmdInfo.Caption = caption;
                                tuple.NoParams = false;
                                tuple.IsOptional = false;
                            } else {
                                tuple.NoParams = false;
                                tuple.IsOptional = true;
                            }

                            for(int ix = 2; ix < callData.GetParamNum(); ++ix) {
                                var param = callData.GetParam(ix) as Dsl.CallData;
                                if (null != param) {
                                    var key = param.GetId();
                                    var val = param.GetParamId(0);
                                    if (key == "noparams") {
                                        tuple.NoParams = val == "true";
                                    } else if (key == "optional") {
                                        tuple.IsOptional = val == "true";
                                    }
                                }
                            }

                            if (tuple.IsOptional)
                                tuple.Used = false;
                            else
                                tuple.Used = true;
                            
                            var list = new List<ParamInfo>();
                            var slist = new List<ParamInfo>();
                            foreach (var comp in funcData.Statements) {
                                var cd = comp as Dsl.CallData;
                                if (null != cd) {
                                    var pname = cd.GetId();
                                    var ptype = cd.GetParamId(0);
                                    var pcaption = cd.GetParamId(1);
                                    var pi = new ParamInfo();
                                    pi.Name = pname;
                                    pi.Type = ptype;
                                    pi.Caption = pcaption;
                                    list.Add(pi);

                                    if (pi.IsOptional)
                                        pi.Used = false;
                                    else
                                        pi.Used = true;

                                    if ((pi.Type == "int" || pi.Type == "float") && string.IsNullOrEmpty(pi.InitValue)) {
                                        pi.DslInitValue = ParamInfo.StringToDsl("0");
                                        pi.InitValue = "0";
                                    }
                                } else {
                                    var fd = comp as Dsl.FunctionData;
                                    if (null != fd) {
                                        cd = fd.Call;
                                        var pname = cd.GetId();
                                        var ptype = cd.GetParamId(0);
                                        var pcaption = cd.GetParamId(1);

                                        var pi = new ParamInfo();
                                        pi.Name = pname;
                                        pi.Type = ptype;
                                        pi.Caption = pcaption;

                                        var opts = new List<Dsl.ISyntaxComponent>();
                                        foreach (var pcomp in fd.Statements) {
                                            var pcd = pcomp as Dsl.CallData;
                                            if (null != pcd) {
                                                var key = pcd.GetId();
                                                var val = pcd.GetParam(0);

                                                if (key == "semantic") {
                                                    pi.Semantic = val.GetId();
                                                } else if (key == "optional") {
                                                    pi.IsOptional = val.GetId() == "true";
                                                } else if (key == "initval") {
                                                    pi.DslInitValue = val;
                                                    pi.InitValue = ParamInfo.DslToString(val);
                                                } else {
                                                    opts.Add(pcomp);
                                                }
                                            } else {
                                                opts.Add(pcomp);
                                            }
                                        }
                                        pi.SemanticOptions = opts.ToArray();

                                        if (pi.IsOptional)
                                            pi.Used = false;
                                        else
                                            pi.Used = true;

                                        if ((pi.Type == "int" || pi.Type == "float") && string.IsNullOrEmpty(pi.InitValue)) {
                                            pi.DslInitValue = ParamInfo.StringToDsl("0");
                                            pi.InitValue = "0";
                                        }

                                        if (pi.IsOptional) {
                                            slist.Add(pi);
                                        } else {
                                            list.Add(pi);
                                        }
                                    }
                                }
                            }
                            tuple.OptionalParamStart = list.Count;
                            list.AddRange(slist);
                            tuple.Params = list.ToArray();
                            tuples.Add(tuple);
                        }
                    }
                    cmdInfo.Tuples = tuples.ToArray();
                    AddApi(cmdInfo);
                }
                m_CaptionArray = m_Captions.ToArray();
            }
        }
        public CommandInfo NewCommandInfo(string caption)
        {
            CommandInfo cmdInfo;
            if(m_Apis.TryGetValue(caption, out cmdInfo)) {
                return cmdInfo.Clone();
            }
            return null;
        }
        public CommandInfo NewCommandInfoByName(string name)
        {
            CommandInfo cmdInfo;
            if (m_ApiByNames.TryGetValue(name, out cmdInfo)) {
                return cmdInfo.Clone();
            }
            return null;
        }
        private void Clear()
        {
            m_Apis.Clear();
            m_ApiByNames.Clear();
            m_Captions.Clear();
            m_CaptionArray = null;
        }
        private void AddApi(CommandInfo cmdInfo)
        {
            var caption = cmdInfo.Caption;
            if (!m_Apis.ContainsKey(caption)) {
                m_Apis.Add(caption, cmdInfo);
                m_Captions.Add(caption);
            }
            var name = cmdInfo.Name;
            if (!m_ApiByNames.ContainsKey(name)) {
                m_ApiByNames.Add(name, cmdInfo);
            }
        }

        private Dictionary<string, CommandInfo> m_Apis = new Dictionary<string, CommandInfo>();
        private Dictionary<string, CommandInfo> m_ApiByNames = new Dictionary<string, CommandInfo>();
        private List<string> m_Captions = new List<string>();
        private string[] m_CaptionArray = null;
    }
}

