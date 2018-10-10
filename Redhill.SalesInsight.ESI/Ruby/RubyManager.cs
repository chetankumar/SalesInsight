using IronRuby;
using IronRuby.Builtins;
using Microsoft.Scripting.Hosting;
using RedHill.SalesInsight.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI
{
    public class RubyManager
    {
        public static dynamic ProcessExpression(string expression, Dictionary<string, dynamic> values)
        {
            try
            {
                if (expression == null)
                    return null;
                // First find the functions
                // $Square(@NUM_1)
                // Calc.Square(@NUM_1)
                // The expression would be something like < @NUM_1 + @NUM_2 > 

                // First we regex, and replace '@' Sign : coverting '@NUM_1 + @NUM_2' --> 'DATA_VALUES["NUM_1"] + DATA_VALUES["NUM_2"]'
                expression = Regex.Replace(expression, IMRegexPatterns.DATA_FUNCTION_PATTERN, IMRegexPatterns.DATA_FUNCTION_REPLACEMENT, RegexOptions.IgnoreCase);
                expression = Regex.Replace(expression, IMRegexPatterns.DATA_LABEL_PATTERN, IMRegexPatterns.DATA_LABEL_REPLACEMENT, RegexOptions.IgnoreCase);

                // Load iron ruby. 
                ScriptRuntime rb = Ruby.CreateRuntime();
                ScriptEngine engine = rb.GetEngine("rb");

                // Declare variable DATA_VALUES = values;
                engine.Runtime.Globals.SetVariable("DATA_VALUES", values);

                // Instantiate Calculator
                engine.Runtime.Globals.SetVariable("Calc", new RubyCalculator());

                // Process the expression 'DATA_VALUES["NUM_1"] + DATA_VALUES["NUM_2"]'
                var result = engine.Execute(expression);
                if (result is MutableString)
                {
                    result = result.ConvertToString();
                }
                return result;
            }
            catch(Exception ex)
            {
                ILogger logger = new FileLogger();
                logger.LogInfo($"ProcessExpression expression:{expression}, Exception: "+ex.ToString());
                
                return 0;
            }
        }
    }

}
