script(main)
{
    local
    {
        @txt(0);
    };
    onmessage("start")
    {
        bindui(@window){
            var("@txt","Text");
        };
        @window.SetActive(changetype(0,"bool"));
    };
    onmessage("show_info")args($id,$info,$interval,$start,$end)
    {
        @window.SetActive(changetype(1,"bool"));
        if(isnull($interval)){
            $interval = 200;
        };
        if(isnull($start)){
            $start = 0;
        };
        if($start > $info.Length){
            $start = 0;
        };
        if(isnull($end) || $end > $info.Length){
            $end = $info.Length;
        };
        $len = $end;
        $ct = $start + 1;
        //log("info:{0} len:{1} ct:{2}",$info,$len,$ct);
        while($ct<=$len && !isstoryskipped()){
            //log("info:{0} len:{1} ct:{2} show:{3} isstoryskipped:{4}",$info,$len,$ct,substring($info,0,$ct),isstoryskipped());
            @txt_Text.text = substring($info, 0, $ct);
            storywait($interval);
            inc($ct);
        };
        storywait(500);
        localmessage("hide_info",$id);
    };
    onmessage("hide_info")args($id)
    {
        @window.SetActive(changetype(0,"bool"));
        firemessage("on_hide_info",$id);
    };
};