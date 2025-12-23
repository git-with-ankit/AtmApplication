using System;
using System.Threading.Tasks;
using Frontend.Helper;
using Frontend.Model;
using Frontend.UserInterface;

namespace Frontend.UserInterface
{
    public class MainMenu
    {
        private readonly ConsoleUI _consoleUI;
        private readonly UserMenu _userMenu;
        private readonly AdminMenu _adminMenu;

        public MainMenu(ConsoleUI consoleUI, UserMenu userMenu, AdminMenu adminMenu)
        {
            _consoleUI = consoleUI;
            _userMenu = userMenu;
            _adminMenu = adminMenu;
        }

        public async Task RunAsync()
        {
            _consoleUI.Clear();
            _consoleUI.DisplayMessage(UIMessages.WelcomeMessage);

            while (true)
            {
                _consoleUI.DisplayMessage(UIMessages.SelectRole);
                Console.Write(UIMessages.EnterChoice + " ");

                int choice = InputHelper.GetIntegerInput((int)RoleOption.User, (int)RoleOption.Admin);
                var role = (RoleOption)choice;

                await HandleRoleSelectionAsync(role);
            }
        }

        private async Task HandleRoleSelectionAsync(RoleOption role)
        {
            switch (role)
            {
                case RoleOption.User:
                    await _userMenu.HandleUserFlowAsync();
                    break;
                case RoleOption.Admin:
                    await _adminMenu.HandleAdminFlowAsync();
                    break;
            }
        }
    }
}
