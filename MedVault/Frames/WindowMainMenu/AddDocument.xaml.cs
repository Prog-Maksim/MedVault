using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MedVault.Enums;
using MedVault.Controls;
using MedVault.Frames.Authorize;
using MedVault.Scripts;

namespace MedVault.Frames.WindowMainMenu;

public partial class AddDocument : Page
{
    private Dictionary<TextBoxTypes, BaseTextBox> _textBoxes = new();
    
    public AddDocument()
    {
        InitializeComponent();
        
        AddControls();
    }

    private void AddControls()
    {
        CustomTextBox date = new CustomTextBox("Дата приема", "дд.мм.гггг", false, TextBoxTypes.Date);
        date.Margin = new Thickness(0, 20, 20, 0);
        _textBoxes[TextBoxTypes.Date] = date;
        
        CustomTextBox name = new CustomTextBox("Имя пациента", "Белоглазов М.В.", true, TextBoxTypes.Name);
        name.Margin = new Thickness(0, 20, 20, 0);
        _textBoxes[TextBoxTypes.Name] = name;
        
        CustomTextBox doctor = new CustomTextBox("Имя доктора", "Фамилия И.О.", false, TextBoxTypes.Name);
        doctor.Margin = new Thickness(0, 20, 20, 0);
        _textBoxes[TextBoxTypes.Name] = doctor;
        
        CustomTextBox specialization = new CustomTextBox("Специальность врача", "кардиолог", false, TextBoxTypes.Name);
        specialization.Margin = new Thickness(0, 20, 20, 0);
        _textBoxes[TextBoxTypes.Specialization] = specialization;
        
        CustomTextBox type = new CustomTextBox("Тип документа", "эпикриз", false, TextBoxTypes.Name);
        type.Margin = new Thickness(0, 20, 20, 0);
        _textBoxes[TextBoxTypes.DocumentType] = type;
        
        PriceTextBox price = new PriceTextBox("Стоимость мед. услуги", "10 000");
        price.Margin = new Thickness(0, 20, 20, 0);
        _textBoxes[TextBoxTypes.Price] = price;

        MainSection.Children.Add(date);
        MainSection.Children.Add(name);
        MainSection.Children.Add(doctor);
        MainSection.Children.Add(specialization);
        MainSection.Children.Add(type);
        MainSection.Children.Add(price);
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        string format = "dd.MM.yyyy";
        CultureInfo provider = CultureInfo.InvariantCulture;
        
        string date = _textBoxes[TextBoxTypes.Date].GetResult();
        string doctorName = _textBoxes[TextBoxTypes.Name].GetResult();
        string doctorSpecialty = _textBoxes[TextBoxTypes.Specialization].GetResult();
        string documentType = _textBoxes[TextBoxTypes.DocumentType].GetResult();
        string priceString = _textBoxes[TextBoxTypes.Price].GetResult();
        
        DateTime? dateAdmission = string.IsNullOrEmpty(date) ? null : DateTime.ParseExact(date, format, provider);
        string? doctorNameValue = string.IsNullOrEmpty(doctorName) ? null : doctorName;
        string? doctorSpecialtyValue = string.IsNullOrEmpty(doctorSpecialty) ? null : doctorSpecialty;
        int? priceValue = string.IsNullOrEmpty(priceString) ? null : Convert.ToInt32(priceString);
        
        Models.Request.AddDocument document = new Models.Request.AddDocument
        {
            DateAdmission = dateAdmission,
            DoctorName = doctorNameValue,
            DoctorSpecialty = doctorSpecialtyValue,
            DocumentType = documentType,
            Price = priceValue
        };

        Authorization.TextChangedEventHandler eventHandler = SetTemporaryText;
        
        _ = RequestHadler.SendRequestAddDocument(document, eventHandler);
    }

    private void SetTemporaryText(string message)
    {
        MessageBox.Show(message);
        // ErrorTextBlock.Text = message;
        //
        // var timer = new DispatcherTimer
        // {
        //     Interval = TimeSpan.FromSeconds(2)
        // };
        //
        // timer.Tick += (s, e) =>
        // {
        //     ErrorTextBlock.Text = string.Empty;
        //     timer.Stop();
        // };
        //
        // timer.Start();
    }
}