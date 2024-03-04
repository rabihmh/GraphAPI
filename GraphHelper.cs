using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace NetwaysPoc
{
    abstract class GraphHelper
    {
        private static Settings? _settings;
        private static GraphServiceClient? _userClient;
        private static ClientSecretCredential? _clientCredential;

        public static void InitializeGraphForUserAuth(Settings settings)
        {
            _settings = settings;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            var clientCredential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret, options);
            _clientCredential = clientCredential;
            _userClient = new GraphServiceClient(clientCredential, new[] {"https://graph.microsoft.com/.default"});
        }
        private static async Task<string?> GetUserIdAsync()
        {
            var meetingOrganizer = _settings.MeetingOrganizer;
            var filter = $"startswith(userPrincipalName,'{meetingOrganizer}')";

            var users = await _userClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = filter;
            });

            return users!.Value!.First().Id;
        }
        public static async Task<string> GetUserTokenAsync()
        {
            try
            {
                if (_settings == null)
                    throw new NullReferenceException("Settings not initialized");

                var authResult = await _clientCredential!.GetTokenAsync(new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" }));

                var accessToken = authResult.Token;

                return accessToken;
            }
            catch (MsalClientException ex)
            {
                Console.WriteLine($"Error getting user token: {ex.Message}");
                return string.Empty;
            }
        }

        public static Task<User> GetUserAsync()
        {
            if (_userClient == null)
            {
                throw new NullReferenceException("Graph has not been initialized for user auth");
            }

            return _userClient.Me.GetAsync();
        }
        

        public static async Task SendMailAsync(string subject, string recipient)
        {
            if (_userClient == null)
            {
                throw new NullReferenceException("Graph has not been initialized for user auth");
            }
        
            var requestBody = new SendMailPostRequestBody
            {
                Message = new Message
                {
                    Subject = subject,
                    Body = new ItemBody
                    {
                        Content = "hello this is a test message",
                        ContentType = BodyType.Text
                    },
                    ToRecipients = new List<Recipient>(new Recipient[]
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = recipient
                            }
                        }
                    })
                },
                SaveToSentItems = true
            };
            await _userClient.Me.SendMail.PostAsync(requestBody);
        }
        
        public static async Task CreateOnlineMeetingAsync(string[] participants)
        {
            var userId= await GetUserIdAsync();
            if (_userClient == null)
            {
                throw new NullReferenceException("Graph has not been initialized for user auth");
            }
        
            DateTime startTime = DateTime.UtcNow.AddMinutes(2);
            DateTime endTime = DateTime.UtcNow.AddHours(1);
        
            var onlineMeeting = new OnlineMeeting
            {
                StartDateTime = startTime,
                EndDateTime = endTime,
                Subject = "Test Meeting",
                LobbyBypassSettings = new LobbyBypassSettings
                {
                    Scope = LobbyBypassScope.Invited
                },
            };
        
            var createdMeeting = await _userClient.Users[userId].OnlineMeetings
                .PostAsync(onlineMeeting);
        
            Console.WriteLine("Meeting created. Join URL: " + createdMeeting.JoinWebUrl);
        
            var newEvent = new Event
            {
                Subject = "Test Graph API Meeting",
                Start = new DateTimeTimeZone
                {
                    DateTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone = "UTC"
                },
                End = new DateTimeTimeZone
                {
                    DateTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone = "UTC"
                },
                Attendees = participants.Select(participant => new Attendee
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = participant
                    },
                    Type = AttendeeType.Required
                }).ToList()
            };
            await _userClient.Users[userId].Events
                .PostAsync(newEvent);
        }
    }
}