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
  height = clamp(rndfloat(0,1),0,0.01)+samplered("img", $x/2, $y/2)/500.0;
  /*
  if(height>0.1 && rndint(0,100)<5){
    addtree(0,$x/128,height,$y/128,0,1,1,0x00ff00,0xff0000);
  };
  */
  $prefab="LightRed";
  $v=rndint(0,100);
  if($v<25){
  	$prefab="LightRed";
  }elseif($v<50){
  	$prefab="LightBlue";
  }elseif($v<75){
  	$prefab="Red";
  }else{
  	$prefab="Blue";
  };
  if(height<0.06 && rndint(0,100)<=1){
    addobject($x,height+rndfloat(20,25),$y,$prefab);
  };
}
alphamap
{
  $x=arg(0);
  $y=arg(1);
  $h = getheight($x, $y);
  clearalpha();
  setalpha(0, sin($x/180.0));
  setalpha(1, cos($y/180.0));
}
detail
{
  $x=arg(0);
  $y=arg(1);
  $layer=arg(2);
  detail = 0;
};