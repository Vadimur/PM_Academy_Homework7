using System;
using System.Net.Http;
using System.Threading.Tasks;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Logging.Implementations;
using RequestProcessor.App.Menu;
using RequestProcessor.App.Services;
using RequestProcessor.App.Services.Implementations;

namespace RequestProcessor.App
{
    /// <summary>
    /// Entry point.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <returns>Returns exit code.</returns>
        private static async Task<int> Main()
        {
            try
            {
                string optionsFilePath = "options.json";
                
                IOptionsSource optionsSource = new OptionSource(optionsFilePath);
                ILogger logger = new Logger();
                IResponseHandler responseHandler = new ResponseHandler();
                IRequestHandler requestHandler = new RequestHandler(new HttpClient());
                IRequestPerformer requestPerformer = new RequestPerformer(requestHandler, responseHandler, logger);
                
                var mainMenu = new MainMenu(requestPerformer, optionsSource, logger);
                
                return await mainMenu.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Critical unhandled exception");
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
