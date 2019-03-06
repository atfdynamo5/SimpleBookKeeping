using Microsoft.EntityFrameworkCore;
using SimpleBookKeeping.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SimpleBookKeeping.View.MainPage;

namespace SimpleBookKeeping.View
{
    public class IncomeGrandTotalViewModel : INotifyPropertyChanged
    {
        public IncomeGrandTotalViewModel(){
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
                

                IncomeGrandTotal = t.Sum(x => x.TotalValue).ToString("C2");
            }
        }
        private string _incomeGrandTotal;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string IncomeGrandTotal
        {

            get { return this._incomeGrandTotal; }
            set
            {
                this._incomeGrandTotal = value;

                this.OnPropertyChanged();
            }
        }

        public async void UpdateViewModel()
        {
            using (var context = new BookKeepingContext())
            {
                await context.IncomeDBSet.LoadAsync();
                var incomeCategoryList = context.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>().ToList();
                var t = context.IncomeDBSet.GroupBy(o => new { o.IncomeCategory })
                    .Select(g => new Total
                    {
                        Category = g.Key.IncomeCategory,
                        TotalValue = g.Sum(o => o.IncomeAmount)
                    });


                IncomeGrandTotal = t.Sum(x => x.TotalValue).ToString("C2");
            }
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
