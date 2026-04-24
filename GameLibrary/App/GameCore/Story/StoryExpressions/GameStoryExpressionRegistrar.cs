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
        public static void RegisterGameExpressions(DslCalculatorApiRegistry registry)
        {
            if (registry == null)
                return;

            // Register Story control expressions
            registry.Register("preload", "preload(dslfile1, dslfile2, ...) - preload story DSL files",
                new ExpressionFactoryHelper<PreloadExp>());
            registry.Register("startstory", "startstory(story_id[, multiple]) - start a story",
                new ExpressionFactoryHelper<StartStoryExp>());
            registry.Register("stopstory", "stopstory(story_id[, multiple]) - stop a story",
                new ExpressionFactoryHelper<StopStoryExp>());
            registry.Register("waitstory", "waitstory(story_id1, story_id2, ...) - wait for stories to finish",
                new ExpressionFactoryHelper<WaitStoryExp>());
            registry.Register("pausestory", "pausestory(story_id) - pause a story",
                new ExpressionFactoryHelper<PauseStoryExp>());
            registry.Register("resumestory", "resumestory(story_id) - resume a paused story",
                new ExpressionFactoryHelper<ResumeStoryExp>());
            registry.Register("firemessage", "firemessage(msg, arg1, arg2, ...) - send a sequential message to the story system",
                new ExpressionFactoryHelper<FireMessageExp>());
            registry.Register("fireconcurrentmessage", "fireconcurrentmessage(msg, arg1, arg2, ...) - send a concurrent message to the story system",
                new ExpressionFactoryHelper<FireConcurrentMessageExp>());
            registry.Register("waitallmessage", "waitallmessage(msgid1, msgid2, ...) - wait for all messages",
                new ExpressionFactoryHelper<WaitAllMessageExp>());
            registry.Register("waitallmessagehandler", "waitallmessagehandler(msgid1, msgid2, ...) - wait for all message handlers",
                new ExpressionFactoryHelper<WaitAllMessageHandlerExp>());
            registry.Register("suspendallmessagehandler", "suspendallmessagehandler(msgid1, msgid2, ...) - suspend message handlers",
                new ExpressionFactoryHelper<SuspendAllMessageHandlerExp>());
            registry.Register("resumeallmessagehandler", "resumeallmessagehandler(msgid1, msgid2, ...) - resume message handlers",
                new ExpressionFactoryHelper<ResumeAllMessageHandlerExp>());
            registry.Register("sendaimessage", "sendaimessage(objid, msg, ...) - send AI message to object",
                new ExpressionFactoryHelper<SendAiMessageExp>());
            registry.Register("sendaiconcurrentmessage", "sendaiconcurrentmessage(objid, msg, ...) - send concurrent AI message",
                new ExpressionFactoryHelper<SendAiConcurrentMessageExp>());
            registry.Register("sendainamespacedmessage", "sendainamespacedmessage(objid, msg, arg1, arg2, ...) - send namespaced AI message to object",
                new ExpressionFactoryHelper<SendAiNamespacedMessageExp>());
            registry.Register("sendaiconcurrentnamespacedmessage", "sendaiconcurrentnamespacedmessage(objid, msg, ...) - send concurrent namespaced AI message",
                new ExpressionFactoryHelper<SendAiConcurrentNamespacedMessageExp>());
            registry.Register("setstoryvariable", "setstoryvariable(story_id[, namespace], name, value) - set a variable on another story instance",
                new ExpressionFactoryHelper<SetStoryVariableExp>());
            registry.Register("setstoryskipped", "setstoryskipped(0_or_1) - set story skipped state",
                new ExpressionFactoryHelper<SetStorySkippedExp>());
            registry.Register("setstoryspeedup", "setstoryspeedup(0_or_1) - set story speedup state",
                new ExpressionFactoryHelper<SetStorySpeedupExp>());
            registry.Register("isstoryskipped", "isstoryskipped() - check if story is skipped",
                new ExpressionFactoryHelper<IsStorySkippedExp>());

            // Register General utility expressions
            registry.Register("publishevent", "publishevent(ev_name, group, arg1, arg2, ...) - publish an event",
                new ExpressionFactoryHelper<PublishEventExp>());
            registry.Register("sendmessage", "sendmessage(obj, msg, ...) - send Unity message to GameObject",
                new ExpressionFactoryHelper<SendMessageExp>());
            registry.Register("sendmessagewithtag", "sendmessagewithtag(tag, msg, ...) - send Unity message to GameObjects by tag",
                new ExpressionFactoryHelper<SendMessageWithTagExp>());
            registry.Register("sendmessagewithgameobject", "sendmessagewithgameobject(obj, msg, ...) - send Unity message (supports GameObject and ID)",
                new ExpressionFactoryHelper<SendMessageWithGameObjectExp>());
            registry.Register("sendscriptmessage", "sendscriptmessage(msg, arg1, arg2, ...) - send a script message",
                new ExpressionFactoryHelper<SendScriptMessageExp>());

            // Register Time and Conversion expressions
            registry.Register("gettime", "gettime() - get Unity game time",
                new ExpressionFactoryHelper<GetTimeExp>());
            registry.Register("gettimescale", "gettimescale() - get Unity time scale",
                new ExpressionFactoryHelper<GetTimeScaleExp>());
            registry.Register("deg2rad", "deg2rad(degree) - convert degrees to radians",
                new ExpressionFactoryHelper<Deg2RadExp>());
            registry.Register("rad2deg", "rad2deg(radian) - convert radians to degrees",
                new ExpressionFactoryHelper<Rad2DegExp>());
            registry.Register("getpersistentpath", "getpersistentpath() - get persistent data path",
                new ExpressionFactoryHelper<GetPersistentPathExp>());

            // Register GameObject inspection expressions
            registry.Register("isactive", "isactive(obj) - check if GameObject is active (activeSelf)",
                new ExpressionFactoryHelper<IsActiveExp>());
            registry.Register("isreallyactive", "isreallyactive(obj) - check if GameObject is active in hierarchy",
                new ExpressionFactoryHelper<IsReallyActiveExp>());
            registry.Register("isvisible", "isvisible(obj) - check if object's renderer is visible",
                new ExpressionFactoryHelper<IsVisibleExp>());
            registry.Register("getparent", "getparent(obj) - get parent GameObject",
                new ExpressionFactoryHelper<GetParentExp>());

            // Register Type resolution expressions
            registry.Register("getunityuitype", "getunityuitype(name) - get UnityEngine.UI type by name",
                new ExpressionFactoryHelper<GetUnityUiTypeExp>());
            registry.Register("getusertype", "getusertype(name) - get user type by name",
                new ExpressionFactoryHelper<GetUserTypeExp>());

            // Register System instance expressions
            registry.Register("global", "global() - get GlobalVariables instance",
                new ExpressionFactoryHelper<GlobalExp>());
            registry.Register("scene", "scene() - get SceneSystem instance",
                new ExpressionFactoryHelper<SceneExp>());
            registry.Register("resourcesystem", "resourcesystem() - get ResourceSystem instance",
                new ExpressionFactoryHelper<ResourceSystemExp>());

            // Register Story system expressions
            registry.Register("getstory", "getstory(story_id[, namespace]) - get story instance",
                new ExpressionFactoryHelper<GetStoryExp>());
            registry.Register("getstoryvariable", "getstoryvariable(story_id, var_name[, namespace]) - get variable from another story",
                new ExpressionFactoryHelper<GetStoryVariableExp>());

            // Register Entity info expressions
            registry.Register("getentityinfo", "getentityinfo(objid) - get entity info by object ID",
                new ExpressionFactoryHelper<GetEntityInfoExp>());
            registry.Register("getentityview", "getentityview(viewid) - get entity view by view ID",
                new ExpressionFactoryHelper<GetEntityViewExp>());

            // Register GameObject expressions
            registry.Register("creategameobject", "creategameobject(name, prefab[, parent])[obj(varname)] { position(v3); rotation(v3); scale(v3); disable(t1,...); remove(t1,...); } - create GameObject",
                new ExpressionFactoryHelper<CreateGameObjectExp>());
            registry.Register("settransform", "settransform(obj, local_or_world) { position(v3); rotation(v3); scale(v3); } - set transform",
                new ExpressionFactoryHelper<SetTransformExp>());
            registry.Register("addtransform", "addtransform(obj, local_or_world[, position, rotation, scale]) - add transform values",
                new ExpressionFactoryHelper<AddTransformExp>());
            registry.Register("destroygameobject", "destroygameobject(path_or_obj) - destroy a GameObject",
                new ExpressionFactoryHelper<DestroyGameObjectExp>());
            registry.Register("autorecycle", "autorecycle(path_or_obj, enable) - add/remove GameObject from auto-recycle list",
                new ExpressionFactoryHelper<AutoRecycleExp>());
            registry.Register("setparent", "setparent(child, parent) - set GameObject parent",
                new ExpressionFactoryHelper<SetParentExp>());
            registry.Register("setactive", "setactive(path_or_obj, active) - set GameObject active state",
                new ExpressionFactoryHelper<SetActiveExp>());
            registry.Register("setvisible", "setvisible(path_or_obj, visible) - set GameObject visibility",
                new ExpressionFactoryHelper<SetVisibleExp>());
            registry.Register("addcomponent", "addcomponent(obj, component_type) - add component to GameObject",
                new ExpressionFactoryHelper<AddComponentExp>());
            registry.Register("removecomponent", "removecomponent(obj, component_type) - remove component from GameObject",
                new ExpressionFactoryHelper<RemoveComponentExp>());
            registry.Register("setactorscale", "setactorscale(objid, scale) - set actor scale",
                new ExpressionFactoryHelper<SetActorScaleExp>());

            // Register UI and Scene expressions
            registry.Register("openurl", "openurl(url) - open a URL",
                new ExpressionFactoryHelper<OpenUrlExp>());
            registry.Register("quit", "quit() - quit the application",
                new ExpressionFactoryHelper<QuitExp>());
            registry.Register("loadui", "loadui(name, prefab, dslfile[, dont_destroy_old]) - load a UI prefab",
                new ExpressionFactoryHelper<LoadUiExp>());
            registry.Register("bindui", "bindui(obj) { var(...); onevent(...); ... } - bind UI events and variables",
                new ExpressionFactoryHelper<BindUiExp>());
            registry.Register("changescene", "changescene(target_scene) - change to a different scene",
                new ExpressionFactoryHelper<ChangeSceneExp>());

            // Register Animation expressions
            registry.Register("gameobjectanimation", "gameobjectanimation(obj, anim_name) - play an animation",
                new ExpressionFactoryHelper<GameObjectAnimationExp>());
            registry.Register("gameobjectanimationparam", "gameobjectanimationparam(obj) { int(key,value); float(key,value); bool(key,value); trigger(key,value); ... } - set animator parameters",
                new ExpressionFactoryHelper<GameObjectAnimationParamExp>());
            registry.Register("npcanimation", "npcanimation(unit_id, anim[, normalized_time]) - play NPC animation",
                new ExpressionFactoryHelper<NpcAnimationExp>());
            registry.Register("npcanimationparam", "npcanimationparam(unit_id) { int(key,value); ... } - set NPC animator parameters",
                new ExpressionFactoryHelper<NpcAnimationParamExp>());
            registry.Register("objanimation", "objanimation(obj_id, anim[, normalized_time]) - play object animation",
                new ExpressionFactoryHelper<ObjAnimationExp>());
            registry.Register("objanimationparam", "objanimationparam(obj_id) { int(key,value); ... } - set object animator parameters",
                new ExpressionFactoryHelper<ObjAnimationParamExp>());
            registry.Register("setanimatorparam", "setanimatorparam(obj, type, name, value) - set animator parameter",
                new ExpressionFactoryHelper<SetAnimatorParamExp>());

            // Register NPC expressions
            registry.Register("createnpc", "createnpc(unit_id, pos, dir, model, type[, ai, aiParams]) - create NPC",
                new ExpressionFactoryHelper<CreateNpcExp>());
            registry.Register("destroynpc", "destroynpc(unit_id[, immediately]) - destroy NPC by unit ID",
                new ExpressionFactoryHelper<DestroyNpcExp>());
            registry.Register("destroynpcwithobjid", "destroynpcwithobjid(objid[, immediately]) - destroy NPC by object ID",
                new ExpressionFactoryHelper<DestroyNpcWithObjIdExp>());
            registry.Register("npcface", "npcface(unit_id, dir[, immediately]) - set NPC face direction",
                new ExpressionFactoryHelper<NpcFaceExp>());
            registry.Register("npcmove", "npcmove(unit_id, pos[, event]) - move NPC to position",
                new ExpressionFactoryHelper<NpcMoveExp>());
            registry.Register("npcmovewithwaypoints", "npcmovewithwaypoints(unit_id, waypoints[, event]) - move NPC along waypoints",
                new ExpressionFactoryHelper<NpcMoveWithWaypointsExp>());
            registry.Register("npcstop", "npcstop(unit_id) - stop NPC movement",
                new ExpressionFactoryHelper<NpcStopExp>());
            registry.Register("npcenableai", "npcenableai(unit_id, enable) - enable/disable NPC AI",
                new ExpressionFactoryHelper<NpcEnableAiExp>());
            registry.Register("npcsetai", "npcsetai(unit_id, ai_logic, ai_params) - set NPC AI",
                new ExpressionFactoryHelper<NpcSetAiExp>());
            registry.Register("npcsetaitarget", "npcsetaitarget(unit_id, target_id) - set NPC AI target",
                new ExpressionFactoryHelper<NpcSetAiTargetExp>());
            registry.Register("npcsetcamp", "npcsetcamp(unit_id, camp_id) - set NPC camp",
                new ExpressionFactoryHelper<NpcSetCampExp>());
            registry.Register("npcattack", "npcattack(unit_id[, target_unit_id]) - command NPC to attack",
                new ExpressionFactoryHelper<NpcAttackExp>());
            registry.Register("npcgetnpctype", "npcgetnpctype(unitid) - get NPC entity type",
                new ExpressionFactoryHelper<NpcGetNpcTypeExp>());
            registry.Register("npcgetaiparam", "npcgetaiparam(unitid, index) - get NPC AI parameter",
                new ExpressionFactoryHelper<NpcGetAiParamExp>());

            // Register Object expressions
            registry.Register("objface", "objface(obj_id, dir[, immediately]) - set object face direction",
                new ExpressionFactoryHelper<ObjFaceExp>());
            registry.Register("objmove", "objmove(obj_id, pos[, event]) - move object to position",
                new ExpressionFactoryHelper<ObjMoveExp>());
            registry.Register("objmovewithwaypoints", "objmovewithwaypoints(obj_id, waypoints[, event]) - move object along waypoints",
                new ExpressionFactoryHelper<ObjMoveWithWaypointsExp>());
            registry.Register("objstop", "objstop(obj_id) - stop object movement",
                new ExpressionFactoryHelper<ObjStopExp>());
            registry.Register("objenableai", "objenableai(obj_id, enable) - enable/disable object AI",
                new ExpressionFactoryHelper<ObjEnableAiExp>());
            registry.Register("objsetai", "objsetai(obj_id, ai_logic, ai_params) - set object AI",
                new ExpressionFactoryHelper<ObjSetAiExp>());
            registry.Register("objsetaitarget", "objsetaitarget(obj_id, target_id) - set object AI target",
                new ExpressionFactoryHelper<ObjSetAiTargetExp>());
            registry.Register("objattack", "objattack(obj_id[, target_obj_id]) - command object to attack",
                new ExpressionFactoryHelper<ObjAttackExp>());
            registry.Register("objsetcamp", "objsetcamp(objid, camp_id) - set entity camp",
                new ExpressionFactoryHelper<ObjSetCampExp>());
            registry.Register("objgetnpctype", "objgetnpctype(objid) - get entity type by object ID",
                new ExpressionFactoryHelper<ObjGetNpcTypeExp>());
            registry.Register("objgetaiparam", "objgetaiparam(objid, index) - get entity AI parameter",
                new ExpressionFactoryHelper<ObjGetAiParamExp>());

            // Register Entity attribute expressions (setters)
            registry.Register("sethp", "sethp(objid, value) - set entity HP",
                new ExpressionFactoryHelper<SetHpExp>());
            registry.Register("setmaxhp", "setmaxhp(objid, value) - set entity max HP",
                new ExpressionFactoryHelper<SetMaxHpExp>());
            registry.Register("setenergy", "setenergy(objid, value) - set entity energy",
                new ExpressionFactoryHelper<SetEnergyExp>());
            registry.Register("setmaxenergy", "setmaxenergy(objid, value) - set entity max energy",
                new ExpressionFactoryHelper<SetMaxEnergyExp>());
            registry.Register("setspeed", "setspeed(objid, value) - set entity speed",
                new ExpressionFactoryHelper<SetSpeedExp>());
            registry.Register("setlevel", "setlevel(objid, value) - set entity level",
                new ExpressionFactoryHelper<SetLevelExp>());
            registry.Register("setexp", "setexp(objid, value) - set entity exp",
                new ExpressionFactoryHelper<SetExpExp>());
            registry.Register("setattr", "setattr(objid, attrid, value) - set entity attribute",
                new ExpressionFactoryHelper<SetAttrExp>());
            registry.Register("setunitid", "setunitid(obj_id, unit_id) - set entity unit ID",
                new ExpressionFactoryHelper<SetUnitIdExp>());
            registry.Register("setplayerid", "setplayerid([objid,] leaderid) - set player or entity leader ID",
                new ExpressionFactoryHelper<SetPlayerIdExp>());

            // Register Entity attribute expressions (getters)
            registry.Register("gethp", "gethp(objid) - get entity HP",
                new ExpressionFactoryHelper<GetHpExp>());
            registry.Register("getmaxhp", "getmaxhp(objid) - get entity max HP",
                new ExpressionFactoryHelper<GetMaxHpExp>());
            registry.Register("getenergy", "getenergy(objid) - get entity energy",
                new ExpressionFactoryHelper<GetEnergyExp>());
            registry.Register("getmaxenergy", "getmaxenergy(objid) - get entity max energy",
                new ExpressionFactoryHelper<GetMaxEnergyExp>());
            registry.Register("getspeed", "getspeed(objid) - get entity speed",
                new ExpressionFactoryHelper<GetSpeedExp>());
            registry.Register("getlevel", "getlevel(objid) - get entity level",
                new ExpressionFactoryHelper<GetLevelExp>());
            registry.Register("getexp", "getexp(objid) - get entity exp",
                new ExpressionFactoryHelper<GetExpExp>());
            registry.Register("getcamp", "getcamp(objid) - get entity camp ID",
                new ExpressionFactoryHelper<GetCampExp>());
            registry.Register("getattr", "getattr(objid, attrid) - get entity attribute value",
                new ExpressionFactoryHelper<GetAttrExp>());

            // Register Position/Rotation/Scale expressions
            registry.Register("getpositionx", "getpositionx(obj_id[, local]) - get entity X position",
                new ExpressionFactoryHelper<GetPositionXExp>());
            registry.Register("getpositiony", "getpositiony(obj_id[, local]) - get entity Y position",
                new ExpressionFactoryHelper<GetPositionYExp>());
            registry.Register("getpositionz", "getpositionz(obj_id[, local]) - get entity Z position",
                new ExpressionFactoryHelper<GetPositionZExp>());
            registry.Register("getrotation", "getrotation(obj_id[, local]) - get entity rotation as Vector3",
                new ExpressionFactoryHelper<GetRotationExp>());
            registry.Register("getrotationx", "getrotationx(obj_id[, local]) - get entity rotation X",
                new ExpressionFactoryHelper<GetRotationXExp>());
            registry.Register("getrotationy", "getrotationy(obj_id[, local]) - get entity rotation Y",
                new ExpressionFactoryHelper<GetRotationYExp>());
            registry.Register("getrotationz", "getrotationz(obj_id[, local]) - get entity rotation Z",
                new ExpressionFactoryHelper<GetRotationZExp>());
            registry.Register("getscale", "getscale(obj_id) - get entity scale as Vector3",
                new ExpressionFactoryHelper<GetScaleExp>());
            registry.Register("getscalex", "getscalex(obj_id) - get entity scale X",
                new ExpressionFactoryHelper<GetScaleXExp>());
            registry.Register("getscaley", "getscaley(obj_id) - get entity scale Y",
                new ExpressionFactoryHelper<GetScaleYExp>());
            registry.Register("getscalez", "getscalez(obj_id) - get entity scale Z",
                new ExpressionFactoryHelper<GetScaleZExp>());
            registry.Register("position", "position(x, y, z) - create a Vector3",
                new ExpressionFactoryHelper<PositionExp>());
            registry.Register("rotation", "rotation(x, y, z) - create a rotation Vector3",
                new ExpressionFactoryHelper<RotationExp>());
            registry.Register("scale", "scale(x, y, z) - create a scale Vector3",
                new ExpressionFactoryHelper<ScaleExp>());

            // Register Direction and Relation expressions
            registry.Register("calcoffset", "calcoffset(obj_id, target_obj_id, offset) - calculate offset position relative to target",
                new ExpressionFactoryHelper<CalcOffsetExp>());
            registry.Register("calcdir", "calcdir(obj_id, target_obj_id) - calculate facing direction from obj to target",
                new ExpressionFactoryHelper<CalcDirExp>());
            registry.Register("isenemy", "isenemy(camp1, camp2) - check if camp1 is enemy of camp2",
                new ExpressionFactoryHelper<IsEnemyExp>());
            registry.Register("isfriend", "isfriend(camp1, camp2) - check if camp1 is friend of camp2",
                new ExpressionFactoryHelper<IsFriendExp>());

            // Register State control expressions
            registry.Register("markcontrolbystory", "markcontrolbystory(objid, true_or_false) - mark entity as controlled by story",
                new ExpressionFactoryHelper<MarkControlByStoryExp>());
            registry.Register("bornfinish", "bornfinish(objid) - finish born state",
                new ExpressionFactoryHelper<BornFinishExp>());
            registry.Register("deadfinish", "deadfinish(objid) - finish dead state",
                new ExpressionFactoryHelper<DeadFinishExp>());

            // Register Blackboard expressions
            registry.Register("blackboardclear", "blackboardclear() - clear the scene blackboard",
                new ExpressionFactoryHelper<BlackboardClearExp>());
            registry.Register("blackboardset", "blackboardset(name, value) - set a blackboard variable",
                new ExpressionFactoryHelper<BlackboardSetExp>());
            registry.Register("blackboardget", "blackboardget(name[, default_value]) - get a blackboard variable",
                new ExpressionFactoryHelper<BlackboardGetExp>());

            // Register ID conversion expressions
            registry.Register("getplayerid", "getplayerid() - get player entity object ID",
                new ExpressionFactoryHelper<GetPlayerIdExp>());
            registry.Register("unitid2objid", "unitid2objid(unitid) - convert unit ID to object ID",
                new ExpressionFactoryHelper<UnitId2ObjIdExp>());
            registry.Register("objid2unitid", "objid2unitid(objid) - convert object ID to unit ID",
                new ExpressionFactoryHelper<ObjId2UnitIdExp>());

            // Register Find expressions
            registry.Register("findobjid", "findobjid(type, position, range) - find nearest entity by type within range",
                new ExpressionFactoryHelper<FindObjIdExp>());
            registry.Register("findobjids", "findobjids(type, position, range) - find all entity IDs by type within range",
                new ExpressionFactoryHelper<FindObjIdsExp>());
            registry.Register("countdyingnpc", "countdyingnpc(campId[, relation]) - count dying NPCs by camp or relation",
                new ExpressionFactoryHelper<CountDyingNpcExp>());

            // Register existing expressions that use older patterns
            registry.Register("getgameobject", "getgameobject(objid) - get GameObject by entity object ID",
                new ExpressionFactoryHelper<GetGameObjectExp>());
            registry.Register("highlightprompt", "highlightprompt(info) - show a highlight prompt",
                new ExpressionFactoryHelper<HighlightPromptExp>());
            registry.Register("getstreamingassets", "getstreamingassets() - get streaming assets path",
                new ExpressionFactoryHelper<GetStreamingAssetsExp>());
            registry.Register("readfile", "readfile(path) - read file content",
                new ExpressionFactoryHelper<ReadFileExp>());
            registry.Register("getunitytype", "getunitytype(typeName) - get Unity type",
                new ExpressionFactoryHelper<GetUnityTypeExp>());
            registry.Register("getposition", "getposition(obj) - get position of object",
                new ExpressionFactoryHelper<GetPositionExp>());
            registry.Register("findallobjids", "findallobjids(position, radius) - find all object IDs within radius",
                new ExpressionFactoryHelper<FindAllObjIdsExp>());
            registry.Register("getchild", "getchild(obj, path) - get child GameObject by path",
                new ExpressionFactoryHelper<GetChildExp>());
            registry.Register("countnpc", "countnpc(campId) - count NPCs by camp ID",
                new ExpressionFactoryHelper<CountNpcExp>());

            // Register GM expressions (also available in normal story scripts)
            GmExpressionRegistrar.RegisterGmExpressions(registry);
        }
    }
}
