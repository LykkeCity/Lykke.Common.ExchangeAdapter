using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
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
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotImplemented)
                throw new NotImplementedException();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (response.Content.Headers.ContentType?.MediaType == MediaTypeNames.Text.Plain)
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

                    foreach (var pair in errors)
                    {
                        if (content.StartsWith(pair.Key))
                            throw pair.Value(content.Substring(pair.Key.Length));
                    }
                }
            }

            return response;
        }
    }
}