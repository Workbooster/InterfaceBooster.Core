grammar Synery;

@parser::members
{
	protected const int EOF = Eof;
}

@lexer::members
{
	protected const int EOF = Eof;
	protected const int HIDDEN = Hidden;
}

/*************************************** 
	PARSER 
***************************************/

program
	:	(programUnit | syneryFunctionBlock | recordTypeDeclaration)*
		EOF
	;

programUnit
	:	variableStatement ';'
	|	libraryPluginVariableStatement ';'
	|	tableStatement ';'
	|	tableAddStatement ';'
	|	tableDropStatement ';'
	|	logStatement ';'
	|	functionCall ';'
	|	providerPluginStatement
	|	ifStatement
	|	observeBlock
	|	eachStatement
	;

block
	:	blockUnit*
	;

blockUnit
	:	programUnit 
	|	returnStatement ';'
	|	emitStatement ';'
	|	throwStatement ';'
	;

/*** DECLARATIONS & ASSIGNMENTS ***/

// VARIABLE

variableStatement
	:	variableDeclartion
	|	variableAssignment
	;

variableAssignment
    :   (variableReference | complexReference) ('=' variableInitializer)?
    ;

variableDeclartion
	:	type variableAssignment (',' variableAssignment)*
	;

variableInitializer
	:	expression
	;

// TABLE

tableStatement
	:	tableAssignment
	;

tableAssignment
	:	InternalPathIdentifier ('=' tableInitializer)?
	;

tableInitializer
	:	requestStatement
	;

tableAddStatement
	:	ADD expressionList TO tableIdentifier
	;

tableDropStatement
	:	DROP tableIdentifier
	;

// LOG

logStatement
	:	LOG '(' expression (',' StringLiteral )? ')'
	;

// PARAMETER

parameterDeclartions
	:	parameterDeclartion (',' parameterDeclartion)*
	;

parameterDeclartion
	:	type Identifier ('=' parameterInitializer)?
	;

parameterInitializer
	:	literal
	;

// KEY-VALUE LIST

keyValueList
	:	keyValueAssignment (',' keyValueAssignment)*
	;

keyValueAssignment
	:	keyValueAssignmentIdentifier '=' keyValueInitializer
	;

keyValueAssignmentIdentifier
	:	(variableReference | complexReference)
	;

keyValueInitializer
	:	expression
	;

// INTERNAL IDENTIFIER LIST

internalIdentifierList
	:	internalIdentifierListItem (',' internalIdentifierListItem)*
	;

internalIdentifierListItem
	:	Identifier
	;

/*** FUNCTION ***/

syneryFunctionBlock
	:	type? Identifier '(' parameterDeclartions? ')' 
			block 
		END
	;

functionCall
	:	syneryFunctionCall
	|	libraryPluginFunctionCall
	;

syneryFunctionCall
	:	(syneryFunctionInternalIdentifier | syneryFunctionExternalIdentifier) 
		'(' expressionList? ')'
	;

syneryFunctionInternalIdentifier
	:	Identifier
	;

syneryFunctionExternalIdentifier
	:	ComplexIdentifier
	;

libraryPluginFunctionCall
	:	libraryPluginFunctionCallIdentifier '(' expressionList? ')'
	;

libraryPluginFunctionCallIdentifier
	:	ExternalLibraryIdentifier	
	;

/*** RECORD ***/

recordType
	:	RecordTypeIdentifier
	|	ComplexRecordTypeIdentifier
	|	SystemRecordTypeIdentifier
	;

recordTypeDeclaration
	:	RecordTypeIdentifier '(' parameterDeclartions? ')' (':' recordTypeDeclarationBaseType)? ';'
	;

recordTypeDeclarationBaseType
	:	recordType
	;

recordInitializer
	:	recordType '(' (expressionList | keyValueList )? ')'
	;

/*** LIBRARY PLUGIN VARIABLE ***/

libraryPluginVariableStatement
	:	libraryPluginVariableAssignment
	;

libraryPluginVariableAssignment
    :   ExternalLibraryIdentifier '=' variableInitializer
    ;

