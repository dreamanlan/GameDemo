@@delimiter(script, $____begin____, $_____end_____);

//$include("StoryDsl.txt");

@doc
$____begin____

一、我们的剧情系统脚本与技能系统使用同一语法的DSL语言（解析器源码在https://github.com/dreamanlan/DSL）。

二、DSL语法元素（本部分可跳过不看）

	DSL语言的语句都以;结束，这可能是最有可能写错的地方，在实际使用时一定要注意，任何语句结束一定要写上;号。

	DSL语言在语法层面都是由语句构成，语句由函数级联而成，最后加一个分号表示语句结束：

  语句 ::= ｛ 函数 ｝;
  函数 ::= 函数调用 \{ 语句列表 \}

  函数调用 ::= 函数名 ( 函数参数表 )
  函数调用 ::= 函数调用 ( 函数参数表 )

  对语句，函数表为可选，亦即单独一个分号也构成一个语句。
  对函数，函数调用、语句部分以及函数调用的函数名部分都是可选的。如下形式都是合法的函数语法：

  (a,b,c,d)

  或

  (a,b,c,d)
	{
	  cmd1();
	  cmd2();
  }

  或

	{
	  cmd1();
	  cmd2();
  }

  函数调用可以级联，即一个函数调用返回一个函数，然后再调用：

	getSomething()(a,b,c,d);

  包含语句的函数不能直接用于构成函数调用，如下语法是合法的语法，但不是函数调用，而是语句：

	func(a,b)
	{
	  cmd1();
	  cmd2();
  }(e,f,g);

  这样的语法实际上被解析成语句的第一个函数与第二个函数，第二个函数是一个仅由函数参数表构成的函数。

  另外一种函数调用的变体是表达式语法：

	a + b 等价于 +(a,b)

	DSL语言支持三种形式的注释：

  /**/
  //
  #

  后两种是单行注释，第一种是与c语言相同的块注释。

	DSL语言里的字符串可以用单引号与双引号括起来。不带空格的字符串也可省略引号。目前true/false也被解释为字符串。可以认为DSL语言只有字符串与浮点数2种类型。
  函数名也是字符串，所以也可以是用引号括起来的。

三、用于剧情的DSL

  剧情脚本语法如下：

	story(main)
	{
	  local
	  {
	      @var1(0);
	      @var2(1);
	  };
	  onmessage("start")
	  {

	  };
	  onmessage("allnpckilled")
	  {

	  };
	};

  目前的实现，一个场景的story总是从main开始执行。story的名称在场景内唯一即可。一个story由局部变量定义与多个消息处理构成，即上面的onmessage语句。onmessage语句是由onmessage函数+分号构成。
	onmessage函数的参数表共同构成消息处理的消息id，多个参数会用:连接成一个字符串构成消息id，比如：

	onmessage("npckilled",12)
	{
	};

  这个消息处理的实际消息id是“npckilled:12”，在我们的游戏系统里触发消息时实际发送的消息ID就是这个。上面的写法与下面写法是完全等价的：

	onmessage("npckilled:12")
	{
	};

  消息id都是在story解析时就能确定的常量value（所以这部分不能出现任何变量）。

	story支持局部变量与全局变量，有一些全局变量将来会由游戏系统定义。局部变量命名必须是@局部变量名，全局变量命名必须是@@全局变量名。除@@变名的全局变量处，story还提供一种动
  态key/value方式的类全局变量，通过propget/propset来读写（它们也能用于全局变量与局部变量读写）。

  局部变量可以在story的local部分定义并指定一个初始value（这里指定的value必须是常量）。全局变量与局部变量都可以在消息处理里直接通过赋value产生新的变量，没有赋value的变量直接读取将得到未知value，所以一定要
  保证变量地读取前有赋value，对局部变量，最好是在local里定义。

  消息在触发时可以带有多个参数，在消息处理里，这些参数可以通过 $+索引 来访问，$0代表第一个参数，$1代表第二个参数，以此类推。

  消息处理也可以通过args语法来绑定参数到变量：

	onmessage("npckilled",12)args(@objId,@leftCt)
	{
	};

  上面写法相当于在消息处理的开头加上(变量与参数按顺序匹配)：

  @objId=$0;
  @leftCt=$1;

  消息处理由具体的处理语句构成。语句部分有三大类元素，一类是通用的类似传统程序语言里的语句，一类是特定的用于实现story逻辑的命令，还有一类是用于变量运算的函数。

  剧情命令与函数利用dotnet反射机制调用dotnet对象成员，此时可以采用与c#相似的写法：

	obj.property = val;
	val = obj.property;
	obj.method(arg1,arg2,...);
	val = obj.method(arg1,arg2,...);

  剧情脚本支持json的literal写法：

  [1,3,4,5];
	{'a' => 1, 'b' => 2, 'c' => 3};

  注：在dsl用于UI脚本时，每一个UI对应一个脚本，为了方便配置，不在dsl里出现UI的名字，消息处理采用下述写法：
	onnamespacedmessage(msgId)
	{
	};
  对于名为ns的UI，上述处理等价于（也就是说这是一种带隐藏名字空间的机制）：
	onmessage(ns,msgId)
	{
	};


四、剧情语句API

$_____end_____;

@snippets("assign")
$____begin____

assign(@var,value);

$_____end_____
@doc
$____begin____

作用：
  赋值操作，其中value可以是常量或者是一个表达式，如果是表达式最好括起来。在后续API，所有value也是这样。

$_____end_____;

@snippets("=")
$____begin____

@var=value;

$_____end_____
@doc
$____begin____

作用：
  赋值操作，其中value可以是常量或者是一个表达式，如果是表达式最好括起来。在后续API，所有value也是这样。

$_____end_____;

@snippets("inc")
$____begin____

inc(@var);
inc(@var,value);

$_____end_____
@doc
$____begin____

作用：
  自增操作。

$_____end_____;

@snippets("dec")
$____begin____

dec(@var);
dec(@var,value);

$_____end_____
@doc
$____begin____

作用：
  自减操作。

$_____end_____;

@snippets("prop_set")
$____begin____

prop_set(value,value);

$_____end_____
@doc
$____begin____

作用：
  第一个value必须是一个字符串value，表示变量名(对于局部变量与全局变量，名字必须包括特殊的@与@@前缀)。

$_____end_____;

@snippets("foreach")
$____begin____

foreach(value1,value2,value3,...)
{
  cmd1($$);
  cmd2($$);
};

$_____end_____
@doc
$____begin____

作用：
  对foreach部分指硋alue膙alue，依次执行命令列表部分，在命令中通过$$来获取当前遍历执行的value。

$_____end_____;

@snippets("loop")
$____begin____

loop(value)
{
  cmd1($$);
  cmd2($$);
};

$_____end_____
@doc
$____begin____

作用：
  执行命令列表由value指定的次数，在命令中通过$$来获取当前第几次执行，从0开始。

$_____end_____;

@snippets("while")
$____begin____

while(value)
{
  cmd1();
  cmd2();
};

$_____end_____
@doc
$____begin____

作用：
  如果value不为0就循环执行命令列表。

$_____end_____;

@snippets("if")
$____begin____

if(value)
{
  cmd1();
  cmd2();
};

$_____end_____
@snippets
$____begin____

if(value)
{
  cmd1();
  cmd2();
}
else
{
  cmd3();
  cmd4();
};

