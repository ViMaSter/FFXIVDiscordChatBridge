using FFXIVHelpers.Models;

namespace FFXIVHelpers.Persistence;

public interface IPersistence
{
    List<Mapping> LoadMappings();
    void WriteMappingsToFile(List<Mapping> mappings);
}