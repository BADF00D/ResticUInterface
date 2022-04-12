namespace ResticUInterface.Console.Interop;

public record Partition(string Name, uint Index, uint DiskIndex, ulong Size, string? ValumeLabel);