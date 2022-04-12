using ResticUInterface.Console.Interop;

namespace ResticUInterface.Console.Extensions;

public static class PartitionExtensions
{
    public static string GetVeryCryptIdentifier(this Partition partition)
    {
        return $@"\Device\Harddisk{partition.DiskIndex}\Partition{partition.Index+1}";
    }
}