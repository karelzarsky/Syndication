using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Linq;

// install:
// first publish, then
// sc create !WEB binPath=c:\Users\Karel\Source\Repos\Syndication\Web\bin\Debug\PublishOutput\Web.exe

namespace SyndicationWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool isService = true;
            if (Debugger.IsAttached || args.Contains("--console"))
            {
                isService = false;
            }

            var pathToContentRoot = Directory.GetCurrentDirectory();
            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe);
            }
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(pathToContentRoot)
                .UseUrls("http://*:5000")
                .UseStartup<Startup>()
                .Build();

            if (isService)
            {
                host.RunAsCustomService();
            }
            else
            {
                host.Run();
            }
        }
    }
}
