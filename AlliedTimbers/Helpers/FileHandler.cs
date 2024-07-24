using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AlliedTimbers.Helpers;

public static class FileHandler
{
    public static class PhoneNumber
    {
        public static bool IsLikelyPhoneNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            if (!input.All(char.IsDigit) || input.Length < 7)
                return false;

            return true;
        }

    }
}