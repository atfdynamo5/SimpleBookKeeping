using Microsoft.Toolkit.Uwp.UI.Controls;
using SimpleBookKeeping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace SimpleBookKeeping.View
{
    public class IncomeDataGridModel
    {
        public string PivotHeader { get; set; }

        public DataGrid DataGrid { get; set; }

    }

}