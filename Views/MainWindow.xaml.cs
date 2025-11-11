using System.Windows;
using System.Runtime.InteropServices;
using Veour.ViewModel;
using System.Windows.Interop;

namespace Veour;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    readonly MainWindowViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        this._vm = new MainWindowViewModel();
        DataContext = _vm;
        CityStateInput.ItemsSource = _vm.Cities;
        this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

    }


    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        _vm.HandleSearch(CityStateInput.Text);
    }


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