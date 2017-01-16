using System.ServiceProcess;

namespace IntrinioService
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
                new IntrinioService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
