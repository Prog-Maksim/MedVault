using MedVault.Scripts;

namespace MedVault.Controls;

public partial class AuthorizeTextBox : BaseTextBox
{
    public AuthorizeTextBox()
    {
        InitializeComponent();
    }
    
    public AuthorizeTextBox(string description)
    {
        InitializeComponent();

        TextBox.Tag = description;
    }

    public string GetResult()
    {
        return TextBox.Text;
    }
}