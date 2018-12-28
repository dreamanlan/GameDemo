script(main)
{
    echo("commandline:{0}", cmdline());
    echo("commandline args:{0}", stringjoin(",",cmdlineargs()));
    echo("args:{0}", stringjoin(",",args()));
    echo("current directory:{0}", pwd());
    echo("script directory:{0}", getscriptdir());
		var(0) = osplatform();
    loop(5){
    	if(var(0)=="Unix"){
            process("open", "robot.app -n --args -batchmode -nographics -suffix _"+$$+" -robot "+$$){
            	nowait(true);
            };
    	}else{
            process("robot.exe", "-batchmode -nographics -suffix _"+$$+" -robot "+$$){
            	nowait(true);
            };
        };
        echo("robot {0} start", $$);
    };
    
    waitall();
    return(0);
};