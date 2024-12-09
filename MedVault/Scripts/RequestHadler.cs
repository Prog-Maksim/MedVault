using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MedVault.Controls;
using MedVault.Frames.Authorize;
using MedVault.Models.DataWarehouseModel;
using MedVault.Models.Response.Error;
using MedVault.Models.Request;
using MedVault.Models.Response;

namespace MedVault.Scripts;

public class RequestHadler
{
    public static async Task SendRegistrationRequest(RegisrationModel registration, Authorization.TextChangedEventHandler updateText, MainWindow.OpenWindow openWindow)
    {
        var url = Variables.BaseUrl + "/auth/registration/user";
        
        using var client = new HttpClient();
        
        var json = JsonSerializer.Serialize(registration);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try
        {
            using var response = await client.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var validationError = JsonSerializer.Deserialize<ValidationErrorResponse>(responseBody);
                        var errorDetails = string.Join("\n", validationError.errors.SelectMany(e => e.Value));
                        updateText(errorDetails);
                    }
                    else
                    {
                        var generalError = JsonSerializer.Deserialize<GeneralErrorResponse>(responseBody);
                        updateText(generalError.message);
                    }
                }
                catch (JsonException)
                {
                    updateText("Неизвестная ошибка. попробуйте позже.");
                }
                return;
            }
            
            var registrationResponse = JsonSerializer.Deserialize<Models.Response.AuthResponse>(responseBody);

            Auth authData = new Auth
            {
                AccessToken = registrationResponse.access_token,
                RefreshToken = registrationResponse.refresh_token,
            };
            
            bool result = await FileSystem.SaveAuthToFileAsync(authData);
            if (!result)
            {
                updateText("Невозможно обновить данные!");
                return;
            }
            openWindow();
        }
        catch (HttpRequestException ex)
        {
            updateText("Ошибка связи с сервером. Попробуйте позже.");
        }
        catch (JsonException ex)
        {
            updateText("Неизвестная ошибка. попробуйте позже.");
        }
    }

    public static async Task SendAuthorizationRequest(AuthorizationModel authorization, Authorization.TextChangedEventHandler updateText, MainWindow.OpenWindow openWindow)
    {
        var url = Variables.BaseUrl + "/auth/authorization/user";
        
        using var client = new HttpClient();
        
        var json = JsonSerializer.Serialize(authorization);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try
        {
            using var response = await client.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var validationError = JsonSerializer.Deserialize<ValidationErrorResponse>(responseBody);
                        var errorDetails = string.Join("\n", validationError.errors.SelectMany(e => e.Value));
                        updateText(errorDetails);
                    }
                    else
                    {
                        var generalError = JsonSerializer.Deserialize<GeneralErrorResponse>(responseBody);
                        updateText(generalError.message);
                    }
                }
                catch (JsonException)
                {
                    updateText("Неизвестная ошибка, попробуйте позже");
                }
                return;
            }
            
            var registrationResponse = JsonSerializer.Deserialize<Models.Response.AuthResponse>(responseBody);

            Auth authData = new Auth
            {
                AccessToken = registrationResponse.access_token,
                RefreshToken = registrationResponse.refresh_token,
            };
            
            bool result = await FileSystem.SaveAuthToFileAsync(authData);
            if (!result)
            {
                updateText("Невозможно обновить данные!");
                return;
            }
            openWindow();
        }
        catch (HttpRequestException ex)
        {
            updateText("Ошибка связи с сервером. Попробуйте позже.");
        }
        catch (JsonException ex)
        {
            updateText("Неизвестная ошибка. попробуйте позже.");
        }
    }

    public static async Task<Auth> SendRefreshTokenRequest(string refreshToken)
    {
        var url = Variables.BaseUrl + "/auth/refresh-token";
        
        using var client = new HttpClient();
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
        HttpContent content = new StringContent(string.Empty);
        using var response = await client.PutAsync(url, content);
        Console.WriteLine(response.StatusCode);

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        
        var registrationResponse = JsonSerializer.Deserialize<Models.Response.AuthResponse>(responseBody);

        Auth authData = new Auth
        {
            AccessToken = registrationResponse.access_token,
            RefreshToken = registrationResponse.refresh_token,
        };

        return authData;
    }
    
    /// <summary>
    /// Проверяет авторизационные данные
    /// </summary>
    /// <returns>статус авторизации</returns>
    public static async Task<bool> VerificationData()
    {
        try
        {
            Auth tokenAuth = await FileSystem.GetAuthData();
        
            try
            {
                var url = Variables.BaseUrl + "/auth/check-auth";
                var token = tokenAuth.AccessToken;
        
                using var client = new HttpClient();
        
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Auth result = await SendRefreshTokenRequest(tokenAuth.RefreshToken);
                await FileSystem.SaveAuthToFileAsync(result);
            }
        }
        catch
        {
            return false;
        }

        return true;
    }


    private static async Task<string> GetAccessToken()
    {
        // TODO: доделать обработку если авторизация не удалась
        await VerificationData();
        Auth tokenAuth = await FileSystem.GetAuthData();
        return tokenAuth.AccessToken;
    }

    public static async Task SendRequestAddDocument(AddDocument document, Authorization.TextChangedEventHandler errorText)
    {
        var url = Variables.BaseUrl + "/document/add-document";
        
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
        
        var json = JsonSerializer.Serialize(document);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try
        {
            using var response = await client.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var validationError = JsonSerializer.Deserialize<ValidationErrorResponse>(responseBody);
                        var errorDetails = string.Join("\n", validationError.errors.SelectMany(e => e.Value));
                        errorText(errorDetails);
                    }
                    else
                    {
                        var generalError = JsonSerializer.Deserialize<GeneralErrorResponse>(responseBody);
                        errorText(generalError.message);
                    }
                }
                catch (JsonException ex)
                {
                    errorText("Неизвестная ошибка. попробуйте позже.");
                }
                return;
            }
            
            DocumentId? addDocumentResponse = JsonSerializer.Deserialize<DocumentId>(responseBody);
            MessageBox.Show($"Уникальный идентификатор документа: {addDocumentResponse.documentId}", "MedVault", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }
        catch (HttpRequestException ex)
        {
            errorText("Ошибка связи с сервером. Попробуйте позже.");
        }
        catch (JsonException ex)
        {
            errorText("Неизвестная ошибка. попробуйте позже.");
        }
    }


    public static async Task SendRequestGetNumDocuments(Run text, Authorization.TextChangedEventHandler errorText)
    {
        var url = Variables.BaseUrl + "/document/get-num-documents";
        
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
        
        
        try
        {
            using var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var validationError = JsonSerializer.Deserialize<ValidationErrorResponse>(responseBody);
                        var errorDetails = string.Join("\n", validationError.errors.SelectMany(e => e.Value));
                        errorText(errorDetails);
                    }
                    else
                    {
                        var generalError = JsonSerializer.Deserialize<GeneralErrorResponse>(responseBody);
                        errorText(generalError.message);
                    }
                }
                catch (JsonException ex)
                {
                    errorText("Неизвестная ошибка. попробуйте позже.");
                }
                return;
            }
            
            text.Text = responseBody;
            
        }
        catch (HttpRequestException ex)
        {
            errorText("Ошибка связи с сервером. Попробуйте позже.");
        }
        catch (JsonException ex)
        {
            errorText("Неизвестная ошибка. попробуйте позже.");
        }

    }
    
    
    public static async Task SendRequestSearchDocument(SearchDocument document, Run text, Authorization.TextChangedEventHandler errorText,  WrapPanel listObjects)
    {
        var url = Variables.BaseUrl + "/document/search-document?";

        if (document.DocumentType is not null)
            url += $"documentType={document.DocumentType}";
        if (document.DateStart is not null)
            url += $"&dateStart={document.DateStart}";
        if (document.DateEnd is not null)
            url += $"&dateEnd={document.DateEnd}";
        if (document.DoctorName is not null)
            url += $"&doctorName={document.DoctorName}";
        if (document.DoctorSpeciality is not null)
            url += $"&doctorSpecialty={document.DoctorSpeciality}";
        if (document.Analyzes is not null)
            url += $"&analyzes={document.Analyzes}";
        if (document.PriceStart is not null)
            url += $"&priceStart={document.PriceStart}";
        if (document.PriceEnd is not null)
            url += $"&priceEnd={document.PriceEnd}";
        
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
        
        var json = JsonSerializer.Serialize(document);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try
        {
            using var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var validationError = JsonSerializer.Deserialize<ValidationErrorResponse>(responseBody);
                        var errorDetails = string.Join("\n", validationError.errors.SelectMany(e => e.Value));
                        errorText(errorDetails);
                    }
                    else
                    {
                        var generalError = JsonSerializer.Deserialize<GeneralErrorResponse>(responseBody);
                        errorText(generalError.message);
                    }
                }
                catch (JsonException ex)
                {
                    errorText("Неизвестная ошибка. попробуйте позже.");
                }
                return;
            }
            
            var searchDocument = JsonSerializer.Deserialize<List<DocumentResponse>>(responseBody);
            text.Text = searchDocument.Count.ToString();
            AddDocumentToWrapPanel(searchDocument, listObjects);
        }
        catch (HttpRequestException ex)
        {
            errorText("Ошибка связи с сервером. Попробуйте позже.");
        }
        catch (JsonException ex)
        {
            errorText("Неизвестная ошибка. попробуйте позже.");
        }
    }

    private static void AddDocumentToWrapPanel(List<DocumentResponse> documents, WrapPanel listObjects)
    {
        listObjects.Children.Clear();
        
        foreach (var item in documents)
        {
            FoundDocument doc = new FoundDocument(item.documentId);
            listObjects.Children.Add(doc);
        }
    }
}