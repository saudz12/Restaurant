using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant;

public class AppSettingsService
{
    private readonly string _settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RestaurantApp",
        "settings.json");

    private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented
    };

    public AppSettings LoadSettings()
    {
        if (!File.Exists(_settingsPath))
            return new AppSettings();

        string json = File.ReadAllText(_settingsPath);
        return JsonConvert.DeserializeObject<AppSettings>(json, _jsonSettings);
    }

    public void SaveSettings(AppSettings settings)
    {
        string directoryPath = Path.GetDirectoryName(_settingsPath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string json = JsonConvert.SerializeObject(settings, _jsonSettings);
        File.WriteAllText(_settingsPath, json);
    }
}

public class AppSettings
{
    public string Theme { get; set; } = "Light";
    public double MenuDiscount { get; set; } = 0.1;
    public string DefaultLanguage { get; set; } = "ro-RO";
    public bool ShowAvailableItemsOnly { get; set; } = false;
}