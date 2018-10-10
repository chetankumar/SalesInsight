using System.ComponentModel;

namespace Redhill.SalesInsight.ESI.Enumerations
{
    public enum ComparisionType
    {
        [Description("Equals")]
        Equals,
        [Description("Not Equals")]
        NotEquals,
        [Description("Greater Than")]
        GreaterThan,
        [Description("Greater Or Equals")]
        GreaterOrEquals,
        [Description("Less Than")]
        LessThan,
        [Description("Less Or Equals")]
        LessOrEquals,
        [Description("In")]
        In,
        [Description("Not In")]
        NotIn,
        [Description("Exists")]
        Exists,
        [Description("Not Exists")]
        NotExists,
        [Description("Range")]
        Range,
    }
}