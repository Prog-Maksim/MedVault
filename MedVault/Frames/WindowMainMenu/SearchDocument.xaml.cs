using System.Globalization;
using System.Windows;
using MedVault.Controls;
using MedVault.Enums;
using MedVault.Frames.Authorize;
using MedVault.Scripts;

namespace MedVault.Frames.WindowMainMenu;

public partial class SearchDocument
{
    private Dictionary<TextBoxTypes, BaseTextBox> _textBoxes = new ();
    private Authorization.TextChangedEventHandler handler;
    
    public SearchDocument()
    {
        InitializeComponent();
        
        AddControls();
        
        handler = SetTemporaryText;
        _ = RequestHadler.SendRequestGetNumDocuments(NumberOfDocumentsText, handler);
    }

    private void AddControls()
    {
        DoubleDateControl data = new DoubleDateControl();
        data.Margin = new Thickness(0, 33, 20, 0);
        _textBoxes[TextBoxTypes.Date] = data;
        CustomTextBox doctor = new CustomTextBox("Имя доктора", "Фамилия И.О", false, TextBoxTypes.Name);
        doctor.Margin = new Thickness(0, 20, 20, 0);
        _textBoxes[TextBoxTypes.Name] = doctor;
        CustomTextBox specialization = new CustomTextBox("Специальность врача", "кардиолог", false, TextBoxTypes.Name);
        specialization.Margin = new Thickness(0,20, 20, 0);
        _textBoxes[TextBoxTypes.Specialization] = specialization;
        CustomTextBox type = new CustomTextBox("Тип документа", "эпикриз", false, TextBoxTypes.Name);
        type.Margin = new Thickness(0,20, 20, 0);
        _textBoxes[TextBoxTypes.DocumentType] = type;
        DoublePriceControl price = new DoublePriceControl();
        price.Margin = new Thickness(0,20, 20, 0);
        _textBoxes[TextBoxTypes.Price] = price;
        
        
        MainSection.Children.Add(data);
        MainSection.Children.Add(doctor);
        MainSection.Children.Add(specialization);
        MainSection.Children.Add(type);
        MainSection.Children.Add(price);
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

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        string format = "dd.MM.yyyy";
        CultureInfo provider = CultureInfo.InvariantCulture;
        
        string dateStart = _textBoxes[TextBoxTypes.Date].GetValues().value1;
        string dateEnd = _textBoxes[TextBoxTypes.Date].GetValues().value2;
        string doctorName = _textBoxes[TextBoxTypes.Name].GetResult();
        string doctorSpecialty = _textBoxes[TextBoxTypes.Specialization].GetResult();
        string documentType = _textBoxes[TextBoxTypes.DocumentType].GetResult();
        string priceStart = _textBoxes[TextBoxTypes.Price].GetValues().value1;
        string priceEnd = _textBoxes[TextBoxTypes.Price].GetValues().value2;
        
        DateTime? dateAdmissionStart = string.IsNullOrEmpty(dateStart) ? null : DateTime.ParseExact(dateStart, format, provider);
        DateTime? dateAdmissionEnd = string.IsNullOrEmpty(dateEnd) ? null : DateTime.ParseExact(dateEnd, format, provider);
        string? doctorNameValue = string.IsNullOrEmpty(doctorName) ? null : doctorName;
        string? documentTypeValue = string.IsNullOrEmpty(documentType) ? null : documentType;
        string? doctorSpecialtyValue = string.IsNullOrEmpty(doctorSpecialty) ? null : doctorSpecialty;
        int? priceValueStart = string.IsNullOrEmpty(priceStart) ? null : Convert.ToInt32(priceStart);
        int? priceValueEnd = string.IsNullOrEmpty(priceEnd) ? null : Convert.ToInt32(priceEnd);

        Models.Request.SearchDocument document = new Models.Request.SearchDocument
        {
            DateStart = dateAdmissionStart,
            DateEnd = dateAdmissionEnd,
            DoctorName = doctorNameValue,
            DoctorSpeciality = doctorSpecialtyValue,
            DocumentType = documentTypeValue,
            PriceStart = priceValueStart,
            PriceEnd = priceValueEnd
        };
        
        _ = RequestHadler.SendRequestSearchDocument(document, NumberOfDocumentSearchText, handler, WrapPanelObject);
        
    }
}