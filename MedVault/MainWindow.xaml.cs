using System.Windows;
using MedVault.Frames;
using MedVault.Frames.Authorize;
using MedVault.Scripts;

namespace MedVault;

public partial class MainWindow : Window
{
    public delegate void OpenWindow();
    
    public MainWindow()
    {
        InitializeComponent();
        
        InitializeAsync();
    }
    
    private async void InitializeAsync()
    {
        bool result = await RequestHadler.VerificationData();
        Console.WriteLine(result);

        if (!result)
        {
            MessageBox.Show("Требуется повторно войти в аккаунт!", "MedVault", MessageBoxButton.OK, MessageBoxImage.Information);
            OpenWindow window = OpenMainWindow;
            MainFrame.Content = new Authorization(window);
        }
        else
        {
            MainFrame.Content = new MainMenu();
        }
    }

    private void OpenMainWindow()
    {
        MainFrame.Content = new MainMenu();
    }
}