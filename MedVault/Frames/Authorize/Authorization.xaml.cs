using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MedVault.Controls;
using MedVault.Enums;
using MedVault.Models.Request;
using MedVault.Scripts;

namespace MedVault.Frames.Authorize;

public partial class Authorization
{
    private AuthorizeType _authType = AuthorizeType.Registration;
    private Dictionary<TextBoxTypes, BaseTextBox> _textBoxes = new();
    private MainWindow.OpenWindow _handler;
    
    public  Authorization()
    {
        InitializeComponent();
        
        AddRegistrationFrame();
    }

    public Authorization(MainWindow.OpenWindow handler): this()
    {
        _handler = handler;
    }

    private void Authorize_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_authType == AuthorizeType.Registration)
        {
            AddAuthorizationFrame();
            _authType = AuthorizeType.Authorization;
        }
        else if (_authType == AuthorizeType.Authorization)
        {
            AddRegistrationFrame();
            _authType = AuthorizeType.Registration;
        }
    }
    
    private void SetTemporaryText(string message)
    {
        ErrorTextBlock.Text = message;
        
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };
        
        timer.Tick += (_, _) =>
        {
            ErrorTextBlock.Text = string.Empty;
            timer.Stop();
        };

        timer.Start();
    }
    
    public delegate void TextChangedEventHandler(string message);
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        TextChangedEventHandler eventHandler = SetTemporaryText;
        
        if (_authType == AuthorizeType.Registration)
        {
            string[] names = _textBoxes[TextBoxTypes.Name].GetResult().Split(' ');

            if (names.Length != 3)
            {
                eventHandler("Некорректный формат ввода ФИО.");
                return;
            }
            
            string format = "dd.MM.yyyy"; 
            CultureInfo provider = CultureInfo.InvariantCulture;
            string input = _textBoxes[TextBoxTypes.Age].GetResult();

            if (DateTime.TryParseExact(input, format, provider, DateTimeStyles.None, out DateTime date))
            {
                RegisrationModel model = new RegisrationModel
                {
                    Name = names[1],
                    Surname = names[0],
                    Patronymic = names[2],
                    Birthday = date,
                    Email = _textBoxes[TextBoxTypes.Login].GetResult(),
                    Password = _textBoxes[TextBoxTypes.Password].GetResult()
                };

                _ = RequestHadler.SendRegistrationRequest(model, eventHandler, _handler);
            }
            else
            {
                eventHandler("Неверный формат даты");
            }
        }
        else if (_authType == AuthorizeType.Authorization)
        {
            AuthorizationModel model = new AuthorizationModel
            {
                Email = _textBoxes[TextBoxTypes.Login].GetResult(),
                Password = _textBoxes[TextBoxTypes.Password].GetResult(),
            };
            
            _ = RequestHadler.SendAuthorizationRequest(model, eventHandler, _handler);
        }
    }
    

    private void AddRegistrationFrame()
    {
        ListsTextBox.Children.Clear();
        _textBoxes.Clear();
        
        TitleText.Text = "Регистрация";
        Authorize.Text = "уже есть аккаунт?";

        Thickness thickness = new Thickness(0, 20, 0, 0);
        
        AuthorizeTextBox name = new AuthorizeTextBox("ФИО");
        _textBoxes.Add(TextBoxTypes.Name, name);
        AuthorizeTextBox age = new AuthorizeTextBox("Возраст")
        {
            Margin = thickness
        };
        _textBoxes.Add(TextBoxTypes.Age, age);
        AuthorizeTextBox login = new AuthorizeTextBox("Логин")
        {
            Margin = thickness
        };
        _textBoxes.Add(TextBoxTypes.Login, login);
        AuthorizePasswordBox password = new AuthorizePasswordBox("Пароль")
        {
            Margin = thickness
        };
        _textBoxes.Add(TextBoxTypes.Password, password);

        ListsTextBox.Children.Add(name);
        ListsTextBox.Children.Add(age);
        ListsTextBox.Children.Add(login);
        ListsTextBox.Children.Add(password);
    }

    private void AddAuthorizationFrame()
    {
        ListsTextBox.Children.Clear();
        _textBoxes.Clear();
        
        TitleText.Text = "Авторизация";
        Authorize.Text = "еще нет аккаунта?";
        
        AuthorizeTextBox login = new AuthorizeTextBox("Логин");
        _textBoxes.Add(TextBoxTypes.Login, login);
        AuthorizePasswordBox password = new AuthorizePasswordBox("Пароль")
        {
            Margin = new Thickness(0, 20, 0, 0)
        };
        _textBoxes.Add(TextBoxTypes.Password, password);

        ListsTextBox.Children.Add(login);
        ListsTextBox.Children.Add(password);
    }
}