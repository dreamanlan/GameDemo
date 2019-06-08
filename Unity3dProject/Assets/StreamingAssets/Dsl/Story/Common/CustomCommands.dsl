command(TestCmd)args($a,$b,$c)
{
  if($a>1){
    TestCmd($a-1,$b,$c);
    storywait(1000);
  };
  log("{0} {1} {2}",$a,$b,$c);
};
command(Test)args($v)
{
  $val = TestVal($v,1,1);
  storywait(1000);
  TestCmd($val,$val,$val);
  storywait(1000);
  log("done.");
};