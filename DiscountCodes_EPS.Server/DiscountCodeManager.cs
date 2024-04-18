using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscountCodes_EPS.Server
{
    class DiscountCodeManager
    {
        private List<string> generatedCodes;
        private HashSet<string> usedCodes;

        public DiscountCodeManager()
        {
            generatedCodes = new List<string>();
            usedCodes = new HashSet<string>();
            LoadGeneratedCodes();
            LoadUsedCodes();
        }

        public bool GenerateAndSaveDiscountCodes(ushort count, byte length)
        {
            HashSet<string> newCodes = new HashSet<string>();

            while (newCodes.Count < count)
            {
                string code = GenerateDiscountCode(length);
                if (!generatedCodes.Contains(code) && !newCodes.Contains(code))
                {
                    newCodes.Add(code);
                }
            }

            generatedCodes.AddRange(newCodes);

            SaveGeneratedCodes();
            return true;
        }

        public byte UseDiscountCode(string code)
        {
            if (generatedCodes.Contains(code) && !usedCodes.Contains(code))
            {
                usedCodes.Add(code);
                SaveUsedCodes();
                return 1;
            }
            else if (!generatedCodes.Contains(code))
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }

        private string GenerateDiscountCode(byte length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        private void LoadGeneratedCodes()
        {
            try
            {
                if (File.Exists("generated_codes.txt"))
                {
                    generatedCodes = new List<string>(File.ReadAllLines("generated_codes.txt"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading generated codes: {ex.Message}");
            }
        }

        private void SaveGeneratedCodes()
        {
            try
            {
                File.WriteAllLines("generated_codes.txt", generatedCodes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving generated codes: {ex.Message}");
            }
        }

        private void LoadUsedCodes()
        {
            try
            {
                if (File.Exists("used_codes.txt"))
                {
                    usedCodes = new HashSet<string>(File.ReadAllLines("used_codes.txt"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading used codes: {ex.Message}");
            }
        }

        private void SaveUsedCodes()
        {
            try
            {
                File.WriteAllLines("used_codes.txt", usedCodes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving used codes: {ex.Message}");
            }
        }

    }
}
