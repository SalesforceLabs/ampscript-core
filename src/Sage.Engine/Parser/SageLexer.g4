// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

lexer grammar SageLexer;

options {
    caseInsensitive = true;
}

HtmlText:       ~[%{]+;
SeaWhitespace:  [ \t\r\n]+ -> channel(HIDDEN);
AmpBlockStart:          '%%['  -> mode(AMP), channel(HIDDEN);
InlineAmpBlockStart:    '%%='  -> mode(AMP);
AttributeNameAtSea:    '%%' [a-z_][a-z_0-9 ]* '%%';
PercentSign:    '%';
CurlyBrace:     '{';
GuideTagStart: '{{' -> pushMode(GuideTag);

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

MultiLineComment:   '/*' .*? '*/' -> channel(HIDDEN);
AmpBlockEnd:        ']%%' -> channel(HIDDEN), mode(DEFAULT_MODE);
InlineAmpBlockEnd:  '=%%'          -> mode(DEFAULT_MODE);
Whitespace:         [ \t\r\n]+ -> channel(HIDDEN);
SingleQuoteStringStart: '\'' -> pushMode(SingleQuoteString);
DoubleQuoteStringStart: '"' -> pushMode(DoubleQuoteString);
AttributeName: '[' ~']'+ ']';


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
fragment NonZeroDigit:         [1-9];
fragment Digit:                [0-9];
NameString:         [a-z_][a-z_0-9]*;

mode SingleQuoteString;
SingleQuoteStringData:       ~[']+;
EscapedSingleQuote:       '\'\'';
SingleQuoteStringEnd:        '\'' -> popMode;


mode DoubleQuoteString;
DoubleQuoteStringData:       ~["]+;
EscapedDoubleQuote:       '""';
DoubleQuoteStringEnd:        '"' -> popMode;
