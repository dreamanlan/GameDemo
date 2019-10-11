script(main)
{
  local
  {
  };
  onmessage("start")
  {
    bindui(@window){
      onevent("toggle","option","Toggle");
    };
    @txt_Text.text="";
    @window.SetActive(changetype(1,"bool"));
  };
  onnamespacedmessage("on_toggle")args($tag,$val,$inputs,$toggles,$sliders,$dropdowns)
  {
    if($tag=="option"){
      sendmessage("ScreenInputCanvas", "SetFollowYaw", $val);
      sendmessage("Camera", "SetFollowYaw", $val);
    };
  };
};