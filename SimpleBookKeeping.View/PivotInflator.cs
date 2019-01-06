using Microsoft.Toolkit.Uwp.UI.Controls;
using SimpleBookKeeping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SimpleBookKeeping.View
{
    public class PivotInflator
    {
       
        public Pivot CreatePivotItems(IEnumerable<string> categories, BookKeepingContext bookKeepingContext, int type, Pivot pivot, PointerEventHandler pointerEventHandler, EventHandler<DataGridColumnEventArgs> DataGrid_ColumnSort_Handler)
        {
            DatagridFactory datagridFactory = new DatagridFactory();

            foreach (var category in categories)
            {
                pivot.Items.Add(new PivotItem
                {
                    Header = category,
                    Content = datagridFactory.DatagridFactory_Make(category, type, bookKeepingContext, pointerEventHandler, DataGrid_ColumnSort_Handler),
                });
            }
            for (var i = 0; i < 100; i++)
            {
                pivot.Items.Add(new PivotItem
                {
                    Header = "+ Add",
                    Content = new TextBlock { Text = "Hello" },
                });
            }
            pivot.Name = "MainPivot";

            return pivot;
        }
    }
}
