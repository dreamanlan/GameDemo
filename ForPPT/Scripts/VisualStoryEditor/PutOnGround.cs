using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PutOnGround))]
public class PutOnGroundInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("自身贴地")) {
            (target as PutOnGround).UpdateAllChildren(false);
        }
        if (GUILayout.Button("子物体贴地")) {
            (target as PutOnGround).UpdateAllChildren(true);
        }
        GUILayout.EndHorizontal();
    }
}
#endif


[ExecuteInEditMode]
public class PutOnGround : MonoBehaviour
{
#if UNITY_EDITOR
    //public bool ForChildren;
    internal void UpdateAllChildren(bool ForChildren)
    {
        if (ForChildren) {
            for (int i = 0; i < transform.childCount; ++i) {
                var child = transform.GetChild(i);
                if (null != child) {
                    Process(child);
                }
            }
        }
        else {
            Process(transform);
        }
    }
    private void Process(Transform trans)
    {
        var pos = trans.position;
        float h = NpcConfig.GetGroundHeight(pos.x, pos.z);
        trans.position = new Vector3(pos.x, h, pos.z);
    }
#endif
}
