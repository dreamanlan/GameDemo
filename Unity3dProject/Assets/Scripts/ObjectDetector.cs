using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLibrary;
using GameLibrary.Story;

public class ObjectDetector : MonoBehaviour
{
    public GameObject[] Objects;
    public string MessageName;
    public float Radius = 3.0f;
    public float Interval = 1.0f;
    public bool Playing = false;

    public void Play()
    {
        Playing = true;
    }
    public void Stop()
    {
        Playing = false;
    }
    public void SetRadius(float radius)
    {
        Radius = radius;
    }

    internal void Start()
    {
        if (null != Objects && Objects.Length > 0) {
            m_KdTree.FullBuild(Objects, 1);
        }
    }

    internal void Update()
    {
        if (!Playing)
            return;
        float curTime = Time.time;
        if (m_LastTime + Interval < curTime) {
            m_LastTime = curTime;

            var playerEntity = SceneSystem.Instance.GetEntityById(SceneSystem.Instance.PlayerId);
            if (null != playerEntity) {
                var pos = playerEntity.GetMovementStateInfo().GetPosition3D();
                m_KdTree.QueryWithAction(pos.x, pos.y, pos.z, Radius, (float distSqr, ObjectKdTree.KdTreeData data) => {
                    if (null != data.Object) {
                        int id = data.Object.GetInstanceID();
                        if (!m_TriggeredSet.Contains(id)) {
                            m_TriggeredSet.Add(id);
                            ClientStorySystem.Instance.SendConcurrentMessage(MessageName, data.Object, data.Object.name, data.Object.tag);
                        }
                    }
                });
            }
        }
    }

    private ObjectKdTree m_KdTree = new ObjectKdTree();
    private HashSet<int> m_TriggeredSet = new HashSet<int>();
    private float m_LastTime = 0;
}
