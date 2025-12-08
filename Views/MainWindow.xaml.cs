using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
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
    // This is set to possibly nullable because an error may be thrown during constructing due to missing city/state list
    readonly MainWindowViewModel? _vm;

    public MainWindow()
    {
        InitializeComponent();
        // Putting this in a try/catch so that we can check if the City file list is missing. It needs to load a different view because the ErrorMessage
        // for normal exceptions is stored in the ViewModel, which is what is failing to load if there is no text file. So this error page has a static message
        try
        {
            this._vm = new MainWindowViewModel();
            // Binds the city list to the ComboBox's autocomplete list
            CityStateInput.ItemsSource = _vm.Cities;
            // Load the welcome screen telling user to enter their location
            WelcomeView welcomeView = new WelcomeView();
            contentBox.Content = welcomeView;
            DataContext = _vm;
        } 
        catch (ElevatedException)
        {
            // Elevated exceptions are to push errors catch nested in the ViewModel's logic that need to bubble up
            DisplayFileErrorView();
        }
        this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        // This keeps the Search button disabled until they start typing and are forced to make a valid selection, handled by CityStateInput_SelectionChanged
        SearchButton.IsEnabled = false;
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // ViewModel handles the search and load the week's forecast object in the ViewModel, then load the view after the data is ready
           if (_vm != null)
            {
                _vm.HandleSearch(CityStateInput.Text);
                DisplayForecastView();
            }
        }
        catch (CoordsNotFoundException)
        {
            DisplayErrorView();
        }
        catch (ArgumentException)
        {
            DisplayErrorView();
        }
        catch (NetworkException)
        {
            if (_vm != null)
            { 
                _vm.ErrorMessage = "Unable to retrieve forecast, please check your network connection and try again";
                DisplayErrorView();
            }
        }
    }

    // Opens users browser to open-meteo.com when they click the hyperlink in footer
    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
            Process.Start(new ProcessStartInfo("https://open-meteo.com") { UseShellExecute = true });
    }

    // Will keep the search button disabled until valid search input is added. This has an interesting characteristic of keeping the button disabled
    // until the user not only puts in text, but that it matches one of the ones in the list. The button is disabled if there is not a comma between city,state
    // This does have some slightly add beheavior though, may change this based on feedback and just use the errors in place to guide user action
    private void CityStateInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SearchButton.IsEnabled = CityStateInput.SelectedItem != null;
    }

    private void DisplayForecastView()
    {
        UserControl forecast = new ForecastView();        
        contentBox.Content = forecast;
    }

    private void DisplayErrorView()
    {
        UserControl errorView = new ErrorView();
        contentBox.Content = errorView;
    }

    private void DisplayFileErrorView()
    {
        UserControl fileErrorView = new FileErrorView();
        contentBox.Content = fileErrorView;
    }

    // All of the code below is related to the ?/min/max/close windows buttons in the top right & the drag to move behavior for whole window
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

    private void btnAbout_Click(object sender, RoutedEventArgs e)
    {
        UserControl aboutView = new AboutView();
        contentBox.Content = aboutView;
    }
}