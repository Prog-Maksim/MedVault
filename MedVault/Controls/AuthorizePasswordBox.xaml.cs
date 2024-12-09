using System.Windows.Controls;
using MedVault.Scripts;

namespace MedVault.Controls;

public partial class AuthorizePasswordBox: BaseTextBox
{
    public AuthorizePasswordBox()
    {
        InitializeComponent();
    }
    
    public AuthorizePasswordBox(string description)
    {
        InitializeComponent();
        
        PasswordBox.Tag = description;
    }

    public string GetResult()
    {
        return PasswordBox.Password;
    }
}