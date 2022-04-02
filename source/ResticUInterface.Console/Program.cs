// See https://aka.ms/new-console-template for more information

using System.Management;
using ResticUInterface.Console;
using ResticUInterface.Console.Configuration;
using ResticUInterface.Console.Extensions;
using ResticUInterface.Console.Model;


var definitions = new []{
    new BackupDefinition("Std", "TBJ6C6K21           "),
    new BackupDefinition("Std2", "SGH3S6D1Y           ")
};

// var serialTodetect = "SGH3S6D1Y           ";
var serialTodetect = "TBJ6C6K21           ";
const string PathToRestic = @"D:\restic_0.12.1_windows_386\restic_0.12.1_windows_386.exe";
const string RelativePathToRepository = "";

var restic = new ResticHelper(new FileInfo(PathToRestic));
var repositoryPassword = ReadPassword();
await restic.CheckAsync("local:M:", repositoryPassword, true);


Console.WriteLine("Detecting HDD");
var disks = ReadDisks();
foreach (var disk in disks)
{
    Console.WriteLine(disk.Caption);
    Console.WriteLine($"  Serial: {disk.Serial}");
    Console.WriteLine($"  Size: {disk.Size.ToDiskSize()}");
    
    foreach (var diskPartition in disk.Partitions)
    {
        Console.WriteLine($"  {diskPartition.Name}");
        Console.WriteLine($"    Index: {diskPartition.Index}");
        Console.WriteLine($"    Size: {diskPartition.Size.ToDiskSize()}");
    }
}

var volumeId = disks
    .Where(d => d.Serial == serialTodetect)
    .Select(d => d.Partitions.FirstOrDefault(p => p.Index == 1))
    .Select(p => p.GetVeryCryptIdentifier())
    .FirstOrDefault();

if (volumeId != null)
{
    Console.WriteLine("Detected very crypt volume: "+volumeId);
    Console.Write("Enter password: ");
    // var password = Console.ReadLine();
    var password = ReadPassword();
    var vera = new VeraCryptHelper(new FileInfo(@"C:\Program Files\VeraCrypt\VeraCrypt.exe"));
    await vera.MountAsync(volumeId, 'M', password);
    
    // Console.WriteLine("Volumen mounted. Dismount with any key.");
    // Console.ReadKey();
    //
    // await vera.DismountAsync('M', false);
}

static string ReadPassword()
{
    var pass = string.Empty;
    ConsoleKey key;
    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;

        if (key == ConsoleKey.Backspace && pass.Length > 0)
        {
            Console.Write("\b \b");
            pass = pass[0..^1];
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            Console.Write("*");
            pass += keyInfo.KeyChar;
        }
    } while (key != ConsoleKey.Enter);

    Console.WriteLine();
    return pass;
}




// foreach (var drive in drives)
// {
//     Console.WriteLine($"{drive.Name}: {drive.VolumeLabel}");
// }




// WqlEventQuery weqQuery = new WqlEventQuery();
// weqQuery.EventClassName = "__InstanceOperationEvent";
// weqQuery.WithinInterval = new TimeSpan(0, 0, 3);
// weqQuery.Condition = @"TargetInstance ISA 'Win32_PnPEntity'";
//
// ManagementEventWatcher m_mewWatcher = new ManagementEventWatcher(weqQuery);
// m_mewWatcher.EventArrived += (_, args) =>
// {
//     // using var managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_LogicalDiskToPartition");
//     // foreach (var drive in managementObjectSearcher.Get().Cast<ManagementObject>().ToList())
//     // {
//     //     // var driveNumber = Regex.Match((string)drive["Antecedent"], @"Disk #(\d*),").Groups[1].Value;
//     //     //
//     //     // Console.WriteLine("Drive Number: " + driveNumber);
//     //     foreach (var property in drive.Properties)
//     //     {
//     //         Console.WriteLine($"{property.Name}: {property.Value}");
//     //     }
//     //     Console.WriteLine(Environment.NewLine);
//     // }
//     
//     var disks = ReadDisks();
//     
//     Console.WriteLine(disks);
//     // using var mos = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
//     // foreach (ManagementObject mo in mos.Get())
//     // {
//     //     ManagementObject query = new ManagementObject("Win32_PhysicalMedia.Tag='" + mo["DeviceID"] + "'");
//     //     Console.WriteLine(query["SerialNumber"]);
//     // }
//     //
//     // using var mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
//     // foreach (ManagementObject moDisk in mosDisks.Get())
//     // {
//     //
//     //     // Set all the fields to the appropriate values
//     //
//     //     Console.WriteLine("Type: " + moDisk["MediaType"].ToString());
//     //
//     //     Console.WriteLine("Model: " + moDisk["Model"].ToString());
//     //
//     //     Console.WriteLine("Serial: " + moDisk["SerialNumber"].ToString());
//     //
//     //     Console.WriteLine("Interface: " + moDisk["InterfaceType"].ToString());
//     //     foreach (var property in moDisk.Properties)
//     //     {
//     //         Console.WriteLine($"   {property.Name}: {property.Value}");
//     //     }
//     // }
//     //
//     // var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskPartition");
//     //
//     // foreach (var queryObj in searcher.Get())
//     // {
//     //     Console.WriteLine("-----------------------------------");
//     //     Console.WriteLine("Win32_DiskPartition instance");
//     //     Console.WriteLine("Name:{0}", (string)queryObj["Name"]);
//     //     Console.WriteLine("Index:{0}", (uint)queryObj["Index"]);
//     //     Console.WriteLine("DiskIndex:{0}", (uint)queryObj["DiskIndex"]);
//     //     Console.WriteLine("BootPartition:{0}", (bool)queryObj["BootPartition"]);
//     //     foreach (var property in queryObj.Properties)
//     //     {
//     //         Console.WriteLine($"   {property.Name}: {property.Value}");
//     //     }
//     // }
// };
//
//
//
// m_mewWatcher.Start();




