using System;
using System.Text;

namespace Vecc.Base32.Internal
{
    public class Base32 : IBase32
    {

        public byte[] DecodeToBytes(string base32)
        {
            if (string.IsNullOrEmpty(base32))
            {
                throw new ArgumentNullException(nameof(base32));
            }

            char[] base32Array;

            //converting the input to a char array once is pretty fast.
            if (base32[base32.Length - 1] == '=')
            {
                base32Array = base32.TrimEnd('=').ToCharArray(); //remove padding characters
            }
            else
            {
                base32Array = base32.ToCharArray();
            }

            var byteCount = base32Array.Length * 5 / 8; //this must be TRUNCATED
            var result = new byte[byteCount];

            var bitsRemaining = 8;
            var curByte = 0;
            var arrayIndex = 0;
            int mask;

            foreach (var c in base32Array)
            {
                var cValue = this.CharToValue(c);

                if (bitsRemaining > 5)
                {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (curByte | mask);
                    result[arrayIndex++] = (byte)curByte;
                    curByte = (cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            //if we didn't end with a full byte
            if (arrayIndex != byteCount)
            {
                result[arrayIndex] = (byte)curByte;
            }

            return result;
        }

        public string DecodeToString(string base32)
        {
            var bytes = this.DecodeToBytes(base32);
            var result = Encoding.ASCII.GetString(bytes);

            return result;
        }

        public string Encode(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            var result = this.Encode(bytes);
            return result;
        }

        public string Encode(byte[] input)
        {
            if (input == null ||
                input.Length == 0)
            {
                throw new ArgumentNullException("input");
            }

            var charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
            var result = new char[charCount];

            var nextChar = 0;
            var bitsRemaining = 5;
            var arrayIndex = 0;

            foreach (var b in input)
            {
                nextChar = nextChar | (b >> (8 - bitsRemaining));
                result[arrayIndex++] = (char)this.ValueToChar(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (b >> (3 - bitsRemaining)) & 31;
                    result[arrayIndex++] = (char)this.ValueToChar(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (b << bitsRemaining) & 31;
            }

            //if we didn't end with a full char
            if (arrayIndex != charCount)
            {
                result[arrayIndex++] = (char)this.ValueToChar(nextChar);
                while (arrayIndex < charCount)
                {
                    result[arrayIndex++] = '='; //padding
                }
            }

            return new string(result);
        }

        private int CharToValue(char c)
        {
            //65-90
            if (c >= 'A' &&
                c <= 'Z')
            {
                return c - 'A';
            }

            //50-57
            if (c >= '2' &&
                c <= '7')
            {
                //since 2 is actual 27, and the ASCII code of 2 is 50, this is for performance
                return c - 24;
            }

            //97-122 == lowercase letters, effectively converting them to upper case
            if (c >= 'a' &&
                c <= 'z')
            {
                return c - 'a'; //this effectively converts the lowercase letters to upper.
            }

            throw new ArgumentException("Character is not a Base32 character.", "c");
        }

        private int ValueToChar(int value)
        {
            if (value < 26)
            {
                return (value + 65); //letter
            }

            return (value + 24); //number
        }
    }
}
