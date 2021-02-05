using System;
using System.Net.Http;
using System.Threading.Tasks;
using RequestProcessor.App.Exceptions;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Models;
using RequestProcessor.App.Models.Implementations;

namespace RequestProcessor.App.Services
{
    /// <summary>
    /// Request performer.
    /// </summary>
    internal class RequestPerformer : IRequestPerformer
    {
        private readonly IRequestHandler _requestHandler;
        private readonly IResponseHandler _responseHandler;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="requestHandler">Request handler implementation.</param>
        /// <param name="responseHandler">Response handler implementation.</param>
        /// <param name="logger">Logger implementation.</param>
        public RequestPerformer(
            IRequestHandler requestHandler, 
            IResponseHandler responseHandler,
            ILogger logger)
        {
            _requestHandler = requestHandler;
            _responseHandler = responseHandler;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> PerformRequestAsync(
            IRequestOptions requestOptions, 
            IResponseOptions responseOptions)
        {
            ValidateArguments(requestOptions, responseOptions);
            
            bool result = true;
            IResponse response;
            
            try
            {
                _logger.Log("Sending request...");
                response = await _requestHandler.HandleRequestAsync(requestOptions);
                _logger.Log("Response received.");
            }
            catch (TaskCanceledException exception)
            {
                response = new Response
                {
                    Handled = false,
                    Code = 504,
                    Content = null
                };
                _logger.Log(exception, "Timeout exception occured when sending request.");
                result = false;
            }
            catch (Exception exception)
            {
                if (exception is ArgumentNullException ||
                    exception is ArgumentOutOfRangeException ||
                    exception is InvalidOperationException ||
                    exception is HttpRequestException)
                {
                    var performException = new PerformException("Critical performer error. See inner exception for more details", exception);
                    _logger.Log(performException, "Exception was thrown.");
                    throw performException;
                }

                throw;
            }

            bool isSaveSuccess = await SaveResponse(requestOptions, responseOptions, response);
            result = result && isSaveSuccess;
            
            return result;
        }

        private void ValidateArguments(IRequestOptions requestOptions, IResponseOptions responseOptions)
        {
            if (requestOptions == null)
            {
                var exception =  new ArgumentNullException(nameof(requestOptions), "One of required parameters are missing.");
                _logger.Log(exception, "Exception was thrown.");
                throw exception;
            }

            if (responseOptions == null)
            {
                var exception = new ArgumentNullException(nameof(responseOptions), "One of required parameters are missing.");
                _logger.Log(exception, "Exception was thrown.");
                throw exception;
            }

            if (!requestOptions.IsValid)
            {
                var exception = new ArgumentOutOfRangeException(nameof(requestOptions), "One of parameters is not valid.");
                _logger.Log(exception, "Exception was thrown.");
                throw exception;
            }
            
            if (!responseOptions.IsValid)
            {                
                var exception = new ArgumentOutOfRangeException(nameof(responseOptions), "One of parameters is not valid.");
                _logger.Log(exception, "Exception was thrown.");
                throw exception;
            }
        }

        private async Task<bool> SaveResponse(IRequestOptions requestOptions, IResponseOptions responseOptions, IResponse response)
        {
            try
            {
                await _responseHandler.HandleResponseAsync(response, requestOptions, responseOptions);
                _logger.Log("Response was saved");
                
                return true;
            }
            catch (ArgumentNullException exception)
            {
                _logger.Log(exception, "One of required parameters for response handling are missing. See inner exception for more details");
                
                return false;
            }
            
        }
    }
}
