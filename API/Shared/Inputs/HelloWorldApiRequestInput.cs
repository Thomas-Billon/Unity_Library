using System.Collections.Generic;

namespace UnityExtension.Api
{
    public class HelloWorldApiRequestInput : ApiRequestInput
    {
        public override string UrlPath { get; } = "helloworld";
        public override HttpRequestMethod HttpRequestMethod { get; } = HttpRequestMethod.Get;
        public override HttpAuthenticationScheme HttpAuthenticationScheme { get; } = HttpAuthenticationScheme.Basic;

        public int KeyCode { get; }

        public override Dictionary<string, string> GetInputProperties()
        {
            return new Dictionary<string, string>
            {
                { nameof(KeyCode), KeyCode.ToString() }
            };
        }
    }
}
