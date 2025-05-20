using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant;

public class ConfigService
{
    private readonly string _configPath = "appsettings.json";
    private JObject _configObject;

    public ConfigService()
    {
        if (File.Exists(_configPath))
        {
            string json = File.ReadAllText(_configPath);
            _configObject = JObject.Parse(json);
        }
        else
        {
            _configObject = new JObject();
        }
    }

    public string GetConnectionString()
    {
        return _configObject["ConnectionStrings"]?["RestaurantDatabase"]?.ToString();
    }

    public double GetMenuDiscount()
    {
        var discountToken = _configObject["MenuDiscount"];
        if (discountToken != null && double.TryParse(discountToken.ToString(), out double discount))
            return discount;

        return 0.1; // Default discount
    }
}
