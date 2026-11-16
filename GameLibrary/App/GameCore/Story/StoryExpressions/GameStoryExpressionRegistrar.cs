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
            registry.Register("start_story", "start_story(story_id[, multiple]) - start a story",
                new ExpressionFactoryHelper<StartStoryExp>());
            registry.Register("stop_story", "stop_story(story_id[, multiple]) - stop a story",
                new ExpressionFactoryHelper<StopStoryExp>());
            registry.Register("wait_story", "wait_story(story_id1, story_id2, ...) - wait for stories to finish",
                new ExpressionFactoryHelper<WaitStoryExp>());
            registry.Register("pause_story", "pause_story(story_id) - pause a story",
                new ExpressionFactoryHelper<PauseStoryExp>());
            registry.Register("resume_story", "resume_story(story_id) - resume a paused story",
                new ExpressionFactoryHelper<ResumeStoryExp>());
            registry.Register("fire_message", "fire_message(msg, arg1, arg2, ...) - send a sequential message to the story system",
                new ExpressionFactoryHelper<FireMessageExp>());
            registry.Register("fire_concurrent_message", "fire_concurrent_message(msg, arg1, arg2, ...) - send a concurrent message to the story system",
                new ExpressionFactoryHelper<FireConcurrentMessageExp>());
            registry.Register("wait_all_message", "wait_all_message(msgid1, msgid2, ...) - wait for all messages",
                new ExpressionFactoryHelper<WaitAllMessageExp>());
            registry.Register("wait_all_message_handler", "wait_all_message_handler(msgid1, msgid2, ...) - wait for all message handlers",
                new ExpressionFactoryHelper<WaitAllMessageHandlerExp>());
            registry.Register("suspend_all_message_handler", "suspend_all_message_handler(msgid1, msgid2, ...) - suspend message handlers",
                new ExpressionFactoryHelper<SuspendAllMessageHandlerExp>());
            registry.Register("resume_all_message_handler", "resume_all_message_handler(msgid1, msgid2, ...) - resume message handlers",
                new ExpressionFactoryHelper<ResumeAllMessageHandlerExp>());
            registry.Register("send_ai_message", "send_ai_message(objid, msg, ...) - send AI message to object",
                new ExpressionFactoryHelper<SendAiMessageExp>());
            registry.Register("send_ai_concurrent_message", "send_ai_concurrent_message(objid, msg, ...) - send concurrent AI message",
                new ExpressionFactoryHelper<SendAiConcurrentMessageExp>());
            registry.Register("send_ai_namespaced_message", "send_ai_namespaced_message(objid, msg, arg1, arg2, ...) - send namespaced AI message to object",
                new ExpressionFactoryHelper<SendAiNamespacedMessageExp>());
            registry.Register("send_ai_concurrent_namespaced_message", "send_ai_concurrent_namespaced_message(objid, msg, ...) - send concurrent namespaced AI message",
                new ExpressionFactoryHelper<SendAiConcurrentNamespacedMessageExp>());
            registry.Register("set_story_variable", "set_story_variable(story_id[, namespace], name, value) - set a variable on another story instance",
                new ExpressionFactoryHelper<SetStoryVariableExp>());
            registry.Register("set_story_skipped", "set_story_skipped(0_or_1) - set story skipped state",
                new ExpressionFactoryHelper<SetStorySkippedExp>());
            registry.Register("set_story_speed_up", "set_story_speed_up(0_or_1) - set story speedup state",
                new ExpressionFactoryHelper<SetStorySpeedupExp>());
            registry.Register("is_story_skipped", "is_story_skipped() - check if story is skipped",
                new ExpressionFactoryHelper<IsStorySkippedExp>());

            // Register General utility expressions
            registry.Register("publish_event", "publish_event(ev_name, group, arg1, arg2, ...) - publish an event",
                new ExpressionFactoryHelper<PublishEventExp>());
            registry.Register("send_message", "send_message(obj, msg, ...) - send Unity message to GameObject",
                new ExpressionFactoryHelper<SendMessageExp>());
            registry.Register("send_message_with_tag", "send_message_with_tag(tag, msg, ...) - send Unity message to GameObjects by tag",
                new ExpressionFactoryHelper<SendMessageWithTagExp>());
            registry.Register("send_message_with_game_object", "send_message_with_game_object(obj, msg, ...) - send Unity message (supports GameObject and ID)",
                new ExpressionFactoryHelper<SendMessageWithGameObjectExp>());
            registry.Register("send_script_message", "send_script_message(msg, arg1, arg2, ...) - send a script message",
                new ExpressionFactoryHelper<SendScriptMessageExp>());

            // Register Time and Conversion expressions
            registry.Register("get_time", "get_time() - get Unity game time",
                new ExpressionFactoryHelper<GetTimeExp>());
            registry.Register("get_time_scale", "get_time_scale() - get Unity time scale",
                new ExpressionFactoryHelper<GetTimeScaleExp>());
            registry.Register("deg_2_rad", "deg_2_rad(degree) - convert degrees to radians",
                new ExpressionFactoryHelper<Deg2RadExp>());
            registry.Register("rad_2_deg", "rad_2_deg(radian) - convert radians to degrees",
                new ExpressionFactoryHelper<Rad2DegExp>());
            registry.Register("get_persistent_path", "get_persistent_path() - get persistent data path",
                new ExpressionFactoryHelper<GetPersistentPathExp>());

            // Register GameObject inspection expressions
            registry.Register("is_active", "is_active(obj) - check if GameObject is active (activeSelf)",
                new ExpressionFactoryHelper<IsActiveExp>());
            registry.Register("is_really_active", "is_really_active(obj) - check if GameObject is active in hierarchy",
                new ExpressionFactoryHelper<IsReallyActiveExp>());
            registry.Register("is_visible", "is_visible(obj) - check if object's renderer is visible",
                new ExpressionFactoryHelper<IsVisibleExp>());
            registry.Register("get_parent", "get_parent(obj) - get parent GameObject",
                new ExpressionFactoryHelper<GetParentExp>());

            // Register Type resolution expressions
            registry.Register("get_unity_ui_type", "get_unity_ui_type(name) - get UnityEngine.UI type by name",
                new ExpressionFactoryHelper<GetUnityUiTypeExp>());
            registry.Register("get_user_type", "get_user_type(name) - get user type by name",
                new ExpressionFactoryHelper<GetUserTypeExp>());

            // Register System instance expressions
            registry.Register("global", "global() - get GlobalVariables instance",
                new ExpressionFactoryHelper<GlobalExp>());
            registry.Register("scene", "scene() - get SceneSystem instance",
                new ExpressionFactoryHelper<SceneExp>());
            registry.Register("resource_system", "resource_system() - get ResourceSystem instance",
                new ExpressionFactoryHelper<ResourceSystemExp>());

            // Register Story system expressions
            registry.Register("get_story", "get_story(story_id[, namespace]) - get story instance",
                new ExpressionFactoryHelper<GetStoryExp>());
            registry.Register("get_story_variable", "get_story_variable(story_id, var_name[, namespace]) - get variable from another story",
                new ExpressionFactoryHelper<GetStoryVariableExp>());

            // Register Entity info expressions
            registry.Register("get_entity_info", "get_entity_info(objid) - get entity info by object ID",
                new ExpressionFactoryHelper<GetEntityInfoExp>());
            registry.Register("get_entity_view", "get_entity_view(viewid) - get entity view by view ID",
                new ExpressionFactoryHelper<GetEntityViewExp>());

            // Register GameObject expressions
            registry.Register("create_game_object", "create_game_object(name, prefab[, parent])[obj(varname)] { position(v3); rotation(v3); scale(v3); disable(t1,...); remove(t1,...); } - create GameObject",
                new ExpressionFactoryHelper<CreateGameObjectExp>());
            registry.Register("set_transform", "set_transform(obj, local_or_world) { position(v3); rotation(v3); scale(v3); } - set transform",
                new ExpressionFactoryHelper<SetTransformExp>());
            registry.Register("add_transform", "add_transform(obj, local_or_world[, position, rotation, scale]) - add transform values",
                new ExpressionFactoryHelper<AddTransformExp>());
            registry.Register("destroy_game_object", "destroy_game_object(path_or_obj) - destroy a GameObject",
                new ExpressionFactoryHelper<DestroyGameObjectExp>());
            registry.Register("auto_recycle", "auto_recycle(path_or_obj, enable) - add/remove GameObject from auto-recycle list",
                new ExpressionFactoryHelper<AutoRecycleExp>());
            registry.Register("set_parent", "set_parent(child, parent) - set GameObject parent",
                new ExpressionFactoryHelper<SetParentExp>());
            registry.Register("set_active", "set_active(path_or_obj, active) - set GameObject active state",
                new ExpressionFactoryHelper<SetActiveExp>());
            registry.Register("set_visible", "set_visible(path_or_obj, visible) - set GameObject visibility",
                new ExpressionFactoryHelper<SetVisibleExp>());
            registry.Register("add_component", "add_component(obj, component_type) - add component to GameObject",
                new ExpressionFactoryHelper<AddComponentExp>());
            registry.Register("remove_component", "remove_component(obj, component_type) - remove component from GameObject",
                new ExpressionFactoryHelper<RemoveComponentExp>());
            registry.Register("set_actor_scale", "set_actor_scale(objid, scale) - set actor scale",
                new ExpressionFactoryHelper<SetActorScaleExp>());

            // Register UI and Scene expressions
            registry.Register("open_url", "open_url(url) - open a URL",
                new ExpressionFactoryHelper<OpenUrlExp>());
            registry.Register("quit", "quit() - quit the application",
                new ExpressionFactoryHelper<QuitExp>());
            registry.Register("load_ui", "load_ui(name, prefab, dslfile[, dont_destroy_old]) - load a UI prefab",
                new ExpressionFactoryHelper<LoadUiExp>());
            registry.Register("bind_ui", "bind_ui(obj) { var(...); onevent(...); ... } - bind UI events and variables",
                new ExpressionFactoryHelper<BindUiExp>());
            registry.Register("change_scene", "change_scene(target_scene) - change to a different scene",
                new ExpressionFactoryHelper<ChangeSceneExp>());

            // Register Animation expressions
            registry.Register("game_object_animation", "game_object_animation(obj, anim_name) - play an animation",
                new ExpressionFactoryHelper<GameObjectAnimationExp>());
            registry.Register("game_object_animation_param", "game_object_animation_param(obj) { int(key,value); float(key,value); bool(key,value); trigger(key,value); ... } - set animator parameters",
                new ExpressionFactoryHelper<GameObjectAnimationParamExp>());
            registry.Register("npc_animation", "npc_animation(unit_id, anim[, normalized_time]) - play NPC animation",
                new ExpressionFactoryHelper<NpcAnimationExp>());
            registry.Register("npc_animation_param", "npc_animation_param(unit_id) { int(key,value); ... } - set NPC animator parameters",
                new ExpressionFactoryHelper<NpcAnimationParamExp>());
            registry.Register("obj_animation", "obj_animation(obj_id, anim[, normalized_time]) - play object animation",
                new ExpressionFactoryHelper<ObjAnimationExp>());
            registry.Register("obj_animation_param", "obj_animation_param(obj_id) { int(key,value); ... } - set object animator parameters",
                new ExpressionFactoryHelper<ObjAnimationParamExp>());
            registry.Register("set_anim_at_or_param", "set_anim_at_or_param(obj, type, name, value) - set animator parameter",
                new ExpressionFactoryHelper<SetAnimatorParamExp>());

            // Register NPC expressions
            registry.Register("create_npc", "create_npc(unit_id, pos, dir, model, type[, ai, aiParams]) - create NPC",
                new ExpressionFactoryHelper<CreateNpcExp>());
            registry.Register("destroy_npc", "destroy_npc(unit_id[, immediately]) - destroy NPC by unit ID",
                new ExpressionFactoryHelper<DestroyNpcExp>());
            registry.Register("destroy_npc_with_obj_id", "destroy_npc_with_obj_id(objid[, immediately]) - destroy NPC by object ID",
                new ExpressionFactoryHelper<DestroyNpcWithObjIdExp>());
            registry.Register("npc_face", "npc_face(unit_id, dir[, immediately]) - set NPC face direction",
                new ExpressionFactoryHelper<NpcFaceExp>());
            registry.Register("npc_move", "npc_move(unit_id, pos[, event]) - move NPC to position",
                new ExpressionFactoryHelper<NpcMoveExp>());
            registry.Register("npc_move_with_waypoints", "npc_move_with_waypoints(unit_id, waypoints[, event]) - move NPC along waypoints",
                new ExpressionFactoryHelper<NpcMoveWithWaypointsExp>());
            registry.Register("npc_stop", "npc_stop(unit_id) - stop NPC movement",
                new ExpressionFactoryHelper<NpcStopExp>());
            registry.Register("npc_enable_ai", "npc_enable_ai(unit_id, enable) - enable/disable NPC AI",
                new ExpressionFactoryHelper<NpcEnableAiExp>());
            registry.Register("npc_set_ai", "npc_set_ai(unit_id, ai_logic, ai_params) - set NPC AI",
                new ExpressionFactoryHelper<NpcSetAiExp>());
            registry.Register("npc_set_ai_target", "npc_set_ai_target(unit_id, target_id) - set NPC AI target",
                new ExpressionFactoryHelper<NpcSetAiTargetExp>());
            registry.Register("npc_set_camp", "npc_set_camp(unit_id, camp_id) - set NPC camp",
                new ExpressionFactoryHelper<NpcSetCampExp>());
            registry.Register("npc_attack", "npc_attack(unit_id[, target_unit_id]) - command NPC to attack",
                new ExpressionFactoryHelper<NpcAttackExp>());
            registry.Register("npc_get_npc_type", "npc_get_npc_type(unitid) - get NPC entity type",
                new ExpressionFactoryHelper<NpcGetNpcTypeExp>());
            registry.Register("npc_get_ai_param", "npc_get_ai_param(unitid, index) - get NPC AI parameter",
                new ExpressionFactoryHelper<NpcGetAiParamExp>());

            // Register Object expressions
            registry.Register("obj_face", "obj_face(obj_id, dir[, immediately]) - set object face direction",
                new ExpressionFactoryHelper<ObjFaceExp>());
            registry.Register("obj_move", "obj_move(obj_id, pos[, event]) - move object to position",
                new ExpressionFactoryHelper<ObjMoveExp>());
            registry.Register("obj_move_with_waypoints", "obj_move_with_waypoints(obj_id, waypoints[, event]) - move object along waypoints",
                new ExpressionFactoryHelper<ObjMoveWithWaypointsExp>());
            registry.Register("obj_stop", "obj_stop(obj_id) - stop object movement",
                new ExpressionFactoryHelper<ObjStopExp>());
            registry.Register("obj_enable_ai", "obj_enable_ai(obj_id, enable) - enable/disable object AI",
                new ExpressionFactoryHelper<ObjEnableAiExp>());
            registry.Register("obj_set_ai", "obj_set_ai(obj_id, ai_logic, ai_params) - set object AI",
                new ExpressionFactoryHelper<ObjSetAiExp>());
            registry.Register("obj_set_ai_target", "obj_set_ai_target(obj_id, target_id) - set object AI target",
                new ExpressionFactoryHelper<ObjSetAiTargetExp>());
            registry.Register("obj_attack", "obj_attack(obj_id[, target_obj_id]) - command object to attack",
                new ExpressionFactoryHelper<ObjAttackExp>());
            registry.Register("obj_set_camp", "obj_set_camp(objid, camp_id) - set entity camp",
                new ExpressionFactoryHelper<ObjSetCampExp>());
            registry.Register("obj_get_npc_type", "obj_get_npc_type(objid) - get entity type by object ID",
                new ExpressionFactoryHelper<ObjGetNpcTypeExp>());
            registry.Register("obj_get_ai_param", "obj_get_ai_param(objid, index) - get entity AI parameter",
                new ExpressionFactoryHelper<ObjGetAiParamExp>());

            // Register Entity attribute expressions (setters)
            registry.Register("set_hp", "set_hp(objid, value) - set entity HP",
                new ExpressionFactoryHelper<SetHpExp>());
            registry.Register("set_max_hp", "set_max_hp(objid, value) - set entity max HP",
                new ExpressionFactoryHelper<SetMaxHpExp>());
            registry.Register("set_energy", "set_energy(objid, value) - set entity energy",
                new ExpressionFactoryHelper<SetEnergyExp>());
            registry.Register("set_max_energy", "set_max_energy(objid, value) - set entity max energy",
                new ExpressionFactoryHelper<SetMaxEnergyExp>());
            registry.Register("set_speed", "set_speed(objid, value) - set entity speed",
                new ExpressionFactoryHelper<SetSpeedExp>());
            registry.Register("set_level", "set_level(objid, value) - set entity level",
                new ExpressionFactoryHelper<SetLevelExp>());
            registry.Register("set_exp", "set_exp(objid, value) - set entity exp",
                new ExpressionFactoryHelper<SetExpExp>());
            registry.Register("set_attr", "set_attr(objid, attrid, value) - set entity attribute",
                new ExpressionFactoryHelper<SetAttrExp>());
            registry.Register("set_unit_id", "set_unit_id(obj_id, unit_id) - set entity unit ID",
                new ExpressionFactoryHelper<SetUnitIdExp>());
            registry.Register("set_player_id", "set_player_id([objid,] leaderid) - set player or entity leader ID",
                new ExpressionFactoryHelper<SetPlayerIdExp>());

            // Register Entity attribute expressions (getters)
            registry.Register("get_hp", "get_hp(objid) - get entity HP",
                new ExpressionFactoryHelper<GetHpExp>());
            registry.Register("get_max_hp", "get_max_hp(objid) - get entity max HP",
                new ExpressionFactoryHelper<GetMaxHpExp>());
            registry.Register("get_energy", "get_energy(objid) - get entity energy",
                new ExpressionFactoryHelper<GetEnergyExp>());
            registry.Register("get_max_energy", "get_max_energy(objid) - get entity max energy",
                new ExpressionFactoryHelper<GetMaxEnergyExp>());
            registry.Register("get_speed", "get_speed(objid) - get entity speed",
                new ExpressionFactoryHelper<GetSpeedExp>());
            registry.Register("get_level", "get_level(objid) - get entity level",
                new ExpressionFactoryHelper<GetLevelExp>());
            registry.Register("get_exp", "get_exp(objid) - get entity exp",
                new ExpressionFactoryHelper<GetExpExp>());
            registry.Register("get_camp", "get_camp(objid) - get entity camp ID",
                new ExpressionFactoryHelper<GetCampExp>());
            registry.Register("get_attr", "get_attr(objid, attrid) - get entity attribute value",
                new ExpressionFactoryHelper<GetAttrExp>());

            // Register Position/Rotation/Scale expressions
            registry.Register("get_position_x", "get_position_x(obj_id[, local]) - get entity X position",
                new ExpressionFactoryHelper<GetPositionXExp>());
            registry.Register("get_position_y", "get_position_y(obj_id[, local]) - get entity Y position",
                new ExpressionFactoryHelper<GetPositionYExp>());
            registry.Register("get_position_z", "get_position_z(obj_id[, local]) - get entity Z position",
                new ExpressionFactoryHelper<GetPositionZExp>());
            registry.Register("get_rotation", "get_rotation(obj_id[, local]) - get entity rotation as Vector3",
                new ExpressionFactoryHelper<GetRotationExp>());
            registry.Register("get_rotation_x", "get_rotation_x(obj_id[, local]) - get entity rotation X",
                new ExpressionFactoryHelper<GetRotationXExp>());
            registry.Register("get_rotation_y", "get_rotation_y(obj_id[, local]) - get entity rotation Y",
                new ExpressionFactoryHelper<GetRotationYExp>());
            registry.Register("get_rotation_z", "get_rotation_z(obj_id[, local]) - get entity rotation Z",
                new ExpressionFactoryHelper<GetRotationZExp>());
            registry.Register("get_scale", "get_scale(obj_id) - get entity scale as Vector3",
                new ExpressionFactoryHelper<GetScaleExp>());
            registry.Register("get_scale_x", "get_scale_x(obj_id) - get entity scale X",
                new ExpressionFactoryHelper<GetScaleXExp>());
            registry.Register("get_scale_y", "get_scale_y(obj_id) - get entity scale Y",
                new ExpressionFactoryHelper<GetScaleYExp>());
            registry.Register("get_scale_z", "get_scale_z(obj_id) - get entity scale Z",
                new ExpressionFactoryHelper<GetScaleZExp>());
            registry.Register("position", "position(x, y, z) - create a Vector3",
                new ExpressionFactoryHelper<PositionExp>());
            registry.Register("rotation", "rotation(x, y, z) - create a rotation Vector3",
                new ExpressionFactoryHelper<RotationExp>());
            registry.Register("scale", "scale(x, y, z) - create a scale Vector3",
                new ExpressionFactoryHelper<ScaleExp>());

            // Register Direction and Relation expressions
            registry.Register("calc_offset", "calc_offset(obj_id, target_obj_id, offset) - calculate offset position relative to target",
                new ExpressionFactoryHelper<CalcOffsetExp>());
            registry.Register("calc_dir", "calc_dir(obj_id, target_obj_id) - calculate facing direction from obj to target",
                new ExpressionFactoryHelper<CalcDirExp>());
            registry.Register("is_enemy", "is_enemy(camp1, camp2) - check if camp1 is enemy of camp2",
                new ExpressionFactoryHelper<IsEnemyExp>());
            registry.Register("is_friend", "is_friend(camp1, camp2) - check if camp1 is friend of camp2",
                new ExpressionFactoryHelper<IsFriendExp>());

            // Register State control expressions
            registry.Register("mark_control_by_story", "mark_control_by_story(objid, true_or_false) - mark entity as controlled by story",
                new ExpressionFactoryHelper<MarkControlByStoryExp>());
            registry.Register("born_finish", "born_finish(objid) - finish born state",
                new ExpressionFactoryHelper<BornFinishExp>());
            registry.Register("dead_finish", "dead_finish(objid) - finish dead state",
                new ExpressionFactoryHelper<DeadFinishExp>());

            // Register Blackboard expressions
            registry.Register("blackboard_clear", "blackboard_clear() - clear the scene blackboard",
                new ExpressionFactoryHelper<BlackboardClearExp>());
            registry.Register("blackboard_set", "blackboard_set(name, value) - set a blackboard variable",
                new ExpressionFactoryHelper<BlackboardSetExp>());
            registry.Register("blackboard_get", "blackboard_get(name[, default_value]) - get a blackboard variable",
                new ExpressionFactoryHelper<BlackboardGetExp>());

            // Register ID conversion expressions
            registry.Register("get_player_id", "get_player_id() - get player entity object ID",
                new ExpressionFactoryHelper<GetPlayerIdExp>());
            registry.Register("unit_id_2_obj_id", "unit_id_2_obj_id(unitid) - convert unit ID to object ID",
                new ExpressionFactoryHelper<UnitId2ObjIdExp>());
            registry.Register("obj_id_2_unit_id", "obj_id_2_unit_id(objid) - convert object ID to unit ID",
                new ExpressionFactoryHelper<ObjId2UnitIdExp>());

            // Register Find expressions
            registry.Register("find_obj_id", "find_obj_id(type, position, range) - find nearest entity by type within range",
                new ExpressionFactoryHelper<FindObjIdExp>());
            registry.Register("find_obj_ids", "find_obj_ids(type, position, range) - find all entity IDs by type within range",
                new ExpressionFactoryHelper<FindObjIdsExp>());
            registry.Register("count_dying_npc", "count_dying_npc(campId[, relation]) - count dying NPCs by camp or relation",
                new ExpressionFactoryHelper<CountDyingNpcExp>());

            // Register existing expressions that use older patterns
            registry.Register("get_game_object", "get_game_object(objid) - get GameObject by entity object ID",
                new ExpressionFactoryHelper<GetGameObjectExp>());
            registry.Register("highlight_prompt", "highlight_prompt(info) - show a highlight prompt",
                new ExpressionFactoryHelper<HighlightPromptExp>());
            registry.Register("get_streaming_assets", "get_streaming_assets() - get streaming assets path",
                new ExpressionFactoryHelper<GetStreamingAssetsExp>());
            registry.Register("read_file", "read_file(path) - read file content",
                new ExpressionFactoryHelper<ReadFileExp>());
            registry.Register("get_unity_type", "get_unity_type(typeName) - get Unity type",
                new ExpressionFactoryHelper<GetUnityTypeExp>());
            registry.Register("get_position", "get_position(obj) - get position of object",
                new ExpressionFactoryHelper<GetPositionExp>());
            registry.Register("find_all_obj_ids", "find_all_obj_ids(position, radius) - find all object IDs within radius",
                new ExpressionFactoryHelper<FindAllObjIdsExp>());
            registry.Register("get_child", "get_child(obj, path) - get child GameObject by path",
                new ExpressionFactoryHelper<GetChildExp>());
            registry.Register("count_npc", "count_npc(campId) - count NPCs by camp ID",
                new ExpressionFactoryHelper<CountNpcExp>());

            // Register GM expressions (also available in normal story scripts)
            GmExpressionRegistrar.RegisterGmExpressions(registry);
        }
    }
}
