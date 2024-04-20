using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.AI;

namespace GameLibrary
{
    public enum PredefinedResourceGroup
    {
        Default = 0,
        PlayerSkillEffect,
        PlayerImpactEffect,
        PlayerBuffEffect,
        OtherSkillEffect,
        OtherImpactEffect,
        OtherBuffEffect,
        Sound,
        MaxCount,
    }
    /// <summary>
    /// Resource manager, providing a resource cache reuse mechanism
    /// </summary>
    public class ResourceSystem
    {
        public void InitGroup(int groupCount)
        {
            for (int i = m_GroupedResources.Count; i < groupCount; ++i) {
                m_GroupedResources.Add(new ResourceGroup());
            }
        }
        public void SetGroupMaxCount(int group, int maxCount)
        {
            int ct = m_GroupedResources.Count;
            if (group >= 0 && group < ct) {
                m_GroupedResources[group].MaxCount = maxCount;
            }
        }
        public int GetGroupResourceCount(int group)
        {
            int r = 0;
            if (group >= 0 && group < m_GroupedResources.Count) {
                r = m_GroupedResources[group].Resources.Count;
            }
            return r;
        }
        public void SetVisible(UnityEngine.GameObject obj, bool visible, CharacterController cc)
        {
            obj.SetActive(visible);
        }
        public void Init()
        {
            m_ResPoolRoot = new GameObject("ResPool");
            GameObject.DontDestroyOnLoad(m_ResPoolRoot);
            m_ResPoolRootTransform = m_ResPoolRoot.transform;

            for (int i = 0; i < (int)PredefinedResourceGroup.MaxCount; ++i) {
                InitGroup(i);
            }
        }
        public void PreloadObject(string res, int ct)
        {
            for (int i = 0; i < ct; ++i) {
                PreloadObject(res);
            }
        }
        public void PreloadObject(string res)
        {
            UnityEngine.Object obj = null;
            UnityEngine.Object prefab = GetSharedResource(res);
            if (null != prefab) {
                int resId = prefab.GetInstanceID();
                obj = GameObject.Instantiate(prefab);
                if (null != obj) {
                    FinalizeObject(obj);
                    AddToUnusedResources(resId, obj);
                }
            }
        }
        public void PreloadSharedResource(string res)
        {
            GetSharedResource(res);
        }
        public bool CanNewObject(int group)
        {
            bool r = false;
            if (group >= 0 && group < m_GroupedResources.Count) {
                int ct = m_GroupedResources[group].Resources.Count;
                int maxCt = m_GroupedResources[group].MaxCount;
                r = ct < maxCt;
            }
            return r;
        }
        public UnityEngine.Object NewObject(string res)
        {
            return NewObject(res, 0.0f);
        }
        public UnityEngine.Object NewObject(string res, float timeToRecycle)
        {
            return NewObject(res, timeToRecycle, 0);
        }
        public UnityEngine.Object NewObject(string res, int group)
        {
            return NewObject(res, 0.0f, group);
        }
        public UnityEngine.Object NewObject(string res, float timeToRecycle, int group)
        {
            UnityEngine.Object prefab = GetSharedResource(res);
            return NewObject(prefab, timeToRecycle, group);
        }
        public UnityEngine.Object NewObject(UnityEngine.Object prefab)
        {
            return NewObject(prefab, 0.0f);
        }
        public UnityEngine.Object NewObject(UnityEngine.Object prefab, float timeToRecycle)
        {
            return NewObject(prefab, timeToRecycle, 0);
        }
        public UnityEngine.Object NewObject(UnityEngine.Object prefab, int group)
        {
            return NewObject(prefab, 0.0f, group);
        }
        public UnityEngine.Object NewObject(UnityEngine.Object prefab, float timeToRecycle, int group)
        {
            UnityEngine.Object obj = null;
            if (null != prefab && CanNewObject(group)) {
                float curTime = Time.time;
                float time = timeToRecycle;
                if (timeToRecycle > 0)
                    time += curTime;
                int resId = prefab.GetInstanceID();
                obj = NewFromUnusedResources(resId);
                if (null == obj) {
                    obj = GameObject.Instantiate(prefab);
                }
                if (null != obj) {
                    AddToUsedResources(obj, resId, time, group);
                    InitializeObject(obj);
                }
            }
            return obj;
        }
        public bool RecycleObject(UnityEngine.Object obj)
        {
            bool ret = false;
            if (null != obj) {
                UnityEngine.GameObject gameObject = obj as UnityEngine.GameObject;
                if (null != gameObject) {
                }
                int objId = obj.GetInstanceID();
                UsedResourceInfo resInfo;
                if (m_UsedResources.TryGetValue(objId, out resInfo)) {
                    if (null != resInfo) {
                        FinalizeObject(resInfo.m_Object);
                        RemoveFromUsedResources(objId, resInfo.m_Group);
                        AddToUnusedResources(resInfo.m_ResId, obj);
                        resInfo.Recycle();
                        ret = true;
                    }
                }
            }
            return ret;
        }
        public void Tick()
        {
            float curTime = Time.time;
            for (LinkedListNode<UsedResourceInfo> node = m_UsedResources.FirstNode; null != node; ) {
                UsedResourceInfo resInfo = node.Value;
                if (resInfo.m_RecycleTime > 0 && resInfo.m_RecycleTime < curTime) {
                    node = node.Next;

                    UnityEngine.GameObject gameObject = resInfo.m_Object as UnityEngine.GameObject;
                    if (null != gameObject) {
                    }

                    FinalizeObject(resInfo.m_Object);
                    AddToUnusedResources(resInfo.m_ResId, resInfo.m_Object);
                    RemoveFromUsedResources(resInfo.m_ObjId, resInfo.m_Group);
                    resInfo.Recycle();
                } else {
                    node = node.Next;
                }
            }
        }
        public void CleanupResourcePool()
        {
            for (LinkedListNode<UsedResourceInfo> node = m_UsedResources.FirstNode; null != node; ) {
                UsedResourceInfo resInfo = node.Value;
                node = node.Next;
                var gameObj = resInfo.m_Object as GameObject;
                if (null != gameObj) {
                    gameObj.transform.SetParent(null);
                    Utility.DestroyObject(gameObj);
                }
                RemoveFromUsedResources(resInfo.m_ObjId, -1);
                resInfo.Recycle();
            }
            m_UsedResources.Clear();
            m_UsedResourceInfoPool.Clear();

            for (int i = 0; i < m_GroupedResources.Count; ++i) {
                m_GroupedResources[i].Resources.Clear();
            }

            foreach (var pair in m_UnusedResources) {
                int key = pair.Key;
                var queue = pair.Value;

                while (queue.Count > 0) {
                    var gameObj = queue.Dequeue() as GameObject;
                    if (null != gameObj) {
                        gameObj.transform.SetParent(null);
                        Utility.DestroyObject(gameObj);
                    }
                }
            }
            m_UnusedResources.Clear();

            m_LoadedPrefabs.Clear();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        public UnityEngine.Object GetSharedResource(string res)
        {
            bool notdestory = SharedResourcePath.Contains(res);
            notdestory = false;
            return GetSharedResource(res, notdestory);
        }

        public static HashSet<string> SharedResourcePath = new HashSet<string>() {
        };

        private UnityEngine.Object GetSharedResource(string res, bool notdestoryed)
        {
            UnityEngine.Object obj = null;
            if (string.IsNullOrEmpty(res)) {
                return obj;
            }
            ObjectEx objEx = null;
            if (!m_LoadedPrefabs.TryGetValue(res, out objEx)) {
                obj = Resources.Load(res);
                if (obj != null) {
                    objEx = new ObjectEx();
                    objEx.Obj = obj;
                    objEx.NotDestroyed = notdestoryed;
                    m_LoadedPrefabs.Add(res, objEx);
                } else {
                    UnityEngine.Debug.Log("LoadAsset failed:" + res);
                    m_LoadedPrefabs.Add(res, null);
                }
            } else if(objEx != null){
                obj = objEx.Obj;
            }
            return obj;
        }
        private UnityEngine.Object NewFromUnusedResources(int res)
        {
            UnityEngine.Object obj = null;
            Queue<UnityEngine.Object> queue;
            if (m_UnusedResources.TryGetValue(res, out queue)) {
                if (queue.Count > 0)
                    obj = queue.Dequeue();
            }
            return obj;
        }
        private void AddToUnusedResources(int res, UnityEngine.Object obj)
        {
            Queue<UnityEngine.Object> queue;
            if (m_UnusedResources.TryGetValue(res, out queue)) {
                queue.Enqueue(obj);
            } else {
                queue = new Queue<UnityEngine.Object>();
                queue.Enqueue(obj);
                m_UnusedResources.Add(res, queue);
            }
        }
        private void AddToUsedResources(UnityEngine.Object obj, int resId, float recycleTime, int group)
        {
            int objId = obj.GetInstanceID();
            if (group >= 0 && group < m_GroupedResources.Count) {
                var grp = m_GroupedResources[group];
                if (!grp.Resources.Contains(objId)) {
                    grp.Resources.Add(objId);
                }
            }
            if (!m_UsedResources.Contains(objId)) {
                UsedResourceInfo info = m_UsedResourceInfoPool.Alloc();
                info.m_ObjId = objId;
                info.m_Object = obj;
                info.m_ResId = resId;
                info.m_RecycleTime = recycleTime;
                info.m_Group = group;

                m_UsedResources.AddLast(objId, info);
            }
        }
        private void RemoveFromUsedResources(int objId, int group)
        {
            if (group >= 0 && group < m_GroupedResources.Count) {
                var grp = m_GroupedResources[group];
                grp.Resources.Remove(objId);
            }
            m_UsedResources.Remove(objId);
        }

        private void InitializeObject(UnityEngine.Object obj)
        {
            GameObject gameObj = obj as GameObject;
            if (null != gameObj) {
                gameObj.transform.SetParent(null);
                gameObj.SetActive(true);
                OnActiveChanged(gameObj, true);
            }
        }
        private void FinalizeObject(UnityEngine.Object obj)
        {
            GameObject gameObj = obj as GameObject;
            if (null != gameObj) {
                gameObj.transform.SetParent(m_ResPoolRootTransform);
                OnActiveChanged(gameObj, false);
                gameObj.SetActive(false);
            }
        }
        private void OnActiveChanged(UnityEngine.GameObject obj, bool active)
        {
            if (active) {
                int instId = obj.GetInstanceID();

                ParticleSystem[] pss = obj.GetComponentsInChildren<ParticleSystem>(true);
                for (int i = 0; i < pss.Length; i++) {
                    if (null != pss[i] && pss[i].main.playOnAwake) {
                        //pss[i].Clear(true);
                        pss[i].Play(true);
                    }
                }
                AudioSource[] audioSources = obj.GetComponentsInChildren<AudioSource>(true);
                for (int i = 0; i < audioSources.Length; i++) {
                    if (null != audioSources[i] && audioSources[i].playOnAwake) {
                        audioSources[i].Play();
                    }
                }
                NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
                if (null != agent) {
                    agent.enabled = false;
                }
                AbstractScriptBehavior[] scriptBehaviors = obj.GetComponentsInChildren<AbstractScriptBehavior>(true);
                for (int i = 0; i < scriptBehaviors.Length; ++i) {
                    if (null != scriptBehaviors[i]) {
                        scriptBehaviors[i].ResourceEnabled = true;
                    }
                }
            } else {            
                ParticleSystem[] pss = obj.GetComponentsInChildren<ParticleSystem>(true);
                for (int i = 0; i < pss.Length; i++) {
                    if (null != pss[i] && pss[i].main.playOnAwake) {
                        pss[i].Clear(true);
                        pss[i].Stop(true);
                    }
                }
                AudioSource[] audioSources = obj.GetComponentsInChildren<AudioSource>(true);
                for (int i = 0; i < audioSources.Length; i++) {
                    if (null != audioSources[i] && audioSources[i].playOnAwake) {
                        audioSources[i].Stop();
                    }
                }

                NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
                if (null != agent) {
                    agent.enabled = false;
                }
                AbstractScriptBehavior[] scriptBehaviors = obj.GetComponentsInChildren<AbstractScriptBehavior>(true);
                for (int i = 0; i < scriptBehaviors.Length; ++i) {
                    if (null != scriptBehaviors[i]) {
                        scriptBehaviors[i].ResourceEnabled = false;
                    }
                }
            }
        }

        public static void ForceSetGameObjectLayer(GameObject obj, int layer)
        {
            SetLayer(obj, layer);
        }
        private static void SetLayer(GameObject obj, int layer)
        {
            Transform t = obj.transform;
            if (null != t) {
                SetLayer(t, layer);
            }
        }
        private static void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            for (int i = 0; i < t.childCount; ++i) {
                Transform c = t.GetChild(i);
                if (null != c) {
                    SetLayer(c, layer);
                }
            }
        }

