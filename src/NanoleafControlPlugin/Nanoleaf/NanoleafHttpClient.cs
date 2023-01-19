namespace Loupedeck.NanoleafControlPlugin.Nanoleaf
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Core;

    using Exceptions;

    internal class NanoleafHttpClient : IDisposable
    {
        private readonly HttpClient _client;
        private String _token;

        public NanoleafHttpClient(String host, String token = "")
        {
            this._token = token;
            this._client = new HttpClient();
            this._client.DefaultRequestHeaders.ExpectContinue = false;

            if (host.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                host = host.Substring(7);
            }

            if (host.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                host = host.Substring(8);
            }

            if (!host.Contains(":"))
            {
                host += ":" + Constants.NanoleafPort;
            }

            this._client.BaseAddress = new Uri($"http://{host}/api/v1/");
        }

        #region IDisposable Members

        public void Dispose() => this._client.Dispose();

        #endregion

        public void AuthorizeRequests(String token) => this._token = token;

        public async Task<String> SendGetRequest(String path = "")
        {
            var authorizedPath = this._token + "/" + path;

            using (var responseMessage = await this._client.GetAsync(authorizedPath))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    this.HandleNanoleafErrorStatusCodes(responseMessage);
                }

                return await responseMessage.Content.ReadAsStringAsync();
            }
        }

        public async Task SendPutRequest(String json, String path = "")
        {
            var authorizedPath = this._token + "/" + path;

            using (var responseMessage = await this._client.PutAsync(authorizedPath, new StringContent(json)))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    this.HandleNanoleafErrorStatusCodes(responseMessage);
                }
            }
        }

        public async Task<String> AddUserRequestAsync()
        {
            using (var responseMessage = await this._client.PostAsync("new/", null))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    this.HandleNanoleafErrorStatusCodes(responseMessage);
                }

                return await responseMessage.Content.ReadAsStringAsync();
            }
        }

        public async Task DeleteUserRequest(String token = "")
        {
            using (var responseMessage = await this._client.DeleteAsync(token))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    this.HandleNanoleafErrorStatusCodes(responseMessage);
                }
            }
        }

        private void HandleNanoleafErrorStatusCodes(HttpResponseMessage responseMessage)
        {
            switch ((Int32)responseMessage.StatusCode)
            {
                case 400:
                    throw new NanoleafHttpException("Error 400: Bad request!");
                case 401:
                    throw new NanoleafUnauthorizedException(
                        $"Error 401: Not authorized! Provided an invalid token for this Aurora. Request path: {responseMessage.RequestMessage.RequestUri.AbsolutePath}");
                case 403:
                    throw new NanoleafHttpException("Error 403: Forbidden!");
                case 404:
                    throw new NanoleafResourceNotFoundException($"Error 404: Resource not found! Request Uri: {responseMessage.RequestMessage.RequestUri.AbsoluteUri}");
                case 422:
                    throw new NanoleafHttpException("Error 422: Unprocessable Entity");
                case 500:
                    throw new NanoleafHttpException("Error 500: Internal Server Error");
                default:
                    throw new NanoleafHttpException("ERROR! UNKNOWN ERROR " + (Int32)responseMessage.StatusCode);
            }
        }
    }
}