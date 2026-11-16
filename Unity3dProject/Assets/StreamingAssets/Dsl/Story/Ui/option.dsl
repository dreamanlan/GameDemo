script(main)
{
  local
  {
  };
  onmessage("start")
  {
    bind_ui(@window){
      onevent("toggle","option","Toggle");
    };
    @window.SetActive(change_type(1,"bool"));
  };
  onnamespacedmessage("on_toggle")args($tag,$val,$inputs,$toggles,$sliders,$dropdowns)
  {
    if($tag=="option"){
      send_message("ScreenInputCanvas", "SetFollowYaw", $val);
      send_message("Camera", "SetFollowYaw", $val);
    };
  };
};