// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;

// The following generates a corpus file for comparisions.
// It will take the left of the KVP and the right of the KVP and will
// generate all combinations of the comparision, along with including and excluding NOT.
var comparison = new Dictionary<string, List<KeyValuePair<string, string>>>()
 {
     { "strings", new List<KeyValuePair<string, string>>()
        {
            new("'a'", "'b'"),
            new("'a'", "1"),
            new("'a'", "'A'"),
            new("'a'", "true"),
            new("'a'", "false"),
            new("'4'", "'50'"),
            new("'4'", "1"),
            new("'Andreas Graßl'", "'Andreas Grassl'"),
        }
     },
     { "bool", new List<KeyValuePair<string, string>>()
        {
            new("true", "false"),
            new("true", "0.0"),
            new("true", "0"),
            new("true", "1"),
            new("true", "1.5"),
            new("true", "2"),
            new("false", "0.0"),
            new("false", "0"),
            new("false", "1"),
            new("false", "1.5"),
            new("false", "2"),
        }
     },
     { "numbers", new List<KeyValuePair<string, string>>()
        {
            new("0", "1"),
            new("0", "2147483647"),
            new("0", "9223372036854775807"),
            new("2147483647", "9223372036854775807"),
            new("0", "null"),
            new("2147483647", "null"),
            new("9223372036854775800.0239532", "1"),
            new("0.010000000000005116", "1"),
            new("9223372036854775800.0239532", "null"),
            new("0.01000000000000511007", "0.01000000000000511007"),
        }
     },
     { "null", new List<KeyValuePair<string, string>>()
         {
             new("null", "true"),
             new("null", "false"),
             new("null", "0"),
             new("null", "1"),
             new("null", "'0'"),
             new("null", "'1'"),
             new("null", "'a'"),
             new("null", "0.0"),
             new("null", "'0.0'"),
             new("null", "0.5"),
             new("null", "'0.5'"),
         }
     },
     { "datetime", new List<KeyValuePair<string, string>>()
         {
             new("NOW()", "true"),
             new("NOW()", "false"),
             new("NOW()", "0"),
             new("NOW()", "1"),
             new("NOW()", "'0'"),
             new("NOW()", "'1'"),
             new("NOW()", "'a'"),
             new("NOW()", "0.0"),
             new("NOW()", "'0.0'"),
             new("NOW()", "0.5"),
             new("NOW()", "'0.5'"),
             new("NOW()", "'a'"),
             new("'10/20/2024'", "'10/21/2024'"),
             new("'01/01/0001'", "0"),
             new("'12/31/9999 23:59:59'","0"),
         }
     }
 };

string[] operators = new[] { "<", "<=", "==", "!=", ">=", ">" };
foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> kvpCategory in comparison)
{
    var corpus = new StringBuilder();


    var getDeclaration = (string value, int index, string varNameClosure) =>
    {
        if (value == "null")
        {
            return $"VAR {varNameClosure}";
        }
        else
        {
            return $"SET {varNameClosure}={value}";
        }
    };

    string category = kvpCategory.Key;

    for (int inputKvpIndex = 0; inputKvpIndex < kvpCategory.Value.Count; inputKvpIndex++)
    {
        string left = kvpCategory.Value[inputKvpIndex].Key;
        string right = kvpCategory.Value[inputKvpIndex].Value;
        string leftVarName = $"@INPUTLEFT";
        string rightVarName = $"@INPUTRIGHT";

        foreach (var op in operators)
        {
            foreach (var withNot in new[] { "", "NOT" })
            {
                string expressionName = $"{withNot} {left} {op} {right}";
                string expressionWithVariables = $"{withNot} @INPUTLEFT {op} @INPUTRIGHT";

                corpus.AppendLine("==========");
                corpus.AppendLine($"{expressionName}");
                corpus.AppendLine("==========");

                corpus.AppendLine($"%%[");
                corpus.AppendLine(getDeclaration(left, inputKvpIndex, leftVarName));
                corpus.AppendLine(getDeclaration(right, inputKvpIndex, rightVarName));
                corpus.AppendLine($"SET @RESULT = false");

                corpus.AppendLine($"IF {expressionWithVariables} THEN");
                corpus.AppendLine("    SET @RESULT = true");
                corpus.AppendLine("ENDIF");
                corpus.AppendLine($"OUTPUTLINE(CONCAT('{expressionName.Replace("'", "''")}','--',@RESULT))");
                corpus.AppendLine("]%%");
                corpus.AppendLine("++++++++++");
                corpus.AppendLine("");
            }
        }
    }

    File.WriteAllText($"comparisons.{category}.txt", corpus.ToString());
}
