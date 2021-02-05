using RequestProcessor.App.Models;
using RequestProcessor.App.Models.Implementations;

namespace RequestProcessor.App.Mappers
{
    internal static class RequestOptionsMapper
    {
        public static (IRequestOptions, IResponseOptions) MapToTuple(this RequestOptions option)
        {
            return (option, option);
        }
    }
}