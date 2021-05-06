using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;

namespace Lontray.Services.Identity.API.Models.AccountViewModels
{
    public record ConsentViewModel : ConsentInputModel
    {
        public ConsentViewModel(ConsentInputModel model, string returnUrl, AuthorizationRequest request, Client client, Resources resources)
        {
            RememberConsent = model?.RememberConsent ?? true;
            ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>();

            ReturnUrl = returnUrl;

            ClientName = client.ClientName;
            ClientUrl = client.ClientUri;
            ClientLogoUrl = client.LogoUri;
            AllowRememberConsent = client.AllowRememberConsent;

            IdentityScopes = resources.IdentityResources.Select(x => new ScopeViewModel(x, ScopesConsented.Contains(x.Name) || model == null)).ToArray();
            ResourceScopes = resources.ApiResources.SelectMany(x => x.Scopes.Select(s => new ScopeViewModel(s, x, ScopesConsented.Contains(s) || model == null))).ToArray();
        }

        public string ClientName { get; init; }
        public string ClientUrl { get; init; }
        public string ClientLogoUrl { get; init; }
        public bool AllowRememberConsent { get; init; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; init; }
        public IEnumerable<ScopeViewModel> ResourceScopes { get; init; }
    }

    public record ScopeViewModel
    {
        public ScopeViewModel(string scopeName, ApiResource resource, bool check)
        {
            Name = scopeName;
            DisplayName = resource.DisplayName;
            Description = resource.Description;
            Checked = check;
        }

        public ScopeViewModel(IdentityResource identity, bool check)
        {
            Name = identity.Name;
            DisplayName = identity.DisplayName;
            Description = identity.Description;
            Emphasize = identity.Emphasize;
            Required = identity.Required;
            Checked = check || identity.Required;
        }

        public string Name { get; init; }
        public string DisplayName { get; init; }
        public string Description { get; init; }
        public bool Emphasize { get; init; }
        public bool Required { get; init; }
        public bool Checked { get; init; }
    }
}
