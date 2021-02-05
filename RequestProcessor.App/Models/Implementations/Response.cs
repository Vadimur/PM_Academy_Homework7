namespace RequestProcessor.App.Models.Implementations
{
    internal class Response : IResponse
    {
        public bool Handled { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
    }
}