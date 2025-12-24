using System;
using AtmApplication.Backend.ApplicationConstants;

namespace AtmApplication.Frontend.Helper
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
                Console.WriteLine(UIMessages.InvalidInput);
                Console.Write(UIMessages.EnterChoice + " ");
            }
        }

        public static string GetStringInput(string prompt)
        {
            Console.Write(prompt + " ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        public static int GetPinInput()
        {
            while (true)
            {
                var prompt = string.Format(UIMessages.EnterPin, Constants.PinLength);
                Console.Write(prompt + " ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (int.TryParse(input, out int pin) && input.Length == Constants.PinLength)
                {
                    return pin;
                }
                Console.WriteLine(string.Format(UIMessages.InvalidPin, Constants.PinLength));
            }
        }

        public static double GetAmountInput()
        {
            while (true)
            {
                Console.Write(UIMessages.EnterAmount + " ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;
                
                if (double.TryParse(input, out double amount) && amount > 0)
                {
                    return amount;
                }
                Console.WriteLine(UIMessages.InvalidAmount);
            }
        }

        public static string GetUsernameInput()
        {
            while (true)
            {
                Console.Write(UIMessages.EnterUsername + " ");
                string username = Console.ReadLine()?.Trim() ?? string.Empty;
                
                if (IsValidUsername(username))
                {
                    return username;
                }
                Console.WriteLine(string.Format(UIMessages.InvalidUsername, Constants.MinUsernameLength, Constants.MaxUsernameLength));
            }
        }

        private static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < Constants.MinUsernameLength || username.Length > Constants.MaxUsernameLength)
            {
                return false;
            }

            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    return false;
                }
            }

            return true;
        }

        public static void PressEnterToContinue()
        {
            Console.WriteLine(UIMessages.PressEnterToContinue);
            Console.ReadLine();
        }
    }
}
