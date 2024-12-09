using System.IO;
using System.Text.Json;
using System.Windows;
using MedVault.Models.DataWarehouseModel;

namespace MedVault.Scripts;

public class FileSystem
{
    public static async Task<bool> SaveAuthToFileAsync(Auth auth)
    {
        try
        {
            var json = JsonSerializer.Serialize(auth, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(Variables.AuthFilePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении Auth в файл: {ex.Message}");
            return false;
        }

        return true;
    }

    public static async Task<Auth> GetAuthData()
    {
        string text = await File.ReadAllTextAsync(Variables.AuthFilePath);
        Auth? json = JsonSerializer.Deserialize<Auth>(text);

        if (json is null)
            throw new JsonException("Невозможно преобразовать json");

        return json;
    }
}