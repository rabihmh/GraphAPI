using Microsoft.Graph.Models;
using NetwaysPoc.GraphServices;
namespace NetwaysPoc
{
    internal class Program
    {
        private static GraphService? _graphService;

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Netways PoC \n");

            var settings = Settings.LoadSettings();

            InitializeGraph(settings);

            int choice = -1;

            while (choice != 0)
            {
                DisplayMenu();

                choice = GetChoiceFromUser();

                await ProcessChoiceAsync(choice);
            }

            Console.WriteLine("Goodbye...");
        }

        static void DisplayMenu()
        {
            Console.WriteLine("Please choose one of the following options:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Send mail");
            Console.WriteLine("2. Create online meeting");
        }

        static int GetChoiceFromUser()
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 2)
            {
                Console.WriteLine("Invalid choice! Please try again.");
            }
            return choice;
        }

        static async Task ProcessChoiceAsync(int choice)
        {
            switch (choice)
            {
                case 0:
                    break;
                case 1:
                    await SendMailAsync();
                    break;
                case 2:
                    await CreateOnlineMeetingAsync();
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please try again.");
                    break;
            }
        }

        static void InitializeGraph(Settings settings)
        {
            _graphService = new GraphService(settings);
        }

        static async Task SendMailAsync()
        {
            try
            {
                Console.WriteLine("Enter the email address you want to send to:");
                var recipientEmail = Console.ReadLine();
                if (String.IsNullOrEmpty(recipientEmail))
                {
                    Console.WriteLine("Couldn't get the recipient's email address, canceling...");
                    return;
                }

                var emailService = new EmailService();
                var message = emailService.CreateStandardEmail(recipientEmail, "Testing Microsoft Graph", "Hello, this is a test email.");

                if (_graphService != null) await _graphService.SendEmailAsync(message);

                Console.WriteLine("Mail sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending mail: {ex.Message}");
            }
        }

        static async Task CreateOnlineMeetingAsync()
        {
            try
            {
                var subject = GetMeetingSubject();
                var participants = GetParticipants();
                if (participants.Count == 0)
                {
                    Console.WriteLine("Couldn't get the participants' email addresses, canceling...");
                    return;
                }
                
                var startDate=GetStartDate();
                var endDate=GetEndDate(startDate);
                Console.WriteLine("Creating online meeting...");
                var onlineMeeting = CreateTeamsMeeting();
                onlineMeeting = TeamsService.AddMeetingParticipants(onlineMeeting, participants);

                if (_graphService != null)
                {
                    var meeting = await _graphService.CreateOnlineMeeting(onlineMeeting);
                    var newEvent = CreateEvent(subject, startDate, endDate, participants);
                    await _graphService.CreateEvent(newEvent);
                    Console.WriteLine("Online meeting created.");
                    Console.WriteLine("Url: " + meeting?.JoinWebUrl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating meeting: {ex.Message}");
            }
        }

        static List<string> GetParticipants()
        {
            Console.WriteLine("Please enter email addresses of participants separated by a comma:");
            var participantsString = Console.ReadLine();
            var participants = new List<string>();
            foreach (var participant in participantsString.Split(','))
            {
                if (!participant.Contains('@'))
                {
                    Console.WriteLine($"Invalid email address: {participant}");
                    continue;
                }
                participants.Add(participant);
            }
            return participants;
        }
        static string GetMeetingSubject()
        {
            Console.WriteLine("Please enter the subject of the meeting:");
            return Console.ReadLine() ?? throw new InvalidOperationException("Meeting subject cannot be null");
        }
        static OnlineMeeting CreateTeamsMeeting()
        {
            return TeamsService.CreateTeamsMeeting("Test Meeting", DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1));
        }

        static Event CreateEvent(string name, DateTimeOffset startDate, DateTimeOffset endDate, List<string> participants)
        {
            var eventService = new EventService();
            return eventService.CreateEvent(name, startDate, endDate, participants);
        }
        static DateTimeOffset GetStartDate()
        {
            Console.WriteLine("Please enter the start date of the meeting, after how many minutes:");
            var minutes = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException("Minutes cannot be null"));
            return DateTimeOffset.Now.AddMinutes(minutes);
        }
        static DateTimeOffset GetEndDate(DateTimeOffset startDate)
        {
            Console.WriteLine("Please enter the end date of the meeting, after how many minutes:");
            var minutes = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException("Minutes cannot be null"));
            return startDate.AddMinutes(minutes);
        }

    }
}

