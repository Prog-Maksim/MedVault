using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MedVault.Controls;

public partial class FoundDocument
{
    private bool state;
    private BrushConverter converter = new ();
    
    public FoundDocument()
    {
        InitializeComponent();
    }
    
    public FoundDocument(int number): this()
    {
        TextBlock.Text = number.ToString();
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Border border = (Border)sender;
        
        if (state)
        {
            border.Background = (Brush)converter.ConvertFromString("#E2E5F3");
            TextBlock.Foreground = (Brush)converter.ConvertFromString("#7927E0");
        }
        else
        {
            border.Background = (Brush)converter.ConvertFromString("#F3F5FF");
            TextBlock.Foreground = Brushes.Gray;
        }
        
        state = !state;
    }
}