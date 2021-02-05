using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RequestProcessor.App.Mappers;
using RequestProcessor.App.Models;

namespace RequestProcessor.App.Services.Implementations
{
    internal class RequestHandler : IRequestHandler
    {
        private readonly HttpClient _httpClient;

        public RequestHandler(HttpClient client)
        {
            _httpClient = client;
        }
        public async Task<IResponse> HandleRequestAsync(IRequestOptions requestOptions)
        {
            if (requestOptions == null)
            {
                throw new ArgumentNullException($"{nameof(requestOptions)} is missing." );
            }
            
            if (!requestOptions.IsValid)
            {
                throw new ArgumentOutOfRangeException($"{nameof(requestOptions)} is not valid.");
            }
            
            
            HttpMethod httpMethod = new HttpMethod(requestOptions.Method.ToString().ToUpper());
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestOptions.Address);

            if (!string.IsNullOrWhiteSpace(requestOptions.Body) &&
                !string.IsNullOrWhiteSpace(requestOptions.ContentType))
            {
                //var contentBytes = Encoding.UTF8.GetBytes(requestOptions.Body);
                //request.Content = new ByteArrayContent(contentBytes); 
                //request.Headers.Add("Content-type", requestOptions.ContentType);

                request.Content = new StringContent(requestOptions.Body,
                    Encoding.UTF8, 
                    requestOptions.ContentType);
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return await response.MapToCustomResponse();
        }
    }
}