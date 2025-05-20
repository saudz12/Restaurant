using Database.Services.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database;

public class DataExportService
{
    public async Task ExportMenusAsync(List<MenuDto> menus, string filePath)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(menus, settings);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<List<MenuDto>> ImportMenusAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return new List<MenuDto>();

        string json = await File.ReadAllTextAsync(filePath);

        var settings = new JsonSerializerSettings
        {
            Error = (sender, args) =>
            {
                args.ErrorContext.Handled = true;
                // Log error
            }
        };

        return JsonConvert.DeserializeObject<List<MenuDto>>(json, settings) ?? new List<MenuDto>();
    }
}