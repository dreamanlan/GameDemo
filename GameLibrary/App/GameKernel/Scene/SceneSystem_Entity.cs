using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLibrary.GmCommands;
using GameLibrary.Story;
using StoryScript;

namespace GameLibrary
{
    public partial class SceneSystem
    {
        public void MoveDir(float dir)
        {
            var player = GetEntityById(PlayerId);
            if (null != player) {
                if (player.IsDead()) {
                    return;
                }
                if (player.GetAiStateInfo().CurState == (int)PredefinedAiStateId.MoveCommand) {
                    player.GetAiStateInfo().ChangeToState((int)PredefinedAiStateId.Idle);
                }
            }
            var playerView = GetEntityViewById(PlayerId);
            if (null != playerView) {
                playerView.MoveDir(dir, UnityEngine.Time.deltaTime);
            }
        }
        public void StopMove()
        {
            var player = GetEntityById(PlayerId);
            if (null != player) {
                if (player.IsDead()) {
                    return;
                }
                if (player.GetAiStateInfo().CurState == (int)PredefinedAiStateId.MoveCommand) {
                    return;
                }
            }
            var playerView = GetEntityViewById(PlayerId);
            if (null != playerView) {
                playerView.StopMove();
            }
        }
        public void MoveTo(float x, float y, float z)
        {
            var player = GetEntityById(PlayerId);
            if (null != player) {
                if (player.IsDead()) {
                    return;
                }
            }
            ClientStorySystem.Instance.SendMessage("move_to", x, y, z);
        }
        public int UnitId2ObjId(int unitId)
        {
            int id = 0;
            EntityInfo entity = GetEntityByUnitId(unitId);
            if (null != entity) {
                id = entity.GetId();
            }
            return id;
        }
        public int ObjId2UnitId(int actorId)
        {
            int id = 0;
            EntityInfo entity = GetEntityById(actorId);
            if (null != entity) {
                id = entity.GetUnitId();
            }
            return id;
        }
        public UnityEngine.GameObject GetGameObject(int actorId)
        {
            return m_EntityViewManager.GetGameObject(actorId);
        }
        public UnityEngine.GameObject GetGameObjectByUnitId(int unitId)
        {
            return m_EntityViewManager.GetGameObjectByUnitId(unitId);
        }
        public int GetGameObjectId(UnityEngine.GameObject obj)
        {
            return m_EntityViewManager.GetGameObjectId(obj);
        }
        public int GetGameObjectUnitId(UnityEngine.GameObject obj)
        {
            return m_EntityViewManager.GetGameObjectUnitId(obj);
        }
        public bool ExistGameObject(UnityEngine.GameObject obj)
        {
            return m_EntityViewManager.ExistGameObject(obj);
        }
        public bool ExistGameObject(int objId)
        {
            return m_EntityViewManager.ExistGameObject(objId);
        }
        public EntityInfo GetEntityByGameObject(UnityEngine.GameObject obj)
        {
            int objId = GetGameObjectId(obj);
            return GetEntityById(objId);
        }
        public UnityEngine.GameObject GetGameObjectByEntity(EntityInfo info)
        {
            return GetGameObject(info.GetId());
        }
        public int GetGameObjectType(int id)
        {
            int type = -1;
            EntityInfo entity = GetEntityById(id);
            if (null != entity) {
                type = entity.EntityType;
            }
            return type;
        }
        public float GetGameObjectHp(int id)
        {
            float hp = -1;
            EntityInfo entity = GetEntityById(id);
            if (null != entity) {
                hp = entity.Hp;
            }
            return hp;
        }
        public float GetGameObjectEnergy(int id)
        {
            float np = -1;
            EntityInfo entity = GetEntityById(id);
            if (null != entity) {
                np = entity.Energy;
            }
            return np;
        }
        public CharacterProperty GetGameObjectProperty(int id)
        {
            CharacterProperty prop = null;
            EntityInfo entity = GetEntityById(id);
            if (null != entity) {
                prop = entity.Property;
            }
            return prop;
        }
        public int GetCampId(int actorId)
        {
            var entity = GetEntityById(actorId);
            if (null != entity)
                return entity.GetCampId();
            else
                return 0;
        }
        public bool GetAIEnable(int objID)
        {
            EntityInfo character = GetEntityById(objID);
            if (character != null) {
                return character.GetAIEnable();
            }
            return false;
        }
        public void SetAIEnable(int objID, bool bEnable)
        {
            EntityInfo character = GetEntityById(objID);
            if (character != null) {
                character.SetAIEnable(bEnable);
            }
        }
    }
}
