using AtmApplication.ConsoleUI.Dependencies;
using System.Threading.Tasks;

namespace AtmApplication.ConsoleUI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var app = new AtmApp();
            await app.MainMenu.RunAsync();
        }
    }
}