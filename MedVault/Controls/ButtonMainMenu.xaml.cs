using System.Windows;

namespace MedVault.Controls;

public partial class ButtonMainMenu
{
    public string buttonText { get; set; } = String.Empty;
    
    public ButtonMainMenu()
    {
        InitializeComponent();

        DataContext = this;
    }

    public ButtonMainMenu(string buttonText): this()
    {
        this.buttonText = buttonText;
    }

    private void BaseButton_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}