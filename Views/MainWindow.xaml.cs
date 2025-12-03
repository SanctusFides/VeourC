using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Veour.Exceptions;
using Veour.ViewModels;

namespace Veour.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// MainWindowViewModel is set as the Datacontext, where all the forecast building logic is contained
/// Only functionality contained in here is related to button handling and view changes
/// </summary>
public partial class MainWindow : Window
{

    readonly MainWindowViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        this._vm = new MainWindowViewModel();
        DataContext = _vm;

        // Binds the city list to the ComboBox's autocomplete list
        CityStateInput.ItemsSource = _vm.Cities;
        this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

        // Load the welcome screen telling user to enter their location
        WelcomeView welcomeView = new WelcomeView();
        contentBox.Content = welcomeView;
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Handle the search and load the week's forecast object in the ViewModel, then load the view after the data is ready
            _vm.HandleSearch(CityStateInput.Text);
            DisplayForecastView();
        }
        catch (CoordsNotFoundException)
        {
            DisplayErrorView();
        }
        catch (ArgumentException)
        {
            DisplayErrorView();
        }
    }

    private void DisplayForecastView()
    {
        UserControl forecast = new ForecastView();        
        contentBox.Content = forecast;
    }

    private void DisplayErrorView()
    {
        Debug.WriteLine("Inside Display Error");
        UserControl errorView = new ErrorView();
        contentBox.Content = errorView;
    }

    // All of the code below is related to the min/max/close windows buttons in the top right & the drag to move behavior for whole window
    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
    private void PnlControlBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        WindowInteropHelper helper = new WindowInteropHelper(this);
        SendMessage(helper.Handle, 161, (IntPtr)2, IntPtr.Zero);
    }

    private void PnlControlBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void BtnMaximize_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
        }
        else
        {
            this.WindowState = WindowState.Normal;
        }
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
}