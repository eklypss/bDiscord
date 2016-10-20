
namespace bDiscord.Classes
{
    static class Printer
    {
        public static void Print(string Message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + Message);
        }
    }
}
