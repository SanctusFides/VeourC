using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Veour.Models;
using Veour.Services;
using Veour.ViewModel;

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
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {

    }
}