using AtmApplication.Backend.ApplicationConstants;
using AtmApplication.Backend.Services;
using AtmApplication.ConsoleUI.ApplicationConstants;


namespace AtmApplication.ConsoleUI.Helper
{
    internal static class InputHelper
    {
        public static TEnum GetEnumInput<TEnum>() where TEnum : struct, Enum
        {
            while (true)
            {
                string input = Console.ReadLine()?.Trim() ?? string.Empty;
                if (int.TryParse(input, out int value) && Enum.IsDefined<TEnum>((TEnum)(object)value))
                {
                    return (TEnum)(object)value;
                }
                DisplayHelper.DisplayError(UIMessages.InvalidInput);
                DisplayHelper.DisplayPrompt(UIMessages.EnterChoice + " ");
            }
        }

        public static string GetStringInput(string prompt)
        {
            DisplayHelper.DisplayPrompt(prompt + " ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        public static int GetPinInput()
        {
            while (true)
            {
                var prompt = string.Format(UIMessages.EnterPin, Constants.PinLength);
                DisplayHelper.DisplayPrompt(prompt + " ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (int.TryParse(input, out int pin) && input.Length == Constants.PinLength)
                {
                    return pin;
                }
                DisplayHelper.DisplayError(string.Format(UIMessages.InvalidPin, Constants.PinLength));
            }
        }

        public static double GetAmountInput()
        {
            while (true)
            {
                DisplayHelper.DisplayPrompt(UIMessages.EnterAmount + " ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;
                
                if (double.TryParse(input, out double amount) && amount > 0)
                {
                    return amount;
                }
                DisplayHelper.DisplayError(UIMessages.InvalidAmount);
            }
        }

        public static string GetUsernameInput(IValidationService validationService)
        {
            while (true)
            {
                DisplayHelper.DisplayPrompt(UIMessages.EnterUsername + " ");
                string username = Console.ReadLine()?.Trim() ?? string.Empty;
                
                if (validationService.ValidateUsernameFormat(username))
                {
                    return username;
                }
                DisplayHelper.DisplayError(string.Format(UIMessages.InvalidUsername, Constants.MinUsernameLength, Constants.MaxUsernameLength));
            }
        }

        public static void PressEnterToContinue()
        {
            DisplayHelper.DisplayMessage(UIMessages.PressEnterToContinue);
            Console.ReadLine();
        }
    }
}
