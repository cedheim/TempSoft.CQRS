﻿using System;
using System.Security.Cryptography;
using System.Threading;

namespace TempSoft.CQRS.Common.Extensions
{
    public static class StringExtensions
    {
        private static readonly ThreadLocal<SHA256> _hasher =
            new ThreadLocal<SHA256>(() => new SHA256CryptoServiceProvider());

        private static SHA256 Hasher => _hasher.Value;

        public static long GetHashCode64(this string txt)
        {
            var byteContents = System.Text.Encoding.UTF8.GetBytes(txt);
            var hash = Hasher.ComputeHash(byteContents);

            //32Byte hashText separate
            //hashCodeStart = 0~7  8Byte
            //hashCodeMedium = 8~23  8Byte
            //hashCodeEnd = 24~31  8Byte
            //and Fold
            var hashCodeStart = BitConverter.ToInt64(hash, 0);
            var hashCodeMedium = BitConverter.ToInt64(hash, 8);
            var hashCodeEnd = BitConverter.ToInt64(hash, 24);
            return hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        }
    }
}