$_____end_____
@snippets
$____begin____

if(value1)
{
  cmd1();
  cmd2();
}
elseif(value2)
{
  cmd3();
  cmd4();
}
elseif(valuen)
{
  cmd5();
  cmd6();
}
else
{
  cmd7();
  cmd8();
}

$_____end_____
@doc
$____begin____

作用：
  如果value不为0则执行cmd1与cmd2命令列表，否则如果有else则执行cmd3与cmd4命令列表。

$_____end_____;

@snippets("wait")
$____begin____

wait(毫秒数)[if(条件表达式)];

$_____end_____
@doc
$____begin____

作用：
  暂停当前的消息处理指定时间。注意，一个消息处理如果没有wait或sleep调用，会一直执行直到结束。因此如果有大的循环处理，里面应该调用sleep(0);来
  释放控制权以允许其它逻辑正常执行。if部分为可选，用于在条件不满足时提前结束等待。

$_____end_____;

@snippets("sleep")
$____begin____

sleep(毫秒数)[if(条件表达式)];

$_____end_____
@doc
$____begin____

作用：
  暂停当前的消息处理指定时间。注意，一个消息处理如果没有wait或sleep调用，会一直执行直到结束。因此如果有大的循环处理，里面应该调用sleep(0);来
  释放控制权以允许其它逻辑正常执行。if部分为可选，用于在条件不满足时提前结束等待。

$_____end_____;

@snippets("story_wait")
$____begin____

story_wait(毫秒数)[if(条件表达式)];

$_____end_____
@doc
$____begin____

作用：
  暂停当前的消息处理指定时间。注意，一个消息处理如果没有wait或sleep调用，会一直执行直到结束。因此如果有大的循环处理，里面应该调用sleep(0);来
  释放控制权以允许其它逻辑正常执行。if部分为可选，用于在条件不满足时提前结束等待。如果取消按钮按下，则等待直接在下一tick结束

$_____end_____;

@snippets("storysleep")
$____begin____

storysleep(毫秒数)[if(条件表达式)];

$_____end_____
@doc
$____begin____

作用：
  暂停当前的消息处理指定时间。注意，一个消息处理如果没有wait或sleep调用，会一直执行直到结束。因此如果有大的循环处理，里面应该调用sleep(0);来
  释放控制权以允许其它逻辑正常执行。if部分为可选，用于在条件不满足时提前结束等待。如果取消按钮按下，则等待直接在下一tick结束

$_____end_____;

@snippets("printf","command")
$____begin____

printf(format，参数表);

$_____end_____
@doc
$____begin____

作用：
  按format指定的格式格式化参数表各个value，然后输出拼接好的字符串到调试日志。

$_____end_____;

@snippets("local_message")
$____begin____

local_message(消息ID，参数表);

$_____end_____
@doc
$____begin____

作用：
  触发一个本story内的消息。注意消息ID是一个字符串，对应onmessage的参数部分所描蕍alue南D，onmessage有多个参数则用:把各参数拼接成消息ID。这里
  的参数表的各个value在onmessage处理里可以通过$0,$1,...来访问。

$_____end_____;

@snippets("story_local_message")
$____begin____

story_local_message(消息ID，参数表);

$_____end_____
@doc
$____begin____

作用：
  触发一个本story内的消息。注意消息ID是一个字符串，对应onmessage的参数部分所描蕍alue南D，onmessage有多个参数则用:把各参数拼接成消息ID。这里
  的参数表的各个value在onmessage处理里可以通过$0,$1,...来访问。如果剧情跳过按钮被按下了，则不触发消息。

$_____end_____;

@snippets("terminate")
$____begin____

terminate();

$_____end_____
@doc
$____begin____

作用：
  结束当前story的执行。

$_____end_____;

@snippets("loop_list")
$____begin____

loop_list(value)
{
  	cmd1();
  	cmd2();
};

$_____end_____
@doc
$____begin____

作用：
  对value指定的list的各个value遍历循环执行命令列表，在命令中通过$$来获得当前遍历到的list元素。

$_____end_____;

@snippets("list_set")
$____begin____

list_set(list,index,value);

$_____end_____
@doc
$____begin____

作用：
  修改指定list的index元素value为value。

$_____end_____;

@snippets("list_add")
$____begin____

list_add(list,value);

$_____end_____
@doc
$____begin____

作用：
  给list添加值为value的元素。

$_____end_____;

@snippets("list_remove")
$____begin____

list_remove(list,value);

$_____end_____
@doc
$____begin____

作用：
  删除list里值为value的元素。

$_____end_____;

@snippets("list_insert")
$____begin____

list_insert(list,index,value);

$_____end_____
@doc
$____begin____

作用：
  往list里index位置插入值为value的元素。

$_____end_____;

@snippets("list_remove_at")
$____begin____

list_remove_at(list,index);

$_____end_____
@doc
$____begin____

作用：
  删除list里索引为index的元素。

$_____end_____;

@snippets("list_clear")
$____begin____

list_clear(list);

$_____end_____
@doc
$____begin____

作用：
  清空list。

$_____end_____;

@snippets("clear_message")
$____begin____

clear_message(msgid1,msgid2,...);

$_____end_____
@doc
$____begin____

作用：
  清空指定的消息队列，如果不指定参数刚清空所有消息队列。

$_____end_____;

@snippets("dotnetexec")
$____begin____

dotnetexec(obj/type,method,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  调用指定类/对象的方法。

$_____end_____;

@snippets("dotnet_set")
$____begin____

dotnet_set(obj/type,property,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  设置指定类/对象的特性/字段。

$_____end_____;

@snippets("write_all_lines")
$____begin____

write_all_lines(file,lines);

$_____end_____
@doc
$____begin____

作用：
  写文件。

$_____end_____;

@snippets("write_file")
$____begin____

write_file(file,txt);

$_____end_____
@doc
$____begin____

作用：
  写文件。

$_____end_____;

@snippets("hashtable_add")
$____begin____

hashtable_add(hashtable,key,value);

$_____end_____
@doc
$____begin____

作用：
  往哈希表里添加元素。

$_____end_____;

@snippets("hashtable_set")
$____begin____

hashtable_set(hashtable,key,value);

$_____end_____
@doc
$____begin____

作用：
  往哈希表里添加或修改元素。

$_____end_____;

@snippets("hashtable_remove")
$____begin____

hashtable_remove(hashtable,key);

$_____end_____
@doc
$____begin____

作用：
  移除哈希表里的元素。

$_____end_____;

@snippets("hashtable_clear")
$____begin____

hashtable_clear(hashtable);

$_____end_____
@doc
$____begin____

作用：
  清空哈希表。

$_____end_____;

@snippets("wait_local_message")
$____begin____

wait_local_message(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
waitlocalmessage等待当前story的指定消息之一触发。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)

$_____end_____;

@snippets("wait_local_message_handler")
$____begin____

wait_local_message_handler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
waitlocalmessagehandler等待当前story的指定消息处理全部执行完毕。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)

$_____end_____;

@snippets("story_wait_local_message")
$____begin____

story_wait_local_message(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
storywaitlocalmessage等待当前story的指定消息之一触发。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)。如果剧情取消按钮被按下，则不再等待按超时处理

$_____end_____;

@snippets("story_wait_local_message_handler")
$____begin____

story_wait_local_message_handler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
storywaitlocalmessagehandler等待当前story的指定消息处理全部执行完毕。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)。如果剧情取消按钮被按下，则不再等待按超时处理

