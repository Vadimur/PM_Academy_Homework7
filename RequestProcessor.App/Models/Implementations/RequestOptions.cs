using System;
using System.Text.Json.Serialization;

namespace RequestProcessor.App.Models.Implementations
{
    internal class RequestOptions : IResponseOptions, IRequestOptions 
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        [JsonPropertyName("method")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestMethod Method { get; set; }
        
        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }
        
        [JsonPropertyName("body")]
        public string Body { get; set; }
        bool IRequestOptions.IsValid => ValidateRequestOptions();
        bool IResponseOptions.IsValid => ValidateResponseOptions();

        private bool ValidateResponseOptions()
        {
            return !string.IsNullOrWhiteSpace(Path);
        }
        private bool ValidateRequestOptions()
        {
            bool isValidAddress = Uri.TryCreate(Address, UriKind.Absolute, out Uri outUri)
                                  && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
            
            bool isValidContentType = (!string.IsNullOrWhiteSpace(Body) && !string.IsNullOrWhiteSpace(ContentType)) 
                                      || string.IsNullOrWhiteSpace(Body);
            
            if (isValidAddress == false ||
                isValidContentType == false ||
                Method == RequestMethod.Undefined)
            {
                return false;
            }

            return true;
        }
    }
}