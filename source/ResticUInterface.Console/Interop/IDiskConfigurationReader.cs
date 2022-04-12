using System.Management;

namespace ResticUInterface.Console.Interop;

internal interface IDiskConfigurationReader
{
    IReadOnlyCollection<Disk> Read();
}

internal class DiskConfigurationReader : IDiskConfigurationReader
{
    public IReadOnlyCollection<Disk> Read()
    {
        var partitions = ReadPartitions();
    
        using var mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
        var results = new List<Disk>();
        foreach (ManagementObject moDisk in mosDisks.Get())
        {
            var diskIndex = (uint)moDisk["Index"];
            var diskPartitions = partitions
                .Where(p => p.DiskIndex == diskIndex)
                .ToArray();
            results.Add(new (moDisk["SerialNumber"].ToString(), (ulong)moDisk["Size"], moDisk["Caption"].ToString(), "", diskIndex, "", 
                diskPartitions, ""));
        }

        return results;
    }
    
    private static IReadOnlyCollection<Partition> ReadPartitions()
    {
        using var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskPartition");
        var results = new List<Partition>();
    
        var mappings = ReadMappings();
    
        foreach (var queryObj in searcher.Get())
        {
            var partitionName = (string)queryObj["Name"];
            var volumeLabel = mappings.FirstOrDefault(m => m.PartitionName == partitionName)?.VolumeLabel;
            results.Add(new (partitionName, (uint)queryObj["Index"], (uint)queryObj["DiskIndex"], (ulong)queryObj["Size"], volumeLabel));
        }

        return results;
    }
    
    private static IReadOnlyCollection<LogicalDiskToPartitionMapping> ReadMappings()
    {
        using var managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_LogicalDiskToPartition");
        var result = new List<LogicalDiskToPartitionMapping>();
        foreach (var drive in managementObjectSearcher.Get().Cast<ManagementObject>().ToList())
        {
            var antecedent = drive["Antecedent"].ToString().Split('=')[1];
            var dependent = drive["Dependent"].ToString().Split('=')[1];
            result.Add(new(antecedent.Trim('"'), dependent));
        }

        return result;
    }
    public record LogicalDiskToPartitionMapping(string PartitionName, string VolumeLabel);
}
