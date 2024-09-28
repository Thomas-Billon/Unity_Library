using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityExtension.Api
{
    public class ApiRequest<TInput, TOutput>
        where TInput : ApiRequestInput
        where TOutput : ApiRequestOutput, new()
    {
        private string _url = "";
        private HttpRequestMethod _httpRequestMethod = HttpRequestMethod.None;
        private HttpAuthenticationScheme _httpAuthenticationScheme = HttpAuthenticationScheme.None;
        private string _queryDataParameters = "";   // GET, DELETE
        private string _bodyDataParameters = "";    // PUT
        private WWWForm _formDataParameters = null; // POST

        private UnityWebRequest _request = null;
        private TOutput _result = null;

        public async UniTask<TOutput> SendAsync(TInput input)
        {
            if (IsWebRequestValid(input) == false)
            {
                return new TOutput { IsError = true, HttpResponseCode = HttpResponseCode.BadRequest };
            }

            GetWebRequestInput(input);
            CreateWebRequest();

            if (await SendWebRequestAsync() == false)
            {
                return new TOutput { IsError = true, HttpResponseCode = HttpResponseCode.ServiceUnavailable };
            }

            if (DeserializeWebRequest() == false)
            {
                return new TOutput { IsError = true, HttpResponseCode = HttpResponseCode.InternalServerError };
            }

            return _result;
        }

        #region IsWebRequestValid

        public bool IsWebRequestValid(TInput input)
        {
            if (string.IsNullOrWhiteSpace(ApiConfig.Instance.UrlBase))
            {
                Debug.LogError("API request error: API base url is missing from config");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ApiConfig.Instance.Version))
            {
                Debug.LogError("API request error: API version is missing from config");
                return false;
            }

            if (input.HttpRequestMethod == HttpRequestMethod.None)
            {
                Debug.LogError("API request error: HTTP request method is missing from input");
                return false;
            }

            if (input.HttpRequestMethod != HttpRequestMethod.Get &&
                input.HttpRequestMethod != HttpRequestMethod.Post &&
                input.HttpRequestMethod != HttpRequestMethod.Put &&
                input.HttpRequestMethod != HttpRequestMethod.Delete)
            {
                Debug.LogError("API request error: Unity is unable to handle this type of request method");
                return false;
            }

            return true;
        }

        #endregion IsWebRequestValid

        #region GetWebRequestInput

        private void GetWebRequestInput(TInput input)
        {
            GetUrlFromInput(input);
            GetHttpMethodFromInput(input);
            GetAuthenticationSchemeFromInput(input);
            GetDataParametersFromInput(input);
        }

        private void GetUrlFromInput(TInput input)
        {
            var urlBase = ApiConfig.Instance.UrlBase.TrimEnd('/');
            var urlPath = input.UrlPath.TrimStart('/');

            _url = $"{urlBase}/{urlPath}";
        }

        private void GetDataParametersFromInput(TInput input)
        {
            if (_httpRequestMethod == HttpRequestMethod.None)
            {
                GetHttpMethodFromInput(input);
            }

            switch (_httpRequestMethod)
            {
                case HttpRequestMethod.Get:
                case HttpRequestMethod.Delete:
                case HttpRequestMethod.Head:
                    GetUrlDataFromInput(input);
                    break;

                case HttpRequestMethod.Post:
                    GetFormDataFromInput(input);
                    break;

                case HttpRequestMethod.Put:
                    GetBodyDataFromInput(input);
                    break;
            }
        }

        private void GetUrlDataFromInput(TInput input)
        {
            var properties = input.GetInputProperties();

            if (properties.Count == 0)
            {
                return;
            }

            _queryDataParameters = "?";

            foreach (var property in properties)
            {
                _queryDataParameters += property.Key;
                _queryDataParameters += '=';
                _queryDataParameters += property.Value;
                _queryDataParameters += '&';
            }

            // Remove last char
            _queryDataParameters = _queryDataParameters.Remove(_queryDataParameters.Length - 1);
        }

        private void GetFormDataFromInput(TInput input)
        {
            _formDataParameters = new WWWForm();

            foreach (var property in input.GetInputProperties())
            {
                _formDataParameters.AddField(property.Key, property.Value);
            }
        }

        private void GetBodyDataFromInput(TInput input)
        {
            _bodyDataParameters = JsonConvert.SerializeObject(input.GetInputProperties());
        }

        private void GetHttpMethodFromInput(TInput input)
        {
            _httpRequestMethod = input.HttpRequestMethod;
        }

        private void GetAuthenticationSchemeFromInput(TInput input)
        {
            _httpAuthenticationScheme = input.HttpAuthenticationScheme;
        }

        #endregion GetWebRequest

        #region CreateWebRequest

        private void CreateWebRequest()
        {
            SetWebRequestMethod();
            SetWebRequestHeader();
        }

        private void SetWebRequestMethod()
        {
            switch (_httpRequestMethod)
            {
                case HttpRequestMethod.Get:
                    _request = UnityWebRequest.Get($"{_url}{_queryDataParameters}");
                    break;

                case HttpRequestMethod.Post:
                    _request = UnityWebRequest.Post(_url, _formDataParameters);
                    break;

                case HttpRequestMethod.Put:
                    _request = UnityWebRequest.Put(_url, _bodyDataParameters);
                    break;

                case HttpRequestMethod.Delete:
                    _request = UnityWebRequest.Delete($"{_url}{_queryDataParameters}");
                    break;

                default:
                    _request = new UnityWebRequest(_url);
                    break;
            }
        }

        private void SetWebRequestHeader()
        {
            if (_request == null)
            {
                return;
            }

            if (_httpAuthenticationScheme != HttpAuthenticationScheme.None)
            {
                var authenticationScheme = _httpAuthenticationScheme.ToString();
                var authenticationCredentials = "";

                switch (_httpAuthenticationScheme)
                {
                    case HttpAuthenticationScheme.Basic:
                        authenticationCredentials = $"{ApiAuthorization.Instance.IdUser}:{ApiAuthorization.Instance.Password}".ToBase64();
                        break;

                    case HttpAuthenticationScheme.Bearer:
                        authenticationCredentials = $"{ApiAuthorization.Instance.IdSession}".ToBase64();
                        break;
                }

                _request.SetRequestHeader("Authorization", $"{authenticationScheme} {authenticationCredentials}");
            }

            _request.SetRequestHeader("Accept-version", ApiConfig.Instance.Version);
        }

        #endregion CreateWebRequest

        #region SendWebRequestAsync

        private async UniTask<bool> SendWebRequestAsync()
        {
            Debug.Log($"API request log: Sending request ({_request.url})");

            try
            {
                await _request.SendWebRequest().WithCancellation(new TimeoutController().Timeout(TimeSpan.FromSeconds(5)));
            }
            catch (Exception exception)
            {
                Debug.LogError($"API request error: Failed to send API request ({exception.Message})");
                return false;
            }

            if (_request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API request error: Failed to receive API result ({_request.result})");
                return false;
            }

            return true;
        }

        #endregion SendWebRequestAsync

        #region DeserializeWebRequest
        private bool DeserializeWebRequest()
        {
            try
            {
                _result = JsonConvert.DeserializeObject<TOutput>(_request.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogError($"API request error: Failed to serialize API result ({exception.Message})");
                return false;
            }

            if (_result == null)
            {
                Debug.LogError($"API request error: Failed to serialize API result (Empty result)");
                return false;
            }

            return true;
        }

        #endregion DeserializeWebRequest
    }
}