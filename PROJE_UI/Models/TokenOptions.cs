using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace PROJE_UI.Models
{
    public class TokenOptions
    { 
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
    public class LoginApiResponseModel
    {
        public TokenOptions  Data { get; set; }
        public string Message { get; set; }
    }

    
}

