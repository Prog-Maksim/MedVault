using MedVault.Scripts;

namespace MedVault.Controls;

public partial class PriceTextBox : BaseTextBox
{
    public PriceTextBox()
    {
        InitializeComponent();
    }

    public PriceTextBox(string title, string description, string? period = null): this()
    {
        Title.Text = title;
        TextBox.Tag = description;

        if (period == null)
            Period.Width = 0;

        Period.Text = period;
    }
    
    public string GetResult()
    {
        if (MainCheckBox.IsChecked == true)
            return TextBox.Text;
        return string.Empty;
    }
}