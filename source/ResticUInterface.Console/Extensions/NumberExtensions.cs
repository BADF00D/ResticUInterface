namespace ResticUInterface.Console.Extensions;

public static class NumberExtensions
{
    public static string ToDiskSize(this ulong sizeInBytes)
    {
        return sizeInBytes switch
        {
            < 1024 => $"{sizeInBytes}",
            < 1024 * 1024 => $"{sizeInBytes / 1024f:0.0}kb",
            < 1024 * 1024 * 1024 => $"{sizeInBytes / 1024f / 1024:0.0}mb",
            < 1024L * 1024 * 1024 * 1024 => $"{sizeInBytes / 1024f / 1024 / 1024:0.0}gb",
            _ => $"{sizeInBytes / 1024f / 1024 / 1024 / 1024:0.0}tb"
        };
    }
}