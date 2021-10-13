script(main)
{
    fileecho(true);

    cfg=arg(1);
    Configuration=arg(1);
    
    paused=true;
    if(argnum()>=3){
    	paused=changetype(arg(2), "bool");
    };

    call("setcolor");
    $encoding = getinputencoding();
    setencoding(936);
    echo("encoding:{0} {1}", $encoding.EncodingName, $encoding.EncodingName);
    echo("commandline:{0}", cmdline());
    echo("commandline args:{0}", stringjoin(",",cmdlineargs()));
    echo("args:{0}", stringjoin(",",args()));
    echo("current directory:{0}", pwd());
    echo("script directory:{0}", getscriptdir());

    rootdir=getscriptdir()+"/../..";
    plugindir=rootdir+"/Unity3dProject/Assets/Plugins/game";
    logdir=rootdir+"/BuildLog";
    libdir=rootdir+"/GameLibrary/ExternLibrary";
    
    Platform="Any CPU";
    xbuild=rootdir+"/Tools/msbuild/msbuild.exe";
    
    looplist(listhashtable(envs())){
        echo("{0}={1}",$$.Key,$$.Value);
    };

    mono=expand("%rootdir%/Tools/mono/mono.exe");
    pdb2mdb=expand("%rootdir%/Tools/lib/mono/4.5/pdb2mdb.exe");
		
    /*********************************************************************************
    * dll编译部分
    **********************************************************************************/

    cd(rootdir);
    command{
        win
        {:
            %xbuild% /version
        :};
        unix
        {:
            msbuild /version
        :};
    };
    
    createdir(logdir);

    echo("building GameLibrary.sln ...");
    $var0 = command{
        win
        {:
            %xbuild% /m /nologo /noconsolelogger /p:Configuration=%cfg% /flp:LogFile=%logdir%/GameLibrary.sln.log;Encoding=UTF-8 /t:clean;rebuild %rootdir%/GameLibrary/GameLibrary.sln /p:Platform="Any CPU"
        :};
        unix
        {:
            msbuild /m /nologo /noconsolelogger /p:Configuration=%cfg% /flp:LogFile=%logdir%/GameLibrary.sln.log;Encoding=UTF-8 /t:clean;rebuild %rootdir%/GameLibrary/GameLibrary.sln /p:Platform="Any CPU"
        :};
    };
    
    echo("result:{0}", $var0);
    if($var0){
        setfgcolor("Red");
        echo("compile GameLibrary.sln failed, see Client/BuildLog/GameLibrary.sln.log");
        call("setcolor");
    };

    cd(rootdir+"/GameLibrary/App/GameKernel/bin/%cfg%");
    echo("curdir:{0}",pwd());
    looplist(listfiles(rootdir+"/GameLibrary/App/GameKernel/bin/%cfg%","*.dll")){
        $filename = getfilename($$);
        $targetPath = plugindir+"/"+$filename;
        copyfile($$, $targetPath);
        if(osplatform()=="Unix"){
			process(mono, pdb2mdb + " " + $filename);
        }else{
            echo("{0} {1}", pdb2mdb, $filename);
            process(mono, pdb2mdb + " " + $filename);
        };
        //echo("copy {0} to {1}", $$, $targetPath);
    };
    copydir(rootdir+"/GameLibrary/App/GameKernel/bin/%cfg%", plugindir, "*.mdb");

    deletefile(plugindir+"/UnityEngine.dll");
    deletefile(plugindir+"/UnityEngine.UI.dll");
    deletefile(plugindir+"/UnityEngine.Timeline.dll");
    deletefile(plugindir+"/UnityEditor.dll");
    cd(rootdir);

    if($var0){
        setfgcolor("Red");
        echo("compile failed !");
        call("setcolor");
    };

    resetcolor();
    if(paused){
	    echo("press any key ...");
	    pause();
	  };
    return($var0);
};

script(setcolor)
{
    if(osplatform()=="Unix"){
        setfgcolor("Blue");
    }else{
        setfgcolor("Yellow");
    };
};