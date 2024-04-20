using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GameLibrary;
using GameLibrary.GmCommands;
using GameLibrary.Story;
using System.Timers;
using UnityEngine;
using System.Collections;
using StoryScript;

namespace GameLibrary
{
    public partial class SceneSystem
    {
        #region 订阅的Event处理
        private void ResetDsl()
        {
            try {
                ResetGmCode();
                
                ClientStorySystem.Instance.Reset();
                ClientStorySystem.Instance.ClearStoryInstancePool();
                StoryScript.StoryConfigManager.Instance.Clear();
                ClientStorySystem.Instance.LoadSceneStories(HomePath.GetAbsolutePath("Dsl/Story/Scene/scene.dsl"),
                    HomePath.GetAbsolutePath("Dsl/Story/Scene/story.dsl"));
                ClientStorySystem.Instance.StartStory("scene_main");

                LogSystem.Warn("ResetDsl finish.");
            } catch (Exception ex) {
                LogSystem.Error("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private void ExecScript(string scriptFile)
        {
            try {
                if (string.IsNullOrEmpty(scriptFile)) {
                    scriptFile = "Dsl/gm.dsl";
                }
                m_LocalGmFile = scriptFile;
                RunLocalGmFile();

                LogSystem.Warn("ExecScript {0} finish.", scriptFile);
            } catch (Exception ex) {
                LogSystem.Error("ExecScript exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private void ExecCommand(string cmd)
        {
            try {
                ClientGmStorySystem.Instance.Reset();
                ClientGmStorySystem.Instance.LoadStoryText(Encoding.UTF8.GetBytes("script(main){onmessage(\"start\"){" + cmd + "}}"));
                ClientGmStorySystem.Instance.StartStory("main");
                LogSystem.Warn("ExecCommand {0} finish.", cmd);
            }
            catch (Exception ex) {
                LogSystem.Error("ExecCommand exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private void ResetGmCode()
        {
            ClientGmStorySystem.Instance.Reset();
            m_LocalGmFile = "";
        }
        private void RunLocalGmFile()
        {
            if (!string.IsNullOrEmpty(m_LocalGmFile)) {
                if (File.Exists(m_LocalGmFile)) {
                    ClientGmStorySystem.Instance.Reset();
                    ClientGmStorySystem.Instance.LoadStory(m_LocalGmFile);
                    ClientGmStorySystem.Instance.StartStory("main");
                } else {
                    var bytes = Utility.LoadFileFromStreamingAssets(m_LocalGmFile);
                    if (null != bytes) {
                        ClientGmStorySystem.Instance.Reset();
                        ClientGmStorySystem.Instance.LoadStoryText(bytes);
                        ClientGmStorySystem.Instance.StartStory("main");
                    } else {
                        m_LocalGmFile = "";
                    }
                }
            }
        }
        #endregion

        public SortedList<string, string> CommandDocs
        {
            get { return m_CommandDocs; }
        }
        public SortedList<string, string> FunctionDocs
        {
            get { return m_FunctionDocs; }
        }
        public BlackBoard BlackBoard
        {
            get { return m_BlackBoard; }
        }
        public void Init()
        {
            ResourceSystem.Instance.Init();
            
            ResourceSystem.Instance.SetGroupMaxCount((int)PredefinedResourceGroup.PlayerSkillEffect, 10);
            ResourceSystem.Instance.SetGroupMaxCount((int)PredefinedResourceGroup.PlayerImpactEffect, 10);
            ResourceSystem.Instance.SetGroupMaxCount((int)PredefinedResourceGroup.PlayerBuffEffect, 10);
            ResourceSystem.Instance.SetGroupMaxCount((int)PredefinedResourceGroup.OtherSkillEffect, 10);
            ResourceSystem.Instance.SetGroupMaxCount((int)PredefinedResourceGroup.OtherImpactEffect, 10);
            ResourceSystem.Instance.SetGroupMaxCount((int)PredefinedResourceGroup.OtherBuffEffect, 10);

            ClientStorySystem.ThreadInitMask();
            ClientGmStorySystem.Instance.Init();
            ClientStorySystem.Instance.Init();

            m_EntityViewManager.Init(this);            
            Utility.EventSystem.Subscribe("gm_resetdsl", "gm", ResetDsl);
            Utility.EventSystem.Subscribe<string>("gm_execscript", "gm", ExecScript);
            Utility.EventSystem.Subscribe<string>("gm_execcommand", "gm", ExecCommand);
            Utility.EventSystem.Subscribe("gm_resetgmcode", "gm", ResetGmCode);

            ClientGmStorySystem.Instance.Reset();
            ClientStorySystem.Instance.Reset();
            ClientStorySystem.Instance.ClearStoryInstancePool();
            StoryScript.StoryConfigManager.Instance.Clear();
            ClientStorySystem.Instance.LoadSceneStories("Dsl/Story/Scene/scene.dsl", "Dsl/Story/Scene/story.dsl");
            ClientStorySystem.Instance.StartStory("scene_main");

            m_BlackBoard.Reset();

            m_CommandDocs = StoryScript.StoryCommandManager.Instance.GenCommandDocs();
            m_FunctionDocs = StoryScript.StoryFunctionManager.Instance.GenFunctionDocs();
        }
        public void Release()
        {
        }
        public void ChangeScene(string name)
        {
            Reset();
            Utility.SendScriptMessage("LoadScene", name);
        }
        public void OnSceneLoaded(string name)
        {
            ClientStorySystem.Instance.SendMessage("scene_loaded", name);
        }
        public void Tick()
        {
            long curTime = TimeUtility.GetLocalMilliseconds();
            if (m_LastTickTime == 0) {
                m_LastTickTime = curTime;
            }
            long deltaTime = curTime - m_LastTickTime;
            m_LastTickTime = curTime;

            m_AsyncActionProcessor.HandleActions(100);
            
            m_EntityKdTree.BeginBuild(m_EntityManager.Entities.Count);
            for (LinkedListNode<EntityInfo> linkNode = m_EntityManager.Entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                EntityInfo info = linkNode.Value;
                m_EntityKdTree.AddObjForBuild(info);
            }
            m_EntityKdTree.EndBuild();

            TickEntities(curTime, deltaTime);
            m_EntityViewManager.Tick();
            TickAi(curTime, deltaTime);

            ClientStorySystem.Instance.Tick();
            ResourceSystem.Instance.Tick();
        }
        public void GmTick()
        {
            ClientGmStorySystem.Instance.Tick();
        }
        public void QueueAction(MyAction action)
        {
            m_AsyncActionProcessor.QueueAction(action);
        }
        public void HighlightPrompt(string info)
        {
            Utility.EventSystem.Publish("ui_highlight_prompt", "ui", info);
        }
        public CharacterRelation GetRelation(int one, int other)
        {
            EntityViewModel view1 = GetEntityViewById(one);
            EntityViewModel view2 = GetEntityViewById(other);
            if (null == view1 || null == view1.Entity || null == view2 || null == view2.Entity)
                return CharacterRelation.RELATION_INVALID;
            else
                return EntityInfo.GetRelation(view1.Entity, view2.Entity);
        }
        public CharacterRelation GetRelationForGameObject(UnityEngine.GameObject one, UnityEngine.GameObject other)
        {
            EntityViewModel view1 = GetEntityView(one);
            EntityViewModel view2 = GetEntityView(other);
            if (null == view1 || null == view1.Entity || null == view2 || null == view2.Entity)
                return CharacterRelation.RELATION_INVALID;
            else
                return EntityInfo.GetRelation(view1.Entity, view2.Entity);
        }
        
        public EntityKdTree EntityKdTree
        {
            get { return m_EntityKdTree; }
        }
        public int PlayerId
        {
            get { return m_PlayerId; }
            set { m_PlayerId = value; }
        }
        public List<GameObject> GameObjectsFromDsl
        {
            get { return m_GameObjectsFromDsl; }
        }
        public List<string> LoadedUiPrefabs
        {
            get { return m_LoadedUiPrefabs; }
        }
        public int GetNpcCountByRelationWithCamp(int campId, CharacterRelation relation)
        {
            int ct = 0;
            for (LinkedListNode<EntityInfo> linkNode = m_EntityManager.Entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                EntityInfo info = linkNode.Value;
                if (null != info && !info.IsDead() && EntityInfo.GetRelation(campId, info.GetCampId()) == relation) {
                    ++ct;
                }
            }
            return ct;
        }
        public int GetNpcCountByCamp(int campId)
        {
            int ct = 0;
            for (LinkedListNode<EntityInfo> linkNode = m_EntityManager.Entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                EntityInfo info = linkNode.Value;
                if (null != info && !info.IsDead() && info.GetCampId() == campId) {
                    ++ct;
                }
            }
            return ct;
        }
        public int GetDyingNpcCountByRelationWithCamp(int campId, CharacterRelation relation)
        {
            int ct = 0;
            for (LinkedListNode<EntityInfo> linkNode = m_EntityManager.Entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                EntityInfo info = linkNode.Value;
                if (null != info && info.IsDead() && info.DeadTime != 0 && EntityInfo.GetRelation(campId, info.GetCampId()) == relation) {
                    ++ct;
                }
            }
            return ct;
        }
        public int GetDyingNpcCountByCamp(int campId)
        {
            int ct = 0;
            for (LinkedListNode<EntityInfo> linkNode = m_EntityManager.Entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                EntityInfo info = linkNode.Value;
                if (null != info && info.IsDead() && info.DeadTime != 0 && info.GetCampId() == campId) {
                    ++ct;
                }
            }
            return ct;
        }
        public EntityInfo GetEntityById(int id)
        {
            EntityInfo obj = null;
            if (null != m_EntityManager)
                obj = m_EntityManager.GetEntityInfo(id);
            return obj;
        }
        public EntityInfo GetEntityByUnitId(int unitId)
        {
            EntityInfo obj = null;
            if (null != m_EntityManager)
                obj = m_EntityManager.GetEntityInfoByUnitId(unitId);
            return obj;
        }
        public void DestroyEntityById(int id)
        {
            if (m_EntityManager.Entities.Contains(id)) {
                m_EntityManager.RemoveEntity(id);
            }
        }
        public EntityViewModel GetEntityViewById(int id)
        {
            EntityViewModel view = null;
            if (null != m_EntityViewManager)
                view = m_EntityViewManager.GetEntityViewById(id);
            return view;
        }
        public EntityViewModel GetEntityViewByUnitId(int unitId)
        {
            EntityViewModel view = null;
            if (null != m_EntityViewManager)
                view = m_EntityViewManager.GetEntityViewByUnitId(unitId);
            return view;
        }
        public EntityViewModel GetEntityView(GameObject obj)
        {
            EntityViewModel view = null;
            if (null != m_EntityViewManager)
                view = m_EntityViewManager.GetEntityView(obj);
            return view;
        }
        public void DestroyEntityViewById(int id)
        {
            m_EntityViewManager.DestroyEntityView(id);
        }
        public EntityInfo FindEntityByRange(EntityTypeEnum type, float x, float y, float range)
        {
            EntityInfo obj = null;
            if (null != m_EntityManager)
                obj = m_EntityManager.FindEntityByRange(type, x, y, range);
            return obj;
        }
        public bool FindEntitiesByRange(EntityTypeEnum type, float x, float y, float range, List<EntityInfo> list)
        {
            bool ret = false;
            if (null != m_EntityManager) {
                ret = m_EntityManager.FindEntitiesByRange(type, x, y, range, list);
            }
            return ret;
        }
        public bool FindAllEntitiesByRange(float x, float y, float range, List<EntityInfo> list)
        {
            m_EntityKdTree.QueryWithAction(x, 0, y, range, (float distSqr, EntityKdTree.KdTreeData obj) => {
                list.Add(obj.Object);
            });
            return list.Count > 0;
        }
        public int CreateEntity(int unitId, float x, float y, float z, float dir, string model, EntityTypeEnum entityType)
        {
            int objId = 0;
            EntityInfo npc = m_EntityManager.AddEntity(unitId, model, entityType, string.Empty);
            if (null != npc) {
                npc.GetMovementStateInfo().SetPosition(x, y, z);
                npc.GetMovementStateInfo().SetFaceDir(dir);
                m_EntityViewManager.CreateEntityView(npc.GetId());
                objId = npc.GetId();
                OnCreateEntity(npc);
            }
            return objId;
        }
        public int CreateEntityWithAi(int unitId, float x, float y, float z, float dir, string model, EntityTypeEnum entityType, string ai, params string[] aiParams)
        {
            int objId = 0;
            EntityInfo npc = m_EntityManager.AddEntity(unitId, model, entityType, ai, aiParams);
            if (null != npc) {
                npc.GetMovementStateInfo().SetPosition(x, y, z);
                npc.GetMovementStateInfo().SetFaceDir(dir);
                m_EntityViewManager.CreateEntityView(npc.GetId());
                objId = npc.GetId();
                OnCreateEntity(npc);
            }
            return objId;
        }
        public void DestroyEntity(EntityInfo ni)
        {
            OnDestroyEntity(ni);
            ni.DeadTime = 0;
            m_EntityViewManager.DestroyEntityView(ni.GetId());
            DestroyEntityById(ni.GetId());
        }
        public void UpdateAiLogic(EntityInfo entity)
        {
            OnAiDestroy(entity);
            OnAiInitDslLogic(entity);
        }

        internal EntityManager EntityManager
        {
            get { return m_EntityManager; }
        }
        internal EntityViewModelManager EntityViewManager
        {
            get { return m_EntityViewManager; }
        }

        private void Reset()
        {
            for (LinkedListNode<EntityInfo> linkNode = m_EntityManager.Entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                EntityInfo info = linkNode.Value;
                int id = info.GetId();
                m_EntityViewManager.DestroyEntityView(id);
            }
            m_EntityManager.Reset();
            m_EntityKdTree.Clear();

            ResourceSystem.Instance.CleanupResourcePool();
        }
        private void TickEntities(long curTime, long deltaTime)
        {
            m_DeletedEntities.Clear();
            for (LinkedListNode<EntityInfo> linkNode = m_EntityManager.Entities.FirstNode; null != linkNode; linkNode = linkNode.Next) {
                EntityInfo info = linkNode.Value;

                // clear AI target。
                EntityInfo target = GetEntityById(info.GetAiStateInfo().Target);
                if (target == null || target.IsDead() || info.GetAiStateInfo().Target == info.GetId()) {
                    info.GetAiStateInfo().Target = 0;
                }
                //Birth and Death Processing
                if (info.IsBorning) {
                    info.SetAIEnable(true);
                    if (null != info.GetAiStateInfo().AiStoryInstanceInfo) {
                        if (info.BornTime <= 0) {
                            info.BornTime = TimeUtility.GetLocalMilliseconds();
                            BoxedValueList args = info.GetAiStateInfo().NewBoxedValueList();
                            info.GetAiStateInfo().SendNamespacedMessage("on_born", args);
                        } else if (info.BornTime + info.BornTimeout < curTime) {
                            info.IsBorning = false;
                            info.BornTime = 0;
                        }
                    } else {
                        info.IsBorning = false;
                        info.BornTime = 0;
                    }
                }
                if (info.IsDead() && !info.NeedDelete) {
                    if (null != info.GetAiStateInfo().AiStoryInstanceInfo) {
                        if (info.DeadTime <= 0) {
                            OnEntityKilled(info);
                            info.DeadTime = TimeUtility.GetLocalMilliseconds();
                            BoxedValueList args = info.GetAiStateInfo().NewBoxedValueList();
                            info.GetAiStateInfo().SendNamespacedMessage("on_dead", args);
                        } else if (info.DeadTime + info.DeadTimeout < curTime) {
                            info.DeadTime = 0;
                            info.NeedDelete = true;
                        }
                    } else {
                        if (info.DeadTime <= 0) {
                            OnEntityKilled(info);
                            info.DeadTime = TimeUtility.GetLocalMilliseconds();
                        } else if (info.DeadTime + 1000 < curTime) {
                            info.DeadTime = 0;
                            info.NeedDelete = true;
                        }
                    }
                }
                if (info.NeedDelete) {
                    m_DeletedEntities.Add(info);
                }
            }
            if (m_DeletedEntities.Count > 0) {
                for (int i = 0; i < m_DeletedEntities.Count; ++i) {
                    EntityInfo ni = m_DeletedEntities[i];
                    DestroyEntity(ni);
                }
            }
        }
        private void OnEntityKilled(EntityInfo ni)
        {
            if (ni.GetMovementStateInfo().IsMoving) {
                ni.GetMovementStateInfo().IsMoving = false;
            }
            ni.GetAiStateInfo().ChangeToState((int)PredefinedAiStateId.Idle);
            ni.GetAiStateInfo().Target = 0;

            ClientStorySystem.Instance.SendMessage("obj_killed", ni.GetId(), ni.GetUnitId());
            ClientStorySystem.Instance.SendMessage("npc_killed:" + ni.GetUnitId(), ni.GetId());
        }
        private void OnCreateEntity(EntityInfo npc)
        {
            if (null != npc) {
                OnAiInitDslLogic(npc);
                EntityViewModel view = m_EntityViewManager.GetEntityViewById(npc.GetId());
                if (null != view) {
                    view.Visible = true;
                }
            }
        }
        private void OnDestroyEntity(EntityInfo npc)
        {
            if (null != npc) {
                OnAiDestroy(npc);
            }
        }

        private SortedList<string, string> m_CommandDocs;
        private SortedList<string, string> m_FunctionDocs;
        private string m_LocalGmFile = string.Empty;
        private BlackBoard m_BlackBoard = new BlackBoard();
        private ClientAsyncActionProcessor m_AsyncActionProcessor = new ClientAsyncActionProcessor();
        
        private long m_LastTickTime = 0;
        private EntityKdTree m_EntityKdTree = new EntityKdTree();
        private EntityManager m_EntityManager = new EntityManager(256);
        private EntityViewModelManager m_EntityViewManager = new EntityViewModelManager();

        private List<EntityInfo> m_EntitiesForAi = new List<EntityInfo>();
        private List<GameObject> m_GameObjectsFromDsl = new List<GameObject>();
        private List<string> m_LoadedUiPrefabs = new List<string>();
        private List<EntityInfo> m_DeletedEntities = new List<EntityInfo>();
        private int m_PlayerId = 0;
        
        public static SceneSystem Instance
        {
            get { return s_Instance; }
        }
        private static SceneSystem s_Instance = new SceneSystem();
    }
}
