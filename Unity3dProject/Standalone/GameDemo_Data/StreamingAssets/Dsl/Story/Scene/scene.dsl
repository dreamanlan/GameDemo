script(scene_main)
{
    local
    {
        //定义玩家角色ObjId
        @playerObjId(0);
    };
    onmessage("start")
    {
	    start_story("story_main");
	    wait(2000);
		load_ui("Start", "UI/Start", "Dsl/Story/Ui/start.dsl");
    };
    onmessage("scene_loaded")
    {
        fire_message("start_game");
    };
    onmessage("move_to")args($x,$y,$z)
    {
        if(get_hp(unit_id_2_obj_id(1001))>0){
            npc_move(1001, vector3($x, $y, $z));
        };
    };
    onmessage("on_start_button_click")
    {
        change_scene("Game");
    };
};
