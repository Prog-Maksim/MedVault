using MedVault.Controls;

namespace MedVault.Frames;

public partial class MainMenu
{
    private string[] buttonsText = { "Главная", "Добавить", "Поиск", "Статистика" }; 
    
    public MainMenu()
    {
        InitializeComponent();
        
        CreateButtons();
    }

    private void CreateButtons()
    {
        foreach (var item in buttonsText)
        {
            ButtonMainMenu button = new ButtonMainMenu(item);
            StackPanelMenuButtons.Children.Add(button);
        }
        
    }
}