libraryPluginVariableReference
	:	ExternalLibraryIdentifier
	;

/*** RETURN STATEMENT ***/

returnStatement
	:	RETURN expression?
	;


/*** IF-ELSEIF-ELSE BLOCK ***/

ifStatement
	:	IF expression
			block
		(elseStatement)*
		END
	;

elseStatement
	:	ELSE (IF expression)?
			block
	;

/*** OBSERVE/HANDLE BLOCK ***/

observeBlock
	:	OBSERVE
			block
		(handleBlock)*
		END
	;

handleBlock
	:	HANDLE '(' recordType Identifier ')'
			block
	;

/*** EVENT / EXCEPTION BROADCASTING ***/

emitStatement
	:	EMIT expression
	;

throwStatement
	:	THROW expression
	;

/*** EACH LOOP ***/

eachStatement
	:	EACH recordType Identifier IN tableIdentifier
			block
		END
	;

/*** PROVIDER PLUGIN ***/

providerPluginStatement
	:	providerPluginConnectStatement
	|	providerPluginDataExchangeStatement
	;

// CONNECT

providerPluginConnectStatement
	:	CONNECT providerPluginConnectStatementProviderPluginInstance
			AS ExternalPathIdentifier
			setCommand?
		END
	;

providerPluginConnectStatementProviderPluginInstance
	:	StringLiteral
	;

// DATA EXCHANGE

providerPluginDataExchangeStatement
	:	providerPluginReadStatement 
	|	providerPluginCreateStatement 
	|	providerPluginUpdateStatement 
	|	providerPluginSaveStatement 
	|	providerPluginDeleteStatement 
	|	providerPluginExecuteStatement
	;

// READ

providerPluginReadStatement
	:	READ providerPluginResourceIdentifier 
		toCommand?
		setCommand?
		fieldsCommand?
		filterCommand?
		providerPluginDataExchangeStatement*
		END
	;

// CREATE

providerPluginCreateStatement
	:	CREATE providerPluginResourceIdentifier 
		fromCommand?
		setCommand?
		providerPluginDataExchangeStatement*
		END
	;

// UPDATE

providerPluginUpdateStatement
	:	UPDATE providerPluginResourceIdentifier 
		fromCommand?
		setCommand?
		filterCommand?
		providerPluginDataExchangeStatement*
		END
	;

// SAVE

providerPluginSaveStatement
	:	SAVE providerPluginResourceIdentifier 
		fromCommand?
		setCommand?
		providerPluginDataExchangeStatement*
		END
	;

// DELETE

providerPluginDeleteStatement
	:	DELETE providerPluginResourceIdentifier 
		fromCommand?
		setCommand?
		filterCommand?
		providerPluginDataExchangeStatement*
		END
	;

// EXECUTE

providerPluginExecuteStatement
	:	EXECUTE providerPluginResourceIdentifier 
		setCommand?
		getCommand?
		providerPluginDataExchangeStatement*
		END
	;


// GENERAL DATA STATEMENT RULES

providerPluginResourceIdentifier
	:	ExternalPathIdentifier
	|	Identifier
	;

tableIdentifier
	:	InternalPathIdentifier
	;

fromCommand
	:	FROM tableIdentifier
	;

toCommand
	:	TO tableIdentifier
	;

setCommand
	:	SET '(' keyValueList ')'
	;

getCommand
	:	GET '(' asAssignmentList ')'
	;

asAssignmentList
	:	asAssignmentListItem ( ',' asAssignmentListItem )*
	;

asAssignmentListItem
	:	asAssignmentListItemExternalField AS asAssignmentListItemInternalVariable
	;

asAssignmentListItemExternalField
	:	Identifier
	;

asAssignmentListItemInternalVariable
	:	Identifier
	;

fieldsCommand
	:	FIELDS '(' internalIdentifierList ')'
	;

filterCommand
	:	FILTER '(' filterCommandItem ')'
	;

// Pattern:
// [Field] [Operator] [Expression]
// Example:
// Size >= 168

