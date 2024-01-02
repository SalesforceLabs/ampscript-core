// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

lexer grammar SageLexer;

options {
    superClass=AmpscriptLexerBase;
    caseInsensitive = true;
}

HtmlText:                   ~[<%{]+;
SeaWhitespace:              [ \t\r\n]+ -> channel(HIDDEN);
AmpBlockStart:              '%%['  -> pushMode(AMP), channel(HIDDEN);
InlineAmpBlockStart:        '%%='  -> pushMode(AMP);
AttributeNameAtSea:         '%%' [a-z_][a-z_0-9 ]* '%%';
PercentSign:                '%';
CurlyBrace:                 '{';
SeaLessThan:                '<';
GuideTagStart:              '{{' -> pushMode(GuideTag);
HtmlScriptOpen:             '<script' -> pushMode(INSIDESCRIPT);

mode GuideTag;
GuideWhitespace:            [ \t\r\n]+ -> channel(HIDDEN);
GuideOpenSlotTagType:       '.slot';
GuideOpenBlockTagType:      '.block';
GuideOpenDataTagType:       '.data';
GuideCloseBlockTagType:     '/block';
GuideCloseSlotTagType:      '/slot';
GuideCloseDataTagType:      '/data';
GuideIdentifier:            [a-z0-9]*;
GuideTagEnd:                '}}' -> popMode;

mode AMP;

MultiLineComment:           '/*' .*? '*/' -> channel(HIDDEN);
AmpBlockEnd:                ']%%' -> channel(HIDDEN), popMode;
InlineAmpBlockEnd:          '=%%'          -> popMode;
Whitespace:                 [ \t\r\n]+ -> channel(HIDDEN);
SingleQuoteStringStart:     '\'' -> pushMode(SingleQuoteString);
DoubleQuoteStringStart:     '"' -> pushMode(DoubleQuoteString);
AttributeName:              '[' ~']'+ ']';
ScriptCloseAmpScript:       '</script>' { _ampscriptScript = false; } -> popMode, channel(HIDDEN);


Set:                'set';
Var:                'var';
For:                'for';
If:                 'if';
Then:               'then';
Elseif:             'elseif';
Else:               'else';
Endif:              'endif';
To:                 'to';
Downto:             'downto';
Do:                 'do';
Next:               'next';
True:               'true';
False:              'false';
Not:                'not';

VarName:            '@' NameString;
Comma:              ',';
Equal:              '=';
EqualTo:            '==';
NotEqual:           '!=';
GreaterThan:        '>';
LessThan:           '<';
GreaterThanOrEqualTo: '>=';
LessThanOrEqualTo:  '<=';
BooleanAnd:         'and';
BooleanOr:          'or';

OpenParen:          '(';
CloseParen:         ')';

Integer:            [-]? '0' | [-]? NonZeroDigit Digit*;
Real:               [-]? (Digit+ '.' Digit* | '.' Digit+);
NameString:         [a-z_][a-z_0-9]*;

mode SingleQuoteString;
SingleQuoteStringData:       ~[']+;
EscapedSingleQuote:       '\'\'';
SingleQuoteStringEnd:        '\'' -> popMode;


mode DoubleQuoteString;
DoubleQuoteStringData:       ~["]+;
EscapedDoubleQuote:       '""';
DoubleQuoteStringEnd:        '"' -> popMode;


mode INSIDESCRIPT;
HtmlClose:                      '>' { this.PushModeOnHtmlClose(); };
HtmlSlashClose:                 '/>' -> popMode;
HtmlSlash:                      '/';
HtmlEquals:                     '=';
HtmlStartQuoteString:           '\\'? '\'' -> pushMode(HtmlQuoteStringMode);
HtmlStartDoubleQuoteString:     '\\'? '"'  -> pushMode(HtmlDoubleQuoteStringMode);
HtmlHex:                        '#' HexDigit+ ;
HtmlDecimal:                    Digit+;
HtmlSpace:                      [ \t\r\n]+;
HtmlName:                       HtmlNameStartChar HtmlNameChar*;

mode HtmlQuoteStringMode;
HtmlEndQuoteString:            '\'' '\''? ->  popMode;
HtmlQuoteString:               ~[<']+;

mode HtmlDoubleQuoteStringMode;
HtmlEndDoubleQuoteString:      '"' '"'? ->  popMode;
HtmlDoubleQuoteString:         ~[<"]+;

fragment HtmlNameChar options { caseInsensitive = false; }
    : HtmlNameStartChar
    | '-'
    | '_'
    | '.'
    | Digit
    | '\u00B7'
    | '\u0300'..'\u036F'
    | '\u203F'..'\u2040'
    ;
fragment HtmlNameStartChar options { caseInsensitive = false; }
    : [:a-zA-Z]
    | '\u2070'..'\u218F'
    | '\u2C00'..'\u2FEF'
    | '\u3001'..'\uD7FF'
    | '\uF900'..'\uFDCF'
    | '\uFDF0'..'\uFFFD'
    ;
fragment LNum:                 Digit+ ('_' Digit+)*;
fragment ExponentPart:         'e' [+-]? LNum;
fragment NonZeroDigit:         [1-9];
fragment Digit:                [0-9];
fragment OctalDigit:           [0-7];
fragment HexDigit:             [a-f0-9];


mode JAVASCRIPT;
JavascriptText:              ~[<]+;
ScriptCloseJavascript:       '</script>' { _javascript = false; } -> popMode, channel(HIDDEN);
