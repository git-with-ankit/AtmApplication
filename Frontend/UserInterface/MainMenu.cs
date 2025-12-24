using System;
using System.Threading.Tasks;
using AtmApplication.Frontend.Helper;
using AtmApplication.Frontend.Model;
using AtmApplication.Frontend.UserInterface;

namespace AtmApplication.Frontend.UserInterface
{
    internal class MainMenu
    {
        private readonly ConsoleUI _consoleUI;
        private readonly UserMenu _userMenu;
        private readonly AdminMenu _adminMenu;

        public MainMenu(ConsoleUI consoleUI, UserMenu userMenu, AdminMenu adminMenu)
        {
            _consoleUI = consoleUI ?? throw new ArgumentNullException(nameof(consoleUI));
            _userMenu = userMenu ?? throw new ArgumentNullException(nameof(userMenu));
            _adminMenu = adminMenu ?? throw new ArgumentNullException(nameof(adminMenu));
        }

        public async Task RunAsync()
        {
            _consoleUI.Clear();
            _consoleUI.DisplayMessage(UIMessages.WelcomeMessage);

            while (true)
            {
                _consoleUI.DisplayMessage(UIMessages.SelectRole);
                Console.Write(UIMessages.EnterChoice + " ");

                var role = InputHelper.GetEnumInput<RoleOption>();

                await HandleRoleSelectionAsync(role);
            }
        }

        private async Task HandleRoleSelectionAsync(RoleOption role)
        {
            switch (role)
            {
                case RoleOption.User:
                    await _userMenu.HandleUserMenuAsync();
                    break;
                case RoleOption.Admin:
                    await _adminMenu.HandleAdminMenuAsync();
                    break;
            }
        }
    }
}
