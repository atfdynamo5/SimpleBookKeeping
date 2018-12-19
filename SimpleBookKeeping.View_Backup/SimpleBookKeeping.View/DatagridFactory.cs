using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBookKeeping.Model;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
namespace SimpleBookKeeping.View
{
    class DatagridFactory
    {
        public DataGrid DatagridFactory_Make(string category, int DBSetName, BookKeeperContext bookKeeperContext)
        {
            try
            {

                DataGrid dataGrid = new DataGrid();
                dataGrid.Name = category + "Datagrid";
                if (DBSetName == 0)
                {
                    bookKeeperContext.IncomeDBSet.Load();
                    dataGrid.AutoGenerateColumns = true;
                    Binding binding = new Binding();
                    binding.Source = "ExpenseDBSet";

                    dataGrid.ItemsSource = bookKeeperContext.IncomeDBSet
                        .Where(i => i.IncomeCategory.Contains(category)).ToList();
                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = category,
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = binding,

                    });
                    //dataGrid.DataContext = bookKeeperContext;
                    return dataGrid;
                }
                else if (DBSetName == 1)
                {
                    bookKeeperContext.ExpenseDBSet.Load();
                    dataGrid.AutoGenerateColumns = true;
                    Binding binding = new Binding();
                    binding.Source = "ExpenseDBSet";
                    dataGrid.ItemsSource = bookKeeperContext.ExpenseDBSet
                        .Where(i => i.ExpenseCategory.Contains(category)).ToList();
                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = category,
                        Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                        Binding = binding
                    });
                    // dataGrid.DataContext = bookKeeperContext;
                    return dataGrid;
                }
                else throw new ArgumentException("Invalid DBSet Name(Category)");
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
