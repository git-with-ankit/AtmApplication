using System;
using System.Threading.Tasks;
using AtmApplication.ConsoleUI.ApplicationConstants;
using AtmApplication.Frontend.Model;
using AtmApplication.ConsoleUI.Helper;

namespace AtmApplication.ConsoleUI.Menus
{
    internal class MainMenu
    {
        private readonly UserMenu _userMenu;
        private readonly AdminMenu _adminMenu;

        public MainMenu(UserMenu userMenu, AdminMenu adminMenu)
        {
            _userMenu = userMenu ?? throw new ArgumentNullException(nameof(userMenu));
            _adminMenu = adminMenu ?? throw new ArgumentNullException(nameof(adminMenu));
        }

        public async Task RunAsync()
        {
            DisplayHelper.Clear();
            DisplayHelper.DisplayMessage(UIMessages.WelcomeMessage);

            while (true)
            {
                DisplayHelper.DisplayMessage(UIMessages.SelectRole);
                DisplayHelper.DisplayPrompt(UIMessages.EnterChoice + " ");

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
