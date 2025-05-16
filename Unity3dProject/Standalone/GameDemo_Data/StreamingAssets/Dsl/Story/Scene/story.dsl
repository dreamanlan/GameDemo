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
		log("story_main start");
		setstoryskipped(0);
	};
	onmessage("start_game")
	{
        $ct = 0;
        $list = fromjson(readfile(getstreamingassets()+"/objlist.txt"));
        looplist($list){
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

		@scoreUi = getcomponent("Canvas/Panel/Score", "UnityEngine.UI.Text");
		@hpUi = getcomponent("Canvas/Panel/Hp", "UnityEngine.UI.Text");
		@gravityUi = getcomponent("Canvas/Panel/Gravity", "UnityEngine.UI.Text");

		sendmessage("StartScript","UiCameraEnable","UICamera",0);
		loadui("Option", "UI/Option", "Dsl/Story/Ui/option.dsl");
		loadui("Info", "UI/Info", "Dsl/Story/Ui/info.dsl");
		loadui("CenterInfo", "UI/CenterInfo", "Dsl/Story/Ui/centerinfo.dsl");
		loadui("Victory", "UI/Victory", "Dsl/Story/Ui/victory.dsl");
		loadui("Failed", "UI/Failed", "Dsl/Story/Ui/failed.dsl");
		wait(200);
		sendmessage("StartScript","UiCameraEnable","UICamera",1);

		createnpc(1001,vector3(50,0,50),0,"Player",0,"ai_player",list("Dsl/Ai/ailogic_player.dsl"));
		createnpc(1002,rndvector3(vector3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		createnpc(1003,rndvector3(vector3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		createnpc(1004,rndvector3(vector3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		createnpc(1005,rndvector3(vector3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		createnpc(1006,rndvector3(vector3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		createnpc(1007,rndvector3(vector3(50,0,50),9),0,"Npc",0,"ai_npc",list("Dsl/Ai/ailogic_npc.dsl"));
		npcsetcamp(1001,3);
		npcsetcamp(1002,4);
		npcsetcamp(1003,4);
		npcsetcamp(1004,4);
		npcsetcamp(1005,4);
		npcsetcamp(1006,4);
		npcsetcamp(1007,4);

		$speed = 5+4.0*6.0/(5.0+@level);
		$npcspeed = $speed+6;
		
		setspeed(unitid2objid(1001),$speed);
		setspeed(unitid2objid(1002),$npcspeed);
		setspeed(unitid2objid(1003),$npcspeed);
		setspeed(unitid2objid(1004),$npcspeed);
		setspeed(unitid2objid(1005),$npcspeed);
		setspeed(unitid2objid(1006),$npcspeed);
		setspeed(unitid2objid(1007),$npcspeed);
		
		wait(300);
		@leftCount = 2048;
		@totalCount = 2048;
		@scoreUi.text = ""+@score;
		$g = vector3(0,-(@totalCount*9.8)/@leftCount,0);
		getunitytype("Physics").gravity = $g;
		@gravityUi.text = ""+$g.Value.y;

		addcomponent(unitid2objid(1001), "StoryObject,Assembly-CSharp");
		addcomponent(unitid2objid(1002), "StoryObject,Assembly-CSharp");
		addcomponent(unitid2objid(1003), "StoryObject,Assembly-CSharp");
		addcomponent(unitid2objid(1004), "StoryObject,Assembly-CSharp");
		addcomponent(unitid2objid(1005), "StoryObject,Assembly-CSharp");
		addcomponent(unitid2objid(1006), "StoryObject,Assembly-CSharp");
		addcomponent(unitid2objid(1007), "StoryObject,Assembly-CSharp");
		npcanimation(1001,"idle2");
		npcanimation(1002,"idle2");
		setplayerid(unitid2objid(1001));
		sendmessage("Camera", "CameraFollow", unitid2objid(1001));
		firemessage("show_centerinfo",1,"重力山谷 (Level "+@level+")",200);
        sendmessage("StartScript", "PlayMusic", rndint(2,5));
		wait(2000);
		@noBuddy = 0;
		sendmessage("objs", "Play");
	};
	onmessage("on_hide_centerinfo")args($id)
	{
        sendmessage("Camera", "Fading", 0, 1000);
	};
	onmessage("notify_player")args($obj)
	{
		if(!isnull($obj)){
			if($obj.name=="obj1160"){
				sendmessage("StartScript", "PlayMusic", 1);
				firemessage("show_victory");
			}else{
				sendmessage("objs/"+$obj.name, "EnableGravity", 1);
				sendmessage("objs/"+$obj.name, "PlaySound", 0);
				if(!isnull($obj.transform) && $obj.transform.childCount==0){
					creategameobject("fire", "SceneFire", $obj){
						position(vector3(0,0,0));
					};
				};
			};
		};
	};
	onmessage("on_trigger_enter")args($name1,$collider1,$name2,$collider2)
	{
		sendmessage("objs/"+$name1, "PlaySound", 1);
		$pt = getposition($collider1.gameObject, 0);	
		$list = findallobjids($pt,5);
		if(!isnull($list)){
			looplist($list){
				$objid=$$;
				sethp($objid,gethp($objid)-100);
				sendmessagewithgameobject(getgameobject($objid),"TweenUiPrefabWithText","BlueText"+$objid,"UI/RedText",vector3(0,2,0),"","-100");
			};
		};
		@hpUi.text = ""+gethp(getplayerid());
		localconcurrentmessage("baozha", $pt);
		wait(1000);
		destroygameobject("objs/"+$name1+"/fire");
		destroygameobject("objs/"+$name1);
	};
	onmessage("on_trigger_exit")args($name1,$collider1,$name2,$collider2)
	{
		$tag = $collider1.tag;
		if($tag=="Red"){
			@score=@score+100;
			sendmessagewithgameobject(getgameobject(getplayerid()),"TweenUiPrefabWithText","RedText"+$objid,"UI/BlueText",vector3(1,2,0),"","+100");
		}elseif($tag=="Blue"){
			@score=@score+200;
			sendmessagewithgameobject(getgameobject(getplayerid()),"TweenUiPrefabWithText","RedText"+$objid,"UI/BlueText",vector3(1,2,0),"","+200");
		}elseif($tag=="LightRed"){
			@score=@score+400;
			sendmessagewithgameobject(getgameobject(getplayerid()),"TweenUiPrefabWithText","RedText"+$objid,"UI/BlueText",vector3(1,2,0),"","+400");
		}else{
			@score=@score+800;
			sendmessagewithgameobject(getgameobject(getplayerid()),"TweenUiPrefabWithText","RedText"+$objid,"UI/BlueText",vector3(1,2,0),"","+800");
		};
		@scoreUi.text = ""+@score;
		dec(@leftCount);
		if(@leftCount<1){
			@leftCount = 1;
		};
		$g = vector3(0,-(@totalCount*9.8)/@leftCount,0);
		getunitytype("Physics").gravity = $g;
		@gravityUi.text = ""+$g.Value.y;
	};
	onmessage("baozha")args($pt)
	{
		creategameobject("gbaozha", "gbaozha")obj("$obj"){
			position($pt);
		};
		wait(1000);
		destroygameobject($obj);
	};
	onmessage("obj_killed")
	{
		log("{0} blue:{1} red:{2}", @noBuddy, countnpc(3), countnpc(4));
		
		$speed = 5+4.0*6.0/(5.0+@level)*countnpc(4)/6;
		setspeed(getplayerid(),$speed);
		if(@noBuddy<=0){
			if(countnpc(4)<=0){
				@noBuddy = 1;
				firemessage("show_info", 1, "你的伙伴们全部牺牲了，你将不得不自己独自走完剩下的路！", 200);
			};
		};
	};
	onmessage("npc_killed", 1001)
	{
		sendmessage("StartScript", "PlayMusic", 0);
		firemessage("show_failed");
	};
	onmessage("on_victory_button_click")
	{
		inc(@level);
		changescene("Game");
	};
	onmessage("on_failed_button_click")
	{
		@score = 0;
		changescene("Game");
	};
};
