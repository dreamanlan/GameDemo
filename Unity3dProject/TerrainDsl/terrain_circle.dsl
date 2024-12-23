input
{
  circle(64,64,32);
  sampler("img", "F:/GitHub/CSharpGameFramework/circle.png");
}
height
{
  $x=arg(0);
  $y=arg(1);
  height = clamp(rndfloat(0,1),0,0.01)+samplered("img", $x, $y)/1000.0+samplegreen("img", $x, $y)/1000.0+sampleblue("img", $x, $y)/1000.0;
}
alphamap
{
  $x=arg(0);
  $y=arg(1);
  $h = getheight($y, $x);
  clearalpha();
  setalpha(0, sin($x/180.0));
  setalpha(1, cos($y/180.0));
  setalpha(2, sin($x/180.0));
  setalpha(3, cos($y/180.0));
};