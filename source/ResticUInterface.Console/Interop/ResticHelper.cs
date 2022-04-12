using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ResticUInterface.Console.Interop;

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

    public IObservable<ProcessOutput> CheckAsync(string pathToRepository, string password, bool readData)
    {
        return Observable.Create<ProcessOutput>(obs =>
        {
            var tmpFile = Path.GetTempFileName();
            
            File.WriteAllTextAsync(tmpFile, password);
            
            var subscription = RunCommandAsync($"-r \"{pathToRepository}\" check --password-file \"{tmpFile}\"", true)
                .Subscribe(obs);
            
            return Disposable.Create(() =>
            {
                subscription.Dispose();
                File.Delete(tmpFile);
            });
        });
    }
    
    
    private IObservable<ProcessOutput> RunCommandAsync(string command, bool killProcessOnDispose)
    {
        return Observable.Create<ProcessOutput>(async obs =>
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = PathToRestic.FullName,
                    Arguments = command,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.EnableRaisingEvents = true;
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                Cleanup(false);
                var error = new Exception("Unexpected ExitCode: " + process.ExitCode);
                obs.OnError(error);
            }
            else
            {
                Cleanup(false);
                obs.OnCompleted();
            }
            
            return Disposable.Create(() =>
            {
                Cleanup(killProcessOnDispose);
            });

            void Cleanup(bool killProcess)
            {
                process.EnableRaisingEvents = false;
                if (!process.HasExited && killProcess)
                {
                    process.Kill();
                }

                process.OutputDataReceived -= ProcessOnOutputDataReceived;
                process.ErrorDataReceived -= ProcessOnErrorDataReceived;
            }

            void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs args)
            {
                obs.OnNext(new StdOutput(args.Data ?? string.Empty));
            }
            void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs args)
            {
                obs.OnNext(new ErrorOutput(args.Data ?? string.Empty));
            }
        });
        
    }
}

public abstract record ProcessOutput(string Message);
public record ErrorOutput(string Message): ProcessOutput(Message);
public record StdOutput(string Message): ProcessOutput(Message);