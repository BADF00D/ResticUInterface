namespace ResticUInterface.Console.Interop;

public record Disk(string Serial, ulong Size, string Caption, string FirmwareRevision, uint Index, string Model, Partition[] Partitions, string PNPDeviceId);