using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AllAuthClass
{
    public class AuthKYC_OTP
    {
        private void SetAttribute(XmlDocument XD, XmlNode XN, string AttName, string AttValue)
        {
            XmlAttribute Att = XN.Attributes.Append(XD.CreateAttribute(AttName));
            Att.InnerText = AttValue;
        }
        public string GenPIDXML_KYC_OTP(string OTP = "", string ts = "")
        {
            XmlDocument XDPid = new XmlDocument();
            XmlNode docNode = XDPid.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XDPid.AppendChild(docNode);
            XmlNode Root = XDPid.AppendChild(XDPid.CreateElement("Pid"));
            SetAttribute(XDPid, Root, "ts", ts);
            XmlNode Pv = Root.AppendChild(XDPid.CreateElement("Pv"));
            SetAttribute(XDPid, Pv, "otp", OTP);
            return XDPid.InnerXml;
        }

        public string GenAuthXML_KYC_OTP(string aadharNo = "", string publicKey = "", string pid = "", string txn = "")
        {
            if (!string.IsNullOrEmpty(aadharNo))
            {
                txn = aadharNo + System.DateTime.Now.ToString("yyyyMMddHHmmss:fff");
                txn = "UKC:" + txn;
            }
            else
            {
                txn = "UKC:" + txn;
            }

            //Values from web.config
            string pip = System.Configuration.ConfigurationManager.AppSettings["ProxyIP"].ToString();
            string sa = System.Configuration.ConfigurationManager.AppSettings["SA"].ToString();
            string lk = System.Configuration.ConfigurationManager.AppSettings["LicenseKey"].ToString();

            Enc xx = new Enc(Convert.FromBase64String(publicKey));
            //Generate Session Key
            byte[] sessionKey = xx.generateSessionKey();
            //Now Encrypt Session Key using Public Certificate of UIDAI
            byte[] encryptedSessionKey = xx.encryptUsingPublicKey(sessionKey);

            byte[] pidXmlBytes = Encoding.UTF8.GetBytes(pid);
            //Encrypt PID block using Session Key
            byte[] encXMLPIDData = xx.encryptUsingSessionKey(sessionKey, pidXmlBytes);
            //Calculate HMAC of PID Block
            byte[] hmac = xx.generateSha256Hash(pidXmlBytes);
            //Encrypt HMAC using Session Key
            byte[] encryptedHmacBytes = xx.encryptUsingSessionKey(sessionKey, hmac);
            //Get Certificate Identifier from Public Key
            string certificateIdentifier = xx.getCertificateIdentifier();


            XmlDocument XDAuth = new XmlDocument();
            XmlNode docNode = XDAuth.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XDAuth.AppendChild(docNode);
            XmlNode Root = XDAuth.AppendChild(XDAuth.CreateElement("Auth", "http://www.uidai.gov.in/authentication/uid-auth-request/1.0"));
            SetAttribute(XDAuth, Root, "uid", aadharNo.Trim());
            SetAttribute(XDAuth, Root, "tid", "public");
            SetAttribute(XDAuth, Root, "ac", "public");
            SetAttribute(XDAuth, Root, "sa", sa);
            SetAttribute(XDAuth, Root, "ver", "1.6");
            SetAttribute(XDAuth, Root, "txn", txn);
            SetAttribute(XDAuth, Root, "lk", lk);
            SetAttribute(XDAuth, Root, "xmlns", "http://www.uidai.gov.in/authentication/uid-auth-request/1.0");


            XmlNode Meta = Root.AppendChild(XDAuth.CreateElement("Meta", "http://www.uidai.gov.in/authentication/uid-auth-request/1.0"));
            SetAttribute(XDAuth, Meta, "udc", "TEST");
            SetAttribute(XDAuth, Meta, "fdc", "NC");
            SetAttribute(XDAuth, Meta, "idc", "NA");
            SetAttribute(XDAuth, Meta, "pip", pip);
            SetAttribute(XDAuth, Meta, "lot", "P");
            SetAttribute(XDAuth, Meta, "lov", "110092");

            XmlNode Skey = Root.AppendChild(XDAuth.CreateElement("Skey", "http://www.uidai.gov.in/authentication/uid-auth-request/1.0"));
            SetAttribute(XDAuth, Skey, "ci", certificateIdentifier);
            Skey.InnerXml = Convert.ToBase64String(encryptedSessionKey);

            XmlNode Uses = Root.AppendChild(XDAuth.CreateElement("Uses", "http://www.uidai.gov.in/authentication/uid-auth-request/1.0"));
            XDAuth.DocumentElement.AppendChild(Uses);
            SetAttribute(XDAuth, Uses, "otp", "y");
            SetAttribute(XDAuth, Uses, "pin", "n");
            SetAttribute(XDAuth, Uses, "pfa", "n");
            SetAttribute(XDAuth, Uses, "pa", "n");
            SetAttribute(XDAuth, Uses, "pi", "n");
            SetAttribute(XDAuth, Uses, "bio", "n");


            XmlNode Data = Root.AppendChild(XDAuth.CreateElement("Data", "http://www.uidai.gov.in/authentication/uid-auth-request/1.0"));
            SetAttribute(XDAuth, Data, "type", "X");
            Data.InnerXml = Convert.ToBase64String(encXMLPIDData);

            XmlNode Hmac2 = Root.AppendChild(XDAuth.CreateElement("Hmac", "http://www.uidai.gov.in/authentication/uid-auth-request/1.0"));
            Hmac2.InnerXml = Convert.ToBase64String(encryptedHmacBytes);

            return XDAuth.OuterXml;
        }

        public string GenKycXML(string allRad = "", string ts = "")
        {

            XmlDocument XDKyc = new XmlDocument();
            XmlNode docNode = XDKyc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XDKyc.AppendChild(docNode);


            XmlNode Root = XDKyc.AppendChild(XDKyc.CreateElement("Kyc"));
            SetAttribute(XDKyc, Root, "xmlns", "http://www.uidai.gov.in/kyc/uid-kyc-request/1.0");
            SetAttribute(XDKyc, Root, "ver", "1.0");
            SetAttribute(XDKyc, Root, "ts", ts);
            SetAttribute(XDKyc, Root, "ra", "O");
            SetAttribute(XDKyc, Root, "rc", "Y");
            SetAttribute(XDKyc, Root, "mec", "Y");
            SetAttribute(XDKyc, Root, "lr", "Y");
            SetAttribute(XDKyc, Root, "de", "N");

            XmlNode Rad = Root.AppendChild(XDKyc.CreateElement("Rad"));
            byte[] allRadXmlBytes = Encoding.UTF8.GetBytes(allRad);
            Rad.InnerXml = Convert.ToBase64String(allRadXmlBytes);

            return XDKyc.OuterXml;


        }
    }
}
