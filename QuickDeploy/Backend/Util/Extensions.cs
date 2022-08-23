using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickDeploy.Backend.Enums;

namespace QuickDeploy.Backend.Util
{
    public static class Extensions
    {

        public static bool ToBooleanValue(this ValidationResult vr)
        {
            return vr == ValidationResult.Valid;
        }

        public static string GetSafeFileName(this string input)
        {
            char[] illegalChars = new char[] { '#', '%', '<', '>', '$', '+', '!', '`', '&', '*', '\'', '|', '{', '}', '?', '\"', '=', '@' };
            foreach (var c in illegalChars)
                input = input.Replace(c.ToString(), "");

            return input;
        }

    }
}
