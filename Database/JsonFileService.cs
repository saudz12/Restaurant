using Database.Services.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database;

public class JsonFileService
{
    public void SaveMenuToFile(MenuDto menu, string filePath)
    {
        string json = JsonConvert.SerializeObject(menu, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public void SavePreparateToFile(List<PreparatDto> preparate, string filePath)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(preparate, settings);
        File.WriteAllText(filePath, json);
    }

    public MenuDto LoadMenuFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<MenuDto>(json);
    }

    public List<PreparatDto> LoadPreparateFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return new List<PreparatDto>();

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<PreparatDto>>(json);
    }
}