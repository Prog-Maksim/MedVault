using System.Windows.Controls;
using MedVault.Scripts;

namespace MedVault.Controls;

public partial class DoublePriceControl : BaseTextBox
{
    private PriceTextBox price1;
    private PriceTextBox price2;
    
    public DoublePriceControl()
    {
        InitializeComponent();
        
        AddControls();
    }
    
    private void AddControls()
    {
        price1 = new PriceTextBox("Стоимость мед.услуги \u20bd", "1 000", "от");
        price2 = new PriceTextBox("", "10 000", "до");
        
        Grid.SetRow(price1, 0);
        Grid.SetColumn(price1, 0);
        
        Grid.SetRow(price2, 0);
        Grid.SetColumn(price2, 2);
        
        MainGrid.Children.Add(price1);
        MainGrid.Children.Add(price2);
    }

    public DoubleValues GetValues()
    {
        DoubleValues values = new DoubleValues
        {
            value1 = price1.GetResult(),
            value2 = price2.GetResult(),
        };

        return values;
    }
}

public class DoubleValues
{
    public string value1 { get; set; }
    public string value2 { get; set; }
}