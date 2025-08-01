﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
namespace StoryScript.CommonCommands
{
    /// <summary>
    /// dotnetcall(obj,method,arg1,arg2,...);
    /// </summary>
    public sealed class DotnetCallCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            DotnetCallCommand cmd = new DotnetCallCommand();
            cmd.m_Object = m_Object.Clone();
            cmd.m_Method = m_Method.Clone();
            for (int i = 0; i < m_Args.Count; i++) {
                cmd.m_Args.Add(m_Args[i].Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Method.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; i++) {
                m_Args[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            object obj = m_Object.Value.GetObject();
            var disp = obj as IObjectDispatch;
            BoxedValueList dispArgs = null;
            ArrayList arglist = null;
            var bvMethod = m_Method.Value;
            string method = bvMethod.IsString ? bvMethod.StringVal : null;
            if (null != disp) {
                dispArgs = instance.NewBoxedValueList();
                for (int i = 0; i < m_Args.Count; i++) {
                    arglist.Add(m_Args[i].Value);
                }
            }
            else {
                arglist = new ArrayList();
                for (int i = 0; i < m_Args.Count; i++) {
                    arglist.Add(m_Args[i].Value.GetObject());
                }
            }
            if (null != obj) {
                if (null != method) {
                    if (null != disp) {
                        if (m_DispId < 0) {
                            m_DispId = disp.GetDispatchId(method);
                        }
                        if (m_DispId >= 0) {
                            disp.InvokeMethod(m_DispId, dispArgs);
                        }
                        instance.RecycleBoxedValueList(dispArgs);
                    }
                    else {
                        object[] args = arglist.ToArray();
                        IDictionary dict = obj as IDictionary;
                        if (null != dict && dict is Dictionary<BoxedValue, BoxedValue> bvDict && bvDict.TryGetValue(bvMethod, out var val) && val.IsObject && val.ObjectVal is Delegate) {
                            var d = val.ObjectVal as Delegate;
                            if (null != d) {
                                d.DynamicInvoke(args);
                            }
                        }
                        else if (null != dict && dict.Contains(method) && dict[method] is Delegate) {
                            var d = dict[method] as Delegate;
                            if (null != d) {
                                d.DynamicInvoke(args);
                            }
                        }
                        else {
                            Type t = obj as Type;
                            if (null != t) {
                                try {
                                    BindingFlags flags = BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
                                    Converter.CastArgsForCall(t, method, flags, args);
                                    t.InvokeMember(method, flags, null, null, args);
                                }
                                catch (Exception ex) {
                                    LogSystem.Warn("DotnetCall {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                                }
                            }
                            else {
                                t = obj.GetType();
                                if (null != t) {
                                    try {
                                        BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
                                        Converter.CastArgsForCall(t, method, flags, args);
                                        t.InvokeMember(method, flags, null, obj, args);
                                    }
                                    catch (Exception ex) {
                                        LogSystem.Warn("DotnetCall {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Method.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryFunction val = new StoryFunction();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryFunction m_Object = new StoryFunction();
        private IStoryFunction m_Method = new StoryFunction();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
        private int m_DispId = -1;
    }
    /// <summary>
    /// dotnetset(obj,method,arg1,arg2,...);
    /// </summary>
    public sealed class DotnetSetCommand : AbstractStoryCommand
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
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Method.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; i++) {
                m_Args[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            object obj = m_Object.Value.GetObject();
            var disp = obj as IObjectDispatch;
            BoxedValue argv = BoxedValue.NullObject;
            var bvMethod = m_Method.Value;
            string method = bvMethod.IsString ? bvMethod.StringVal : null;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; i++) {
                if (null != disp && i == 0)
                    argv = m_Args[i].Value;
                arglist.Add(m_Args[i].Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (null != obj) {
                if (null != method) {
                    if (null != disp) {
                        if (m_DispId < 0) {
                            m_DispId = disp.GetDispatchId(method);
                        }
                        if (m_DispId >= 0) {
                            disp.SetProperty(m_DispId, argv);
                        }
                    }
                    else {
                        IDictionary dict = obj as IDictionary;
                        if (null != dict && null == obj.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic)) {
                            if (null != dict && dict is Dictionary<BoxedValue, BoxedValue> bvDict) {
                                bvDict[bvMethod] = BoxedValue.FromObject(args[0]);
                            }
                            else {
                                dict[method] = args[0];
                            }
                        }
                        else {
                            Type t = obj as Type;
                            if (null != t) {
                                try {
                                    BindingFlags flags = BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
                                    Converter.CastArgsForSet(t, method, flags, args);
                                    t.InvokeMember(method, flags, null, null, args);
                                }
                                catch (Exception ex) {
                                    LogSystem.Warn("DotnetSet {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                                }
                            }
                            else {
                                t = obj.GetType();
                                if (null != t) {
                                    try {
                                        BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
                                        Converter.CastArgsForSet(t, method, flags, args);
                                        t.InvokeMember(method, flags, null, obj, args);
                                    }
                                    catch (Exception ex) {
                                        LogSystem.Warn("DotnetSet {0}.{1} Exception:{2}\n{3}", t.Name, method, ex.Message, ex.StackTrace);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Method.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryFunction val = new StoryFunction();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryFunction m_Object = new StoryFunction();
        private IStoryFunction m_Method = new StoryFunction();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
        private int m_DispId = -1;
    }
    /// <summary>
    /// collectionexec(obj,method,arg1,arg2,...);
    /// </summary>
    public sealed class CollectionCallCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            CollectionCallCommand cmd = new CollectionCallCommand();
            cmd.m_Object = m_Object.Clone();
            cmd.m_Method = m_Method.Clone();
            for (int i = 0; i < m_Args.Count; i++) {
                cmd.m_Args.Add(m_Args[i].Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Method.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; i++) {
                m_Args[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            object obj = m_Object.Value.GetObject();
            var bvMethod = m_Method.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; i++) {
                arglist.Add(m_Args[i].Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (null != obj) {
                IDictionary dict = obj as IDictionary;
                var mobj = bvMethod.GetObject();
                if (null != dict && dict is Dictionary<BoxedValue, BoxedValue> bvDict && bvDict.TryGetValue(bvMethod, out var val)) {
                    var d = val.As<Delegate>();
                    if (null != d) {
                        d.DynamicInvoke(args);
                    }
                }
                else if (null != dict && dict.Contains(mobj)) {
                    var d = dict[mobj] as Delegate;
                    if (null != d) {
                        d.DynamicInvoke(args);
                    }
                }
                else {
                    IEnumerable enumer = obj as IEnumerable;
                    if (null != enumer && bvMethod.IsInteger) {
                        int index = bvMethod.GetInt();
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
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Method.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryFunction val = new StoryFunction();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryFunction m_Object = new StoryFunction();
        private IStoryFunction m_Method = new StoryFunction();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
    }
    /// <summary>
    /// collectionset(obj,method,arg1,arg2,...);
    /// </summary>
    public sealed class CollectionSetCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            CollectionSetCommand cmd = new CollectionSetCommand();
            cmd.m_Object = m_Object.Clone();
            cmd.m_Method = m_Method.Clone();
            for (int i = 0; i < m_Args.Count; i++) {
                cmd.m_Args.Add(m_Args[i].Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Method.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; i++) {
                m_Args[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            object obj = m_Object.Value.GetObject();
            var bvMethod = m_Method.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; i++) {
                arglist.Add(m_Args[i].Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (null != obj) {
                IDictionary dict = obj as IDictionary;
                var mobj = bvMethod.GetObject();
                if (null != dict && dict is Dictionary<BoxedValue, BoxedValue> bvDict) {
                    bvDict[bvMethod] = BoxedValue.FromObject(args[0]);
                }
                else if (null != dict) {
                    dict[mobj] = args[0];
                }
                else {
                    IList list = obj as IList;
                    if (null != list && bvMethod.IsInteger) {
                        int index = bvMethod.GetInt();
                        if (index >= 0 && index < list.Count) {
                            list[index] = args[0];
                        }
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Method.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryFunction val = new StoryFunction();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryFunction m_Object = new StoryFunction();
        private IStoryFunction m_Method = new StoryFunction();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
    }
}
