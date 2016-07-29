using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace AllAuthClass
{
    public class AuthOTP
    {
        private void SetAttribute(XmlDocument XD, XmlNode XN, string AttName, string AttValue)
        {
            XmlAttribute Att = XN.Attributes.Append(XD.CreateAttribute(AttName));
            Att.InnerText = AttValue;
        }

        public string GenPIDXML_OTP(string OTP = "", string ts = "")
        {
            XmlDocument XDPid = new XmlDocument();
            XmlNode docNode = XDPid.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XDPid.AppendChild(docNode);
            XmlNode Root = XDPid.AppendChild(XDPid.CreateElement("Pid"));
            SetAttribute(XDPid, Root, "ts", ts);
            SetAttribute(XDPid, Root, "ver", "1.0");
            XmlNode Pv = Root.AppendChild(XDPid.CreateElement("Pv"));
            SetAttribute(XDPid, Pv, "otp", OTP);
            return XDPid.InnerXml;
        }


        public string GenOTPXML(string aadharNo, string txn)
        {
            //Values from web.config
            string pip = System.Configuration.ConfigurationManager.AppSettings["ProxyIP"].ToString();
            string sa = System.Configuration.ConfigurationManager.AppSettings["SA"].ToString();
            string lk = System.Configuration.ConfigurationManager.AppSettings["LicenseKey"].ToString();

            XmlDocument XDOtp = new XmlDocument();
            XmlNode docNode = XDOtp.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XDOtp.AppendChild(docNode);
            XmlNode Root = XDOtp.AppendChild(XDOtp.CreateElement("Otp"));
            SetAttribute(XDOtp, Root, "uid", aadharNo.Trim());
            SetAttribute(XDOtp, Root, "tid", "public");
            SetAttribute(XDOtp, Root, "ac", "public");
            SetAttribute(XDOtp, Root, "sa", sa);
            SetAttribute(XDOtp, Root, "ver", "1.5");
            SetAttribute(XDOtp, Root, "txn", txn);
            SetAttribute(XDOtp, Root, "lk", lk);
            SetAttribute(XDOtp, Root, "pip", pip);

            XmlNode Opts = Root.AppendChild(XDOtp.CreateElement("Opts"));
            XDOtp.DocumentElement.AppendChild(Opts);
            SetAttribute(XDOtp, Opts, "ch", "00");
            return XDOtp.InnerXml;
        }


        public static void SignXml(XmlDocument xmlDoc, X509Certificate2 uidCert)
        {

            RSACryptoServiceProvider rsaKey = (RSACryptoServiceProvider)uidCert.PrivateKey;


            // Check arguments. 
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (rsaKey == null)
                throw new ArgumentException("Key");

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Add the key to the SignedXml document.
            signedXml.SigningKey = rsaKey;


            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);


            // Add an RSAKeyValue KeyInfo (optional; helps recipient find key to validate).
            KeyInfo keyInfo = new KeyInfo();

            KeyInfoX509Data clause = new KeyInfoX509Data();
            clause.AddSubjectName(uidCert.Subject);
            clause.AddCertificate(uidCert);
            keyInfo.AddClause(clause);
            signedXml.KeyInfo = keyInfo;

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save 
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            System.Console.WriteLine(signedXml.GetXml().InnerXml);

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));


        }


        public string GenOTPResXML()
        {
            XmlDocument XDOtpRes = new XmlDocument();
            XmlNode Root = XDOtpRes.AppendChild(XDOtpRes.CreateElement("OtpRes"));
            SetAttribute(XDOtpRes, Root, "ret", "");
            SetAttribute(XDOtpRes, Root, "code", "");
            SetAttribute(XDOtpRes, Root, "txn", "");
            SetAttribute(XDOtpRes, Root, "err", "");
            SetAttribute(XDOtpRes, Root, "ts", "");
            return XDOtpRes.InnerXml;
        }
    }

}