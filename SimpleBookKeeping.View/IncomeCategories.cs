using Microsoft.EntityFrameworkCore;
using SimpleBookKeeping.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimpleBookKeeping.Model
{
    public class IncomeCategories : ObservableCollection<string>
    {

        ICollection<string> IncomeCategoryList { get; set; }
        public ObservableCollection<string> _categoryList = new ObservableCollection<string>();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public IncomeCategories()
        {
            using (var context = new BookKeepingContext())
            {
                context.IncomeDBSet.Load();
                var s = context.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().ToList<string>();
                Categories = new ObservableCollection<string>(s);
            }

        }

        public void UpdateCategoryList()
        {
            Categories.Clear();
            using (var context = new BookKeepingContext())
            {
                context.IncomeDBSet.Load();
                var s = context.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().ToList<string>();
                foreach ( var item in s)
                {
                    Categories.Add(item);
                }
            }
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

        public ObservableCollection<string> Categories
        {
            get { return _categoryList; }
            set
            {
                _categoryList = value;
            }

        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
