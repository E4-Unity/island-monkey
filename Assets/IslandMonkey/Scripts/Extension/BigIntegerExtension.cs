﻿using System.Linq;
using System.Numerics;

namespace IslandMonkey.Utils
{
    public static class BigIntExtension
    {
        const int AsciiAlphabetStart = 'a';
        const int AsciiAlphabetEnd = 'z';

        public static BigInteger Clamp(this in BigInteger value, in BigInteger min, in BigInteger max)
        {
	        BigInteger result = value;

	        if (value > max)
	        {
		        result = max;
	        }
	        else if (value < min)
	        {
		        result = min;
	        }

	        return result;
        }

        public static string FormatLargeNumber(this BigInteger number)
        {
	        if (number < 1000)
	        {
		        return number.ToString();
	        }
	        else
	        {
		        int index = -1;
		        while (number >= 1000)
		        {
			        number /= 1000;
			        index++;
		        }

		        if (index <= 26)
		        {
			        return number.ToString() + (char)(index + AsciiAlphabetStart);
		        }
		        else
		        {
			        return "999z"; // 최대값
		        }
	        }
        }

        public static BigInteger ToBigInteger(this string numString)
        {
	        char lastChar = numString[^1];

	        if (lastChar < AsciiAlphabetStart || lastChar > AsciiAlphabetEnd)
	        {
		        return BigInteger.Parse(numString);
	        }
	        else
	        {
		        var index = lastChar - AsciiAlphabetStart + 1; // lastChar == a 일 경우, index = 1
		        numString = numString.Remove(numString.Length - 1);
		        return BigInteger.Parse(numString) * BigInteger.Pow(1000, index);
	        }
        }
    }
}
