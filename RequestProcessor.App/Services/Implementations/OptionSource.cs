using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;
using RequestProcessor.App.Exceptions;
using RequestProcessor.App.Mappers;
using RequestProcessor.App.Models;
using RequestProcessor.App.Models.Implementations;

namespace RequestProcessor.App.Services.Implementations
{
    internal class OptionSource : IOptionsSource
    {
        private readonly string _filePath;

        public OptionSource(string filePath)
        {
            _filePath = filePath;
        }
        
        public async Task<IEnumerable<(IRequestOptions, IResponseOptions)>> GetOptionsAsync()
        {
            string fileContent;
            try
            {
                fileContent = await File.ReadAllTextAsync(_filePath);
            }
            catch (Exception exception)
            {
                if (exception is UnauthorizedAccessException ||
                    exception is FileNotFoundException ||
                    exception is SecurityException ||
                    exception is IOException)
                {
                    throw new OptionsAccessException($"Could not read options from file {_filePath}.", exception);
                }
                throw;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(fileContent))
                {
                    return await Task.FromResult<IEnumerable<(IRequestOptions, IResponseOptions)>>(null);
                }
                
                List<RequestOptions> options = JsonSerializer.Deserialize<List<RequestOptions>>(fileContent);

                if (options == null)
                {
                    return await Task.FromResult<IEnumerable<(IRequestOptions, IResponseOptions)>>(null);
                }
                
                List<(IRequestOptions, IResponseOptions)> result = options.Select(o => o.MapToTuple()).ToList();

                return result;
            }
            catch (Exception exception)
            {
                if (exception is ArgumentNullException ||
                    exception is JsonException)
                {
                    throw new OptionsAccessException($"Could not deserialize options from file {_filePath}.", exception);
                }
                throw;
            }
        }
    }
}