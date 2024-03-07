using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Identity.Client;

namespace NetwaysPoc.GraphServices
{
    public class GraphService
    {
        private GraphServiceClient? _graphServiceClient;
        private static Settings? _settings;

        public GraphService(Settings settings)
        {
            _settings = settings;
        }

        private GraphServiceClient GetGraphClient()
        {
            if (_graphServiceClient != null)
                return _graphServiceClient;

            string[] scopes = new[] { "https://graph.microsoft.com/.default" };

            var chainedTokenCredential = GetChainedTokenCredentials();
            _graphServiceClient = new GraphServiceClient(chainedTokenCredential, scopes);

            return _graphServiceClient;
        }

        private ChainedTokenCredential GetChainedTokenCredentials()
        {
            var tenantId = _settings?.TenantId;
            var clientId = _settings?.ClientId;
            var clientSecret = _settings?.ClientSecret;

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var chainedTokenCredential = new ChainedTokenCredential(clientSecretCredential);

            return chainedTokenCredential;
        }

        private async Task<string?> GetUserIdAsync()
        {
            var meetingOrganizer = _settings?.MeetingOrganizer;
            var filter = $"startswith(userPrincipalName,'{meetingOrganizer}')";
            var graphServiceClient = GetGraphClient();

            var users = await graphServiceClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = filter;
            });
            return users!.Value!.First().Id;
        }

        public async Task SendEmailAsync(Message message)
        {
            var graphServiceClient = GetGraphClient();
            var userId = await GetUserIdAsync();
            var saveToSentItems = true;

            var body = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = saveToSentItems
            };

            await graphServiceClient.Users[userId].SendMail.PostAsync(body);
        }

        public async Task<OnlineMeeting?> CreateOnlineMeeting(OnlineMeeting onlineMeeting)
        {
            var graphServiceClient = GetGraphClient();
            var userId = await GetUserIdAsync();

            return await graphServiceClient.Users[userId].OnlineMeetings.PostAsync(onlineMeeting);
        }

        public async Task<OnlineMeeting?> UpdateOnlineMeeting(OnlineMeeting onlineMeeting)
        {
            var graphServiceClient = GetGraphClient();
            var userId = await GetUserIdAsync();

            return await graphServiceClient.Users[userId].OnlineMeetings[onlineMeeting.Id].PatchAsync(onlineMeeting);
        }

        public async Task<OnlineMeeting?> GetOnlineMeeting(string onlineMeetingId)
        {
            var graphServiceClient = GetGraphClient();
            var userId = await GetUserIdAsync();

            return await graphServiceClient.Users[userId].OnlineMeetings[onlineMeetingId].GetAsync();
        }
        public async Task CreateEvent(Event newEvent)
        {
            var graphServiceClient = GetGraphClient();
            var userId = await GetUserIdAsync();

            await graphServiceClient.Users[userId].Events.PostAsync(newEvent);
        }
    }
}
/*
 private GraphServiceClient InitializeGraph()
{
    var tenantId = _settings?.TenantId;
    var clientId = _settings?.ClientId;
    var clientSecret = _settings?.ClientSecret;
    string[] scopes = new[] { "https://graph.microsoft.com/.default" };

    var confidentialClientApplication = ConfidentialClientApplicationBuilder
        .Create(clientId)
        .WithClientSecret(clientSecret)
        .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
        .Build();

    var authenticationProvider = new ClientCredentialProvider(confidentialClientApplication);

    return new GraphServiceClient(authenticationProvider);
}
*/