        private ResourceSystem()
        {
            m_UsedResourceInfoPool.Init(256);
        }

        private class UsedResourceInfo : IPoolAllocatedObject<UsedResourceInfo>
        {
            internal int m_ObjId;
            internal UnityEngine.Object m_Object;
            internal int m_ResId;
            internal float m_RecycleTime;
            internal int m_Group;

            internal void Recycle()
            {
                m_Object = null;
                m_Pool.Recycle(this);
            }
            public void InitPool(ObjectPool<UsedResourceInfo> pool)
            {
                m_Pool = pool;
            }
            public UsedResourceInfo Downcast()
            {
                return this;
            }
            private ObjectPool<UsedResourceInfo> m_Pool = null;
        }

        private ObjectPool<UsedResourceInfo> m_UsedResourceInfoPool = new ObjectPool<UsedResourceInfo>();

        public class ObjectEx
        {
            public UnityEngine.Object Obj;
            public bool NotDestroyed = false;
        }
        private class ResourceGroup
        {
            internal int MaxCount = int.MaxValue;
            internal HashSet<int> Resources = new HashSet<int>();
        }
        private List<ResourceGroup> m_GroupedResources = new List<ResourceGroup>();

        private Dictionary<string, ObjectEx> m_LoadedPrefabs = new Dictionary<string, ObjectEx>();
        private List<string> m_WaitDeleteLoadedPrefabEntrys = new List<string>();

        private LinkedListDictionary<int, UsedResourceInfo> m_UsedResources = new LinkedListDictionary<int, UsedResourceInfo>();
        private Dictionary<int, Queue<UnityEngine.Object>> m_UnusedResources = new Dictionary<int, Queue<UnityEngine.Object>>();

        private GameObject m_ResPoolRoot = null;
        private Transform m_ResPoolRootTransform = null;
                
        public static ResourceSystem Instance
        {
            get { return s_Instance; }
        }
        private static ResourceSystem s_Instance = new ResourceSystem();
    }
}
