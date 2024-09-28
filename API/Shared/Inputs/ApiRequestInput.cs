using System.Collections.Generic;

namespace UnityExtension.Api
{
    public abstract class ApiRequestInput
    {
        public abstract string UrlPath { get; }
        public abstract HttpRequestMethod HttpRequestMethod { get; }
        public virtual HttpAuthenticationScheme HttpAuthenticationScheme { get; } = HttpAuthenticationScheme.Bearer;

        public abstract Dictionary<string, string> GetInputProperties();
    }
}