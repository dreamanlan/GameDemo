using System;
using StoryScript;
using StoryScript.DslExpression;
using GameLibrary.GmCommands;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// Registers game-specific story expressions with DslCalculator
    /// Call this method after initializing DslCalculator
    /// </summary>
    public static class GameStoryExpressionRegistrar
    {
        public static void RegisterGameExpressions(DslCalculator calc)
        {
            if (calc == null)
                return;

            // Register Story control expressions
            calc.Register("preload", "preload(dslfile1, dslfile2, ...) - preload story DSL files",
                new ExpressionFactoryHelper<PreloadExp>());
            calc.Register("startstory", "startstory(story_id[, multiple]) - start a story",
                new ExpressionFactoryHelper<StartStoryExp>());
            calc.Register("stopstory", "stopstory(story_id[, multiple]) - stop a story",
                new ExpressionFactoryHelper<StopStoryExp>());
            calc.Register("waitstory", "waitstory(story_id1, story_id2, ...) - wait for stories to finish",
                new ExpressionFactoryHelper<WaitStoryExp>());
            calc.Register("pausestory", "pausestory(story_id) - pause a story",
                new ExpressionFactoryHelper<PauseStoryExp>());
            calc.Register("resumestory", "resumestory(story_id) - resume a paused story",
                new ExpressionFactoryHelper<ResumeStoryExp>());
            calc.Register("firemessage", "firemessage(msg, arg1, arg2, ...) - send a sequential message to the story system",
                new ExpressionFactoryHelper<FireMessageExp>());
            calc.Register("fireconcurrentmessage", "fireconcurrentmessage(msg, arg1, arg2, ...) - send a concurrent message to the story system",
                new ExpressionFactoryHelper<FireConcurrentMessageExp>());
            calc.Register("waitallmessage", "waitallmessage(msgid1, msgid2, ...) - wait for all messages",
                new ExpressionFactoryHelper<WaitAllMessageExp>());
            calc.Register("waitallmessagehandler", "waitallmessagehandler(msgid1, msgid2, ...) - wait for all message handlers",
                new ExpressionFactoryHelper<WaitAllMessageHandlerExp>());
            calc.Register("suspendallmessagehandler", "suspendallmessagehandler(msgid1, msgid2, ...) - suspend message handlers",
                new ExpressionFactoryHelper<SuspendAllMessageHandlerExp>());
            calc.Register("resumeallmessagehandler", "resumeallmessagehandler(msgid1, msgid2, ...) - resume message handlers",
                new ExpressionFactoryHelper<ResumeAllMessageHandlerExp>());
            calc.Register("sendaimessage", "sendaimessage(objid, msg, ...) - send AI message to object",
                new ExpressionFactoryHelper<SendAiMessageExp>());
            calc.Register("sendaiconcurrentmessage", "sendaiconcurrentmessage(objid, msg, ...) - send concurrent AI message",
                new ExpressionFactoryHelper<SendAiConcurrentMessageExp>());
            calc.Register("sendainamespacedmessage", "sendainamespacedmessage(objid, msg, arg1, arg2, ...) - send namespaced AI message to object",
                new ExpressionFactoryHelper<SendAiNamespacedMessageExp>());
            calc.Register("sendaiconcurrentnamespacedmessage", "sendaiconcurrentnamespacedmessage(objid, msg, ...) - send concurrent namespaced AI message",
                new ExpressionFactoryHelper<SendAiConcurrentNamespacedMessageExp>());
            calc.Register("setstoryvariable", "setstoryvariable(story_id[, namespace], name, value) - set a variable on another story instance",
                new ExpressionFactoryHelper<SetStoryVariableExp>());
            calc.Register("setstoryskipped", "setstoryskipped(0_or_1) - set story skipped state",
                new ExpressionFactoryHelper<SetStorySkippedExp>());
            calc.Register("setstoryspeedup", "setstoryspeedup(0_or_1) - set story speedup state",
                new ExpressionFactoryHelper<SetStorySpeedupExp>());
            calc.Register("isstoryskipped", "isstoryskipped() - check if story is skipped",
                new ExpressionFactoryHelper<IsStorySkippedExp>());

            // Register General utility expressions
            calc.Register("publishevent", "publishevent(ev_name, group, arg1, arg2, ...) - publish an event",
                new ExpressionFactoryHelper<PublishEventExp>());
            calc.Register("sendmessage", "sendmessage(obj, msg, ...) - send Unity message to GameObject",
                new ExpressionFactoryHelper<SendMessageExp>());
            calc.Register("sendmessagewithtag", "sendmessagewithtag(tag, msg, ...) - send Unity message to GameObjects by tag",
                new ExpressionFactoryHelper<SendMessageWithTagExp>());
            calc.Register("sendmessagewithgameobject", "sendmessagewithgameobject(obj, msg, ...) - send Unity message (supports GameObject and ID)",
                new ExpressionFactoryHelper<SendMessageWithGameObjectExp>());
            calc.Register("sendscriptmessage", "sendscriptmessage(msg, arg1, arg2, ...) - send a script message",
                new ExpressionFactoryHelper<SendScriptMessageExp>());

            // Register Time and Conversion expressions
            calc.Register("gettime", "gettime() - get Unity game time",
                new ExpressionFactoryHelper<GetTimeExp>());
            calc.Register("gettimescale", "gettimescale() - get Unity time scale",
                new ExpressionFactoryHelper<GetTimeScaleExp>());
            calc.Register("deg2rad", "deg2rad(degree) - convert degrees to radians",
                new ExpressionFactoryHelper<Deg2RadExp>());
            calc.Register("rad2deg", "rad2deg(radian) - convert radians to degrees",
                new ExpressionFactoryHelper<Rad2DegExp>());
            calc.Register("getpersistentpath", "getpersistentpath() - get persistent data path",
                new ExpressionFactoryHelper<GetPersistentPathExp>());

            // Register GameObject inspection expressions
            calc.Register("isactive", "isactive(obj) - check if GameObject is active (activeSelf)",
                new ExpressionFactoryHelper<IsActiveExp>());
            calc.Register("isreallyactive", "isreallyactive(obj) - check if GameObject is active in hierarchy",
                new ExpressionFactoryHelper<IsReallyActiveExp>());
            calc.Register("isvisible", "isvisible(obj) - check if object's renderer is visible",
                new ExpressionFactoryHelper<IsVisibleExp>());
            calc.Register("getparent", "getparent(obj) - get parent GameObject",
                new ExpressionFactoryHelper<GetParentExp>());

            // Register Type resolution expressions
            calc.Register("getunityuitype", "getunityuitype(name) - get UnityEngine.UI type by name",
                new ExpressionFactoryHelper<GetUnityUiTypeExp>());
            calc.Register("getusertype", "getusertype(name) - get user type by name",
                new ExpressionFactoryHelper<GetUserTypeExp>());

            // Register System instance expressions
            calc.Register("global", "global() - get GlobalVariables instance",
                new ExpressionFactoryHelper<GlobalExp>());
            calc.Register("scene", "scene() - get SceneSystem instance",
                new ExpressionFactoryHelper<SceneExp>());
            calc.Register("resourcesystem", "resourcesystem() - get ResourceSystem instance",
                new ExpressionFactoryHelper<ResourceSystemExp>());

            // Register Story system expressions
            calc.Register("getstory", "getstory(story_id[, namespace]) - get story instance",
                new ExpressionFactoryHelper<GetStoryExp>());
            calc.Register("getstoryvariable", "getstoryvariable(story_id, var_name[, namespace]) - get variable from another story",
                new ExpressionFactoryHelper<GetStoryVariableExp>());

            // Register Entity info expressions
            calc.Register("getentityinfo", "getentityinfo(objid) - get entity info by object ID",
                new ExpressionFactoryHelper<GetEntityInfoExp>());
            calc.Register("getentityview", "getentityview(viewid) - get entity view by view ID",
                new ExpressionFactoryHelper<GetEntityViewExp>());

            // Register GameObject expressions
            calc.Register("creategameobject", "creategameobject(name, prefab[, parent])[obj(varname)] { position(v3); rotation(v3); scale(v3); disable(t1,...); remove(t1,...); } - create GameObject",
                new ExpressionFactoryHelper<CreateGameObjectExp>());
            calc.Register("settransform", "settransform(obj, local_or_world) { position(v3); rotation(v3); scale(v3); } - set transform",
                new ExpressionFactoryHelper<SetTransformExp>());
            calc.Register("addtransform", "addtransform(obj, local_or_world[, position, rotation, scale]) - add transform values",
                new ExpressionFactoryHelper<AddTransformExp>());
            calc.Register("destroygameobject", "destroygameobject(path_or_obj) - destroy a GameObject",
                new ExpressionFactoryHelper<DestroyGameObjectExp>());
            calc.Register("autorecycle", "autorecycle(path_or_obj, enable) - add/remove GameObject from auto-recycle list",
                new ExpressionFactoryHelper<AutoRecycleExp>());
            calc.Register("setparent", "setparent(child, parent) - set GameObject parent",
                new ExpressionFactoryHelper<SetParentExp>());
            calc.Register("setactive", "setactive(path_or_obj, active) - set GameObject active state",
                new ExpressionFactoryHelper<SetActiveExp>());
            calc.Register("setvisible", "setvisible(path_or_obj, visible) - set GameObject visibility",
                new ExpressionFactoryHelper<SetVisibleExp>());
            calc.Register("addcomponent", "addcomponent(obj, component_type) - add component to GameObject",
                new ExpressionFactoryHelper<AddComponentExp>());
            calc.Register("removecomponent", "removecomponent(obj, component_type) - remove component from GameObject",
                new ExpressionFactoryHelper<RemoveComponentExp>());
            calc.Register("setactorscale", "setactorscale(objid, scale) - set actor scale",
                new ExpressionFactoryHelper<SetActorScaleExp>());

            // Register UI and Scene expressions
            calc.Register("openurl", "openurl(url) - open a URL",
                new ExpressionFactoryHelper<OpenUrlExp>());
            calc.Register("quit", "quit() - quit the application",
                new ExpressionFactoryHelper<QuitExp>());
            calc.Register("loadui", "loadui(name, prefab, dslfile[, dont_destroy_old]) - load a UI prefab",
                new ExpressionFactoryHelper<LoadUiExp>());
            calc.Register("bindui", "bindui(obj) { var(...); onevent(...); ... } - bind UI events and variables",
                new ExpressionFactoryHelper<BindUiExp>());
            calc.Register("changescene", "changescene(target_scene) - change to a different scene",
                new ExpressionFactoryHelper<ChangeSceneExp>());

            // Register Animation expressions
            calc.Register("gameobjectanimation", "gameobjectanimation(obj, anim_name) - play an animation",
                new ExpressionFactoryHelper<GameObjectAnimationExp>());
            calc.Register("gameobjectanimationparam", "gameobjectanimationparam(obj) { int(key,value); float(key,value); bool(key,value); trigger(key,value); ... } - set animator parameters",
                new ExpressionFactoryHelper<GameObjectAnimationParamExp>());
            calc.Register("npcanimation", "npcanimation(unit_id, anim[, normalized_time]) - play NPC animation",
                new ExpressionFactoryHelper<NpcAnimationExp>());
            calc.Register("npcanimationparam", "npcanimationparam(unit_id) { int(key,value); ... } - set NPC animator parameters",
                new ExpressionFactoryHelper<NpcAnimationParamExp>());
            calc.Register("objanimation", "objanimation(obj_id, anim[, normalized_time]) - play object animation",
                new ExpressionFactoryHelper<ObjAnimationExp>());
            calc.Register("objanimationparam", "objanimationparam(obj_id) { int(key,value); ... } - set object animator parameters",
                new ExpressionFactoryHelper<ObjAnimationParamExp>());
            calc.Register("setanimatorparam", "setanimatorparam(obj, type, name, value) - set animator parameter",
                new ExpressionFactoryHelper<SetAnimatorParamExp>());

            // Register NPC expressions
            calc.Register("createnpc", "createnpc(unit_id, pos, dir, model, type[, ai, aiParams]) - create NPC",
                new ExpressionFactoryHelper<CreateNpcExp>());
            calc.Register("destroynpc", "destroynpc(unit_id[, immediately]) - destroy NPC by unit ID",
                new ExpressionFactoryHelper<DestroyNpcExp>());
            calc.Register("destroynpcwithobjid", "destroynpcwithobjid(objid[, immediately]) - destroy NPC by object ID",
                new ExpressionFactoryHelper<DestroyNpcWithObjIdExp>());
            calc.Register("npcface", "npcface(unit_id, dir[, immediately]) - set NPC face direction",
                new ExpressionFactoryHelper<NpcFaceExp>());
            calc.Register("npcmove", "npcmove(unit_id, pos[, event]) - move NPC to position",
                new ExpressionFactoryHelper<NpcMoveExp>());
            calc.Register("npcmovewithwaypoints", "npcmovewithwaypoints(unit_id, waypoints[, event]) - move NPC along waypoints",
                new ExpressionFactoryHelper<NpcMoveWithWaypointsExp>());
            calc.Register("npcstop", "npcstop(unit_id) - stop NPC movement",
                new ExpressionFactoryHelper<NpcStopExp>());
            calc.Register("npcenableai", "npcenableai(unit_id, enable) - enable/disable NPC AI",
                new ExpressionFactoryHelper<NpcEnableAiExp>());
            calc.Register("npcsetai", "npcsetai(unit_id, ai_logic, ai_params) - set NPC AI",
                new ExpressionFactoryHelper<NpcSetAiExp>());
            calc.Register("npcsetaitarget", "npcsetaitarget(unit_id, target_id) - set NPC AI target",
                new ExpressionFactoryHelper<NpcSetAiTargetExp>());
            calc.Register("npcsetcamp", "npcsetcamp(unit_id, camp_id) - set NPC camp",
                new ExpressionFactoryHelper<NpcSetCampExp>());
            calc.Register("npcattack", "npcattack(unit_id[, target_unit_id]) - command NPC to attack",
                new ExpressionFactoryHelper<NpcAttackExp>());
            calc.Register("npcgetnpctype", "npcgetnpctype(unitid) - get NPC entity type",
                new ExpressionFactoryHelper<NpcGetNpcTypeExp>());
            calc.Register("npcgetaiparam", "npcgetaiparam(unitid, index) - get NPC AI parameter",
                new ExpressionFactoryHelper<NpcGetAiParamExp>());

            // Register Object expressions
            calc.Register("objface", "objface(obj_id, dir[, immediately]) - set object face direction",
                new ExpressionFactoryHelper<ObjFaceExp>());
            calc.Register("objmove", "objmove(obj_id, pos[, event]) - move object to position",
                new ExpressionFactoryHelper<ObjMoveExp>());
            calc.Register("objmovewithwaypoints", "objmovewithwaypoints(obj_id, waypoints[, event]) - move object along waypoints",
                new ExpressionFactoryHelper<ObjMoveWithWaypointsExp>());
            calc.Register("objstop", "objstop(obj_id) - stop object movement",
                new ExpressionFactoryHelper<ObjStopExp>());
            calc.Register("objenableai", "objenableai(obj_id, enable) - enable/disable object AI",
                new ExpressionFactoryHelper<ObjEnableAiExp>());
            calc.Register("objsetai", "objsetai(obj_id, ai_logic, ai_params) - set object AI",
                new ExpressionFactoryHelper<ObjSetAiExp>());
            calc.Register("objsetaitarget", "objsetaitarget(obj_id, target_id) - set object AI target",
                new ExpressionFactoryHelper<ObjSetAiTargetExp>());
            calc.Register("objattack", "objattack(obj_id[, target_obj_id]) - command object to attack",
                new ExpressionFactoryHelper<ObjAttackExp>());
            calc.Register("objsetcamp", "objsetcamp(objid, camp_id) - set entity camp",
                new ExpressionFactoryHelper<ObjSetCampExp>());
            calc.Register("objgetnpctype", "objgetnpctype(objid) - get entity type by object ID",
                new ExpressionFactoryHelper<ObjGetNpcTypeExp>());
            calc.Register("objgetaiparam", "objgetaiparam(objid, index) - get entity AI parameter",
                new ExpressionFactoryHelper<ObjGetAiParamExp>());

            // Register Entity attribute expressions (setters)
            calc.Register("sethp", "sethp(objid, value) - set entity HP",
                new ExpressionFactoryHelper<SetHpExp>());
            calc.Register("setmaxhp", "setmaxhp(objid, value) - set entity max HP",
                new ExpressionFactoryHelper<SetMaxHpExp>());
            calc.Register("setenergy", "setenergy(objid, value) - set entity energy",
                new ExpressionFactoryHelper<SetEnergyExp>());
            calc.Register("setmaxenergy", "setmaxenergy(objid, value) - set entity max energy",
                new ExpressionFactoryHelper<SetMaxEnergyExp>());
            calc.Register("setspeed", "setspeed(objid, value) - set entity speed",
                new ExpressionFactoryHelper<SetSpeedExp>());
            calc.Register("setlevel", "setlevel(objid, value) - set entity level",
                new ExpressionFactoryHelper<SetLevelExp>());
            calc.Register("setexp", "setexp(objid, value) - set entity exp",
                new ExpressionFactoryHelper<SetExpExp>());
            calc.Register("setattr", "setattr(objid, attrid, value) - set entity attribute",
                new ExpressionFactoryHelper<SetAttrExp>());
            calc.Register("setunitid", "setunitid(obj_id, unit_id) - set entity unit ID",
                new ExpressionFactoryHelper<SetUnitIdExp>());
            calc.Register("setplayerid", "setplayerid([objid,] leaderid) - set player or entity leader ID",
                new ExpressionFactoryHelper<SetPlayerIdExp>());

            // Register Entity attribute expressions (getters)
            calc.Register("gethp", "gethp(objid) - get entity HP",
                new ExpressionFactoryHelper<GetHpExp>());
            calc.Register("getmaxhp", "getmaxhp(objid) - get entity max HP",
                new ExpressionFactoryHelper<GetMaxHpExp>());
            calc.Register("getenergy", "getenergy(objid) - get entity energy",
                new ExpressionFactoryHelper<GetEnergyExp>());
            calc.Register("getmaxenergy", "getmaxenergy(objid) - get entity max energy",
                new ExpressionFactoryHelper<GetMaxEnergyExp>());
            calc.Register("getspeed", "getspeed(objid) - get entity speed",
                new ExpressionFactoryHelper<GetSpeedExp>());
            calc.Register("getlevel", "getlevel(objid) - get entity level",
                new ExpressionFactoryHelper<GetLevelExp>());
            calc.Register("getexp", "getexp(objid) - get entity exp",
                new ExpressionFactoryHelper<GetExpExp>());
            calc.Register("getcamp", "getcamp(objid) - get entity camp ID",
                new ExpressionFactoryHelper<GetCampExp>());
            calc.Register("getattr", "getattr(objid, attrid) - get entity attribute value",
                new ExpressionFactoryHelper<GetAttrExp>());

            // Register Position/Rotation/Scale expressions
            calc.Register("getpositionx", "getpositionx(obj_id[, local]) - get entity X position",
                new ExpressionFactoryHelper<GetPositionXExp>());
            calc.Register("getpositiony", "getpositiony(obj_id[, local]) - get entity Y position",
                new ExpressionFactoryHelper<GetPositionYExp>());
            calc.Register("getpositionz", "getpositionz(obj_id[, local]) - get entity Z position",
                new ExpressionFactoryHelper<GetPositionZExp>());
            calc.Register("getrotation", "getrotation(obj_id[, local]) - get entity rotation as Vector3",
                new ExpressionFactoryHelper<GetRotationExp>());
            calc.Register("getrotationx", "getrotationx(obj_id[, local]) - get entity rotation X",
                new ExpressionFactoryHelper<GetRotationXExp>());
            calc.Register("getrotationy", "getrotationy(obj_id[, local]) - get entity rotation Y",
                new ExpressionFactoryHelper<GetRotationYExp>());
            calc.Register("getrotationz", "getrotationz(obj_id[, local]) - get entity rotation Z",
                new ExpressionFactoryHelper<GetRotationZExp>());
            calc.Register("getscale", "getscale(obj_id) - get entity scale as Vector3",
                new ExpressionFactoryHelper<GetScaleExp>());
            calc.Register("getscalex", "getscalex(obj_id) - get entity scale X",
                new ExpressionFactoryHelper<GetScaleXExp>());
            calc.Register("getscaley", "getscaley(obj_id) - get entity scale Y",
                new ExpressionFactoryHelper<GetScaleYExp>());
            calc.Register("getscalez", "getscalez(obj_id) - get entity scale Z",
                new ExpressionFactoryHelper<GetScaleZExp>());
            calc.Register("position", "position(x, y, z) - create a Vector3",
                new ExpressionFactoryHelper<PositionExp>());
            calc.Register("rotation", "rotation(x, y, z) - create a rotation Vector3",
                new ExpressionFactoryHelper<RotationExp>());
            calc.Register("scale", "scale(x, y, z) - create a scale Vector3",
                new ExpressionFactoryHelper<ScaleExp>());

            // Register Direction and Relation expressions
            calc.Register("calcoffset", "calcoffset(obj_id, target_obj_id, offset) - calculate offset position relative to target",
                new ExpressionFactoryHelper<CalcOffsetExp>());
            calc.Register("calcdir", "calcdir(obj_id, target_obj_id) - calculate facing direction from obj to target",
                new ExpressionFactoryHelper<CalcDirExp>());
            calc.Register("isenemy", "isenemy(camp1, camp2) - check if camp1 is enemy of camp2",
                new ExpressionFactoryHelper<IsEnemyExp>());
            calc.Register("isfriend", "isfriend(camp1, camp2) - check if camp1 is friend of camp2",
                new ExpressionFactoryHelper<IsFriendExp>());

            // Register State control expressions
            calc.Register("markcontrolbystory", "markcontrolbystory(objid, true_or_false) - mark entity as controlled by story",
                new ExpressionFactoryHelper<MarkControlByStoryExp>());
            calc.Register("bornfinish", "bornfinish(objid) - finish born state",
                new ExpressionFactoryHelper<BornFinishExp>());
            calc.Register("deadfinish", "deadfinish(objid) - finish dead state",
                new ExpressionFactoryHelper<DeadFinishExp>());

            // Register Blackboard expressions
            calc.Register("blackboardclear", "blackboardclear() - clear the scene blackboard",
                new ExpressionFactoryHelper<BlackboardClearExp>());
            calc.Register("blackboardset", "blackboardset(name, value) - set a blackboard variable",
                new ExpressionFactoryHelper<BlackboardSetExp>());
            calc.Register("blackboardget", "blackboardget(name[, default_value]) - get a blackboard variable",
                new ExpressionFactoryHelper<BlackboardGetExp>());

            // Register ID conversion expressions
            calc.Register("getplayerid", "getplayerid() - get player entity object ID",
                new ExpressionFactoryHelper<GetPlayerIdExp>());
            calc.Register("unitid2objid", "unitid2objid(unitid) - convert unit ID to object ID",
                new ExpressionFactoryHelper<UnitId2ObjIdExp>());
            calc.Register("objid2unitid", "objid2unitid(objid) - convert object ID to unit ID",
                new ExpressionFactoryHelper<ObjId2UnitIdExp>());

            // Register Find expressions
            calc.Register("findobjid", "findobjid(type, position, range) - find nearest entity by type within range",
                new ExpressionFactoryHelper<FindObjIdExp>());
            calc.Register("findobjids", "findobjids(type, position, range) - find all entity IDs by type within range",
                new ExpressionFactoryHelper<FindObjIdsExp>());
            calc.Register("countdyingnpc", "countdyingnpc(campId[, relation]) - count dying NPCs by camp or relation",
                new ExpressionFactoryHelper<CountDyingNpcExp>());

            // Register existing expressions that use older patterns
            calc.Register("getgameobject", "getgameobject(objid) - get GameObject by entity object ID",
                new ExpressionFactoryHelper<GetGameObjectExp>());
            calc.Register("highlightprompt", "highlightprompt(info) - show a highlight prompt",
                new ExpressionFactoryHelper<HighlightPromptExp>());
            calc.Register("getstreamingassets", "getstreamingassets() - get streaming assets path",
                new ExpressionFactoryHelper<GetStreamingAssetsExp>());
            calc.Register("readfile", "readfile(path) - read file content",
                new ExpressionFactoryHelper<ReadFileExp>());
            calc.Register("getunitytype", "getunitytype(typeName) - get Unity type",
                new ExpressionFactoryHelper<GetUnityTypeExp>());
            calc.Register("getposition", "getposition(obj) - get position of object",
                new ExpressionFactoryHelper<GetPositionExp>());
            calc.Register("findallobjids", "findallobjids(position, radius) - find all object IDs within radius",
                new ExpressionFactoryHelper<FindAllObjIdsExp>());
            calc.Register("getchild", "getchild(obj, path) - get child GameObject by path",
                new ExpressionFactoryHelper<GetChildExp>());
            calc.Register("countnpc", "countnpc(campId) - count NPCs by camp ID",
                new ExpressionFactoryHelper<CountNpcExp>());

            // Register GM expressions (also available in normal story scripts)
            GmExpressionRegistrar.RegisterGmExpressions(calc);
        }
    }
}
