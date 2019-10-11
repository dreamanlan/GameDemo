script(ai_npc)
{
  local
  {
  };
  onmessage(start)
  {
    $objid = @objid;
    while(gethp($objid)>0){
      $tid = unitid2objid(1001);
      objsetaitarget($objid,$tid);
      if(ai_need_chase($objid,3)){
        ai_chase($objid,3);
      }else{
        ai_rand_move($objid, 3000, 12);
      };
      wait(300);
      if(rndint(0,100)<10){
        $hp=gethp($objid);
        $maxhp=getmaxhp($objid);
        if($hp<$maxhp/5){
          sendmessagewithgameobject(getgameobject($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector3(0,2,0),"","啊，我要死了！",2);
        }elseif($hp<$maxhp/4){
          sendmessagewithgameobject(getgameobject($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector3(0,2,0),"","啊，我要回家！",2);
        }elseif($hp<$maxhp/3){
          sendmessagewithgameobject(getgameobject($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector3(0,2,0),"","啊，我伤的很重！",2);
        }elseif($hp<$maxhp/2){
          sendmessagewithgameobject(getgameobject($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector3(0,2,0),"","啊，我受伤了！",2);
        }elseif($hp<$maxhp){
          sendmessagewithgameobject(getgameobject($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector3(0,2,0),"","啊，好恐怖！",2);
        }else{
          sendmessagewithgameobject(getgameobject($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector3(0,2,0),"","这是哪里？？？",2);
        };
      };
    };
  };
  onnamespacedmessage("on_born")
  {
    bornfinish(@objid);
  };
  onnamespacedmessage("on_dead")
  {
    wait(500);
    objanimation(@objid, "dyingbackward", 0);
    wait(5000);
    deadfinish(@objid);
  };
};