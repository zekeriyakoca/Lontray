using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Web.BFF.Shopping.Infrastructure
{
    public class AuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly HttpContext httpContext;

        public AuthorizationDelegatingHandler(IHttpContextAccessor contextAccessor)
        {
            this.httpContext = contextAccessor?.HttpContext ?? throw new ArgumentNullException();
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"];

            if (!String.IsNullOrWhiteSpace(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }

            var token = await httpContext.GetTokenAsync("access_token");
            if (!String.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
