// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0
 
 parser grammar SageParser;

options { tokenVocab=SageLexer; }

contentBlockFile
    : contentBlock EOF
    ;

contentBlock
    : ampOrEmbeddedContent*
    ;

attributeNameAtSea
    : AttributeNameAtSea
    ;

inlineHtml
    : HtmlText 
    | scriptTag
    | HtmlOpen
    | HtmlSpace
    | HtmlScriptClose
    | PercentSign
    | CurlyBrace
    | SeaLessThan
    ;

scriptTag
    : HtmlScriptOpen (HtmlEquals
    | HtmlStartQuoteString
    | HtmlStartDoubleQuoteString
    | HtmlName
    | HtmlEndQuoteString
    | HtmlQuoteString
    | HtmlEndDoubleQuoteString
    | HtmlDoubleQuoteString
    | HtmlSpace)+ HtmlClose
    ;

ampBlock
    : ampStatement+
    ;
    
inlineAmpBlock
    : InlineAmpBlockStart expression InlineAmpBlockEnd
    ;

ampStatement
    : varDeclaration
    | setVariable
    | forLoop
    | ifStatement
    | expressionStatement
    ;

ampOrEmbeddedContent
    : ampBlock
    | guideContent
    | inlineHtml+
    | inlineAmpBlock
    | attributeNameAtSea
    ;

// AMPscript
varDeclaration
    : Var VarName (Comma VarName)*
    ;

setVariable
    : Set variableAssignment
    ;

variableAssignment
    : VarName Equal expression
    ;

forLoop
    : For variableAssignment (To|Downto) expression Do contentBlock Next (VarName)?
    ;

ifStatement
    : If expression Then contentBlock elseIfStatement* elseStatement? Endif
    ;

elseIfStatement
    : Elseif expression Then contentBlock
    ;

elseStatement
    : Else contentBlock
    ;

expressionStatement
    : expression
    ;

    
parentheses
    : OpenParen expression CloseParen
    ;

expression
    : constant                                                                                          #constantExpression
    | VarName                                                                                           #constantExpression
    | functionCall                                                                                      #functionExpression
    | parentheses                                                                                       #parenthesisExpression
    | attribute                                                                                         #attributeExpression
    | expression op=(LessThan | LessThanOrEqualTo | GreaterThan | GreaterThanOrEqualTo) expression      #comparisionExpression
    | expression op=(EqualTo | NotEqual) expression                                                     #comparisionExpression
    | expression op=(BooleanAnd | BooleanOr) expression                                                 #logicalExpression
    | Not expression                                                                                    #negateExpression
    | variableAssignment                                                                                #notanassignmentExpression
    ;

constant
    : True
    | False
    | Real
    | Integer
    | string
    ;

functionCall
    : NameString arguments
    ;
    
singleQuoteString
    :  SingleQuoteStringStart (SingleQuoteStringData|EscapedSingleQuote)* SingleQuoteStringEnd
    ;
doubleQuoteString
    :  DoubleQuoteStringStart (DoubleQuoteStringData|EscapedDoubleQuote)* DoubleQuoteStringEnd
    ;

string
    : singleQuoteString
    | doubleQuoteString
    ;

attribute
    : AttributeName
    | NameString
    ;

arguments
    : OpenParen ( expression (Comma expression)* )? CloseParen
    ;

// Guide
guideContent
    : guideSlotTag
    | guideBlockTag
    | guideDataTag
    ;

// {{.data}}
guideDataTag
    : guideDataTagOpen inlineHtml* guideDataTagClose;

guideDataTagOpen
    : GuideTagStart GuideOpenDataTagType GuideTagEnd;

guideDataTagClose
    : GuideTagStart GuideCloseDataTagType GuideTagEnd;


// {{.block}}
guideBlockTag
    : guideBlockTagOpen contentBlock guideBlockTagClose;

guideBlockTagOpen
    : GuideTagStart GuideOpenBlockTagType GuideIdentifier GuideIdentifier GuideTagEnd;

guideBlockTagClose
    : GuideTagStart GuideCloseBlockTagType GuideTagEnd;

// {{.slot}}
guideSlotTag
    : guideSlotTagOpen contentBlock guideSlotTagClose;

guideSlotTagOpen
    : GuideTagStart GuideOpenSlotTagType GuideIdentifier GuideTagEnd;

guideSlotTagClose
    : GuideTagStart GuideCloseSlotTagType GuideTagEnd;