filterCommandItem
	:	'(' filterCommandItem ')'
	|	filterCommandItem (AND | OR) filterCommandItem
	|	filterCommandConditionItem
	;

filterCommandConditionItem
	:	filterCommandListItemField (GT | LT | GE | LE | EQUAL | NOTEQUAL | EXACTLYEQUAL | EXACTLYNOTEQUAL) filterCommandListItemExpression
	;

filterCommandListItemField
	:	Identifier
	;

filterCommandListItemExpression
	:	singleValue
	;


/*** QUERY LANGUAGE ***/

// REQUEST

// the first command must be a "FROM"-command
requestStatement
	:	requestFromCommand requestCommand*
	;

requestCommand
	:	requestDistinctCommand
	|	requestJoinCommand
	|	requestLeftJoinCommand
	|	requestOrderByCommand
	|	requestWhereCommand
	|	requestSelectCommand
	;

requestFromCommand
	:	FROM InternalPathIdentifier (AS Identifier)?
	;

requestDistinctCommand
	:	DISTINCT
	;

requestJoinCommand
	:	JOIN InternalPathIdentifier (AS requestJoinPrefix)? COMPARE  requestJoinInnerKeySelector TO requestJoinOuterKeySelector
	;

requestLeftJoinCommand
	:	LEFT JOIN InternalPathIdentifier (AS requestJoinPrefix)? COMPARE  requestJoinInnerKeySelector TO requestJoinOuterKeySelector
	;

requestJoinPrefix
	:	Identifier
	;

requestJoinInnerKeySelector
	:	requestExpressionList
	;

requestJoinOuterKeySelector
	:	requestExpressionList
	;

requestOrderByCommand
	:	ORDER BY requestFieldReference (',' requestFieldReference)* (DESC)?
	;

requestWhereCommand
	:	WHERE requestExpression
	;

requestSelectCommand
	:	SELECT requestSelectItem (',' requestSelectItem)*
	;

requestSelectItem
	:	requestSelectSingle
	|	requestSelectMany
	;

requestSelectSingle
	:	requestFieldReference
	|	requestSelectFieldAssignment
	;

requestSelectMany
	:	(Identifier '.')? '*'
	;

requestSelectFieldAssignment
	:	Identifier '=' requestExpression
	;

requestExpressionList
	:	requestExpression (',' requestExpression)*
	;

requestExpression
	:	requestPrimary
	|	requestCastExpression
	|	requestExpression (PERCENT) requestExpression
	|	requestExpression (STAR | SLASH) requestExpression
    |   requestExpression (PLUS | MINUS) requestExpression
	|	requestExpression (GT | LT | GE | LE) requestExpression
    |   requestExpression (EQUAL | NOTEQUAL) requestExpression
	|   requestExpression (AND) requestExpression
	|   requestExpression (OR) requestExpression
	|	requestExpression QUESTION requestExpression COLON requestExpression
	;

requestCastExpression
	:	'(' primitiveType ')' requestExpression
	;

requestPrimary
	:	'(' requestExpression ')'
	|	requestSingleValue
	;

requestSingleValue
	:	literal
	|	requestFieldReference
	|	libraryPluginVariableReference
	|	requestFunctionCall
	;

requestFieldReference
	:	complexReference
	|	variableReference
	;

requestFunctionCall
	:	requestSyneryFunctionCall
	|	requestLibraryPluginFunctionCall
	;

requestSyneryFunctionCall
	:	(syneryFunctionInternalIdentifier | syneryFunctionExternalIdentifier) 
		'(' requestExpressionList? ')'
	;

requestLibraryPluginFunctionCall
	:	libraryPluginFunctionCallIdentifier '(' requestExpressionList? ')'
	;


/*** EXPRESSIONS ***/

expressionList
    :   expression (',' expression)*
    ;

expression
	:	primary
	|	castExpression
	|	expression (PERCENT) expression
	|	expression (STAR | SLASH) expression
    |   expression (PLUS | MINUS) expression
	|	expression (GT | LT | GE | LE) expression
    |   expression (EQUAL | NOTEQUAL) expression
	|   expression (AND) expression
	|   expression (OR) expression
	|	expression QUESTION expression COLON expression
	;