Console.ReadLine();

static IReadOnlyCollection<Disk> ReadDisks()
{
    var partitions = ReadPartitions();
    
    using var mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
    var results = new List<Disk>();
    foreach (ManagementObject moDisk in mosDisks.Get())
    {

        // Set all the fields to the appropriate values

        // Console.WriteLine("Type: " + moDisk["MediaType"].ToString());
        //
        // Console.WriteLine("Model: " + moDisk["Model"].ToString());
        //
        // Console.WriteLine("Serial: " + moDisk["SerialNumber"].ToString());
        //
        // Console.WriteLine("Interface: " + moDisk["InterfaceType"].ToString());
        // foreach (var property in moDisk.Properties)
        // {
        //     Console.WriteLine($"   {property.Name}: {property.Value}");
        // }

        var diskIndex = (uint)moDisk["Index"];
        var diskPartitions = partitions
            .Where(p => p.DiskIndex == diskIndex)
            .ToArray();
        results.Add(new (moDisk["SerialNumber"].ToString(), (ulong)moDisk["Size"], moDisk["Caption"].ToString(), "", diskIndex, "", 
            diskPartitions, ""));
    }

    return results;
}

static IReadOnlyCollection<Partition> ReadPartitions()
{
    using var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskPartition");
    var results = new List<Partition>();
    
    var mappings = ReadMappings();
    
    foreach (var queryObj in searcher.Get())
    {
        // Console.WriteLine("-----------------------------------");
        // Console.WriteLine("Win32_DiskPartition instance");
        // Console.WriteLine("Name:{0}", (string)queryObj["Name"]);
        // Console.WriteLine("Index:{0}", (uint)queryObj["Index"]);
        // Console.WriteLine("DiskIndex:{0}", (uint)queryObj["DiskIndex"]);
        // Console.WriteLine("BootPartition:{0}", (bool)queryObj["BootPartition"]);
        // foreach (var property in queryObj.Properties)
        // {
        //     Console.WriteLine($"   {property.Name}: {property.Value}");
        // }

        var partitionName = (string)queryObj["Name"];
        var volumeLabel = mappings.FirstOrDefault(m => m.PartitionName == partitionName)?.VolumeLabel;
        results.Add(new (partitionName, (uint)queryObj["Index"], (uint)queryObj["DiskIndex"], (ulong)queryObj["Size"], volumeLabel));
        
    }

    return results;
}

static IReadOnlyCollection<LogicalDiskToPartitionMapping> ReadMappings()
{
    using var managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_LogicalDiskToPartition");
    var result = new List<LogicalDiskToPartitionMapping>();
    foreach (var drive in managementObjectSearcher.Get().Cast<ManagementObject>().ToList())
    {
        // foreach (var property in drive.Properties)
        // {
        //     Console.WriteLine($"{property.Name}: {property.Value}");
        // }

        var antecedent = drive["Antecedent"].ToString().Split('=')[1];
        var dependent = drive["Dependent"].ToString().Split('=')[1];
        result.Add(new(antecedent.Trim('"'), dependent));
        // Console.WriteLine(Environment.NewLine);
    }

    return result;
}

public record LogicalDiskToPartitionMapping(string PartitionName, string VolumeLabel);