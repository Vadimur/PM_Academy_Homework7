using System.Net.Http;
using System.Threading.Tasks;
using RequestProcessor.App.Models;
using RequestProcessor.App.Models.Implementations;

namespace RequestProcessor.App.Mappers
{
    internal static class HttpResponseToResponseMapper
    {
        public static async Task<IResponse> MapToCustomResponse(this HttpResponseMessage responseMessage)
        {
            return new Response
            {
                Handled = true,
                Code = (int)responseMessage.StatusCode,
                Content = await responseMessage.Content.ReadAsStringAsync()
            };
        }
    }
}