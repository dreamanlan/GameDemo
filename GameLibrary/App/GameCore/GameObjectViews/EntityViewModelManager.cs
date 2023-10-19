using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary;

namespace GameLibrary
{
    internal class EntityViewModelManager
    {
        internal void Init(SceneSystem scene)
        {
            m_Scene = scene;
        }

        internal void Release()
        {
        }

        internal void Tick()
        {
            foreach (KeyValuePair<int, EntityViewModel> pair in m_EntityViews) {
                EntityViewModel view = pair.Value;
                view.Update();
            }
        }

        internal void CreateEntityView(int objId)
        {
            if (objId <= 0) {
                LogSystem.Error("Error:CreateEntityView objId<=0 !!!");
                Helper.LogCallStack();
                return;
            }
            if (!m_EntityViews.ContainsKey(objId)) {
                EntityInfo obj = m_Scene.EntityManager.GetEntityInfo(objId);
                if (null != obj) {
                    EntityViewModel view = new EntityViewModel();
                    view.Create(obj);
                    m_EntityViews.Add(objId, view);
                    if (null != view.Actor) {
                        if (m_Object2Ids.ContainsKey(view.Actor)) {
                            m_Object2Ids[view.Actor] = objId;
                        } else {
                            m_Object2Ids.Add(view.Actor, objId);
                        }
                    } else {
                        LogSystem.Warn("CreateEntityView:{0}, model:{1}, actor is null", objId, obj.GetModel());
                    }
                }
            }
        }
        internal void DestroyEntityView(int objId)
        {
            EntityViewModel view;
            if (m_EntityViews.TryGetValue(objId, out view)) {
                if (view != null && null != view.Actor) {
                    m_Object2Ids.Remove(view.Actor);
                    view.Destroy();
                }
                m_EntityViews.Remove(objId);
            }
        }

        internal EntityViewModel GetEntityViewById(int objId)
        {
            EntityViewModel view = null;
            m_EntityViews.TryGetValue(objId, out view);
            return view;
        }
        internal EntityViewModel GetEntityViewByUnitId(int unitId)
        {
            EntityViewModel view = null;
            EntityInfo obj = SceneSystem.Instance.GetEntityByUnitId(unitId);
            if (null != obj) {
                m_EntityViews.TryGetValue(obj.GetId(), out view);
            }
            return view;
        }
        
        internal UnityEngine.GameObject GetGameObject(int objId)
        {
            UnityEngine.GameObject obj = null;
            EntityViewModel view = GetEntityViewById(objId);
            if (null != view) {
                obj = view.Actor;
            }
            return obj;
        }
        internal UnityEngine.GameObject GetGameObjectByUnitId(int unitId)
        {
            UnityEngine.GameObject obj = null;
            EntityViewModel view = GetEntityViewByUnitId(unitId);
            if (null != view) {
                obj = view.Actor;
            }
            return obj;
        }
        internal EntityViewModel GetEntityView(UnityEngine.GameObject obj)
        {
            EntityViewModel view = null;
            int id;
            if (m_Object2Ids.TryGetValue(obj, out id)) {
                m_EntityViews.TryGetValue(id, out view);
            }
            return view;
        }
        internal int GetGameObjectUnitId(UnityEngine.GameObject obj)
        {
            int unitId = 0;
            EntityViewModel view = GetEntityView(obj);
            if (null != view) {
                unitId = view.Entity.GetUnitId();
            }
            return unitId;
        }
        internal int GetGameObjectId(UnityEngine.GameObject obj)
        {
            int id;
            m_Object2Ids.TryGetValue(obj, out id);
            return id;
        }
        internal bool ExistGameObject(UnityEngine.GameObject obj)
        {
            return m_Object2Ids.ContainsKey(obj);
        }
        internal bool ExistGameObject(int objId)
        {
            return m_EntityViews.ContainsKey(objId);
        }

        internal void AddActorForRecreate(UnityEngine.GameObject obj, int id)
        {
            m_Object2Ids.Add(obj, id);
        }
        internal void RemoveActorForRecreate(UnityEngine.GameObject obj)
        {
            m_Object2Ids.Remove(obj);
        }
        
        private SceneSystem m_Scene;
        private Dictionary<int, EntityViewModel> m_EntityViews = new Dictionary<int, EntityViewModel>();
        private Dictionary<UnityEngine.GameObject, int> m_Object2Ids = new Dictionary<UnityEngine.GameObject, int>();
    }
}
