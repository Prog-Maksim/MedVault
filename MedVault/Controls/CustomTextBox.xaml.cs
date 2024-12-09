using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MedVault.Enums;
using MedVault.Scripts;

namespace MedVault.Controls;

public partial class CustomTextBox : BaseTextBox
{
    public CustomTextBox()
    {
        InitializeComponent();
    }

    public CustomTextBox(string title, string description, bool state, TextBoxTypes type): this()
    {
        Title.Text = title;
        TextBox.Tag = description;
        
        if (type == TextBoxTypes.Date)
        {
            ComboBox.Visibility = Visibility.Hidden;
            TextBox.MaxLength = 10;

            TextBox.PreviewTextInput += TextBox_OnPreviewTextInput;
        }
        else
        {
            MainBorder.Width = 200;
        }

        if (state)
        {
            MainCheckBox.IsChecked = false;
            MainCheckBox.IsEnabled = false;
            MainCheckBox.Width = 0;
        }
    }

    public CustomTextBox(string title, string description, bool state, TextBoxTypes type, DoubleDateControl.UpdatePeriodHandler handler): this(title, description, state, type)
    {
        TextBox.TextChanged += (_, _) => handler();
        MainCheckBox.Checked += (_, _) => handler();
        MainCheckBox.Unchecked += (_, _) => handler();
    }

    private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (!Char.IsDigit(e.Text[0]))
        {
            e.Handled = true;
            return; 
        }

        var textBox = sender as TextBox;
        if (textBox == null) return;

        string currentText = textBox.Text;

        if (currentText.Length == 1 || currentText.Length == 4)
        {
            textBox.Text = currentText + e.Text + ".";
            textBox.CaretIndex = textBox.Text.Length;
            e.Handled = true;
        }
    }


    public string GetResult()
    {
        if (MainCheckBox.IsChecked == true)
            return TextBox.Text;
        return String.Empty;
    }
}