$_____end_____;

@snippets("pauselocalmessagehandler")
$____begin____

pauselocalmessagehandler(msgid,msgid,...);

$_____end_____
@doc
$____begin____

作用：
  暂停同一story里的指定的消息处理。

$_____end_____;

@snippets("resume_local_message_handler")
$____begin____

resume_local_message_handler(msgid,msgid,...);

$_____end_____
@doc
$____begin____

作用：
  恢复同一story里的指定的消息处理。

$_____end_____;

@snippets("pauselocalnamespacedmessagehandler")
$____begin____

pauselocalnamespacedmessagehandler(msgid,msgid,...);

$_____end_____
@doc
$____begin____

作用：
  暂停同一story里的指定的名字空间里的消息处理。

$_____end_____;

@snippets("resume_local_namespaced_message_handler")
$____begin____

resume_local_namespaced_message_handler(msgid,msgid,...);

$_____end_____
@doc
$____begin____

作用：
  恢复同一story里的指定的名字空间里的消息处理。

$_____end_____;

@snippets("pause")
$____begin____

pause();

$_____end_____
@doc
$____begin____

作用：
  暂停当前story

$_____end_____;

@doc
$____begin____

五、运算

1、数字加     +

2、数字减     -

3、数字乘     *

4、数字比较   >=

5、数字比较   >

6、数字比较   ==

7、数字比较   <=

8、数字比较   <

9、逻辑与     &&

10、逻辑或    ||

11、逻辑非    !

12、数学除    /

13、数学取模  %

$_____end_____;

@order(14);

@snippets("abs")
$____begin____

abs(x);

$_____end_____;

@snippets("floor")
$____begin____

floor(x);

$_____end_____;

@snippets("ceiling")
$____begin____

ceiling(x);

$_____end_____;

@snippets("round")
$____begin____

round(x);

$_____end_____;

@snippets("pow")
$____begin____

pow(x,y); or pow(x);

$_____end_____;

@snippets("log","function")
$____begin____

log(x,y); or log(x);

$_____end_____;

@snippets("sqrt")
$____begin____

sqrt(x);

$_____end_____;

@snippets("sin")
$____begin____

sin(x);

$_____end_____;

@snippets("cos")
$____begin____

cos(x);

$_____end_____;

@snippets("sinh")
$____begin____

sinh(x);

$_____end_____;

@snippets("cosh")
$____begin____

cosh(x);

$_____end_____;

@snippets("min")
$____begin____

min(x,y,z,...);

$_____end_____;

@snippets("max")
$____begin____

max(x,y,z,...);

$_____end_____;

@doc
$____begin____

六、内部函数

$_____end_____;

@snippets("vector_2")
$____begin____

vector_2(x,y);

$_____end_____
@doc
$____begin____

作用：
  构造一个vector2对象。

$_____end_____;

@snippets("vector_3")
$____begin____

vector_3(x,y,z);

$_____end_____
@doc
$____begin____

作用：
  构造一个vector3对象。

$_____end_____;

@snippets("vector_4")
$____begin____

vector_4(x,y,z,w);

$_____end_____
@doc
$____begin____

作用：
  构造一个vector4对象。

$_____end_____;

@snippets("quaternion")
$____begin____

quaternion(x,y,z,w);

$_____end_____
@doc
$____begin____

作用：
  构造一个quaternion对象。

$_____end_____;

@snippets("eular")
$____begin____

eular(x,y,z);

$_____end_____
@doc
$____begin____

作用：
  基于欧拉角构造一个quaternion对象。

$_____end_____;

@snippets("time")
$____begin____

time();

$_____end_____
@doc
$____begin____

作用：
  获取当前时间（单位：毫秒）

$_____end_____;

@snippets("format")
$____begin____

format(format_string，参数列表);

$_____end_____
@doc
$____begin____

作用：
  按格式化串格式化指定的参数列表，构成一个字符串。

$_____end_____;

@snippets("prop_get")
$____begin____

prop_get(value[,defaultvalue]);

$_____end_____
@doc
$____begin____

作用：
value必须是个字符串value，表示要读取value的变量名。此函数用于动态获取变量value。

$_____end_____;

@snippets("int_list")
$____begin____

int_list("1 2 3 4 5");

$_____end_____
@doc
$____begin____

作用：
  从字符串里读取数字列表，分隔符可以是' '、','、'|'之一。返回value用于looplist语句遍历。

$_____end_____;

@snippets("string_list")
$____begin____

string_list("a b c d e");

$_____end_____
@doc
$____begin____

作用：
  从字符串里读取字符串列表，分隔符可以是' '、','、'|'之一。返回value用于looplist语句遍历。

$_____end_____;

@snippets("vector_2_list")
$____begin____

vector_2_list("v1x v1y v2x v2y");

$_____end_____
@doc
$____begin____

作用：
  从字符串里读取vector2列表，分隔符可以是' '、','、'|'之一。返回value用于looplist语句遍历。

$_____end_____;

@snippets("vector_3_list")
$____begin____

vector_3_list("v1x v1y v1z v2x v2y v2z");

$_____end_____
@doc
$____begin____

作用：
  从字符串里读取vector3列表，分隔符可以是' '、','、'|'之一。返回value用于looplist语句遍历。

$_____end_____;

@snippets("list")
$____begin____

list(1,2,3,4,5);

$_____end_____
@doc
$____begin____

作用：
  由参数表构成一个列表。返回value用于looplist语句遍历。

$_____end_____;

@snippets("substring")
$____begin____

substring(字符串,start[,length]);

$_____end_____
@doc
$____begin____

作用：
  获取字符串子串。

$_____end_____;

@snippets("rnd_int")
$____begin____

rnd_int(min,max);

$_____end_____
@doc
$____begin____

作用：
  获得一个介于[min,max)间的随机整数。

$_____end_____;

@snippets("rnd_float")
$____begin____

rnd_float();

$_____end_____
@doc
$____begin____

作用：
  获得一个0~1之前的随机浮点数。

$_____end_____;

@snippets("float_list")
$____begin____

float_list("1.0 2.0 3.0 4.0 5.0");

$_____end_____
@doc
$____begin____

作用：
  从字符串里读取数字列表，分隔符可以是' '、','、'|'之一。返回value用于looplist语句遍历。

$_____end_____;

@snippets("rnd_from_list")
$____begin____
  用法:
rnd_from_list(int_list("1 2 3 4 5 6 7 8")[,defaultvalue]);

$_____end_____
@doc
$____begin____

作用：
  从指定的list里随机取一个value。

$_____end_____;

@snippets("list_get")
$____begin____

list_get(list,index[,defaultvalue]);

$_____end_____
@doc
$____begin____

作用：
  从指定的list取第index个元素。

$_____end_____;

@snippets("list_size")
$____begin____

list_size(list);

$_____end_____
@doc
$____begin____

作用：
  取指定list的元素个数。

$_____end_____;

@snippets("vector_2_dist")
$____begin____

vector_2_dist(vector_2(1,2),vector_2(3,4));

$_____end_____
@doc
$____begin____

作用：
  计算2点距离。

$_____end_____;

@snippets("vector_3_dist")
$____begin____

vector_3_dist(vector_3(1,2,3),vector_3(4,5,6));

$_____end_____
@doc
$____begin____

