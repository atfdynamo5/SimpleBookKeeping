using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleBookKeeping.Model
{
    public class ExpenseEntry
    {
        public int ExpenseId { get; set; }
        public string ExpenseCategory { get; set; }
        public int CheckNumber { get; set; }
        public string EntryNotes { get; set; }
        public Decimal AmountPaid { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime DatePaid { get; set; }
        public int Month { get; set; }
        public Decimal ExpenseTotal { get; set; }
    }
}
