@@delimiter(script, $____begin____, $_____end_____);

@doc
$____begin____

{
	
$_____end_____;  

@include("snippets.json");

@doc
$____begin____

  //------------------------------------story部分----------------------------------------
	"story": {
		"prefix": "story",
		"body": [
			"story(main)",
			"{",
			"\tlocal",
			"\t{",
			"\t};",
			"\tonmessage(\"start\")",
			"\t{",
			"\t};",
			"};",
		],
		"description": "define story"
	},
	"local": {
		"prefix": "local",
		"body": [
			"local",
			"{",
			"};",
		],
		"description": "define story local variables"
	},	
	"onmessage": {
		"prefix": "onmessage",
		"body": [
			"onmessage(\"[msgid]\")[args($a,$b,...)]",
			"{",
			"};",
		],
		"description": "define story onmessage"
	},
	"onnamespacedmessage": {
		"prefix": "onnamespacedmessage",
		"body": [
			"onnamespacedmessage(\"[msgid]\")[args($a,$b,...)]",
			"{",
			"};",
		],
		"description": "define story onnamespacedmessage"
	},
	//-------------------------------------------------------------------------------------

$_____end_____;

@include("story.json");

@doc
$____begin____


}

$_____end_____;