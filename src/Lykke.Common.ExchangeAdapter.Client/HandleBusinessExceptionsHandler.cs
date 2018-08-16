using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Server.Fails;

namespace Lykke.Common.ExchangeAdapter.Client
{
    internal class HandleBusinessExceptionsHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotImplemented)
            {
                throw new NotImplementedException();
            }

            // if (response.Headers[])
            if (response.StatusCode == HttpStatusCode.BadRequest
                && response.Content.Headers.TryGetValues("Content-Type", out var values))
            {
                var contentType = values.FirstOrDefault();

                if (contentType == "text/plain")
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var errors = new Dictionary<string, Func<string, Exception>>
                    {
                        {"notFound", msg => new OrderNotFoundException(msg)},
                        {"volumeTooSmall: ", msg => new VolumeTooSmallException(msg)},
                        {"priceError: ", msg => new InvalidOrderPriceException(msg)},
                        {"orderIdFormat", _ => new InvalidOrderIdException()},
                        {"notEnoughBalance", msg => new InsufficientBalanceException(msg)},
                        {"instrumentIsNotSupported", msg => new InvalidInstrumentException(msg)}
                    };

                    foreach (var kv in errors)
                    {
                        if (content.StartsWith(kv.Key))
                        {
                            throw kv.Value(content.Substring(kv.Key.Length));
                        }
                    }
                }
            }

            return response;
        }
    }
}