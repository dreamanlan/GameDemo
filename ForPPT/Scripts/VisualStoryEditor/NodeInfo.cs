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
        public bool IsCamera;
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
            if (!string.IsNullOrEmpty(Value) && InitValue != Value) {
                newObj.InitValue = Value;
                newObj.DslInitValue = StringToDsl(Value);
            } else {
                newObj.InitValue = InitValue;
                newObj.DslInitValue = DslInitValue;
            }
            newObj.IsCamera = IsCamera;
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
            DslToStringBuilder(comp, sb, true, false);
            return sb.ToString();
        }
        public static Dsl.ISyntaxComponent StringToDsl(string val)
        {
            if(s_DslFile.LoadFromString(val, string.Empty, msg => Debug.LogWarningFormat("{0}", msg))) {
                var vd = s_DslFile.DslInfos[0] as Dsl.ValueData;
                var cd = s_DslFile.DslInfos[0] as Dsl.FunctionData;
                if (null != vd) {
                    return vd;
                } else {
                    return cd;
                }
            }
            return null;
        }
        private static void DslToStringBuilder(Dsl.ISyntaxComponent comp, StringBuilder sb, bool toplevel, bool inOperator)
        {
            var vd = comp as Dsl.ValueData;
            if (null != vd) {
                if (vd.GetIdType() == Dsl.ValueData.STRING_TOKEN && !toplevel) {
                    sb.Append('\'');
                    sb.Append(vd.GetId());
                    sb.Append('\'');
                } else {
                    sb.Append(vd.GetId());
                }
            } else {
                var cd = comp as Dsl.FunctionData;
                if (null != cd) {
                    if (cd.HaveParam()) {
                        var id = cd.GetId();
                        int paramClass = cd.GetParamClass();
                        int num = cd.GetParamNum();
                        if (paramClass == (int)Dsl.FunctionData.ParamClassEnum.PARAM_CLASS_OPERATOR && num >= 1 && num <= 2) {
                            if (!string.IsNullOrEmpty(id) && !toplevel && inOperator)
                                sb.Append("(");
                            if (num == 1) {
                                sb.Append(id);
                                var pcomp = cd.GetParam(0);
                                DslToStringBuilder(pcomp, sb, false, true);
                            }
                            else if (num == 2) {
                                var pcomp1 = cd.GetParam(0);
                                DslToStringBuilder(pcomp1, sb, false, true);
                                sb.Append(id);
                                var pcomp2 = cd.GetParam(1);
                                DslToStringBuilder(pcomp2, sb, false, true);
                            }
                            if (!string.IsNullOrEmpty(id) && !toplevel && inOperator)
                                sb.Append(")");
                        }
                        else {
                            sb.Append(id);
                            switch (paramClass) {
                                case (int)Dsl.FunctionData.ParamClassEnum.PARAM_CLASS_PARENTHESIS:
                                    sb.Append("(");
                                    break;
                                case (int)Dsl.FunctionData.ParamClassEnum.PARAM_CLASS_BRACKET:
                                    sb.Append("[");
                                    break;
                            }
                            for (int i = 0; i < cd.GetParamNum(); ++i) {
                                if (i > 0)
                                    sb.Append(",");
                                var pcomp = cd.GetParam(i);
                                DslToStringBuilder(pcomp, sb, false, false);
                            }
                            switch (paramClass) {
                                case (int)Dsl.FunctionData.ParamClassEnum.PARAM_CLASS_PARENTHESIS:
                                    sb.Append(")");
                                    break;
                                case (int)Dsl.FunctionData.ParamClassEnum.PARAM_CLASS_BRACKET:
                                    sb.Append("]");
                                    break;
                            }
                        }
                    }
                    else if(cd.HaveStatement()) {
                        var fd = cd;
                        DslToStringBuilder(fd.LowerOrderFunction, sb, toplevel, false);
                        sb.Append("{");
                        for (int i = 0; i < fd.GetParamNum(); ++i) {
                            if (i > 0)
                                sb.Append(",");
                            var fcomp = fd.GetParam(i);
                            DslToStringBuilder(fcomp, sb, false, false);
                        }
                        sb.Append("}");
                    }
                    else {
                        Debug.LogError("[Error] Param must be Value or Call !");
                    }
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
        public string Comment;
        public CommandTupleInfo[] Tuples;
        public List<GameObject> SceneObjects;
        public bool Selected;

        public CommandInfo Clone()
        {
            var newObj = new CommandInfo();
            newObj.Caption = Caption;
            newObj.Name = Name;
            newObj.Comment = Comment;
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
            if (!string.IsNullOrEmpty(Comment)) {
                sb.AppendFormat("comment(\"{0}\")", Comment);
            }
            sb.AppendLine(";");
        }
    }
    public sealed class OutputInfo
    {
        public CommandInfo Command;
        public CommandTupleInfo Tuple;
        public ParamInfo Param;
        public NodeInfo Node;
    }
    public sealed class FixedOutputInfo
    {
        public CommandInfo Command;
        public CommandTupleInfo Tuple;
        public ParamInfo Param;
        public IList<string> Messages;
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
        public List<FixedOutputInfo> FixedOutputs = new List<FixedOutputInfo>();
        [NonSerialized]
        public string[] CommandCaptions;
        [NonSerialized]
        public int SelectedIndex;
        [NonSerialized]
        public bool MessageChanged = false;
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
        public void SyncOutputs()
        {
            foreach(var output in Outputs) {
                if (null != output.Node) {
                    output.Param.Value = output.Node.Message;
                }
                else {
                    output.Param.Value = string.Empty;
                }
            }
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
                                    list.Add(new OutputInfo { Command = cmd, Tuple = tuple, Param = param, Node = c.Node });
                                    break;
                                }
                            }
                            if (!find) {
                                list.Add(new OutputInfo { Command = cmd, Tuple = tuple, Param = param, Node = null });
                            }
                        }
                    }
                }
            }
        }
        public void RefreshFixedOutputs(ParamInfo paramInfo, CommandTupleInfo tupleInfo, CommandInfo cmdInfo, IList<string> msgs)
        {
            bool find = false;
            for(int ix = FixedOutputs.Count - 1; ix >= 0; --ix) {
                var output = FixedOutputs[ix];
                if(output.Param == paramInfo && output.Tuple==tupleInfo && output.Command == cmdInfo) {
                    find = true;
                    output.Messages = msgs;
                    break;
                }
            }
            if (!find) {
                FixedOutputs.Add(new FixedOutputInfo { Command = cmdInfo, Tuple = tupleInfo, Param = paramInfo, Messages = msgs });
            }
        }
        public void Export(StringBuilder sb)
        {
            SyncOutputs();
            sb.Append("\tonmessage(\"");
            sb.Append(Message);
            sb.AppendLine("\")");
            if (null != Container) {
                sb.AppendLine("\tcomments");
                sb.AppendLine("\t{");
                var pos = Container.Rect.position;               sb.AppendFormat("\t\tpos({0:f2},{1:f2});", pos.x, pos.y);
                sb.AppendLine();
                sb.AppendLine("\t}");
            }
            sb.AppendLine("\tbody");
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
                    var dslFunction = dslInfo as Dsl.FunctionData;
                    var dslStatement = dslInfo as Dsl.StatementData;
                    var funcs = new List<Dsl.FunctionData>();
                    if (null != dslFunction) {
                        funcs.Add(dslFunction);
                    }
                    else if (null != dslStatement) {
                        funcs.AddRange(dslStatement.Functions);
                    }
                    else {
                        continue;
                    }
                    foreach (var funcData in funcs) {
                        var id = funcData.GetId();
                        if (id == "defapi" || id == "tuple") {
                            var callData = funcData.ThisOrLowerOrderCall;

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
                                var param = callData.GetParam(ix) as Dsl.FunctionData;
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
                            if (funcData.HaveStatement()) {
                                foreach (var comp in funcData.Params) {
                                    var cd = comp as Dsl.FunctionData;
                                    if (null != cd) {
                                        if (cd.HaveParam()) {
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

                                            if (pi.Type == "bool" && string.IsNullOrEmpty(pi.InitValue)) {
                                                pi.DslInitValue = ParamInfo.StringToDsl("false");
                                                pi.InitValue = "false";
                                            }
                                            else if ((pi.Type == "int" || pi.Type == "float") && string.IsNullOrEmpty(pi.InitValue)) {
                                                pi.DslInitValue = ParamInfo.StringToDsl("0");
                                                pi.InitValue = "0";
                                            }
                                            else if (pi.Type == "vector3" && string.IsNullOrEmpty(pi.InitValue)) {
                                                pi.DslInitValue = ParamInfo.StringToDsl("vector3(0,0,0)");
                                                pi.InitValue = "vector3(0,0,0)";
                                            }
                                        }
                                        else if(cd.HaveStatement()) {
                                            var fd = cd;
                                            cd = fd.LowerOrderFunction;
                                            var pname = cd.GetId();
                                            var ptype = cd.GetParamId(0);
                                            var pcaption = cd.GetParamId(1);

                                            var pi = new ParamInfo();
                                            pi.Name = pname;
                                            pi.Type = ptype;
                                            pi.Caption = pcaption;

                                            var opts = new List<Dsl.ISyntaxComponent>();
                                            foreach (var pcomp in fd.Params) {
                                                var pcd = pcomp as Dsl.FunctionData;
                                                if (null != pcd) {
                                                    var key = pcd.GetId();
                                                    var val = pcd.GetParam(0);

                                                    if (key == "semantic") {
                                                        pi.Semantic = val.GetId();
                                                    }
                                                    else if (key == "optional") {
                                                        pi.IsOptional = val.GetId() == "true";
                                                    }
                                                    else if (key == "initval") {
                                                        pi.DslInitValue = val;
                                                        pi.InitValue = ParamInfo.DslToString(val);
                                                    }
                                                    else if (key == "camera") {
                                                        pi.IsCamera = val.GetId() == "true";
                                                    }
                                                    else {
                                                        opts.Add(pcomp);
                                                    }
                                                }
                                                else {
                                                    opts.Add(pcomp);
                                                }
                                            }
                                            pi.SemanticOptions = opts.ToArray();

                                            if (pi.IsOptional)
                                                pi.Used = false;
                                            else
                                                pi.Used = true;

                                            if (pi.Type == "bool" && string.IsNullOrEmpty(pi.InitValue)) {
                                                pi.DslInitValue = ParamInfo.StringToDsl("false");
                                                pi.InitValue = "false";
                                            }
                                            else if ((pi.Type == "int" || pi.Type == "float") && string.IsNullOrEmpty(pi.InitValue)) {
                                                pi.DslInitValue = ParamInfo.StringToDsl("0");
                                                pi.InitValue = "0";
                                            }
                                            else if (pi.Type == "vector3" && string.IsNullOrEmpty(pi.InitValue)) {
                                                pi.DslInitValue = ParamInfo.StringToDsl("vector3(0,0,0)");
                                                pi.InitValue = "vector3(0,0,0)";
                                            }

                                            if (pi.IsOptional) {
                                                slist.Add(pi);
                                            }
                                            else {
                                                list.Add(pi);
                                            }
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