castExpression
	:	'(' primitiveType ')' expression
	;

primary
	:	'(' expression ')'
	|	singleValue
	|	recordInitializer
	;

singleValue
	:	literal
	|	variableReference
	|	complexReference
	|	libraryPluginVariableReference
	|	functionCall
	;

literal
    :   IntegerLiteral
    |   DecimalLiteral
	|	DoubleLiteral
    |   CharLiteral
    |   StringLiteral
	|	VerbatimStringLiteral
    |   BooleanLiteral
	|	dateTimeLiteral
    |   NullLiteral
    ;

// DateTime Literals
// Pattern for the 15th March, 2014 at 20:00 o'clock:
//		$DATETIME(2014,3,17,20,0,0)
//		$DATETIME('17.03.2014 20:00:00', 'de-ch')

dateTimeLiteral
	:	'DATETIME' '('
		expression (',' expression)*
		')'
	;

variableReference
	:	Identifier
	;

complexReference
	:	ComplexIdentifier
	;

/*** TYPES ***/

type
	:	primitiveType
	|	recordType
	;

primitiveType
    :   STRING
    |   BOOL
    |   INT
    |   DECIMAL
    |   DOUBLE
    |   CHAR
    |   DATETIME
    ;

/*************************************** 
	LEXER 
***************************************/

// String Literals

StringLiteral
    :   '"' (EscapeSequence | StringCharacter)* '"'
    ;

fragment
StringCharacter
    :   ~('"' | '\\')
    ;

fragment
EscapeSequence 
   :   '\\' (
                'b' 
            |   't' 
            |   'n' 
            |   'f' 
            |   'r'
            |   'v'
            |   'a'
            |   '\"' 
            |   '\'' 
            |   '\\'
            |   ('0'..'3') ('0'..'7') ('0'..'7')
            |   ('0'..'7') ('0'..'7') 
            |   ('0'..'7')
            ) ;  

VerbatimStringLiteral
	:	'@'   '"' VerbatimStringLiteralCharacter* '"'
	;

fragment
VerbatimStringLiteralCharacter
	:	'"' '"' | ~('"') 
	;

// Boolean Literals

BooleanLiteral
    :   TRUE
    |   FALSE
    ;

// Integer Literals

IntegerLiteral
	:	Sign? UnsignedIntegerLiteral
	;

UnsignedIntegerLiteral
	:	'0'
	|	(NonZeroDigit Digits?)
	;

// Decimal Literals
// Pattern: 7M / 12M / 7.3M / 7.95M / 12.3M / 12.95M / 0.3M / 0.95M

DecimalLiteral
	:	Sign?
	(	NonZeroDigit Digits?
	|	NonZeroDigit Digits? '.' Digit Digits?
	|	'0' '.' Digit Digits? )	
		DecimalSuffix
	;

fragment
DecimalSuffix
	:	('M'|'m') -> skip
	;

// Double Literals
// Pattern: 7.3 / 7.95 / 12.3 / 12.95 / 0.3 / 0.95
// -> 7 or 12 are IntegerLiterals and the cast must happen in the interpreter

DoubleLiteral
	:	Sign?
	(	NonZeroDigit Digits? '.' Digit Digits?
	|	'0' '.' Digit Digits? )
	;

// Char Literals
// Pattern: 'a'

