defapi(createnpc,"创建NPC"){
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
	tableId(int, "NPC表ID"){
		initval(10101000);
	};
};
defapi(createainpc,"创建带AI的NPC"){
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
	tableId(int, "NPC表ID"){
		initval(10101000);
	};
	ai(string, "AI"){
		semantic(tuplepopup);
		option("普通AI", "ai_normal", "stringlist('Ai/ailogic_normal')");
		params(ai,aiParams);
	};
	aiParams(list, "AI参数"){
		semantic(hidden);
	};
};
defapi(npcmove,"NPC移动"){
	objId(int, "UnitID"){
		initval(1001);
	};
	pos(vector3, "位置"){
		semantic(position);
		initval(vector3(100,100,100));
	};
	msg(string, "结束消息"){
		semantic(message);
		optional(true);
	};
};
defapi(objmove,"OBJ移动"){
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
defapi(storywait, "等待"){
	time(int, "时间（毫秒）"){
		initval(1000);
	};
};
defapi(localmessage, "触发本地消息"){
	msgId(string, "消息"){
		semantic(message);
	};
};
defapi(ShowBrief, "显示章节信息"){
	info1(string, "信息1");
	info2(string, "信息2");
	info3(string, "信息3");
	info4(string, "信息4");
};
defapi(UpdateTip, "显示提示信息"){
	info(string, "信息");
	interval(int, "间隔");
	start(int, "开始字符索引");
	end(int, "结束字符索引");
};
defapi(CloseTip, "关闭提示信息");
defapi(PlayTimeLine, "播放Timeline"){
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(file);
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称");
};
defapi(StopTimeLine, "停止Timeline"){
	isSmoothCamera(bool, "使用镜头融合");
	timelinePrefab(string, "Timeline Prefab"){
		semantic(file);
		path("LogicScenes/");
	};
	timelineNode(string, "TimeLine结点名称");
};