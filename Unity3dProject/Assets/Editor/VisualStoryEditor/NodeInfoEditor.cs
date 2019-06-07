using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VisualStoryTool
{
    [CustomPropertyDrawer(typeof(NodeInfo), true)]
    public sealed class NodeInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var container = property.serializedObject.targetObject as NodeContainer;
            NodeInfo node = container.Node;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Message:", GUILayout.Width(80));
            node.Message = EditorGUILayout.TextField(node.Message);
            EditorGUILayout.EndHorizontal();

            var selectedIndex = node.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < node.Commands.Count) {
                var cmdInfo = node.Commands[selectedIndex];
                if (null != cmdInfo) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(cmdInfo.Caption);
                    EditorGUILayout.EndHorizontal();

                    foreach (var tuple in cmdInfo.Tuples) {
                        EditorGUILayout.BeginHorizontal();
                        if (tuple.IsOptional) {
                            tuple.Used = EditorGUILayout.Toggle(tuple.Used, GUILayout.Width(20));
                        } else {
                            tuple.Used = true;
                        }
                        EditorGUILayout.LabelField(tuple.Caption, GUILayout.Width(80));
                        EditorGUILayout.EndHorizontal();
                        if (tuple.Used) {
                            foreach (var p in tuple.Params) {
                                if (p.Semantic == "hidden")
                                    continue;
                                EditorGUILayout.BeginHorizontal();
                                if (p.IsOptional) {
                                    p.Used = EditorGUILayout.Toggle(p.Used, GUILayout.Width(20));
                                } else {
                                    p.Used = true;
                                }
                                EditorGUILayout.LabelField(p.Caption, GUILayout.Width(80));
                                if (null == p.Drawer) {
                                    var oldVal = p.Value;
                                    p.Value = EditorGUILayout.TextField(oldVal);
                                } else {
                                    p.Drawer.Inspect();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                }
            }
        }
    }

    internal abstract class AbstractParamDrawer : IParamDrawer
    {
        public string GetValue()
        {
            return m_Value;
        }

        public void SetValue(string val)
        {
            m_Value = val;
        }

        public bool Inspect()
        {
            if (null == VisualStoryEditor.Instance)
                return false;
            return OnInspect();
        }

        public bool Update()
        {
            if (null == VisualStoryEditor.Instance)
                return false;
            return OnUpdate();
        }

        internal void Init(ParamInfo param, CommandTupleInfo tupleInfo, CommandInfo cmdInfo, NodeInfo nodeInfo)
        {
            if (null == VisualStoryEditor.Instance)
                return;
            m_Param = param;
            m_TupleInfo = tupleInfo;
            m_CmdInfo = cmdInfo;
            m_NodeInfo = nodeInfo;
            OnInit();
        }

        protected abstract void OnInit();
        protected abstract bool OnInspect();
        protected virtual bool OnUpdate() { return false; }

        protected void SelectObject(UnityEngine.Object obj)
        {
            m_NodeInfo.DeferredActions.Enqueue(() => {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(Selection.activeObject);
                //SceneView.lastActiveSceneView.FrameSelected(true);
                //SceneView.FrameLastActiveSceneView();
            });
        }
        protected GameObject CreatePosObj(Vector3 pos)
        {
            var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
            if (null != pobj) {
                var id = VisualStoryEditor.Instance.GetNextSceneObjID();
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.AddComponent<PutOnGround>();
                obj.hideFlags = HideFlags.DontSave;
                obj.name = "pos_" + id;
                obj.transform.SetParent(pobj.transform, false);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                m_Param.SceneObject = obj;
            }
            return m_Param.SceneObject;
        }
        protected GameObject CreateDirObj(float dir)
        {
            var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
            if (null != pobj) {
                var id = VisualStoryEditor.Instance.GetNextSceneObjID();
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.AddComponent<PutOnGround>();
                obj.hideFlags = HideFlags.DontSave;
                obj.name = "dir_" + id;
                obj.transform.SetParent(pobj.transform, false);
                obj.transform.localPosition = Vector3.one * 100;
                obj.transform.localEulerAngles = new Vector3(0, Mathf.Rad2Deg * dir, 0);
                obj.transform.localScale = Vector3.one;
                m_Param.SceneObject = obj;
            }
            return m_Param.SceneObject;
        }
        protected GameObject CreatePosDirObj(bool isPos, Vector3 pos, float dir)
        {
            if (null != m_CmdInfo.SceneObjects && m_CmdInfo.SceneObjects.Count > 0) {
                m_Param.SceneObject = m_CmdInfo.SceneObjects[0];
            } else {
                var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
                if (null != pobj) {
                    var id = VisualStoryEditor.Instance.GetNextSceneObjID();
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    obj.AddComponent<PutOnGround>();
                    obj.hideFlags = HideFlags.DontSave;
                    obj.name = "posdir_" + id;
                    obj.transform.SetParent(pobj.transform, false);
                    if (isPos) {
                        obj.transform.localPosition = pos;
                        obj.transform.localRotation = Quaternion.identity;
                    } else {
                        obj.transform.localPosition = Vector3.one * 100;
                        obj.transform.localEulerAngles = new Vector3(0, Mathf.Rad2Deg * dir, 0);
                    }
                    obj.transform.localScale = Vector3.one;
                    m_Param.SceneObject = obj;
                    if (null == m_CmdInfo.SceneObjects)
                        m_CmdInfo.SceneObjects = new List<GameObject>();
                    m_CmdInfo.SceneObjects.Add(obj);
                }
            }
            return m_Param.SceneObject;
        }

        protected ParamInfo m_Param;
        protected CommandTupleInfo m_TupleInfo;
        protected CommandInfo m_CmdInfo;
        protected NodeInfo m_NodeInfo;
        protected string m_Value;
    }

    internal sealed class BoolDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            var vd = m_Param.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                bool v = bool.Parse(vd.GetId());
                m_Val = v;
            }
        }

        protected override bool OnInspect()
        {
            bool newVal = EditorGUILayout.Toggle(m_Val);
            if (newVal != m_Val) {
                m_Val = newVal;
                m_Value = m_Val ? "true" : "false";
                return true;
            }
            return false;
        }

        private bool m_Val = false;
    }

    internal sealed class IntDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            var vd = m_Param.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                int v = int.Parse(vd.GetId());
                m_Val = v;
            }
        }

        protected override bool OnInspect()
        {
            int newVal = EditorGUILayout.IntField(m_Val);
            if (newVal != m_Val) {
                m_Val = newVal;
                m_Value = m_Val.ToString();
                return true;
            }
            return false;
        }

        private int m_Val = 0;
    }

    internal sealed class FloatDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            var vd = m_Param.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                float v = float.Parse(vd.GetId());
                m_Val = v;
            }
        }

        protected override bool OnInspect()
        {
            float newVal = EditorGUILayout.FloatField(m_Val);
            if (Mathf.Abs(newVal - m_Val) > Mathf.Epsilon) {
                m_Val = newVal;
                m_Value = m_Val.ToString();
                return true;
            }
            return false;
        }

        private float m_Val = 0;
    }

    internal sealed class IntSliderDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            int.TryParse(m_Value, out m_CurValue);
            if (null != m_Param.SemanticOptions) {
                foreach (var comp in m_Param.SemanticOptions) {
                    var cd = comp as Dsl.CallData;
                    if (null != cd && cd.GetId() == "range") {
                        var left = cd.GetParamId(0);
                        var right = cd.GetParamId(1);
                        int.TryParse(left, out m_LeftValue);
                        int.TryParse(right, out m_RightValue);
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            int newVal = EditorGUILayout.IntSlider(m_CurValue, m_LeftValue, m_RightValue);
            if (newVal != m_CurValue) {
                m_CurValue = newVal;
                m_Value = newVal.ToString();
                return true;
            }
            return false;
        }

        private int m_CurValue;
        private int m_LeftValue;
        private int m_RightValue;
    }

    internal sealed class FloatSliderDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            float.TryParse(m_Value, out m_CurValue);
            if (null != m_Param.SemanticOptions) {
                foreach (var comp in m_Param.SemanticOptions) {
                    var cd = comp as Dsl.CallData;
                    if (null != cd && cd.GetId() == "range") {
                        var left = cd.GetParamId(0);
                        var right = cd.GetParamId(1);
                        float.TryParse(left, out m_LeftValue);
                        float.TryParse(right, out m_RightValue);
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            float newVal = EditorGUILayout.Slider(m_CurValue, m_LeftValue, m_RightValue);
            if (Mathf.Abs(newVal - m_CurValue) > Mathf.Epsilon) {
                m_CurValue = newVal;
                m_Value = newVal.ToString();
                return true;
            }
            return false;
        }

        private float m_CurValue;
        private float m_LeftValue;
        private float m_RightValue;
    }

    internal sealed class PopupDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            if (null != m_Param.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_Param.SemanticOptions) {
                    var cd = comp as Dsl.CallData;
                    if (null != cd && cd.GetId() == "option") {
                        var key = cd.GetParamId(0);
                        var val = cd.GetParamId(1);
                        keys.Add(key);
                        m_List.Add(key, val);
                    }
                }
                m_Captions = keys.ToArray();
                m_SelectedIndex = -1;
                for (int i = 0; i < keys.Count; ++i) {
                    string key = keys[i];
                    string val;
                    if (m_List.TryGetValue(key, out val) && val == m_Value) {
                        m_SelectedIndex = i;
                        break;
                    }
                }
            } else {
                m_Captions = new string[0];
            }
        }

        protected override bool OnInspect()
        {
            int newIndex = EditorGUILayout.Popup(m_SelectedIndex, m_Captions);
            if (newIndex != m_SelectedIndex) {
                m_SelectedIndex = newIndex;
                string key = m_Captions[newIndex];
                string val;
                m_List.TryGetValue(key, out val);
                m_Value = val;
                return true;
            }
            return false;
        }

        private string[] m_Captions;
        private Dictionary<string, string> m_List = new Dictionary<string, string>();
        private int m_SelectedIndex;
    }

    internal sealed class TuplePopupDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            if (null != m_Param.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_Param.SemanticOptions) {
                    var cd = comp as Dsl.CallData;
                    if (null != cd) {
                        var id = cd.GetId();
                        if (id == "option") {
                            var key = cd.GetParamId(0);
                            keys.Add(key);
                            var vals = new List<string>();
                            for (int i = 1; i < cd.GetParamNum(); ++i) {
                                var val = cd.GetParamId(i);
                                vals.Add(val);
                            }
                            m_List.Add(key, vals);
                        } else if (id == "params") {
                            for(int i = 0; i < cd.GetParamNum(); ++i) {
                                var name = cd.GetParamId(i);
                                foreach(var param in m_TupleInfo.Params) {
                                    if (param.Name == name) {
                                        m_Params.Add(param);
                                    }
                                }
                            }
                        }
                    }
                }
                m_Captions = keys.ToArray();
                m_SelectedIndex = -1;
                for (int i = 0; i < keys.Count; ++i) {
                    string key = keys[i];
                    List<string> vals;
                    if (m_List.TryGetValue(key, out vals) && IsSame(vals)) {
                        m_SelectedIndex = i;
                        break;
                    }
                }
            } else {
                m_Captions = new string[0];
            }
        }

        protected override bool OnInspect()
        {
            int newIndex = EditorGUILayout.Popup(m_SelectedIndex, m_Captions);
            if (newIndex != m_SelectedIndex) {
                m_SelectedIndex = newIndex;
                string key = m_Captions[newIndex];
                List<string> vals;
                if (m_List.TryGetValue(key, out vals)) {
                    for (int i = 0; i < m_Params.Count && i < vals.Count; ++i) {
                        m_Params[i].Value = vals[i];
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsSame(List<string> vals)
        {
            bool ret = true;
            if (vals.Count == m_Params.Count) {
                for (int i = 0; i < vals.Count && i < m_Params.Count; ++i) {
                    if (vals[i] != m_Params[i].InitValue) {
                        ret = false;
                        break;
                    }
                }
            } else {
                ret = false;
            }
            return ret;
        }

        private string[] m_Captions;
        private Dictionary<string, List<string>> m_List = new Dictionary<string, List<string>>();
        private List<ParamInfo> m_Params = new List<ParamInfo>();
        private int m_SelectedIndex;
    }

    internal sealed class ToggleDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            if (null != m_Param.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_Param.SemanticOptions) {
                    var cd = comp as Dsl.CallData;
                    if (null != cd && cd.GetId() == "option") {
                        var key = cd.GetParamId(0);
                        var val = cd.GetParamId(1);
                        keys.Add(key);
                        m_List.Add(key, val);
                    }
                }
                m_Captions = keys.ToArray();
            } else {
                m_Captions = new string[0];
            }
        }

        protected override bool OnInspect()
        {
            bool changed = false;
            string newVal = m_Value;
            foreach (var key in m_Captions) {
                string val;
                if (m_List.TryGetValue(key, out val)) {
                    if (changed) {
                        EditorGUILayout.Toggle(key, false);
                    } else {
                        bool toggle = val == m_Value;
                        if (EditorGUILayout.Toggle(key, toggle)) {
                            if (!toggle) {
                                changed = true;
                                newVal = val;
                            }
                        }
                    }
                }
            }
            if (changed) {
                m_Value = newVal;
                return true;
            }
            return false;
        }

        private string[] m_Captions;
        private Dictionary<string, string> m_List = new Dictionary<string, string>();
    }

    internal sealed class MultipleDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            if (null != m_Param.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_Param.SemanticOptions) {
                    var cd = comp as Dsl.CallData;
                    if (null != cd && cd.GetId() == "option") {
                        var key = cd.GetParamId(0);
                        var val = cd.GetParamId(1);
                        keys.Add(key);
                        m_List.Add(key, val);
                    }
                }
                m_Captions = keys.ToArray();
                m_Values = m_Value.Split('|');
            } else {
                m_Captions = new string[0];
                m_Values = new List<string>();
            }
        }

        protected override bool OnInspect()
        {
            bool changed = false;
            var newVals = new List<string>();
            foreach (var key in m_Captions) {
                string val;
                if (m_List.TryGetValue(key, out val)) {
                    bool toggle = m_Values.IndexOf(val) >= 0;
                    if (EditorGUILayout.Toggle(key, toggle)) {
                        newVals.Add(val);
                        if (!toggle)
                            changed = true;
                    } else if (toggle) {
                        changed = true;
                    }
                }
            }
            if (changed) {
                m_Value = string.Join("|", newVals.ToArray());
                m_Values = newVals;
                return true;
            }
            return false;
        }

        private string[] m_Captions;
        private Dictionary<string, string> m_List = new Dictionary<string, string>();
        private IList<string> m_Values;
    }

    internal sealed class BitMultipleDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            int.TryParse(m_Value, out m_Values);
            if (null != m_Param.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_Param.SemanticOptions) {
                    var cd = comp as Dsl.CallData;
                    if (null != cd && cd.GetId() == "option") {
                        var key = cd.GetParamId(0);
                        var val = cd.GetParamId(1);
                        keys.Add(key);
                        int intVal;
                        int.TryParse(val, out intVal);
                        m_List.Add(key, intVal);
                    }
                }
                m_Captions = keys.ToArray();
            } else {
                m_Captions = new string[0];
            }
        }

        protected override bool OnInspect()
        {
            bool changed = false;
            int newVals = m_Values;
            foreach (var key in m_Captions) {
                int val;
                if (m_List.TryGetValue(key, out val)) {
                    bool toggle = (m_Values & val) != 0;
                    if (EditorGUILayout.Toggle(key, toggle)) {
                        newVals |= val;
                        if (!toggle)
                            changed = true;
                    } else if (toggle) {
                        changed = true;
                    }
                }
            }
            if (changed) {
                m_Values = newVals;
                m_Value = m_Values.ToString();
                return true;
            }
            return false;
        }

        private string[] m_Captions;
        private Dictionary<string, int> m_List = new Dictionary<string, int>();
        private int m_Values;
    }

    internal sealed class MessageDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            m_Index = -1;
            for (int i = 0; i < m_NodeInfo.Outputs.Count; ++i) {
                var output = m_NodeInfo.Outputs[i];
                if (output.Param == m_Param) {
                    m_Index = i;
                    break;
                }
            }
        }

        protected override bool OnInspect()
        {
            if (m_Index >= 0 && m_Index < m_NodeInfo.Outputs.Count) {
                var output = m_NodeInfo.Outputs[m_Index];
                if (null != output.Node) {
                    m_Value = output.Node.Message;
                }
            }
            EditorGUILayout.LabelField(m_Value);
            return false;
        }

        private int m_Index;
    }

    internal sealed class PositionDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            var cd = m_Param.DslInitValue as Dsl.CallData;
            if (null != cd && cd.GetId() == "vector3") {
                float x = float.Parse(cd.GetParamId(0));
                float y = float.Parse(cd.GetParamId(1));
                float z = float.Parse(cd.GetParamId(2));
                m_Pos.Set(x, y, z);
            }
            CreatePosObj(m_Pos);
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.LabelField(m_Value, GUILayout.Width(180));
            if(GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_Param.SceneObject) {
                    SelectObject(m_Param.SceneObject);
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            if (null != m_Param.SceneObject) {
                var newPos = m_Param.SceneObject.transform.position;
                if ((newPos - m_Pos).sqrMagnitude > Mathf.Epsilon) {
                    m_Pos = newPos;
                    m_Value = string.Format("vector3({0},{1},{2})", m_Pos.x, m_Pos.y, m_Pos.z);
                    return true;
                }
            }
            return false;
        }

        private Vector3 m_Pos = Vector3.zero;
    }

    internal sealed class DirDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            var vd = m_Param.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                float v = float.Parse(vd.GetId());
                m_Dir = v;
            }
            CreateDirObj(m_Dir);
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.LabelField(m_Value, GUILayout.Width(180));
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_Param.SceneObject) {
                    SelectObject(m_Param.SceneObject);
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            if (null != m_Param.SceneObject) {
                var newDir = Mathf.Deg2Rad * m_Param.SceneObject.transform.localEulerAngles.y;
                if (Mathf.Abs(newDir - m_Dir) > Mathf.Epsilon) {
                    m_Dir = newDir;
                    m_Value = string.Format("{0}", m_Dir);
                    return true;
                }
            }
            return false;
        }

        private float m_Dir = 0;
    }

    internal sealed class PosDirPosDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            var cd = m_Param.DslInitValue as Dsl.CallData;
            if (null != cd && cd.GetId() == "vector3") {
                float x = float.Parse(cd.GetParamId(0));
                float y = float.Parse(cd.GetParamId(1));
                float z = float.Parse(cd.GetParamId(2));
                m_Pos.Set(x, y, z);
            }
            CreatePosDirObj(true, m_Pos, 0);
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.LabelField(m_Value, GUILayout.Width(180));
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_Param.SceneObject) {
                    SelectObject(m_Param.SceneObject);
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            if (null != m_Param.SceneObject) {
                var newPos = m_Param.SceneObject.transform.position;
                if ((newPos - m_Pos).sqrMagnitude > Mathf.Epsilon) {
                    m_Pos = newPos;
                    m_Value = string.Format("vector3({0},{1},{2})", m_Pos.x, m_Pos.y, m_Pos.z);
                    return true;
                }
            }
            return false;
        }

        private Vector3 m_Pos = Vector3.zero;
    }

    internal sealed class PosDirDirDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            var vd = m_Param.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                float v = float.Parse(vd.GetId());
                m_Dir = v;
            }
            CreatePosDirObj(false, Vector3.zero, m_Dir);
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.LabelField(m_Value, GUILayout.Width(180));
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_Param.SceneObject) {
                    SelectObject(m_Param.SceneObject);
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            if (null != m_Param.SceneObject) {
                var newDir = Mathf.Deg2Rad * m_Param.SceneObject.transform.localEulerAngles.y;
                if (Mathf.Abs(newDir - m_Dir) > Mathf.Epsilon) {
                    m_Dir = newDir;
                    m_Value = string.Format("{0}", m_Dir);
                    return true;
                }
            }
            return false;
        }

        private float m_Dir = 0;
    }

    internal sealed class UnityObjectDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            m_Object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(c_Prefix + m_Value);
            foreach (var comp in m_Param.SemanticOptions) {
                var cd = comp as Dsl.CallData;
                if (null != cd) {
                    var key = cd.GetId();
                    if (key == "unitytype") {
                        var t = cd.GetParamId(0);
                        m_Type = System.Type.GetType("UnityEngine." + t + ",UnityEngine");
                    } else if (key == "path") {
                        m_Path = cd.GetParamId(0);
                    } else if (key == "hasextension") {
                        m_HasExtension = key == "true";
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.TextField(m_Value);
            var newObj = EditorGUILayout.ObjectField(m_Object, m_Type, true);
            if (null != newObj) {
                var path = AssetDatabase.GetAssetPath(newObj);
                if (path.StartsWith(c_Prefix + m_Path)) {
                    var res = path.Substring(c_Prefix.Length + m_Path.Length);
                    if (!m_HasExtension) {
                        var ext = System.IO.Path.GetExtension(res);
                        res = res.Substring(0, res.Length - ext.Length);
                    }
                    if (0 != string.Compare(res, m_Value, true)) {
                        m_Object = newObj;
                        m_Value = res;
                    }
                }
            } else {
                m_Object = null;
                m_Value = string.Empty;
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private System.Type m_Type = null;
        private UnityEngine.Object m_Object = null;
        private string m_Path = "LogicScenes/";
        private bool m_HasExtension = false;
        private const string c_Prefix = "Assets/ResourceAB/";
    }

    internal sealed class FileDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_Param.InitValue;
            foreach (var comp in m_Param.SemanticOptions) {
                var cd = comp as Dsl.CallData;
                if (null != cd) {
                    var key = cd.GetId();
                    if (key == "extension") {
                        m_Ext = cd.GetParamId(0);
                    } else if(key == "path") {
                        m_Path = cd.GetParamId(0);
                    } else if (key == "hasextension") {
                        m_HasExtension = key == "true";
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.TextField(m_Value);
            if (GUILayout.Button("选择", GUILayout.Width(40))) {
                string newFile = EditorUtility.OpenFilePanel(string.Empty, string.Empty, m_Ext);
                if (!string.IsNullOrEmpty(newFile)) {
                    newFile = newFile.Replace('\\', '/');
                    int ix = newFile.IndexOf(c_Prefix + m_Path);
                    var res = newFile;
                    if (ix >= 0) {
                        res = newFile.Substring(ix + c_Prefix.Length + m_Path.Length);
                    }
                    if (!m_HasExtension) {
                        var ext = System.IO.Path.GetExtension(res);
                        res = res.Substring(0, res.Length - ext.Length);
                    }
                    if (0 != string.Compare(res, m_Value, true)) {
                        m_Value = res;
                    }
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private string m_Ext = string.Empty;
        private string m_Path = "LogicScenes/";
        private bool m_HasExtension = false;
        private const string c_Prefix = "Assets/ResourceAB/";
    }

    internal static class ParamDrawerFactory
    {
        internal delegate IParamDrawer CreateParamDrawerDelegation(ParamInfo param, CommandTupleInfo tupleInfo, CommandInfo cmdInfo, NodeInfo nodeInfo);
        internal static void InitDrawers(CommandTupleInfo tupleInfo, CommandInfo cmdInfo, NodeInfo nodeInfo)
        {
            for (int i = 0; i < tupleInfo.Params.Length; ++i) {
                SetParamDrawer(tupleInfo.Params[i], tupleInfo, cmdInfo, nodeInfo);
            }
        }
        internal static void SetParamDrawer(ParamInfo paramInfo, CommandTupleInfo tupleInfo, CommandInfo cmdInfo, NodeInfo nodeInfo)
        {
            string initVal = paramInfo.InitValue;
            if (null != paramInfo.Drawer) {
                initVal = paramInfo.Drawer.GetValue();
            }
            paramInfo.Drawer = CreateParamDrawer(paramInfo, tupleInfo, cmdInfo, nodeInfo);
            if (null != paramInfo.Drawer) {
                paramInfo.Drawer.SetValue(initVal);
            }
        }
        private static IParamDrawer CreateParamDrawer(ParamInfo param, CommandTupleInfo tupleInfo, CommandInfo cmdInfo, NodeInfo nodeInfo)
        {
            string type = param.Type;
            string semantic = param.Semantic;
            if (!string.IsNullOrEmpty(semantic)) {
                CreateParamDrawerDelegation creator;
                if (s_DrawerCreators.TryGetValue(semantic, out creator)) {
                    return creator(param, tupleInfo, cmdInfo, nodeInfo);
                }
            } else if (type == "bool") {
                return CreateParamDrawerImpl<BoolDrawer>(param, tupleInfo, cmdInfo, nodeInfo);
            } else if (type == "int") {
                return CreateParamDrawerImpl<IntDrawer>(param, tupleInfo, cmdInfo, nodeInfo);
            } else if (type == "float") {
                return CreateParamDrawerImpl<FloatDrawer>(param, tupleInfo, cmdInfo, nodeInfo);
            }
            return null;
        }
        private static IParamDrawer CreateParamDrawerImpl<T>(ParamInfo param, CommandTupleInfo tupleInfo, CommandInfo cmdInfo, NodeInfo nodeInfo) where T : AbstractParamDrawer, new()
        {
            var drawer = new T();
            drawer.Init(param, tupleInfo, cmdInfo, nodeInfo);
            return drawer;
        }

        private static Dictionary<string, CreateParamDrawerDelegation> s_DrawerCreators = new Dictionary<string, CreateParamDrawerDelegation> {
            { "intslider",  CreateParamDrawerImpl<IntSliderDrawer>},
            { "floatslider",  CreateParamDrawerImpl<FloatSliderDrawer>},
            { "popup",  CreateParamDrawerImpl<PopupDrawer>},
            { "tuplepopup",  CreateParamDrawerImpl<TuplePopupDrawer>},
            { "toggle",  CreateParamDrawerImpl<ToggleDrawer>},
            { "multiple",  CreateParamDrawerImpl<MultipleDrawer>},
            { "bitmultiple",  CreateParamDrawerImpl<BitMultipleDrawer>},
            { "message",  CreateParamDrawerImpl<MessageDrawer>},
            { "position",  CreateParamDrawerImpl<PositionDrawer>},
            { "dir",  CreateParamDrawerImpl<DirDrawer>},
            { "posdir_pos",  CreateParamDrawerImpl<PosDirPosDrawer>},
            { "posdir_dir",  CreateParamDrawerImpl<PosDirDirDrawer>},
            { "unityobject",  CreateParamDrawerImpl<UnityObjectDrawer>},
            { "file",  CreateParamDrawerImpl<FileDrawer>},
        };
    }
}