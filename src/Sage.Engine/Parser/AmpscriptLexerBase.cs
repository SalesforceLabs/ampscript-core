// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime;

namespace Sage.Engine.Parser
{
    /// <summary>
    /// This class is a helper class for the lexer to assist in complex lexing tasks.
    ///
    /// For the moment, this is meant to identify when HTML scrip tags are AMPscript tags,
    /// in order to support the HTML script tag syntax. (See: https://ampscript.guide/tag-based-syntax/)
    /// </summary>
    public abstract class AmpscriptLexerBase : Lexer
    {
        protected string _htmlNameText;
        protected bool _ampscriptScript;

        protected AmpscriptLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput)
            : base(input, output, errorOutput)
        {
        }

        /// <summary>
        /// Determines if the script tag contained "ampscript", and if it did
        /// then change the mode to AMP
        /// </summary>
        public override IToken NextToken()
        {
            CommonToken token = (CommonToken)base.NextToken();

            if (token.Type == SageLexer.HtmlName)
            {
                _htmlNameText = token.Text;
            }
            else if (token.Type == SageLexer.HtmlDoubleQuoteString)
            {
                if (string.Equals(token.Text, "ampscript", System.StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(_htmlNameText, "language"))
                {
                    _ampscriptScript = true;
                }
            }

            return token;
        }

        /// <summary>
        /// When the lexer identifies a closing tag for HTML, it'll invoke this.
        ///
        /// This will then use the logic from above to determine if the language was AMPscript,
        /// and if so, put the lexer in AMP mode.
        /// </summary>
        protected void PushModeOnHtmlClose()
        {
            PopMode();
            if (_ampscriptScript)
            {
                PushMode(SageLexer.AMP);
                _ampscriptScript = false;
            }
        }
    }
}
