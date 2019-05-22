using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GameLibrary;
namespace StorySystem
{
    public static class CustomCommandValueParser
    {
        public static Dsl.DslFile LoadStory(string file)
        {
            if (!string.IsNullOrEmpty(file)) {
                Dsl.DslFile dataFile = new Dsl.DslFile();
                var bytes = new byte[Dsl.DslFile.c_BinaryIdentity.Length];
                using (var fs = File.OpenRead(file)) {
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                }
                var id = System.Text.Encoding.ASCII.GetString(bytes);
                if (id == Dsl.DslFile.c_BinaryIdentity) {
                    try {
                        dataFile.LoadBinaryFile(file);
                        return dataFile;
                    } catch {
                    }
                } else {
                    try {
                        if (dataFile.Load(file, LogSystem.Log)) {
                            return dataFile;
                        } else {
                            LogSystem.Error("LoadStory file:{0} failed", file);
                        }
                    } catch (Exception ex) {
                        LogSystem.Error("LoadStory file:{0} Exception:{1}\n{2}", file, ex.Message, ex.StackTrace);
                    }
                }
            }
            return null;
        }
        public static Dsl.DslFile LoadStoryText(string file, byte[] bytes)
        {
            if (Dsl.DslFile.IsBinaryDsl(bytes, 0)) {
                try {
                    Dsl.DslFile dataFile = new Dsl.DslFile();
                    dataFile.LoadBinaryCode(bytes);
                    return dataFile;
                } catch {
                    return null;
                }
            } else {
                string text = Converter.FileContent2Utf8String(bytes);
                try {
                    Dsl.DslFile dataFile = new Dsl.DslFile();
                    if (dataFile.LoadFromString(text, file, LogSystem.Log)) {
                        return dataFile;
                    } else {
                        LogSystem.Error("LoadStoryText text:{0} failed", file);
                    }
                } catch (Exception ex) {
                    LogSystem.Error("LoadStoryText text:{0} Exception:{1}\n{2}", text, ex.Message, ex.StackTrace);
                }
                return null;
            }
        }
        public static void FirstParse(params Dsl.DslFile[] dataFiles)
        {
            for (int ix = 0; ix < dataFiles.Length; ++ix) {
                Dsl.DslFile dataFile = dataFiles[ix];
                FirstParse(dataFile.DslInfos);
            }
        }
        public static void FinalParse(params Dsl.DslFile[] dataFiles)
        {
            for (int ix = 0; ix < dataFiles.Length; ++ix) {
                Dsl.DslFile dataFile = dataFiles[ix];
                FinalParse(dataFile.DslInfos);
            }
        }
        public static void FirstParse(IList<Dsl.DslInfo> dslInfos)
        {
            for (int i = 0; i < dslInfos.Count; i++) {
                Dsl.DslInfo dslInfo = dslInfos[i];
                FirstParse(dslInfo);
            }
        }
        public static void FinalParse(IList<Dsl.DslInfo> dslInfos)
        {
            for (int i = 0; i < dslInfos.Count; i++) {
                Dsl.DslInfo dslInfo = dslInfos[i];
                FinalParse(dslInfo);
            }
        }
        public static void FirstParse(Dsl.DslInfo dslInfo)
        {            
            string id = dslInfo.GetId();
            if (id == "command") {
                if (dslInfo.Functions.Count == 2) {
                    StorySystem.CommonCommands.CompositeCommand cmd = new CommonCommands.CompositeCommand();
                    cmd.InitSharedData();
                    Dsl.FunctionData first = dslInfo.First;
                    cmd.Name = first.Call.GetParamId(0);
                    Dsl.FunctionData second = dslInfo.Second;
                    for (int ix = 0; ix < second.Call.GetParamNum(); ++ix) {
                        cmd.ArgNames.Add(second.Call.GetParamId(ix));
                    }
                    //注册
                    StoryCommandManager.Instance.RegisterCommandFactory(cmd.Name, new CommonCommands.CompositeCommandFactory(cmd), true);
                }
            } else if (id == "value") {
                if (dslInfo.Functions.Count == 3) {
                    StorySystem.CommonValues.CompositeValue val = new CommonValues.CompositeValue();
                    val.InitSharedData();
                    Dsl.FunctionData first = dslInfo.First;
                    val.Name = first.Call.GetParamId(0);
                    Dsl.FunctionData second = dslInfo.Second;
                    for (int ix = 0; ix < second.Call.GetParamNum(); ++ix) {
                        val.ArgNames.Add(second.Call.GetParamId(ix));
                    }
                    Dsl.FunctionData third = dslInfo.Functions[2];
                    val.ReturnName = third.Call.GetParamId(0);
                    //注册
                    StoryValueManager.Instance.RegisterValueFactory(val.Name, new CommonValues.CompositeValueFactory(val), true);
                }
            }
        }
        public static void FinalParse(Dsl.DslInfo dslInfo)
        {
            string id = dslInfo.GetId();
            if (id == "command") {
                if (dslInfo.Functions.Count == 2) {
                        
                    Dsl.FunctionData first = dslInfo.First;
                    string name = first.Call.GetParamId(0);
                    IStoryCommandFactory factory = StoryCommandManager.Instance.FindFactory(name);
                    if (null != factory) {
                        StorySystem.CommonCommands.CompositeCommand cmd = factory.Create() as StorySystem.CommonCommands.CompositeCommand;
                        Dsl.FunctionData second = dslInfo.Second;
                        cmd.InitialCommands.Clear();
                        for (int ix = 0; ix < second.GetStatementNum(); ++ix) {
                            Dsl.ISyntaxComponent syntaxComp = second.GetStatement(ix);
                            IStoryCommand sub = StoryCommandManager.Instance.CreateCommand(syntaxComp);
                            cmd.InitialCommands.Add(sub);
                        }
                    } else {
                        LogSystem.Error("Can't find command factory '{0}'", name);
                    }
                }
            } else if (id == "value") {
                if (dslInfo.Functions.Count == 3) {
                    Dsl.FunctionData first = dslInfo.First;
                    string name = first.Call.GetParamId(0);
                    IStoryValueFactory factory = StoryValueManager.Instance.FindFactory(name);
                    if (null != factory) {
                        StorySystem.CommonValues.CompositeValue val = factory.Build() as StorySystem.CommonValues.CompositeValue;
                        Dsl.FunctionData second = dslInfo.Second;
                        Dsl.FunctionData third = dslInfo.Functions[2];
                        val.InitialCommands.Clear();
                        for (int ix = 0; ix < third.GetStatementNum(); ++ix) {
                            Dsl.ISyntaxComponent syntaxComp = third.GetStatement(ix);
                            IStoryCommand sub = StoryCommandManager.Instance.CreateCommand(syntaxComp);
                            val.InitialCommands.Add(sub);
                        }
                    } else {
                        LogSystem.Error("Can't find value factory '{0}'", name);
                    }
                }
            }
        }       
    }
}
