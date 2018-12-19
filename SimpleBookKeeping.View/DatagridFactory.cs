using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

using SimpleBookKeeping.Model;

using Microsoft.EntityFrameworkCore;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
namespace SimpleBookKeeping.View
{
    class DatagridFactory
    {
         public DataGrid DatagridFactory_Make(string category, int DBSetType, BookKeepingContext bookKeepingContext, PointerEventHandler pointerEventHandler, EventHandler<DataGridColumnEventArgs> DataGrid_ColumnSort_Handler)
        {
            try
            {

                if (DBSetType == 0)
                {
                    
                    DataGrid dataGrid = new DataGrid();
                    dataGrid.Name = category + "IncomeDatagrid";
                    dataGrid.IsReadOnly = true;
                    bookKeepingContext.IncomeDBSet.Load();
                    dataGrid.AutoGenerateColumns = false;
                    dataGrid.CanUserSortColumns = true;
                    dataGrid.Sorting += DataGrid_ColumnSort_Handler;
                   // DataGridTextColumn gridTextColumn = new DataGridTextColumn();

                    // binding.ElementName = "EntryNotes";
                    Binding bindingMonth = new Binding() { Path = new PropertyPath("Month") };
                    Binding bindingIncomeId = new Binding() { Path = new PropertyPath("IncomeId") };
                    Binding bindingDatePaid = new Binding() { Path = new PropertyPath("DatePaid") };
                    Binding bindingIncomeAmount = new Binding() { Path = new PropertyPath("IncomeAmount") };
                    Binding bindingCheckNumber = new Binding() { Path = new PropertyPath("CheckNumber") };
                    Binding bindingEntryNotes = new Binding() { Path = new PropertyPath("EntryNotes") };
                    Binding bindingIncomeTotal = new Binding() { Path = new PropertyPath("IncomeTotal") };
                    
                    // dataGrid.DataContext = bookKeepingContext;
                    dataGrid.ItemsSource = bookKeepingContext.IncomeDBSet
                        .Where(i => i.IncomeCategory.Contains(category)).ToList();
                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Month",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingMonth,                       

                    });

                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "ID",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingIncomeId,

                    });


                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Date Paid",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingDatePaid,

                    });
                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Check Number",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingCheckNumber,
               

                    });
                    
                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Amount Paid",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingIncomeAmount,

                    });

                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Total Income",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingIncomeTotal,

                    });

                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Entry Notes",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingEntryNotes,

                    });

                    //dataGrid.DataContext = bookKeeperContext;
                    
                    dataGrid.PointerPressed += pointerEventHandler;
                    
                    return dataGrid;
                }
                else if (DBSetType == 1)
                {
                    DataGrid dataGrid = new DataGrid();
                    dataGrid.Name = category + "ExpenseDatagrid";
                    // dataGrid.PointerPressed += new PointerEventHandler((object sender, PointerRoutedEventArgs e) => { Console.Out.WriteLine("Ok"); });
                    dataGrid.IsReadOnly = true;
                    dataGrid.Sorting += DataGrid_ColumnSort_Handler;
                    bookKeepingContext.ExpenseDBSet.Load();
                    dataGrid.AutoGenerateColumns = false;
                    Binding bindingMonth = new Binding() { Path = new PropertyPath("Month") };
                    Binding bindingExpenseId = new Binding() { Path = new PropertyPath("ExpenseId") };
                    Binding bindingDatePaid = new Binding() { Path = new PropertyPath("DatePaid") };
                    Binding bindingAmountPaid = new Binding() { Path = new PropertyPath("AmountPaid") };
                    Binding bindingCheckNumber = new Binding() { Path = new PropertyPath("CheckNumber") };
                    Binding bindingExpenseTotal = new Binding() { Path = new PropertyPath("ExpenseTotal") };
                    Binding bindingEntryNotes = new Binding() { Path = new PropertyPath("EntryNotes") };
                    dataGrid.ItemsSource = bookKeepingContext.ExpenseDBSet
                        .Where(i => i.ExpenseCategory.Contains(category)).OrderBy(c => c.DatePaid).ToList();
                    
                    //dataGrid.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    //dataGrid.Background = new SolidColorBrush(Color.FromArgb(255,70,70,70));
                    //dataGrid.FontFamily = new FontFamily("Calibri");

                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Month",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingMonth,

                    });

                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "ID",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingExpenseId,

                    });


                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Date Paid",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingDatePaid,

                    });



                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Amount Paid",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingAmountPaid,

                    });



                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Check Number",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingCheckNumber,

                    });



                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Total Expense",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingExpenseTotal,

                    });



                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Entry Notes",
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        Binding = bindingEntryNotes,

                    });




                    dataGrid.CanUserSortColumns = true;
                    // dataGrid.DataContext = bookKeeperContext;
                    dataGrid.PointerPressed += pointerEventHandler;
                    
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
