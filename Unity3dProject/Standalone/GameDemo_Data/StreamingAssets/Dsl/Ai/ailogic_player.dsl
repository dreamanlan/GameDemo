script(ai_player)
{
  local
  {
  };
  onmessage(start)
  {
    $objid = @objid;
  };
  onnamespacedmessage("on_born")
  {
    born_finish(@objid);
  };
  onnamespacedmessage("on_dead")
  {
    wait(500);
    obj_animation(@objid, "dying", 0);
    wait(5000);
    dead_finish(@objid);
  };
};