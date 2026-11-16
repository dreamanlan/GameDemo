script(ai_npc)
{
  local
  {
  };
  onmessage(start)
  {
    $objid = @objid;
    while(get_hp($objid)>0){
      $tid = unit_id_2_obj_id(1001);
      obj_set_ai_target($objid,$tid);
      if(ai_need_chase($objid,3)){
        ai_chase($objid,3);
      }else{
        ai_rand_move($objid, 3000, 12);
      };
      wait(300);
      if(rnd_int(0,100)<10){
        $hp=get_hp($objid);
        $maxhp=get_max_hp($objid);
        if($hp<$maxhp/5){
          send_message_with_game_object(get_game_object($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector_3(0,2,0),"","啊，我要死了！",2);
        }elseif($hp<$maxhp/4){
          send_message_with_game_object(get_game_object($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector_3(0,2,0),"","啊，我要回家！",2);
        }elseif($hp<$maxhp/3){
          send_message_with_game_object(get_game_object($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector_3(0,2,0),"","啊，我伤的很重！",2);
        }elseif($hp<$maxhp/2){
          send_message_with_game_object(get_game_object($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector_3(0,2,0),"","啊，我受伤了！",2);
        }elseif($hp<$maxhp){
          send_message_with_game_object(get_game_object($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector_3(0,2,0),"","啊，好恐怖！",2);
        }else{
          send_message_with_game_object(get_game_object($objid),"TweenUiPrefabWithText",""+$objid,"UI/YellowText",vector_3(0,2,0),"","这是哪里？？？",2);
        };
      };
    };
  };
  onnamespacedmessage("on_born")
  {
    born_finish(@objid);
  };
  onnamespacedmessage("on_dead")
  {
    wait(500);
    obj_animation(@objid, "dyingbackward", 0);
    wait(5000);
    dead_finish(@objid);
  };
};