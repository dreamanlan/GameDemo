using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEditor.SceneManagement;

namespace VisualStoryTool
{
    class VisualStoryEditor : EditorWindow
    {
        [MenuItem("Dsl资源工具/剧情编辑", false, 100)]
        internal static void ShowEditor()
        {
            VisualStoryEditor editor = (VisualStoryEditor)EditorWindow.GetWindow(typeof(VisualStoryEditor));
            editor.Init();
            s_Instance = editor;
        }

        internal static VisualStoryEditor Instance
        {
            get { return s_Instance; }
        }
        private static VisualStoryEditor s_Instance = null;
        
        internal GameObject EditorNodeInScene
        {
            get {
                if (null == m_EditorNodeInScene) {
                    var scene = EditorSceneManager.GetActiveScene();
                    var roots = scene.GetRootGameObjects();
                    foreach (var root in roots) {
                        if (root.name == "VisualStoryEditor") {
                            m_EditorNodeInScene = root;
                            break;
                        }
                    }
                    if (null == m_EditorNodeInScene) {
                        m_EditorNodeInScene = new GameObject("VisualStoryEditor");
                        m_EditorNodeInScene.hideFlags = HideFlags.DontSave;
                        m_EditorNodeInScene.transform.position = Vector3.zero;
                        m_EditorNodeInScene.transform.localRotation = Quaternion.identity;
                        m_EditorNodeInScene.transform.localScale = Vector3.one;
                    }
                }
                return m_EditorNodeInScene;
            }
        }

        internal int GetNextSceneObjID()
        {
            return m_NextSceneObjID++;
        }

        internal void Update()
        {
            if (m_bDragingInputLine || m_bDragingOutputLine) {
                this.Repaint();
            }
            if (null != m_CurSelectedNode) {
                var node = m_CurSelectedNode.Node;
                var selectedIndex = node.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < node.Commands.Count) {
                    var cmd = node.Commands[selectedIndex];
                    bool changed = false;
                    foreach (var tuple in cmd.Tuples) {
                        foreach (var p in tuple.Params) {
                            if (null != p.Drawer) {
                                changed = p.Drawer.Update() || changed;
                            }
                        }
                    }
                    if (changed) {
                        this.Repaint();
                    }
                }
            }
            if (m_DebuggerUpdated) {
                this.Repaint();
                m_DebuggerUpdated = false;
            }
        }

        private void Init()
        {
            minSize = new Vector2(400, 400);
            m_SplitColor = new Texture2D(1, 1);
            m_SplitColor.SetPixel(0, 0, Color.gray);
            m_SplitColor.Apply();

            m_PortColor = new Texture2D(1, 1);
            m_PortColor.SetPixel(0, 0, Color.white);
            m_PortColor.Apply();

            m_SplitRect.Set(position.width - m_ToolWindowWidth, 0, 5, position.height);
            m_MaxRect.Set(-m_WorkspaceWidth / 2, -m_WorkspaceHeight / 2, m_WorkspaceWidth, m_WorkspaceHeight);
            //m_ScrollPosition = new Vector2(m_WorkspaceWidth / 2 - m_SplitRect.x / 2, m_WorkspaceHeight / 2 - position.height / 2);
            m_ScrollPosition = Vector2.zero;

            m_TexPort = new Texture2D(1, 1);
            m_TexPort.SetPixel(0, 0, new Color(1, 1, 1));
            m_TexPort.Apply();
            Repaint();

            m_ApiManager.Init("./VisualStory.dsl");
            Clear();
        }

