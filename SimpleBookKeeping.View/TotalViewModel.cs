using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBookKeeping.View
{
    public class TotalViewModel : INotifyPropertyChanged
    {
        
        private Dictionary<string, string> IncomeTotals { get; set; }
        private Dictionary<string, string> ExpenseTotals { get; set; }
        private string _incomeGrandTotal;
        private string _expenseGrandTotal;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        public TotalViewModel()
        {
            // this.TotalText = "123";
           
                   }

        public string IncomeGrandTotal
        {
           
            get { return this._incomeGrandTotal; }
            set
            {
                this._incomeGrandTotal = value;
                this.OnPropertyChanged();
            }
        }

        public string ExpenseGrandTotal
        {

            get { return this._expenseGrandTotal; }
            set
            {
                this._expenseGrandTotal = value;
                this.OnPropertyChanged();
            }
        }

        public Dictionary<string, string> IncomeTotalsList
        {
            get { return this.IncomeTotals; }
            set
            {
                this.IncomeTotals = value;
                this.OnPropertyChanged();
            }
        }

        public Dictionary<string, string> ExpenseTotalsList
        {
            get { return this.ExpenseTotals; }
            set
            {
                this.ExpenseTotals = value;
                this.OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
