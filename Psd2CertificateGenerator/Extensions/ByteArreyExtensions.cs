using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Psd2CertificateGenerator.Extensions
{
    public static class ByteArreyExtensions
    {
        private static Regex Base64PEMLineBreaks = new Regex(".{1,64}");

        public static string ToPem(this byte[] buffer, PemFormatLabel format)
        {
            var sb = new StringBuilder();
            var label = format.ToLabel();
            sb.AppendLine($"-----BEGIN {label}-----");
            sb.AppendLine(string.Join("\n", Base64PEMLineBreaks.Matches(Convert.ToBase64String(buffer))));
            sb.AppendLine($"-----END {label}-----");
            return sb.ToString();
        }
    }
}
