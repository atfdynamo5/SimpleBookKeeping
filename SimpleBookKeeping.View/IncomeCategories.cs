using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimpleBookKeeping.Model
{
    public class IncomeCategories : INotifyPropertyChanged
    {
        ICollection<string> IncomeCategoryList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public IncomeCategories()
        {

        }

        public ICollection<string> NextIncomeCategory
        {
            get { return this.IncomeCategoryList; }
            set
            {
                this.IncomeCategoryList = value;
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
