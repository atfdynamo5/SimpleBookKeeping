using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimpleBookKeeping.Model
{
    public class ExpenseCategories : INotifyPropertyChanged
    {
        ICollection<string> ExpenseCategoryList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ExpenseCategories()
        {
            
        }

        public ICollection<string> NextExpenseCategory
        {
            get { return this.ExpenseCategoryList; }
            set
            {
                this.ExpenseCategoryList = value;
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
