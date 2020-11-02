grammar scheme_lang;

/*
 * Parser Rules
 */

doc : scheme ;

scheme : SCHEME scheme_name parents? L_BRACE scheme_body R_BRACE ;
    scheme_name : identifier ;
    parents : COLON parent_name+ ;
	parent_name : identifier ;
    
scheme_body : scheme_body_element* ;

scheme_body_element :   body_variable_definition |
                        parameter_definition |
                        init_block |
                        action_block ;

body_variable_definition : variable_definition ;

variable_definition : variable_type variable_name (EQUALS expression)? ;
    variable_type : identifier ;
    variable_name : identifier;

parameter_definition : PARAM variable_type variable_name ;

init_block : INIT L_BRACE function_body end_of_init ;
	end_of_init : R_BRACE ;

action_block : ACTION action_name L_PAREN action_point R_PAREN L_BRACE function_body end_of_action ;
	end_of_action : R_BRACE ;
    action_point : integer ;
    action_name : identifier ;

function_body : command* ;

/* Commands */

command : cmd_report |
          cmd_desc |
          cmd_manage_actions |
          cmd_create_var |
          cmd_set_var |         // a = b
		  cmd_set_of |
          //cmd_modify_var |      // for += -= *= /=
          cmd_if |
          cmd_fail |
		  cmd_end |
          cmd_toggle |
          cmd_set_attr |
          cmd_add_item |
          cmd_remove_item |
          cmd_teach_spell |
          cmd_remove_spell ;
          
cmd_report : REPORT content ;
    content : string_const ;
    
cmd_desc : DESC content ;

cmd_manage_actions : ACTIONS set_action+ ;
    set_action : clear_actions | add_action | remove_action ;
    clear_actions : CLEAR ;
    add_action : PLUS action_name ;
    remove_action : MINUS action_name ;
    
cmd_create_var : variable_definition ;
    
cmd_set_var : variable_name EQUALS expression ;

cmd_set_of : property_name OF object EQUALS expression ;

//cmd_modify_var : modif_variable modif_operator numeric_expression ;
	//modif_variable
	//	: variable_name
	//	| property_name OF object ;
    //modif_operator : MOD_ADD | MOD_SUB | MOD_MUL | MOD_DIV ;
    
cmd_if : IF logical_expression
            function_body
         else?
		 endif ;
    
	else : (ELSE function_body) ;
	endif : ENDIF ;

cmd_fail : FAIL ;

cmd_end : END ;

cmd_toggle : TOGGLE object_name ;
    object : object_atom | property_of ;
	object_atom : object_name ;
    object_name : identifier ;
    property_of : property_name OF object ;

property_name : identifier ;

cmd_set_attr : attr_type attr_name OF object ;
    attr_type : SET|FORBID|REMOVE ;
    attr_name : identifier ;

cmd_add_item : ADD item_number? item_name TO character_name ;
	item_number : numeric_expression ;
    item_name : identifier ;
    character_name : identifier ;
    
cmd_remove_item : REMOVE item_number? item_name FROM character_name ;

cmd_teach_spell : TEACH spell_name TO character_name;
    spell_name : identifier ;

//cmd_remove_spell : REMOVE spell_name FROM character_name ;
cmd_remove_spell : MAKE character_name FORGET spell_name ;

/* Expressions */

/*
* An expression can be simple (string const) or
* complex (numeric or logical expressions). This
* division is necessary because we don't allow
* string consts to be in a complex expression.
*/

expression
    : string_const_expr
    | numeric_expression
    | logical_expression
    ;

/* String const expression */

string_const_expr : string_const ;

/* Numeric expressions */

numeric_expression : multiplying_expr | complex_numeric_expr;
	complex_numeric_expr : multiplying_expr (PLUS|MINUS) numeric_expression ;

multiplying_expr : atom | complex_multiplying_expr ;
    complex_multiplying_expr : atom (MUL|DIV) multiplying_expr ;

atom
    : inverted_atom
    | numeric_value
    | L_PAREN numeric_expression R_PAREN
    ;

inverted_atom : MINUS atom ;

//numeric_value : változó ('of' is), szám
numeric_value
    : variable_expr
    | integer_expr
    ;

integer_expr : integer ;

variable_expr
    : variable_name
    | property_of
    ;

/* Logical expressions */

logical_expression : and_expression | complex_logical_expr ;
	complex_logical_expr : and_expression OR logical_expression ;

and_expression : logical_atom | complex_and_expr ;
	complex_and_expr : logical_atom AND and_expression ;

logical_atom
    : inverted_logical_atom
    | logical_value
    | comparison
    | has_item
    | knows_spell
    | is
    | L_PAREN logical_expression R_PAREN
    ;

inverted_logical_atom : NOT logical_atom ;

logical_variable : identifier ;

logical_value
	: logical_const_expr
	| variable_expr
	;

logical_const_expr : logical_const ;

comparison : numeric_expression relational_operator numeric_expression ;

relational_operator
    : EQUALS
    | GREATER
    | LOWER
    | GREATER_EQUALS
    | LOWER_EQUALS
    ;

has_item : character_name HAS NOT? item_number? item_name ;
    
knows_spell : character_name KNOWS NOT? spell_name ;

is : object IS identifier ;



/* Basic rules */

logical_const : TRUE | FALSE ;
string_const : STRING_CONST ;
identifier : STR ;
integer : INT ;

/*
 * Lexer Rules
 */

ACTION : 'action' ;
ACTIONS : 'actions' ;
ADD : 'add' ;
AND : 'and' ;
CLASS : 'class' ;
CLEAR : 'clear' ;
DESC: 'desc' ;
ELSE : 'else' ;
END : 'end' ;
ENDIF : 'endif' ;
FAIL : 'fail' ;
FORBID : 'forbid' ;
FORGET : 'forget' ;
FROM : 'from' ;
HAS : 'has' ;
IF : 'if' ;
INIT : 'init' ;
IS : 'is' ;
KNOWS : 'knows' ;
MAKE : 'make' ;
NOT : 'not' ;
OF : 'of' ;
OR : 'or' ;
PARAM : 'param' ;
REMOVE : 'remove' ;
REPORT : 'report' ;
SCHEME : 'scheme' ;
SET : 'set' ;
TEACH : 'teach' ;
TO : 'to' ;
TOGGLE : 'toggle' ;

PLUS: '+' ; MINUS : '-' ; MUL : '*' ; DIV : '/' ;
//MOD_ADD : '+=' ; MOD_SUB : '-=' ; MOD_MUL : '*=' ; MOD_DIV : '/=' ;
EQUALS : '=' ;
GREATER : '>' ;
LOWER : '<' ;
GREATER_EQUALS : '>=' ;
LOWER_EQUALS : '<=' ;

TRUE : 'true' | 'True' | 'TRUE' ;
FALSE : 'false' | 'False' | 'FALSE' ;
STRING_CONST : '$'STR ;
STR : [a-zA-Z][a-zA-Z0-9_]* ;
INT : '0'|([1-9][0-9]*) ;

COLON : ':' ;
L_BRACE : '{' ; R_BRACE : '}' ;
L_PAREN : '(' ; R_PAREN : ')' ;

WS
	:	(' '|'\n'|'\t'|'\r') -> channel(HIDDEN)
	;

