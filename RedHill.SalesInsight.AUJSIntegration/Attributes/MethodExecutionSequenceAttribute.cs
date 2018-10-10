using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Attributes
{
    /// <summary>
    /// Defines the execution sequence for the method that can be used to specify the sequence number as to when the method would be called
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodExecutionSequenceAttribute : System.Attribute
    {
        public int SequenceNumber { get; set; }

        public MethodExecutionSequenceAttribute(int sequenceNumber)
        {
            this.SequenceNumber = sequenceNumber;
        }
    }
}
