input
{
  resettrees(true);
  rect(11,11,489,489);
  list("Assets/StreamingAssets/objlist.txt");
  sampler("img", "Assets/Content/Texture/spiral.png");
}
height
{
  $x=arg(0);
  $y=arg(1);
  height = clamp(rnd_float(0,1),0,0.01)+sample_red("img", $x/2, $y/2)/500.0;
  /*
  if(height>0.1 && rnd_int(0,100)<5){
    add_tree(0,$x/128,height,$y/128,0,1,1,0x00ff00,0xff0000);
  };
  */
  $prefab="LightRed";
  $v=rnd_int(0,100);
  if($v<25){
  	$prefab="LightRed";
  }elseif($v<50){
  	$prefab="LightBlue";
  }elseif($v<75){
  	$prefab="Red";
  }else{
  	$prefab="Blue";
  };
  if(height<0.06 && rnd_int(0,100)<=1){
    add_object($x,height+rnd_float(20,25),$y,$prefab);
  };
}
alphamap
{
  $x=arg(0);
  $y=arg(1);
  $h = get_height($y, $x);
  clear_alpha();
  set_alpha(0, sin($x/180.0));
  set_alpha(1, cos($y/180.0));
}
detail
{
  $x=arg(0);
  $y=arg(1);
  $layer=arg(2);
  detail = 0;
};