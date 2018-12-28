@@delimiter(script, $____begin____, $_____end_____);

@doc
$____begin____

/L18"Dsl" DSL_LANG EnableMLS Line Comment = // Line Comment Alt = # Block Comment On = /* Block Comment Off = */ Escape Char = \ String Chars = '" File Extensions = DSL SCP GM
/Delimiters = ~!%^&*()-+=|\/{}[]:;"'<> ,	.?
/Regexp Type = Perl

/Indent Strings = "{"
/Unindent Strings = "}"
/Open Brace Strings =  "{" "(" "["
/Close Brace Strings = "}" ")" "]"
/Open Fold Strings = "{"
/Close Fold Strings = "}"

/C1"Operators" STYLE_OPERATOR
+
-
*
// /
%
>
>=
==
!=
<
<=
&&
||
!
=
.
(
)
,
{
}

/C2"Variables"
** @a @b @c @d @e @f @g @h @i @j @k @l @m @n @o @p @q @r @s @t @u @v @w @x @y @z

/C3"Globals"
** @@a @@b @@c @@d @@e @@f @@g @@h @@i @@j @@k @@l @@m @@n @@o @@p @@q @@r @@s @@t @@u @@v @@w @@x @@y @@z

/C4"Skip"

/C5"Arguments"
** $

/C6"Skip"


$_____end_____;

@include("uew.txt");