﻿using Microsoft.Toolkit.Uwp.UI.Controls;
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
                if (category == "Gasoline")
                {
                    pivot.Items.Add(new PivotItem
                    {
                        Header = "Gasoline",
                        Content = datagridFactory.DatagridFactory_Make("Gasoline", 2, bookKeepingContext, pointerEventHandler, DataGrid_ColumnSort_Handler),
                    });
                }
                else
                {
                    pivot.Items.Add(new PivotItem
                    {
                        Header = category,
                        Content = datagridFactory.DatagridFactory_Make(category, type, bookKeepingContext, pointerEventHandler, DataGrid_ColumnSort_Handler),
                    });
                }

            }


            pivot.Name = "MainPivot";

            return pivot;
        }
    }
}