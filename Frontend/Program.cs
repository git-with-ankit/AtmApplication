using Frontend.Dependencies;
using System.Threading.Tasks;

namespace Frontend
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var app = new AtmApp();
            await app.MainMenu.RunAsync();
        }
    }
}//atm interface separate 
//backend code review
//ui exception handling
//access modifiers
//comments
//factory service
//input doing validation
//dto change , input validation change
//freeze bug