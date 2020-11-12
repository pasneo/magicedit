

/* Possible syntaxes:

[ClassName]
    [Ability] '+=' [unsigned integer]
    [Ability] '-=' [unsigned integer]
    'set' [Attribute]
    'forbid' [Attribute]
    'add' [Item]
	'teach' [Spell]
...

Example:

Dwarf
    STR += 2
    SPC -= 1
    set can_use_hammer
    forbid can_climb_mountains
    add Hammer_of_Tek
	teach Frost

Elf
    STR -= 1
    INT += 1
    
*/

grammar classlist_lang;

doc : classListExpr* ;
classListExpr : classListName classListShownName? LEFT_BRACE classListBody RIGHT_BRACE ;

classListBody : classExpr* ;

classExpr : className classShownName? LEFT_BRACE classBody RIGHT_BRACE ;

classBody : classLine* ;
classLine : abilityLine | attributeLine | itemLine | spellLine ;

abilityLine : ability abilityModifier value ;
attributeLine : attributeOption attribute ;
itemLine : ADD item_number? item ;
spellLine : TEACH spell ;

classListName : STR ;
className : STR ;

classListShownName : string_const;
classShownName : string_const ;

ability : STR ;
abilityModifier : PLUS | MINUS ;
attribute : STR ;
attributeOption : (SET | FORBID) ;
value : VALUE ;
item : STR ;
item_number : VALUE ;
spell : STR ;

string_const : STRING_CONST ;

SET : 'set' ;
FORBID : 'forbid' ;
ADD : 'add' ;
TEACH : 'teach' ;
PLUS : '+' ;
MINUS : '-' ;

VALUE : '0' | [1-9]([0-9])* ;
STR : ([a-zA-Z] | '_')([a-zA-Z0-9] | '_')* ;

STRING_CONST : '$'STR ;

WS : (' ' | '\r' | '\n' | '\t') -> skip ;

LEFT_BRACE : '{' ;
RIGHT_BRACE : '}' ;