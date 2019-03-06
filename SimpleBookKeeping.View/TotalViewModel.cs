using Microsoft.EntityFrameworkCore;
using SimpleBookKeeping.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SimpleBookKeeping.View.MainPage;

namespace SimpleBookKeeping.View
{
    public class TotalViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public TotalViewModel()
        {
            AllIncomeTotals();
            AllExpenseTotals();
        }

        public void AllIncomeTotals()
        {

            incomeTotals.Clear();
            using (var context = new BookKeepingContext())
            {
                context.IncomeDBSet.Load();

                var incomeCategoryList = context.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>().ToList();
                var t = context.IncomeDBSet.GroupBy(o => new { o.IncomeCategory })
                    .Select(g => new Total
                    {
                        Category = g.Key.IncomeCategory,
                        TotalValue = g.Sum(o => o.IncomeAmount)
                    });
                if (t != null)
                {
                    foreach (var item in t)
                    {

                        incomeTotals.Add(item);
                    }
                    IncomeGrandTotal = t.Sum(x => x.TotalValue).ToString("C2");
                }

                var q = incomeTotals.FirstOrDefault(x => x.Category == "Gasoline");
                if (q != null)
                {
                    q.TotalValue = q.TotalValue * .08m;
                }
                
            }
        }

        public void AllExpenseTotals()
        {

            expenseTotals.Clear();
            using (var context = new BookKeepingContext())
            {
                context.ExpenseDBSet.Load();
                var expenseCategoryList = context.ExpenseDBSet.Select(expense => expense.ExpenseCategory).Distinct().Cast<string>().ToList();
                var t = context.ExpenseDBSet.GroupBy(o => new { o.ExpenseCategory })
                    .Select(g => new Total
                    {
                        Category = g.Key.ExpenseCategory,
                        TotalValue = g.Sum(o => o.AmountPaid)
                    });

                foreach (var item in t)
                {

                    expenseTotals.Add(item);
                }

                ExpenseGrandTotal = t.Sum(x => x.TotalValue).ToString("C2");
            }
        }
        private ObservableCollection<Total> incomeTotals = new ObservableCollection<Total>();
        private ObservableCollection<Total> expenseTotals = new ObservableCollection<Total>();
        private string _expenseGrandTotal;
        private string _incomeGrandTotal;

        //public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Total> IncomeTotals
        {
            get { return this.incomeTotals; }
            set { this.incomeTotals = value; }

        }

        public ObservableCollection<Total> ExpenseTotals
        {
            get { return this.expenseTotals; }
            set { this.expenseTotals = value; }

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


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