作用：
  计算2点距离（2D距离）。

$_____end_____;

@snippets("vector_2_to_3")
$____begin____

vector_2_to_3(vector_2(1,2));

$_____end_____
@doc
$____begin____

作用：
  将vector2转换成vector3，yvalue部分置0。

$_____end_____;

@snippets("vector_3_to_2")
$____begin____

vector_3_to_2(vector_3(1,2,3));

$_____end_____
@doc
$____begin____

作用：
  将vector3转换成vector2，丢掉yvalue部分。

$_____end_____;

@snippets("is_null")
$____begin____

is_null(x);

$_____end_____
@doc
$____begin____

作用：
  判断指定value是否为空。

$_____end_____;

@snippets("str_2_int")
$____begin____

str_2_int(str);

$_____end_____
@doc
$____begin____

作用：
  将字符串转成整数。

$_____end_____;

@snippets("str_2_float")
$____begin____

str_2_float(str);

$_____end_____
@doc
$____begin____

作用：
  将字符串转成浮点数。

$_____end_____;

@snippets("dotnet_call")
$____begin____

dotnet_call(obj/type,method,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  调用指定类/对象的方法，并取得返回value。

$_____end_____;

@snippets("dotnet_get")
$____begin____

dotnet_get(obj/type,property,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  获取指定类/对象的特性/字段的value。

$_____end_____;

@snippets("change_type")
$____begin____

change_type(value,obj/type);

$_____end_____
@doc
$____begin____

作用：
  改变value的类型，返回新类型的value。(type可以是sbyte、byte、short、ushort、int、uint、long、ulong、float、double、string、bool等)

$_____end_____;

@snippets("parse_enum")
$____begin____

parse_enum(obj/type, "enumvalue");

$_____end_____
@doc
$____begin____

作用：
  按指定枚举类型解析字符串，返回一个枚举值。

$_____end_____;

@snippets("read_all_lines")
$____begin____

read_all_lines(file);

$_____end_____
@doc
$____begin____

作用：
  读文件所有行，返回文件行内容列表。

$_____end_____;

@snippets("read_file")
$____begin____

read_file(file);

$_____end_____
@doc
$____begin____

作用：
  读文件，返回文件内容字符串。

$_____end_____;

@snippets("to_json")
$____begin____

to_json(hashtable);

$_____end_____
@doc
$____begin____

作用：
  将哈希表转为json字符串。

$_____end_____;

@snippets("from_json")
$____begin____

from_json(str);

$_____end_____
@doc
$____begin____

作用：
  解析指定json字符串，返回哈希表或数组。

$_____end_____;

@snippets("hashtable")
$____begin____

hashtable('a' => 1, 'b' => 2, 'c' => 3);
  或
{'a' => 1, 'b' => 2, 'c' => 3};

$_____end_____
@doc
$____begin____

作用：
  构造一个哈希表。

$_____end_____;

@snippets("hashtable_get")
$____begin____

hashtable_get(hashtable,key[,defval]);

$_____end_____
@doc
$____begin____

作用：
  获取哈希表指定名称或索引的value。（可选指定缺省value）

$_____end_____;

@snippets("hashtable_size")
$____begin____

hashtable_size(hashtable);

$_____end_____
@doc
$____begin____

作用：
  获取指定哈希表的元素个数。

$_____end_____;

@snippets("hashtable_keys")
$____begin____

hashtable_keys(hashtable);

$_____end_____
@doc
$____begin____

作用：
  获取指定哈希表的key列表，结果可用于looplist语句。

$_____end_____;

@snippets("hashtable_values")
$____begin____

hashtable_values(hashtable);

$_____end_____
@doc
$____begin____

作用：
  获取指定哈希表的value列表，结果可用于looplist语句。

$_____end_____;

@snippets("dict_format")
$____begin____

dict_format(dictId,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  按譾alue浜胖付ǖ母袷交袷交址?

$_____end_____;

@snippets("dict_get")
$____begin____

dict_get(dictId);

$_____end_____
@doc
$____begin____

作用：
  读取譾alue浜哦杂Φ淖址?

$_____end_____;

@snippets("dictparse")
$____begin____

dictparse(str_from_server);

$_____end_____
@doc
$____begin____

作用：
  对服务器传回使用了譾alue涞淖址凶value涮婊弧?

$_____end_____;

@snippets("get_type")
$____begin____

get_type("System.Console");

$_____end_____
@doc
$____begin____

作用：
  获取指定名称的Class的Type。
  静态类名写法：TopNamespace.SubNameSpace.ContainingClass+NestedClass, MyAssembly, Version=1.3.0.0, Culture=neutral, PublicKeyToken=b17a5c561934e089

$_____end_____;

@snippets("rnd_vector_3")
$____begin____

rnd_vector_3(center,radius);

$_____end_____
@doc
$____begin____

作用：
  取指定点周围的随机点。

$_____end_____;

@snippets("rnd_vector_2")
$____begin____

rnd_vector_2(center,radius);

$_____end_____
@doc
$____begin____

作用：
  取指定点周围的随机点。

$_____end_____;

@doc
$____begin____

七、剧情函数

$_____end_____;

@snippets("obj_id_2_unit_id")
$____begin____

obj_id_2_unit_id(objid);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的unitid。

$_____end_____;

@snippets("unit_id_2_obj_id")
$____begin____

unit_id_2_obj_id(unitid);

$_____end_____
@doc
$____begin____

作用：
  取得指定unitid对象的objid，仅对npc有效。

$_____end_____;

@snippets("get_position")
$____begin____

get_position(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的位置，可用于需要位置的函数与命令。

$_____end_____;

@snippets("get_camp")
$____begin____

get_camp(objid);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的阵营，用于判断敌我关系。

$_____end_____;

@snippets("is_enemy")
$____begin____

is_enemy(camp1,camp2);

$_____end_____
@doc
$____begin____

作用：
  判断2个阵营是否敌对。

$_____end_____;

@snippets("is_friend")
$____begin____

is_friend(camp1,camp2);

$_____end_____
@doc
$____begin____

作用：
  判断2个阵营是否友好。

$_____end_____;

@snippets("get_position_x")
$____begin____

get_position_x(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的位置，可用于需要位置的函数与命令。

$_____end_____;

@snippets("getpostiony")
$____begin____

get_position_y(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的位置，可用于需要位置的函数与命令。

$_____end_____;

@snippets("get_position_z")
$____begin____

get_position_z(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的位置，可用于需要位置的函数与命令。

$_____end_____;

@snippets("get_rotation")
$____begin____

get_rotation(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的旋转（欧拉角）。

$_____end_____;

@snippets("get_rotation_x")
$____begin____

get_rotation_x(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的旋转（欧拉角）。

$_____end_____;

@snippets("get_rotation_y")
$____begin____

get_rotation_y(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的旋转（欧拉角）。

$_____end_____;

@snippets("get_rotation_z")
$____begin____

get_rotation_z(objid[, world_or_local]);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的旋转（欧拉角）。

$_____end_____;

@snippets("get_scale")
$____begin____

get_scale(objid);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的缩放。

$_____end_____;

@snippets("get_scale_x")
$____begin____

get_scale_x(objid);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的缩放。

$_____end_____;

@snippets("get_scale_y")
$____begin____

get_scale_y(objid);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的缩放。

$_____end_____;

@snippets("get_scale_z")
$____begin____

get_scale_z(objid);

$_____end_____
@doc
$____begin____

作用：
  取得指定objid对象的缩放。

$_____end_____;

@snippets("get_hp")
$____begin____

get_hp(objid);

$_____end_____
@doc
$____begin____

作用：
  获取指定对象的hp

$_____end_____;

@snippets("get_max_hp")
$____begin____

get_max_hp(objid);

$_____end_____
@doc
$____begin____

作用：
  获取指定对象的maxhp属性value。

$_____end_____;

@snippets("calc_dir")
$____begin____

calc_dir(objid,targetid);

$_____end_____
@doc
$____begin____

作用：
  计算从指定对象面向目标对象的方向value。（弧度）

$_____end_____;

@snippets("get_level")
$____begin____

get_level(objid);

$_____end_____
@doc
$____begin____

作用：
  获取指定对象的等级。

$_____end_____;

@snippets("get_attr")
$____begin____

get_attr(objid,attrid,value[,defaultvalue]);

$_____end_____
@doc
$____begin____

作用：
  获取指定对象的指定属性value。

$_____end_____;

@snippets("is_control_by_story")
$____begin____

is_control_by_story(objid);

$_____end_____
@doc
$____begin____

作用：
  判断指定对象当前是否标记了由story脚本控制。

$_____end_____;

@snippets("get_unity_type")
$____begin____

get_unity_type(typename);

$_____end_____
@doc
$____begin____

作用：
  获取指定类型的unity3d类型Type实例。

$_____end_____;

@snippets("get_unity_ui_type")
$____begin____

get_unity_ui_type(typename);

$_____end_____
@doc
$____begin____

作用：
  获取指定类型的unity3d UI类型Type实例。

$_____end_____;

@snippets("getscripttype")
$____begin____

getscripttype(type_name);

$_____end_____
@doc
$____begin____

作用：
  获取Assembly-CSharp.dll里指定类型的对象。

$_____end_____;

@snippets("get_entity_info")
$____begin____

get_entity_info(objid);

$_____end_____
@doc
$____begin____

作用：
  获取指定id的EntityInfo对象。

$_____end_____;

@snippets("get_entity_view")
$____begin____

get_entity_view(objid);

$_____end_____
@doc
$____begin____

作用：
  获取指定id的EntityViewModel对象。

$_____end_____;

@snippets("deg_2_rad")
$____begin____

deg_2_rad(val);

$_____end_____
@doc
$____begin____

作用：
  将角度转换为弧度。

$_____end_____;

@snippets("rad_2_deg")
$____begin____

rad_2_deg(val);

$_____end_____
@doc
$____begin____

作用：
  将弧度转换为角度。

$_____end_____;

@snippets("npc_get_ai_param")
$____begin____

npc_get_ai_param(unitId,index);

$_____end_____
@doc
$____begin____

作用：
  获得指定AI参数。

$_____end_____;

@snippets("obj_get_ai_param")
$____begin____

obj_get_ai_param(objId,index);

$_____end_____
@doc
$____begin____

作用：
  获得指定AI参数。

$_____end_____;

@snippets("get_player_id")
$____begin____

get_player_id();

$_____end_____
@doc
$____begin____

作用：
  获取当前玩家的objid。

$_____end_____;

@snippets("find_obj_id")
$____begin____

find_obj_id(type, vector_3(x, 0 ,z), range);

$_____end_____
@doc
$____begin____

作用：
  查找符合条件的obj的objid。type为TypedEntityEnum的值：
  public enum TypedEntityEnum
  {
      Player = 0,
      Npc,
      MaxTypeNum
  }

$_____end_____;

@snippets("find_obj_ids")
$____begin____

find_obj_ids(type, vector_3(x, 0 ,z), range);

$_____end_____
@doc
$____begin____

作用：
  查找符合条件的obj的objid。type为TypedEntityEnum的值：
  public enum TypedEntityEnum
  {
      Player = 0,
      Npc,
      MaxTypeNum
  }

$_____end_____;

@snippets("find_all_obj_ids")
$____begin____

find_all_obj_ids(vector_3(x, 0 ,z), range);

$_____end_____
@doc
$____begin____

作用：
  查找符合条件的obj的objid。

$_____end_____;

@snippets("npc_get_npc_type")
$____begin____

npc_get_npc_type(unitid);

$_____end_____
@doc
$____begin____

作用：
  获取指定npc的类型。

$_____end_____;

@snippets("obj_get_npc_type")
$____begin____

obj_get_npc_type(objid);

$_____end_____
@doc
$____begin____

作用：
  获取指定obj的类型。

$_____end_____;

@snippets("calc_offset")
$____begin____

calc_offset(objid,targetobjid,vector_3(x,y,z));

$_____end_____
@doc
$____begin____

作用：
  计算沿objid->targetobjid方向，相对objid偏移(x,y,z)的位置。

$_____end_____;

@snippets("is_visible")
$____begin____

is_visible(objid);

$_____end_____
@doc
$____begin____

作用：
  获取指定objid的显示状态。

$_____end_____;

@doc
$____begin____

八、剧情命令

$_____end_____;

@snippets("start_story")
$____begin____

start_story("clientlogic");

$_____end_____
@doc
$____begin____

作用：
  启动指定的story。

$_____end_____;

@snippets("stop_story")
$____begin____

stop_story("clientlogic");

$_____end_____
@doc
$____begin____

作用：
  停止指定的story。

$_____end_____;

@snippets("fire_message")
$____begin____

fire_message(消息id,参数表);

$_____end_____
@doc
$____begin____

作用：
  触发指定消息，与localmessage的不同是这个命令触发的消息将广播到当前所有在执行的story。

$_____end_____;

@snippets("change_scene")
$____begin____

change_scene(scene_name);

$_____end_____
@doc
$____begin____

作用：
  切换到目标场景。

$_____end_____;

@snippets("create_npc")
$____begin____

create_npc(npc_unit_id,vector_3(x,y,z),dir,camp,linkId[,ai,string_list("param1 param2 param3 ...")])[objid("@objid")];

$_____end_____
@doc
$____begin____

作用：
  创建npc，可指定位置与朝向。带objid语法的样式用于获取objid到指定变量名（字符串名，可以是运算结果构成的变量名）

$_____end_____;

@snippets("destroy_npc")
$____begin____

destroy_npc(npc_unit_id);

$_____end_____
@doc
$____begin____

作用：
  删除指定unit_id的npc，如果有多个，则都会删除。

$_____end_____;

@snippets("npc_move")
$____begin____

npc_move(unit_id,vector_3(x,y,z)[, event]);

$_____end_____
@doc
$____begin____

作用：
  移动指定npc到目标点。

$_____end_____;

@snippets("npc_enable_ai")
$____begin____

npc_enable_ai(unit_id,true_or_false);

$_____end_____
@doc
$____begin____

作用：
  允许或禁止npc AI，第二个参数必须是"true"或"false"。
  以一定速度移向目标点。

$_____end_____;

@snippets("npc_face")
$____begin____

npc_face(unit_id,dir);

$_____end_____
@doc
$____begin____

作用：
  控制npc朝向（如果有ai控制，这一命令不一定有效果）。

$_____end_____;

@snippets("npc_move_with_waypoints")
$____begin____

npc_move_with_waypoints(unit_id,vector_3_list("v1x v1y v2x v2y")[, event]);

$_____end_____
@doc
$____begin____

作用：
  控制npc沿指定路线移动。

$_____end_____;

@snippets("npc_stop")
$____begin____

npc_stop(unit_id);

$_____end_____
@doc
$____begin____

作用：
  停止正在执行的移动或巡逻命令，进入休闲ai状态。

$_____end_____;

@snippets("npc_set_camp")
$____begin____

npc_set_camp(unit_id,camp_id);

$_____end_____
@doc
$____begin____

作用：
  设置npc阵营id。

$_____end_____;

@snippets("obj_face")
$____begin____

obj_face(objid,dir);

$_____end_____
@doc
$____begin____

作用：
  控制游戏对象朝向。

$_____end_____;

@snippets("obj_move")
$____begin____

obj_move(objid,vector_3(x,y,z)[, event]);

$_____end_____
@doc
$____begin____

作用：
  移动游戏对象到目标点。

$_____end_____;

@snippets("obj_move_with_waypoints")
$____begin____

obj_move_with_waypoints(objid,vector_3_list("v1x v1y v2x v2y")[, event]);

$_____end_____
@doc
$____begin____

作用：
  控制游戏对象沿指定路线移动。

$_____end_____;

@snippets("set_story_skipped")
$____begin____

set_story_skipped(0_or_1);

$_____end_____
@doc
$____begin____

作用：
  设置是否跳过story状态。

$_____end_____;

@snippets("is_story_skipped")
$____begin____

is_story_skipped();

$_____end_____
@doc
$____begin____

作用：
  检查是否跳过story状态。

$_____end_____;

@snippets("send_ai_message")
$____begin____

send_ai_message(objid,msg,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里给某个obj的ai发剧情消息。（注意：参数只支持整数、浮点数与字符串三种类型）

$_____end_____;

@snippets("send_ai_concurrent_message")
$____begin____

send_ai_concurrent_message(objid,msg,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里给某个obj的ai发剧情消息。（注意：参数只支持整数、浮点数与字符串三种类型）

$_____end_____;

@snippets("send_ai_namespaced_message")
$____begin____

send_ai_namespaced_message(objid,msg,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里给某个obj的ai发剧情消息。（注意：参数只支持整数、浮点数与字符串三种类型）

$_____end_____;

@snippets("send_ai_concurrent_namespaced_message")
$____begin____

send_ai_concurrent_namespaced_message(objid,msg,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里给某个obj的ai发剧情消息。（注意：参数只支持整数、浮点数与字符串三种类型）

$_____end_____;

@snippets("publish_event")
$____begin____

publish_event(ev_name,group,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里直接发布逻辑层或渲染层事件，从而触发订阅对应事件的处理。（注意：参数只支持整数、浮点数与字符串三种类型，所以并不是所有事件都能由此命令触发）

$_____end_____;

@snippets("obj_animation")
$____begin____

obj_animation(objid,anim[,normalized_start_time]);

$_____end_____
@doc
$____begin____

作用：
  让指定对象播放指定动作anim。

$_____end_____;

@snippets("npc_animation")
$____begin____

npc_animation(unitid,anim[,normalized_start_time]);

$_____end_____
@doc
$____begin____

作用：
  让指定对象播放指定动作anim。

$_____end_____;

@snippets("send_message")
$____begin____

send_message(name,msg,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里直接给渲染层指定gameobject发消息。（注意：参数只支持整数、浮点数与字符串三种类型）

$_____end_____;

@snippets("send_message_with_tag")
$____begin____

send_message_with_tag(tag,msg,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里直接给渲染层指定gameobject发消息。（注意：参数只支持整数、浮点数与字符串三种类型）

$_____end_____;

@snippets("send_message_with_game_object")
$____begin____

send_message_with_game_object(gameobject or objid,msg,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  在剧情脚本里直接给渲染层指定gameobject发消息。（注意：参数只支持整数、浮点数与字符串三种类型）

$_____end_____;

@snippets("npc_set_ai")
$____begin____

npc_set_ai(unit_id,ai_logic_id,string_list("aiparam0 aiparam1 aiparam2"));

$_____end_____
@doc
$____begin____

作用：
  改变指定npc的ai逻辑并指定对应ai逻辑的ai参数。

$_____end_____;

@snippets("obj_stop")
$____begin____

obj_stop(objid);

$_____end_____
@doc
$____begin____

作用：与对应版本的npcXXX相同，只是第一个参数改成objid，并可用于玩家。

$_____end_____;

@snippets("obj_enable_ai")
$____begin____

obj_enable_ai(objid,true_or_false);

$_____end_____
@doc
$____begin____

作用：与对应版本的npcXXX相同，只是第一个参数改成objid，并可用于玩家。

$_____end_____;

@snippets("obj_set_ai")
$____begin____

obj_set_ai(objid,ailogicid,string_list("p1 p2 p3 p4"));

$_____end_____
@doc
$____begin____

作用：与对应版本的npcXXX相同，只是第一个参数改成objid，并可用于玩家。

$_____end_____;

@snippets("highlight_prompt")
$____begin____

highlight_prompt(msg);

$_____end_____
@doc
$____begin____

作用：
  在屏幕中上方显示醒目提示信息

$_____end_____;

@snippets("destroy_npc_with_obj_id")
$____begin____

destroy_npc_with_obj_id(objid);

$_____end_____
@doc
$____begin____

作用：
  销毁指定objid的npc。

$_____end_____;

@snippets("set_hp")
$____begin____

set_hp(objid,hp);

$_____end_____
@doc
$____begin____

作用：
  设置指定obj的战斗属性value。

$_____end_____;

@snippets("set_level")
$____begin____

set_level(objid,lvl);

$_____end_____
@doc
$____begin____

作用：
  设置指定对象的等级。

$_____end_____;

@snippets("set_attr")
$____begin____

set_attr(objid,attrid,value);

$_____end_____
@doc
$____begin____

作用：
  设置指定对象的指定属性value。

$_____end_____;

@snippets("mark_control_by_story")
$____begin____

mark_control_by_story(objid,true/false);

$_____end_____
@doc
$____begin____

作用：
  标记指定对象是否由story脚本控制。（主要用在战神赛的机器人放技能时）

$_____end_____;

@snippets("wait_story")
$____begin____

wait_story(storyid1,storyid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
  等待运行的各story执行完毕。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)

$_____end_____;

@snippets("wait_all_message")
$____begin____

wait_all_message(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
waitallmessage等待指定的消息之一触发。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)

$_____end_____;

@snippets("wait_all_message_handler")
$____begin____

wait_all_message_handler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
waitallmessagehandler等待运行的各story的指定消息都执行完毕。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)

$_____end_____;

@snippets("set_unit_id")
$____begin____

set_unit_id(objid,unitid);

$_____end_____
@doc
$____begin____

作用：
  修改某个obj的unitid。

$_____end_____;

@snippets("set_player_id")
$____begin____

set_player_id(objid);

$_____end_____
@doc
$____begin____

作用：
  设置玩家对象objid。

$_____end_____;

@snippets("game_object_animation")
$____begin____

game_object_animation(objpath, anim[, normalized_time]);

$_____end_____
@doc
$____begin____

作用：
  让指定unity gameobject播放动作。

$_____end_____;

@snippets("game_object_animation_param")
$____begin____

game_object_animation_param(obj)
{
    float(name,val);
    int(name,val);
    bool(name,val);
    trigger(name,val);
};

$_____end_____
@doc
$____begin____

作用：
  修改unity gameobject的动画控制器参数。

$_____end_____;

@snippets("npc_set_ai_target")
$____begin____

npc_set_ai_target(unit_id,target);

$_____end_____
@doc
$____begin____

作用：
  设置指定npc的AI目标。

$_____end_____;

@snippets("obj_set_ai_target")
$____begin____

obj_set_ai_target(objid,target);

$_____end_____
@doc
$____begin____

作用：
  设置指定npc的AI目标。

$_____end_____;

@snippets("obj_set_camp")
$____begin____

obj_set_camp(objid,camp_id);

$_____end_____
@doc
$____begin____

作用：
  设置npc阵营id。

$_____end_____;

@snippets("set_visible")
$____begin____

set_visible(objid,0_or_1);

$_____end_____
@doc
$____begin____

作用：
  设置npc是否显示。

$_____end_____;

@snippets(load_ui)
$____begin____

load_ui(name, prefab, dsl_file);

$_____end_____
@doc
$____begin____

作用：
  加载dsl实现逻辑的UI。

$_____end_____;

@snippets(bind_ui)
$____begin____

bind_ui(gameobject){
    var("@varname","Panel/Input"); 										//variable bind to control
    inputs("",...);           												//the InputField args to event
    toggles("",...);																	//the Toggle args to event
    sliders("",...);																	//the Slider args to event
    dropdowns("",...);																//the Dropdown args to event
    onevent("button","eventtag","Panel/Button"); 			//on_click event's tag and the control bind to
    onevent("toogle","eventtag","Panel/Toggle"); 			//on_toggle event's tag and the control bind to
    onevent("dropdown","eventtag","Panel/Dropdown"); 	//on_dropdown event's tag and the control bind to
    onevent("slider","eventtag","Panel/Slider"); 			//on_slider event's tag and the control bind to
    onevent("input","eventtag","Panel/Input"); 				//on_input event's tag and the control bind to
};

$_____end_____
@doc
$____begin____

作用：
  与UI prefab绑定。其中inputs、toggles、sliders、dropdowns指定在事件处理里除固定参数外额外传递的参数，参数顺序为inputs、toggles、sliders、dropdowns，但没有指定的不会作为参数传递，此时后面顺序的参数相应前移。
on_click事件的固定参数为tag
on_toggle事件的固定参数为tag、toggle_value(整数value：1--选中 0--未选中)
on_dropdown事件的固定参数为tag、selected_index（整数value）
on_slider事件的固定参数为tag、slider_value（浮点数)
on_input事件的固定参数为tag、input_value（字符串)

$_____end_____;

//---------------------------------------------------------------------------------------------------------
@snippets("pause_story")
$____begin____

pause_story(story_id,story_id,...);

$_____end_____
@doc
$____begin____

作用：
  暂停指定的story。

$_____end_____;

@snippets("resume_story")
$____begin____

resume_story(story_id,story_id,...);

$_____end_____
@doc
$____begin____

作用：
  恢复指定的story。

$_____end_____;

@snippets("pauseallmessagehandler")
$____begin____

pauseallmessagehandler(msgid,msgid,...);

$_____end_____
@doc
$____begin____

作用：
  暂停当前所有story里的指定消息处理

$_____end_____;

@snippets("resume_all_message_handler")
$____begin____

resume_all_message_handler(msgid,msgid,...);

$_____end_____
@doc
$____begin____

作用：
  恢复当前所有story里的指定消息处理

$_____end_____;

@snippets("obj_animation_param")
$____begin____

obj_animation_param(obj_id)
{
    float(name,val);
    int(name,val);
    bool(name,val);
    trigger(name,val);
};

$_____end_____
@doc
$____begin____

作用：
  修改指定对象的动画控制器的参数或触发器。

$_____end_____;

@snippets("npc_animation_param")
$____begin____

npc_animation_param(unit_id)
{
    float(name,val);
    int(name,val);
    bool(name,val);
    trigger(name,val);
};

$_____end_____
@doc
$____begin____

作用：
  修改指定对象的动画控制器的参数或触发器。

$_____end_____;

@doc
$____begin____

九、渲染层函数

$_____end_____;

@snippets("blackboard_get")
$____begin____

blackboard_get(name);

$_____end_____
@doc
$____begin____

作用：
  获取渲染层指定名譾alue幕捍媸荨?

$_____end_____;

@snippets("get_time")
$____begin____

get_time();

$_____end_____
@doc
$____begin____

作用：
  获取渲染层时间。

$_____end_____;

@snippets("get_time_scale")
$____begin____

get_time_scale();

$_____end_____
@doc
$____begin____

作用：
  获取渲染层时间倍率。

$_____end_____;

@snippets("is_active")
$____begin____

is_active(game_object_path);

$_____end_____
@doc
$____begin____

作用：
  判断指定GameObject是否active，isactive判断对象自身，isreallyactive检查对象所在层次路径是否都active。（主要用于判断窗口是否可见）

$_____end_____;

@snippets("is_really_active")
$____begin____

is_really_active(game_object_path);

$_____end_____
@doc
$____begin____

作用：
  判断指定GameObject是否active，isactive判断对象自身，isreallyactive检查对象所在层次路径是否都active。（主要用于判断窗口是否可见）

$_____end_____;

@snippets("get_component")
$____begin____

get_component(game_object_path，component_type_name/type);

$_____end_____
@doc
$____begin____

作用：
  获取指定gameobject上的指定component实例。

$_____end_____;

@snippets("get_game_object")
$____begin____

get_game_object(game_object_path);

$_____end_____
@doc
$____begin____

作用：
  获取gameobject对象实例。

$_____end_____;

@snippets("get_parent")
$____begin____

get_parent(game_object_path);

$_____end_____
@doc
$____begin____

作用：
  获取gameobject父对象实例。

$_____end_____;

@snippets("get_child")
$____begin____

get_child(game_object_path, child_path);

$_____end_____
@doc
$____begin____

作用：
  获取gameobject子对象实例。

$_____end_____;

@snippets("namespace")
$____begin____

namespace();

$_____end_____
@doc
$____begin____

作用：
  获取当前脚本的namespace（用于UI脚本，一般UI脚本的namespace为UI名，由加载UI时传入）。

$_____end_____;

@doc
$____begin____

十、渲染染层命令

$_____end_____;

@snippets("blackboard_clear")
$____begin____

blackboard_clear();

$_____end_____
@doc
$____begin____

作用：
  清空渲染层数据缓存。

$_____end_____;

@snippets("blackboard_set")
$____begin____

blackboard_set(name,value);

$_____end_____
@doc
$____begin____

作用：
  添加/修改渲染层数据缓存。

$_____end_____;

@snippets("create_game_object")
$____begin____

create_game_object(name, prefab[, parent])[obj("varname")]{
    position(vector_3(x,y,z));
    rotation(vector_3(x,y,z));
    scale(vector_3(x,y,z));
};

$_____end_____
@doc
$____begin____

作用：
  创建一个GameObject，可选的挂在指定父节点上。

$_____end_____;

@snippets("set_transform")
$____begin____

set_transform(name, world_or_local){
    position(vector_3(x,y,z));
    rotation(vector_3(x,y,z));
    scale(vector_3(x,y,z));
};

$_____end_____
@doc
$____begin____

作用：
  修改指定对象的transform。

$_____end_____;

@snippets("destroy_game_object")
$____begin____

destroy_game_object(path);

$_____end_____
@doc
$____begin____

作用：
  销毁gameobject。

$_____end_____;

@snippets("auto_recycle")
$____begin____

auto_recycle(path, 1_or_0);

$_____end_____
@doc
$____begin____

作用：
  设置是否在场景重置时自动销毁gameobject。

$_____end_____;

@snippets("followtarget")
$____begin____

followtarget(objid,parent);

$_____end_____
@doc
$____begin____

作用：
  让obj跟随指定目标，parent为空串或0则解除跟随。

$_____end_____;

@snippets("set_parent")
$____begin____

set_parent(objpath,parent,stay_world_pos);

$_____end_____
@doc
$____begin____

作用：
  销毁gameobject。

$_____end_____;

@snippets("set_active")
$____begin____

set_active(objpath,1_or_0);

$_____end_____
@doc
$____begin____

作用：
  改变gameobject的active状态。

$_____end_____;

@snippets("add_component")
$____begin____

add_component(objpath, component_name/component_type)[obj("@varname")];

$_____end_____
@doc
$____begin____

作用：
  添加component到obj.可选部分指定一个对象名来存储component对象实例。

$_____end_____;

@snippets("remove_component")
$____begin____

remove_component(objpath,type);

$_____end_____
@doc
$____begin____

作用：
  删除指定组件。

$_____end_____;

@snippets("open_url")
$____begin____

open_url(url);

$_____end_____
@doc
$____begin____

作用：
  用浏览器打开指定url。

$_____end_____;

@snippets("quit")
$____begin____

quit();

$_____end_____
@doc
$____begin____

作用：
  退出游戏。

$_____end_____;

@snippets("local_namespaced_message")
$____begin____

local_namespaced_message(msgId,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  发送本名字空间内的消息，对应的消息处理由下述语法提供
  onnamespacedmessage(msgId)
  {
      //消息处理
  };
  因为名字空间无法由dsl本身指定，所以单独提供了一套消息/处理机制，这套机制其实是对原来的消息机制的msgId添加了一个namespace前缀，如果知道dsl对应的名字空间，假设为ns，则下述语法与上面的处理等价
  onmessage(ns,msgId)
  {
      //消息处理
  };
  类似的，local_message(ns+":"+msgId,arg1,arg2,...);与发送消息的语法等价，后面类似的基于namespace的方法是这样与对应的普通方法等价，不再一一缀述。

$_____end_____;

@snippets("story_local_namespaced_message")
$____begin____

story_local_namespaced_message(msgId,arg1,arg2,...);

$_____end_____
@doc
$____begin____

作用：
  发送本名字空间内的消息（如果取消按钮被按下，则不再发送消息），对应的消息处理由下述语法提供
  onnamespacedmessage(msgId)
  {
      //消息处理
  };
  因为名字空间无法由dsl本身指定，所以单独提供了一套消息/处理机制，这套机制其实是对原来的消息机制的msgId添加了一个namespace前缀，如果知道dsl对应的名字空间，假设为ns，则下述语法与上面的处理等价
  onmessage(ns,msgId)
  {
      //消息处理
  };
  类似的，story_local_message(ns+":"+msgId,arg1,arg2,...);与发送消息的语法等价，后面类似的基于namespace的方法是这样与对应的普通方法等价，不再一一缀述。

$_____end_____;

@snippets("clear_namespaced_message")
$____begin____

clear_namespaced_message(msgid1,msgid2,...);

$_____end_____
@doc
$____begin____

作用：
  清空指定的本地名字空间内的消息。

$_____end_____;

@snippets("wait_local_namespaced_message")
$____begin____

wait_local_namespaced_message(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
  waitlocalnamespacedmessage等待指定的本地名字空间内的消息之一触发。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)

$_____end_____;

@snippets("wait_local_namespaced_message_handler")
$____begin____

wait_local_namespaced_message_handler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
  waitlocalnamespacedmessagehandler等待指定的本地名字空间内的消息处理全部执行完毕。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)

$_____end_____;

@snippets("story_wait_local_namespaced_message")
$____begin____

story_wait_local_namespaced_message(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
  storywaitlocalnamespacedmessage等待指定的本地名字空间内的消息之一触发。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)。如果剧情取消按钮被按下，则不再等待按超时处理

$_____end_____;

@snippets("story_wait_local_namespaced_message_handler")
$____begin____

story_wait_local_namespaced_message_handler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)];

$_____end_____
@doc
$____begin____

作用：
  storywaitlocalnamespacedmessagehandler等待指定的本地名字空间内的消息处理全部执行完毕。(var部分用字符串指定变量名，这样允许一个变量或表达式的value作为变量)。如果剧情取消按钮被按下，则不再等待按超时处理

$_____end_____;

@doc
$____begin____

十一、脚本支持消息

$_____end_____;

@snippets("send_message DimScreen")
$____begin____

send_message("ZhanChangLogic/Camera", "DimScreen", time);

$_____end_____
@doc
$____begin____

作用：
  淡出屏幕（变暗），time为毫秒，在time时间内变暗

$_____end_____;

@snippets("send_message LightScreen")
$____begin____

send_message("ZhanChangLogic/Camera", "LightScreen", time);

$_____end_____
@doc
$____begin____

作用：
  淡入屏幕（变亮），time为毫秒，在time时间内变亮

$_____end_____;

@snippets("send_message LightTo")
$____begin____

send_message("ZhanChangLogic/Camera", "LightTo", time, red, green, blue, alpha);

$_____end_____
@doc
$____begin____

作用：
  变亮到指定颜色，time为毫秒，在time时间内变亮, r,g,b,a的范围为0~2，超过1表示过曝光。

$_____end_____;

@snippets("send_message EnableObstacle")
$____begin____

send_message("ZhanChangLogic/DynamicObstacle","EnableObstacle",1_or_0);

$_____end_____
@doc
$____begin____

作用：
  开关动态阻挡。

$_____end_____;

@snippets("send_message SetLightVisible")
$____begin____

send_message("ZhanChangLogic/Light","SetLightVisible",1_or_0);

$_____end_____
@doc
$____begin____

作用：
  开关点光。

$_____end_____;

@snippets("send_message SetVisible")
$____begin____

send_message("ZhanChangLogic/Object","SetVisible",1_or_0);

$_____end_____
@doc
$____begin____

作用：
  显示/隐藏对象。

$_____end_____;

@snippets("send_message PlayAnimation")
$____begin____

send_message("ZhanChangLogic/Object","PlayAnimation", anim, speed);

$_____end_____
@doc
$____begin____

作用：
  播放动画。

$_____end_____;

@snippets("send_message PlayParticle")
$____begin____

send_message("ZhanChangLogic/Object","PlayParticle", 1_or_0);

$_____end_____
@doc
$____begin____

作用：
  播放特效。

$_____end_____;

@snippets("send_message PlaySound")
$____begin____

send_message("ZhanChangLogic/Object","PlaySound", index);

$_____end_____
@doc
$____begin____

作用：
  播放声音，使用unity3d的AudioSource。

$_____end_____;

@snippets("send_message StopSound")
$____begin____

send_message("ZhanChangLogic/Object","StopSound", index);

$_____end_____
@doc
$____begin____

作用：
  停止播放声音，使用unity3d的AudioSource。

$_____end_____;
