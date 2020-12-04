defapi(storybreak,"Common/断点");
defapi(EnterDramaState,"Common/剧情开始");
defapi(EnterDramaStateWithFadeOut,"Common/剧情开始（带黑屏）"){
	time(float, "时间"){
		initval(0);
	};
};
defapi(CreatePlayer,"Common/准备玩家");
defapi(OutDramaState,"Common/剧情结束"){
	notReset(int, "不重置主相机"){
		initval(0);
	};
};
defapi(NotifyStoryStop,"Common/通知剧情结束");
defapi(setstorystate,"Common/设置剧情状态"){
	state(int, "值"){
		initval(1);
	};
};
defapi(setdramastate,"Common/设置剧情模式"){
	state(int, "值"){
		initval(1);
	};
};
defapi(setstoryskipped,"Common/设置剧情跳过"){
	state(int, "值"){
		initval(0);
	};
	time(int, "延迟生效时间"){
		initval(0);
	};
};
defapi(setstoryspeedup,"Common/设置剧情加速"){
	state(int, "值"){
		initval(0);
	};
	time(int, "延迟生效时间"){
		initval(0);
	};
};
defapi(defplayer,"Common/定义玩家NPC配置"){
	npcTableId(int, "Npc配置ID"){
		initval(10100);
	};
};
defapi(propset,"变量与条件/设置变量值"){
    name(string, "变量名");
	val(int, "变量值"){
		initval(0);
	};
};
defapi(LocalMessageWithCondition,"变量与条件/条件消息"){
    msg(string, "消息"){
        semantic(message);  
    };
	condition(int, "条件"){
		semantic(variable);
		initval(@var_abc==1);
	};
};
defapi(ShowHideNpc, "Npc/显示隐藏Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	show(int, "显示隐藏"){
		initval(0);
	};
};
defapi(setactive, "Obj/显示隐藏Obj"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	show(int, "显示隐藏"){
		initval(0);
	};
};
defapi(ShowHideNpcChild, "Npc/显示隐藏Npc子结点"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	path(string, "子结点");
	show(int, "显示隐藏"){
		initval(0);
	};
};
defapi(ShowHideObjChild, "Obj/显示隐藏Obj子结点"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	path(string, "子结点");
	show(int, "显示隐藏"){
		initval(0);
	};
};
defapi(npcenablerope, "Npc/开关物理布料"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	bone(string, "骨骼"){
	    initval("");
	};
	enable(int, "1-打开0-关闭"){
		initval(1);
	};
};
defapi(objenablerope, "Obj/开关物理布料"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	bone(string, "骨骼"){
	    initval("");
	};
	enable(int, "1-打开0-关闭"){
		initval(1);
	};
};
defapi(npcenableropegroup, "Npc/开关物理布料组"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	group(string, "布料组");
	enable(int, "1-打开0-关闭"){
		initval(1);
	};
};
defapi(objenableropegroup, "Obj/开关物理布料组"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	group(string, "布料组");
	enable(int, "1-打开0-关闭"){
		initval(1);
	};
};
defapi(npchideweapon, "Npc/隐藏Npc武器"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	type(int, "类型"){	    
		semantic(popup);
		option("名剑",0);
		option("主武器",1);
		option("副武器",2);
		initval(0);
	};
	show(int, "1-隐藏0-显示"){
		initval(1);
	};
};
defapi(objhideweapon, "Obj/隐藏Obj武器"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	type(int, "类型"){	    
		semantic(popup);
		option("名剑",0);
		option("主武器",1);
		option("副武器",2);
		initval(0);
	};
	show(int, "1-隐藏0-显示"){
		initval(1);
	};
};
defapi(npcautosync, "Npc/移动/小范围平移Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(position);
		initval(vector3(100,100,100));
	};
};
defapi(objautosync, "Obj/小范围平移Obj"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	pos(vector3, "位置"){
		semantic(position);
		initval(vector3(100,100,100));
	};
};
defapi(SetPlayerId, "Npc/设置为玩家"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(RestorePlayerId, "Obj/恢复玩家");
defapi(CreateNpcNoWait,"Npc/创建/创建Npc(不等待)"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(posdir_pos);
		initval(vector3(100,100,100));
	};
	dir(float, "朝向"){
		semantic(posdir_dir);
	};
	camp(int, "阵营"){
		initval(100);
	};
	tableId(int, "Npc表ID"){
		initval(10101000);
	};
};
defapi(CreateFloatNpcNoWait,"Npc/创建/创建浮空Npc(不等待)"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(posdir3d_pos);
		initval(vector3(100,100,100));
	};
	dir(vector3, "朝向"){
		semantic(posdir3d_dir);
		initval(vector3(0,0,0));
	};
	camp(int, "阵营"){
		initval(100);
	};
	tableId(int, "Npc表ID"){
		initval(10101000);
	};
};
defapi(CreateAiNpcNoWait,"Npc/创建/创建带AI的Npc(不等待)"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(posdir_pos);
		initval(vector3(100,100,100));
	};
	dir(float, "朝向"){
		semantic(posdir_dir);
	};
	camp(int, "阵营"){
		initval(100);
	};
	tableId(int, "Npc表ID"){
		initval(10101000);
	};
	ai(string, "AI"){
		semantic(tuplepopup);
		option("普通AI", "ai_normal", "stringlist('Ai/ailogic_normal')");
		option("左侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;0;1')");
		option("右侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;1;1')");
		option("后面跟随", "ai_follow", "stringlist('Ai/ailogic_follow;2;1')");
		option("左后侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;3;1')");
		option("右后侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;4;1')");
		option("左侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;0;2')");
		option("右侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;1;2')");
		option("后面跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;2;2')");
		option("左后侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;3;2')");
		option("右后侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;4;2')");
		option("左侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;0;3')");
		option("右侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;1;3')");
		option("后面跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;2;3')");
		option("左后侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;3;3')");
		option("右后侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;4;3')");
		option("左侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;0;4')");
		option("右侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;1;4')");
		option("后面跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;2;4')");
		option("左后侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;3;4')");
		option("右后侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;4;4')");
		option("左侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;0;5')");
		option("右侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;1;5')");
		option("后面跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;2;5')");
		option("左后侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;3;5')");
		option("右后侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;4;5')");
		params(ai,aiParams);
	};
	aiParams(stringlist, "AI参数"){
		semantic(hidden);
	};
};
defapi(CreateNpc,"Npc/创建/创建Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(posdir_pos);
		initval(vector3(100,100,100));
	};
	dir(float, "朝向"){
		semantic(posdir_dir);
	};
	camp(int, "阵营"){
		initval(100);
	};
	tableId(int, "Npc表ID"){
		initval(10101000);
	};
};
defapi(CreateFloatNpc,"Npc/创建/创建浮空Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(posdir3d_pos);
		initval(vector3(100,100,100));
	};
	dir(vector3, "朝向"){
		semantic(posdir3d_dir);
		initval(vector3(0,0,0));
	};
	camp(int, "阵营"){
		initval(100);
	};
	tableId(int, "Npc表ID"){
		initval(10101000);
	};
};
defapi(CreateAiNpc,"Npc/创建/创建带AI的Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(posdir_pos);
		initval(vector3(100,100,100));
	};
	dir(float, "朝向"){
		semantic(posdir_dir);
	};
	camp(int, "阵营"){
		initval(100);
	};
	tableId(int, "Npc表ID"){
		initval(10101000);
	};
	ai(string, "AI"){
		semantic(tuplepopup);
		option("普通AI", "ai_normal", "stringlist('Ai/ailogic_normal')");
		option("左侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;0;1')");
		option("右侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;1;1')");
		option("后面跟随", "ai_follow", "stringlist('Ai/ailogic_follow;2;1')");
		option("左后侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;3;1')");
		option("右后侧跟随", "ai_follow", "stringlist('Ai/ailogic_follow;4;1')");
		option("左侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;0;2')");
		option("右侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;1;2')");
		option("后面跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;2;2')");
		option("左后侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;3;2')");
		option("右后侧跟随第二位", "ai_follow", "stringlist('Ai/ailogic_follow;4;2')");
		option("左侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;0;3')");
		option("右侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;1;3')");
		option("后面跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;2;3')");
		option("左后侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;3;3')");
		option("右后侧跟随第三位", "ai_follow", "stringlist('Ai/ailogic_follow;4;3')");
		option("左侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;0;4')");
		option("右侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;1;4')");
		option("后面跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;2;4')");
		option("左后侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;3;4')");
		option("右后侧跟随第四位", "ai_follow", "stringlist('Ai/ailogic_follow;4;4')");
		option("左侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;0;5')");
		option("右侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;1;5')");
		option("后面跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;2;5')");
		option("左后侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;3;5')");
		option("右后侧跟随第五位", "ai_follow", "stringlist('Ai/ailogic_follow;4;5')");
		params(ai,aiParams);
	};
	aiParams(stringlist, "AI参数"){
		semantic(hidden);
	};
};
defapi(NpcsPreload, "Npc/加载/批量Npc预加载"){
	tableIds(intlist, "TableIDs"){
		initval([10101001,10101002]);
	};
	asyncLoad(bool, "异步加载"){
		initval(false);
	};
};
defapi(WaitNpcLoad, "Npc/加载/等待Npc加载"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	time(int, "等待超时"){
		initval(3000);
	};
};
defapi(WaitNpcsLoad, "Npc/加载/批量等待Npc加载"){
	unitIds(intlist, "UnitIDs"){
		initval([1001,1002]);
	};
	time(int, "等待超时"){
		initval(3000);
	};
};
defapi(WaitObjLoad, "Obj/加载/等待Obj加载"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	time(int, "等待超时"){
		initval(3000);
	};
};
defapi(DestroyNpcs,"Npc/销毁/批量销毁Npc"){
	unitIds(intlist, "UnitIDs"){
		initval([1001,1002]);
	};
};
defapi(destroynpc,"Npc/销毁/销毁Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(destroynpcwithobjid,"Obj/销毁/销毁Obj"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(npcface,"Npc/移动/Npc朝向"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	dir(float, "朝向"){
		semantic(dir);
	};
	time(float, "时间"){
		initval(0.3);
	}
};
defapi(objface,"Obj/移动/Obj朝向"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	dir(float, "朝向"){
		semantic(dir);
	};
	time(float, "时间"){
		initval(0.3);
	}
};
defapi(NpcLookAt,"Npc/看向/Npc看向某点"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(position3d);
		initval(vector3(100,100,100));
	};
};
defapi(NpcLookAtNpc,"Npc/看向/Npc看向Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	targetUnitId(int, "UnitID"){
		initval(1002);
	};
	bone(string, "骨骼名"){
		initval("TX_Tou");
	};
};
defapi(NpcLookAtObj,"Npc/看向/Npc看向Obj"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	bone(string, "骨骼名"){
		initval("TX_Tou");
	};
};
defapi(NpcLookAtCamera,"Npc/看向/Npc看向相机"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(NpcStopLookAt,"Npc/看向/Npc停止看向"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(ObjLookAt,"Obj/看向/Obj看向某点"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	pos(vector3, "位置"){
		semantic(position3d);
		initval(vector3(100,100,100));
	};
};
defapi(ObjLookAtNpc,"Obj/看向/Obj看向Npc"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	targetUnitId(int, "UnitID"){
		initval(1001);
	};
	bone(string, "骨骼名"){
		initval("TX_Tou");
	};
};
defapi(NpcLookAtCamera,"Obj/看向/Obj看向相机"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(ObjStopLookAt,"Obj/看向/Obj停止看向"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(npcreplaceanimation,"Npc/动作/Npc替换动作状态"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	anim(string, "动作状态名");
	repanim(string, "替换为动作状态名（空表示恢复原动作）");
};
defapi(objreplaceanimation,"Obj/动作/Obj替换动作状态"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	anim(string, "动作状态名");
	repanim(string, "替换为动作状态名（空表示恢复原动作）");
};
defapi(npcfacialanimation,"Npc/动作/Npc做面部动作"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	anim(string, "动作状态名");
};
defapi(objfacialanimation,"Obj/动作/Obj做面部动作"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	anim(string, "动作状态名");
};
defapi(npcanimation,"Npc/动作/Npc做动作"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	anim(string, "动作状态名");
};
defapi(objanimation,"Obj/动作/Obj做动作"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	anim(string, "动作状态名");
};
defapi(npcstopfacialanimation,"Npc/动作/Npc停止面部动作"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(objstopfacialanimation,"Obj/动作/Obj停止面部动作"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(npcstopanimation,"Npc/动作/Npc停止动作"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(objstopanimation,"Obj/动作/Obj停止动作"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(npcenableai,"Npc/AI/Npc允许|禁用AI"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	enable(int, "是否允许"){
		initval(0);
	};
};
defapi(objenableai,"Obj/AI/Obj允许|禁用AI"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	enable(int, "是否允许"){
		initval(0);
	};
};
defapi(npcstop,"Npc/AI/Npc停止当前AI行为"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(objstop,"Obj/AI/Obj停止当前AI行为"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(NpcSetAiLeaderNpc,"Npc/AI/Npc设置AI Leader Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	unitId2(int, "UnitID"){
		initval(1002);
	};
};
defapi(NpcSetAiLeaderObj,"Npc/AI/Npc设置AI Leader Obj"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(ObjSetAiLeaderNpc,"Obj/AI/Obj设置AI Leader Npc"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(NpcSetAiTargetNpc,"Npc/AI/Npc设置AI目标Npc"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	unitId2(int, "UnitID"){
		initval(1002);
	};
};
defapi(NpcSetAiTargetObj,"Npc/AI/Npc设置AI目标Obj"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(ObjSetAiTargetNpc,"Obj/AI/Obj设置AI目标Npc"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(npcdisplay,"Npc/表现组/Npc播放表现组"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	display(string, "表现组名");
};
defapi(npcstopdisplay,"Npc/表现组/Npc停止表现组"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	display(string, "表现组名");
};
defapi(npccaststoryskill,"Npc/Npc释放剧情技能"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	skillId(int, "SkillID"){
		initval(1001);
	};
};
defapi(npccastplayerskill,"Npc/Npc释放玩家技能"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	skillId(int, "SkillID"){
		initval(1001);
	};
};
defapi(npcstopskill,"Npc/Npc停止技能释放"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(objdisplay,"Obj/表现组/Obj播放表现组"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	display(string, "表现组名");
};
defapi(objstopdisplay,"Obj/表现组/Obj停止表现组"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	display(string, "表现组名");
};
defapi(PlayerDisplay,"Obj/表现组/玩家播放表现组"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	male(string, "男表现组名");
	female(string, "女表现组名");
};
defapi(PlayerStopDisplay,"Obj/表现组/玩家停止表现组"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	male(string, "男表现组名");
	female(string, "女表现组名");
};
defapi(objcaststoryskill,"Obj/Obj释放剧情技能"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	skillId(int, "SkillID"){
		initval(1001);
	};
};
defapi(objcastplayerskill,"Obj/Obj释放玩家技能"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	skillId(int, "SkillID"){
		initval(1001);
	};
};
defapi(objstopskill,"Obj/Obj停止技能释放"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(setmovetimeout,"Common/移动超时"){
	timeout(int, "超时时间"){
		initval(10000);
	};
};
defapi(npcsetaireachscope,"Npc/移动/Npc移动停止范围"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	scope(float, "停止范围"){
		initval(1.0);
	};
};
defapi(npcspeedmode,"Npc/移动/Npc速度模式"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	speedMode(int, "速度模式"){
		semantic(popup);
		option("慢走",0);
		option("正常走",1);
		option("快走",2);
		initval(0);
	};
};
defapi(objspeedmode,"Obj/移动/Obj速度模式"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	speedMode(int, "速度模式"){
		initval(0);
	};
};
defapi(SetNpcSpeed,"Npc/移动/Npc移动速度"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	speed(float, "速度"){
		initval(8);
	};
};
defapi(setspeed,"Obj/移动/Obj移动速度"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	speed(float, "速度"){
		initval(8);
	};
};
defapi(SetNpcTurnSpeed,"Npc/移动/Npc转身速度"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	speed(float, "速度"){
		initval(720.0);
	};
};
defapi(SetObjTurnSpeed,"Obj/移动/Obj转身速度"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	speed(float, "速度"){
		initval(720.0);
	};
};
defapi(npcmove,"Npc/移动/Npc移动"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(position);
		initval(vector3(100,100,100));
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(objmove,"Obj/Obj移动"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	pos(vector3, "位置"){
		semantic(position);
		initval(vector3(100,100,100));
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(npcmovetoward,"Npc/移动/Npc移动到目标前"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	targetUnitId(int, "UnitID"){
		initval(1002);
	};
	dist(float, "停止时离目标距离"){
		initval(1.0);
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(objmovetoward,"Obj/移动/Obj移动目标前"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	targetObjId(int, "ObjID"){
		semantic(objid);
		initval(unitid2objid(1002));
	};
	dist(float, "停止时离目标距离"){
		initval(1.0);
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(npcmovewithwaypoints,"Npc/移动/Npc沿路径移动"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3list, "路径"){
		semantic(spline);
		initval(getsplinepoints("path"));
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(objmovewithwaypoints,"Obj/移动/Obj沿路径移动"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	pos(vector3list, "路径"){
		semantic(spline);
		initval(getsplinepoints("path"));
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(storywait, "Common/等待"){
	time(int, "时间（毫秒）"){
		initval(1000);
	};
};
defapi(storylocalmessage, "Common/触发本地消息"){
	msgId(string, "消息"){
		semantic(message);
	};
};
defapi(firemessage, "Common/触发广播消息"){
	msgId(string, "消息");
};
defapi(ShowBrief, "Text/章节信息/显示章节信息"){
	info1(string, "信息1");
	info2(string, "信息2");
	info3(string, "信息3");
	info4(string, "信息4");
};
defapi(HideBrief, "Text/章节信息/关闭章节信息");
defapi(ShowChapterTitle, "Text/章节标题/显示章节标题"){
	info1(string, "参数1");
	info2(string, "参数2");
	info3(string, "参数3");
	info4(string, "参数4");
	info4(string, "参数5");
};
defapi(HideChapterTitle, "Text/章节标题/隐藏章节标题");
defapi(ShowText, "Text/对话/显示对话内容"){
	info(string, "信息");
	delay(float, "延迟显示时间");
};
defapi(HideText, "Text/对话/隐藏对话内容");
defapi(ShowOption, "Text/对话/显示对话选项"){
	dlgId(int, "对话组ID"){
		initval(100);
	};
};
defapi(ShowLocalOption, "Text/对话/显示本地对话选项"){
	count(int, "选项总数"){
		initval(2);
	};
	index(int, "选项序号"){
		initval(0);
	};
	caption(string, "选项标题"){
		initval("选项标题");
	};
	msg(string, "点击触发消息"){
		semantic(message);
	};
};
defapi(ShowSkip, "Text/对话/显示按钮"){
	msg(string, "点击触发消息"){
		semantic(message);
	};
};
defapi(ShowTextAndSkip, "Text/对话/显示对话内容及加速按钮"){
	info(string, "信息");
	delay(float, "延迟显示时间");
};
defapi(ShowNpcPaoPao, "Text/显示Npc头顶泡泡"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	txt(string, "信息");
	duration(float, "显示时间");
};
defapi(ShowObjPaoPao, "Text/显示Obj头顶泡泡"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	txt(string, "信息");
	duration(float, "显示时间");
};
defapi(HideNpcPaoPao, "Text/隐藏Npc头顶泡泡"){
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(HideObjPaoPao, "Text/隐藏Obj头顶泡泡"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(ShowNpcCard, "Text/名字卡/显示NPC名字卡"){
	tip(string, "信息");
	posX(int, "X坐标"){
		initval(0);
	};
	posY(int, "Y坐标"){
		initval(0);
	};
};
defapi(HideNpcCard, "Text/名字卡/隐藏NPC名字卡");
defapi(ShowNpcMingPian, "Text/名片/显示NPC新名片"){
	tip(int, "名片id信息");
	posX(int, "X坐标"){
		initval(0);
	};
	posY(int, "Y坐标"){
		initval(0);
	};
};
defapi(HideNpcMingPian, "Text/名片/隐藏NPC新名片(5s自动隐藏)");
defapi(EnterNpcStareMode, "Npc/凝视/进入Npc凝视模式"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	height(float, "高度"){
		initval(1.1);
	};
	msg(string, "结束触发消息"){
		semantic(message);
	};
	posX(int, "X坐标"){
		initval(0);
	};
	posY(int, "Y坐标"){
		initval(0);
	};
	id(string, "隐藏npc");
};
defapi(CloseNpcStareMode, "Npc/凝视/关闭Npc凝视模式");
defapi(EnterObjStareMode, "Obj/进入Obj凝视模式"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	height(float, "高度"){
		initval(1.1);
	};
	msg(string, "结束触发消息"){
		semantic(message);
	};
	posX(int, "X坐标"){
		initval(0);
	};
	posY(int, "Y坐标"){
		initval(0);
	};
	id(string, "隐藏npc");
};
defapi(CloseObjStareMode, "Obj/关闭Obj凝视模式");
defapi(PlayTimeLine, "Timeline/普通Timeline/播放Timeline"){
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(PlayTimeLineNoFade, "Timeline/普通Timeline/播放Timeline(无淡入淡出)"){
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(StopTimeLine, "Timeline/普通Timeline/停止Timeline"){
    forceMainCamera(int, "切换回主相机"){
        initval(0);  
    };
};
defapi(PlayTimeLineWithPlayer, "Timeline/带玩家Timeline/播放带玩家Timeline"){
	male(string, "男挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	female(string, "女挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(PlayTimeLineWithPlayerNoFade, "Timeline/带玩家Timeline/播放带玩家Timeline(无淡入淡出)"){
	male(string, "男挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	female(string, "女挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(StopTimeLineWithPlayer, "Timeline/带玩家Timeline/停止带玩家Timeline"){
    forceMainCamera(int, "切换回主相机"){
        initval(0);  
    };
		pos(vector3, "位置"){
			semantic(posdir3d_pos);
			initval(vector3(0,0,0));
		};
		dir(vector3, "朝向"){
			semantic(posdir3d_dir);
			initval(vector3(0,0,0));
		};
};
defapi(PlayTimeLineWithPlayerWithoutCam, "Timeline/带玩家Timeline/播放带玩家Timeline不带镜头"){
	male(string, "男挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	female(string, "女挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(PlayTimeLineWithPlayerNoFadeWithoutCam, "Timeline/带玩家Timeline/播放带玩家Timeline不带镜头(无淡入淡出)"){
	male(string, "男挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	female(string, "女挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(StopTimeLineWithPlayerWithoutCam, "Timeline/带玩家Timeline/停止带玩家Timeline不带镜头"){
		pos(vector3, "位置"){
			semantic(posdir3d_pos);
			initval(vector3(0,0,0));
		};
		dir(vector3, "朝向"){
			semantic(posdir3d_dir);
			initval(vector3(0,0,0));
		};
};
defapi(PlayComplexTimeLine, "Timeline/带玩家Timeline/播放Timeline（指定玩家结点）"){
	male(string, "男挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	female(string, "女挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	playerNode(string, "玩家结点"){
	    initval("/JS_Root/Player");
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(PlayComplexTimeLineNoFade, "Timeline/带玩家Timeline/播放Timeline(无淡入淡出,指定玩家结点)"){
	male(string, "男挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	female(string, "女挂点Key"){
		semantic(timeline_track_binding);
		prefab(timelinePrefab);
	};
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(timeline_prefab);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称"){
		semantic(timeline_node);
		prefab(timelinePrefab);
	};
	playerNode(string, "玩家结点"){
	    initval("/JS_Root/Player");
	};
	atPlayerPos(int, "在玩家位置播放"){
		initval(0);
	};
	timelineEndMsg(string, "TimeLine结束事件"){
		semantic(timeline_end_msg);
	};
};
defapi(StopComplexTimeLine, "Timeline/带玩家Timeline/停止带玩家Timeline（指定玩家结点）"){
    forceMainCamera(int, "切换回主相机"){
        initval(0);  
    };
		pos(vector3, "位置"){
			semantic(posdir3d_pos);
			initval(vector3(0,0,0));
		};
		dir(vector3, "朝向"){
			semantic(posdir3d_dir);
			initval(vector3(0,0,0));
		};
};
defapi(SwitchToStoryCameraVC, "Camera/VC相机/切换到剧情VC相机");
defapi(SwitchToStoryCameraVCA, "Camera/VC相机阵列/切换到剧情VC相机阵列"){
	index(int, "索引（0~7）"){
		initval(0);
	};
};
defapi(SwitchToMainCameraAndDelayReset, "Camera/主相机/切换到主相机"){
	time(float, "时间"){
		initval(1.0);
	};
};
defapi(SwitchToMainCamera, "Camera/主相机/切换到主相机（使用EaseInOut混合模式）");
defapi(VC_Damping, "Camera/VC相机/VC相机缓动"){
	aim(float, "aim幅度"){
		initval(0.5);
		camera(true);
	};
	body(float, "body幅度"){
		initval(1.0);
		camera(true);
	};
	yaw(float, "yaw幅度"){
		initval(20.0);
		camera(true);
	};
};
defapi(VC_SetPosAndDir,"Camera/VC相机/VC相机放置"){
	pos(vector3, "位置"){
		semantic(posdir3d_pos);
		initval(vector3(0,0,0));
		camera(true);
	};
	dir(vector3, "朝向"){
		semantic(posdir3d_dir);
		initval(vector3(0,0,0));
		camera(true);
	};
};
defapi(VC_SetInheritPosition,"Camera/VC相机/VC相机初始使用当前相机位置"){
	val(int, "是否使用当前相机位置"){
	    initval(1);  
	};
};
defapi(VC_SetBlendHint,"Camera/VC相机/VC相机BlendHint"){
	blend(int, "BlendHint"){
		semantic(popup);
		option("线性",0);
		option("球面",1);
		option("柱面",2);
		option("屏幕空间线性",3);
		initval(0);
	};
};
defapi(VC_LookAt, "Camera/VC相机/VC相机LookAt Npc"){
	unitId(int, "UnitID"){
		initval(1001);
		camera(true);
	};
	offset(vector3, "偏移"){
		initval(vector3(0,1.5,0));
		camera(true);
	};
};
defapi(VC_Follow, "Camera/VC相机/VC相机Follow Npc"){
	unitId(int, "UnitID"){
		initval(1001);
		camera(true);
	};
	offset(vector3, "偏移"){
		initval(vector3(0,2,3));
		camera(true);
	};
};
defapi(VC_LookAtObj, "Camera/VC相机/VC相机LookAt Obj"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	offset(vector3, "偏移"){
		initval(vector3(0,1.5,0));
		camera(true);
	};
};
defapi(VC_FollowObj, "Camera/VC相机/VC相机Follow Obj"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	offset(vector3, "偏移"){
		initval(vector3(0,2,3));
		camera(true);
	};
};
defapi(VC_ScreenXY, "Camera/VC相机/VC相机LookAt屏幕偏移"){
	screenx(float, "屏幕X（0~1）"){
		initval(0.0);
		camera(true);
	};
	screeny(float, "屏幕Y（0~1）"){
		initval(0.0);
		camera(true);
	};
};
defapi(VC_Lens, "Camera/VC相机/VC相机镜头设置"){
	fov(float, "FOV"){
		initval(38);
		camera(true);
	};
	nearClip(float, "近裁剪面"){
		initval(0.1);
		camera(true);
	};
	farClip(float, "远裁剪面"){
		initval(500.0);
		camera(true);
	};
	dutch(float, "镜头侧翻角度"){
		initval(0.0);
		camera(true);
	};
};
defapi(VCA_Damping, "Camera/VC相机/VC相机缓动"){
	aim(float, "aim幅度"){
		initval(0.5);
		camera(true);
	};
	body(float, "body幅度"){
		initval(1.0);
		camera(true);
	};
	yaw(float, "yaw幅度"){
		initval(20.0);
		camera(true);
	};
};
defapi(VCA_SetPosAndDir,"Camera/VC相机阵列/VC相机阵列放置"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	pos(vector3, "位置"){
		semantic(posdir3d_pos);
		initval(vector3(0,0,0));
		camera(true);
	};
	dir(vector3, "朝向"){
		semantic(posdir3d_dir);
		initval(vector3(0,0,0));
		camera(true);
	};
};
defapi(VCA_SetInheritPosition,"Camera/VC相机阵列/VC相机阵列初始使用当前相机位置"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	val(int, "是否使用当前相机位置"){
	    initval(1);  
	};
};
defapi(VCA_SetBlendHint,"Camera/VC相机阵列/VC相机阵列BlendHint"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	blend(int, "BlendHint"){
		semantic(popup);
		option("线性",0);
		option("球面",1);
		option("柱面",2);
		option("屏幕空间线性",3);
		initval(0);
	};
};
defapi(VCA_LookAt, "Camera/VC相机阵列/VC相机阵列LookAt Npc"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	unitId(int, "UnitID"){
		initval(1001);
		camera(true);
	};
	offset(vector3, "偏移"){
		initval(vector3(0,1.5,0));
		camera(true);
	};
};
defapi(VCA_Follow, "Camera/VC相机阵列/VC相机阵列Follow Npc"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	unitId(int, "UnitID"){
		initval(1001);
		camera(true);
	};
	offset(vector3, "偏移"){
		initval(vector3(0,2,3));
		camera(true);
	};
};
defapi(VCA_LookAtObj, "Camera/VC相机阵列/VC相机阵列LookAt Obj"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	offset(vector3, "偏移"){
		initval(vector3(0,1.5,0));
		camera(true);
	};
};
defapi(VCA_FollowObj, "Camera/VC相机阵列/VC相机阵列Follow Obj"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	offset(vector3, "偏移"){
		initval(vector3(0,2,3));
		camera(true);
	};
};
defapi(VCA_ScreenXY, "Camera/VC相机阵列/VC相机阵列LookAt屏幕偏移"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	screenx(float, "屏幕X（0~1）"){
		initval(0.0);
		camera(true);
	};
	screeny(float, "屏幕Y（0~1）"){
		initval(0.0);
		camera(true);
	};
};
defapi(VCA_Lens, "Camera/VC相机阵列/VC相机阵列镜头设置"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	fov(float, "FOV"){
		initval(38);
		camera(true);
	};
	nearClip(float, "近裁剪面"){
		initval(0.1);
		camera(true);
	};
	farClip(float, "远裁剪面"){
		initval(500.0);
		camera(true);
	};
	dutch(float, "镜头侧翻角度"){
		initval(0.0);
		camera(true);
	};
};
defapi(MC_SyncPosAndDir,"Camera/主相机/主相机同步跟随位置与朝向");
defapi(MC_SetPosAndDir,"Camera//主相机主相机放置"){
	pos(vector3, "位置"){
		semantic(posdir3d_pos);
		initval(vector3(0,0,0));
		camera(true);
	};
	dir(vector3, "朝向"){
		semantic(posdir3d_dir);
		initval(vector3(0,0,0));
		camera(true);
	};
};
defapi(MC_SetInheritPosition,"Camera/主相机/主相机初始使用当前相机位置"){
	val(int, "是否使用当前相机位置"){
	    initval(1);  
	};
};
defapi(MC_SetBlendHint,"Camera/主相机/主相机BlendHint"){
	blend(int, "BlendHint"){
		semantic(popup);
		option("线性",0);
		option("球面",1);
		option("柱面",2);
		option("屏幕空间线性",3);
		initval(0);
	};
};
defapi(MC_Lens, "Camera/主相机/主相机镜头设置"){
	fov(float, "FOV"){
		initval(38);
		camera(true);
	};
	nearClip(float, "近裁剪面"){
		initval(0.1);
		camera(true);
	};
	farClip(float, "远裁剪面"){
		initval(500.0);
		camera(true);
	};
	dutch(float, "镜头侧翻角度"){
		initval(0.0);
		camera(true);
	};
};
defapi(SetBrainBlend, "Camera/相机混合模式"){
	blend(int, "混合模式"){
		semantic(popup);
		option("Cut",0);
		option("EaseInOut",1);
		option("EaseIn",2);
		option("EaseOut",3);
		option("HardIn",4);
		option("HardOut",5);
		option("Linear",6);
		initval(0);
		camera(true);
	};
	time(float, "过渡时间"){
		initval(2.0);
		camera(true);
	};
};
defapi(setocclusionculling, "Camera/开关Occusion Culling"){
	enable(int, "开--1 关--0"){
		initval(0);
	};
};
defapi(VcamPrefabSetLookat, "Camera/VcamPrefab相机/VcamPrefab里相机lookat Npc"){	
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(VcamPrefabSetFollow, "Camera/VcamPrefab相机/VcamPrefab里相机follow Npc"){	
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	unitId(int, "UnitID"){
		initval(1001);
	};
};
defapi(VcamPrefabSetLookatObj, "Camera/VcamPrefab相机/VcamPrefab里相机lookat Obj"){	
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(VcamPrefabSetFollowObj, "Camera/VcamPrefab相机/VcamPrefab里相机follow Obj"){	
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(camerafollow, "Camera/主相机/主相机跟随Obj"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
};
defapi(camerayaw, "Camera/主相机/主相机yaw"){
	angle(float, "Yaw"){
		initval(0);
	};
};
defapi(camerapitch, "Camera/主相机/主相机pitch"){
	angle(float, "Pitch"){
		initval(0);
	};
};
defapi(cameradistance, "Camera/主相机/主相机距离"){
	dist(float, "距离"){
		initval(0);
	};
};
defapi(cameraheight, "Camera/主相机/主相机高度"){
	height(float, "高度"){
		initval(0);
	};
};
defapi(camerasetdistanceheight, "Camera/主相机/主相机距离与高度"){
	dist(float, "距离"){
		initval(0);
	};
	height(float, "高度"){
		initval(0);
	};
};
defapi(camerastoredistanceheight, "Camera/主相机/主相机保存距离与高度");
defapi(camerarestoredistanceheight, "Camera/主相机/主相机恢复距离与高度");
defapi(PlayVideo, "Video/播放视频"){
	path(string, "视频路径");
	canSkip(int, "是否可跳过"){
	    initval(0);  
	};
	msg(string, "结束触发消息"){
		semantic(message);
	};
	ratio(float, "画面比例"){
		initval(2.167);
	};
	timeSkip(float, "跳过出现时间"){
		initval(0);
	};
};
defapi(SetTodOrWeather, "Effect/天气与昼夜"){
	key(string, "天气或昼夜");
};
defapi(SetMotionBlur, "Effect/运动模糊"){
	open(int, "开1关0"){
		initval(1);
	};
};
defapi(VC_Shake, "Effect/VC相机震动"){
	time(float, "时间"){
		initval(1.0);
	};
	amp(float, "幅度"){
		initval(2.0);
	};
};
defapi(VCA_Shake, "Effect/VC相机阵列震动"){
	index(int, "索引（0~7）"){
		initval(0);
	};
	time(float, "时间"){
		initval(1.0);
	};
	amp(float, "幅度"){
		initval(2.0);
	};
};
defapi(MC_Shake, "Effect/主相机震动"){
	time(float, "时间"){
		initval(1.0);
	};
	amp(float, "幅度"){
		initval(2.0);
	};
};
defapi(CameraDrunkEffect, "Effect/酒醉"){
	val(int, "开关"){
		initval(1);
	};
};
defapi(CameraBlackWhiteEffect, "Effect/黑白"){
	val(int, "开关"){
		initval(1);
	};
};
defapi(CameraLightYellowEffect, "Effect/淡黄色"){
	val(int, "开关"){
		initval(1);
	};
};
defapi(CameraLightGreenEffect, "Effect/淡青色"){
	val(int, "开关"){
		initval(1);
	};
};
defapi(CameraDOF, "Effect/景深"){
	val(int, "开关"){
		initval(1);
	};
};
defapi(enablelight, "Effect/开关灯"){
	path(string, "灯所在场景结点路径");
	val(int, "开1或关0"){
		initval(1);
	};
};
defapi(playparticle, "Effect/播放或停止特效"){
	path(string, "特效所在场景结点路径");
	val(int, "开1或关0"){
		initval(1);
	};
};
defapi(PlaySound, "Sound/播放声音"){
	event(string, "声音event"){
		semantic(fmod_event);
	};
};
defapi(StopSound, "Sound/停止声音"){
	event(string, "声音event"){
		semantic(fmod_event);
	};
};
defapi(PlayPlayerSound, "Sound/播放玩家声音"){
	event1(string, "男声音event"){
		semantic(fmod_event);
	};
	event2(string, "女声音event"){
		semantic(fmod_event);
	};
};
defapi(StopPlayerSound, "Sound/停止玩家声音"){
	event1(string, "男声音event"){
		semantic(fmod_event);
	};
	event2(string, "女声音event"){
		semantic(fmod_event);
	};
};
defapi(PlayMusic, "Sound/播放音乐"){
	event(string, "音乐event"){
		semantic(fmod_event);
	};
};
defapi(StopMusic, "Sound/停止音乐"){
	fade(int, "是否fade");
};
defapi(SetSceneMusicParam, "Sound/设置场景音乐参数"){
	paramName(string, "参数名");
	paramValue(float, "参数值");
};
defapi(SetMusicParam, "Sound/设置背景音乐参数"){
	paramName(string, "参数名");
	paramValue(float, "参数值");
};
defapi(PlayCurvy, "Curvy/播放曲线"){
	path(string, "曲线目标路径");
	msg(string, "结束触发消息"){
		semantic(message);
	};
};
defapi(PauseCurvy, "Curvy/暂停曲线"){
	path(string, "曲线目标路径");
};
defapi(ResumeCurvy, "Curvy/恢复曲线"){
	path(string, "曲线目标路径");
};
defapi(StopCurvy, "Curvy/停止曲线"){
	path(string, "曲线目标路径");
};
defapi(LoadPrefab, "Load/加载prefab"){
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
};
defapi(DestroyPrefab, "Load/销毁prefab"){
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
};
defapi(LoadVcamPrefab, "Load/加载prefab（带相机）"){
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
};
defapi(DestroyVcamPrefab, "Load/销毁prefab（带相机）"){
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("LogicScenes/");
	};
};
defapi(creategameobject, "Load/创建GameObject"){
	name(string, "名称");
	res(string, "prefab资源名"){
		semantic(unityobject);
		unitytype("GameObject");
		path("");
	};
	path(string, "父对象路径"){
		initval("");
	};
	position(vector3, "位置"){
		semantic(posdir3d_pos);
		initval(vector3(0,0,0));
		optional(true);
	};
	rotation(vector3, "朝向"){
		semantic(posdir3d_dir);
		initval(vector3(0,0,0));
		optional(true);
	};
};
defapi(destroygameobject, "Load/销毁GameObject"){
	name(string, "名称");
};
defapi(setparent, "Load/挂接对象"){	
	cpath(string, "子对象路径");
	ppath(string, "父对象路径");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
	dontStopAnimator(int, "不停动画"){
		semantic(hidden);
		initval(1);
	};
};
defapi(NpcSetParent, "Load/Npc挂接对象"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	ppath(string, "父对象路径");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
};
defapi(ObjSetParent, "Load/Obj挂接对象"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	ppath(string, "父对象路径");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
};
defapi(SetParentNpc, "Load/挂接到NPC"){
	cpath(string, "对象路径");
	unitId(int, "UnitID"){
		initval(1001);
	};
	bone(string, "挂点名");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
};
defapi(SetParentObj, "Load/挂接到Obj"){
	cpath(string, "对象路径");
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	bone(string, "挂点名");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
};
defapi(NpcSetParentNpc, "Load/Npc挂接到NPC"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	unitId2(int, "UnitID"){
		initval(1001);
	};
	bone(string, "挂点名");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
};
defapi(NpcSetParentObj, "Load/Npc挂接到Obj"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	bone(string, "挂点名");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
};
defapi(ObjSetParentNpc, "Load/Obj挂接到NPC"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	unitId(int, "UnitID"){
		initval(1001);
	};
	bone(string, "挂点名");
	stayWorld(int, "保持世界坐标"){
		initval(0);
	};
};
defapi(settransform, "Load/调整GameObject位置与旋转"){
	name(string, "名称");
	worldOrLocal(int, "1-世界坐标 0-本地坐标"){
		initval(1);
	};
	position(vector3, "位置"){
		initval(vector3(0,0,0));
		optional(true);
	};
	rotation(vector3, "朝向"){
		initval(vector3(0,0,0));
		optional(true);
	};
	scale(vector3, "缩放"){
		initval(vector3(1,1,1));
		optional(true);
	};
};
defapi(NpcSetTransform, "Load/调整挂接Npc位置与旋转"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	worldOrLocal(int, "1-世界坐标 0-本地坐标"){
		initval(1);
	};
	$position(vector3, "位置"){
		initval(vector3(0,0,0));
		optional(true);
	};
	$rotation(vector3, "朝向"){
		initval(vector3(0,0,0));
		optional(true);
	};
	$scale(vector3, "缩放"){
		initval(vector3(1,1,1));
		optional(true);
	};
};
defapi(ObjSetTransform, "Load/调整挂接Obj位置与旋转"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	worldOrLocal(int, "1-世界坐标 0-本地坐标"){
		initval(1);
	};
	$position(vector3, "位置"){
		initval(vector3(0,0,0));
		optional(true);
	};
	$rotation(vector3, "朝向"){
		initval(vector3(0,0,0));
		optional(true);
	};
	$scale(vector3, "缩放"){
		initval(vector3(1,1,1));
		optional(true);
	};
};
defapi(setvisible, "Load/显示隐藏"){
	path(string, "对象路径");
	show(int, "显示隐藏"){
		initval(0);
	};
};
defapi(ShowHideChild, "Load/显示隐藏子结点"){
	objpath(string, "对象路径");
	path(string, "子结点");
	show(int, "显示隐藏"){
		initval(0);
	};
};
defapi(ScreenFadeOut, "Effect/屏幕渐黑"){
	time(float, "时间"){
		initval(1.0);
	};
};
defapi(ScreenFadeIn, "Effect/屏幕渐白"){
	time(float, "时间"){
		initval(1.0);
	};
};
defapi(setvehiclepathpointnum,"载具/载具路线插值点数"){
	num(int, "最大点数"){
		initval(100);
	};
};
defapi(createvehicle,"载具/创建载具"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	vehicleTableId(int, "载具表ID"){
		initval(1001);
	};
	camp(int, "阵营"){
		initval(100);
	};
};
defapi(NpcUseVehicle,"载具/Npc使用载具"){
	unitId(int, "UnitID"){
		initval(1001);
	};
	vehicleUnitId(int, "载具UnitID"){
		initval(1001);
	};
	vehicleTableId(int, "载具表ID"){
		initval(1001);
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(ObjUseVehicle,"载具/Obj使用载具"){
	objId(int, "ObjID"){
		semantic(objid);
		initval(getplayerid());
	};
	vehicleUnitId(int, "载具UnitID"){
		initval(1001);
	};
	vehicleTableId(int, "载具表ID"){
		initval(1001);
	};
	msg(string, "结束消息"){
		semantic(message);
	};
};
defapi(PublishEvent, "游戏事件/无参数事件"){
	evt(string, "事件名");
};
defapi(PublishEventInt, "游戏事件/整数参数事件"){
	evt(string, "事件名");
	arg(int, "参数");
};
defapi(PublishEventFloat, "游戏事件/浮点数参数事件"){
	evt(string, "事件名");
	arg(float, "参数");
};
defapi(PublishEventStr, "游戏事件/字符串参数事件"){
	evt(string, "事件名");
	arg(string, "参数");
};
defapi(PublishEventObj, "游戏事件/GameObject参数事件"){
	evt(string, "事件名");
	arg(object, "参数");
};
defapi(SendScriptMessage, "启动脚本消息/无参数消息"){
	evt(string, "事件名");
};
defapi(SendScriptMessageInt, "启动脚本消息/整数参数消息"){
	evt(string, "事件名");
	arg(int, "参数");
};
defapi(SendScriptMessageFloat, "启动脚本消息/浮点数参数消息"){
	evt(string, "事件名");
	arg(float, "参数");
};
defapi(SendScriptMessageStr, "启动脚本消息/字符串参数消息"){
	evt(string, "事件名");
	arg(string, "参数");
};
defapi(SendScriptMessageObj, "启动脚本消息/GameObject参数消息"){
	evt(string, "事件名");
	arg(object, "参数");
};
defapi(SendMessage, "U3D脚本消息/无参数消息"){
	path(string, "脚本对象路径");
	evt(string, "事件名");
};
defapi(SendMessageInt, "U3D脚本消息/整数参数消息"){
	path(string, "脚本对象路径");
	evt(string, "事件名");
	arg(int, "参数");
};
defapi(SendMessageFloat, "U3D脚本消息/浮点数参数消息"){
	path(string, "脚本对象路径");
	evt(string, "事件名");
	arg(float, "参数");
};
defapi(SendMessageStr, "U3D脚本消息/字符串参数消息"){
	path(string, "脚本对象路径");
	evt(string, "事件名");
	arg(string, "参数");
};
defapi(SendMessageObj, "U3D脚本消息/GameObject参数消息"){
	path(string, "脚本对象路径");
	evt(string, "事件名");
	arg(object, "参数");
};
defapi(ShowMiddleSkip, "跳过/中跳"){
	delayTime(int, "延迟时间");
	endMsg(string, "结束消息");
};
defapi(ClearMiddleSkipState, "跳过/中跳结束"){
};