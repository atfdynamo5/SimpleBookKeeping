using Microsoft.Toolkit.Uwp.UI.Controls;
using SimpleBookKeeping.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace SimpleBookKeeping.View
{
    public class IncomePivotViewModel : ObservableCollection<IncomeDataGridModel>
    {
        private List<string> categories = new List<string>();
       
        private DatagridFactory datagridFactory = new DatagridFactory();
        public IncomePivotViewModel(BookKeepingContext bookKeepingContext, PointerEventHandler pointerEventHandler, EventHandler<DataGridColumnEventArgs> DataGrid_ColumnSort_Handler)
        {
            using (var context = new BookKeepingContext())
            {
                categories = context.IncomeDBSet.Select(x => x.IncomeCategory).Distinct().ToList();
            }
            foreach (var category in categories)
            {
                Add(new IncomeDataGridModel { PivotHeader = category, DataGrid = datagridFactory.DatagridFactory_Make(category, 0, bookKeepingContext, pointerEventHandler, DataGrid_ColumnSort_Handler) });
            }
        }

        public void AddIncomePivotItem(string category, BookKeepingContext bookKeepingContext, PointerEventHandler pointerEventHandler, EventHandler<DataGridColumnEventArgs> DataGrid_ColumnSort_Handler)
        {
            
            Add(new IncomeDataGridModel { PivotHeader = category, DataGrid = datagridFactory.DatagridFactory_Make(category, 0, bookKeepingContext, pointerEventHandler, DataGrid_ColumnSort_Handler) });
            
        }

        public void UpdateIncomePivotItem(BookKeepingContext bookKeepingContext, PointerEventHandler pointerEventHandler, EventHandler<DataGridColumnEventArgs> DataGrid_ColumnSort_Handler)
        {
            this.Clear();
            using (var context = new BookKeepingContext())
            {
                categories = context.IncomeDBSet.Select(x => x.IncomeCategory).Distinct().ToList();
            }
            foreach (var category in categories)
            {
                Add(new IncomeDataGridModel { PivotHeader = category, DataGrid = datagridFactory.DatagridFactory_Make(category, 0, bookKeepingContext, pointerEventHandler, DataGrid_ColumnSort_Handler) });
            }

        }


    }
}
