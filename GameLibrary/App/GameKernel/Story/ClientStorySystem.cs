using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using StorySystem;
using GameLibrary;

namespace GameLibrary.Story
{
    public sealed class ClientStorySystem
    {
        public void Init()
        {
            //注册剧情命令
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "preload", new StoryCommandFactoryHelper<Story.Commands.PreloadCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "startstory", new StoryCommandFactoryHelper<Story.Commands.StartStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "stopstory", new StoryCommandFactoryHelper<Story.Commands.StopStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "waitstory", new StoryCommandFactoryHelper<Story.Commands.WaitStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "pausestory", new StoryCommandFactoryHelper<Story.Commands.PauseStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "resumestory", new StoryCommandFactoryHelper<Story.Commands.ResumeStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "firemessage", new Story.Commands.FireMessageCommandFactory());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "fireconcurrentmessage", new Story.Commands.FireConcurrentMessageCommandFactory());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "waitallmessage", new StoryCommandFactoryHelper<Story.Commands.WaitAllMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "waitallmessagehandler", new StoryCommandFactoryHelper<Story.Commands.WaitAllMessageHandlerCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "suspendallmessagehandler", new StoryCommandFactoryHelper<Story.Commands.SuspendAllMessageHandlerCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "resumeallmessagehandler", new StoryCommandFactoryHelper<Story.Commands.ResumeAllMessageHandlerCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendaimessage", new StoryCommandFactoryHelper<Story.Commands.SendAiMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendaiconcurrentmessage", new StoryCommandFactoryHelper<Story.Commands.SendAiConcurrentMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendainamespacedmessage", new StoryCommandFactoryHelper<Story.Commands.SendAiNamespacedMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendaiconcurrentnamespacedmessage", new StoryCommandFactoryHelper<Story.Commands.SendAiConcurrentNamespacedMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "publishevent", new StoryCommandFactoryHelper<Story.Commands.PublishEventCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendmessage", new StoryCommandFactoryHelper<Story.Commands.SendMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendmessagewithtag", new StoryCommandFactoryHelper<Story.Commands.SendMessageWithTagCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendmessagewithgameobject", new StoryCommandFactoryHelper<Story.Commands.SendMessageWithGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendscriptmessage", new StoryCommandFactoryHelper<Story.Commands.SendScriptMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "creategameobject", new StoryCommandFactoryHelper<Story.Commands.CreateGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "settransform", new StoryCommandFactoryHelper<Story.Commands.SetTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "destroygameobject", new StoryCommandFactoryHelper<Story.Commands.DestroyGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "autorecycle", new StoryCommandFactoryHelper<Story.Commands.AutoRecycleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setparent", new StoryCommandFactoryHelper<Story.Commands.SetParentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setactive", new StoryCommandFactoryHelper<Story.Commands.SetActiveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setvisible", new StoryCommandFactoryHelper<Story.Commands.SetVisibleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "addcomponent", new StoryCommandFactoryHelper<Story.Commands.AddComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "removecomponent", new StoryCommandFactoryHelper<Story.Commands.RemoveComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "openurl", new StoryCommandFactoryHelper<Story.Commands.OpenUrlCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "quit", new StoryCommandFactoryHelper<Story.Commands.QuitCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "loadui", new StoryCommandFactoryHelper<Story.Commands.LoadUiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "bindui", new StoryCommandFactoryHelper<Story.Commands.BindUiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setactorscale", new StoryCommandFactoryHelper<Story.Commands.SetActorScaleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setstoryvariable", new StoryCommandFactoryHelper<Story.Commands.SetStoryVariableCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "gameobjectanimation", new StoryCommandFactoryHelper<Story.Commands.GameObjectAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "gameobjectanimationparam", new StoryCommandFactoryHelper<Story.Commands.GameObjectAnimationParamCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "changescene", new StoryCommandFactoryHelper<Story.Commands.ChangeSceneCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "createnpc", new StoryCommandFactoryHelper<Story.Commands.CreateNpcCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "destroynpc", new StoryCommandFactoryHelper<Story.Commands.DestroyNpcCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "destroynpcwithobjid", new StoryCommandFactoryHelper<Story.Commands.DestroyNpcWithObjIdCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcface", new StoryCommandFactoryHelper<Story.Commands.NpcFaceCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcmove", new StoryCommandFactoryHelper<Story.Commands.NpcMoveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcmovewithwaypoints", new StoryCommandFactoryHelper<Story.Commands.NpcMoveWithWaypointsCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcstop", new StoryCommandFactoryHelper<Story.Commands.NpcStopCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcenableai", new StoryCommandFactoryHelper<Story.Commands.NpcEnableAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcsetai", new StoryCommandFactoryHelper<Story.Commands.NpcSetAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcsetaitarget", new StoryCommandFactoryHelper<Story.Commands.NpcSetAiTargetCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcanimation", new StoryCommandFactoryHelper<Story.Commands.NpcAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcanimationparam", new StoryCommandFactoryHelper<Story.Commands.NpcAnimationParamCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcsetcamp", new StoryCommandFactoryHelper<Story.Commands.NpcSetCampCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objface", new StoryCommandFactoryHelper<Story.Commands.ObjFaceCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objmove", new StoryCommandFactoryHelper<Story.Commands.ObjMoveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objmovewithwaypoints", new StoryCommandFactoryHelper<Story.Commands.ObjMoveWithWaypointsCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objstop", new StoryCommandFactoryHelper<Story.Commands.ObjStopCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objenableai", new StoryCommandFactoryHelper<Story.Commands.ObjEnableAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objsetai", new StoryCommandFactoryHelper<Story.Commands.ObjSetAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objsetaitarget", new StoryCommandFactoryHelper<Story.Commands.ObjSetAiTargetCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objanimation", new StoryCommandFactoryHelper<Story.Commands.ObjAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objanimationparam", new StoryCommandFactoryHelper<Story.Commands.ObjAnimationParamCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objsetcamp", new StoryCommandFactoryHelper<Story.Commands.ObjSetCampCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sethp", new StoryCommandFactoryHelper<Story.Commands.SetHpCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setmaxhp", new StoryCommandFactoryHelper<Story.Commands.SetMaxHpCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setenerge", new StoryCommandFactoryHelper<Story.Commands.SetEnergyCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setmaxenergy", new StoryCommandFactoryHelper<Story.Commands.SetMaxEnergyCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setspeed", new StoryCommandFactoryHelper<Story.Commands.SetSpeedCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setlevel", new StoryCommandFactoryHelper<Story.Commands.SetLevelCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setexp", new StoryCommandFactoryHelper<Story.Commands.SetExpCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setattr", new StoryCommandFactoryHelper<Story.Commands.SetAttrCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setunitid", new StoryCommandFactoryHelper<Story.Commands.SetUnitIdCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setplayerid", new StoryCommandFactoryHelper<Story.Commands.SetPlayerIdCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "markcontrolbystory", new StoryCommandFactoryHelper<Story.Commands.MarkControlByStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setstoryskipped", new StoryCommandFactoryHelper<Story.Commands.SetStorySkippedCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "bornfinish", new StoryCommandFactoryHelper<Story.Commands.BornFinishCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "deadfinish", new StoryCommandFactoryHelper<Story.Commands.DeadFinishCommand>());

            //注册值与函数处理
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "gettime", new StoryValueFactoryHelper<Story.Values.GetTimeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "gettimescale", new StoryValueFactoryHelper<Story.Values.GetTimeScaleValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "isactive", new StoryValueFactoryHelper<Story.Values.IsActiveValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "isreallyactive", new StoryValueFactoryHelper<Story.Values.IsReallyActiveValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "isvisible", new StoryValueFactoryHelper<Story.Values.IsVisibleValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getcomponent", new StoryValueFactoryHelper<Story.Values.GetComponentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getcomponentinparent", new StoryValueFactoryHelper<Story.Values.GetComponentInParentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getcomponentinchildren", new StoryValueFactoryHelper<Story.Values.GetComponentInChildrenValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getcomponents", new StoryValueFactoryHelper<Story.Values.GetComponentsValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getcomponentsinparent", new StoryValueFactoryHelper<Story.Values.GetComponentsInParentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getcomponentsinchildren", new StoryValueFactoryHelper<Story.Values.GetComponentsInChildrenValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getgameobject", new StoryValueFactoryHelper<Story.Values.GetGameObjectValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getparent", new StoryValueFactoryHelper<Story.Values.GetParentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getchild", new StoryValueFactoryHelper<Story.Values.GetChildValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getunitytype", new StoryValueFactoryHelper<Story.Values.GetUnityTypeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getunityuitype", new StoryValueFactoryHelper<Story.Values.GetUnityUiTypeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getscripttype", new StoryValueFactoryHelper<Story.Values.GetScriptTypeValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getentityinfo", new StoryValueFactoryHelper<Story.Values.GetEntityInfoValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getentityview", new StoryValueFactoryHelper<Story.Values.GetEntityViewValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "global", new StoryValueFactoryHelper<Story.Values.GlobalValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "scene", new StoryValueFactoryHelper<Story.Values.SceneValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "resourcesystem", new StoryValueFactoryHelper<Story.Values.ResourceSystemValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getstory", new StoryValueFactoryHelper<Story.Values.GetStoryValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getstoryvariable", new StoryValueFactoryHelper<Story.Values.GetStoryVariableValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "deg2rad", new StoryValueFactoryHelper<Story.Values.Deg2RadValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "rad2deg", new StoryValueFactoryHelper<Story.Values.Rad2DegValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "blackboardget", new StoryValueFactoryHelper<Story.Values.BlackboardGetValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "unitid2objid", new StoryValueFactoryHelper<Story.Values.UnitId2ObjIdValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "objid2unitid", new StoryValueFactoryHelper<Story.Values.ObjId2UnitIdValue>());
            
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "npcgetnpctype", new StoryValueFactoryHelper<Story.Values.NpcGetNpcTypeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "npcgetaiparam", new StoryValueFactoryHelper<Story.Values.NpcGetAiParamValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "objgetnpctype", new StoryValueFactoryHelper<Story.Values.ObjGetNpcTypeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "objgetaiparam", new StoryValueFactoryHelper<Story.Values.ObjGetAiParamValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "isenemy", new StoryValueFactoryHelper<Story.Values.IsEnemyValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "isfriend", new StoryValueFactoryHelper<Story.Values.IsFriendValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getposition", new StoryValueFactoryHelper<Story.Values.GetPositionValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getpositionx", new StoryValueFactoryHelper<Story.Values.GetPositionXValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getpositiony", new StoryValueFactoryHelper<Story.Values.GetPositionYValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getpositionz", new StoryValueFactoryHelper<Story.Values.GetPositionZValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getrotation", new StoryValueFactoryHelper<Story.Values.GetRotationValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getrotationx", new StoryValueFactoryHelper<Story.Values.GetRotationXValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getrotationy", new StoryValueFactoryHelper<Story.Values.GetRotationYValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getrotationz", new StoryValueFactoryHelper<Story.Values.GetRotationZValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getscale", new StoryValueFactoryHelper<Story.Values.GetScaleValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getscalex", new StoryValueFactoryHelper<Story.Values.GetScaleXValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getscaley", new StoryValueFactoryHelper<Story.Values.GetScaleYValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getscalez", new StoryValueFactoryHelper<Story.Values.GetScaleZValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getcamp", new StoryValueFactoryHelper<Story.Values.GetCampValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "gethp", new StoryValueFactoryHelper<Story.Values.GetHpValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getmaxhp", new StoryValueFactoryHelper<Story.Values.GetMaxHpValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getenergy", new StoryValueFactoryHelper<Story.Values.GetEnergyValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getmaxenergy", new StoryValueFactoryHelper<Story.Values.GetMaxEnergyValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getspeed", new StoryValueFactoryHelper<Story.Values.GetSpeedValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getlevel", new StoryValueFactoryHelper<Story.Values.GetLevelValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getexp", new StoryValueFactoryHelper<Story.Values.GetExpValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getattr", new StoryValueFactoryHelper<Story.Values.GetAttrValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "calcoffset", new StoryValueFactoryHelper<Story.Values.CalcOffsetValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "calcdir", new StoryValueFactoryHelper<Story.Values.CalcDirValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "iscontrolbystory", new StoryValueFactoryHelper<Story.Values.IsControlByStoryValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "isstoryskipped", new StoryValueFactoryHelper<Story.Values.IsStorySkippedValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getplayerid", new StoryValueFactoryHelper<Story.Values.GetPlayerIdValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "findobjid", new StoryValueFactoryHelper<Story.Values.FindObjIdValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "findobjids", new StoryValueFactoryHelper<Story.Values.FindObjIdsValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "findallobjids", new StoryValueFactoryHelper<Story.Values.FindAllObjIdsValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "countnpc", new StoryValueFactoryHelper<Story.Values.CountNpcValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "countdyingnpc", new StoryValueFactoryHelper<Story.Values.CountDyingNpcValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getfilename", new StoryValueFactoryHelper<Story.Values.GetFileNameValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getdirname", new StoryValueFactoryHelper<Story.Values.GetDirectoryNameValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getextension", new StoryValueFactoryHelper<Story.Values.GetExtensionValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "combinepath", new StoryValueFactoryHelper<Story.Values.CombinePathValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getstreamingassets", new StoryValueFactoryHelper<Story.Values.GetStreamingAssetsValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GFX, "getpersistentpath", new StoryValueFactoryHelper<Story.Values.GetPersistentPathValue>());

            AiRegister.Register();

            LoadCustomCommandsAndValues();
        }
        public void Reset()
        {
            LoadCustomCommandsAndValues();

            m_GlobalVariables.Clear();
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (null != info) {
                    info.Reset();
                    m_StoryLogicInfos.RemoveAt(index);
                }
            }
            m_StoryLogicInfos.Clear();
        }
        public void LoadSceneStories(params string[] files)
        {
            for (int ix = 0; ix < files.Length; ++ix) {
                var filePath = files[ix];
                LoadAssetStory(string.Empty, filePath);
                Dictionary<string, StoryInstance> stories = StoryConfigManager.Instance.GetStories(filePath);
                if (null != stories) {
                    foreach (KeyValuePair<string, StoryInstance> pair in stories) {
                        AddStoryInstance(pair.Key, pair.Value.Clone());
                    }
                }
            }
        }
        public void LoadAiStory(string _namespace, string file)
        {
            LoadAssetStory(_namespace, file);
            Dictionary<string, StoryInstance> stories = StoryConfigManager.Instance.GetStories(file);
            if (null != stories) {
                foreach (KeyValuePair<string, StoryInstance> pair in stories) {
                    AiStoryInstanceInfo info = new AiStoryInstanceInfo();
                    info.m_StoryInstance = pair.Value.Clone();
                    info.m_IsUsed = false;
                    AddAiStoryInstanceInfoToPool(pair.Key, info);
                }
            }
        }
        public void LoadStoryFromFile(string _namespace, string file)
        {
            LoadAssetStory(_namespace, file);
            Dictionary<string, StoryInstance> stories = StoryConfigManager.Instance.GetStories(file);
            if (null != stories) {
                foreach (KeyValuePair<string, StoryInstance> pair in stories) {
                    AddStoryInstance(pair.Key, pair.Value.Clone());
                }
            }
        }
        public void LoadStoryFromBytes(string _namespace, string file, byte[] bytes)
        {
            StoryConfigManager.Instance.LoadStoryText(file, bytes, 0, _namespace);
            Dictionary<string, StoryInstance> stories = StoryConfigManager.Instance.GetStories(file);
            if (null != stories) {
                foreach (KeyValuePair<string, StoryInstance> pair in stories) {
                    AddStoryInstance(pair.Key, pair.Value.Clone());
                }
            }
        }
        public void ClearStoryInstancePool()
        {
            m_StoryInstancePool.Clear();
            m_AiStoryInstancePool.Clear();
        }

        public AiStoryInstanceInfo NewAiStoryInstance(string storyId, string _namespace, params string[] aiFiles)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            AiStoryInstanceInfo instInfo = GetUnusedAiStoryInstanceInfoFromPool(storyId);
            if (null == instInfo) {
                int ct;
                string[] filePath;
                ct = aiFiles.Length;
                filePath = new string[ct];
                for (int i = 0; i < ct; i++) {
                    filePath[i] = aiFiles[i];
                }
                LoadAssetStory(_namespace, filePath);
                StoryInstance instance = StoryConfigManager.Instance.NewStoryInstance(storyId, 0);
                if (instance == null) {
                    LogSystem.Error("Can't load ai story, story:{0} !", storyId);
                    return null;
                }
                for (int ix = 0; ix < filePath.Length; ++ix) {
                    Dictionary<string, StoryInstance> stories = StoryConfigManager.Instance.GetStories(filePath[ix]);
                    if (null != stories) {
                        foreach (KeyValuePair<string, StoryInstance> pair in stories) {
                            if (pair.Key != storyId) {
                                AiStoryInstanceInfo info = new AiStoryInstanceInfo();
                                info.m_StoryInstance = pair.Value.Clone();
                                info.m_IsUsed = false;
                                AddAiStoryInstanceInfoToPool(pair.Key, info);
                            }
                        }
                    }
                }
                AiStoryInstanceInfo res = new AiStoryInstanceInfo();
                res.m_StoryInstance = instance;
                res.m_IsUsed = true;

                AddAiStoryInstanceInfoToPool(storyId, res);
                return res;
            } else {
                instInfo.m_IsUsed = true;
                return instInfo;
            }
        }
        public void RecycleAiStoryInstance(AiStoryInstanceInfo info)
        {
            info.m_StoryInstance.Reset(false);
            info.m_IsUsed = false;
        }

        public int ActiveStoryCount
        {
            get
            {
                return m_StoryLogicInfos.Count;
            }
        }
        public IList<StoryInstance> ActiveStories
        {
            get { return m_StoryLogicInfos; }
        }
        public StrObjDict GlobalVariables
        {
            get { return m_GlobalVariables; }
        }
        public StoryInstance GetStory(string storyId)
        {
            return GetStory(storyId, string.Empty);
        }
        public StoryInstance GetStory(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            return GetStoryInstance(storyId);
        }
        public void StartStories(string storyId)
        {
            StartStories(storyId, string.Empty);
        }
        public void StartStories(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            foreach(var pair in m_StoryInstancePool) {
                var info = pair.Value;
                if (IsMatch(info.StoryId, storyId)) {
                    StopStory(info.StoryId);
                    m_StoryLogicInfos.Add(info);
                    info.Context = null;
                    info.GlobalVariables = m_GlobalVariables;
                    info.Start();

                    LogSystem.Info("StartStory {0}", info.StoryId);
                }
            }
        }
        public void PauseStories(string storyId, bool pause)
        {
            PauseStories(storyId, string.Empty, pause);
        }
        public void PauseStories(string storyId, string _namespace, bool pause)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (IsMatch(info.StoryId, storyId)) {
                    info.IsPaused = pause;
                }
            }
        }
        public void StopStories(string storyId)
        {
            StopStories(storyId, string.Empty);
        }
        public void StopStories(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (IsMatch(info.StoryId, storyId)) {
                    m_StoryLogicInfos.RemoveAt(index);
                }
            }
        }
        public int CountStories(string storyId)
        {
            return CountStories(storyId, string.Empty);
        }
        public int CountStories(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int ct = 0;
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (null != info && IsMatch(info.StoryId, storyId) && !info.IsInTick) {
                    ++ct;
                }
            }
            return ct;
        }
        public void MarkStoriesTerminated(string storyId)
        {
            MarkStoriesTerminated(storyId, string.Empty);
        }
        public void MarkStoriesTerminated(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (IsMatch(info.StoryId, storyId)) {
                    info.IsTerminated = true;
                }
            }
        }
        public void SkipCurMessageHandlers(string endMsg, string storyId)
        {
            SkipCurMessageHandlers(endMsg, storyId, string.Empty);
        }
        public void SkipCurMessageHandlers(string endMsg, string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (IsMatch(info.StoryId, storyId)) {
                    var enumer = info.GetMessageHandlerEnumerator();
                    while (enumer.MoveNext()) {
                        var handler = enumer.Current;
                        if (handler.IsTriggered && handler.MessageId != endMsg) {
                            handler.CanSkip = true;
                        }
                    }
                    var cenumer = info.GetConcurrentMessageHandlerEnumerator();
                    while (cenumer.MoveNext()) {
                        var handler = cenumer.Current;
                        if (handler.IsTriggered && handler.MessageId != endMsg) {
                            handler.CanSkip = true;
                        }
                    }
                }
            }
        }
        public void StartStory(string storyId)
        {
            StartStory(storyId, string.Empty);
        }
        public void StartStory(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            StoryInstance inst = GetStoryInstance(storyId);
            if (null != inst) {
                StopStory(storyId);
                m_StoryLogicInfos.Add(inst);
                inst.Context = null;
                inst.GlobalVariables = m_GlobalVariables;
                inst.Start();

                LogSystem.Info("StartStory {0}", storyId);
            } else {
                LogSystem.Error("Can't find story, story:{0} !", storyId);
            }
        }
        public void PauseStory(string storyId, bool pause)
        {
            PauseStory(storyId, string.Empty, pause);
        }
        public void PauseStory(string storyId, string _namespace, bool pause)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (info.StoryId == storyId) {
                    info.IsPaused = pause;
                }
            }
        }
        public void StopStory(string storyId)
        {
            StopStory(storyId, string.Empty);
        }
        public void StopStory(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (info.StoryId == storyId) {
                    m_StoryLogicInfos.RemoveAt(index);
                }
            }
        }
        public int CountStory(string storyId)
        {
            return CountStory(storyId, string.Empty);
        }
        public int CountStory(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int ct = 0;
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (null != info && info.StoryId == storyId && !info.IsInTick) {
                    ++ct;
                }
            }
            return ct;
        }
        public void MarkStoryTerminated(string storyId)
        {
            MarkStoryTerminated(storyId, string.Empty);
        }
        public void MarkStoryTerminated(string storyId, string _namespace)
        {
            if (!string.IsNullOrEmpty(_namespace)) {
                storyId = string.Format("{0}:{1}", _namespace, storyId);
            }
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (info.StoryId == storyId) {
                    info.IsTerminated = true;
                }
            }
        }
        public void Tick()
        {
            try {
                UnityEngine.Profiling.Profiler.BeginSample("GfxStorySystem.Tick");
                long curTime = TimeUtility.GetLocalMilliseconds();
                if (m_SleepMode) {
                    if (m_LastSleepTickTime + c_SleepTickInterval <= curTime) {
                        m_TotalSleepTime += curTime - m_LastSleepTickTime;
                        m_LastSleepTickTime = curTime;


                        bool sleepMode = true;
                        if (sleepMode) {
                            int bct = m_BindedStoryInstances.Count;
                            for (int ix = 0; ix < bct; ++ix) {
                                var info = m_BindedStoryInstances[ix];
                                if (!info.Instance.CanSleep()) {
                                    sleepMode = false;
                                    break;
                                }
                            }
                        }
                        if (sleepMode) {
                            int ct = m_StoryLogicInfos.Count;
                            for (int ix = 0; ix < ct; ++ix) {
                                StoryInstance info = m_StoryLogicInfos[ix];
                                if (!info.CanSleep()) {
                                    sleepMode = false;
                                    break;
                                }
                            }
                        }
                        m_SleepMode = sleepMode;
                    }
                }
                if (!m_SleepMode) {
                    m_TotalSleepTime = 0;
                    int bct = m_BindedStoryInstances.Count;
                    for (int ix = bct - 1; ix >= 0; --ix) {
                        var info = m_BindedStoryInstances[ix];
                        if (null == info.Object) {
                            m_StoryLogicInfos.Remove(info.Instance);
                            m_BindedStoryInstances.RemoveAt(ix);
                        }
                    }

                    int ct = m_StoryLogicInfos.Count;
                    for (int ix = ct - 1; ix >= 0; --ix) {
                        StoryInstance info = m_StoryLogicInfos[ix];
                        info.Tick(curTime);
                        if (info.IsTerminated) {
                            m_StoryLogicInfos.RemoveAt(ix);
                        }
                    }
                }
            } finally {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
        public void AddBindedStory(UnityEngine.Object obj, StoryInstance inst)
        {
            m_BindedStoryInstances.Add(new BindedStoryInfo { Object = obj, Instance = inst });
        }
        public void SendMessage(string msgId, params object[] args)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SendMessage(msgId, args);
            }
            foreach (var pair in m_AiStoryInstancePool) {
                var infos = pair.Value;
                int aiCt = infos.Count;
                for (int ix = aiCt - 1; ix >= 0; --ix) {
                    if (infos[ix].m_IsUsed && null != infos[ix].m_StoryInstance) {
                        infos[ix].m_StoryInstance.SendMessage(msgId, args);
                    }
                }
            }
            m_SleepMode = false;
            m_LastSleepTickTime = 0;
        }
        public void SendConcurrentMessage(string msgId, params object[] args)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SendConcurrentMessage(msgId, args);
            }
            foreach (var pair in m_AiStoryInstancePool) {
                var infos = pair.Value;
                int aiCt = infos.Count;
                for (int ix = aiCt - 1; ix >= 0; --ix) {
                    if (infos[ix].m_IsUsed && null != infos[ix].m_StoryInstance) {
                        infos[ix].m_StoryInstance.SendConcurrentMessage(msgId, args);
                    }
                }
            }
            m_SleepMode = false;
            m_LastSleepTickTime = 0;
        }
        public int CountMessage(string msgId)
        {
            int sum = 0;
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                sum += info.CountMessage(msgId);
            }
            foreach (var pair in m_AiStoryInstancePool) {
                var infos = pair.Value;
                int aiCt = infos.Count;
                for (int ix = aiCt - 1; ix >= 0; --ix) {
                    if (infos[ix].m_IsUsed && null != infos[ix].m_StoryInstance) {
                        sum += infos[ix].m_StoryInstance.CountMessage(msgId);
                    }
                }
            }
            return sum;
        }
        public void SuspendMessageHandler(string msgId, bool suspend)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SuspendMessageHandler(msgId, suspend);
            }
            foreach (var pair in m_AiStoryInstancePool) {
                var infos = pair.Value;
                int aiCt = infos.Count;
                for (int ix = aiCt - 1; ix >= 0; --ix) {
                    if (infos[ix].m_IsUsed && null != infos[ix].m_StoryInstance) {
                        infos[ix].m_StoryInstance.SuspendMessageHandler(msgId, suspend);
                    }
                }
            }
        }

        private void AddStoryInstance(string storyId, StoryInstance info)
        {
            if (!m_StoryInstancePool.ContainsKey(storyId)) {
                m_StoryInstancePool.Add(storyId, info);
            } else {
                m_StoryInstancePool[storyId] = info;
            }
        }
        private StoryInstance GetStoryInstance(string storyId)
        {
            StoryInstance info;
            m_StoryInstancePool.TryGetValue(storyId, out info);
            return info;
        }
        private void LoadCustomCommandsAndValues()
        {
            string valFile = "Dsl/Story/Common/CustomValues";
            string cmdFile = "Dsl/Story/Common/CustomCommands";
            Dsl.DslFile file1 = CustomCommandValueParser.LoadStoryText(valFile, LoadAssetFile(valFile));
            Dsl.DslFile file2 = CustomCommandValueParser.LoadStoryText(cmdFile, LoadAssetFile(cmdFile));
            CustomCommandValueParser.FirstParse(file1, file2);
            CustomCommandValueParser.FinalParse(file1, file2);
        }
        private void AddAiStoryInstanceInfoToPool(string storyId, AiStoryInstanceInfo info)
        {
            List<AiStoryInstanceInfo> infos;
            if (m_AiStoryInstancePool.TryGetValue(storyId, out infos)) {
                infos.Add(info);
            } else {
                infos = new List<AiStoryInstanceInfo>();
                infos.Add(info);
                m_AiStoryInstancePool.Add(storyId, infos);
            }
        }
        private AiStoryInstanceInfo GetUnusedAiStoryInstanceInfoFromPool(string storyId)
        {
            AiStoryInstanceInfo info = null;
            List<AiStoryInstanceInfo> infos;
            if (m_AiStoryInstancePool.TryGetValue(storyId, out infos)) {
                int ct = infos.Count;
                for (int ix = 0; ix < ct; ++ix) {
                    if (!infos[ix].m_IsUsed) {
                        info = infos[ix];
                        break;
                    }
                }
            }
            return info;
        }

        private void LoadAssetStory(string _namespace, params string[] files)
        {
            for (int i = 0; i < files.Length; i++) {
                string file = files[i];
                var text = LoadAssetFile(file);
                StoryConfigManager.Instance.LoadStoryText(file, text, 0, _namespace);
            }
        }
        private byte[] LoadAssetFile(string file)
        {
            var bytes = Utility.LoadFileFromStreamingAssets(file);
            return bytes;
        }
        private bool IsMatch(string realId, string prefixId)
        {
            if (realId == prefixId || realId.Length > prefixId.Length && realId.StartsWith(prefixId) && realId[prefixId.Length] == ':')
                return true;
            return false;
        }

        private ClientStorySystem() { }

        private class BindedStoryInfo
        {
            internal UnityEngine.Object Object;
            internal StoryInstance Instance;
        }
        private List<BindedStoryInfo> m_BindedStoryInstances = new List<BindedStoryInfo>();

        private StrObjDict m_GlobalVariables = new StrObjDict();
        private bool m_SleepMode = false;
        private long m_LastSleepTickTime = 0;
        private const long c_SleepTickInterval = 1000;
        private long m_TotalSleepTime = 0;
        private const long c_MaxStorySleepTime = 5000;

        private List<StoryInstance> m_StoryLogicInfos = new List<StoryInstance>();
        private Dictionary<string, StoryInstance> m_StoryInstancePool = new Dictionary<string, StoryInstance>();
        private Dictionary<string, List<AiStoryInstanceInfo>> m_AiStoryInstancePool = new Dictionary<string, List<AiStoryInstanceInfo>>();

        public static ClientStorySystem Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private static ClientStorySystem s_Instance = new ClientStorySystem();

        public static void ThreadInitMask()
        {
            StoryCommandManager.ThreadCommandGroupsMask = (ulong)((1 << (int)StoryCommandGroupDefine.GM) + (1 << (int)StoryCommandGroupDefine.GFX));
            StoryValueManager.ThreadValueGroupsMask = (ulong)((1 << (int)StoryValueGroupDefine.GM) + (1 << (int)StoryValueGroupDefine.GFX));
        }
    }
}
