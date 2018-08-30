using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Common.ExchangeAdapter.Server
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILog _log;

        public LoggingHandler(
            ILog log,
            HttpMessageHandler next,
            params string[] pathsToIgnore) : base(next)
        {
            _log = log;
            PathsToIngore = (pathsToIgnore ?? new string[0]).ToHashSet();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var requestPart = "GET";
            var responsePart = "<empty>";

            var query = $"{request.Method} {request.RequestUri}";
            if (request.Method == HttpMethod.Post)
            {
                requestPart = await request.Content.ReadAsStringAsync();
            }

            var sw = Stopwatch.StartNew();
            try
            {
                var result = await base.SendAsync(request, cancellationToken);

                if (!IgnoreSuccess(request.RequestUri))
                {
                    if (result.Content != null)
                    {
                        responsePart = await result.Content.ReadAsStringAsync();
                    }

                    var context = new
                    {
                        Request = requestPart,
                        Response = responsePart,
                        Elapsed = sw.Elapsed
                    };

                    _log.Info(request.RequestUri.PathAndQuery, context);
                }

                return result;
            }
            catch (Exception ex)
            {
                var context = new
                {
                    Request = requestPart,
                    Response = responsePart,
                    Elapsed = sw.Elapsed,
                    Error = ex.Message
                };

                _log.Info(request.RequestUri.PathAndQuery, context);
                throw;
            }
        }

        private readonly HashSet<string> PathsToIngore;

        private bool IgnoreSuccess(Uri requestRequestUri)
        {
            if (PathsToIngore.Contains(requestRequestUri.AbsolutePath))
            {
                return true;
            }

            return false;
        }
    }
}
