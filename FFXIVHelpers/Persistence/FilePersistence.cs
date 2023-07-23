using FFXIVHelpers.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace FFXIVHelpers.Persistence;

public class FilePersistence : IPersistence
{
    private const string MAPPING_FILE_NAME = "usernameMappings.json";
    private readonly string _absolutePathToMappingFile = Path.Combine(Directory.GetCurrentDirectory(), MAPPING_FILE_NAME);
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Converters = new List<JsonConverter>
        {
            new StringEnumConverter()
        }
    };
    
    public static void DeleteMappingFile()
    {
        var absolutePathToMappingFile = Path.Combine(Directory.GetCurrentDirectory(), MAPPING_FILE_NAME);
        if (File.Exists(absolutePathToMappingFile))
        {
            File.Delete(absolutePathToMappingFile);
        }
    }
    
    public List<Mapping> LoadMappings()
    {
        if (!File.Exists(_absolutePathToMappingFile))
        {
            return new List<Mapping>();
        }

        var json = File.ReadAllText(_absolutePathToMappingFile);
        return JsonConvert.DeserializeObject<List<Mapping>>(json, _jsonSerializerSettings) ?? new List<Mapping>();
    }

    public void WriteMappingsToFile(List<Mapping> mappings)
    {
        // enum as string
        var json = JsonConvert.SerializeObject(mappings, Formatting.Indented, _jsonSerializerSettings);
        File.WriteAllText(_absolutePathToMappingFile, json);
    }
}