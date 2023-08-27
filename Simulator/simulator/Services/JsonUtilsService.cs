using Newtonsoft.Json;
using Simulator.simulator.Models;
using System.Collections.Generic;
using System.IO;


namespace Simulator.simulator.Services
{
    public class JsonUtilsService
    {
        public IEnumerable<IcdParameter> DeserializeIcdFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "simulator/ConfigDocuments/FlightBox.json");
            string jsonText = File.ReadAllText(path);                        
            var deserializedList = JsonConvert.DeserializeObject<IcdParameter[]>(jsonText);
            return deserializedList;
        }
    }
}
