using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;

namespace NetwaysPoc
{
    class GraphHelper
    {
        private static Settings? _settings;
        private static DeviceCodeCredential? _deviceCodeCredential;
        private static GraphServiceClient? _userClient;

        public static void InitializeGraphForUserAuth(Settings settings,
            Func<DeviceCodeInfo, CancellationToken, Task> deviceCodePrompt)
        {
            _settings = settings;
            _deviceCodeCredential = new DeviceCodeCredential(deviceCodePrompt,
                settings.TenantId, settings.ClientId);

            _userClient = new GraphServiceClient(_deviceCodeCredential, settings.GraphUserScopes);
        }

        public static async Task<string> GetUserTokenAsync()
        {
            if (_deviceCodeCredential == null)
            {
                throw new NullReferenceException("Graph has not been initialized for user auth");
            }

            if (_settings == null || _settings.GraphUserScopes == null)
            {
                throw new ArgumentNullException(nameof(_settings.GraphUserScopes), "Argument 'scopes' cannot be null");
            }

            var context = new TokenRequestContext(_settings.GraphUserScopes);
            var response = await _deviceCodeCredential.GetTokenAsync(context);
            return response.Token;
        }

        public static Task<User> GetUserAsync()
        {
            if (_userClient == null)
            {
                throw new NullReferenceException("Graph has not been initialized for user auth");
            }

            return  _userClient.Me.GetAsync();
        }

        public static async Task SendMailAsync(string subject, string recipient)
        {
            if (_userClient == null)
            {
                throw new NullReferenceException("Graph has not been initialized for user auth");
            }

            var requestBody=new SendMailPostRequestBody
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

            var createdMeeting = await _userClient.Me.OnlineMeetings
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
            await _userClient.Me.Events.PostAsync(newEvent);
        }

    }
}
