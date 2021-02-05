using System;
using System.IO;
using System.Threading.Tasks;
using RequestProcessor.App.Models;

namespace RequestProcessor.App.Services.Implementations
{
    internal class ResponseHandler : IResponseHandler
    {
        public async Task HandleResponseAsync(IResponse response, IRequestOptions requestOptions, IResponseOptions responseOptions)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response), "One of required parameters are missing");
            }

            if (requestOptions == null)
            {
                throw new ArgumentNullException(nameof(requestOptions), "One of required parameters are missing");
            }

            if (responseOptions == null)
            {
                throw new ArgumentNullException(nameof(responseOptions), "One of required parameters are missing");
            }
            
            string fileHeader;
            if (response.Handled && response.Code >= 200 && response.Code < 300)
            {
                fileHeader = $"{response.Code} Success\n";
            }
            else
            {
                fileHeader = $"{response.Code} Fail\n";
            }

            string textToSave = fileHeader + response.Content;
            
            await File.WriteAllTextAsync(responseOptions.Path, textToSave);
        }
    }
}