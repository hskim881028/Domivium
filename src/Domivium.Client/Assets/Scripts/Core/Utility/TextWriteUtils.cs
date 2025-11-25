using System;

namespace Domivium.Client.Core.Utility
{
    public static class TextWriteUtils
    {
        public static int WriteIntToBuffer(int value, char[] buf, int offset = 0)
        {
            var end = buf.Length;
            var i = end;
            var neg = value < 0;
            var v = neg ? (uint)-value : (uint)value;

            do
            {
                if (i == offset) return 0;

                var d = v % 10u;
                v /= 10u;
                buf[--i] = (char)('0' + d);
            } while (v > 0);

            if (neg)
            {
                if (i == offset) return 0;

                buf[--i] = '-';
            }

            var written = end - i;
            for (var k = 0; k < written; k++)
            {
                buf[offset + k] = buf[i + k];
            }

            return written;
        }

        public static int WriteIntGrouped(int value, char[] buf, char groupSep) => WriteIntGrouped(value, buf, 0, groupSep);

        public static int WriteIntGrouped(int value, char[] buf, int offset, char groupSep)
        {
            var end = offset + 16;
            if (end > buf.Length)
            {
                end = buf.Length;
            }

            var i = end;
            var neg = value < 0;
            var v = neg ? (uint)-value : (uint)value;
            var groupCount = 0;
            do
            {
                if (groupCount == 3)
                {
                    if (i == offset) return 0;

                    buf[--i] = groupSep;
                    groupCount = 0;
                }
                var d = v % 10u;
                v /= 10u;
                if (i == offset) return 0;

                buf[--i] = (char)('0' + d);
                groupCount++;
            } while (v > 0);

            if (neg)
            {
                if (i == offset) return 0;

                buf[--i] = '-';
            }

            var written = end - i;
            for (var k = 0; k < written; k++)
            {
                buf[offset + k] = buf[i + k];
            }

            return written;
        }

        public static int WriteFixed2(float value, char[] buf, int offset = 0)
        {
            var intPart = ScaleToFixed2(value, out var fracPart);
            var len = WriteIntToBuffer(intPart, buf, offset);
            return len == 0 ? 0 : WriteFrac2(buf, offset, len, fracPart);
        }

        public static int WriteIntGroupedFixed2(float value, char[] buf, int offset, char groupSep)
        {
            var intPart = ScaleToFixed2(value, out var fracPart);
            var len = WriteIntGrouped(intPart, buf, offset, groupSep);
            return len == 0 ? 0 : WriteFrac2(buf, offset, len, fracPart);
        }

        public static int WriteIntGroupedFixed2(float value, char[] buf, char groupSep)
            => WriteIntGroupedFixed2(value, buf, 0, groupSep);

        public static int WriteString(ReadOnlySpan<char> value, char[] buf, int offset = 0)
        {
            var len = value.Length;
            if (offset + len > buf.Length) return 0;

            value.CopyTo(buf.AsSpan(offset));
            return len;
        }

        public static int WriteWithPrefix(int value, ReadOnlySpan<char> prefix, char[] buf, int offset = 0)
            => WriteWithPrefixCore(prefix, buf, offset, (b, o) => WriteIntToBuffer(value, b, o));

        public static int WriteWithPrefix(float value, ReadOnlySpan<char> prefix, char[] buf, int offset = 0)
            => WriteWithPrefixCore(prefix, buf, offset, (b, o) => WriteFixed2(value, b, o));

        private static int ScaleToFixed2(float value, out int fracPart)
        {
            var scaled = (int)MathF.Round(value * 100f);
            var intPart = scaled / 100;
            fracPart = Math.Abs(scaled % 100);
            return intPart;
        }

        private static int WriteFrac2(char[] buf, int offset, int len, int fracPart)
        {
            var pos = offset + len;
            if (pos >= buf.Length) return 0;

            buf[pos++] = '.';
            if (pos + 2 > buf.Length) return 0;

            buf[pos++] = (char)('0' + fracPart / 10);
            buf[pos++] = (char)('0' + fracPart % 10);
            return pos - offset;
        }

        private static int WriteWithPrefixCore(
            ReadOnlySpan<char> prefix,
            char[] buf,
            int offset,
            Func<char[], int, int> writeValue)
        {
            var pLen = prefix.Length;
            if (offset + pLen > buf.Length) return 0;

            prefix.CopyTo(buf.AsSpan(offset, pLen));
            var n = writeValue(buf, offset + pLen);
            if (n == 0) return 0;

            return pLen + n;
        }
    }
}