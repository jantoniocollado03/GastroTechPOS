using Avalonia.Controls;
using GastroTechPOS.UI.ViewModels;

namespace GastroTechPOS.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
