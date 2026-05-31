using Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;


namespace Model.Data
{
    public class JsonDataService : DataService
    {
        public override void Save<T>(string path, T data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public override T Load<T>(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}