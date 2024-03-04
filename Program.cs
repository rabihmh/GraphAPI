using System;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using NetwaysPoc.GraphServices;
namespace NetwaysPoc
{
    internal class Program
    {
        private static GraphService _graphService;

        static async Task Main(string[] args)
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

                await _graphService.SendEmailAsync(message);

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
                var teamsService = new TeamsService();
                var eventService=new EventService();

                var onlineMeeting = teamsService.CreateTeamsMeeting("Test Meeting", DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1));

                var participants = new List<string>(){ "rabihmahmoud772@gmail.com", "rfbarakat@netways.com" };

                onlineMeeting = teamsService.AddMeetingParticipants(onlineMeeting, participants);

              var meeting=  await _graphService.CreateOnlineMeeting(onlineMeeting);
            
               var newEvent= eventService.CreateEvent("Test Meeting", DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1), participants);
                await _graphService.CreateEvent(newEvent);
                Console.WriteLine("Online meeting created.");
                Console.WriteLine("Url: " + meeting?.JoinWebUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating meeting: {ex.Message}");
            }
        }
    }
}

