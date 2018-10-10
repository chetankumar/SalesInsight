using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProjectQuotationNotes
    {
        public SIProjectQuotationNotes()
        {
            ProjectNoteList = new List<ProjectNote>();
            QuoteList = new List<Quotation>();
        }
        public List<ProjectNote> ProjectNoteList { get; set; }
        public List<Quotation> QuoteList { get; set; }
    }
}
