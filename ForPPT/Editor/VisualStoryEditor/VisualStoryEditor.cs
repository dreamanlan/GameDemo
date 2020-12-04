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

        internal void QueueFiles(string[] files)
        {
            m_FilesQueue.Enqueue(files);
        }

        private void Init()
        {
            m_ItemHeight = EditorStyles.toolbarButton.fixedHeight;

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
            ReloadTemplates();

            if (null != HeightMapData.Instance && HeightMapData.Instance.heightData == null) {
                //SceneRangeInspector.UpdateHeightmap(SceneRange.Instance);
            }

            //EditorApplication.ExecuteMenuItem("Window/Timeline");

            CsLibrary.GlobalVariables.Instance.StoryEditorOpen = true;
        }

        private void OnGUI()
        {
            var oldTextColor = GUI.skin.button.normal.textColor;
            GUI.skin.button.normal.textColor = Color.magenta;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(m_SecondId, EditorStyles.toolbarTextField);
            if (EditorApplication.isPlaying) {
                if (GUILayout.Button("刷新", EditorStyles.toolbarButton)) {
                    var gcs = new List<GUIContent>();
                    foreach (var si in CsLibrary.Story.GfxStorySystem.Instance.ActiveStories) {
                        var id = si.StoryId;
                        m_StoryIds.Add(id);
                    }
                    m_StoryIds.Sort();
                    foreach (var id in m_StoryIds) {
                        gcs.Add(new GUIContent(id, id));
                    }
                    m_StoryIds.Insert(0, "--");
                    gcs.Insert(0, new GUIContent("--", "--"));
                    m_Stories = gcs.ToArray();
                    m_StoryIndex = 0;
                }
                if (null != m_Stories) {
                    int index = EditorGUILayout.Popup(m_StoryIndex, m_Stories, EditorStyles.toolbarPopup);
                    if (index != m_StoryIndex) {
                        m_StoryIndex = index;

                        if (index > 0) {
                            var id = m_StoryIds[index];
                            var storyInstance = CsLibrary.Story.GfxStorySystem.Instance.GetStory(id);
                            if (null != storyInstance) {
                                if (null != m_DebugStoryInstance) {
                                    m_DebugStoryInstance.IsDebug = false;
                                    m_DebugStoryInstance.OnExecDebugger = null;
                                }
                                storyInstance.IsDebug = true;
                                storyInstance.OnExecDebugger = OnExecDebugger;
                                m_DebugStoryInstance = storyInstance;

                                Clear();
                                ImportDslInfo(storyInstance.Config as Dsl.FunctionData, "visual", false);
                                m_TextChanged = false;
                                m_LastTempText = Export();
                                m_LastHistoryText = m_LastTempText;
                                m_StartTime = Time.time;
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("清空", EditorStyles.toolbarButton)) {
                if (EditorUtility.DisplayDialog(string.Empty, "确定要清空所有编辑内容么？", "是", "否")) {
                    Clear();
                }
            }
            if (GUILayout.Button("打开", EditorStyles.toolbarButton)) {
                string path = EditorUtility.OpenFilePanel("打开剧情脚本", string.Empty, "dsl");
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    Clear();
                    Import(path, true);
                    if (EditorApplication.isPlaying) {
                        var id = m_FirstId;
                        if (!string.IsNullOrEmpty(m_SecondId))
                            id = m_FirstId + ":" + m_SecondId;
                        var storyInstance = CsLibrary.Story.GfxStorySystem.Instance.GetStory(id);
                        if (null != storyInstance) {
                            storyInstance.IsDebug = true;
                            storyInstance.OnExecDebugger = OnExecDebugger;
                            if (null != m_DebugStoryInstance) {
                                m_DebugStoryInstance.IsDebug = false;
                                m_DebugStoryInstance.OnExecDebugger = null;
                            }
                            m_DebugStoryInstance = storyInstance;
                        }
                    }
                    m_TextChanged = false;
                    m_LastTempText = Export();
                    m_LastHistoryText = m_LastTempText;
                }
            }
            if (GUILayout.Button("合并", EditorStyles.toolbarButton)) {
                VisualStoryFileListWindow.InitWindow(this);
            }
            EditorGUI.BeginDisabledGroup(!m_TextChanged);
            if (GUILayout.Button("保存", EditorStyles.toolbarButton)) {
                Save(m_StoryFilePath);
            }
            if (GUILayout.Button("另存为", EditorStyles.toolbarButton)) {
                Save(string.Empty);
            }
            EditorGUI.EndDisabledGroup();
            if (EditorApplication.isPlaying) {
                if (GUILayout.Button("应用到场景", EditorStyles.toolbarButton)) {
                    Utility.EventSystem.Publish((int)CsLibrary.GameEventLib.gm_resetdsl);
                    var txt = Export();
                    byte[] text = Encoding.UTF8.GetBytes(txt);
                    CsLibrary.Story.GfxStorySystem.Instance.LoadStoryFromBytes(string.Empty, "visual.dsl", text);
                    var storyId = m_FirstId;
                    if (!string.IsNullOrEmpty(m_SecondId)) {
                        storyId = m_FirstId + ":" + m_SecondId;
                    }
                    CsLibrary.Story.GfxStorySystem.Instance.StartStory(storyId);
                    var story = CsLibrary.Story.GfxStorySystem.Instance.GetStory(storyId);
                    if (null != story) {
                        if (null != m_DebugStoryInstance) {
                            m_DebugStoryInstance.IsDebug = false;
                            m_DebugStoryInstance.OnExecDebugger = null;
                        }
                        story.IsDebug = true;
                        story.OnExecDebugger = OnExecDebugger;
                        m_DebugStoryInstance = story;
                    }
                }
                if (GUILayout.Button("播放", EditorStyles.toolbarButton)) {
                    if (null != m_CurSelectedNode) {
                        HideEditorObject();
                        var msgId = m_CurSelectedNode.Node.Message;
                        CsLibrary.Story.GfxStorySystem.Instance.SendMessage(msgId);
                        m_StartTime = Time.time;
                    }
                    else {
                        EditorUtility.DisplayDialog(string.Empty, "请选中要播放的剧情结点！", "ok");
                    }
                }
                if (GUILayout.Button("继续", EditorStyles.toolbarButton)) {
                    CsLibrary.GlobalVariables.Instance.StoryEditorContinue = true;
                }
                int newCameraIndex = EditorGUILayout.Popup(m_CameraIndex, m_CameraNames);
                if (newCameraIndex != m_CameraIndex) {
                    m_CameraIndex = newCameraIndex;
                    switch (m_CameraIndex) {
                        case 1:
                            Utility.SendScriptMessage("SwitchToStoryCameraVC", null);
                            break;
                        case 2:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 0);
                            break;
                        case 3:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 1);
                            break;
                        case 4:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 2);
                            break;
                        case 5:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 3);
                            break;
                        case 6:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 4);
                            break;
                        case 7:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 5);
                            break;
                        case 8:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 6);
                            break;
                        case 9:
                            Utility.SendScriptMessage("SwitchToStoryCameraVCA", 7);
                            break;
                        case 10:
                            Utility.SendScriptMessage("SwitchToMainCamera", null);
                            break;
                        case 11:
                            Utility.SendScriptMessage("SwitchToMainCameraAndDelayReset", 1.0f);
                            break;
                    }
                }
            }
            if (GUILayout.Button("拷贝", EditorStyles.toolbarButton)) {
                GUIUtility.systemCopyBuffer = Export();
            }
            EditorGUILayout.TextField("模板名：", EditorStyles.toolbarTextField, GUILayout.Width(40));
            m_TemplateId = EditorGUILayout.TextField(m_TemplateId, EditorStyles.toolbarTextField, GUILayout.Width(80));
            if (GUILayout.Button("保存为模板", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                if (string.IsNullOrEmpty(m_TemplateId)) {
                    EditorUtility.DisplayDialog(string.Empty, "请指定模板名！", "ok");
                }
                else {
                    string path = EditorUtility.SaveFilePanel("保存剧情模板", string.Empty, "visual", "dsl");
                    if (!string.IsNullOrEmpty(path)) {
                        File.WriteAllText(path, Export(m_TemplateId));
                    }
                }
            }
            if (GUILayout.Button("加载模板", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                ReloadTemplates();
            }
            EditorGUILayout.EndHorizontal();
            float areaTop = 20;
            float areaHeight = position.height - areaTop;
            m_SplitRect.Set(position.width - m_ToolWindowWidth - m_SplitRect.width, areaTop, m_SplitRect.width, areaHeight);
            ResizeSplit();
            GUI.BeginGroup(new Rect(0, areaTop, m_SplitRect.x, areaHeight));
            {
                GUI.DrawTexture(new Rect(0, 0, m_SplitRect.x, position.height), m_SplitColor);
                m_ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, m_SplitRect.x, areaHeight), m_ScrollPosition, m_MaxRect, true, true);

                BeginWindows();
                {
                    bool messageChanged = false;
                    foreach (var n in m_NodeContainers) {
                        Rect r = DrawNodeWindow(n);
                        if (n.Node.MessageChanged) {
                            n.Node.MessageChanged = false;
                            messageChanged = true;
                        }
                    }
                    if (messageChanged) {
                        foreach (var n in m_NodeContainers) {
                            n.Node.SyncOutputs();
                        }
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

            MyKeyUpDown();

            if (m_NeedSort) {
                m_NeedSort = false;
                SortNodes();
            }

            HandleFilesQueue();
            HandleTempAndHistory();

            GUI.skin.button.normal.textColor = oldTextColor;
        }
        private void OnDestroy()
        {
            Clear();
            s_Instance = null;
            CsLibrary.GlobalVariables.Instance.StoryEditorOpen = false;
        }
        private void HideEditorObject()
        {
            var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
            if (null != pobj) {
                pobj.SetActive(false);
            }
        }
        private bool OnExecDebugger(StorySystem.StoryInstance instance, StorySystem.StoryMessageHandler handler, StorySystem.IStoryCommand cmd, long delta, BoxedValue iterator, BoxedValueList args)
        {
            var curMessage = handler.MessageId;
            var curCommand = cmd.GetId();
            var h = instance.GetMessageHandler(curMessage);
            if (null == h) {
                return false;
            }
            DebuggerInfo info;
            if (!m_DebuggerInfos.TryGetValue(curMessage, out info)) {
                info = new DebuggerInfo();
                m_DebuggerInfos.Add(curMessage, info);
            }
            int cct = 0;
            if (h != handler) {
                var list = instance.GetConcurrentMessageHandler(curMessage);
                cct = list.Count;
            }
            if (h == handler || null == info.CurCommand) {
                info.CurMessage = curMessage;
                info.CurCommand = curCommand;

                StorySystem.StoryRuntime last = null;
                foreach (var runtime in handler.RuntimeStack) {
                    last = runtime;
                }
                int cmdCt = 0;
                if (null != last) {
                    foreach (var c in last.CommandQueue) {
                        if (null != c.GetConfig()) {
                            ++cmdCt;
                        }
                    }
                }
                info.LeftCommandCount = cmdCt;
                info.ConcurrentCount = cct;
            }
            m_DebuggerUpdated = true;
            return false;
        }

        private TableConfig.SceneInfo GetOriginalSceneInfo(int sceneId)
        {
            TableConfig.SceneInfo info;
            while (ConfigManagerBase.sceneList.TryGetValue(sceneId, out info)) {
                if (info.FollowSceneId <= 0)
                    break;
                sceneId = info.FollowSceneId;
            }
            return info;
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

            m_StoryFilePath = string.Empty;
            m_SecondId = string.Empty;
            m_TemplateId = string.Empty;

            m_TextChanged = false;
        }
        private void ReloadTemplates()
        {
            m_Templates.Clear();
            var files = Directory.GetFiles("templates", "*.dsl", SearchOption.AllDirectories);
            foreach (var file in files) {
                ImportTemplate(file);
            }
        }
        private void ImportTemplate(string path)
        {
            Dsl.DslFile file = new Dsl.DslFile();
            if (file.Load(path, msg => Debug.LogWarningFormat("{0}", msg))) {
                if (file.DslInfos.Count == 1) {
                    var dslFunc = file.DslInfos[0] as Dsl.FunctionData;
                    if (null == dslFunc) {
                        EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！", "ok");
                        return;
                    }
                    var call = dslFunc.ThisOrLowerOrderCall;
                    int idNum = call.GetParamNum();
                    var firstId = call.GetParamId(0);
                    var secondId = idNum > 1 ? call.GetParamId(1) : string.Empty;
                    if (dslFunc.GetId() == "story" && idNum == 2 && firstId == "visual_main" && !string.IsNullOrEmpty(secondId) && dslFunc.HaveStatement()) {
                        foreach (var comp in dslFunc.Params) {
                            var firstFuncData = comp as Dsl.FunctionData;
                            var lastFuncData = comp as Dsl.FunctionData;
                            if (null == lastFuncData) {
                                var statementData = comp as Dsl.StatementData;
                                firstFuncData = statementData.First;
                                lastFuncData = statementData.Last;
                            }
                            if (null != firstFuncData && null != lastFuncData && firstFuncData.GetId() != "local") {
                                var msgId = firstFuncData.LowerOrderFunction.GetParamId(0);
                                var nodeInfo = new NodeInfo();
                                nodeInfo.Message = msgId.Trim();

                                foreach (var fc in lastFuncData.Params) {
                                    var name = fc.GetId();
                                    var cmdInfo = m_ApiManager.NewCommandInfoByName(name);
                                    if (null == cmdInfo) {
                                        Debug.LogErrorFormat("Can't find command config '{0}' !", name);
                                        continue;
                                    }
                                    nodeInfo.Commands.Add(cmdInfo);

                                    var cd = fc as Dsl.FunctionData;
                                    if (null != cd && !cd.IsHighOrder) {
                                        var tupleInfo = cmdInfo.Tuples[0];
                                        ImportCallData(tupleInfo, cd);
                                    }
                                    else {
                                        var fd = fc as Dsl.FunctionData;
                                        if (null != fd) {
                                            var tupleInfo = cmdInfo.Tuples[0];
                                            ImportFunctionData(tupleInfo, fd);
                                        }
                                        else {
                                            var sd = fc as Dsl.StatementData;
                                            if (null != sd) {
                                                ImportStatementData(cmdInfo, sd);
                                            }
                                            else {
                                                Debug.LogWarningFormat("command {0} use default data !", fc.GetId());
                                            }
                                        }
                                    }
                                }

                                AddTemplate(secondId, nodeInfo);
                            }
                        }
                    }
                    else {
                        EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！[story(visual_main)]", "ok");
                    }
                }
                else {
                    EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！", "ok");
                }
            }
        }
        private void Import(string path, bool renameDuplicate)
        {
            m_StoryFilePath = path;
            Dsl.DslFile file = new Dsl.DslFile();
            if (file.Load(path, msg => Debug.LogWarningFormat("{0}", msg))) {
                if (file.DslInfos.Count == 1) {
                    var dslInfo = file.DslInfos[0] as Dsl.FunctionData;
                    if (null != dslInfo) {
                        ImportDslInfo(dslInfo, path, renameDuplicate);
                    }
                    else {
                        EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！", "ok");
                    }
                }
                else {
                    EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！", "ok");
                }
            }
        }
        private void ImportDslInfo(Dsl.FunctionData dslInfo, string path, bool renameDuplicate)
        {
            var call = dslInfo.ThisOrLowerOrderCall;
            int idNum = call.GetParamNum();
            var firstId = call.GetParamId(0);
            var secondId = idNum > 1 ? call.GetParamId(1) : string.Empty;
            if (dslInfo.GetId() == "story" && idNum <= 2 && firstId == m_FirstId && dslInfo.HaveStatement()) {
                m_SecondId = secondId;
                if (string.IsNullOrEmpty(m_SecondId)) {
                    m_SecondId = Path.GetFileNameWithoutExtension(path);
                }
                int index = 0;
                float x = 32, y = 32;
                float maxHeight = 0;
                foreach (var comp in dslInfo.Params) {
                    var bodyFuncData = comp as Dsl.FunctionData;
                    Dsl.FunctionData msgFuncData = null;
                    Vector2 pos = Vector2.zero;
                    if (null == bodyFuncData) {
                        var statementData = comp as Dsl.StatementData;
                        if (null != statementData) {
                            msgFuncData = statementData.First;
                            foreach (var func in statementData.Functions) {
                                if (func.HaveStatement() && (func.GetId() == "comment" || func.GetId() == "comments" || func.GetId() == "opts")) {
                                    foreach (var opt in func.Params) {
                                        var optId = opt.GetId();
                                        if (optId == "pos") {
                                            var optData = opt as Dsl.FunctionData;
                                            if (null != optData) {
                                                var xstr = optData.GetParamId(0);
                                                var ystr = optData.GetParamId(1);
                                                float xv = float.Parse(xstr);
                                                float yv = float.Parse(ystr);
                                                pos = new Vector2(xv, yv);
                                                pos += new Vector2(m_WorkspaceWidth / 2, m_WorkspaceHeight / 2) - m_ScrollPosition;
                                            }
                                        }
                                    }
                                }
                                else if (func.HaveStatement()) {
                                    bodyFuncData = func;
                                }
                            }
                        }
                    }
                    else {
                        msgFuncData = bodyFuncData.ThisOrLowerOrderCall;
                    }
                    if (null != bodyFuncData) {
                        if (msgFuncData.IsValid() && bodyFuncData.GetId() != "local" && bodyFuncData.HaveStatement()) {
                            var msgId = msgFuncData.GetParamId(0);
                            if (ExistMessage(msgId)) {
                                if (renameDuplicate) {
                                    msgId = GetNewMessage(msgId);
                                }
                                else {
                                    var errInfo = string.Format("{0} message {1} duplicated, ignore it !", path, msgId);
                                    Debug.LogError(errInfo);
                                    EditorUtility.DisplayDialog(string.Empty, errInfo, "ok");
                                    continue;
                                }
                            }

                            int nx = index % 4;
                            if (pos.sqrMagnitude <= Mathf.Epsilon) {
                                pos = new Vector2(x + nx * 300, y);
                            }
                            var nodeInfo = new NodeInfo();
                            nodeInfo.Message = msgId.Trim();

                            foreach (var fc in bodyFuncData.Params) {
                                var name = fc.GetId();
                                var cmdInfo = m_ApiManager.NewCommandInfoByName(name);
                                if (null == cmdInfo) {
                                    Debug.LogErrorFormat("Can't find command config '{0}' !", name);
                                    continue;
                                }
                                nodeInfo.Commands.Add(cmdInfo);

                                var cd = fc as Dsl.FunctionData;
                                if (null != cd && ! cd.IsHighOrder) {
                                    var tupleInfo = cmdInfo.Tuples[0];
                                    ImportCallData(tupleInfo, cd);
                                }
                                else {
                                    var fd = fc as Dsl.FunctionData;
                                    if (null != fd) {
                                        var tupleInfo = cmdInfo.Tuples[0];
                                        ImportFunctionData(tupleInfo, fd);
                                    }
                                    else {
                                        var sd = fc as Dsl.StatementData;
                                        if (null != sd) {
                                            ImportStatementData(cmdInfo, sd);
                                        }
                                        else {
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
                }
                foreach (var c in m_NodeContainers) {
                    var node = c.Node;
                    node.RefreshOutputs(m_NodeContainers);
                    foreach (var cmd in node.Commands) {
                        foreach (var tuple in cmd.Tuples) {
                            ParamDrawerFactory.InitDrawers(tuple, cmd, node);
                        }
                    }
                }
            }
            else {
                EditorUtility.DisplayDialog(string.Empty, "请指定剧情编辑器保存的剧情脚本！[story(visual_main)]", "ok");
            }
        }
        private void ImportCallData(CommandTupleInfo tupleInfo, Dsl.FunctionData cd)
        {
            for (int i = 0; i < tupleInfo.OptionalParamStart && i < tupleInfo.Params.Length && i < cd.GetParamNum(); ++i) {
                var pc = cd.GetParam(i);
                tupleInfo.Params[i].DslInitValue = pc;
                tupleInfo.Params[i].InitValue = ParamInfo.DslToString(pc);
                tupleInfo.Params[i].Used = true;
            }
        }
        private void ImportFunctionData(CommandTupleInfo tupleInfo, Dsl.FunctionData fd)
        {
            var cd = fd.ThisOrLowerOrderCall;
            ImportCallData(tupleInfo, cd);
            if (fd.HaveStatement()) {
                foreach (var fcomp in fd.Params) {
                    var fcd = fcomp as Dsl.FunctionData;
                    if (null != fcd) {
                        var key = fcd.GetId();
                        var pc = fcd.GetParam(0);
                        foreach (var param in tupleInfo.Params) {
                            if (param.Name == key) {
                                param.DslInitValue = pc;
                                param.InitValue = ParamInfo.DslToString(pc);
                                param.Used = true;
                            }
                        }
                    }
                }
            }
        }
        private void ImportStatementData(CommandInfo cmdInfo, Dsl.StatementData sd)
        {
            int funcNum = sd.GetFunctionNum();
            var lastFunc = sd.Last;
            var id = lastFunc.GetId();
            if (funcNum >= 2 && id == "comment" || id == "comments") {
                var cd = lastFunc.ThisOrLowerOrderCall;
                cmdInfo.Comment = cd.GetParamId(0);
                sd.Functions.RemoveAt(funcNum - 1);
            }
            foreach (var fd in sd.Functions) {
                var key = fd.GetId();
                foreach (var tupleInfo in cmdInfo.Tuples) {
                    if (tupleInfo.Name == key) {
                        ImportFunctionData(tupleInfo, fd);
                    }
                }
            }
        }
        private string Export()
        {
            return Export(string.Empty);
        }
        private string Export(string templateId)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(templateId)) {
                if (string.IsNullOrEmpty(m_SecondId)) {
                    sb.AppendFormat("story({0})", m_FirstId);
                }
                else {
                    sb.AppendFormat("story({0},\"{1}\")", m_FirstId, m_SecondId.Replace('"', '_'));
                }
            }
            else {
                sb.AppendFormat("story({0},\"{1}\")", m_FirstId, templateId.Replace('"', '_'));
            }
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\tlocal");
            sb.AppendLine("\t{");
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

        private void Save(string path)
        {
            if (string.IsNullOrEmpty(path)) {
                string dir = string.Empty;
                string filename = "visual";
                if (!string.IsNullOrEmpty(m_StoryFilePath)) {
                    dir = Path.GetDirectoryName(m_StoryFilePath);
                    filename = Path.GetFileNameWithoutExtension(m_StoryFilePath);
                }
                path = EditorUtility.SaveFilePanel("保存剧情脚本", dir, filename, "dsl");
                if (!string.IsNullOrEmpty(path) && path != m_StoryFilePath) {
                    if (File.Exists(path)) {
                        File.Copy(path, string.Format("{0}.bak", path), true);
                    }
                }
            }
            if (!string.IsNullOrEmpty(path)) {
                m_TextChanged = false;
                m_SecondId = Path.GetFileNameWithoutExtension(path);
                m_StoryFilePath = path;
                File.WriteAllText(path, Export());
            }
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

            for (int i = 0; i < container.GetFixedOutputNum(); ++i) {
                GUI.DrawTexture(container.GetFixedOutputPortRect(i), m_TexPort);
                GUI.Label(container.GetFixedOutputPortRect(i), "➨");
            }

            GUI.DrawTexture(container.GetInputPortRect(), m_TexPort);
            GUI.Label(container.GetInputPortRect(), " ➨");

            /*
            //测试各个命令的位置的代码，正常运行注掉
            for (int ix = 0; ix < container.Node.Commands.Count; ++ix) {
                var rt = container.Rect;
                rt.xMin = rt.xMin + 2;
                rt.xMax = rt.xMax - 2;
                rt.yMin = rt.yMin + 24 + m_ItemHeight * ix;
                rt.yMax = rt.yMin + m_ItemHeight;
                Handles.color = Color.red;
                Handles.DrawPolyLine(rt.position, rt.position + new Vector2(0, rt.height), rt.position + rt.size, rt.position + new Vector2(rt.width, 0), rt.position);
            }
            var crt = container.Rect;
            var rtGrid = new Rect(crt.xMin, crt.yMin + 24, crt.width, m_ItemHeight * container.Node.Commands.Count);
            Handles.color = Color.blue;
            Handles.DrawPolyLine(rtGrid.position, rtGrid.position + new Vector2(0, rtGrid.height), rtGrid.position + rtGrid.size, rtGrid.position + new Vector2(rtGrid.width, 0), rtGrid.position);
            */
            return container.Rect;
        }
        private void DoNodeWindowGUI(NodeContainer container)
        {
            if (Event.current.type == EventType.MouseDrag) {
            }
            else if (Event.current.type == EventType.MouseDown) {
                SetNodeSelected(container);
            }
            else if (Event.current.type == EventType.MouseUp) {
                if (Event.current.button == 1) {
                    ShowPopupMenu_OnNode(container);
                }
                else {
                    m_NeedSort = true;
                }
            }
            GUI.Label(new Rect(28, 4, container.Rect.width - 96, 16), container.Node.Message);
            if (GUI.Button(new Rect(4, 4, 18, 16), "X")) {
                DeleteNode(container);
            }
            if (GUI.Button(new Rect(container.Rect.width - 72, 4, 16, 16), "↑")) {
                MoveUp(container);
            }
            if (GUI.Button(new Rect(container.Rect.width - 48, 4, 16, 16), "↓")) {
                MoveDown(container);
            }
            if (GUI.Button(new Rect(container.Rect.width - 24, 4, 18, 16), "X")) {
                DeleteSelectedCommands(container.Node);
            }

            var nodeInfo = container.Node;
            if (null == nodeInfo.CommandCaptions) {
                var cmds = container.Node.Commands;
                nodeInfo.CommandCaptions = new string[cmds.Count];
                for (int i = 0; i < cmds.Count; ++i) {
                    var cmd = cmds[i];
                    if (string.IsNullOrEmpty(cmd.Comment)) {
                        nodeInfo.CommandCaptions[i] = cmd.Caption;
                    }
                    else {
                        nodeInfo.CommandCaptions[i] = string.Format("{0}【{1}】", cmd.Caption, cmd.Comment);
                    }
                }
            }
            nodeInfo.SelectedIndex = GUI.SelectionGrid(new Rect(22, 24, container.Rect.width - 26, m_ItemHeight * nodeInfo.CommandCaptions.Length), nodeInfo.SelectedIndex, nodeInfo.CommandCaptions, 1, EditorStyles.toolbarButton);

            var rt = new Rect(4, 24, nodeInfo.Container.Rect.width - 8, m_ItemHeight * nodeInfo.CommandCaptions.Length);
            for (int i = 0; i < nodeInfo.Commands.Count; ++i) {
                var cmd = nodeInfo.Commands[i];
                var crt = new Rect(rt.xMin, rt.yMin + i * m_ItemHeight, 18, m_ItemHeight);
                cmd.Selected = GUI.Toggle(crt, cmd.Selected, string.Empty);
            }
            //拖动窗口
            GUI.DragWindow();
        }
        private int FindCommand(NodeInfo node, Vector3 pos)
        {
            var rt = new Rect(4, 24, node.Container.Rect.width - 8, m_ItemHeight * node.CommandCaptions.Length);
            if (!rt.Contains(pos))
                return -1;
            for (int i = 0; i < node.Commands.Count; ++i) {
                var crt = new Rect(rt.xMin, rt.yMin + i * m_ItemHeight, rt.width, m_ItemHeight);
                if (crt.Contains(pos)) {
                    return i;
                }
            }
            return -1;
        }
        private void MyKeyUpDown()
        {
            if (Event.current.type == EventType.MouseUp) {
                if (Event.current.button == 1) {
                    ShowPopupMenu_OnBkg();
                }
            }
            if (Event.current.type == EventType.KeyUp) {
                if (Event.current.control) {
                    switch (Event.current.keyCode) {
                        case KeyCode.Insert:
                            if (null != m_CurSelectedNode) {
                                var pos = new Vector2(m_CurSelectedNode.Rect.xMax - 32, m_CurSelectedNode.Rect.yMax - 32);
                                pos -= m_ScrollPosition - new Vector2(m_WorkspaceWidth / 2, m_WorkspaceHeight / 2);
                                AddNode(new NodeInfo(), pos);
                            }
                            else {
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
                                DeleteCommand(m_CurSelectedNode.Node, Vector2.zero);
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
                    AddCommand(container.Node, cmdInfo, pos);
                });
            }
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Duplicate Command"), false, () => {
                DuplicateCommand(container.Node, pos);
            });
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Copy Command"), false, () => {
                CopyCommand(container.Node, pos);
            });
            toolsMenu.AddItem(new GUIContent("Paste Command"), false, () => {
                PasteCommand(container.Node, pos);
            });
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Reverse Select"), false, () => {
                ReverseSelectCommands(container.Node);
            });
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Clear Batch Selected"), false, () => {
                ClearBatchSelected();
            });
            toolsMenu.AddItem(new GUIContent("Paste Batch Selected"), false, () => {
                PasteBatchSelected(container.Node, pos);
            });
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Delete Command"), false, () => {
                DeleteCommand(container.Node, pos);
            });
            toolsMenu.AddItem(new GUIContent("Delete All Break Commands"), false, () => {
                DeleteBreakCommands();
            });
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Duplicate Node"), false, () => {
                var node = container.Node.Clone();
                node.Message = GetNewMessage(node.Message);
                AddNode(node, pos);
                node.RefreshOutputs(m_NodeContainers);
                foreach (var cmd in node.Commands) {
                    foreach (var tuple in cmd.Tuples) {
                        ParamDrawerFactory.InitDrawers(tuple, cmd, node);
                    }
                }
            });
            toolsMenu.DropDown(new Rect(pos.x, pos.y, 0, 0));
            Event.current.Use();
        }
        private void ShowPopupMenu_OnBkg()
        {
            GenericMenu toolsMenu = new GenericMenu();
            Vector2 pos = Event.current.mousePosition;
            float areaTop = 20;
            float areaHeight = position.height - areaTop;
            var rt = new Rect(0, areaTop, m_SplitRect.x, areaHeight);
            if (!rt.Contains(pos))
                return;

            toolsMenu.AddItem(new GUIContent("Add Node"), false, () => {
                AddNode(new NodeInfo(), pos);
            });
            foreach (var pair in m_Templates) {
                var key = pair.Key;
                toolsMenu.AddItem(new GUIContent("Template/" + key), false, () => {
                    AddFromTemplate(key, pos);
                });
            }
            toolsMenu.AddSeparator(string.Empty);
            toolsMenu.AddItem(new GUIContent("Layout"), false, () => {
                Layout();
            });
            toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
            Event.current.Use();
        }
        private void MyDragLine()
        {
            if (m_StartLineNode != null) {
                if (m_bDragingInputLine) {
                    Vector2 lineEnd = m_StartLineNode.GetInputPortRect().center;
                    DrawBezier(Event.current.mousePosition, lineEnd, new Color(0.8f, 0.8f, 0.8f), true);
                }
                else if (m_bDragingOutputLine) {
                    Vector2 lineStart = m_StartLineNode.GetOutputPortRect(m_StartLineIndex).center;
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
                for (int i = 0; i < n.GetFixedOutputNum(); i++) {
                    var outputNode = n.Node.FixedOutputs[i];
                    if (outputNode != null && null != outputNode.Messages && outputNode.Messages.Count > 0) {
                        var startPos = n.GetFixedOutputPortRect(i).center;
                        foreach (var c in m_NodeContainers) {
                            if (outputNode.Messages.IndexOf(c.Node.Message) >= 0) {
                                var endPos = c.GetInputPortRect().center;
                                DrawBezier(startPos, endPos, new Color(0.8f, 0.8f, 0.8f), true);
                            }
                        }
                    }
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
                                    m_StartLineIndex = i;
                                    break;
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
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (m_bDragingOutputLine) {
                        bool connected = false;
                        foreach (var n in m_NodeContainers) {
                            if (n != m_StartLineNode) {
                                if (n.GetInputPortRect().Contains(Event.current.mousePosition)) {
                                    var outputNode = m_StartLineNode.Node.Outputs[m_StartLineIndex];
                                    outputNode.Node = n.Node;
                                    outputNode.Param.Value = n.Node.Message;
                                    connected = true;
                                    break;
                                }
                            }
                        }
                        if (!connected) {
                            var outputNode = m_StartLineNode.Node.Outputs[m_StartLineIndex];
                            outputNode.Node = null;
                            outputNode.Param.Value = string.Empty;
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
            foreach (var pair in m_DebuggerInfos) {
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
            foreach (var key in m_WaitDeletes) {
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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("播放时长:", EditorStyles.toolbarTextField, GUILayout.Width(120));
            if (null != m_DebugStoryInstance && !m_DebugStoryInstance.CanSleep()) {
                var curTime = Time.time;
                var playTime = curTime - m_StartTime;
                m_PlayTimeStr = playTime.ToString("F3");
            }
            EditorGUILayout.TextField(m_PlayTimeStr, EditorStyles.toolbarTextField);
            EditorGUILayout.EndHorizontal();

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
                    EditorGUILayout.LabelField("concurrent count:");
                    EditorGUILayout.LabelField(info.ConcurrentCount.ToString());
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Separator();
                    EditorGUILayout.EndHorizontal();
                    if (null != m_DebugStoryInstance) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("===Global Variables===");
                        EditorGUILayout.EndHorizontal();
                        foreach (var v in m_DebugStoryInstance.GlobalVariables) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(v.Key + ":");
                            if (v.Value.IsNullObject)
                                EditorGUILayout.LabelField("null");
                            else
                                EditorGUILayout.LabelField(v.Value.ToString());
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("===Local Variables===");
                        EditorGUILayout.EndHorizontal();
                        foreach (var v in m_DebugStoryInstance.LocalVariables) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(v.Key + ":");
                            if (v.Value.IsNullObject)
                                EditorGUILayout.LabelField("null");
                            else
                                EditorGUILayout.LabelField(v.Value.ToString());
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("===Stack Variables===");
                        EditorGUILayout.EndHorizontal();
                        foreach (var v in m_DebugStoryInstance.StackVariables) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(v.Key + ":");
                            if (v.Value.IsNullObject)
                                EditorGUILayout.LabelField("null");
                            else
                                EditorGUILayout.LabelField(v.Value.ToString());
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Separator();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void AddTemplate(string id, NodeInfo node)
        {
            List<NodeInfo> nodes;
            if (!m_Templates.TryGetValue(id, out nodes)) {
                nodes = new List<NodeInfo>();
                m_Templates.Add(id, nodes);
            }
            node.Message = string.Empty;
            nodes.Add(node);
        }
        private void AddFromTemplate(string id, Vector2 pos)
        {
            List<NodeInfo> templateNodes;
            if (m_Templates.TryGetValue(id, out templateNodes)) {
                var list = new List<NodeInfo>();
                foreach (var node in templateNodes) {
                    var n = node.Clone();
                    list.Add(n);
                    AddNode(n, pos);
                    pos = pos + new Vector2(20, 20);
                }
                foreach (var node in list) {
                    node.RefreshOutputs(m_NodeContainers);
                    foreach (var cmd in node.Commands) {
                        foreach (var tuple in cmd.Tuples) {
                            ParamDrawerFactory.InitDrawers(tuple, cmd, node);
                        }
                    }
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
            c.Rect = new Rect(pos + m_ScrollPosition - new Vector2(m_WorkspaceWidth / 2, m_WorkspaceHeight / 2), new Vector2(256, 44 + node.Commands.Count * m_ItemHeight));
            c.Rect.x = (int)c.Rect.x;
            c.Rect.y = (int)c.Rect.y;
            m_NodeContainers.Add(c);
            m_NeedSort = true;
            Repaint();
        }
        private void DeleteNode(NodeContainer container)
        {
            if (EditorUtility.DisplayDialog("危险操作提示", string.Format("删除消息处理结点‘{0}’？", container.Node.Message), "是", "否")) {
                m_NodeContainers.Remove(container);
                m_CurSelectedNode = null;
                m_CurSerializedObject = null;
                Repaint();
            }
        }
        private void CopyCommand(NodeInfo node, Vector2 pos)
        {
            var selectedIndex = node.SelectedIndex;
            var hotIndex = FindCommand(node, pos);
            if (hotIndex < 0)
                hotIndex = selectedIndex;
            if (hotIndex >= 0 && hotIndex < node.Commands.Count) {
                m_CopyItemNode = node;
                m_CopyItemIndex = hotIndex;
            }
        }
        private void PasteCommand(NodeInfo node, Vector2 pos)
        {
            if (null == m_CopyItemNode || m_CopyItemIndex < 0 || m_CopyItemIndex >= m_CopyItemNode.Commands.Count)
                return;
            var selectedIndex = node.SelectedIndex;
            var hotIndex = FindCommand(node, pos);
            if (hotIndex < 0)
                hotIndex = node.Commands.Count - 1;
            var cmdInfo = m_CopyItemNode.Commands[m_CopyItemIndex].Clone();
            if (hotIndex + 1 < node.Commands.Count) {
                node.Commands.Insert(hotIndex + 1, cmdInfo);
            }
            else {
                node.Commands.Add(cmdInfo);
            }
            node.CommandCaptions = null;
            node.RefreshOutputs(m_NodeContainers);
            foreach (var tuple in cmdInfo.Tuples) {
                ParamDrawerFactory.InitDrawers(tuple, cmdInfo, node);
            }
            node.Container.Rect.height += m_ItemHeight;
            if (hotIndex < selectedIndex) {
                node.SelectedIndex = node.SelectedIndex + 1;
            }
            Repaint();
        }
        private void DeleteSelectedCommands(NodeInfo node)
        {
            if (EditorUtility.DisplayDialog("危险操作提示", "确定要删除当前结点上所有选中的命令么？", "是", "否")) {
                CommandInfo selectedCmd = null;
                if (node.SelectedIndex >= 0 && node.SelectedIndex < node.Commands.Count) {
                    selectedCmd = node.Commands[node.SelectedIndex];
                }
                for (int ix = node.Commands.Count - 1; ix >= 0; --ix) {
                    var cmd = node.Commands[ix];
                    if (cmd.Selected) {
                        node.Commands.RemoveAt(ix);
                        node.Container.Rect.height -= m_ItemHeight;
                    }
                }
                node.SelectedIndex = node.Commands.IndexOf(selectedCmd);
                node.CommandCaptions = null;
                node.RefreshOutputs(m_NodeContainers);
                Repaint();
            }
        }
        private void ReverseSelectCommands(NodeInfo node)
        {
            foreach (var cmd in node.Commands) {
                cmd.Selected = !cmd.Selected;
            }
            Repaint();
        }
        private void ClearBatchSelected()
        {
            foreach (var c in m_NodeContainers) {
                foreach (var cmd in c.Node.Commands) {
                    cmd.Selected = false;
                }
            }
            Repaint();
        }
        private void PasteBatchSelected(NodeInfo node, Vector2 pos)
        {
            CommandInfo selectedCmd = null;
            if (node.SelectedIndex >= 0 && node.SelectedIndex < node.Commands.Count) {
                selectedCmd = node.Commands[node.SelectedIndex];
            }
            var hotIndex = FindCommand(node, pos);
            if (hotIndex < 0)
                hotIndex = node.Commands.Count - 1;

            List<CommandInfo> list = new List<CommandInfo>();
            foreach (var c in m_NodeContainers) {
                foreach (var cmd in c.Node.Commands) {
                    if (cmd.Selected) {
                        list.Add(cmd);
                    }
                }
            }

            foreach (var cmd in list) {
                var cmdInfo = cmd.Clone();
                if (hotIndex + 1 < node.Commands.Count) {
                    node.Commands.Insert(hotIndex + 1, cmdInfo);
                    ++hotIndex;
                }
                else {
                    node.Commands.Add(cmdInfo);
                    hotIndex = node.Commands.Count - 1;
                }
                foreach (var tuple in cmdInfo.Tuples) {
                    ParamDrawerFactory.InitDrawers(tuple, cmdInfo, node);
                }
                node.Container.Rect.height += m_ItemHeight;
            }

            node.CommandCaptions = null;
            node.RefreshOutputs(m_NodeContainers);
            node.SelectedIndex = node.Commands.IndexOf(selectedCmd);
            Repaint();
        }
        private void DuplicateCommand(NodeInfo node, Vector2 pos)
        {
            var selectedIndex = node.SelectedIndex;
            var hotIndex = FindCommand(node, pos);
            if (hotIndex < 0)
                hotIndex = selectedIndex;
            if (hotIndex >= 0 && hotIndex < node.Commands.Count) {
                var cmdInfo = node.Commands[hotIndex].Clone();
                if (hotIndex + 1 < node.Commands.Count) {
                    node.Commands.Insert(hotIndex + 1, cmdInfo);
                }
                else {
                    node.Commands.Add(cmdInfo);
                }
                node.CommandCaptions = null;
                node.RefreshOutputs(m_NodeContainers);
                foreach (var tuple in cmdInfo.Tuples) {
                    ParamDrawerFactory.InitDrawers(tuple, cmdInfo, node);
                }
                node.Container.Rect.height += m_ItemHeight;
                if (hotIndex < selectedIndex) {
                    node.SelectedIndex = node.SelectedIndex + 1;
                }
                Repaint();
            }
        }
        private void AddCommand(NodeInfo node, CommandInfo cmdInfo, Vector2 pos)
        {
            var selectedIndex = node.SelectedIndex;
            var hotIndex = FindCommand(node, pos);
            if (hotIndex >= 0 && hotIndex < node.Commands.Count - 1) {
                node.Commands.Insert(hotIndex + 1, cmdInfo);
            }
            else {
                node.Commands.Add(cmdInfo);
            }
            node.CommandCaptions = null;
            node.RefreshOutputs(m_NodeContainers);
            foreach (var tuple in cmdInfo.Tuples) {
                ParamDrawerFactory.InitDrawers(tuple, cmdInfo, node);
            }
            node.Container.Rect.height += m_ItemHeight;
            if (hotIndex < selectedIndex) {
                node.SelectedIndex = node.SelectedIndex + 1;
            }
            Repaint();
        }
        private void DeleteCommand(NodeInfo node, Vector2 pos)
        {
            var selectedIndex = node.SelectedIndex;
            var ix = FindCommand(node, pos);
            if (ix < 0)
                ix = selectedIndex;
            if (ix >= 0 && ix < node.Commands.Count) {
                if (EditorUtility.DisplayDialog("危险操作提示", string.Format("删除命令‘{0}’？", node.Commands[ix].Caption), "是", "否")) {
                    node.Commands.RemoveAt(ix);
                    node.CommandCaptions = null;
                    node.RefreshOutputs(m_NodeContainers);
                    node.Container.Rect.height -= m_ItemHeight;

                    if (node.SelectedIndex >= node.Commands.Count) {
                        if (node.Commands.Count > 0) {
                            node.SelectedIndex = node.Commands.Count - 1;
                        }
                        else {
                            node.SelectedIndex = 0;
                        }
                    }

                    Repaint();
                }
            }
        }
        private void DeleteBreakCommands()
        {
            foreach (var c in m_NodeContainers) {
                var node = c.Node;

                CommandInfo selectedCmd = null;
                if (node.SelectedIndex >= 0 && node.SelectedIndex < node.Commands.Count) {
                    selectedCmd = node.Commands[node.SelectedIndex];
                }
                for (int ix = node.Commands.Count - 1; ix >= 0; --ix) {
                    var cmd = node.Commands[ix];
                    if (cmd.Name == "storybreak") {
                        node.Commands.RemoveAt(ix);
                        node.Container.Rect.height -= m_ItemHeight;
                    }
                }
                node.SelectedIndex = node.Commands.IndexOf(selectedCmd);
                node.CommandCaptions = null;
                node.RefreshOutputs(m_NodeContainers);
            }
            Repaint();
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
            }
            else if (m_NodeContainers.Count > 0) {
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
            }
            else if (m_NodeContainers.Count > 0) {
                SetNodeSelected(m_NodeContainers[0]);
            }
        }
        private void Layout()
        {
            SortNodes();
            m_ScrollPosition = Vector2.zero;

            int index = 0;
            float x = 32, y = 32;
            float maxHeight = 0;
            foreach (var c in m_NodeContainers) {
                var node = c.Node;

                int nx = index % 4;
                var pos = new Vector2(x + nx * 300, y);
                c.Rect = new Rect(pos + m_ScrollPosition - new Vector2(m_WorkspaceWidth / 2, m_WorkspaceHeight / 2), new Vector2(256, 44 + node.Commands.Count * m_ItemHeight));
                c.Rect.x = (int)c.Rect.x;
                c.Rect.y = (int)c.Rect.y;
                ++index;

                if (maxHeight < c.Rect.height)
                    maxHeight = c.Rect.height;
                if (nx == 3) {
                    y += maxHeight + 16;
                    maxHeight = 0;
                }
            }
            Repaint();
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
                    }
                    catch {
                    }
                }
            }
        }
        private void SetNodeSelected(NodeContainer container)
        {
            if (m_CurSelectedNode != container) {
                m_CurSelectedNode = container;
                m_CurSerializedObject = new SerializedObject(container);
            }
            else if (null == m_CurSerializedObject) {
                m_CurSerializedObject = new SerializedObject(container);
            }
        }
        private void SortNodes()
        {
            m_NodeContainers.Sort((a, b) => {
                if (IsSucceed(a, b)) {
                    return -1;
                }
                else if (IsSucceed(b, a)) {
                    return 1;
                }
                else {
                    var rt1 = a.Rect;
                    var rt2 = b.Rect;
                    if (rt1.yMin < rt2.yMin - Mathf.Epsilon) {
                        return -1;
                    }
                    else if (rt1.yMin > rt2.yMin + Mathf.Epsilon) {
                        return 1;
                    }
                    else {
                        if (rt1.xMin < rt2.xMin - Mathf.Epsilon)
                            return -1;
                        else if (rt1.xMin > rt2.xMin + Mathf.Epsilon)
                            return 1;
                        else
                            return 0;
                    }
                }
            });
        }
        private bool IsSucceed(NodeContainer a, NodeContainer b)
        {
            foreach (var output in a.Node.Outputs) {
                if (output.Node == b.Node)
                    return true;
            }
            foreach (var output in a.Node.FixedOutputs) {
                if (output.Messages.Contains(b.Node.Message))
                    return true;
            }
            return false;
        }
        private void HandleFilesQueue()
        {
            while (m_FilesQueue.Count > 0) {
                var list = m_FilesQueue.Dequeue();
                foreach (var path in list) {
                    Import(path, false);
                }
            }
        }
        private void HandleTempAndHistory()
        {
            double curTime = EditorApplication.timeSinceStartup;
            if (m_LastTempSaveTime + c_TempSaveInterval < curTime) {
                m_LastTempSaveTime = curTime;

                SaveTemp();
            }
            if (m_LastHistoryFileSaveTime + c_HistoryFileSaveInterval < curTime) {
                m_LastHistoryFileSaveTime = curTime;

                SaveHistory();
            }
        }
        private void SaveTemp()
        {
            if (m_NodeContainers.Count <= 0)
                return;
            if (!Directory.Exists(c_HistoryDirName))
                Directory.CreateDirectory(c_HistoryDirName);
            var txt = Export();
            if (txt != m_LastTempText) {
                m_TextChanged = true;
                m_LastTempText = txt;
                File.WriteAllText(string.Format("{0}/temp.dsl", c_HistoryDirName), txt);
            }
        }
        private void SaveHistory()
        {
            if (m_NodeContainers.Count <= 0)
                return;
            if (!Directory.Exists(c_HistoryDirName))
                Directory.CreateDirectory(c_HistoryDirName);
            var txt = Export();
            if (txt != m_LastHistoryText) {
                m_LastHistoryText = txt;
                File.WriteAllText(string.Format("{0}/history_{1:D3}.dsl", c_HistoryDirName, m_HistoryFileIndex), txt);
                m_HistoryFileIndex = (m_HistoryFileIndex + 1) % c_MaxHistoryFileNum;
            }
        }

        private bool ExistMessage(string label)
        {
            foreach (var n in m_NodeContainers) {
                if (n.Node.Message == label) {
                    return true;
                }
            }
            return false;
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
            internal int ConcurrentCount = 0;
        }

        private Dictionary<string, DebuggerInfo> m_DebuggerInfos = new Dictionary<string, DebuggerInfo>();
        private List<string> m_WaitDeletes = new List<string>();
        private StorySystem.StoryInstance m_DebugStoryInstance = null;
        private StorySystem.StoryMessageHandlerList m_HandlerList = new StorySystem.StoryMessageHandlerList();

        //layout
        private float m_ItemHeight = 18;
        private Vector2 m_ScrollPosition = new Vector2(0, 0);
        private int m_WorkspaceWidth = 1280;
        private int m_WorkspaceHeight = 40960;
        private Rect m_MaxRect;

        //for drag line
        private bool m_bDragingInputLine = false;
        private bool m_bDragingOutputLine = false;
        private NodeContainer m_StartLineNode = null;
        private int m_StartLineIndex = 0;

        //
        private int m_CurWinID = 100;
        private List<NodeContainer> m_NodeContainers = new List<NodeContainer>();
        private NodeContainer m_CurSelectedNode = null;
        private SerializedObject m_CurSerializedObject = null;
        private GameObject m_EditorNodeInScene = null;
        private int m_NextSceneObjID = 1;
        private NodeInfo m_CopyItemNode = null;
        private int m_CopyItemIndex = 0;
        private bool m_NeedSort = false;

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

        //story file
        private string m_StoryFilePath = string.Empty;
        private bool m_TextChanged = false;

        //story ids
        private string m_TemplateId = string.Empty;
        private string m_FirstId = "visual_main";
        private string m_SecondId = string.Empty;

        //当前加载story
        private GUIContent[] m_Stories = null;
        private List<string> m_StoryIds = new List<string>();
        private int m_StoryIndex = 0;

        //camera
        private string[] m_CameraNames = new string[] { "默认相机", "切换到剧情VC相机", "切换到剧情VC相机阵列0", "切换到剧情VC相机阵列1", "切换到剧情VC相机阵列2", "切换到剧情VC相机阵列3", "切换到剧情VC相机阵列4", "切换到剧情VC相机阵列5", "切换到剧情VC相机阵列6", "切换到剧情VC相机阵列7", "切换到主相机", "切换到主相机（延迟重置blend）" };
        private int m_CameraIndex = 0;

        //time
        private float m_StartTime = 0;
        private string m_PlayTimeStr = string.Empty;

        //files queue
        private Queue<string[]> m_FilesQueue = new Queue<string[]>();

        //temp file
        private string m_LastTempText = string.Empty;
        private double m_LastTempSaveTime = 0;
        private const double c_TempSaveInterval = 1.0f;
        //history file
        private string m_LastHistoryText = string.Empty;
        private int m_HistoryFileIndex = 0;
        private double m_LastHistoryFileSaveTime = 0;
        private const double c_HistoryFileSaveInterval = 60.0f;
        private const string c_HistoryDirName = "histories";
        private const int c_MaxHistoryFileNum = 120;

        //模板数据
        private Dictionary<string, List<NodeInfo>> m_Templates = new Dictionary<string, List<NodeInfo>>();
        //编辑器元数据
        private ApiManager m_ApiManager = new ApiManager();
    }

    public class VisualStoryFileListWindow : EditorWindow
    {
        internal static void InitWindow(VisualStoryEditor vsEdit)
        {
            VisualStoryFileListWindow window = (VisualStoryFileListWindow)EditorWindow.GetWindow(typeof(VisualStoryFileListWindow));
            window.Init(vsEdit);
            window.Show();
        }

        private void Init(VisualStoryEditor vsEdit)
        {
            m_VisualStoryWindow = vsEdit;
            m_IsReady = true;
        }

        private void OnGUI()
        {
            bool handle = false;
            int deleteIndex = -1;
            var rt = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加", EditorStyles.toolbarButton)) {
                m_IsReady = false;
                string path = EditorUtility.OpenFilePanel("打开剧情脚本", string.Empty, "dsl");
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    m_List.Add(path);
                }
                m_IsReady = true;
            }
            if (GUILayout.Button("合并", EditorStyles.toolbarButton)) {
                handle = true;
            }
            EditorGUILayout.EndHorizontal();
            if (m_IsReady) {
                m_Pos = EditorGUILayout.BeginScrollView(m_Pos);
                for (int i = 0; i < m_List.Count; ++i) {
                    EditorGUILayout.BeginHorizontal();
                    var file = m_List[i];
                    EditorGUILayout.LabelField("剧情脚本:", GUILayout.Width(40));
                    EditorGUILayout.LabelField(file);
                    if (GUILayout.Button("删除", GUILayout.Width(40))) {
                        deleteIndex = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            if (deleteIndex >= 0) {
                m_List.RemoveAt(deleteIndex);
            }
            if (handle) {
                m_VisualStoryWindow.QueueFiles(m_List.ToArray());
            }
        }

        private bool m_IsReady = false;
        private VisualStoryEditor m_VisualStoryWindow = null;
        private List<string> m_List = new List<string>();
        private Vector2 m_Pos = Vector2.zero;
    }
}