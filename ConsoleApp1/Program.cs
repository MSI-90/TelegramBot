namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                TelegramBot.GetUpdates();
                Thread.Sleep(1000);
            }  
        }
    }
}