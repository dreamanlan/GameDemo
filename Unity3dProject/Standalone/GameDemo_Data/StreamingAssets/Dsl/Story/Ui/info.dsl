script(main)
{
    local
    {
        @txt(0);
    };
    onmessage("start")
    {
        bind_ui(@window){
            var("@txt","Text");
        };
        @window.SetActive(change_type(0,"bool"));
    };
    onmessage("show_info")args($id,$info,$interval,$start,$end)
    {
        @window.SetActive(change_type(1,"bool"));
        if(is_null($interval)){
            $interval = 200;
        };
        if(is_null($start)){
            $start = 0;
        };
        if($start > $info.Length){
            $start = 0;
        };
        if(is_null($end) || $end > $info.Length){
            $end = $info.Length;
        };
        $len = $end;
        $ct = $start + 1;
        //printf("info:{0} len:{1} ct:{2}",$info,$len,$ct);
        while($ct<=$len && !is_story_skipped()){
            //printf("info:{0} len:{1} ct:{2} show:{3} is_story_skipped:{4}",$info,$len,$ct,substring($info,0,$ct),is_story_skipped());
            @txt_Text.text = substring($info, 0, $ct);
            story_wait($interval);
            inc($ct);
        };
        story_wait(500);
        local_message("hide_info",$id);
    };
    onmessage("hide_info")args($id)
    {
        @window.SetActive(change_type(0,"bool"));
        fire_message("on_hide_info",$id);
    };
};