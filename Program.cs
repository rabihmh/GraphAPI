
namespace NetwaysPoc
{
    internal class Program
    {
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
            Console.WriteLine("1. Display access token");
            Console.WriteLine("2. Send mail");
            Console.WriteLine("3. Create online meeting");
        }

        static int GetChoiceFromUser()
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 3)
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
                    await DisplayAccessTokenAsync();
                    break;
                case 2:
                    await SendMailAsync();
                    break;
                case 3:
                    await CreateOnlineMeetingAsync();
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please try again.");
                    break;
            }
        }

        static void InitializeGraph(Settings settings)
        {
            GraphHelper.InitializeGraphForUserAuth(settings,
                (info, cancel) =>
                {
                    Console.WriteLine(info.Message);
                    return Task.FromResult(0);
                });
        }
        static async Task DisplayAccessTokenAsync()
        {
            try
            {
                var userToken = await GraphHelper.GetUserTokenAsync();
                Console.WriteLine($"User token: {userToken}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user access token: {ex.Message}");
            }

        }
        static async Task SendMailAsync()
        {
            try
            {
                var user = await GraphHelper.GetUserAsync();
                var userEmail = user?.Mail ?? user?.UserPrincipalName;
                Console.WriteLine("Enter the email that u want to send to ");
                var emailRead = Console.ReadLine();
                if (string.IsNullOrEmpty(userEmail)|| String.IsNullOrEmpty(emailRead))
                {
                    Console.WriteLine("Couldn't get your email address, canceling...");
                    return;
                }
                Console.WriteLine("sent from :" + userEmail);
                await GraphHelper.SendMailAsync("Testing Microsoft Graph", emailRead);

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
                var participants = new string[] { "rfbarakat@netways.com", "rabihmahmoud772@gmail.com"};
                 await GraphHelper.CreateOnlineMeetingAsync(participants);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating meeting: {ex.Message}");
            }
        }

    }
}
