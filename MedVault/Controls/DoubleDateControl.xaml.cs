using System.Data;
using System.Globalization;
using System.Windows.Controls;
using MedVault.Enums;
using MedVault.Scripts;

namespace MedVault.Controls;

public partial class DoubleDateControl : BaseTextBox
{
    private CustomTextBox date1;
    private CustomTextBox date2;
    
    public delegate void UpdatePeriodHandler();
    
    public DoubleDateControl()
    {
        InitializeComponent();
        
        AddControls();
    }

    private void AddControls()
    {
        UpdatePeriodHandler updatePeriodHandler = UpdatePeriod;
        
        date1 = new CustomTextBox("Начало приема", "дд.мм.гггг", false, TextBoxTypes.Date, updatePeriodHandler);
        date2 = new CustomTextBox("Конец приема", "дд.мм.гггг", false, TextBoxTypes.Date, updatePeriodHandler);
        
        Grid.SetRow(date1, 0);
        Grid.SetColumn(date1, 0);
        
        Grid.SetRow(date2, 0);
        Grid.SetColumn(date2, 2);
        
        MainGrid.Children.Add(date1);
        MainGrid.Children.Add(date2);
    }

    private void UpdatePeriod()
    {
        string format = "dd.MM.yyyy"; 
        CultureInfo provider = CultureInfo.InvariantCulture;

        try
        {
            DateTime formatDate1 = DateTime.ParseExact(date1.GetResult(), format, provider);
            DateTime formatDate2 = DateTime.ParseExact(date2.GetResult(), format, provider);

            TimeSpan difference = formatDate2 - formatDate1;
        
            int totalDays = (int)difference.TotalDays + 1;
            string result;

            if (totalDays < 0)
                throw new DataException();
            
            if (totalDays < 30)
                result = totalDays == 1 ? "1 день" : $"{totalDays} дней";
            else if (totalDays < 365)
            {
                int weeks = totalDays / 7;
                result = $"{weeks} недели";
            }
            else
            {
                int months = (formatDate2.Year - formatDate1.Year) * 12 + formatDate2.Month - formatDate1.Month;
                result = months == 1 ? "1 месяц" : $"{months} месяца(ев)";
            }

            Period.Text = result;
        }
        catch
        {
            Period.Text = "неопределено";
        }
    }

    public DoubleValues GetValues()
    {
        DoubleValues values = new DoubleValues
        {
            value1 = date1.GetResult(),
            value2 = date2.GetResult(),
        };

        return values;
    }
}