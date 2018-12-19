using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls;
using SimpleBookKeeping.Model;

namespace SimpleBookKeeping
{
    class TabcontrolMaker
    {
        private Pivot pivotControl = new Pivot();
        
        public Pivot getPivotControl(List<string> categories, BookKeeperContext bookKeeperContext, int type)
        {
            DatagridFactory datagridFactory = new DatagridFactory();

            foreach (var category in categories)
            {
                pivotControl.Items.Add(new PivotItem
                {
                    Header = category,
                    Content = datagridFactory.DatagridFactory_Make(category, type, bookKeeperContext),
                });
            }
            return pivotControl;
        }
       
    }
}

