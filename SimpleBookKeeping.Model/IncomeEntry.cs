using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleBookKeeping.Model
{
    public class IncomeEntry
    {
        public int IncomeId { get; set; }
        public int Month { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime DatePaid { get; set; }
        public Decimal IncomeAmount { get; set; }
        public string IncomeCategory { get; set; }
        public decimal Percentage { get; set; }
        public string EntryNotes { get; set; }
        public Decimal IncomeTotal { get; set; }
    
    }
}
