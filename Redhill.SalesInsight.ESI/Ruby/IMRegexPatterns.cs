using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI
{
    public class IMRegexPatterns
    {
        public const string DATA_FUNCTION_PATTERN = @"\$([^\$]*)\(";
        public const string DATA_FUNCTION_REPLACEMENT = "Calc.$1(";

        public const string DATA_LABEL_COMPILER_PATTERN = "@[A-Za-z_0-9]+";
        public const string DATA_LABEL_PATTERN = "@([A-Za-z_0-9]+)(\\[\\S+\\])?";//"@([A-Za-z0-9_\\[\\]]+)";
        public const string DATA_LABEL_REPLACEMENT = "DATA_VALUES['$1']$2";
        public const string DATA_LABEL_TEXT_REPLACEMENT = "#{Calc.STRINGIFY(DATA_VALUES['$1']$2)}";

    }
    public class IMDataLabelHelper
    {
        public static string[] ReserveDataLabels { get { return new string[] { "TODAY", "LOOP_VARIABLE", "LOOP_COUNTER", "CURRENT_ITEM", "DEBUG_CURRENT_ITEM" }; } }
    }
}
