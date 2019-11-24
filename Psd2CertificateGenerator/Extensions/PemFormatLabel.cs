namespace Psd2CertificateGenerator.Extensions
{
    public enum PemFormatLabel
    {
        PublicKey,
        RsaPrivateKey,
        Certificate
    }

    internal static class PemFormatLabelExtensions
    {
        public static string ToLabel(this PemFormatLabel formatLabel)
        {
            switch (formatLabel)
            {
                case PemFormatLabel.Certificate: return "CERTIFICATE";
                case PemFormatLabel.PublicKey: return "PUBLIC KEY";
                case PemFormatLabel.RsaPrivateKey: return "RSA PRIVATE KEY";
            }
            return null;
        }
    }
}
