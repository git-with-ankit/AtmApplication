using System;
using Backend.ApplicationConstants;
using Backend.ApplicationConstants;

namespace Frontend.Helper
{
    public class InputHelper
    {
        public static int GetIntegerInput(int minValue, int maxValue)
        {
            while (true)
            {
                string input = Console.ReadLine()?.Trim() ?? string.Empty;
                if (int.TryParse(input, out int value) && value >= minValue && value <= maxValue)
                {
                    return value;
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
            return GetPinInput(UIMessages.EnterPin);
        }

        public static int GetPinInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;
                
                if (int.TryParse(input, out int pin) && input.Length == Constants.PinLength)
                {
                    return pin;
                }
                Console.WriteLine(UIMessages.InvalidPin);
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
                Console.WriteLine(UIMessages.InvalidUsername);
            }
        }

        private static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 20)
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
