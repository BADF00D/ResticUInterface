namespace ResticUInterface.Console.Model;

public record Partition(string Name, uint Index, uint DiskIndex, ulong Size, string? ValumeLabel);