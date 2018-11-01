using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PoeItemFinder
{
    class Currency
    {
        static Dictionary<string, float> currencyMap = new Dictionary<string, float> {
            { "chaos", 1 },
            { "c", 1 },
            { "exalted", 80 },
            { "exa", 80 },
            { "ex", 80 },
            { "fuse", 0.66f},
            { "fuseing", 0.66f},
            { "alch", 0.3f},
            { "alchemy", 0.3f},
            { "alc", 0.3f},
            { "alt", 0.08f},
            { "alteration", 0.08f},
            { "jew", 0.16f},
            { "jeweller", 0.16f}
        };

        private static string _validCurrencyPattern = null;
        private static string ValidCurrencyPattern
        {
            get
            {
                if(_validCurrencyPattern == null)
                {
                    string pattern = "(";
                    var keys = currencyMap.Keys.ToList();
                    for(int i = 0; i < keys.Count; i++)
                    {
                        pattern += keys[i] + (i==keys.Count-1 ? "": "|");
                    }
                    pattern += ")";
                    _validCurrencyPattern = pattern;
                }
                return _validCurrencyPattern;
            }
        }

        /// <summary>
        /// returns the price in chaos orbs from the priceString
        /// </summary>
        /// <param name="priceString"></param>
        /// <returns></returns>
        public static float getItemPrice(string priceString)
        {
            if (priceString == null) return -1f;
            string amountPattern = @"\d+(\.\d*)?|\.\d+";
            var amountMatch = Regex.Match(priceString, amountPattern);

            float amount = float.Parse(amountMatch.Value, System.Globalization.CultureInfo.InvariantCulture);
            var currencyMatch = Regex.Match(priceString, ValidCurrencyPattern);
            if (!currencyMap.ContainsKey(currencyMatch.Value))
            {
                return -1;
            }
            float currencyModifier = currencyMap[currencyMatch.Value];
            return amount * currencyModifier;
        }

        /// <summary>
        /// returns the string that contains the price of the item
        /// </summary>
        /// <param name="note"></param>
        /// <param name="stashName"></param>
        /// <returns></returns>
        public static string getPriceString(string note, string stashName)
        {
            string validPattern = @"(~?b\/o) (\d+(\.\d*)?|\.\d+) " + ValidCurrencyPattern;
            if (note != null)
            {
                if (Regex.Match(note, validPattern).Success)
                { //check the note first since it overriders the stash
                    return note;
                }
            }
            if (stashName != null)
            {
                if (Regex.Match(stashName, validPattern).Success)
                {
                    return stashName;
                }
            }
            return null; //could not find a valid price in the strings
        }
    }
}
