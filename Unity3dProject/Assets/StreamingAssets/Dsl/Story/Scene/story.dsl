story(story_main)
{
	local
	{
		@leftCount(2048);
		@totalCount(2048);
		@score(0);
		@level(1);
		@scoreUi(0);
		@gravityUi(0);
		@hpUi(0);
		@noBuddy(0);
	};
	onmessage("start")
	{
		//启动时默认会进行的处理，一般用来执行初始化
		printf("story_main start");
		set_story_skipped(0);
	};
	onmessage("start_game")
	{
        $ct = 0;
        $list = from_json(read_file(get_streaming_assets()+"/objlist.txt"));
        loop_list($list){
            $info = $$;
            $name = $info[0];
            $x = $info[1];
            $y = $info[2];
            $z = $info[3];
            inc($ct);
            if($ct%100==0){
                wait(0);
            };
        };

		@scoreUi = get_component("Canvas/Panel/Score", "UnityEngine.UI.Text");
		@hpUi = get_component("Canvas/Panel/Hp", "UnityEngine.UI.Text");
		@gravityUi = get_component("Canvas/Panel/Gravity", "UnityEngine.UI.Text");

		send_message("StartScript","UiCameraEnable","UICamera",0);
		load_ui("Option", "UI/Option", "Dsl/Story/Ui/option.dsl");
		load_ui("Info", "UI/Info", "Dsl/Story/Ui/info.dsl");
		load_ui("CenterInfo", "UI/CenterInfo", "Dsl/Story/Ui/centerinfo.dsl");
		load_ui("Victory", "UI/Victory", "Dsl/Story/Ui/victory.dsl");
		load_ui("Failed", "UI/Failed", "Dsl/Story/Ui/failed.dsl");
		wait(200);
		send_message("StartScript","UiCameraEnable","UICamera",1);

		create_npc(1001,vector_3(50,0,50),0,"Player",0,"ai_player",list("Dsl/Ai/ailogic_player.dsl"));
		create_npc(1002,rnd_vector_3(vector_3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		create_npc(1003,rnd_vector_3(vector_3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		create_npc(1004,rnd_vector_3(vector_3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		create_npc(1005,rnd_vector_3(vector_3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		create_npc(1006,rnd_vector_3(vector_3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		create_npc(1007,rnd_vector_3(vector_3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		npc_set_camp(1001,3);
		npc_set_camp(1002,4);
		npc_set_camp(1003,4);
		npc_set_camp(1004,4);
		npc_set_camp(1005,4);
		npc_set_camp(1006,4);
		npc_set_camp(1007,4);

		$speed = 5+4.0*6.0/(5.0+@level);
		$npcspeed = $speed+6;
		
		set_speed(unit_id_2_obj_id(1001),$speed);
		set_speed(unit_id_2_obj_id(1002),$npcspeed);
		set_speed(unit_id_2_obj_id(1003),$npcspeed);
		set_speed(unit_id_2_obj_id(1004),$npcspeed);
		set_speed(unit_id_2_obj_id(1005),$npcspeed);
		set_speed(unit_id_2_obj_id(1006),$npcspeed);
		set_speed(unit_id_2_obj_id(1007),$npcspeed);
		
		wait(300);
		@leftCount = 2048;
		@totalCount = 2048;
		@scoreUi.text = ""+@score;
		$g = vector_3(0,-(@totalCount*9.8)/@leftCount,0);
		get_unity_type("Physics").gravity = $g;
		@gravityUi.text = ""+$g.Value.y;

		add_component(unit_id_2_obj_id(1001), "StoryObject,Assembly-CSharp");
		add_component(unit_id_2_obj_id(1002), "StoryObject,Assembly-CSharp");
		add_component(unit_id_2_obj_id(1003), "StoryObject,Assembly-CSharp");
		add_component(unit_id_2_obj_id(1004), "StoryObject,Assembly-CSharp");
		add_component(unit_id_2_obj_id(1005), "StoryObject,Assembly-CSharp");
		add_component(unit_id_2_obj_id(1006), "StoryObject,Assembly-CSharp");
		add_component(unit_id_2_obj_id(1007), "StoryObject,Assembly-CSharp");
		npc_animation(1001,"idle2");
		npc_animation(1002,"idle2");
		set_player_id(unit_id_2_obj_id(1001));
		send_message("Camera", "CameraFollow", unit_id_2_obj_id(1001));
		fire_message("show_centerinfo",1,"重力山谷 (Level "+@level+")",200);
        send_message("StartScript", "PlayMusic", rnd_int(2,5));
		wait(2000);
		@noBuddy = 0;
		send_message("objs", "Play");
	};
	onmessage("on_hide_centerinfo")args($id)
	{
        send_message("Camera", "Fading", 0, 1000);
	};
	onmessage("notify_player")args($obj)
	{
		if(!is_null($obj)){
			if($obj.name=="obj1201"){
				send_message("StartScript", "PlayMusic", 1);
				//fire_message("show_victory");
			}else{
				send_message_with_game_object($obj, "EnableGravity", 1);
				send_message_with_game_object($obj, "PlaySound", 0);
				if(!is_null($obj.transform) && $obj.transform.childCount==0){
					create_game_object("fire", "SceneFire", $obj){
						position(vector_3(0,0,0));
					};
				};
			};
		};
	};
	onmessage("on_trigger_enter")args($obj1,$collider1,$obj2,$collider2,$pt1,$pt2,$time)
	{
		send_message_with_game_object($obj1, "PlaySound", 1);
		$pt = get_position($obj1, 0);
		if($pt.Value.y < -20){
			printf("please check obj {0} {1} {2} <= {3} {4} {5}", $obj1.name, $pt, time(), $pt1, $pt2, change_type($time*1000,"long"));
			//editor_break();
		};
		$list = find_all_obj_ids($pt,5);
		if(!is_null($list)){
			loop_list($list){
				$objid=$$;
				set_hp($objid,get_hp($objid)-100);
				printf("damage {0}", $objid);
				send_message_with_game_object(get_game_object($objid),"TweenUiPrefabWithText","BlueText"+$objid,"UI/RedText",vector_3(0,2,0),"","-100");
			};
		};
		@hpUi.text = ""+get_hp(get_player_id());
		local_concurrent_message("baozha", $pt, $obj1);
		wait(3000);
		destroy_game_object(get_child($obj1,"fire"));
		destroy_game_object($obj1);
	};
	onmessage("on_trigger_exit")args($obj1,$collider1,$obj2,$collider2,$pt1,$pt2,$time)
	{
		$tag = $obj1.tag;
		if($tag=="Red"){
			@score=@score+100;
			send_message_with_game_object(get_game_object(get_player_id()),"TweenUiPrefabWithText","RedText"+get_player_id(),"UI/BlueText",vector_3(1,2,0),"","+100");
		}elseif($tag=="Blue"){
			@score=@score+200;
			send_message_with_game_object(get_game_object(get_player_id()),"TweenUiPrefabWithText","RedText"+get_player_id(),"UI/BlueText",vector_3(1,2,0),"","+200");
		}elseif($tag=="LightRed"){
			@score=@score+400;
			send_message_with_game_object(get_game_object(get_player_id()),"TweenUiPrefabWithText","RedText"+get_player_id(),"UI/BlueText",vector_3(1,2,0),"","+400");
		}else{
			@score=@score+800;
			send_message_with_game_object(get_game_object(get_player_id()),"TweenUiPrefabWithText","RedText"+get_player_id(),"UI/BlueText",vector_3(1,2,0),"","+800");
		};
		@scoreUi.text = ""+@score;
		dec(@leftCount);
		if(@leftCount<1){
			@leftCount = 1;
		};
		$g = vector_3(0,-(@totalCount*9.8)/@leftCount,0);
		get_unity_type("Physics").gravity = $g;
		@gravityUi.text = ""+$g.Value.y;
	};
	onmessage("baozha")args($pt, $cobj)
	{
		printf("baozha => {0} {1} {2}", $pt, $cobj.name, get_position($cobj, 1));
		create_game_object("gbaozha", "gbaozha")obj("$obj"){
			position($pt);
		};
		wait(3000);
		destroy_game_object($obj);
	};
	onmessage("obj_killed")
	{
		printf("{0} blue:{1} red:{2}", @noBuddy, count_npc(3), count_npc(4));
		
		$speed = 5+4.0*6.0/(5.0+@level)*count_npc(4)/6;
		set_speed(get_player_id(),$speed);
		if(@noBuddy<=0){
			if(count_npc(4)<=0){
				@noBuddy = 1;
				fire_message("show_info", 1, "你的伙伴们全部牺牲了，你将不得不自己独自走完剩下的路！", 200);
			};
		};
	};
	onmessage("npc_killed", 1001)
	{
		send_message("StartScript", "PlayMusic", 0);
		fire_message("show_failed");
	};
	onmessage("on_victory_button_click")
	{
		inc(@level);
		change_scene("Game");
	};
	onmessage("on_failed_button_click")
	{
		@score = 0;
		change_scene("Game");
	};
};
