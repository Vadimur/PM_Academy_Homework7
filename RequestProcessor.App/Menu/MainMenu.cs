using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RequestProcessor.App.Exceptions;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Models;
using RequestProcessor.App.Services;

namespace RequestProcessor.App.Menu
{
    /// <summary>
    /// Main menu.
    /// </summary>
    internal class MainMenu : IMainMenu
    {
        private readonly IRequestPerformer _performer;
        private readonly IOptionsSource _options;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="options">Options source</param>
        /// <param name="performer">Request performer.</param>
        /// <param name="logger">Logger implementation.</param>
        public MainMenu(
            IRequestPerformer performer, 
            IOptionsSource options, 
            ILogger logger)
        {
            _performer = performer;
            _options = options;
            _logger = logger;
        }

        public async Task<int> StartAsync()
        {
            Console.WriteLine("\nMade by Mulish Vadym");
            Console.WriteLine("Task 1. HTTP request processor\n");
            List<(IRequestOptions, IResponseOptions)> options;
            try
            {
                IEnumerable<(IRequestOptions, IResponseOptions)> result = await _options.GetOptionsAsync();
                if (result == null)
                {
                    _logger.Log("Options list is empty");
                    Console.WriteLine("Options list is empty");
                    
                    return 0;
                }
                options = result.ToList();
            }
            catch (OptionsAccessException exception)
            {
                _logger.Log(exception, "Error occured when trying to read options from file.");
                Console.WriteLine("Error occured when trying to read options from file.\n" +
                                  $"{exception.Message}");
                
                return -1;
            }

            if (!options.Any())
            {
                _logger.Log("Options list is empty");
                Console.WriteLine("Options list is empty");
                
                return 0;
            }
            
            var validOptions = options.Where(o => o.Item1.IsValid && o.Item2.IsValid).ToList();
            int numberOfRemovedRequests = options.Count - validOptions.Count;
            if (numberOfRemovedRequests > 0)
            {
                _logger.Log($"Amount of invalid options: {numberOfRemovedRequests}");
                Console.WriteLine($"Amount of invalid options: {numberOfRemovedRequests}");
            }

            try
            {
                _logger.Log($"Amount of requests to be processed: {validOptions.Count}\n");
                Console.WriteLine($"Amount of requests to be processed: {validOptions.Count}\n");
                
                _logger.Log("Processing begins...");
                Console.WriteLine("Processing begins...\n");
                
                Task<bool>[] tasks = validOptions.Select(opt => _performer.PerformRequestAsync(opt.Item1, opt.Item2))
                    .ToArray();
                Task.WaitAll(tasks);
                
                _logger.Log($"Amount of successfully processed requests: {tasks.Count(t => t.Result)}");
                Console.WriteLine($"Amount of successfully processed requests: {tasks.Count(t => t.Result)}");
                
                _logger.Log($"Amount of unprocessed requests: {tasks.Count(t => t.Result == false)}");
                Console.WriteLine($"Amount of unprocessed requests: {tasks.Count(t => t.Result == false)}");
                
                _logger.Log("Program has ended");
                Console.WriteLine("\nProgram has ended");

                return 0;
            }
            catch (Exception exception)
            {
                if (exception is PerformException ||
                    exception is ArgumentOutOfRangeException ||
                    exception is ArgumentNullException)
                {
                    _logger.Log(exception, "Error occured.");
                    Console.WriteLine($"Error occured.\n{exception.Message}");
                    
                    return -1;
                }

                throw;
            }
            
        }
    }
}
