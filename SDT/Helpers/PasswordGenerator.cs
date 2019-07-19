using System;
using System.Collections.Generic;
using System.Linq;

namespace SDT.Helpers
{
    public class PasswordGenerator
    {
        private readonly Pages.PasswordWindow _passwordWindow;

        public PasswordGenerator(Pages.PasswordWindow PasswordWindow)
        {
            _passwordWindow = PasswordWindow;
        }

        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public string Generate()
        {
            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            ".?!@$%*"                        // non-alphanumeric
            };
            int requiredLength = 8;
            int requiredUniqueChars = 3;
            bool requireDigit = true;
            bool requireLowercase = true;
            bool requireNonAlphanumeric = true;
            bool requireUppercase = true;

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (requireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (requireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (requireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (requireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < requiredLength
                || chars.Distinct().Count() < requiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            string test = string.Join("", chars);
            _passwordWindow.passwordTextBox.Text = test;

            return null;
        }
       
    }
}
