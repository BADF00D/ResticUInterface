using System.Diagnostics;

namespace ResticUInterface.Console.Interop;

public class VeraCryptHelper
{
    public FileInfo PathToVeraCryptExe { get; }

    public VeraCryptHelper(FileInfo pathToVeraCryptExe)
    {
        if (!pathToVeraCryptExe.Exists)
        {
            throw new ArgumentException("Path should exists", nameof(pathToVeraCryptExe));
        }

        PathToVeraCryptExe = pathToVeraCryptExe;
    }

    public Task MountAsync(string volumeId, char letter, string password)
    {
        //.\VeraCrypt.exe /v "\Device\Harddisk1\Partition2" /l M /p "password" /q /s
        var command = $"/v {volumeId} /l {letter} /p \"{password}\" /q /s";
        return RunCommandAsync(command);
    }

    public Task DismountAsync(char letter, bool force)
    {
        //.\VeraCrypt.exe /d M /q /s
        var command = $"/d {letter} /q /s";
        return RunCommandAsync(command);
    }

    private async Task RunCommandAsync(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = PathToVeraCryptExe.FullName,
                Arguments = command
            }
        };
        process.Start();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception("Unexpected ExitCode: " + process.ExitCode);
        } 
    }
}