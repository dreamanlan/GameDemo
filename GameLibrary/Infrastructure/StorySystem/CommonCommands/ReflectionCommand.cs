﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
namespace StorySystem.CommonCommands
{
    /// <summary>
    /// dotnetexec(obj,method,arg1,arg2,...);
    /// </summary>
    internal sealed class DotnetExecCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            DotnetExecCommand cmd = new DotnetExecCommand();
            cmd.m_Object = m_Object.Clone();
            cmd.m_Method = m_Method.Clone();
            for (int i = 0; i < m_Args.Count; i++) {
                cmd.m_Args.Add(m_Args[i].Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Method.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; i++) {
                m_Args[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            object obj = m_Object.Value;
            object methodObj = m_Method.Value;
            string method = methodObj as string;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; i++) {
                arglist.Add(m_Args[i].Value);
            }
            object[] args = arglist.ToArray();
            if (null != obj) {
                if (null != method) {
                    IDictionary dict = obj as IDictionary;
                    if (null != dict && dict.Contains(method) && dict[method] is Delegate) {
                        var d = dict[method] as Delegate;
                        if (null != d) {
                            d.DynamicInvoke(args);
                        }
                    } else {
                        Type t = obj as Type;
                        if (null != t) {
                            try {
                                BindingFlags flags = BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;
                                GameLibrary.Converter.CastArgsForCall(t, method, flags, args);
                                t.InvokeMember(method, flags, null, null, args);
                            } catch (Exception ex) {
                                GameLibrary.LogSystem.Warn("DotnetExec {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                            }
                        } else {
                            t = obj.GetType();
                            if (null != t) {
                                try {
                                    BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;
                                    GameLibrary.Converter.CastArgsForCall(t, method, flags, args);
                                    t.InvokeMember(method, flags, null, obj, args);
                                } catch (Exception ex) {
                                    GameLibrary.LogSystem.Warn("DotnetExec {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                                }
                            }
                        }
                    }
                } else {
                    IDictionary dict = obj as IDictionary;
                    if (null != dict && dict.Contains(methodObj)) {
                        var d = dict[methodObj] as Delegate;
                        if (null != d) {
                            d.DynamicInvoke(args);
                        }
                    } else {
                        IEnumerable enumer = obj as IEnumerable;
                        if (null != enumer && methodObj is int) {
                            int index = (int)methodObj;
                            var e = enumer.GetEnumerator();
                            for (int i = 0; i <= index; ++i) {
                                e.MoveNext();
                            }
                            var d = e.Current as Delegate;
                            if (null != d) {
                                d.DynamicInvoke(args);
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Method.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
        }
        private IStoryValue m_Object = new StoryValue();
        private IStoryValue m_Method = new StoryValue();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// dotnetset(obj,method,arg1,arg2,...);
    /// </summary>
    internal sealed class DotnetSetCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            DotnetSetCommand cmd = new DotnetSetCommand();
            cmd.m_Object = m_Object.Clone();
            cmd.m_Method = m_Method.Clone();
            for (int i = 0; i < m_Args.Count; i++) {
                cmd.m_Args.Add(m_Args[i].Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Method.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; i++) {
                m_Args[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            object obj = m_Object.Value;
            object methodObj = m_Method.Value;
            string method = methodObj as string;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; i++) {
                arglist.Add(m_Args[i].Value);
            }
            object[] args = arglist.ToArray();
            if (null != obj) {
                if (null != method) {
                    IDictionary dict = obj as IDictionary;
                    if (null != dict && null == obj.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic)) {
                        dict[method] = args[0];
                    } else {
                        Type t = obj as Type;
                        if (null != t) {
                            try {
                                BindingFlags flags = BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic;
                                GameLibrary.Converter.CastArgsForSet(t, method, flags, args);
                                t.InvokeMember(method, flags, null, null, args);
                            } catch (Exception ex) {
                                GameLibrary.LogSystem.Warn("DotnetSet {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                            }
                        } else {
                            t = obj.GetType();
                            if (null != t) {
                                try {
                                    BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic;
                                    GameLibrary.Converter.CastArgsForSet(t, method, flags, args);
                                    t.InvokeMember(method, flags, null, obj, args);
                                } catch (Exception ex) {
                                    GameLibrary.LogSystem.Warn("DotnetSet {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                                }
                            }
                        }
                    }
                } else {
                    IDictionary dict = obj as IDictionary;
                    if (null != dict && dict.Contains(methodObj)) {
                        dict[methodObj] = args[0];
                    } else {
                        IList list = obj as IList;
                        if (null != list && methodObj is int) {
                            int index = (int)methodObj;
                            if (index >= 0 && index < list.Count) {
                                list[index] = args[0];
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Method.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
        }
        private IStoryValue m_Object = new StoryValue();
        private IStoryValue m_Method = new StoryValue();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    /// <summary>
    /// system(file,args);
    /// </summary>
    internal sealed class SystemCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SystemCommand cmd = new SystemCommand();
            cmd.m_FileName = m_FileName.Clone();
            cmd.m_Arguments = m_Arguments.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, object iterator, object[] args)
        {
            m_FileName.Evaluate(instance, handler, iterator, args);
            m_Arguments.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            try {
                Process.Start(m_FileName.Value, m_Arguments.Value);
            } catch (Exception ex) {
                GameLibrary.LogSystem.Warn("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_FileName.InitFromDsl(callData.GetParam(0));
                m_Arguments.InitFromDsl(callData.GetParam(1));
            }
        }
        private IStoryValue<string> m_FileName = new StoryValue<string>();
        private IStoryValue<string> m_Arguments = new StoryValue<string>();
    }
}
