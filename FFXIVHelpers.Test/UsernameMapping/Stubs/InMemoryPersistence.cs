using FFXIVHelpers.Models;
using FFXIVHelpers.Persistence;

namespace FFXIVHelpers.Test.UsernameMapping.Stubs;

public class InMemoryPersistence : IPersistence
{
    private List<Mapping> _mapping;

    public InMemoryPersistence(List<Mapping> mapping)
    {
        _mapping = mapping;
    }
    
    public List<Mapping> LoadMappings()
    {
        return _mapping;
    }

    public void WriteMappingsToFile(List<Mapping> mappings)
    {
        _mapping = mappings;
    }
}