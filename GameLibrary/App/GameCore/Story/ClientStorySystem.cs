using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using StoryScript;
using GameLibrary;

namespace GameLibrary.Story
{
    public sealed class ClientStorySystem
    {
        public void Init()
        {
            //注册剧情命令
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "preload", "preload command", new StoryCommandFactoryHelper<Story.Commands.PreloadCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "startstory", "startstory command", new StoryCommandFactoryHelper<Story.Commands.StartStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "stopstory", "stopstory command", new StoryCommandFactoryHelper<Story.Commands.StopStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "waitstory", "waitstory command", new StoryCommandFactoryHelper<Story.Commands.WaitStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "pausestory", "pausestory command", new StoryCommandFactoryHelper<Story.Commands.PauseStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "resumestory", "resumestory command", new StoryCommandFactoryHelper<Story.Commands.ResumeStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "firemessage", "firemessage command", new Story.Commands.FireMessageCommandFactory());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "fireconcurrentmessage", "fireconcurrentmessage command", new Story.Commands.FireConcurrentMessageCommandFactory());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "waitallmessage", "waitallmessage command", new StoryCommandFactoryHelper<Story.Commands.WaitAllMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "waitallmessagehandler", "waitallmessagehandler command", new StoryCommandFactoryHelper<Story.Commands.WaitAllMessageHandlerCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "suspendallmessagehandler", "suspendallmessagehandler command", new StoryCommandFactoryHelper<Story.Commands.SuspendAllMessageHandlerCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "resumeallmessagehandler", "resumeallmessagehandler command", new StoryCommandFactoryHelper<Story.Commands.ResumeAllMessageHandlerCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendaimessage", "sendaimessage command", new StoryCommandFactoryHelper<Story.Commands.SendAiMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendaiconcurrentmessage", "sendaiconcurrentmessage command", new StoryCommandFactoryHelper<Story.Commands.SendAiConcurrentMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendainamespacedmessage", "sendainamespacedmessage command", new StoryCommandFactoryHelper<Story.Commands.SendAiNamespacedMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendaiconcurrentnamespacedmessage", "sendaiconcurrentnamespacedmessage command", new StoryCommandFactoryHelper<Story.Commands.SendAiConcurrentNamespacedMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "publishevent", "publishevent command", new StoryCommandFactoryHelper<Story.Commands.PublishEventCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendmessage", "sendmessage command", new StoryCommandFactoryHelper<Story.Commands.SendMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendmessagewithtag", "sendmessagewithtag command", new StoryCommandFactoryHelper<Story.Commands.SendMessageWithTagCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendmessagewithgameobject", "sendmessagewithgameobject command", new StoryCommandFactoryHelper<Story.Commands.SendMessageWithGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sendscriptmessage", "sendscriptmessage command", new StoryCommandFactoryHelper<Story.Commands.SendScriptMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "creategameobject", "creategameobject command", new StoryCommandFactoryHelper<Story.Commands.CreateGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "settransform", "settransform command", new StoryCommandFactoryHelper<Story.Commands.SetTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "addtransform", "addtransform command", new StoryCommandFactoryHelper<Story.Commands.AddTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "destroygameobject", "destroygameobject command", new StoryCommandFactoryHelper<Story.Commands.DestroyGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "autorecycle", "autorecycle command", new StoryCommandFactoryHelper<Story.Commands.AutoRecycleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setparent", "setparent command", new StoryCommandFactoryHelper<Story.Commands.SetParentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setactive", "setactive command", new StoryCommandFactoryHelper<Story.Commands.SetActiveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setvisible", "setvisible command", new StoryCommandFactoryHelper<Story.Commands.SetVisibleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "addcomponent", "addcomponent command", new StoryCommandFactoryHelper<Story.Commands.AddComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "removecomponent", "removecomponent command", new StoryCommandFactoryHelper<Story.Commands.RemoveComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "openurl", "openurl command", new StoryCommandFactoryHelper<Story.Commands.OpenUrlCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "quit", "quit command", new StoryCommandFactoryHelper<Story.Commands.QuitCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "loadui", "loadui command", new StoryCommandFactoryHelper<Story.Commands.LoadUiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "bindui", "bindui command", new StoryCommandFactoryHelper<Story.Commands.BindUiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setactorscale", "setactorscale command", new StoryCommandFactoryHelper<Story.Commands.SetActorScaleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setstoryvariable", "setstoryvariable command", new StoryCommandFactoryHelper<Story.Commands.SetStoryVariableCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "gameobjectanimation", "gameobjectanimation command", new StoryCommandFactoryHelper<Story.Commands.GameObjectAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "gameobjectanimationparam", "gameobjectanimationparam command", new StoryCommandFactoryHelper<Story.Commands.GameObjectAnimationParamCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "changescene", "changescene command", new StoryCommandFactoryHelper<Story.Commands.ChangeSceneCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "createnpc", "createnpc command", new StoryCommandFactoryHelper<Story.Commands.CreateNpcCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "destroynpc", "destroynpc command", new StoryCommandFactoryHelper<Story.Commands.DestroyNpcCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "destroynpcwithobjid", "destroynpcwithobjid command", new StoryCommandFactoryHelper<Story.Commands.DestroyNpcWithObjIdCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcface", "npcface command", new StoryCommandFactoryHelper<Story.Commands.NpcFaceCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcmove", "npcmove command", new StoryCommandFactoryHelper<Story.Commands.NpcMoveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcmovewithwaypoints", "npcmovewithwaypoints command", new StoryCommandFactoryHelper<Story.Commands.NpcMoveWithWaypointsCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcstop", "npcstop command", new StoryCommandFactoryHelper<Story.Commands.NpcStopCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcenableai", "npcenableai command", new StoryCommandFactoryHelper<Story.Commands.NpcEnableAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcsetai", "npcsetai command", new StoryCommandFactoryHelper<Story.Commands.NpcSetAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcsetaitarget", "npcsetaitarget command", new StoryCommandFactoryHelper<Story.Commands.NpcSetAiTargetCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcanimation", "npcanimation command", new StoryCommandFactoryHelper<Story.Commands.NpcAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcanimationparam", "npcanimationparam command", new StoryCommandFactoryHelper<Story.Commands.NpcAnimationParamCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "npcsetcamp", "npcsetcamp command", new StoryCommandFactoryHelper<Story.Commands.NpcSetCampCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objface", "objface command", new StoryCommandFactoryHelper<Story.Commands.ObjFaceCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objmove", "objmove command", new StoryCommandFactoryHelper<Story.Commands.ObjMoveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objmovewithwaypoints", "objmovewithwaypoints command", new StoryCommandFactoryHelper<Story.Commands.ObjMoveWithWaypointsCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objstop", "objstop command", new StoryCommandFactoryHelper<Story.Commands.ObjStopCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objenableai", "objenableai command", new StoryCommandFactoryHelper<Story.Commands.ObjEnableAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objsetai", "objsetai command", new StoryCommandFactoryHelper<Story.Commands.ObjSetAiCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objsetaitarget", "objsetaitarget command", new StoryCommandFactoryHelper<Story.Commands.ObjSetAiTargetCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objanimation", "objanimation command", new StoryCommandFactoryHelper<Story.Commands.ObjAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objanimationparam", "objanimationparam command", new StoryCommandFactoryHelper<Story.Commands.ObjAnimationParamCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "objsetcamp", "objsetcamp command", new StoryCommandFactoryHelper<Story.Commands.ObjSetCampCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "sethp", "sethp command", new StoryCommandFactoryHelper<Story.Commands.SetHpCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setmaxhp", "setmaxhp command", new StoryCommandFactoryHelper<Story.Commands.SetMaxHpCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setenerge", "setenerge command", new StoryCommandFactoryHelper<Story.Commands.SetEnergyCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setmaxenergy", "setmaxenergy command", new StoryCommandFactoryHelper<Story.Commands.SetMaxEnergyCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setspeed", "setspeed command", new StoryCommandFactoryHelper<Story.Commands.SetSpeedCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setlevel", "setlevel command", new StoryCommandFactoryHelper<Story.Commands.SetLevelCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setexp", "setexp command", new StoryCommandFactoryHelper<Story.Commands.SetExpCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setattr", "setattr command", new StoryCommandFactoryHelper<Story.Commands.SetAttrCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setunitid", "setunitid command", new StoryCommandFactoryHelper<Story.Commands.SetUnitIdCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setplayerid", "setplayerid command", new StoryCommandFactoryHelper<Story.Commands.SetPlayerIdCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "markcontrolbystory", "markcontrolbystory command", new StoryCommandFactoryHelper<Story.Commands.MarkControlByStoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "setstoryskipped", "setstoryskipped command", new StoryCommandFactoryHelper<Story.Commands.SetStorySkippedCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "bornfinish", "bornfinish command", new StoryCommandFactoryHelper<Story.Commands.BornFinishCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GFX, "deadfinish", "deadfinish command", new StoryCommandFactoryHelper<Story.Commands.DeadFinishCommand>());

            //注册值与函数处理
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "gettime", "gettime function", new StoryFunctionFactoryHelper<Story.Functions.GetTimeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "gettimescale", "gettimescale function", new StoryFunctionFactoryHelper<Story.Functions.GetTimeScaleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "isactive", "isactive function", new StoryFunctionFactoryHelper<Story.Functions.IsActiveFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "isreallyactive", "isreallyactive function", new StoryFunctionFactoryHelper<Story.Functions.IsReallyActiveFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "isvisible", "isvisible function", new StoryFunctionFactoryHelper<Story.Functions.IsVisibleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getcomponent", "getcomponent function", new StoryFunctionFactoryHelper<Story.Functions.GetComponentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getcomponentinparent", "getcomponentinparent function", new StoryFunctionFactoryHelper<Story.Functions.GetComponentInParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getcomponentinchildren", "getcomponentinchildren function", new StoryFunctionFactoryHelper<Story.Functions.GetComponentInChildrenFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getcomponents", "getcomponents function", new StoryFunctionFactoryHelper<Story.Functions.GetComponentsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getcomponentsinparent", "getcomponentsinparent function", new StoryFunctionFactoryHelper<Story.Functions.GetComponentsInParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getcomponentsinchildren", "getcomponentsinchildren function", new StoryFunctionFactoryHelper<Story.Functions.GetComponentsInChildrenFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getgameobject", "getgameobject function", new StoryFunctionFactoryHelper<Story.Functions.GetGameObjectFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getparent", "getparent function", new StoryFunctionFactoryHelper<Story.Functions.GetParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getchild", "getchild function", new StoryFunctionFactoryHelper<Story.Functions.GetChildFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getunitytype", "getunitytype function", new StoryFunctionFactoryHelper<Story.Functions.GetUnityTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getunityuitype", "getunityuitype function", new StoryFunctionFactoryHelper<Story.Functions.GetUnityUiTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getusertype", "getusertype function", new StoryFunctionFactoryHelper<Story.Functions.GetUserTypeFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getentityinfo", "getentityinfo function", new StoryFunctionFactoryHelper<Story.Functions.GetEntityInfoFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getentityview", "getentityview function", new StoryFunctionFactoryHelper<Story.Functions.GetEntityViewFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "global", "global function", new StoryFunctionFactoryHelper<Story.Functions.GlobalFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "scene", "scene function", new StoryFunctionFactoryHelper<Story.Functions.SceneFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "resourcesystem", "resourcesystem function", new StoryFunctionFactoryHelper<Story.Functions.ResourceSystemFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getstory", "getstory function", new StoryFunctionFactoryHelper<Story.Functions.GetStoryFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getstoryvariable", "getstoryvariable function", new StoryFunctionFactoryHelper<Story.Functions.GetStoryVariableFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "deg2rad", "deg2rad function", new StoryFunctionFactoryHelper<Story.Functions.Deg2RadFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "rad2deg", "rad2deg function", new StoryFunctionFactoryHelper<Story.Functions.Rad2DegFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "blackboardget", "blackboardget function", new StoryFunctionFactoryHelper<Story.Functions.BlackboardGetFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "unitid2objid", "unitid2objid function", new StoryFunctionFactoryHelper<Story.Functions.UnitId2ObjIdFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "objid2unitid", "objid2unitid function", new StoryFunctionFactoryHelper<Story.Functions.ObjId2UnitIdFunction>());
            
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "npcgetnpctype", "npcgetnpctype function", new StoryFunctionFactoryHelper<Story.Functions.NpcGetNpcTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "npcgetaiparam", "npcgetaiparam function", new StoryFunctionFactoryHelper<Story.Functions.NpcGetAiParamFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "objgetnpctype", "objgetnpctype function", new StoryFunctionFactoryHelper<Story.Functions.ObjGetNpcTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "objgetaiparam", "objgetaiparam function", new StoryFunctionFactoryHelper<Story.Functions.ObjGetAiParamFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "isenemy", "isenemy function", new StoryFunctionFactoryHelper<Story.Functions.IsEnemyFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "isfriend", "isfriend function", new StoryFunctionFactoryHelper<Story.Functions.IsFriendFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getposition", "getposition function", new StoryFunctionFactoryHelper<Story.Functions.GetPositionFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getpositionx", "getpositionx function", new StoryFunctionFactoryHelper<Story.Functions.GetPositionXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getpositiony", "getpositiony function", new StoryFunctionFactoryHelper<Story.Functions.GetPositionYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getpositionz", "getpositionz function", new StoryFunctionFactoryHelper<Story.Functions.GetPositionZFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getrotation", "getrotation function", new StoryFunctionFactoryHelper<Story.Functions.GetRotationFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getrotationx", "getrotationx function", new StoryFunctionFactoryHelper<Story.Functions.GetRotationXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getrotationy", "getrotationy function", new StoryFunctionFactoryHelper<Story.Functions.GetRotationYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getrotationz", "getrotationz function", new StoryFunctionFactoryHelper<Story.Functions.GetRotationZFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getscale", "getscale function", new StoryFunctionFactoryHelper<Story.Functions.GetScaleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getscalex", "getscalex function", new StoryFunctionFactoryHelper<Story.Functions.GetScaleXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getscaley", "getscaley function", new StoryFunctionFactoryHelper<Story.Functions.GetScaleYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getscalez", "getscalez function", new StoryFunctionFactoryHelper<Story.Functions.GetScaleZFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "position", "position function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "rotation", "rotation function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "scale", "scale function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getcamp", "getcamp function", new StoryFunctionFactoryHelper<Story.Functions.GetCampFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "gethp", "gethp function", new StoryFunctionFactoryHelper<Story.Functions.GetHpFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getmaxhp", "getmaxhp function", new StoryFunctionFactoryHelper<Story.Functions.GetMaxHpFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getenergy", "getenergy function", new StoryFunctionFactoryHelper<Story.Functions.GetEnergyFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getmaxenergy", "getmaxenergy function", new StoryFunctionFactoryHelper<Story.Functions.GetMaxEnergyFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getspeed", "getspeed function", new StoryFunctionFactoryHelper<Story.Functions.GetSpeedFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getlevel", "getlevel function", new StoryFunctionFactoryHelper<Story.Functions.GetLevelFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getexp", "getexp function", new StoryFunctionFactoryHelper<Story.Functions.GetExpFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getattr", "getattr function", new StoryFunctionFactoryHelper<Story.Functions.GetAttrFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "calcoffset", "calcoffset function", new StoryFunctionFactoryHelper<Story.Functions.CalcOffsetFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "calcdir", "calcdir function", new StoryFunctionFactoryHelper<Story.Functions.CalcDirFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "isstoryskipped", "isstoryskipped function", new StoryFunctionFactoryHelper<Story.Functions.IsStorySkippedFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getplayerid", "getplayerid function", new StoryFunctionFactoryHelper<Story.Functions.GetPlayerIdFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "findobjid", "findobjid function", new StoryFunctionFactoryHelper<Story.Functions.FindObjIdFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "findobjids", "findobjids function", new StoryFunctionFactoryHelper<Story.Functions.FindObjIdsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "findallobjids", "findallobjids function", new StoryFunctionFactoryHelper<Story.Functions.FindAllObjIdsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "countnpc", "countnpc function", new StoryFunctionFactoryHelper<Story.Functions.CountNpcFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "countdyingnpc", "countdyingnpc function", new StoryFunctionFactoryHelper<Story.Functions.CountDyingNpcFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getfilename", "getfilename function", new StoryFunctionFactoryHelper<Story.Functions.GetFileNameFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getdirname", "getdirname function", new StoryFunctionFactoryHelper<Story.Functions.GetDirectoryNameFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getextension", "getextension function", new StoryFunctionFactoryHelper<Story.Functions.GetExtensionFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "combinepath", "combinepath function", new StoryFunctionFactoryHelper<Story.Functions.CombinePathFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getstreamingassets", "getstreamingassets function", new StoryFunctionFactoryHelper<Story.Functions.GetStreamingAssetsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GFX, "getpersistentpath", "getpersistentpath function", new StoryFunctionFactoryHelper<Story.Functions.GetPersistentPathFunction>());

            AiRegister.Register();

            LoadCustomCommandsAndFunctions();
        }
        public void Reset()
        {
            Reset(true);
        }
        public void Reset(bool logIfTriggered)
        {
            LoadCustomCommandsAndFunctions();

            m_GlobalVariables.Clear();
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (null != info) {
                    info.Reset(logIfTriggered);
                    m_StoryLogicInfos.RemoveAt(index);
                }
            }
            m_StoryLogicInfos.Clear();
            m_SleepMode = false;
            m_LastSleepTickTime = 0;
            m_TotalSleepTime = 0;
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
            info.m_StoryInstance.Reset();
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
        public StrBoxedValueDict GlobalVariables
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
                    info.ClearMessage();
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
                UnityEngine.Profiling.Profiler.BeginSample("ClientStorySystem.Tick");
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
        public BoxedValueList NewBoxedValueList()
        {
            var args = m_BoxedValueListPool.Alloc();
            args.Clear();
            return args;
        }
        public void SendMessage(string msgId)
        {
            var args = NewBoxedValueList();
            SendMessage(msgId, args);
        }
        public void SendMessage(string msgId, BoxedValue arg1)
        {
            var args = NewBoxedValueList();
            args.Add(arg1);
            SendMessage(msgId, args);
        }
        public void SendMessage(string msgId, BoxedValue arg1, BoxedValue arg2)
        {
            var args = NewBoxedValueList();
            args.Add(arg1);
            args.Add(arg2);
            SendMessage(msgId, args);
        }
        public void SendMessage(string msgId, BoxedValue arg1, BoxedValue arg2, BoxedValue arg3)
        {
            var args = NewBoxedValueList();
            args.Add(arg1);
            args.Add(arg2);
            args.Add(arg3);
            SendMessage(msgId, args);
        }
        public void SendMessage(string msgId, BoxedValueList args)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                var newArgs = info.NewBoxedValueList();
                newArgs.AddRange(args);
                info.SendMessage(msgId, newArgs);
            }
            foreach (var pair in m_AiStoryInstancePool) {
                var infos = pair.Value;
                int aiCt = infos.Count;
                for (int ix = aiCt - 1; ix >= 0; --ix) {
                    if (infos[ix].m_IsUsed && null != infos[ix].m_StoryInstance) {
                        var newArgs = infos[ix].m_StoryInstance.NewBoxedValueList();
                        newArgs.AddRange(args);
                        infos[ix].m_StoryInstance.SendMessage(msgId, newArgs);
                    }
                }
            }
            m_SleepMode = false;
            m_LastSleepTickTime = 0;
            m_BoxedValueListPool.Recycle(args);
        }
        public void SendConcurrentMessage(string msgId)
        {
            var args = NewBoxedValueList();
            SendConcurrentMessage(msgId, args);
        }
        public void SendConcurrentMessage(string msgId, BoxedValue arg1)
        {
            var args = NewBoxedValueList();
            args.Add(arg1);
            SendConcurrentMessage(msgId, args);
        }
        public void SendConcurrentMessage(string msgId, BoxedValue arg1, BoxedValue arg2)
        {
            var args = NewBoxedValueList();
            args.Add(arg1);
            args.Add(arg2);
            SendConcurrentMessage(msgId, args);
        }
        public void SendConcurrentMessage(string msgId, BoxedValue arg1, BoxedValue arg2, BoxedValue arg3)
        {
            var args = NewBoxedValueList();
            args.Add(arg1);
            args.Add(arg2);
            args.Add(arg3);
            SendConcurrentMessage(msgId, args);
        }
        public void SendConcurrentMessage(string msgId, BoxedValueList args)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                var newArgs = info.NewBoxedValueList();
                newArgs.AddRange(args);
                info.SendConcurrentMessage(msgId, newArgs);
            }
            foreach (var pair in m_AiStoryInstancePool) {
                var infos = pair.Value;
                int aiCt = infos.Count;
                for (int ix = aiCt - 1; ix >= 0; --ix) {
                    if (infos[ix].m_IsUsed && null != infos[ix].m_StoryInstance) {
                        var newArgs = infos[ix].m_StoryInstance.NewBoxedValueList();
                        newArgs.AddRange(args);
                        infos[ix].m_StoryInstance.SendConcurrentMessage(msgId, newArgs);
                    }
                }
            }
            m_SleepMode = false;
            m_LastSleepTickTime = 0;
            m_BoxedValueListPool.Recycle(args);
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
        private void LoadCustomCommandsAndFunctions()
        {
            string funcFile = "Dsl/Story/Common/CustomFunctions";
            string cmdFile = "Dsl/Story/Common/CustomCommands";
            Dsl.DslFile file1 = CustomCommandFunctionParser.LoadStoryText(funcFile, LoadAssetFile(funcFile));
            Dsl.DslFile file2 = CustomCommandFunctionParser.LoadStoryText(cmdFile, LoadAssetFile(cmdFile));
            CustomCommandFunctionParser.FirstParse(file1, file2);
            CustomCommandFunctionParser.FinalParse(file1, file2);
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
        private SimpleObjectPool<BoxedValueList> m_BoxedValueListPool = new SimpleObjectPool<BoxedValueList>();

        private StrBoxedValueDict m_GlobalVariables = new StrBoxedValueDict();
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
            StoryFunctionManager.ThreadFunctionGroupsMask = (ulong)((1 << (int)StoryFunctionGroupDefine.GM) + (1 << (int)StoryFunctionGroupDefine.GFX));
        }
    }
}
