using System;

namespace Domivium.Client.Core.Utility
{
    public static class TextWriteUtils
    {
        public static int WriteLevelText(int level, char[] buf, int offset = 0)
        {
            return WriteWithPrefix(level, "Lv. ".AsSpan(), buf, offset);
        }

        public static int WriteWithPrefix(int value, ReadOnlySpan<char> prefix, char[] buf, int offset = 0)
        {
            int pLen = prefix.Length;
            if (offset + pLen >= buf.Length) return 0;

            prefix.CopyTo(buf.AsSpan(offset, pLen));

            int n = WriteIntToBuffer(value, buf, offset + pLen);
            if (n == 0) return 0;

            return pLen + n;
        }

        private static int WriteIntToBuffer(int value, char[] buf, int offset)
        {
            int end = buf.Length;
            int i = end;

            bool neg = value < 0;
            uint v = neg ? (uint)(-value) : (uint)value;

            do
            {
                if (i == offset) return 0; // 공간 부족

                uint d = v % 10u;
                v /= 10u;
                buf[--i] = (char)('0' + d);
            } while (v > 0);

            if (neg)
            {
                if (i == offset) return 0;

                buf[--i] = '-';
            }

            int written = end - i;
            // [i..end) → [offset..offset+written) 로 제자리 복사
            for (int k = 0; k < written; k++)
                buf[offset + k] = buf[i + k];

            return written;
        }

        public static int WriteIntToBuffer(int value, char[] buf)
        {
            var i = buf.Length;
            var neg = value < 0;
            var v = neg ? (uint)-value : (uint)value;

            do
            {
                var d = v % 10u;
                v /= 10u;
                buf[--i] = (char)('0' + d);
            } while (v > 0);

            if (neg)
            {
                buf[--i] = '-';
            }

            var written = buf.Length - i;
            if (i != 0)
            {
                Array.Copy(buf, i, buf, 0, written);
            }

            return written;
        }

        public static int WriteIntGrouped(int value, char[] buf, char groupSep = ',') => WriteIntGrouped(value, buf, 0, groupSep);

        public static int WriteIntGrouped(int value, char[] buf, int offset, char groupSep = ',')
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

        public static int WriteString(ReadOnlySpan<char> value, char[] buf)
        {
            var len = value.Length;
            value.CopyTo(buf.AsSpan(0, len));
            return len;
        }
    }
}