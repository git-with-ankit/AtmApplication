using System;
using System.Threading.Tasks;
using AtmApplication.Backend.DTOs;
using AtmApplication.Backend.Exceptions;
using AtmApplication.Backend.Services;
using AtmApplication.ConsoleUI.ApplicationConstants;
using AtmApplication.DataAccess.Entities;
using AtmApplication.ConsoleUI.Helper;
using AtmApplication.ConsoleUI.Model;
using AtmApplication.ConsoleUI.Menus;

namespace AtmApplication.ConsoleUI.Menus
{
    internal sealed class AdminMenu
    {
        private readonly IIdentityService _identityService;
        private readonly ITransactionService _transactionService;
        private readonly IValidationService _validationService;

        public AdminMenu(IIdentityService identityService, ITransactionService transactionService, IValidationService validationService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public async Task HandleAdminMenuAsync()
        {
            DisplayHelper.DisplayMessage(UIMessages.WelcomeAdmin);
            DisplayHelper.DisplayMessage(UIMessages.AdminMenu);
            DisplayHelper.DisplayPrompt(UIMessages.EnterChoice + " ");

            var choice = InputHelper.GetEnumInput<AdminMenuOption>();
            
            if (choice == AdminMenuOption.Login)
            {
                await HandleAdminLoginAsync();
            }
        }

        private async Task HandleAdminLoginAsync()
        {
            try
            {
                DisplayHelper.DisplayMessage(UIMessages.WelcomeAdminLogin);
                
                string username = InputHelper.GetUsernameInput(_validationService);
                int pin = InputHelper.GetPinInput();

                var loginDto = new LoginDto
                {
                    Username = username,
                    Pin = pin
                };

                var result = await _identityService.LoginAsync(loginDto);

                if (!result.IsLoginSuccessful || !result.IsAdmin)
                {
                    DisplayHelper.DisplayError("Invalid admin credentials.");
                    return;
                }

                DisplayHelper.DisplaySuccess(string.Format(UIMessages.LoginSuccess, username));
                await HandleAdminActionsAsync(username);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Admin login failed: {ex.Message}");
            }
        }

        private async Task HandleAdminActionsAsync(string adminUsername)
        {
            while (true)
            {
                DisplayHelper.DisplayMessage(UIMessages.AdminActionMenu);
                DisplayHelper.DisplayPrompt(UIMessages.EnterChoice + " ");

                var action = InputHelper.GetEnumInput<AdminActionOption>();

                switch (action)
                {
                    case AdminActionOption.ViewFrozenAccounts:
                        await HandleViewFrozenAccountsAsync(adminUsername);
                        break;
                    case AdminActionOption.UnfreezeAccount:
                        await HandleUnfreezeAccountAsync(adminUsername);
                        break;
                    case AdminActionOption.ViewAtmBalance:
                        await HandleViewAtmBalanceAsync(adminUsername);
                        break;
                    case AdminActionOption.DepositToAtm:
                        await HandleDepositToAtmAsync(adminUsername);
                        break;
                    case AdminActionOption.ViewAtmTransactions:
                        await HandleViewAtmTransactionsAsync(adminUsername);
                        break;
                    case AdminActionOption.ChangeAdmin:
                        await HandleChangeAdminAsync(adminUsername);
                        return; 
                    case AdminActionOption.Exit:
                        DisplayHelper.DisplayMessage(UIMessages.Exiting);
                        return;
                }
            }
        }

        private async Task HandleViewFrozenAccountsAsync(string adminUsername)
        {
            try
            {
                var frozenAccounts = await _identityService.GetFrozenAccountsAsync(adminUsername);
                DisplayHelper.DisplayFrozenAccounts(frozenAccounts);
            }
            catch (UnauthorizedAccessException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Failed to retrieve frozen accounts: {ex.Message}");
            }
        }

        private async Task HandleUnfreezeAccountAsync(string adminUsername)
        {
            try
            {
                string username = InputHelper.GetUsernameInput(_validationService);

                var unfreezeDto = new UnfreezeUserDto
                {
                    AdminUsername = adminUsername,
                    Username = username
                };

                bool success = await _identityService.UnfreezeAccountAsync(unfreezeDto);
                
                if (success)
                {
                    DisplayHelper.DisplaySuccess(UIMessages.UnfreezeSuccess);
                }
                else
                {
                    DisplayHelper.DisplayError("Failed to unfreeze account.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Unfreeze operation failed: {ex.Message}");
            }
        }

        private async Task HandleViewAtmBalanceAsync(string adminUsername)
        {
            try
            {
                var atmBalance = await _transactionService.GetAtmBalanceAsync(adminUsername);
                DisplayHelper.DisplayAtmBalance(atmBalance.TotalBalance);
            }
            catch (UnauthorizedAccessException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Failed to retrieve ATM balance: {ex.Message}");
            }
        }

        private async Task HandleDepositToAtmAsync(string adminUsername)
        {
            try
            {
                double amount = InputHelper.GetAmountInput();

                var depositDto = new DepositCashDto
                {
                    AdminUsername = adminUsername,
                    Amount = amount
                };

                bool success = await _transactionService.DepositToAtmAsync(depositDto);
                
                if (success)
                {
                    DisplayHelper.DisplaySuccess(string.Format(UIMessages.AtmDepositSuccess, amount.ToString("F2")));
                }
                else
                {
                    DisplayHelper.DisplayError("ATM deposit failed.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"ATM deposit failed: {ex.Message}");
            }
        }

        private async Task HandleViewAtmTransactionsAsync(string adminUsername)
        {
            try
            {
                var history = await _transactionService.GetTransactionHistoryAsync(adminUsername, Backend.ApplicationConstants.Constants.DefaultTransactionHistoryCount);
                DisplayHelper.DisplayMessage("\n--- Last 5 ATM Transactions ---");
                DisplayHelper.DisplayTransactionHistory(history);
            }
            catch (UnauthorizedAccessException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Failed to retrieve ATM transactions: {ex.Message}");
            }
        }

        private async Task HandleChangeAdminAsync(string adminUsername)
        {
            try
            {
                DisplayHelper.DisplayMessage("\n--- Change Admin ---");
                DisplayHelper.DisplayMessage("WARNING: You will lose admin privileges after this operation!");
                
                string newAdminUsername = InputHelper.GetUsernameInput(_validationService);
                int newAdminPin = InputHelper.GetPinInput();

                var changeAdminDto = new ChangeAdminDto
                {
                    CurrentAdminUsername = adminUsername,
                    NewAdminUsername = newAdminUsername,
                    NewAdminPin = newAdminPin
                };

                bool success = await _identityService.ChangeAdminAsync(changeAdminDto);
                
                if (success)
                {
                    DisplayHelper.DisplaySuccess(UIMessages.ChangeAdminSuccess);
                    DisplayHelper.DisplayMessage($"New admin: {newAdminUsername}");
                    DisplayHelper.DisplayMessage("You have been logged out.");
                }
                else
                {
                    DisplayHelper.DisplayError("Failed to change admin. User not found or invalid operation.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Change admin operation failed: {ex.Message}");
            }
        }
    }
}
