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
    bornfinish(@objid);
  };
  onnamespacedmessage("on_dead")
  {
    wait(500);
    objanimation(@objid, "dying", 0);
    wait(5000);
    deadfinish(@objid);
  };
};