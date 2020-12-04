using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            var newMsg = EditorGUILayout.TextField(node.Message);
            if (newMsg != node.Message) {
                node.Message = newMsg.Trim();
                node.MessageChanged = true;
            }
            EditorGUILayout.EndHorizontal();

            var selectedIndex = node.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < node.Commands.Count) {
                var cmdInfo = node.Commands[selectedIndex];
                if (null != cmdInfo) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(cmdInfo.Caption, cmdInfo.Caption));
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

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("标注：", GUILayout.Width(80));
                    string newVal = EditorGUILayout.TextField(cmdInfo.Comment);
                    if (newVal != cmdInfo.Comment) {
                        cmdInfo.Comment = newVal.Replace('"', '_');
                        node.CommandCaptions = null;
                    }
                    EditorGUILayout.EndHorizontal();

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
            m_ParamInfo = param;
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
            var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
            if (null != pobj && !pobj.activeSelf) {
                pobj.SetActive(true);
            }
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
                obj.GetComponent<Renderer>().enabled = false;
                GameObjectIconSetter.SetIcon(obj, GameObjectIconSetter.LabelIcon.Green);
                m_ParamInfo.SceneObject = obj;
            }
            return m_ParamInfo.SceneObject;
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
                obj.GetComponent<Renderer>().enabled = false;
                GameObjectIconSetter.SetIcon(obj, GameObjectIconSetter.LabelIcon.Green);
                m_ParamInfo.SceneObject = obj;
            }
            return m_ParamInfo.SceneObject;
        }
        protected GameObject CreatePosDirObj(bool isPos, Vector3 pos, float dir)
        {
            if (null != m_CmdInfo.SceneObjects && m_CmdInfo.SceneObjects.Count > 0) {
                m_ParamInfo.SceneObject = m_CmdInfo.SceneObjects[0];
                var obj = m_ParamInfo.SceneObject;
                if (isPos) {
                    obj.transform.localPosition = pos;
                } else {
                    obj.transform.localEulerAngles = new Vector3(0, Mathf.Rad2Deg * dir, 0);
                }
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
                    obj.GetComponent<Renderer>().enabled = false;
                    GameObjectIconSetter.SetIcon(obj, GameObjectIconSetter.LabelIcon.Purple);
                    m_ParamInfo.SceneObject = obj;
                    if (null == m_CmdInfo.SceneObjects)
                        m_CmdInfo.SceneObjects = new List<GameObject>();
                    m_CmdInfo.SceneObjects.Add(obj);
                }
            }
            return m_ParamInfo.SceneObject;
        }
        protected GameObject CreatePos3dObj(Vector3 pos)
        {
            var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
            if (null != pobj) {
                var id = VisualStoryEditor.Instance.GetNextSceneObjID();
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.hideFlags = HideFlags.DontSave;
                obj.name = "pos3d_" + id;
                obj.transform.SetParent(pobj.transform, false);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                obj.GetComponent<Renderer>().enabled = false;
                GameObjectIconSetter.SetIcon(obj, GameObjectIconSetter.LabelIcon.Green);
                m_ParamInfo.SceneObject = obj;
            }
            return m_ParamInfo.SceneObject;
        }
        protected GameObject CreateDir3dObj(Vector3 dir)
        {
            var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
            if (null != pobj) {
                var id = VisualStoryEditor.Instance.GetNextSceneObjID();
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.hideFlags = HideFlags.DontSave;
                obj.name = "dir3d_" + id;
                obj.transform.SetParent(pobj.transform, false);
                obj.transform.localPosition = Vector3.one * 100;
                obj.transform.localEulerAngles = dir;
                obj.transform.localScale = Vector3.one;
                obj.GetComponent<Renderer>().enabled = false;
                GameObjectIconSetter.SetIcon(obj, GameObjectIconSetter.LabelIcon.Green);
                m_ParamInfo.SceneObject = obj;
            }
            return m_ParamInfo.SceneObject;
        }
        protected GameObject CreatePosDir3dObj(bool isPos, Vector3 pos, Vector3 dir)
        {
            if (null != m_CmdInfo.SceneObjects && m_CmdInfo.SceneObjects.Count > 0) {
                m_ParamInfo.SceneObject = m_CmdInfo.SceneObjects[0];
                var obj = m_ParamInfo.SceneObject;
                if (isPos) {
                    obj.transform.localPosition = pos;
                } else {
                    obj.transform.localEulerAngles = dir;
                }
            } else {
                var pobj = VisualStoryEditor.Instance.EditorNodeInScene;
                if (null != pobj) {
                    var id = VisualStoryEditor.Instance.GetNextSceneObjID();
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    obj.hideFlags = HideFlags.DontSave;
                    obj.name = "posdir3d_" + id;
                    obj.transform.SetParent(pobj.transform, false);
                    if (isPos) {
                        obj.transform.localPosition = pos;
                        obj.transform.localRotation = Quaternion.identity;
                    } else {
                        obj.transform.localPosition = Vector3.one * 100;
                        obj.transform.localEulerAngles = dir;
                    }
                    obj.transform.localScale = Vector3.one;
                    obj.GetComponent<Renderer>().enabled = false;
                    GameObjectIconSetter.SetIcon(obj, GameObjectIconSetter.LabelIcon.Blue);
                    m_ParamInfo.SceneObject = obj;
                    if (null == m_CmdInfo.SceneObjects)
                        m_CmdInfo.SceneObjects = new List<GameObject>();
                    m_CmdInfo.SceneObjects.Add(obj);
                }
            }
            return m_ParamInfo.SceneObject;
        }

        protected void SyncCamera(int v)
        {
            if (!EditorApplication.isPlaying)
                return;
            var cname = m_CmdInfo.Name;
            var pname = m_ParamInfo.Name;

            if (cname == "VC_LookAt") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    var obj = CsLibrary.ClientModule.Instance.GetGameObjectByUnitId(v);
                    if (null != obj) {
                        vcam.LookAt = obj.transform;
                    }
                }
            } else if (cname == "VC_Follow") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    var obj = CsLibrary.ClientModule.Instance.GetGameObjectByUnitId(v);
                    if (null != obj) {
                        vcam.Follow = obj.transform;
                    }
                }
            } else
            if (cname == "VCA_LookAt") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        var obj = CsLibrary.ClientModule.Instance.GetGameObjectByUnitId(v);
                        if (null != obj) {
                            vcam.LookAt = obj.transform;
                        }
                    }
                }
            } else if (cname == "VCA_Follow") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        var obj = CsLibrary.ClientModule.Instance.GetGameObjectByUnitId(v);
                        if (null != obj) {
                            vcam.Follow = obj.transform;
                        }
                    }
                }
            } else if (cname == "SetBrainBlend") {
                if (pname == "blend") {
                    var mc = CameraManager.Instance.mainCamera;
                    if (null != mc) {
                        var brain = mc.GetComponent<Cinemachine.CinemachineBrain>();
                        if (null != brain) {
                            brain.m_DefaultBlend.m_Style = (Cinemachine.CinemachineBlendDefinition.Style)v;
                        }
                    }
                }
            }
        }
        protected void SyncCamera(float v)
        {
            if (!EditorApplication.isPlaying)
                return;
            var cname = m_CmdInfo.Name;
            var pname = m_ParamInfo.Name;

            if (cname == "VC_Damping") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    if (pname == "aim") {
                        var composer = vcam.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
                        if (null != composer) {
                            composer.m_HorizontalDamping = v;
                            composer.m_VerticalDamping = v;
                        }
                    } else {
                        var transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
                        if (null != transposer) {
                            if (pname == "body") {
                                transposer.m_XDamping = v;
                                transposer.m_YDamping = 0;
                                transposer.m_ZDamping = v;
                            }
                            if (pname == "yaw") {
                                transposer.m_YawDamping = v;
                            }
                        }
                    }
                }
            } else if (cname == "VC_ScreenXY") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    var composer = vcam.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
                    if (null != composer) {
                        if (pname == "screenx")
                            composer.m_ScreenX = v;
                        if (pname == "screeny")
                            composer.m_ScreenY = v;
                    }
                }
            } else if (cname == "VC_Lens") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    if (pname == "fov")
                        vcam.m_Lens.FieldOfView = v;
                    if (pname == "nearClip")
                        vcam.m_Lens.NearClipPlane = v;
                    if (pname == "farClip")
                        vcam.m_Lens.FarClipPlane = v;
                    if (pname == "dutch")
                        vcam.m_Lens.Dutch = v;
                }
            } else if (cname == "VCA_Damping") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        if (pname == "aim") {
                            var composer = vcam.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
                            if (null != composer) {
                                composer.m_HorizontalDamping = v;
                                composer.m_VerticalDamping = v;
                            }
                        } else {
                            var transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
                            if (null != transposer) {
                                if (pname == "body") {
                                    transposer.m_XDamping = v;
                                    transposer.m_YDamping = 0;
                                    transposer.m_ZDamping = v;
                                }
                                if (pname == "yaw") {
                                    transposer.m_YawDamping = v;
                                }
                            }
                        }
                    }
                }
            } else if (cname == "VCA_ScreenXY") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        var composer = vcam.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
                        if (null != composer) {
                            if (pname == "screenx")
                                composer.m_ScreenX = v;
                            if (pname == "screeny")
                                composer.m_ScreenY = v;
                        }
                    }
                }
            } else if (cname == "VCA_Lens") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        if (pname == "fov")
                            vcam.m_Lens.FieldOfView = v;
                        if (pname == "nearClip")
                            vcam.m_Lens.NearClipPlane = v;
                        if (pname == "farClip")
                            vcam.m_Lens.FarClipPlane = v;
                        if (pname == "dutch")
                            vcam.m_Lens.Dutch = v;
                    }
                }
            } else if (cname == "MC_Lens") {
                var vcam = CameraManager.Instance.controller.cmController;
                if (null != vcam) {
                    if (pname == "fov")
                        vcam.m_Lens.FieldOfView = v;
                    if (pname == "nearClip")
                        vcam.m_Lens.NearClipPlane = v;
                    if (pname == "farClip")
                        vcam.m_Lens.FarClipPlane = v;
                    if (pname == "dutch")
                        vcam.m_Lens.Dutch = v;
                }
            } else if (cname == "SetBrainBlend") {
                if (pname == "time") {
                    var mc = CameraManager.Instance.mainCamera;
                    if (null != mc) {
                        var brain = mc.GetComponent<Cinemachine.CinemachineBrain>();
                        if (null != brain) {
                            brain.m_DefaultBlend.m_Time = v;
                        }
                    }
                }
            }
        }
        protected void SyncCamera(Vector3 v)
        {
            if (!EditorApplication.isPlaying)
                return;
            var cname = m_CmdInfo.Name;
            var pname = m_ParamInfo.Name;

            if (cname == "VC_SetPosAndDir") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    if (pname == "pos")
                        vcam.transform.position = v;
                    if (pname == "dir")
                        vcam.transform.localEulerAngles = v;
                }
            } else if (cname == "VC_LookAt" || cname == "VC_LookAtObj") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    var composer = vcam.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
                    if (null != composer) {
                        composer.m_TrackedObjectOffset = v;
                    }
                }
            } else if (cname == "VC_Follow" || cname == "VC_FollowObj") {
                var vcam = CameraManager.Instance.storyCamera.cmStoryVC;
                if (null != vcam) {
                    var transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
                    if (null != transposer) {
                        transposer.m_FollowOffset = v;
                    }
                }
            } else if (cname == "VCA_SetPosAndDir") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        if (pname == "pos")
                            vcam.transform.position = v;
                        if (pname == "dir")
                            vcam.transform.localEulerAngles = v;
                    }
                }
            } else if (cname == "VCA_LookAt" || cname == "VCA_LookAtObj") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        var composer = vcam.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
                        if (null != composer) {
                            composer.m_TrackedObjectOffset = v;
                        }
                    }
                }
            } else if (cname == "VCA_Follow" || cname == "VCA_FollowObj") {
                int ix;
                if (int.TryParse(m_TupleInfo.Params[0].Value, out ix)) {
                    var vcam = CameraManager.Instance.storyCamera.GetVC(ix);
                    if (null != vcam) {
                        var transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
                        if (null != transposer) {
                            transposer.m_FollowOffset = v;
                        }
                    }
                }
            } else if (cname == "MC_SetPosAndDir") {
                var vcam = CameraManager.Instance.controller.cmController;
                if (null != vcam) {
                    if (pname == "pos")
                        vcam.transform.position = v;
                    if (pname == "dir")
                        vcam.transform.localEulerAngles = v;
                }
            }
        }

        protected ParamInfo m_ParamInfo;
        protected CommandTupleInfo m_TupleInfo;
        protected CommandInfo m_CmdInfo;
        protected NodeInfo m_NodeInfo;
        protected string m_Value;

        protected static bool TryParseVector3(string val, out Vector3 v)
        {
            v = Vector3.zero;
            if (string.IsNullOrEmpty(val)) {
                return true;
            } else {
                if (m_Parser.LoadFromString(val, string.Empty, msg => { Debug.LogError(msg); }) && m_Parser.DslInfos.Count == 1) {
                    var cd = m_Parser.DslInfos[0] as Dsl.FunctionData;
                    return TryParseVector3(cd, out v);
                }
            }
            return false;
        }
        protected static bool TryParseVector3(Dsl.FunctionData cd, out Vector3 v)
        {
            v = Vector3.zero;
            if (cd.GetId() == "vector3") {
                float x, y, z;
                bool bx = float.TryParse(cd.GetParamId(0), out x);
                bool by = float.TryParse(cd.GetParamId(1), out y);
                bool bz = float.TryParse(cd.GetParamId(2), out z);
                v = new Vector3(x, y, z);
                return bx && by && bz;
            }
            return false;
        }
        protected static long GetFmodEventLength(string musicEvent)
        {
            var inst = FMODUnity.RuntimeManager.CreateInstance(musicEvent);
            if (inst.isValid()) {
                FMOD.Studio.EventDescription desc;        
                inst.getDescription(out desc);
                int length;
                desc.getLength(out length);
                return length;
            }
            return 0;
        }
        protected static string GetScenePath(GameObject obj)
        {
            var tr = obj.transform;
            List<string> list = new List<string>();
            list.Add(tr.name);
            while (null != tr.parent) {
                tr = tr.parent;
                list.Insert(0, tr.name);
            }
            return string.Join("/", list.ToArray());
        }

        protected static Dsl.DslFile m_Parser = new Dsl.DslFile();
    }

    internal sealed class BoolDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var vd = m_ParamInfo.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                bool v;
                if (bool.TryParse(vd.GetId(), out v)) {
                    m_Val = v;
                } else {
                    m_Val = false;
                    m_Value = "false";
                }
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
            m_Value = m_ParamInfo.InitValue;
            var vd = m_ParamInfo.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                int v;
                if(int.TryParse(vd.GetId(), out v)) {
                    m_Val = v;
                } else {
                    m_Val = 0;
                    m_Value = "0";
                }
            }
        }

        protected override bool OnInspect()
        {
            int newVal = EditorGUILayout.IntField(m_Val);
            if (newVal != m_Val) {
                m_Val = newVal;
                m_Value = m_Val.ToString();
                if (m_ParamInfo.IsCamera) {
                    SyncCamera(newVal);
                }
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
            m_Value = m_ParamInfo.InitValue;
            var vd = m_ParamInfo.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                float v;
                if (float.TryParse(vd.GetId(), out v)) {
                    m_Val = v;
                } else {
                    m_Val = 0;
                    m_Value = "0";
                }
            }
        }

        protected override bool OnInspect()
        {
            float newVal = EditorGUILayout.FloatField(m_Val);
            if (Mathf.Abs(newVal - m_Val) > Mathf.Epsilon) {
                m_Val = newVal;
                m_Value = m_Val.ToString("f3");
                if (m_ParamInfo.IsCamera) {
                    SyncCamera(newVal);
                }
                return true;
            }
            return false;
        }

        private float m_Val = 0;
    }

    internal sealed class Vector3Drawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd) {
                if(!TryParseVector3(cd, out m_Val)) {
                    m_Value = "vector3(0,0,0)";
                }
            }
        }

        protected override bool OnInspect()
        {
            var newVal = EditorGUILayout.Vector3Field(string.Empty, m_Val);
            if ((newVal - m_Val).sqrMagnitude > Mathf.Epsilon) {
                m_Val = newVal;
                m_Value = string.Format("vector3({0:f2},{1:f2},{2:f2})", m_Val.x, m_Val.y, m_Val.z);
                if (m_ParamInfo.IsCamera) {
                    SyncCamera(newVal);
                }
                return true;
            }
            return false;
        }

        private Vector3 m_Val = Vector3.zero;
    }

    internal sealed class IntSliderDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            int.TryParse(m_Value, out m_CurValue);
            if (null != m_ParamInfo.SemanticOptions) {
                foreach (var comp in m_ParamInfo.SemanticOptions) {
                    var cd = comp as Dsl.FunctionData;
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
            m_Value = m_ParamInfo.InitValue;
            float.TryParse(m_Value, out m_CurValue);
            if (null != m_ParamInfo.SemanticOptions) {
                foreach (var comp in m_ParamInfo.SemanticOptions) {
                    var cd = comp as Dsl.FunctionData;
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
                m_Value = newVal.ToString("f3");
                return true;
            }
            return false;
        }

        private float m_CurValue;
        private float m_LeftValue;
        private float m_RightValue;
    }

    internal sealed class IntListDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd) {
                m_List.Clear();
                foreach(var p in cd.Params) {
                    int v;
                    if(int.TryParse(p.GetId(), out v)) {
                        m_List.Add(v);
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            EditorGUILayout.LabelField("数量：", GUILayout.Width(60));
            int oldCt = m_List.Count;
            int newCt = EditorGUILayout.IntField(oldCt);
            if (newCt != oldCt) {
                if (newCt > oldCt) {
                    for(int i = oldCt; i < newCt; ++i) {
                        m_List.Add(0);
                    }
                } else if(newCt < oldCt) {
                    for(int i = oldCt - 1; i >= newCt; ++i) {
                        m_List.RemoveAt(i);
                    }
                }
                ret = true;
            }
            for(int i = 0; i < m_List.Count; ++i) {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                var oldVal = m_List[i];
                var newVal = EditorGUILayout.IntField(oldVal);
                if (newVal != oldVal) {
                    m_List[i] = newVal;
                    ret = true;
                }
            }
            if (ret) {
                m_Builder.Length = 0;
                m_Builder.Append('[');
                for(int i = 0; i < m_List.Count; ++i) {
                    if (i > 0)
                        m_Builder.Append(',');
                    m_Builder.Append(m_List[i]);
                }
                m_Builder.Append(']');
                m_Value = m_Builder.ToString();
            }
            return ret;
        }

        private List<int> m_List = new List<int>();
        private StringBuilder m_Builder = new StringBuilder();
    }

    internal sealed class FloatListDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd) {
                m_List.Clear();
                foreach (var p in cd.Params) {
                    float v;
                    if (float.TryParse(p.GetId(), out v)) {
                        m_List.Add(v);
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            EditorGUILayout.LabelField("数量：", GUILayout.Width(60));
            int oldCt = m_List.Count;
            int newCt = EditorGUILayout.IntField(oldCt);
            if (newCt != oldCt) {
                if (newCt > oldCt) {
                    for (int i = oldCt; i < newCt; ++i) {
                        m_List.Add(0);
                    }
                } else if (newCt < oldCt) {
                    for (int i = oldCt - 1; i >= newCt; ++i) {
                        m_List.RemoveAt(i);
                    }
                }
                ret = true;
            }
            for (int i = 0; i < m_List.Count; ++i) {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                var oldVal = m_List[i];
                var newVal = EditorGUILayout.FloatField(oldVal);
                if (Mathf.Abs(newVal - oldVal) > Mathf.Epsilon) {
                    m_List[i] = newVal;
                    ret = true;
                }
            }
            if (ret) {
                m_Builder.Length = 0;
                m_Builder.Append('[');
                for (int i = 0; i < m_List.Count; ++i) {
                    if (i > 0)
                        m_Builder.Append(',');
                    m_Builder.AppendFormat("{0:f3}", m_List[i]);
                }
                m_Builder.Append(']');
                m_Value = m_Builder.ToString();
            }
            return ret;
        }

        private List<float> m_List = new List<float>();
        private StringBuilder m_Builder = new StringBuilder();
    }

    internal sealed class StringListDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd) {
                m_List.Clear();
                foreach (var p in cd.Params) {
                    m_List.Add(p.GetId());
                }
            }
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            EditorGUILayout.LabelField("数量：", GUILayout.Width(60));
            int oldCt = m_List.Count;
            int newCt = EditorGUILayout.IntField(oldCt);
            if (newCt != oldCt) {
                if (newCt > oldCt) {
                    for (int i = oldCt; i < newCt; ++i) {
                        m_List.Add(string.Empty);
                    }
                } else if (newCt < oldCt) {
                    for (int i = oldCt - 1; i >= newCt; ++i) {
                        m_List.RemoveAt(i);
                    }
                }
                ret = true;
            }
            for (int i = 0; i < m_List.Count; ++i) {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                var oldVal = m_List[i];
                var newVal = EditorGUILayout.TextField(oldVal);
                if (newVal.Trim() != oldVal) {
                    m_List[i] = newVal;
                    ret = true;
                }
            }
            if (ret) {
                m_Builder.Length = 0;
                m_Builder.Append('[');
                for (int i = 0; i < m_List.Count; ++i) {
                    if (i > 0)
                        m_Builder.Append(',');
                    m_Builder.AppendFormat("\"{0}\"", m_List[i].Replace('"', '_'));
                }
                m_Builder.Append(']');
                m_Value = m_Builder.ToString();
            }
            return ret;
        }

        private List<string> m_List = new List<string>();
        private StringBuilder m_Builder = new StringBuilder();
    }

    internal sealed class PopupDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            if (null != m_ParamInfo.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_ParamInfo.SemanticOptions) {
                    var cd = comp as Dsl.FunctionData;
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
            m_Value = m_ParamInfo.InitValue;
            if (null != m_ParamInfo.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_ParamInfo.SemanticOptions) {
                    var cd = comp as Dsl.FunctionData;
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
            m_Value = m_ParamInfo.InitValue;
            if (null != m_ParamInfo.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_ParamInfo.SemanticOptions) {
                    var cd = comp as Dsl.FunctionData;
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
            m_Value = m_ParamInfo.InitValue;
            if (null != m_ParamInfo.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_ParamInfo.SemanticOptions) {
                    var cd = comp as Dsl.FunctionData;
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
            m_Value = m_ParamInfo.InitValue;
            int.TryParse(m_Value, out m_Values);
            if (null != m_ParamInfo.SemanticOptions) {
                var keys = new List<string>();
                foreach (var comp in m_ParamInfo.SemanticOptions) {
                    var cd = comp as Dsl.FunctionData;
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
            m_Value = m_ParamInfo.InitValue;
            m_Index = -1;
            for (int i = 0; i < m_NodeInfo.Outputs.Count; ++i) {
                var output = m_NodeInfo.Outputs[i];
                if (output.Param == m_ParamInfo) {
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
            EditorGUILayout.TextField(m_Value);
            return false;
        }

        private int m_Index;
    }

    internal sealed class PositionDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd) {
                if (!TryParseVector3(cd, out m_Pos)) {
                    m_Value = "vector3(0,0,0)";
                }
            }
            CreatePosObj(m_Pos);
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            var newVal = EditorGUILayout.TextField(m_Value, GUILayout.Width(180));
            if (newVal != m_Value) {
                Vector3 newV;
                if (TryParseVector3(newVal, out newV)) {
                    m_Pos = newV;
                    m_Value = newVal;
                    if (null != m_ParamInfo.SceneObject) {
                        m_ParamInfo.SceneObject.transform.position = m_Pos;
                    }
                    ret = true;
                }
            }
            if(GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_ParamInfo.SceneObject) {
                    SelectObject(m_ParamInfo.SceneObject);
                }
            }
            return ret;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newPos = m_ParamInfo.SceneObject.transform.position;
                if ((newPos - m_Pos).sqrMagnitude > Mathf.Epsilon) {
                    m_Pos = newPos;
                    m_Value = string.Format("vector3({0:f2},{1:f2},{2:f2})", m_Pos.x, m_Pos.y, m_Pos.z);
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
            m_Value = m_ParamInfo.InitValue;
            var vd = m_ParamInfo.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                if(!float.TryParse(vd.GetId(), out m_Dir)) {
                    m_Value = "0";
                }
            }
            CreateDirObj(m_Dir);
        }

        protected override bool OnInspect()
        {
            var newVal = EditorGUILayout.FloatField(m_Dir, GUILayout.Width(180));
            if (Mathf.Abs(newVal - m_Dir) > Mathf.Epsilon) {
                m_Dir = newVal;
                m_Value = string.Format("{0:f3}", m_Dir);
                if (null != m_ParamInfo.SceneObject) {
                    m_ParamInfo.SceneObject.transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * m_Dir, 0);
                }
            }
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_ParamInfo.SceneObject) {
                    SelectObject(m_ParamInfo.SceneObject);
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newDir = Mathf.Deg2Rad * m_ParamInfo.SceneObject.transform.localEulerAngles.y;
                if (Mathf.Abs(newDir - m_Dir) > Mathf.Epsilon) {
                    m_Dir = newDir;
                    m_Value = string.Format("{0:f3}", m_Dir);
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
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd && cd.GetId() == "vector3") {
                if (!TryParseVector3(cd, out m_Pos)) {
                    m_Value = "vector3(0,0,0)";
                }
            }
            CreatePosDirObj(true, m_Pos, 0);
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            var newVal = EditorGUILayout.TextField(m_Value, GUILayout.Width(180));
            if (newVal != m_Value) {
                Vector3 newV;
                if (TryParseVector3(newVal, out newV)) {
                    m_Pos = newV;
                    m_Value = newVal;
                    if (null != m_ParamInfo.SceneObject) {
                        m_ParamInfo.SceneObject.transform.position = m_Pos;
                    }
                    ret = true;
                }
            }
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_ParamInfo.SceneObject) {
                    SelectObject(m_ParamInfo.SceneObject);
                }
            }
            return ret;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newPos = m_ParamInfo.SceneObject.transform.position;
                if ((newPos - m_Pos).sqrMagnitude > Mathf.Epsilon) {
                    m_Pos = newPos;
                    m_Value = string.Format("vector3({0:f2},{1:f2},{2:f2})", m_Pos.x, m_Pos.y, m_Pos.z);
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
            m_Value = m_ParamInfo.InitValue;
            var vd = m_ParamInfo.DslInitValue as Dsl.ValueData;
            if (null != vd) {
                if (!float.TryParse(vd.GetId(), out m_Dir)) {
                    m_Value = "0";
                }
            }
            CreatePosDirObj(false, Vector3.zero, m_Dir);
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            var newVal = EditorGUILayout.FloatField(m_Dir, GUILayout.Width(180));
            if (Mathf.Abs(newVal - m_Dir) > Mathf.Epsilon) {
                m_Dir = newVal;
                m_Value = string.Format("{0:f3}", m_Dir);
                if (null != m_ParamInfo.SceneObject) {
                    m_ParamInfo.SceneObject.transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * m_Dir, 0);
                }
                ret = true;
            }
            return ret;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newDir = Mathf.Deg2Rad * m_ParamInfo.SceneObject.transform.localEulerAngles.y;
                if (Mathf.Abs(newDir - m_Dir) > Mathf.Epsilon) {
                    m_Dir = newDir;
                    m_Value = string.Format("{0:f3}", m_Dir);
                    return true;
                }
            }
            return false;
        }

        private float m_Dir = 0;
    }

    internal sealed class Position3dDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd) {
                if (!TryParseVector3(cd, out m_Pos)) {
                    m_Value = "vector3(0,0,0)";
                }
            }
            CreatePos3dObj(m_Pos);
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            var newVal = EditorGUILayout.TextField(m_Value, GUILayout.Width(180));
            if (newVal != m_Value) {
                Vector3 newV;
                if (TryParseVector3(newVal, out newV)) {
                    m_Pos = newV;
                    m_Value = newVal;
                    if (null != m_ParamInfo.SceneObject) {
                        m_ParamInfo.SceneObject.transform.position = m_Pos;
                    }
                    ret = true;
                }
            }
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_ParamInfo.SceneObject) {
                    SelectObject(m_ParamInfo.SceneObject);
                }
            }
            return ret;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newPos = m_ParamInfo.SceneObject.transform.position;
                if ((newPos - m_Pos).sqrMagnitude > Mathf.Epsilon) {
                    m_Pos = newPos;
                    m_Value = string.Format("vector3({0:f2},{1:f2},{2:f2})", m_Pos.x, m_Pos.y, m_Pos.z);
                    return true;
                }
            }
            return false;
        }

        private Vector3 m_Pos = Vector3.zero;
    }

    internal sealed class Dir3dDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd && cd.GetId() == "vector3") {
                if (!TryParseVector3(cd, out m_Dir)) {
                    m_Value = "vector3(0,0,0)";
                }
            }
            CreateDir3dObj(m_Dir);
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            var newVal = EditorGUILayout.TextField(m_Value, GUILayout.Width(180));
            if (newVal != m_Value) {
                Vector3 newV;
                if (TryParseVector3(newVal, out newV)) {
                    m_Dir = newV;
                    m_Value = newVal;
                    if (null != m_ParamInfo.SceneObject) {
                        m_ParamInfo.SceneObject.transform.eulerAngles = m_Dir;
                    }
                    ret = true;
                }
            }
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_ParamInfo.SceneObject) {
                    SelectObject(m_ParamInfo.SceneObject);
                }
            }
            return ret;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newDir = m_ParamInfo.SceneObject.transform.localEulerAngles;
                if ((newDir - m_Dir).sqrMagnitude > Mathf.Epsilon) {
                    m_Dir = newDir;
                    m_Value = string.Format("vector3({0:f2},{1:f2},{2:f2})", m_Dir.x, m_Dir.y, m_Dir.z);
                    if (m_ParamInfo.IsCamera) {
                        SyncCamera(newDir);
                    }
                    return true;
                }
            }
            return false;
        }

        private Vector3 m_Dir = Vector3.zero;
    }

    internal sealed class PosDir3dPosDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd && cd.GetId() == "vector3") {
                if (!TryParseVector3(cd, out m_Pos)) {
                    m_Value = "vector3(0,0,0)";
                }
            }
            CreatePosDir3dObj(true, m_Pos, Vector3.zero);
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            var newVal = EditorGUILayout.TextField(m_Value, GUILayout.Width(180));
            if (newVal != m_Value) {
                Vector3 newV;
                if (TryParseVector3(newVal, out newV)) {
                    m_Pos = newV;
                    m_Value = newVal;
                    if (null != m_ParamInfo.SceneObject) {
                        m_ParamInfo.SceneObject.transform.position = m_Pos;
                    }
                    ret = true;
                }
            }
            if (GUILayout.Button(new GUIContent("激活", "激活场景里的对应物件"), GUILayout.Width(40))) {
                if (null != m_ParamInfo.SceneObject) {
                    SelectObject(m_ParamInfo.SceneObject);
                }
            }
            return ret;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newPos = m_ParamInfo.SceneObject.transform.position;
                if ((newPos - m_Pos).sqrMagnitude > Mathf.Epsilon) {
                    m_Pos = newPos;
                    m_Value = string.Format("vector3({0:f2},{1:f2},{2:f2})", m_Pos.x, m_Pos.y, m_Pos.z);
                    if (m_ParamInfo.IsCamera) {
                        SyncCamera(newPos);
                    }
                    return true;
                }
            }
            return false;
        }

        private Vector3 m_Pos = Vector3.zero;
    }

    internal sealed class PosDir3dDirDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            var cd = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cd && cd.GetId() == "vector3") {
                if (!TryParseVector3(cd, out m_Dir)) {
                    m_Value = "vector3(0,0,0)";
                }
            }
            CreatePosDir3dObj(false, Vector3.zero, m_Dir);
        }

        protected override bool OnInspect()
        {
            bool ret = false;
            var newVal = EditorGUILayout.TextField(m_Value, GUILayout.Width(180));
            if (newVal != m_Value) {
                Vector3 newV;
                if (TryParseVector3(newVal, out newV)) {
                    m_Dir = newV;
                    m_Value = newVal;
                    if (null != m_ParamInfo.SceneObject) {
                        m_ParamInfo.SceneObject.transform.eulerAngles = m_Dir;
                    }
                    ret = true;
                }
            }
            return ret;
        }

        protected override bool OnUpdate()
        {
            if (null != m_ParamInfo.SceneObject) {
                var newDir = m_ParamInfo.SceneObject.transform.localEulerAngles;
                if ((newDir - m_Dir).sqrMagnitude > Mathf.Epsilon) {
                    m_Dir = newDir;
                    m_Value = string.Format("vector3({0:f2},{1:f2},{2:f2})", m_Dir.x, m_Dir.y, m_Dir.z);
                    if (m_ParamInfo.IsCamera) {
                        SyncCamera(newDir);
                    }
                    return true;
                }
            }
            return false;
        }

        private Vector3 m_Dir = Vector3.zero;
    }

    internal sealed class UnityObjectDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            foreach (var comp in m_ParamInfo.SemanticOptions) {
                var cd = comp as Dsl.FunctionData;
                if (null != cd) {
                    var key = cd.GetId();
                    if (key == "unitytype") {
                        var t = cd.GetParamId(0);
                        m_Type = System.Type.GetType("UnityEngine." + t + ",UnityEngine");
                    } else if (key == "path") {
                        m_Path = cd.GetParamId(0);
                    } else if (key == "extension") {
                        m_Ext = cd.GetParamId(0);
                    } else if (key == "hasextension") {
                        m_HasExtension = key == "true";
                    }
                }
            }
            m_Object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(c_Prefix + m_Path + m_Value + m_Ext);
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.TextField(m_Value);
            var newObj = EditorGUILayout.ObjectField(m_Object, m_Type, false);
            if (newObj != m_Object) {
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
                return true;
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private System.Type m_Type = null;
        private UnityEngine.Object m_Object = null;
        private string m_Ext = ".prefab";
        private string m_Path = "LogicScenes/";
        private bool m_HasExtension = false;
        private const string c_Prefix = "Assets/ResourceAB/";
    }

    internal sealed class FileDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            foreach (var comp in m_ParamInfo.SemanticOptions) {
                var cd = comp as Dsl.FunctionData;
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
                        return true;
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

    internal sealed class TimelineTrackBindingDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            foreach (var comp in m_ParamInfo.SemanticOptions) {
                var cd = comp as Dsl.FunctionData;
                if (null != cd) {
                    var key = cd.GetId();
                    if (key == "prefab") {
                        m_Prefab = cd.GetParamId(0);
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            if (null == m_Names) {
                EditorGUILayout.TextField(m_Value);
            } else {
                int ix = System.Array.IndexOf<string>(m_Values, m_Value);
                ix = EditorGUILayout.Popup(ix, m_Names);
                if (ix >= 0 && ix < m_Values.Length) {
                    m_Value = m_Values[ix];
                    return true;
                }
            }
            if (GUILayout.Button("刷新")) {
                string val = string.Empty;
                foreach(var p in m_TupleInfo.Params) {
                    if (p.Name == m_Prefab) {
                        val = p.Value;
                    }
                }
                if (!string.IsNullOrEmpty(val)) {
                    var newObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(c_Prefix + val + c_Ext);
                    if (null != newObj) {
                        var gobj = newObj as GameObject;
                        if (null != gobj) {
                            var list = new List<string>();
                            var list2 = new List<string>();
                            var director = gobj.GetComponentInChildren<UnityEngine.Playables.PlayableDirector>();
                            foreach (var output in director.playableAsset.outputs) {
                                var binding = director.GetGenericBinding(output.sourceObject);
                                if (null != output.outputTargetType) {
                                    list.Add(output.streamName + " val:" + (null != binding ? binding.name : string.Empty));
                                    list2.Add(output.streamName);
                                }
                            }
                            var asset = director.playableAsset as UnityEngine.Timeline.TimelineAsset;
                            if (null != asset) {
                                foreach(var t in asset.GetRootTracks()) {
                                    var playableTrack = t as UnityEngine.Timeline.PlayableTrack;
                                    if (null != playableTrack) {
                                        bool isValid = false;
                                        GameObject binding = null;
                                        foreach (var clip in t.GetClips()) {
                                            var clipAsset = clip.asset as PlayableMixerPlayableAsset;
                                            if (null != clipAsset) {
                                                bool idValid;
                                                binding = director.GetReferenceValue(clipAsset.AnimatorNode.exposedName, out idValid) as GameObject;
                                                isValid = true;
                                            }
                                        }
                                        if (isValid) {
                                            list.Add(playableTrack.name + " val:" + (null != binding ? binding.name : string.Empty));
                                            list2.Add(playableTrack.name);
                                        }
                                    }
                                }
                            }
                            m_Names = list.ToArray();
                            m_Values = list2.ToArray();
                        }
                    }
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private string[] m_Names;
        private string[] m_Values;
        private string m_Prefab = string.Empty;
        private const string c_Prefix = "Assets/ResourceAB/LogicScenes/";
        private const string c_Ext = ".prefab";
    }

    internal sealed class TimelineNodeDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            foreach (var comp in m_ParamInfo.SemanticOptions) {
                var cd = comp as Dsl.FunctionData;
                if (null != cd) {
                    var key = cd.GetId();
                    if (key == "prefab") {
                        m_Prefab = cd.GetParamId(0);
                    }
                }
            }
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.TextField(m_Value);
            if (GUILayout.Button("刷新")) {
                string val = string.Empty;
                foreach (var p in m_TupleInfo.Params) {
                    if (p.Name == m_Prefab) {
                        val = p.Value;
                    }
                }
                if (!string.IsNullOrEmpty(val)) {
                    var newObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(c_Prefix + val + c_Ext);
                    if (null != newObj) {
                        var gobj = newObj as GameObject;
                        if (null != gobj) {
                            var director = gobj.GetComponentInChildren<UnityEngine.Playables.PlayableDirector>();
                            m_Value = director.gameObject.name;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private string m_Prefab = string.Empty;
        private const string c_Prefix = "Assets/ResourceAB/LogicScenes/";
        private const string c_Ext = ".prefab";
    }

    internal sealed class TimelineEndMsgDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.TextField(m_Value);
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

    }
    internal sealed class TimelinePrefabDrawer : AbstractParamDrawer
    {
        static List<string> timelineEndMsgblackList = new List<string> {
            "show_story_text", "hide_story_text",
            "show_chapter", "slide_story_text",
            "screen_fade_in", "screen_fade_out",
            "Hide_Player"
            };
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            m_Object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(c_Prefix + m_Path + m_Value + c_Ext);
            foreach (var comp in m_ParamInfo.SemanticOptions) {
                var cd = comp as Dsl.FunctionData;
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
            Object newObj = null;
            if (null == m_Type) {
                if (GUILayout.Button("选择", GUILayout.Width(40))) {
                    string newFile = EditorUtility.OpenFilePanel(string.Empty, string.Empty, c_Ext.Substring(1));
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
                            m_Object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(c_Prefix + m_Path + m_Value + c_Ext);
                            m_Changed = true;
                        }
                    }
                }
            } else {
                newObj = EditorGUILayout.ObjectField(m_Object, m_Type, false);
                if (newObj != m_Object) {
                    if (null != newObj) {
                        var path = AssetDatabase.GetAssetPath(newObj);
                        if (path.StartsWith(c_Prefix + m_Path)) {
                            var res = path.Substring(c_Prefix.Length + m_Path.Length);
                            if (!m_HasExtension) {
                                var ext = System.IO.Path.GetExtension(res);
                                res = res.Substring(0, res.Length - ext.Length);
                            }
                            if (0 != string.Compare(res, m_Value, true)) {
                                m_Value = res;
                                m_Object = newObj;
                                m_Changed = true;
                            }
                        }
                    } else {
                        m_Value = string.Empty;
                        m_Object = null;
                        m_Changed = true;
                    }
                }
            }
            if (null != m_Object) {
                ShowObject(m_Object);
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private void ShowObject(Object obj)
        {
          
            var gobj = obj as GameObject;
            if (null != gobj) {
                var director = gobj.GetComponentInChildren<UnityEngine.Playables.PlayableDirector>();
                var timeline = director.playableAsset as UnityEngine.Timeline.TimelineAsset;
                if (null != timeline) {
                    if (m_Changed)
                        m_Messages.Clear();
                    foreach (var track in timeline.GetRootTracks()) {
                        foreach (var clip in track.GetClips()) {
                            var storyMsgAsset = clip.asset as StoryMessagePlayableAsset;
                            if (null != storyMsgAsset) {
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.TextField(storyMsgAsset.MessageId);
                                if (m_Changed)
                                    m_Messages.Add(storyMsgAsset.MessageId);
                            }
                        }
                    }
                    if (m_Changed) {
                        m_Changed = false;
                        m_NodeInfo.RefreshFixedOutputs(m_ParamInfo, m_TupleInfo, m_CmdInfo, m_Messages);
                        foreach (var p in m_TupleInfo.Params){
                            if (p.Name == "timelineEndMsg")
                            {
                                if (m_Messages.Count > 0)
                                {
                                    for (int i = m_Messages.Count - 1; i >= 0; i--)
                                    {
                                        var val = m_Messages[i];
                                        if (timelineEndMsgblackList.Find((s) => s == val) == null)
                                        {
                                            p.Value = val;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool m_Changed = true;
        private List<string> m_Messages = new List<string>();
        private System.Type m_Type = null;
        private UnityEngine.Object m_Object = null;
        private string m_Path = "LogicScenes/";
        private bool m_HasExtension = false;
        private const string c_Prefix = "Assets/ResourceAB/";
        private const string c_Ext = ".prefab";
    }

    internal sealed class SceneObjectDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            foreach (var comp in m_ParamInfo.SemanticOptions) {
                var cd = comp as Dsl.FunctionData;
                if (null != cd) {
                    var key = cd.GetId();
                    if (key == "unitytype") {
                        var t = cd.GetParamId(0);
                        m_Type = System.Type.GetType("UnityEngine." + t + ",UnityEngine");
                    }
                }
            }
            m_Object = GameObject.Find(m_Value);
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.TextField(m_Value);
            var newObj = EditorGUILayout.ObjectField(m_Object, m_Type, true);
            if (newObj != m_Object) {
                if (null != newObj) {
                    m_Object = newObj;
                    var go = newObj as GameObject;
                    var mb = newObj as Component;
                    if (null != go) {
                        m_Value = GetScenePath(go);
                    } else if (null != mb) {
                        m_Value = GetScenePath(mb.gameObject);
                    }
                } else {
                    m_Object = null;
                    m_Value = string.Empty;
                }
                return true;
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private System.Type m_Type = null;
        private UnityEngine.Object m_Object = null;
    }

    internal sealed class SceneSplineDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            foreach (var comp in m_ParamInfo.SemanticOptions) {
                var cd = comp as Dsl.FunctionData;
                if (null != cd) {
                    var key = cd.GetId();
                    if (key == "unitytype") {
                        var t = cd.GetParamId(0);
                        m_Type = System.Type.GetType("UnityEngine." + t + ",UnityEngine");
                    }
                }
            }
            var cdv = m_ParamInfo.DslInitValue as Dsl.FunctionData;
            if (null != cdv) {
                m_Path = cdv.GetParamId(0);
                m_Object = GameObject.Find(m_Path);
            }
        }

        protected override bool OnInspect()
        {
            EditorGUILayout.TextField(m_Value);
            var newObj = EditorGUILayout.ObjectField(m_Object, m_Type, true);
            if (newObj != m_Object) {
                if (null != newObj) {
                    m_Object = newObj;
                    var go = newObj as GameObject;
                    var mb = newObj as Component;
                    if (null != go) {
                        m_Path = GetScenePath(go);
                        m_Value = string.Format("getsplinepoints(\"{0}\")", m_Path);
                    } else if (null != mb) {
                        m_Path = GetScenePath(mb.gameObject);
                        m_Value = string.Format("getsplinepoints(\"{0}\")", m_Path);
                    }
                } else {
                    m_Object = null;
                    m_Value = string.Empty;
                }
                return true;
            }
            return false;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private System.Type m_Type = typeof(FluffyUnderware.Curvy.CurvySpline);
        private UnityEngine.Object m_Object = null;
        private string m_Path = string.Empty;
    }

    internal sealed class FmodEventDrawer : AbstractParamDrawer
    {
        protected override void OnInit()
        {
            m_Value = m_ParamInfo.InitValue;
            if (!string.IsNullOrEmpty(m_Value) && EditorApplication.isPlaying) {
                m_Time = GetFmodEventLength(m_Value);
            } else {
                m_Time = 0;
            }
        }

        protected override bool OnInspect()
        {
            bool isPlaying = EditorApplication.isPlaying;
            bool isChanged = false;
            Utils.DrawFMODEventPropertyLayout(m_Value, (newStr) => {
                m_Value = newStr;
                isChanged = true;
                if (!string.IsNullOrEmpty(m_Value) && isPlaying) {
                    m_Time = GetFmodEventLength(m_Value);
                } else {
                    m_Time = 0;
                }
            });
            EditorGUILayout.LongField(m_Time, GUILayout.Width(40));
            return isChanged;
        }

        protected override bool OnUpdate()
        {
            return false;
        }

        private long m_Time = 0;
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
            } else if (type == "vector3") {
                return CreateParamDrawerImpl<Vector3Drawer>(param, tupleInfo, cmdInfo, nodeInfo);
            } else if (type == "intlist") {
                return CreateParamDrawerImpl<IntListDrawer>(param, tupleInfo, cmdInfo, nodeInfo);
            } else if (type == "floatlist") {
                return CreateParamDrawerImpl<FloatListDrawer>(param, tupleInfo, cmdInfo, nodeInfo);
            } else if (type == "stringlist") {
                return CreateParamDrawerImpl<StringListDrawer>(param, tupleInfo, cmdInfo, nodeInfo);
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
            { "position3d",  CreateParamDrawerImpl<Position3dDrawer>},
            { "dir3d",  CreateParamDrawerImpl<Dir3dDrawer>},
            { "posdir3d_pos",  CreateParamDrawerImpl<PosDir3dPosDrawer>},
            { "posdir3d_dir",  CreateParamDrawerImpl<PosDir3dDirDrawer>},
            { "unityobject",  CreateParamDrawerImpl<UnityObjectDrawer>},
            { "file",  CreateParamDrawerImpl<FileDrawer>},
            { "timeline_track_binding",  CreateParamDrawerImpl<TimelineTrackBindingDrawer>},
            { "timeline_node",  CreateParamDrawerImpl<TimelineNodeDrawer>},
            { "timeline_prefab",  CreateParamDrawerImpl<TimelinePrefabDrawer>},
            { "timeline_end_msg",  CreateParamDrawerImpl<TimelineEndMsgDrawer>},
            { "sceneobject",  CreateParamDrawerImpl<SceneObjectDrawer>},
            { "spline",  CreateParamDrawerImpl<SceneSplineDrawer>},
            { "fmod_event",  CreateParamDrawerImpl<FmodEventDrawer>},
        };
    }
}