CharLiteral
    :   '\'' ~['\\] '\''
    ;

// NULL Literal

NullLiteral
    :   NULL
    ;

/*** GENERAL LITERAL RULES ***/

fragment
Digits
    :   Digit+
    ;

fragment
Digit
    :   '0'
    |   NonZeroDigit
    ;

fragment
NonZeroDigit
    :   [1-9]
    ;

fragment
Sign
    :   [+-]
    ;

/*** KEYWORDS ***/

// Types
STRING			: 'STRING';
INT				: 'INT';
BOOL			: 'BOOL';
DECIMAL			: 'DECIMAL';
DOUBLE			: 'DOUBLE';
CHAR			: 'CHAR';
DATETIME		: 'DATETIME';

// Query Language Keywords
CONNECT			: 'CONNECT' ;
READ			: 'READ' ;
CREATE			: 'CREATE' ;
UPDATE			: 'UPDATE' ;
DELETE			: 'DELETE' ;
SAVE			: 'SAVE' ;
EXECUTE			: 'EXECUTE' ;
SET				: 'SET' ;
GET				: 'GET' ;
FIELDS			: 'FIELDS' ;
FILTER			: 'FILTER' ;
FROM			: 'FROM' ;
TO				: 'TO' ;
AS				: 'AS' ;
LEFT			: 'LEFT' ;
JOIN			: 'JOIN' ;
COMPARE			: 'COMPARE' ;
ADD				: 'ADD' ;
SELECT			: 'SELECT' ;
WHERE			: 'WHERE' ;
DISTINCT		: 'DISTINCT' ;
ORDER			: 'ORDER' ;
BY				: 'BY' ;
DESC			: 'DESC';
DROP			: 'DROP' ;
LOG				: 'LOG' ;
EACH			: 'EACH' ;
IN				: 'IN' ;

// Event / Exception Handling
OBSERVE			: 'OBSERVE' ;
HANDLE			: 'HANDLE' ;
EMIT			: 'EMIT' ;
THROW			: 'THROW' ;

// Other Keywords
RETURN			: 'RETURN';
END				: 'END' ;
NULL			: 'NULL' ;
TRUE			: 'TRUE' ;
FALSE			: 'FALSE' ;
IF				: 'IF';
ELSE			: 'ELSE';

// Separators

LPAREN          : '(' ;
RPAREN          : ')' ;
COLON           : ':' ;
SEMI            : ';' ;
COMMA           : ',' ;
DOT             : '.' ;
BACKSLASH		: '\\' ;
DOUBLEBACKSLASH	: '\\\\' ;

// Operators

ASSIGN			: '=' ;
GT				: '>' ;
LT				: '<' ;
BANG			: '!' ;
EQUAL			: '==' ;
EXACTLYEQUAL	: '===' ;
LE				: '<=' ;
GE				: '>=' ;
NOTEQUAL		: '!=' ;
EXACTLYNOTEQUAL	: '!==' ;
AND				: 'AND' ;
OR				: 'OR' ;
PLUS			: '+' ;
MINUS			: '-' ;
STAR			: '*' ;
SLASH			: '/' ;
PERCENT			: '%' ;
QUESTION		: '?';

// Various
DOLLAR			: '$' ;

/*** IDENTIFIERS ***/

Identifier
    :   SyneryLetter SyneryLetterOrDigit*
    ;

fragment
SyneryLetter	
	:   [a-zA-Z_] 
	;

fragment
SyneryLetterOrDigit
	:   [a-zA-Z0-9_] 
	;

// e.g. "\tableGroup\myTable"
InternalPathIdentifier
	:	InternalPathPart+
	;

fragment
InternalPathPart 
	:	'\\' Identifier 
	;

// e.g. "\\myConnection\recodSetGroup\someRecordSet"
ExternalPathIdentifier
	:	'\\\\' Identifier InternalPathPart*
	;

// e.g. "p.PersonId"
ComplexIdentifier
	:	Identifier ComplexIdentifierPart+
	;

fragment
ComplexIdentifierPart 
	:	'.' Identifier 
	;

// e.g. "$String.ToUpper"
ExternalLibraryIdentifier
	:	'$' ComplexIdentifier
	;

// e.g. "#Person"
RecordTypeIdentifier
	:	'#' Identifier
	;

// e.g. "#ExternalCode.Person"
ComplexRecordTypeIdentifier
	:	'#' ComplexIdentifier
	;

// e.g. "#.Event"
SystemRecordTypeIdentifier
	:	'#.' Identifier
	;

/*** WHITESPACE AND COMMENTS ***/

WS  :  [ \t\r\n\u000C]+ -> skip
    ;

COMMENT
    :   '/*' .*? '*/' -> skip
    ;

LINE_COMMENT
    :   '//' ~[\r\n]* -> skip
    ;
