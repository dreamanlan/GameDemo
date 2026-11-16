command(TestCmd)args($a,$b,$c)
{
  if($a>1){
    TestCmd($a-1,$b,$c);
    story_wait(1000);
  };
  printf("{0} {1} {2}",$a,$b,$c);
};
command(Test)args($v)
{
  $val = TestVal($v,1,1);
  story_wait(1000);
  TestCmd($val,$val,$val);
  story_wait(1000);
  printf("done.");
};