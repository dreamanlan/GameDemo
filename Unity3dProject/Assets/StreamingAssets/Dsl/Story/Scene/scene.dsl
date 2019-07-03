script(scene_main)
{
    local
    {
        //定义玩家角色ObjId
        @playerObjId(0);
    };
    onmessage("start")
    {
	      startstory("story_main");
	      wait(2000);
				loadui("Start", "UI/Start", "Dsl/Story/Ui/start.dsl");
    };
    onmessage("scene_loaded")
    {
        firemessage("start_game");
    };
    onmessage("move_to")args($x,$y,$z)
    {
        if(gethp(unitid2objid(1001))>0){
            npcmove(1001, vector3($x, $y, $z));
        };
    };
		onmessage("on_start_button_click")
		{
				changescene("Game");
		};
};
