

/* Possible syntaxes:

[ClassName]
    [Ability] '+=' [unsigned integer]
    [Ability] '-=' [unsigned integer]
    'set' [Attribute]
    'forbid' [Attribute]
    'add' [Item]
...

Example:

Dwarf
    STR += 2
    SPC -= 1
    set can_use_hammer
    forbid can_climb_mountains
    add Hammer_of_Tek

Elf
    STR -= 1
    INT += 1
    
*/

grammar classlist_lang;

doc : classListExpr* ;
classListExpr : classListName LEFT_BRACE classListBody RIGHT_BRACE ;

classListBody : classExpr* ;

classExpr : className LEFT_BRACE classBody RIGHT_BRACE ;

classBody : classLine* ;
classLine : abilityLine | attributeLine | itemLine ;

abilityLine : ability abilityModifier value ;
attributeLine : attributeOption attribute ;
itemLine : ADD item_number? item ;

classListName : STR ;
className : STR ;

ability : STR ;
abilityModifier : PLUS | MINUS ;
attribute : STR ;
attributeOption : (SET | FORBID) ;
value : VALUE ;
item : STR ;
item_number : VALUE ;

SET : 'set' ;
FORBID : 'forbid' ;
ADD : 'add' ;
PLUS : '+' ;
MINUS : '-' ;

VALUE : '0' | [1-9]([0-9])* ;
STR : ([a-zA-Z] | '_')([a-zA-Z0-9] | '_')* ;
WS : (' ' | '\r' | '\n' | '\t') -> skip ;

LEFT_BRACE : '{' ;
RIGHT_BRACE : '}' ;