        private void OnGUI()
        {
            GUI.skin.button.normal.textColor = Color.magenta;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清空")) {
                if (EditorUtility.DisplayDialog(string.Empty, "确定要清空所有编辑内容么？", "是", "否")) {
                    Clear();
                }
            }
            if (GUILayout.Button("打开")) {
                string path = EditorUtility.OpenFilePanel("打开剧情脚本", string.Empty, "dsl");
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    Clear();
                    Import(path);
                }
            }
            if (GUILayout.Button("保存")) {
                string path = EditorUtility.SaveFilePanel("保存剧情脚本", string.Empty, "visual", "dsl");
                if (!string.IsNullOrEmpty(path)) {
                    File.WriteAllText(path, Export());
                }
            }
            if (EditorApplication.isPlaying) {
                if (GUILayout.Button("应用到场景")) {
                    Utility.EventSystem.Publish("gm_resetdsl", "gm");
                    var txt = Export();
                    byte[] text = Encoding.UTF8.GetBytes(txt);
                    GameLibrary.Story.ClientStorySystem.Instance.PreloadStoryBytes(string.Empty, "visual.dsl", text);
                    GameLibrary.Story.ClientStorySystem.Instance.StartStory("visual_main");
                    var story = GameLibrary.Story.ClientStorySystem.Instance.GetStory("visual_main");
                    if (null != story) {
                        story.IsDebug = true;
                        story.OnExecDebugger = OnExecDebugger;
                        m_DebugStoryInstance = story;
                    }
                }
                if (GUILayout.Button("播放")) {
                    if (null != m_CurSelectedNode) {
                        var msgId = m_CurSelectedNode.Node.Message;
                        GameLibrary.Story.ClientStorySystem.Instance.SendMessage(msgId);
                    } else {
                        EditorUtility.DisplayDialog(string.Empty, "请选中的要播放的剧情结点！", "ok");
                    }
                }
            }
            if (GUILayout.Button("拷贝")) {
                GUIUtility.systemCopyBuffer = Export();
            }
            EditorGUILayout.EndHorizontal();
            float areaTop = 24;
            float areaHeight = position.height - areaTop;
            m_SplitRect.Set(position.width - m_ToolWindowWidth - m_SplitRect.width, areaTop, m_SplitRect.width, areaHeight);
            ResizeSplit();
            GUI.BeginGroup(new Rect(0, areaTop, m_SplitRect.x, areaHeight));
            {
                GUI.DrawTexture(new Rect(0, 0, m_SplitRect.x, position.height), m_SplitColor);
                m_ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, m_SplitRect.x, areaHeight), m_ScrollPosition, m_MaxRect, true, true);

                BeginWindows();
                {
                    foreach (var n in m_NodeContainers) {
                        Rect r = DrawNodeWindow(n);
                    }
                }
                EndWindows();

                DrawDebugger();
                MyDragLine();
                GUI.EndScrollView();
            }
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(m_SplitRect.xMax, areaTop, m_ToolWindowWidth, areaHeight), "");
            {
                GUILayout.BeginArea(new Rect(0, 0, m_ToolWindowWidth, areaHeight));
                ToolWindowOnGUI();
                GUILayout.EndArea();
            }
            GUI.EndGroup();

            MyDragWindow();
        }
        private void OnDestroy()
        {
            Clear();
            s_Instance = null;
        }
        private bool OnExecDebugger(StorySystem.StoryInstance instance, StorySystem.StoryMessageHandler handler, StorySystem.IStoryCommand cmd, long delta, object iterator, object[] args)
        {
            var curMessage = handler.MessageId;
            var curCommand = cmd.GetId();
            var h = instance.GetMessageHandler(curMessage);
            if (null == h || h != handler) {
                //剧情编辑器忽略ConcurrentMessageHandler的调试信息
                return false;
            }
            DebuggerInfo info;
            if(!m_DebuggerInfos.TryGetValue(curMessage, out info)) {
                info = new DebuggerInfo();
                m_DebuggerInfos.Add(curMessage, info);
            }
            info.CurMessage = curMessage;
            info.CurCommand = curCommand;

            StorySystem.StoryRuntime last = null;
            foreach(var runtime in handler.RuntimeStack) {
                last = runtime;
            }
            info.LeftCommandCount = null != last ? last.CommandQueue.Count : 0;

            m_DebuggerUpdated = true;
            return false;
        }
        private void Clear()
        {
            m_CurSelectedNode = null;
            m_CurSerializedObject = null;
            m_NodeContainers.Clear();

            GameObject obj;
            while (obj = GameObject.Find("VisualStoryEditor")) {
                GameObject.DestroyImmediate(obj);
            }
            m_EditorNodeInScene = null;
            m_NextSceneObjID = 1;
        }
        private void Import(string path)
        {
            Dsl.DslFile file = new Dsl.DslFile();
            if (file.Load(path, msg => Debug.LogWarningFormat("{0}", msg))) {
                if (file.DslInfos.Count == 1) {
                    var dslInfo = file.DslInfos[0];
                    if (dslInfo.GetId() == "story" && dslInfo.GetFunctionNum() == 1 && dslInfo.First.Call.GetParamId(0) == "visual_main") {
                        int index = 0;
                        float x = 32, y = 32;
                        float maxHeight = 0;
                        foreach(var comp in dslInfo.First.Statements) {
                            var funcData = comp as Dsl.FunctionData;
                            if (null != funcData && funcData.GetId() != "local") {
                                var msgId = funcData.Call.GetParamId(0);
                                int nx = index % 4;
                                var pos = new Vector2(x + nx * 300, y);
                                var nodeInfo = new NodeInfo();
                                nodeInfo.Message = msgId;
                                
                                foreach(var fc in funcData.Statements) {
                                    var name = fc.GetId();
                                    var cmdInfo = m_ApiManager.NewCommandInfoByName(name);
                                    if (null == cmdInfo) {
                                        Debug.LogErrorFormat("Can't find command config '{0}' !", name);
                                        continue;
                                    }
                                    nodeInfo.Commands.Add(cmdInfo);

                                    var cd = fc as Dsl.CallData;
                                    if (null != cd) {
                                        var tupleInfo = cmdInfo.Tuples[0];
                                        ImportCallData(tupleInfo, cd);
                                    } else {
                                        var fd = fc as Dsl.FunctionData;
                                        if (null != fd) {
                                            var tupleInfo = cmdInfo.Tuples[0];
                                            ImportFunctionData(tupleInfo, fd);
                                        } else {
                                            var sd = fc as Dsl.StatementData;
                                            if (null != sd) {
                                                ImportStatementData(cmdInfo, sd);
                                            } else {
                                                Debug.LogWarningFormat("command {0} use default data !", fc.GetId());
                                            }
                                        }
                                    }
                                }

                                AddNode(nodeInfo, pos);
                                ++index;

                                if (maxHeight < nodeInfo.Container.Rect.height)
                                    maxHeight = nodeInfo.Container.Rect.height;
                                if (nx == 3) {
                                    y += maxHeight + 16;
                                    maxHeight = 0;
                                }
                            }
                        }
                        foreach (var c in m_NodeContainers) {
                            var node = c.Node;
                            node.RefreshOutputs(m_NodeContainers);
                            foreach(var cmd in node.Commands) {
                                foreach (var tuple in cmd.Tuples) {
                                    ParamDrawerFactory.InitDrawers(tuple, cmd, node);
                                }
                            }
                        }
                    } else {
                        EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！[story(visual_story_main)]", "ok");
                    }
                } else {
                    EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！", "ok");
                }
            }
        }
        private void ImportCallData(CommandTupleInfo tupleInfo, Dsl.CallData cd)
        {
            for (int i = 0; i < tupleInfo.OptionalParamStart && i < tupleInfo.Params.Length && i < cd.GetParamNum(); ++i) {
                var pc = cd.GetParam(i);
                tupleInfo.Params[i].DslInitValue = pc;
                tupleInfo.Params[i].InitValue = ParamInfo.DslToString(pc);
            }
        }
        private void ImportFunctionData(CommandTupleInfo tupleInfo, Dsl.FunctionData fd)
        {
            var cd = fd.Call;
            ImportCallData(tupleInfo, cd);
            foreach (var fcomp in fd.Statements) {
                var fcd = fcomp as Dsl.CallData;
                if (null != fcd) {
                    var key = fcd.GetId();
                    var pc = fcd.GetParam(0);
                    foreach(var param in tupleInfo.Params) {
                        if (param.Name == key) {
                            param.DslInitValue = pc;
                            param.InitValue = ParamInfo.DslToString(pc);
                        }
                    }
                }
            }
        }
        private void ImportStatementData(CommandInfo cmdInfo, Dsl.StatementData sd)
        {
            foreach(var fd in sd.Functions) {
                var key = fd.GetId();
                foreach(var tupleInfo in cmdInfo.Tuples) {
                    if (tupleInfo.Name == key) {
                        ImportFunctionData(tupleInfo, fd);
                    }
                }
            }
        }
        private string Export()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("story(visual_main)");
            sb.AppendLine("{");
            sb.AppendLine("\tlocal");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\t@realPlayerId(0);");
            sb.AppendLine("\t\t@prefab1(0);");
            sb.AppendLine("\t\t@prefab2(0);");
            sb.AppendLine("\t\t@prefab3(0);");
            sb.AppendLine("\t\t@prefab4(0);");
            sb.AppendLine("\t\t@prefab5(0);");
            sb.AppendLine("\t};");
            foreach (var c in m_NodeContainers) {
                var node = c.Node;
                node.Export(sb);
            }
            sb.AppendLine("};");
            return sb.ToString();
        }

        private Rect DrawNodeWindow(NodeContainer container)
        {
            //container.rect.x = container.rect.x < 0 ? 0 : container.rect.x;
            //container.rect.y = container.rect.y < 0 ? 0 : container.rect.y;
            container.Rect = GUI.Window(container.WinID, container.Rect, (int id) => { DoNodeWindowGUI(container); }, "", EditorStyles.helpBox);
            if (container == m_CurSelectedNode) {
                GUI.FocusWindow(container.WinID);
            }

            for (int i = 0; i < container.GetOutputNum(); ++i) {
                GUI.DrawTexture(container.GetOutputPortRect(i), m_TexPort);
                GUI.Label(container.GetOutputPortRect(i), "➨");
            }
            GUI.DrawTexture(container.GetInputPortRect(), m_TexPort);
            GUI.Label(container.GetInputPortRect(), " ➨");
            return container.Rect;
        }
        private void DoNodeWindowGUI(NodeContainer container)
        {
            if (Event.current.type == EventType.MouseDrag) {
            } else if (Event.current.type == EventType.MouseDown) {
                SetNodeSelected(container);
            } else if (Event.current.type == EventType.MouseUp) {
                if (Event.current.button == 1) {
                    ShowPopupMenu_OnNode(container);
                    Event.current.Use();
                }
            }
            GUI.Label(new Rect(28, 4, container.Rect.width - 96, 16), container.Node.Message);
            if (GUI.Button(new Rect(4, 4, 16, 16), "X")) {
                DeleteNode(container);
            }
            if (GUI.Button(new Rect(container.Rect.width - 72, 4, 16, 16), "↑")) {
                MoveUp(container);
            }
            if (GUI.Button(new Rect(container.Rect.width - 48, 4, 16, 16), "↓")) {
                MoveDown(container);
            }
            if (GUI.Button(new Rect(container.Rect.width - 24, 4, 16, 16), "X")) {
                DeleteCommand(container);
            }

            var nodeInfo = container.Node;
            if (null == nodeInfo.CommandCaptions) {
                var cmds = container.Node.Commands;
                nodeInfo.CommandCaptions = new string[cmds.Count];
                for (int i = 0; i < cmds.Count; ++i) {
                    var cmd = cmds[i];
                    nodeInfo.CommandCaptions[i] = cmd.Caption;
                }
            }
            nodeInfo.SelectedIndex = GUI.SelectionGrid(new Rect(4, 24, container.Rect.width - 8, 20 * nodeInfo.CommandCaptions.Length), nodeInfo.SelectedIndex, nodeInfo.CommandCaptions, 1, EditorStyles.toolbarButton);

            //拖动窗口
            GUI.DragWindow();
        }
        private void MyDragWindow()
        {
            if (Event.current.type == EventType.MouseDrag) {
                if (m_bDragingWindow) {
                    m_ScrollPosition = m_StartDragMousePos - Event.current.mousePosition + m_StargDragScrollPos;
                    Event.current.Use();
                }
            }
            if (Event.current.type == EventType.MouseDown) {
                if (Event.current.button == 2) {
                    m_bDragingWindow = true;
                    m_StartDragMousePos = Event.current.mousePosition;
                    m_StargDragScrollPos = m_ScrollPosition;
                }
            }
            if (Event.current.type == EventType.MouseUp) {
                if (Event.current.button == 2) {
                    m_bDragingWindow = false;
                }
                if (Event.current.button == 1) {
                    ShowPopupMenu_OnBkg();
                }
            }
            if(Event.current.type == EventType.KeyUp) {
                if (Event.current.control) {
                    switch (Event.current.keyCode) {
                        case KeyCode.Insert:
                            if (null != m_CurSelectedNode) {
                                var pos = new Vector2(m_CurSelectedNode.Rect.xMax - 32, m_CurSelectedNode.Rect.yMax - 32);
                                pos -= m_ScrollPosition - new Vector2(m_WorkspaceWidth / 2, m_WorkspaceHeight / 2);
                                AddNode(new NodeInfo(), pos);
                            } else {
                                var pos = new Vector2(m_SplitRect.x / 2, position.height / 2);
                                AddNode(new NodeInfo(), pos);
                            }
                            break;
                        case KeyCode.Delete:
                            if (null != m_CurSelectedNode) {
                                DeleteNode(m_CurSelectedNode);
                            }
                            break;
                    }
                }
                if (Event.current.shift) {
                    switch (Event.current.keyCode) {
                        case KeyCode.UpArrow:
                            if (null != m_CurSelectedNode) {
                                MoveUp(m_CurSelectedNode);
                            }
                            break;
                        case KeyCode.DownArrow:
                            if (null != m_CurSelectedNode) {
                                MoveDown(m_CurSelectedNode);
                            }
                            break;
                        case KeyCode.Insert:
                            if (null != m_CurSelectedNode) {
                                ShowPopupMenu_OnNode(m_CurSelectedNode);
                            }
                            break;
                        case KeyCode.Delete:
                            if (null != m_CurSelectedNode) {
                                DeleteCommand(m_CurSelectedNode);
                            }
                            break;
                    }
                }
                if (Event.current.alt) {
                    switch (Event.current.keyCode) {
                        case KeyCode.UpArrow:
                            CommandSelectPrev();
                            break;
                        case KeyCode.DownArrow:
                            CommandSelectNext();
                            break;
                        case KeyCode.LeftArrow:
                            NodeSelectPrev();
                            break;
                        case KeyCode.RightArrow:
                            NodeSelectNext();
                            break;
                    }
                }
            }
        }
        private void ShowPopupMenu_OnNode(NodeContainer container)
        {
            Vector2 pos = Event.current.mousePosition;
            GenericMenu toolsMenu = new GenericMenu();
            string[] types = m_ApiManager.Captions;
            for (int i = 0; i < types.Length; i++) {
                string sub = types[i];
                toolsMenu.AddItem(new GUIContent("Add Command/" + sub), false, () => {
                    var cmdInfo = m_ApiManager.NewCommandInfo(sub);
                    AddCommand(container.Node, cmdInfo);
                });
            }
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Duplicate Command"), false, () => {
                DuplicateCommand(container.Node);
            });
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Duplicate Node"), false, () => {
                var node = container.Node.Clone();
                node.Message = GetNewMessage(node.Message);
                AddNode(node, pos);
                node.RefreshOutputs(m_NodeContainers);
                foreach(var cmd in node.Commands) {
                    foreach (var tuple in cmd.Tuples) {
                        ParamDrawerFactory.InitDrawers(tuple, cmd, node);
                    }
                }
            });
            toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
        }
        private void ShowPopupMenu_OnBkg()
        {
            GenericMenu toolsMenu = new GenericMenu();
            Vector2 pos = Event.current.mousePosition;
            toolsMenu.AddItem(new GUIContent("Add Node"), false, () => {
                AddNode(new NodeInfo(), pos);
            });
            toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
        }
        private void MyDragLine()
        {
            if (m_StartLineNode != null) {
                if (m_bDragingInputLine) {
                    Vector2 lineEnd = m_StartLineNode.GetInputPortRect().center;
                    DrawBezier(Event.current.mousePosition, lineEnd, new Color(0.8f, 0.8f, 0.8f), true);
                } else if (m_bDragingOutputLine) {
                    Vector2 lineStart = m_StartLineNode.GetOutputPortRect(m_StartLineNodeIndex).center;
                    DrawBezier(lineStart, Event.current.mousePosition, new Color(0.8f, 0.8f, 0.8f), true);
                }
            }
            foreach (var n in m_NodeContainers) {
                for (int i = 0; i < n.GetOutputNum(); i++) {
                    var outputNode = n.Node.Outputs[i];
                    if (outputNode != null && null != outputNode.Node) {
                        var startPos = n.GetOutputPortRect(i).center;
                        var endPos = outputNode.Node.Container.GetInputPortRect().center;
                        DrawBezier(startPos, endPos, new Color(0.8f, 0.8f, 0.8f), true);
                    }
                }
            }
            if (Event.current.type == EventType.MouseDrag) {
                if (m_bDragingInputLine) {

                }
            }
            if (Event.current.type == EventType.MouseDown) {
                if (Event.current.button == 0) {
                    foreach (var n in m_NodeContainers) {
                        for (int i = 0; i < n.GetOutputNum(); i++) {
                            var outputNode = n.Node.Outputs[i];
                            if (outputNode != null) {
                                if (n.GetOutputPortRect(i).Contains(Event.current.mousePosition)) {
                                    m_bDragingOutputLine = true;
                                    m_StartLineNode = n;
                                    m_StartLineNodeIndex = i;
                                }
                            }
                        }
                        if (n.GetInputPortRect().Contains(Event.current.mousePosition)) {
                            m_bDragingInputLine = true;
                            m_StartLineNode = n;
                        }
                    }
                }
            }
            if (Event.current.type == EventType.MouseUp) {
                if (Event.current.button == 0) {
                    if (m_bDragingInputLine) {
                        foreach (var n in m_NodeContainers) {
                            if (n != m_StartLineNode) {
                                for (int i = 0; i < n.GetOutputNum(); i++) {
                                    var outputNode = n.Node.Outputs[i];
                                    if (outputNode != null) {
                                        if (n.GetOutputPortRect(i).Contains(Event.current.mousePosition)) {
                                            outputNode.Node = m_StartLineNode.Node;
                                            outputNode.Param.Value = m_StartLineNode.Node.Message;
                                        }
                                    }
                                }
                            }
                        }
                    } else if (m_bDragingOutputLine) {
                        foreach (var n in m_NodeContainers) {
                            if (n != m_StartLineNode) {
                                if (n.GetInputPortRect().Contains(Event.current.mousePosition)) {
                                    var outputNode = m_StartLineNode.Node.Outputs[m_StartLineNodeIndex];
                                    outputNode.Node = n.Node;
                                    outputNode.Param.Value = n.Node.Message;
                                }
                            }
                        }
                    }
                    m_bDragingInputLine = false;
                    m_bDragingOutputLine = false;
                    this.Repaint();
                }
            }
        }
        private int GetNewWinID()
        {
            m_CurWinID++;
            return m_CurWinID;
        }
        private void ResizeSplit()
        {
            //GUI.DrawTexture(splitRect, splitColor);
            EditorGUIUtility.AddCursorRect(m_SplitRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && m_SplitRect.Contains(Event.current.mousePosition)) {
                m_bResize = true;
            }
            if (m_bResize) {
                m_ToolWindowWidth = position.width - Event.current.mousePosition.x;
                m_SplitRect.Set(Event.current.mousePosition.x - m_SplitRect.width, m_SplitRect.y, m_SplitRect.width, m_SplitRect.height);

                this.Repaint();
            }
            if (Event.current.type == EventType.MouseUp) {
                m_bResize = false;
            }
        }
        private void DrawDebugger()
        {
            if (null == m_DebugStoryInstance)
                return;
            m_WaitDeletes.Clear();
            foreach(var pair in m_DebuggerInfos) {
                var id = pair.Key;
                bool triggered = false;
                var handler = m_DebugStoryInstance.GetMessageHandler(id);
                if (handler.IsTriggered) {
                    triggered = true;
                }
                if (!triggered) {
                    m_WaitDeletes.Add(id);
                }
            }
            foreach(var key in m_WaitDeletes) {
                m_DebuggerInfos.Remove(key);
            }
            foreach (var pair in m_DebuggerInfos) {
                var info = pair.Value;
                var curMessage = info.CurMessage;
                var leftCommandCount = info.LeftCommandCount;
                foreach (var c in m_NodeContainers) {
                    var node = c.Node;
                    if (node.Message == curMessage) {
                        var rt0 = c.Rect;
                        Handles.color = Color.yellow;
                        Handles.DrawPolyLine(rt0.position, rt0.position + new Vector2(0, rt0.height), rt0.position + rt0.size, rt0.position + new Vector2(rt0.width, 0), rt0.position);

                        int ix = node.Commands.Count - leftCommandCount;
                        if (ix >= 0 && ix < node.Commands.Count) {
                            var cmd = node.Commands[ix];
                            var rt = c.Rect;
                            rt.xMin = rt.xMin + 2;
                            rt.xMax = rt.xMax - 2;
                            rt.yMin = rt.yMin + 23 + 18 * ix;
                            rt.yMax = rt.yMin + 18;
                            Handles.color = Color.red;
                            Handles.DrawPolyLine(rt.position, rt.position + new Vector2(0, rt.height), rt.position + rt.size, rt.position + new Vector2(rt.width, 0), rt.position);
                            break;
                        }
                    }
                }
            }
        }
        private void ToolWindowOnGUI()
        {
            GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            myFoldoutStyle.fontStyle = FontStyle.Bold;

            if (m_CurSelectedNode != null) {
                m_bShowDetailConfig = EditorGUILayout.Foldout(m_bShowDetailConfig, "Detail Config", myFoldoutStyle);
                if (m_bShowDetailConfig) {
                    DrawNodeProperty();
                }
            }

            m_bShowDebugger = EditorGUILayout.Foldout(m_bShowDebugger, "Debugger", myFoldoutStyle);
            if (m_bShowDebugger) {
                foreach (var pair in m_DebuggerInfos) {
                    var info = pair.Value;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("cur message:");
                    EditorGUILayout.LabelField(info.CurMessage);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("cur command:");
                    EditorGUILayout.LabelField(info.CurCommand);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("left command:");
                    EditorGUILayout.LabelField(info.LeftCommandCount.ToString());
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Separator();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void AddNode(NodeInfo node, Vector2 pos)
        {
            if (string.IsNullOrEmpty(node.Message)) {
                node.Message = GetNewMessage("noname_");
            }
            NodeContainer c = ScriptableObject.CreateInstance<NodeContainer>();
            c.SetNode(node);
            c.WinID = GetNewWinID();
            c.Rect = new Rect(pos + m_ScrollPosition - new Vector2(m_WorkspaceWidth / 2, m_WorkspaceHeight / 2), new Vector2(256, 44 + node.Commands.Count * 20));
            c.Rect.x = (int)c.Rect.x;
            c.Rect.y = (int)c.Rect.y;
            c.AllNodeContainers = m_NodeContainers;
            m_NodeContainers.Add(c);
            Repaint();
        }
        private void DeleteNode(NodeContainer container)
        {
            if (EditorUtility.DisplayDialog("危险操作提示", "删除消息处理结点？", "是", "否")) {
                m_NodeContainers.Remove(container);
                m_CurSelectedNode = null;
                m_CurSerializedObject = null;
                Repaint();
            }
        }
        private void DuplicateCommand(NodeInfo node)
        {
            var selectedIndex = node.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < node.Commands.Count) {
                var cmdInfo = node.Commands[selectedIndex].Clone();
                node.Commands.Add(cmdInfo);
                node.CommandCaptions = null;
                node.RefreshOutputs(m_NodeContainers);
                foreach (var tuple in cmdInfo.Tuples) {
                    ParamDrawerFactory.InitDrawers(tuple, cmdInfo, node);
                }
                node.Container.Rect.height += 20;
                Repaint();
            }
        }
        private void AddCommand(NodeInfo node, CommandInfo cmdInfo)
        {
            node.Commands.Add(cmdInfo);
            node.CommandCaptions = null;
            node.RefreshOutputs(m_NodeContainers);
            foreach (var tuple in cmdInfo.Tuples) {
                ParamDrawerFactory.InitDrawers(tuple, cmdInfo, node);
            }
            node.Container.Rect.height += 20;
            Repaint();
        }
        private void DeleteCommand(NodeContainer container)
        {
            var node = container.Node;
            var ix = node.SelectedIndex;
            if (ix >= 0 && ix < node.Commands.Count) {
                if (EditorUtility.DisplayDialog("危险操作提示", "删除命令？", "是", "否")) {
                    node.Commands.RemoveAt(ix);
                    node.CommandCaptions = null;
                    node.RefreshOutputs(m_NodeContainers);
                    node.Container.Rect.height -= 20;

                    if (node.SelectedIndex >= node.Commands.Count) {
                        if (node.Commands.Count > 0) {
                            node.SelectedIndex = node.Commands.Count - 1;
                        } else {
                            node.SelectedIndex = 0;
                        }
                    }

                    Repaint();
                }
            }
        }
        private void MoveUp(NodeContainer container)
        {
            var node = container.Node;
            var ix = node.SelectedIndex;
            if (ix > 0) {
                var ix2 = ix - 1;
                var t = node.Commands[ix2];
                node.Commands[ix2] = node.Commands[ix];
                node.Commands[ix] = t;
                node.SelectedIndex = ix2;
                node.CommandCaptions = null;
                Repaint();
            }
        }
        private void MoveDown(NodeContainer container)
        {
            var node = container.Node;
            var ix = node.SelectedIndex;
            if (ix < node.Commands.Count - 1) {
                var ix2 = ix + 1;
                var t = node.Commands[ix2];
                node.Commands[ix2] = node.Commands[ix];
                node.Commands[ix] = t;
                node.SelectedIndex = ix2;
                node.CommandCaptions = null;
                Repaint();
            }
        }
        private void CommandSelectPrev()
        {
            if (null != m_CurSelectedNode) {
                var node = m_CurSelectedNode.Node;
                if (node.SelectedIndex > 0) {
                    --node.SelectedIndex;
                    Repaint();
                }
            }
        }
        private void CommandSelectNext()
        {
            if (null != m_CurSelectedNode) {
                var node = m_CurSelectedNode.Node;
                if (node.SelectedIndex >= 0 && node.SelectedIndex < node.Commands.Count - 1) {
                    ++node.SelectedIndex;
                    Repaint();
                }
            }
        }
        private void NodeSelectPrev()
        {
            if (null != m_CurSelectedNode) {
                int ix = m_NodeContainers.IndexOf(m_CurSelectedNode);
                if (ix > 0) {
                    SetNodeSelected(m_NodeContainers[ix - 1]);
                    Repaint();
                }
            } else if (m_NodeContainers.Count > 0) {
                SetNodeSelected(m_NodeContainers[0]);
            }
        }
        private void NodeSelectNext()
        {
            if (null != m_CurSelectedNode) {
                int ix = m_NodeContainers.IndexOf(m_CurSelectedNode);
                if (ix >= 0 && ix < m_NodeContainers.Count - 1) {
                    SetNodeSelected(m_NodeContainers[ix + 1]);
                    Repaint();
                }
            } else if (m_NodeContainers.Count > 0) {
                SetNodeSelected(m_NodeContainers[0]);
            }
        }

        private void DrawNodeProperty()
        {
            if (null != m_CurSerializedObject) {
                var nodeProp = m_CurSerializedObject.FindProperty("Node");
                EditorGUILayout.PropertyField(nodeProp);

                var node = m_CurSelectedNode.Node;
                while (node.DeferredActions.Count > 0) {
                    var action = node.DeferredActions.Dequeue();
                    try {
                        if (null != action)
                            action();
                    } catch {
                    }
                }
            }
        }
        private void SetNodeSelected(NodeContainer container)
        {
            if (m_CurSelectedNode != container) {
                m_CurSelectedNode = container;
                m_CurSerializedObject = new SerializedObject(container);
            } else if (null == m_CurSerializedObject) {
                m_CurSerializedObject = new SerializedObject(container);
            }
        }
        
        private string GetNewMessage(string label)
        {
            string rlt = label;
            int i = 0;
            while (true) {
                string temp = label + i.ToString();
                bool bFound = false;
                foreach (var n in m_NodeContainers) {
                    if (n.Node.Message == temp) {
                        bFound = true;
                        break;
                    }
                }
                i++;
                if (!bFound) {
                    rlt = temp;
                    break;
                }
            }
            return rlt;
        }
        private void DrawBezier(Vector2 fromPos, Vector2 toPos, Color color, bool bArrow)
        {
            Vector3 pos0 = new Vector3(fromPos.x, fromPos.y, 0);
            Vector3 pos1 = new Vector3((fromPos.x + toPos.x) * 0.5f, fromPos.y, 0);
            Vector3 pos2 = new Vector3((fromPos.x + toPos.x) * 0.5f, toPos.y, 0);
            Vector3 pos3 = new Vector3(toPos.x, toPos.y, 0);
            Handles.DrawBezier(pos0, pos3, pos1, pos2, color, null, 3f);
            if (bArrow) {
                int positive = toPos.x > fromPos.x ? 1 : -1;
                Vector3 arrowSideUp = new Vector3(toPos.x - 12 * positive, toPos.y - 6, 0);
                Vector3 arrowSideDown = new Vector3(toPos.x - 12 * positive, toPos.y + 6, 0);
                Handles.color = color;
                Handles.DrawLine(toPos, arrowSideUp);
                Handles.DrawLine(toPos, arrowSideDown);
            }
        }

        //debugger
        private sealed class DebuggerInfo
        {
            internal string CurMessage = string.Empty;
            internal string CurCommand = string.Empty;
            internal int LeftCommandCount = 0;
        }

        private Dictionary<string, DebuggerInfo> m_DebuggerInfos = new Dictionary<string, DebuggerInfo>();
        private List<string> m_WaitDeletes = new List<string>();
        private StorySystem.StoryInstance m_DebugStoryInstance = null;
        private StorySystem.StoryMessageHandlerList m_HandlerList = new StorySystem.StoryMessageHandlerList();

        //for drag window
        private Vector2 m_ScrollPosition = new Vector2(0, 0);
        private int m_WorkspaceWidth = 1280;
        private int m_WorkspaceHeight = 4096;
        private bool m_bDragingWindow = false;
        private bool m_bDragingInputLine = false;
        private bool m_bDragingOutputLine = false;
        private Vector2 m_StartDragMousePos;
        private Vector2 m_StargDragScrollPos;
        private Rect m_MaxRect;

        //for drag line
        private NodeContainer m_StartLineNode = null;
        private int m_StartLineNodeIndex = 0;

        //
        private int m_CurWinID = 100;
        private List<NodeContainer> m_NodeContainers = new List<NodeContainer>();
        private NodeContainer m_CurSelectedNode = null;
        private SerializedObject m_CurSerializedObject = null;
        private GameObject m_EditorNodeInScene = null;
        private int m_NextSceneObjID = 1;

        //for tool
        private bool m_bShowDetailConfig = true;
        private bool m_bShowDebugger = true;
        private bool m_DebuggerUpdated = false;

        //split and resize
        private bool m_bResize = false;
        private Rect m_SplitRect;
        private float m_ToolWindowWidth = 320;
        private Texture2D m_SplitColor;
        private Texture2D m_PortColor;
        private Texture2D m_TexPort;

        //编辑器元数据
        private ApiManager m_ApiManager = new ApiManager();
    }
}