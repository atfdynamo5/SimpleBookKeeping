using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using OfficeOpenXml;
using Windows.UI.Xaml.Navigation;
using Microsoft.EntityFrameworkCore;
using SimpleBookKeeping.Model;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Core;
using LiveCharts;
using LiveCharts.Uwp;
using System.Reflection;
using Windows.UI;
using System.Text;
using Windows.UI.ViewManagement;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleBookKeeping.View
{

    class NamedColor
    {
        public NamedColor(string colorName, Color colorValue)
        {
            Name = colorName;
            Color = colorValue;
        }

        public string Name { get; set; }

        public Color Color { get; set; }

        public SolidColorBrush Brush
        {
            get { return new SolidColorBrush(Color); }
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BookKeepingContext bookKeepingContext = new BookKeepingContext();

        public Pivot expensePivot = new Pivot();
        public Pivot incomePivot = new Pivot();
        public PivotInflator pivotInflator = new PivotInflator();
        //public ICollection<string> expenseCategoryList { get; set; }
        public TotalViewModel ViewModel { get; set; }
        public DataGrid currentExpenseDatagrid { get; set; }
        public DataGrid currentIncomeDatagrid { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels = new List<string>();
        public Func<double, string> Formatter { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }
        public List<string> listTotal = new List<string>();
        public ExpenseCategories ExpenseCategories { get; set; }
        public IncomeCategories IncomeCategories { get; set; }
        public List<string> incomeCategoryList = new List<string>();
        public List<string> expenseCategoryList = new List<string>();
        public decimal TotalIncome;
        public decimal TotalExpense;
        public IChartValues Values1 { get; set; } = new ChartValues<decimal>();
        public IChartValues Values2 { get; set; } = new ChartValues<decimal>();


        public MainPage()
        {

            this.InitializeComponent();
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            // Set active window colors
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 70, 70, 70);
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 70, 70, 70);

            this.ViewModel = new TotalViewModel();

            this.ExpenseCategories = new ExpenseCategories();
            this.IncomeCategories = new IncomeCategories();
            HideAllGrids();
            DashboardGrid.Visibility = Visibility.Visible;
            ChartValues<double> cv = new ChartValues<double>();

            using (var context = new BookKeepingContext())
            {
                context.IncomeDBSet.Load();
                incomeCategoryList = context.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>().ToList();
                //this.ViewModel.TotalsList; 

                this.ViewModel.IncomeTotalsList = context.IncomeDBSet.GroupBy(o => new { o.IncomeCategory })
                    .Select(g => new
                    {
                        Category = g.Key.IncomeCategory,
                        Sum = g.Sum(o => o.IncomeAmount).ToString("C2")
                    }).ToDictionary(d => d.Category, d => d.Sum);

                this.ViewModel.IncomeGrandTotal = context.IncomeDBSet.Sum(t => t.IncomeAmount).ToString("C2");

                this.ViewModel.ExpenseTotalsList = context.ExpenseDBSet.GroupBy(o => new { o.ExpenseCategory })
                    .Select(g => new
                    {
                        Category = g.Key.ExpenseCategory,
                        Sum = g.Sum(o => o.AmountPaid).ToString("C2")
                    }).ToDictionary(d => d.Category, d => d.Sum);

                this.ViewModel.ExpenseGrandTotal = context.ExpenseDBSet.Sum(t => t.AmountPaid).ToString("C2");


                Values1.Clear();
                Values2.Clear();
                TotalIncome = context.IncomeDBSet.Sum(t => t.IncomeAmount);
                Values1.Add(TotalIncome);
                context.ExpenseDBSet.Load();
                TotalExpense = context.ExpenseDBSet.Sum(t => t.AmountPaid);
                Values2.Add(TotalExpense);

            }

            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            //bookKeepingContext.ExpenseDBSet.Load();
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "2018",
                Values = new ChartValues<double> { 11, 56, 42 }
            });

            //also adding values updates and animates the chart automatically
            SeriesCollection[1].Values.Add(48d);

            Labels = incomeCategoryList;
            Formatter = value => value.ToString("N");



            DataContext = this;
        }


        private void DashBoardButton_Click(object sender, RoutedEventArgs e)
        {

            using (var context = new BookKeepingContext())
            {
                context.IncomeDBSet.Load();
                
                this.ViewModel.IncomeTotalsList = context.IncomeDBSet.GroupBy(o => new { o.IncomeCategory })
                    .Select(g => new
                    {
                        Category = g.Key.IncomeCategory,
                        Sum = g.Sum(o => o.IncomeAmount).ToString("C2")
                    }).ToDictionary(d => d.Category, d => d.Sum);

                this.ViewModel.IncomeGrandTotal = context.IncomeDBSet.Sum(t => t.IncomeAmount).ToString("C2");

                this.ViewModel.ExpenseTotalsList = context.ExpenseDBSet.GroupBy(o => new { o.ExpenseCategory })
                    .Select(g => new
                    {
                        Category = g.Key.ExpenseCategory,
                        Sum = g.Sum(o => o.AmountPaid).ToString("C2")
                    }).ToDictionary(d => d.Category, d => d.Sum);

                this.ViewModel.ExpenseGrandTotal = context.ExpenseDBSet.Sum(t => t.AmountPaid).ToString("C2");
            }
            HideAllGrids();
            DashboardGrid.Visibility = Visibility.Visible;
        }

        private async Task Test(BookKeepingContext db)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {


                using (db = new BookKeepingContext())

                {
                    db.ExpenseDBSet.LoadAsync();

                }
                ExpenseProgressRing.IsActive = false;
            });


        }

        private async void ExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = new PivotItem();
            HideAllGrids();
            MenuExpenseButton.IsEnabled = false;
            //ExpenseGrid_Pivot.Children.Clear();
            //expensePivot.Items.Clear();
            //  pivot.Items.Add(new DataGrid());
            ExpenseProgressRing.IsActive = true;
            ExpenseGrid.Visibility = Visibility.Visible;
            loadingText.Visibility = Visibility.Visible;
            IEnumerable<string> newList = new List<string>();
            if (expensePivot.Items.Count == 0)
            {
                using (var db = new BookKeepingContext())
                {
                    await db.ExpenseDBSet.LoadAsync();
                    //expenseCategoryList = db.ExpenseDBSet.Select(expense => expense.ExpenseCategory).Distinct().Cast<string>();
                    this.ExpenseCategories.NextExpenseCategory = db.ExpenseDBSet.Select(expense => expense.ExpenseCategory).Distinct().Cast<string>().ToList();
                    //expenseCategoryList = expenseCategories.NextExpenseCategory;
                    //dataGrid.ItemsSource = db.ExpenseDBSet.ToList();
                    //dataGrid.PointerPressed += grid_click;
                    //dataGrid.PointerPressed += grid_click;
                    ExpenseActivityList.ItemsSource = db.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                    expensePivot = pivotInflator.CreatePivotItems(this.ExpenseCategories.NextExpenseCategory, db, 1, expensePivot, Ep_MouseUp, Expense_Datagrid_Sorting);
                    ExpenseGrid_Pivot.Children.Add(expensePivot); //1 is of expense type; 0 is of income type
                    pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
                    expenseTotalTextBlock.Text = db.ExpenseDBSet.Where(t => t.ExpenseCategory == pivotItem.Header.ToString()).Sum(s => s.AmountPaid).ToString("C2");
                    currentExpenseDatagrid = pivotItem.Content as DataGrid; //get first datagrid and set it to currentExpenseDatagrid so that it can be selected by default
                    currentExpenseDatagrid.SelectedIndex = 0;


                }
            }
            int count = VisualTreeHelper.GetChildrenCount(this);
            // ExpenseGrid_Pivot.SizeChanged += ExpenseGrid_Pivot_SizeChanged; PivotItem pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
            // currentExpenseDatagrid.Sorting += Expense_Datagrid_Sorting;
            //foreach (PivotItem item in expensePivot.Items)
            //{
            //    //DataGrid expense_datagrid = getExpenseDataGrid(item);
            //   // expense_datagrid = (DataGrid)FindName("testgrid");
            //    //expense_datagrid.PointerReleased += new PointerEventHandler(Ep_MouseUp);
            //}
            UpdateTotalExpense();
            expensePivot.Tapped += ExpensePivot_Tapped;
            MenuExpenseButton.IsEnabled = true;
            ExpenseProgressRing.IsActive = false;
            loadingText.Visibility = Visibility.Collapsed;

        }

       
        internal static void FindChildren<T>(List<T> results, DependencyObject startNode) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(startNode);
            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
                if ((current.GetType()).Equals(typeof(T)) || (current.GetType().GetTypeInfo().IsSubclassOf(typeof(T))))
                {
                    T asType = (T)current;
                    results.Add(asType);
                }
                FindChildren<T>(results, current);
            }
        }
        private async void IncomeButton_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = new PivotItem();
            HideAllGrids();
            MenuIncomeButton.IsEnabled = false;
            //IncomeGrid_Pivot.Children.Clear();
            //  pivot.Items.Add(new DataGrid());
            IncomeProgressRing.IsActive = true;
            IncomeGrid.Visibility = Visibility.Visible;
            IncomeLoadingText.Visibility = Visibility.Visible;


            if (incomePivot.Items.Count == 0)
            {
                try
                {

                    using (var db = new BookKeepingContext())

                    {
                        await db.IncomeDBSet.LoadAsync();
                        //IEnumerable<string> incomeCategoryList = new List<string>();
                        incomeCategoryList = db.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>().ToList();
                        this.IncomeCategories.NextIncomeCategory = incomeCategoryList.ToList();
                        //expenseCategoryList = expenseCategories.NextExpenseCategory;
                        //dataGrid.ItemsSource = db.ExpenseDBSet.ToList();
                        //dataGrid.PointerPressed += grid_click;
                        //dataGrid.PointerPressed += grid_click;
                        IncomeActivityList.ItemsSource = db.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                        incomePivot = pivotInflator.CreatePivotItems(this.IncomeCategories.NextIncomeCategory, db, 0, incomePivot, In_MouseUp, Income_Datagrid_Sorting);
                        IncomeGrid_Pivot.Children.Add(incomePivot);
                        UpdateTotalIncome();
                        pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;
                        currentIncomeDatagrid = pivotItem.Content as DataGrid; //get first datagrid and set it to currentExpenseDatagrid so that it can be selected by default
                        currentIncomeDatagrid.SelectedIndex = 0;

                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }
            incomePivot.Tapped += IncomePivot_Tapped;
            MenuIncomeButton.IsEnabled = true;
            IncomeProgressRing.IsActive = false;
            IncomeLoadingText.Visibility = Visibility.Collapsed;
        }

        private async void UpdateTotalIncome()
        {
            try
            {

                using (var db = new BookKeepingContext())

                {
                    await db.IncomeDBSet.LoadAsync();

                    PivotItem pivotItem = new PivotItem();
                    pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;
                    TotalIncome = db.IncomeDBSet.Where(t => t.IncomeCategory == pivotItem.Header.ToString()).Sum(s => s.IncomeAmount);
                    incomeTotalTextBlock.Text = TotalIncome.ToString("C2");

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void UpdateTotalExpense()
        {
            try
            {

                using (var db = new BookKeepingContext())

                {
                    await db.ExpenseDBSet.LoadAsync();

                    PivotItem pivotItem = new PivotItem();
                    pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
                    TotalExpense = db.ExpenseDBSet.Where(t => t.ExpenseCategory == pivotItem.Header.ToString()).Sum(s => s.AmountPaid);
                    expenseTotalTextBlock.Text = TotalExpense.ToString("C2"); //Two steps are required so that we can use TotalExpense for adding new entries as a decimal
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void HideAllGrids()
        {
            DashboardGrid.Visibility = Visibility.Collapsed;
            ExpenseGrid.Visibility = Visibility.Collapsed;
            IncomeGrid.Visibility = Visibility.Collapsed;
            ExportGrid.Visibility = Visibility.Collapsed;
            ProfileGrid.Visibility = Visibility.Collapsed;
            SettingsGrid.Visibility = Visibility.Collapsed;
        }

        private bool isEditExpenseEntryEmpty()
        {
            if (String.IsNullOrWhiteSpace(EditExpenseEntryNotesTextBox.Text) || String.IsNullOrWhiteSpace(EditExpenseAmountPaidTextBox.Text) || String.IsNullOrWhiteSpace(EditExpenseCheckNumberTextBox.Text))
            {
                return true;
            }
            else return false;
        }

        private bool isEditExpenseEntryCategoryEmpty()
        {
            if (String.IsNullOrWhiteSpace(EditExpenseCategoryTextBox.Text))
            {
                return true;
            }
            else return false;
        }

        private bool isAddExpenseEntryEmpty()
        {
            if (String.IsNullOrWhiteSpace(AddExpenseEntryNotesTextBox.Text) || String.IsNullOrWhiteSpace(AddExpenseAmountPaidTextBox.Text) || String.IsNullOrWhiteSpace(AddExpenseCheckNumberTextBox.Text))
            {
                return true;
            }
            else return false;
        }

        private bool isAddExpenseEntryCategoryEmpty()
        {
            if (String.IsNullOrWhiteSpace(AddExpenseCategoryTextBox.Text))
            {
                return true;
            }
            else return false;
        }


        private async void Expense_EditRecord_UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Expense_EditRecord_UpdateButton.IsEnabled = false;
            if (isEditExpenseEntryEmpty())
            {
                ContentDialog formNotFilledDialog = new ContentDialog
                {
                    Title = "Missing Entries",
                    Content = "Please make sure all fields are filled to update this record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await formNotFilledDialog.ShowAsync();
            }
            else if (isEditExpenseEntryCategoryEmpty())
            {
                ContentDialog editRecordNoCategoryDialog = new ContentDialog
                {
                    Title = "Missing Category",
                    Content = "Please make sure you select a category before updating a record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await editRecordNoCategoryDialog.ShowAsync();
            }
            else
            {
                var expense = new ExpenseEntry();
                using (var context = new BookKeepingContext())
                {
                    context.ExpenseDBSet.Load();
                    try
                    {
                        ExpenseProgressRing.IsActive = true;
                        ExpenseEntry expEntry = currentExpenseDatagrid.SelectedItem as ExpenseEntry;
                        expense = context.ExpenseDBSet.Single(b => b.ExpenseId == expEntry.ExpenseId);
                        expense.AmountPaid = decimal.Parse(EditExpenseAmountPaidTextBox.Text);
                        expense.CheckNumber = int.Parse(EditExpenseCheckNumberTextBox.Text);
                        expense.DatePaid = EditExpenseDatePaidDatePicker.Date.DateTime;
                        expense.ExpenseCategory = EditExpenseCategoryTextBox.Text;
                        expense.Month = EditExpenseDatePaidDatePicker.Date.DateTime.Month;
                        expense.TimeStamp = DateTime.Now;
                        expense.EntryNotes = EditExpenseEntryNotesTextBox.Text;
                        await context.SaveChangesAsync();
                        expense.ExpenseTotal = context.ExpenseDBSet.Where(t => t.ExpenseCategory == EditExpenseCategoryTextBox.Text).Sum(s => s.AmountPaid);
                        await context.SaveChangesAsync();
                        currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet.Where(ee => ee.ExpenseCategory == EditExpenseCategoryTextBox.Text);
                        ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                    }
                    catch (Exception exc)
                    {
                        ContentDialog editRecordExceptionDialog = new ContentDialog
                        {
                            Title = "Incorrect Input Entry",
                            Content = exc.Message.ToString() + " Please make sure all fields are in the correct format and try again.",
                            CloseButtonText = "Ok"
                        };
                        ContentDialogResult exceptionResult = await editRecordExceptionDialog.ShowAsync();
                        Expense_EditExpense_InputArea.Visibility = Visibility.Collapsed;
                        Expense_OptionBar_EditButton.IsEnabled = false;
                        Expense_OptionBar_AddButton.IsEnabled = true;
                        expensePivot.IsEnabled = true;
                        Expense_EditRecord_UpdateButton.IsEnabled = true;
                        Expense_OptionBar_EditButton.IsEnabled = true;
                        return;
                    }
                }

                ContentDialog editRecordUpdatedDialog = new ContentDialog
                {
                    Title = "Record Updated",
                    Content = expense.ExpenseCategory + " expense Record ID: " + expense.ExpenseId + " has been updated with the amount of: $" + expense.AmountPaid.ToString(),
                    CloseButtonText = "Ok"
                };
                ContentDialogResult result = await editRecordUpdatedDialog.ShowAsync();
            }

            ExpenseProgressRing.IsActive = false;
            UpdateTotalExpense();
            Expense_EditExpense_InputArea.Visibility = Visibility.Collapsed;
            Expense_OptionBar_EditButton.IsEnabled = false;
            Expense_OptionBar_AddButton.IsEnabled = true;
            expensePivot.IsEnabled = true;
            Expense_EditRecord_UpdateButton.IsEnabled = true;
            RefreshExpenseDatagrid(EditExpenseCategoryTextBox.Text, currentExpenseDatagrid);
        }

        private async void Income_EditRecord_UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Income_EditRecord_UpdateButton.IsEnabled = false;
            if (isEditIncomeEntryEmpty())
            {
                ContentDialog formNotFilledDialog = new ContentDialog
                {
                    Title = "Missing Entries",
                    Content = "Please make sure all fields are filled to update this record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await formNotFilledDialog.ShowAsync();
            }
            else if (isEditIncomeEntryCategoryEmpty())
            {
                ContentDialog editRecordNoCategoryDialog = new ContentDialog
                {
                    Title = "Missing Category",
                    Content = "Please make sure you select a category before updating a record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await editRecordNoCategoryDialog.ShowAsync();
            }
            else
            {
                var income = new IncomeEntry();
                using (var context = new BookKeepingContext())
                {
                    context.IncomeDBSet.Load();
                    try
                    {
                        ExpenseProgressRing.IsActive = true;
                        IncomeEntry incEntry = currentIncomeDatagrid.SelectedItem as IncomeEntry;
                        income = context.IncomeDBSet.Single(b => b.IncomeId == incEntry.IncomeId);
                        income.IncomeAmount = decimal.Parse(EditIncomeAmountPaidTextBox.Text);
                        //income.CheckNumber = int.Parse(EditIncomeCheckNumberTextBox.Text);
                        income.DatePaid = EditIncomeDatePaidDatePicker.Date.DateTime;
                        income.IncomeCategory = EditIncomeCategoryTextBox.Text;
                        income.Month = EditIncomeDatePaidDatePicker.Date.DateTime.Month;
                        income.TimeStamp = DateTime.Now;
                        income.EntryNotes = EditIncomeEntryNotesTextBox.Text;

                        await context.SaveChangesAsync();
                        income.IncomeTotal = context.IncomeDBSet.Where(t => t.IncomeCategory == EditIncomeCategoryTextBox.Text).Sum(s => s.IncomeAmount);
                        await context.SaveChangesAsync();
                        context.IncomeDBSet.Load();
                        currentIncomeDatagrid.ItemsSource = context.IncomeDBSet.Where(ee => ee.IncomeCategory == EditIncomeCategoryTextBox.Text);
                        IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                    }
                    catch (Exception exc)
                    {
                        ContentDialog editRecordExceptionDialog = new ContentDialog
                        {
                            Title = "Incorrect Input Entry",
                            Content = exc.Message.ToString() + " Please make sure all fields are in the correct format and try again.",
                            CloseButtonText = "Ok"
                        };
                        ContentDialogResult exceptionResult = await editRecordExceptionDialog.ShowAsync();
                        Income_EditIncome_InputArea.Visibility = Visibility.Collapsed;
                        Income_OptionBar_EditButton.IsEnabled = false;
                        Income_OptionBar_AddButton.IsEnabled = true;
                        incomePivot.IsEnabled = true;
                        Income_EditRecord_UpdateButton.IsEnabled = true;
                        Income_OptionBar_EditButton.IsEnabled = true;
                        return;
                    }
                }

                ContentDialog editRecordUpdatedDialog = new ContentDialog
                {
                    Title = "Record Updated",
                    Content = income.IncomeCategory + " income Record ID: " + income.IncomeId + " has been updated with the amount of: $" + income.IncomeAmount.ToString(),
                    CloseButtonText = "Ok"
                };
                ContentDialogResult result = await editRecordUpdatedDialog.ShowAsync();
            }
            ExpenseProgressRing.IsActive = false;
            UpdateTotalIncome();
            Income_EditIncome_InputArea.Visibility = Visibility.Collapsed;
            Income_OptionBar_EditButton.IsEnabled = false;
            Income_OptionBar_AddButton.IsEnabled = true;
            incomePivot.IsEnabled = true;
            Income_EditRecord_UpdateButton.IsEnabled = true;
            RefreshIncomeDatagrid(EditIncomeCategoryTextBox.Text, currentIncomeDatagrid);
        }

        private bool isEditIncomeEntryCategoryEmpty()
        {
            if (String.IsNullOrWhiteSpace(EditIncomeCategoryTextBox.Text))
            {
                return true;
            }
            else return false;
        }


        private bool isEditIncomeEntryEmpty()
        {
            if (String.IsNullOrWhiteSpace(EditIncomeAmountPaidTextBox.Text) || (EditIncomeDatePaidDatePicker.Date == null))
            {
                return true;
            }
            else return false;
        }

        private async void Expense_AddRecord_SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Expense_OptionBar_EditButton.IsEnabled = false;
            if (isAddExpenseEntryEmpty())
            {
                ContentDialog Expense_AddRecord_formNotFilledDialog = new ContentDialog
                {
                    Title = "Missing Entries",
                    Content = "Please make sure all fields are filled to add this record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Expense_AddRecord_formNotFilledDialog.ShowAsync();
            }
            else if (isAddExpenseEntryCategoryEmpty())
            {
                ContentDialog Expense_AddRecordNoCategoryDialog = new ContentDialog
                {
                    Title = "Missing Category",
                    Content = "Please make sure you select a category before adding a record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Expense_AddRecordNoCategoryDialog.ShowAsync();
            }
            else
            {
                ExpenseEntry expenseEntry = new ExpenseEntry
                {
                    AmountPaid = decimal.Parse(AddExpenseAmountPaidTextBox.Text),
                    CheckNumber = int.Parse(AddExpenseCheckNumberTextBox.Text),
                    DatePaid = AddExpenseDatePaidDatePicker.Date.DateTime,
                    ExpenseCategory = AddExpenseCategoryTextBox.Text,
                    Month = AddExpenseDatePaidDatePicker.Date.DateTime.Month,
                    TimeStamp = DateTime.Now,
                    EntryNotes = AddExpenseEntryNotesTextBox.Text,
                    ExpenseTotal = TotalExpense + decimal.Parse(AddExpenseAmountPaidTextBox.Text)
                };

                using (var context = new BookKeepingContext())
                {
                    //context.ExpenseDBSet.Load();
                    try
                    {
                        context.ExpenseDBSet.Add(expenseEntry);
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                ContentDialog editRecordUpdatedDialog = new ContentDialog
                {
                    Title = "Record Updated",
                    Content = expenseEntry.ExpenseCategory + " expense Record ID: " + expenseEntry.ExpenseId + " has been updated with the amount of: $" + expenseEntry.AmountPaid.ToString(),
                    CloseButtonText = "Ok"
                };
                ContentDialogResult result = await editRecordUpdatedDialog.ShowAsync();
            }
            UpdateTotalExpense();
            RefreshExpenseDatagrid(AddExpenseCategoryTextBox.Text, currentExpenseDatagrid);
            Clear_AddExpense_ControlElements();
            Expense_AddExpense_InputArea.Visibility = Visibility.Collapsed;
            Expense_OptionBar_EditButton.IsEnabled = true;
            expensePivot.IsEnabled = true;
        }

        private async void Expense_ShowYear_Button_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = new PivotItem();
            pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
            ExpenseProgressRing.IsActive = true;
            try
            {


                using (var db = new BookKeepingContext())
                {
                    await db.ExpenseDBSet.LoadAsync();
                    //expenseCategoryList = db.ExpenseDBSet.Select(expense => expense.ExpenseCategory).Distinct().Cast<string>();

                    currentExpenseDatagrid.ItemsSource = db.ExpenseDBSet.Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).Where(d => (d.DatePaid.Date.Year == Expense_From_DatePicker.Date.Year)).OrderBy(c => c.DatePaid).ToList();

                    currentExpenseDatagrid.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {

                ContentDialog Expense_FromToInvalid_Dialog = new ContentDialog
                {
                    Title = "Invalid Date Entry",
                    Content = "Please ensure the date range is valid and try again.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Expense_FromToInvalid_Dialog.ShowAsync();
            }

            ExpenseProgressRing.IsActive = false;
        }

        private void Income_EditRecord_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Income_EditIncome_InputArea.Visibility = Visibility.Collapsed;
            incomePivot.IsEnabled = true;
            Clear_EditIncome_ControlElements();
            Income_OptionBar_AddButton.IsEnabled = true;
        }

        private void Clear_EditIncome_ControlElements()
        {
            EditIncomeCategoryTextBox.Text = "";
            //EditIncomeCheckNumberTextBox.Text = "";
            EditIncomeDatePaidDatePicker.Date = DateTime.Now;
            EditIncomeEntryNotesTextBox.Text = "";
            EditIncomeAmountPaidTextBox.Text = "";
        }

        private void Expense_AddRecord_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Clear_AddExpense_ControlElements();
            Expense_AddExpense_InputArea.Visibility = Visibility.Collapsed;
            expensePivot.IsEnabled = true;

        }

        private void Expense_OptionsBar_EditButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridColumn ExpenseGridIdColumn = (DataGridColumn)currentExpenseDatagrid.Columns.First(p => p.Header.ToString() == "ID");
            ExpenseEntry expEntry = currentExpenseDatagrid.SelectedItem as ExpenseEntry;
            expensePivot.IsEnabled = false;
            EditExpenseCategoryTextBox.Text = expEntry.ExpenseCategory.ToString();
            EditExpenseCheckNumberTextBox.Text = expEntry.CheckNumber.ToString();

            EditExpenseDatePaidDatePicker.Date = expEntry.DatePaid;
            EditExpenseEntryNotesTextBox.Text = expEntry.EntryNotes.ToString();
            EditExpenseAmountPaidTextBox.Text = expEntry.AmountPaid.ToString();

            Expense_AddExpense_InputArea.Visibility = Visibility.Collapsed;
            Expense_EditExpense_InputArea.Visibility = Visibility.Visible;
            Expense_OptionBar_AddButton.IsEnabled = false;

        }


        private void Expense_EditRecord_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Expense_EditExpense_InputArea.Visibility = Visibility.Collapsed;
            expensePivot.IsEnabled = true;
            Clear_EditExpense_ControlElements();
            Expense_OptionBar_AddButton.IsEnabled = true;
        }

        private void Expense_OptionsBar_AddButton_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
            currentExpenseDatagrid = pivotItem.Content as DataGrid;
            currentExpenseDatagrid.SelectedIndex = 0;

            DataGridColumn ExpenseGridIdColumn = (DataGridColumn)currentExpenseDatagrid.Columns.First(p => p.Header.ToString() == "ID");
            ExpenseEntry expenseEntry = currentExpenseDatagrid.SelectedItem as ExpenseEntry;
            expensePivot.IsEnabled = false;
            AddExpenseCategoryTextBox.Text = pivotItem.Header.ToString();

            Expense_EditExpense_InputArea.Visibility = Visibility.Collapsed;
            Expense_AddExpense_InputArea.Visibility = Visibility.Visible;
            Expense_OptionBar_EditButton.IsEnabled = false;

        }

        private void Income_OptionsBar_AddButton_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;
            currentIncomeDatagrid = pivotItem.Content as DataGrid;
            currentIncomeDatagrid.SelectedIndex = 0;

            DataGridColumn IncomeGridIdColumn = (DataGridColumn)currentIncomeDatagrid.Columns.First(p => p.Header.ToString() == "ID");
            IncomeEntry incomeEntry = currentIncomeDatagrid.SelectedItem as IncomeEntry;
            incomePivot.IsEnabled = false;
            AddIncomeCategoryTextBox.Text = pivotItem.Header.ToString();

            Income_EditIncome_InputArea.Visibility = Visibility.Collapsed;
            Income_AddIncome_InputArea.Visibility = Visibility.Visible;
            Income_OptionBar_EditButton.IsEnabled = false;
        }

        private void Income_OptionsBar_EditButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridColumn IncomeGridIdColumn = (DataGridColumn)currentIncomeDatagrid.Columns.First(p => p.Header.ToString() == "ID");
            IncomeEntry incomeEntry = currentIncomeDatagrid.SelectedItem as IncomeEntry;
            incomePivot.IsEnabled = false;
            EditIncomeCategoryTextBox.Text = incomeEntry.IncomeCategory.ToString();
            //EditIncomeCheckNumberTextBox.Text = incomeEntry.CheckNumber.ToString();

            EditIncomeDatePaidDatePicker.Date = incomeEntry.DatePaid;
            EditIncomeEntryNotesTextBox.Text = incomeEntry.EntryNotes.ToString();
            EditIncomeAmountPaidTextBox.Text = incomeEntry.IncomeAmount.ToString();

            Income_AddIncome_InputArea.Visibility = Visibility.Collapsed;
            Income_EditIncome_InputArea.Visibility = Visibility.Visible;
            Income_OptionBar_AddButton.IsEnabled = false;
        }


        private async void Income_AddRecord_SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Income_OptionBar_EditButton.IsEnabled = false;
            if (isAddIncomeEntryEmpty())
            {
                ContentDialog Income_AddRecord_formNotFilledDialog = new ContentDialog
                {
                    Title = "Missing Entries",
                    Content = "Please make sure all fields are filled to add this record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Income_AddRecord_formNotFilledDialog.ShowAsync();
            }
            else if (isAddIncomeEntryCategoryEmpty())
            {
                ContentDialog Income_AddRecordNoCategoryDialog = new ContentDialog
                {
                    Title = "Missing Category",
                    Content = "Please make sure you select a category before adding a record.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Income_AddRecordNoCategoryDialog.ShowAsync();
            }
            else
            {
                IncomeEntry incomeEntry = new IncomeEntry
                {
                    IncomeAmount = decimal.Parse(AddIncomeAmountPaidTextBox.Text),
                    //CheckNumber = int.Parse(AddIncomeCheckNumberTextBox.Text),
                    DatePaid = AddIncomeDatePaidDatePicker.Date.DateTime,
                    IncomeCategory = AddIncomeCategoryTextBox.Text,
                    Month = AddIncomeDatePaidDatePicker.Date.DateTime.Month,
                    TimeStamp = DateTime.Now,
                    EntryNotes = AddIncomeEntryNotesTextBox.Text,
                    IncomeTotal = TotalIncome + decimal.Parse(AddIncomeAmountPaidTextBox.Text)
                };

                using (var context = new BookKeepingContext())
                {
                    //context.IncomeDBSet.Load();
                    try
                    {
                        context.IncomeDBSet.Add(incomeEntry);
                        context.SaveChanges();


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                ContentDialog editRecordUpdatedDialog = new ContentDialog
                {
                    Title = "Record Updated",
                    Content = incomeEntry.IncomeCategory + " income Record ID: " + incomeEntry.IncomeId + " has been updated with the amount of: $" + incomeEntry.IncomeAmount.ToString(),
                    CloseButtonText = "Ok"
                };
                ContentDialogResult result = await editRecordUpdatedDialog.ShowAsync();
            }
            UpdateTotalIncome();
            RefreshIncomeDatagrid(AddIncomeCategoryTextBox.Text, currentIncomeDatagrid);
            Clear_AddIncome_ControlElements();
            Income_AddIncome_InputArea.Visibility = Visibility.Collapsed;
            Income_OptionBar_EditButton.IsEnabled = true;
            incomePivot.IsEnabled = true;
        }

        private void Clear_AddIncome_ControlElements()
        {
            AddIncomeCategoryTextBox.Text = "";
            //AddIncomeCheckNumberTextBox.Text = "";
            AddIncomeDatePaidDatePicker.Date = DateTime.Now;
            AddIncomeEntryNotesTextBox.Text = "";
            AddIncomeAmountPaidTextBox.Text = "";
        }

        private bool isAddIncomeEntryCategoryEmpty()
        {
            if (String.IsNullOrWhiteSpace(AddIncomeCategoryTextBox.Text))
            {
                return true;
            }
            else return false;
        }

        private bool isAddIncomeEntryEmpty()
        {
            if (String.IsNullOrWhiteSpace(AddIncomeEntryNotesTextBox.Text) || String.IsNullOrWhiteSpace(AddIncomeAmountPaidTextBox.Text))
            {
                return true;
            }
            else return false;
        }

        private void Income_AddRecord_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Clear_AddIncome_ControlElements();
            Income_AddIncome_InputArea.Visibility = Visibility.Collapsed;
            incomePivot.IsEnabled = true;
        }

        private async void ExportExecuteButton_Click(object sender, RoutedEventArgs e)
        {

            if (expenseCategoryListView.SelectedItems.Count == 0 && incomeCategoryListView.SelectedItems.Count == 0)
            {
                ContentDialog Export_NoSelectedCategoryDialog = new ContentDialog
                {
                    Title = "No Category Selected",
                    Content = "Please select at least one category",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult resultNoCategory = await Export_NoSelectedCategoryDialog.ShowAsync();

                return;
            }
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Excel Document", new List<string>() { ".xlsx" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "Accounting Report";
            try
            {

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (file == null)
                {
                    return;
                }
                FileInfo choosenFile = new FileInfo(file.Path);

                using (ExcelPackage package = new ExcelPackage(choosenFile))
                {
                    foreach (string category in expenseCategoryListView.SelectedItems)
                    {
                        var context = new BookKeepingContext();
                        IList<ExpenseEntry> expenseList;
                        using (context = new BookKeepingContext())
                        {
                            await context.ExpenseDBSet.LoadAsync();
                            expenseList = context.ExpenseDBSet.Where(c => c.ExpenseCategory == category).ToList();
                        }
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(category + " Expense");
                        int totalRows = expenseList.Count();

                        worksheet.Cells[1, 1].Value = "Expense ID";
                        worksheet.Cells[1, 2].Value = "DatePaid";
                        worksheet.Cells[1, 3].Value = "Amount Paid";
                        worksheet.Cells[1, 4].Value = "Check Number";
                        worksheet.Cells[1, 5].Value = "Expense Total";
                        worksheet.Cells[1, 6].Value = "Entry Notes";
                        int i = 0;
                        for (int row = 2; row <= totalRows + 1; row++)
                        {
                            worksheet.Cells[row, 1].Value = expenseList[i].ExpenseId;
                            worksheet.Cells[row, 2].Value = expenseList[i].DatePaid;
                            worksheet.Cells[row, 3].Value = expenseList[i].AmountPaid;
                            worksheet.Cells[row, 4].Value = expenseList[i].CheckNumber;
                            worksheet.Cells[row, 5].Value = expenseList[i].ExpenseTotal;
                            worksheet.Cells[row, 6].Value = expenseList[i].EntryNotes;

                            i++;
                        }
                    }

                    foreach (string category in incomeCategoryListView.SelectedItems)
                    {
                        var context = new BookKeepingContext();
                        IList<IncomeEntry> incomeList;
                        using (context = new BookKeepingContext())
                        {
                            await context.IncomeDBSet.LoadAsync();
                            incomeList = context.IncomeDBSet.Where(c => c.IncomeCategory == category).ToList();
                        }
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(category + " Income");
                        int totalRows = incomeList.Count();

                        worksheet.Cells[1, 1].Value = "Income ID";
                        worksheet.Cells[1, 2].Value = "DatePaid";
                        worksheet.Cells[1, 3].Value = "Amount";
                        worksheet.Cells[1, 4].Value = "Check Number";
                        worksheet.Cells[1, 5].Value = "Income Total";
                        worksheet.Cells[1, 6].Value = "Entry Notes";
                        int i = 0;
                        for (int row = 2; row <= totalRows + 1; row++)
                        {
                            worksheet.Cells[row, 1].Value = incomeList[i].IncomeId;
                            worksheet.Cells[row, 2].Value = incomeList[i].DatePaid;
                            worksheet.Cells[row, 3].Value = incomeList[i].IncomeAmount;
                            worksheet.Cells[row, 4].Value = incomeList[i].CheckNumber;
                            worksheet.Cells[row, 5].Value = incomeList[i].IncomeTotal;
                            worksheet.Cells[row, 6].Value = incomeList[i].EntryNotes;

                            i++;
                        }
                    }

                    var stream = await file.OpenStreamForWriteAsync();
                    package.SaveAs(stream);
                    stream.Dispose();
                    //package.Dispose();

                }

                ContentDialog Export_FileExportedDialog = new ContentDialog
                {
                    Title = "Export successfull",
                    Content = "File exported to: " + choosenFile.FullName,
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Export_FileExportedDialog.ShowAsync();
            }
            catch (Exception exportException)
            {

                ContentDialog Export_ErrorDialog = new ContentDialog
                {
                    Title = "Unexpected error",
                    Content = exportException.Message,
                    CloseButtonText = "Ok",
                };

                ContentDialogResult result = await Export_ErrorDialog.ShowAsync();
            }

        }
        private async void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {


            HideAllGrids();
            ExportGrid.Visibility = Visibility.Visible;
            using (var db = new BookKeepingContext())

            {
                await db.IncomeDBSet.LoadAsync();
                incomeCategoryList = db.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>().ToList();
                incomeCategoryListView.ItemsSource = incomeCategoryList;
                await db.ExpenseDBSet.LoadAsync();
                expenseCategoryList = db.ExpenseDBSet.Select(expense => expense.ExpenseCategory).Distinct().Cast<string>().ToList();
                expenseCategoryListView.ItemsSource = expenseCategoryList;
            }


            // categoryListView.ItemsSource = list1;



            //using (var p = new ExcelPackage())
            //{
            //    var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            //    var folderPath = localFolder.Path;
            //    //A workbook must have at least on cell, so lets add one... 
            //    var ws = p.Workbook.Worksheets.Add("MySheet");
            //    //To set values in the spreadsheet use the Cells indexer.
            //    ws.Cells["A1"].Value = "This is cell A1";
            //    //Save the new workbook. We haven't specified the filename so use the Save as method.
            //    p.SaveAs(new FileInfo(folderPath.ToString() + @"\myworkbook.xlsx"));
            //    ContentDialog Export_FileExportedDialog = new ContentDialog
            //    {
            //        Title = "Export successfull",
            //        Content = "File exported to: "+ folderPath.ToString() + @"\myworkbook.xlsx",
            //        CloseButtonText = "Ok"
            //    };

            //    ContentDialogResult result = await Export_FileExportedDialog.ShowAsync();
            //}
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            List<DataGrid> results = new List<DataGrid>();
            FindChildren<DataGrid>(results, MainPageXaml);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public static T FindControl<T>(UIElement parent, Type targetType, string ControlName) where T : FrameworkElement
        {

            if (parent == null) return null;

            if (parent.GetType() == targetType && ((T)parent).Name == ControlName)
            {
                return (T)parent;
            }
            T result = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);

                if (FindControl<T>(child, targetType, ControlName) != null)
                {
                    result = FindControl<T>(child, targetType, ControlName);
                    break;
                }
            }
            return result;
        }


        private void Ep_MouseUp(object sender, PointerRoutedEventArgs e)
        {

            currentExpenseDatagrid = sender as DataGrid;

            //DataGridRow dataGridRow = (DataGridRow)currentDataGrid.Columns.First(r => r.Header.ToString() == "Grocery");

            Expense_OptionBar_EditButton.IsEnabled = true;
            Expense_OptionBar_DarkBackground_Button.IsEnabled = true;
            Expense_OptionBar_LightBackground_Button.IsEnabled = true;
            Expense_OptionBar_DeleteButton.IsEnabled = true;
            //UIElement el = sender as UIElement;
            //el.ReleasePointerCapture();
            //var g = p.Columns.Where(h => h.Header.ToString() == "ExpenseId");
            //List<PivotItem> pi = new List<PivotItem>();
            //foreach (PivotItem item in expensePivot.Items)
            //{
            //    pi.Add(item);
            //}
            ////PivotItem pivotName = (PivotItem)expensePivot.SelectedItem;

            //var ep = (DataGrid)LogicalTreeHelper.FindLogicalNode(this, tabName.Header.ToString() + "Datagrid");
            //DataGrid ep = (DataGrid)pi[0].Content;
            //DataGrid ep = sender as DataGrid;
            //object item2 = ep.SelectedItem;
            //if (ep.SelectedIndex > 1)
            // {
            //   var gr2 = ep.SelectedItem as ExpenseEntry;
            // EditExpenseCategoryTextBox.Text = gr2.ExpenseId.ToString();

            //  DataGridColumn ExpenseGridIdColumn = (DataGridColumn)ep.Columns.First(p => p.Header.ToString() == "ExpenseId");
            //DataGridCell gr = new DataGridCell();
            //var gr2 = (DataGridRow)ExpenseGridIdColumn.GetCellContent(gr);
            //ListBox listBox = new ListBox();
            //listBox.ItemsSource = ep.SelectedItem;
            // listBox.ItemsSource = ExpenseGridIdColumn.GetCellContent(gr);
            //CurrentColumn.GetCellContent() as TextBlock).Text;
            //  ContentDialog showID = new ContentDialog()
            //{

            //    Title = "ID",
            //    Content = gr2.ExpenseId,
            //    CloseButtonText = "Ok"
            //};

            //ContentDialogResult result = await showID.ShowAsync();

            //if (!String.IsNullOrEmpty(id.ToString()))
            //{
            //    using (var context = new BookKeepingContext())
            //    {
            //        context.ExpenseDBSet.Load();

            //        var expense = context.ExpenseDBSet.Single(b => b.ExpenseId == int.Parse(id));
            //        //categoryText.Text = expense.ExpenseCategory;
            //        //grid.DataContext = expense;

            //    }
            //}
            //}
        }
        private void In_MouseUp(object sender, PointerRoutedEventArgs e)
        {

            currentIncomeDatagrid = sender as DataGrid;
            //DataGridRow dataGridRow = (DataGridRow)currentDataGrid.Columns.First(r => r.Header.ToString() == "Grocery");
            incomePivot.Tapped += IncomePivot_Tapped;
            if (currentIncomeDatagrid != null)
            {
                Income_OptionBar_EditButton.IsEnabled = true;
                Income_OptionBar_DarkBackground_Button.IsEnabled = true;
                Income_OptionBar_LightBackground_Button.IsEnabled = true;
                Income_OptionBar_DeleteButton.IsEnabled = true;
            }
        }

        private async void IncomePivot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PivotItem pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;
            currentIncomeDatagrid = pivotItem.Content as DataGrid;
            
            
            currentIncomeDatagrid.SelectedIndex = 0;
            if (currentIncomeDatagrid.SelectedItems.Count > 0)
            {
                Income_OptionBar_EditButton.IsEnabled = true;
            }
            else
            {
                Income_OptionBar_EditButton.IsEnabled = false;
                return;
            }
            using (var context = new BookKeepingContext())
            {
                await context.IncomeDBSet.LoadAsync();

                incomeTotalTextBlock.Text = context.IncomeDBSet.Where(t => t.IncomeCategory == pivotItem.Header.ToString()).Sum(s => s.IncomeAmount).ToString("C2");

            }
           
            
        }

        private async void ExpensePivot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PivotItem pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
            currentExpenseDatagrid = pivotItem.Content as DataGrid;
            currentExpenseDatagrid.SelectedIndex = 0;

            using (var context = new BookKeepingContext())
            {
                await context.ExpenseDBSet.LoadAsync();

                expenseTotalTextBlock.Text = context.ExpenseDBSet.Where(t => t.ExpenseCategory == pivotItem.Header.ToString()).Sum(s => s.AmountPaid).ToString("C");

            }
            Expense_OptionBar_EditButton.IsEnabled = true;
        }

        private DataGrid getExpenseDataGrid(PivotItem pivotItem)
        {
            var ExpenseDataGrid = (DataGrid)FindName(pivotItem.Header.ToString());
            return ExpenseDataGrid;
        }

        private void Clear_EditExpense_ControlElements()
        {
            EditExpenseCategoryTextBox.Text = "";
            EditExpenseCheckNumberTextBox.Text = "";
            EditExpenseDatePaidDatePicker.Date = DateTime.Now;
            EditExpenseEntryNotesTextBox.Text = "";
            EditExpenseAmountPaidTextBox.Text = "";
        }

        private void Clear_AddExpense_ControlElements()
        {
            AddExpenseCategoryTextBox.Text = "";
            AddExpenseCheckNumberTextBox.Text = "";
            AddExpenseDatePaidDatePicker.Date = DateTime.Now;
            AddExpenseEntryNotesTextBox.Text = "";
            AddExpenseAmountPaidTextBox.Text = "";
        }



        private void RefreshExpenseDatagrid(string category, DataGrid datagrid)
        {
            var ep = datagrid;


            using (var context = new BookKeepingContext())
            {
                context.ExpenseDBSet.Load();
                try
                {
                    ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                    ep.ItemsSource = context.ExpenseDBSet
                        .Where(i => i.ExpenseCategory.Contains(category)).Where(d => d.DatePaid.Year == Expense_From_DatePicker.Date.Year).OrderBy(c => c.DatePaid).ToList();

                }
                catch (Exception)
                {
                    throw;
                }
                //    ep.MouseUp += Ep_MouseUp;
            }
            //ep.Sorting += Expense_Datagrid_Sorting;
            PivotItem pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
            ep = pivotItem.Content as DataGrid;
            ep.SelectedIndex = 0;
            ep.CanUserSortColumns = true;

            // ep.Columns.ElementAt(5).SortDirection = DataGridSortDirection.Ascending;
        }

        private void Expense_Datagrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            PivotItem pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                //Use the Tag property to pass the bound column name for the sorting implementation 
                if (e.Column.Header.ToString() == "Date Paid")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.ExpenseDBSet.Load();
                        try
                        {
                            ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet
                                .Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.DatePaid).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "ID")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.ExpenseDBSet.Load();
                        try
                        {
                            ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet
                                .Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.ExpenseId).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Month")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.ExpenseDBSet.Load();
                        try
                        {
                            ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet
                                .Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.Month).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Total Expense")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.ExpenseDBSet.Load();
                        try
                        {
                            ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet
                                .Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.ExpenseTotal).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Entry Notes")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.ExpenseDBSet.Load();
                        try
                        {
                            ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet
                                .Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.EntryNotes).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Check Number")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.ExpenseDBSet.Load();
                        try
                        {
                            ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet
                                .Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.CheckNumber).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Amount Paid")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.ExpenseDBSet.Load();
                        try
                        {
                            ExpenseActivityList.ItemsSource = context.ExpenseDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentExpenseDatagrid.ItemsSource = context.ExpenseDBSet
                                .Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.AmountPaid).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
            }
        }
        private void Income_Datagrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            PivotItem pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                //Use the Tag property to pass the bound column name for the sorting implementation 
                if (e.Column.Header.ToString() == "Date Paid")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.IncomeDBSet.Load();
                        try
                        {
                            IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentIncomeDatagrid.ItemsSource = context.IncomeDBSet
                                .Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.DatePaid).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "ID")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.IncomeDBSet.Load();
                        try
                        {
                            IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentIncomeDatagrid.ItemsSource = context.IncomeDBSet
                                .Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.IncomeId).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Month")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.IncomeDBSet.Load();
                        try
                        {
                            IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentIncomeDatagrid.ItemsSource = context.IncomeDBSet
                                .Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.Month).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Total Income")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.IncomeDBSet.Load();
                        try
                        {
                            IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentIncomeDatagrid.ItemsSource = context.IncomeDBSet
                                .Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.IncomeTotal).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Entry Notes")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.IncomeDBSet.Load();
                        try
                        {
                            IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentIncomeDatagrid.ItemsSource = context.IncomeDBSet
                                .Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.EntryNotes).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Check Number")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.IncomeDBSet.Load();
                        try
                        {
                            IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentIncomeDatagrid.ItemsSource = context.IncomeDBSet
                                .Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.CheckNumber).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
                else if (e.Column.Header.ToString() == "Amount Paid")
                {
                    //Implement ascending sort on the column "Range" using LINQ
                    using (var context = new BookKeepingContext())
                    {
                        context.IncomeDBSet.Load();
                        try
                        {
                            IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                            currentIncomeDatagrid.ItemsSource = context.IncomeDBSet
                                .Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).OrderByDescending(c => c.IncomeAmount).ToList();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        //    ep.MouseUp += Ep_MouseUp;
                    }

                }
            }
        }
        private void RefreshIncomeDatagrid(string category, DataGrid datagrid)
        {
            var ep = datagrid;
            using (var context = new BookKeepingContext())
            {
                context.IncomeDBSet.Load();
                try
                {
                    IncomeActivityList.ItemsSource = context.IncomeDBSet.OrderByDescending(d => d.TimeStamp).ToList();
                    ep.ItemsSource = context.IncomeDBSet
                        .Where(i => i.IncomeCategory.Contains(category)).Where(d => d.DatePaid.Year == Income_From_DatePicker.Date.Year).OrderByDescending(c => c.DatePaid).ToList();

                }
                catch (Exception)
                {
                    throw;
                }
                //    ep.MouseUp += Ep_MouseUp;
            }
            PivotItem pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;
            ep = pivotItem.Content as DataGrid;
            ep.SelectedIndex = 0;

        }

        private void Expense_AppBarButton_DarkBackground_Click(object sender, RoutedEventArgs e)
        {

            currentExpenseDatagrid.Background = new SolidColorBrush(Color.FromArgb(255, 70, 70, 70));
            //currentDataGrid.Background = new SolidColorBrush(Color.FromArgb(255, 70, 70, 70));
            currentExpenseDatagrid.Foreground = new SolidColorBrush(Colors.LightGoldenrodYellow);
        }

        private void Expense_AppBarButton_LightBackground_Click(object sender, RoutedEventArgs e)
        {
            currentExpenseDatagrid.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
            currentExpenseDatagrid.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void Income_AppBarButton_DarkBackground_Click(object sender, RoutedEventArgs e)
        {

            currentIncomeDatagrid.Background = new SolidColorBrush(Color.FromArgb(255, 70, 70, 70));
            //currentDataGrid.Background = new SolidColorBrush(Color.FromArgb(255, 70, 70, 70));
            currentIncomeDatagrid.Foreground = new SolidColorBrush(Colors.LightGoldenrodYellow);
        }

        private void Income_AppBarButton_LightBackground_Click(object sender, RoutedEventArgs e)
        {
            currentIncomeDatagrid.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
            currentIncomeDatagrid.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void OptionsAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void OptionsAllCheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {

        }

        private void Option1CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Option1CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Option2CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Option2CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Option3CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Option3CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private async void Income_ShowYear_Button_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = new PivotItem();
            pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;
            IncomeProgressRing.IsActive = true;
            try
            {


                using (var db = new BookKeepingContext())
                {
                    await db.IncomeDBSet.LoadAsync();
                    //incomeCategoryList = db.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>();

                    currentIncomeDatagrid.ItemsSource = db.IncomeDBSet.Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).Where(d => (d.DatePaid.Date.Year == Income_From_DatePicker.Date.Year)).OrderBy(c => c.DatePaid).ToList();

                    currentIncomeDatagrid.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {

                ContentDialog Income_FromToInvalid_Dialog = new ContentDialog
                {
                    Title = "Invalid Date Entry",
                    Content = "Please ensure the date range is valid and try again.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Income_FromToInvalid_Dialog.ShowAsync();
            }

            IncomeProgressRing.IsActive = false;
        }

        private async void Income_OptionBar_DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog Income_DeleteEntry_Dialog = new ContentDialog
            {
                Title = "Delete Entry Permanently?",
                Content = "This will permanently remove the selected entry, are you sure you want to continue?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };


            ContentDialogResult result = await Income_DeleteEntry_Dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                PivotItem pivotItem = new PivotItem();
                pivotItem = incomePivot.Items[incomePivot.SelectedIndex] as PivotItem;
                IncomeProgressRing.IsActive = true;
                try
                {


                    using (var db = new BookKeepingContext())
                    {
                        await db.IncomeDBSet.LoadAsync();
                        IncomeEntry incEntry = currentIncomeDatagrid.SelectedItem as IncomeEntry;
                        var income = db.IncomeDBSet.Single(b => b.IncomeId == incEntry.IncomeId);
                        db.IncomeDBSet.Remove(income);
                        db.SaveChanges();
                        //incomeCategoryList = db.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>();
                        if(incomePivot.Items.Count > 1)
                        {
                            currentIncomeDatagrid.ItemsSource = db.IncomeDBSet.Where(i => i.IncomeCategory.Contains(pivotItem.Header.ToString())).Where(d => (d.DatePaid.Date.Year == Income_From_DatePicker.Date.Year)).OrderBy(c => c.DatePaid).ToList();
                        }
                        
                    }
                    
                    Income_OptionBar_DeleteButton.IsEnabled = false;
                    currentIncomeDatagrid.SelectedIndex = 0;

                }
                catch (Exception)
                {
                    IncomeProgressRing.IsActive = false;
                    ContentDialog Income_ErrorDeleting_Dialog = new ContentDialog
                    {
                        Title = "Unexpected Error",
                        Content = "Unexpected error occured while attempting to delete the entry. Please try again or contact technical support.",
                        CloseButtonText = "Ok"
                    };

                    ContentDialogResult errorResult = await Income_ErrorDeleting_Dialog.ShowAsync();
                }

                IncomeProgressRing.IsActive = false;
            }
            else
            {
                Income_OptionBar_DeleteButton.IsEnabled = false;
                return;
            }



        }

        private async void Income_AddCategory_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BookKeepingContext())
            {
                context.IncomeDBSet.Load();
                incomeCategoryList = context.IncomeDBSet.Select(income => income.IncomeCategory).Distinct().Cast<string>().ToList();
            }
                string inputString;

            inputString = await InputStringDialogAsync(
            "What would you like to name the new Category?",
                    "Groceries",
                    "Ok",
                    "Cancel"
        );
            if (String.IsNullOrEmpty(inputString))
            { return; }
            else if (incomeCategoryList.Contains(inputString, StringComparer.OrdinalIgnoreCase))
            {
                ContentDialog Duplicate_CategoryDialog = new ContentDialog
                {
                    Title = "Duplicate Category Name",
                    Content = "A category with the name " + inputString + " already exists. Please choose a different name and try again.",
                    CloseButtonText = "Ok"
                };

                ContentDialogResult result = await Duplicate_CategoryDialog.ShowAsync();
                return;
            }
            
            DatagridFactory datagridFactory = new DatagridFactory();
            incomePivot.Items.Add(new PivotItem
            {
                Header = inputString,
                Content = datagridFactory.DatagridFactory_Make(inputString, 0, bookKeepingContext, In_MouseUp, Income_Datagrid_Sorting)
            });

            //IncomeEntry incomeEntry = new IncomeEntry
            //{
            //    IncomeAmount = 0,
            //    //CheckNumber = int.Parse(AddIncomeCheckNumberTextBox.Text),
            //    DatePaid = DateTime.Now,
            //    IncomeCategory = inputString,
            //    Month = DateTime.Now.Month,
            //    TimeStamp = DateTime.Now,
            //    EntryNotes = "Edit first entry",
            //    IncomeTotal = TotalIncome
            //};

            //using (var context = new BookKeepingContext())
            //{
            //    //context.IncomeDBSet.Load();
            //    try
            //    {
            //        context.IncomeDBSet.Add(incomeEntry);
            //        context.SaveChanges();
            //        await context.IncomeDBSet.LoadAsync();
            //        currentIncomeDatagrid.ItemsSource = context.IncomeDBSet.Where(ee => ee.IncomeCategory == inputString);

            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}
            incomePivot.SelectedIndex = incomePivot.Items.Count - 1;
            Income_OptionBar_EditButton.IsEnabled = false;
            RefreshIncomeDatagrid(inputString, currentIncomeDatagrid);


        }
        public static async Task<string> InputStringDialogAsync(string title, string defaultText, string okButtonText, string cancelButtonText)
        {
            var inputTextBox = new TextBox
            {
                AcceptsReturn = false,
                Height = 32,
                Text = defaultText,
                SelectionStart = defaultText.Length,
                BorderThickness = new Thickness(1),
                // BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["CustomDialogBorderColor"])
            };
            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = okButtonText,
                SecondaryButtonText = cancelButtonText
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return inputTextBox.Text;
            }
            else
            {
                return string.Empty;
            }
        }
        private async void Expense_OptionBar_DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog Expense_DeleteEntry_Dialog = new ContentDialog
            {
                Title = "Delete Entry Permanently?",
                Content = "This will permanently remove the selected entry, are you sure you want to continue?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };


            ContentDialogResult result = await Expense_DeleteEntry_Dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                PivotItem pivotItem = new PivotItem();
                pivotItem = expensePivot.Items[expensePivot.SelectedIndex] as PivotItem;
                ExpenseProgressRing.IsActive = true;
                try
                {


                    using (var db = new BookKeepingContext())
                    {
                        await db.ExpenseDBSet.LoadAsync();
                        ExpenseEntry expEntry = currentExpenseDatagrid.SelectedItem as ExpenseEntry;
                        var expense = db.ExpenseDBSet.Single(b => b.ExpenseId == expEntry.ExpenseId);
                        db.ExpenseDBSet.Remove(expense);
                        db.SaveChanges();
                        //expenseCategoryList = db.ExpenseDBSet.Select(expense => expense.ExpenseCategory).Distexpt().Cast<string>();

                        currentExpenseDatagrid.ItemsSource = db.ExpenseDBSet.Where(i => i.ExpenseCategory.Contains(pivotItem.Header.ToString())).Where(d => (d.DatePaid.Date.Year == Expense_From_DatePicker.Date.Year)).OrderBy(c => c.DatePaid).ToList();

                        currentExpenseDatagrid.SelectedIndex = 0;
                        Expense_OptionBar_DeleteButton.IsEnabled = false;
                    }
                }
                catch (Exception)
                {

                    ContentDialog Expense_ErrorDeleting_Dialog = new ContentDialog
                    {
                        Title = "Unexpected Error",
                        Content = "Unexpected error occured while attempting to delete the entry. Please try again or contact technical support.",
                        CloseButtonText = "Ok"
                    };

                    ContentDialogResult errorResult = await Expense_ErrorDeleting_Dialog.ShowAsync();
                }

                ExpenseProgressRing.IsActive = false;
            }
            else
            {
                Expense_OptionBar_DeleteButton.IsEnabled = false;
                return;
            }


        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {            //using (var db = new BookKeepingContext())
                     //{
                     //    await db.ExpenseDBSet.LoadAsync();
                     //    this.ViewModel.TotalsList = db.IncomeDBSet.GroupBy(o => new { o.IncomeCategory })
                     //        .Select(g => new
                     //        {
                     //            g.Key.IncomeCategory,
                     //            Sum = g.Sum(o => o.IncomeAmount)
                     //        }.ToString()).ToList<string>();
                     //}


        }

        private void TotalIncomeCategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }


}
