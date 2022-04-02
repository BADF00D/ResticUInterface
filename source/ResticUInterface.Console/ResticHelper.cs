using System.Diagnostics;

namespace ResticUInterface.Console;

public class ResticHelper
{
    public FileInfo PathToRestic { get; set; }

    public ResticHelper(FileInfo pathToRestic)
    {
        if (!pathToRestic.Exists)
        {
            throw new ArgumentException("Path to restic is invalid");
        }

        PathToRestic = pathToRestic;
    }

    public Task CheckAsync(string pathToRepository, string password, bool readData)
    {
        return RunCommandAsync($"-r \"{pathToRepository}\" check --password-command \"{password}\"");
    }
    
    
    private async Task RunCommandAsync(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = PathToRestic.FullName,
                Arguments = command,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            }
        };
        process.Start();

        await process.WaitForExitAsync();

        var error = await process.StandardError.ReadToEndAsync();
        var output = await process.StandardOutput.ReadToEndAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception("Unexpected ExitCode: " + process.ExitCode);
        } 
    }
}