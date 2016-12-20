using System.ServiceProcess;

namespace SyndicateProcessing
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Processing()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
