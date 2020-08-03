﻿using Psd2CertificateGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Psd2CertificateGenerator
{
    public class PSD2Certificate
    {
        public string PublicKey { get; private set;  }
        public string PrivateKey { get; private set; }
        public string Certificate { get; private set; }

        private const string OID_emailAddress = "1.2.840.113549.1.9.1";
        private const string OID_organizationIdentifier = "2.5.4.97";
        private const string OID_serverAuth = "1.3.6.1.5.5.7.3.1";
        private const string OID_clientAuth = "1.3.6.1.5.5.7.3.2";

        private static X500DistinguishedName CreateDistinguishedName(Dictionary<string, string> subjectFields)
        {
            return new X500DistinguishedName(string.Join(", ", subjectFields.Select(kvp => string.IsNullOrEmpty(kvp.Value) ? null : $"{kvp.Key}={kvp.Value}").Where(x => x != null)));
        }

        private static byte[] StringToByteArray(string hex)
        {
            var length = hex.Length;
            if (length % 2 != 0)
                throw new ArgumentException("Hex string length must be a multiple of 2");
            var bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static PSD2Certificate createRSAKeysAndCertificate(PSD2CertificateParameters parameters)
        {
            var context = new ValidationContext(parameters, null, null);
            var validationResults = new List<ValidationResult>();

            bool valid = Validator.TryValidateObject(parameters, context, validationResults, true);
            if (!valid)
            {
                throw new ValidationException("Invalid Request", new AggregateException(validationResults.Select(validationResult => new ValidationException(validationResult.ErrorMessage))));
            }

            var subjectDN = CreateDistinguishedName(new Dictionary<string, string>
            {
                { "O", parameters.Subject.Organization },
                { "CN", parameters.Subject.CommonName },
                { "C", parameters.Subject.Country },
                { OID_organizationIdentifier, parameters.Subject.OrganizationIdentifier },
            });
            // self-signed cert
            var issuerDN = subjectDN;
            var issuerDnsName = parameters.IssuerDnsName;

            using (RSA rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(subjectDN, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, false));
                if (parameters.CertificateType == PSD2CertificateType.QWAC) // only for qwac
                    request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid(OID_serverAuth), new Oid(OID_clientAuth) }, false));                
                request.CertificateExtensions.Add(new X509QcStatmentExtension(parameters.Roles, parameters.CertificateType, parameters.RetentionPeriod, parameters.NcaName, parameters.NcaId, false));

                var sanBuilder = new SubjectAlternativeNameBuilder();
                sanBuilder.AddDnsName(issuerDnsName);
                request.CertificateExtensions.Add(sanBuilder.Build());

                var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10));

                return new PSD2Certificate
                {
                    PublicKey = rsa.ExportSubjectPublicKeyInfo().ToPem(PemFormatLabel.PublicKey),
                    PrivateKey = rsa.ExportPkcs8PrivateKey().ToPem(PemFormatLabel.RsaPrivateKey),
                    Certificate = certificate.Export(X509ContentType.Cert).ToPem(PemFormatLabel.Certificate)
                };
            }
        }
    }
}
