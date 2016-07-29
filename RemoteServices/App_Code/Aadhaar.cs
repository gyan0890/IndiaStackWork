using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AllAuthClass;
using System.Xml;
using System.IO;

/// <summary>
/// Summary description for Aadhaar
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

public class Aadhaar : System.Web.Services.WebService {
    Commonfun objCommonfun = new Commonfun();

    /// <summary>
    /// Function for Sending Aadhaar OTP
    /// </summary>
    /// <param name="AadharNo">Aadhaar Number of the user</param>
    /// <param name="access_token">Access Token for internal usage</param>
    /// <param name="transaction_id">Unique Transaction ID (will be send to UIDAI)</param>
    /// <returns>JSON Data with status, message and relevant data</returns>
    [WebMethod]
    public string GetAadhaarOTP(string AadhaarNo, string access_token, string transaction_id)
    {
        string output = string.Empty;
        string result = "";
        string json = "";
        string newFileName = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["OTPLogFile"].ToString());
        if (AadhaarNo == "")
        {
            json = "";
            json += "{";
            json += "\"status\":false";
            json += ",\"msg\":\"Enter Aadhaar No.!\"";
            json += "}";
            result = json;
        }
        else
        {
            if (objCommonfun.isValidAadhaar(AadhaarNo))
            {
                AuthOTP objAuthOTP = new AuthOTP();
                string otpXmlToSend = objAuthOTP.GenOTPXML(AadhaarNo.Trim(), transaction_id);
                string ResponseXML = "";

                string HttpWebRequesturl = objCommonfun.getHttpWebRequesturl();
                ResponseXML = objCommonfun.Postauthxml_OnAUA(otpXmlToSend, HttpWebRequesturl);

                output = objCommonfun.ParseRespXML(ResponseXML);
                if (output == "y")
                {
                    json = "";
                    json += "{";
                    json += "\"status\":true";
                    json += ",\"msg\":\"OTP Sent Successfully\"";
                    json += ",\"access_token\":\"" + access_token + "\"";
                    json += ",\"transaction_id\":\"" + transaction_id + "\"";
                    json += "}";
                    result = json;
                    string clientDetails = AadhaarNo + "," + access_token + "," + transaction_id + "," + "true" + "," + "OTP Sent Successfully" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
        
                    File.AppendAllText(newFileName, clientDetails);
                }
                else if (output == "n")
                {
                    json = "";
                    json += "{";
                    json += "\"status\":false";
                    json += ",\"code\":101";
                    json += ",\"msg\":\"OTP Sent Failed\"";
                    json += "}";
                    result = json;
                    string clientDetails = AadhaarNo + "," + access_token + "," + transaction_id + "," + "false" + "," + "OTP Sent Failed" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                    File.AppendAllText(newFileName, clientDetails);
                }
                else
                {
                    json = "";
                    json += "{";
                    json += "\"status\":false";
                    json += ",\"code\":102";
                    json += ",\"msg\":\"" + output + "\"";
                    json += "}";
                    result = json;
                    string clientDetails = AadhaarNo + "," + access_token + "," + transaction_id + "," + "false" + "," + output + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                    File.AppendAllText(newFileName, clientDetails);
                }
            }
            else
            {
                json = "";
                json += "{";
                json += "\"status\":false";
                json += ",\"msg\":\"Invalid Aadhar !\"";
                json += "}";
                result = json;

            }
        }
        return result;
    }

    /// <summary>
    /// Function for receiving KYC Data from UIDAI
    /// </summary>
    /// <param name="AadharNo">Aadhaar Number of the user</param>
    /// <param name="otp">OTP received from UIDAI</param>
    /// <param name="access_token">Access Token for internal usage</param>
    /// <param name="transaction_id">Unique Transaction ID (will be send to UIDAI)</param>
    /// <returns>JSON Data with status, message and relevant data</returns>
    [WebMethod]
    public string KYCData(string AadhaarNo, string otp, string access_token, string transaction_id)
    {
        string output = string.Empty;
        string result = "";
        string json = "";
        List<string> stringList = new List<string>();
        string newFileName = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["LogFile"].ToString());
        if (AadhaarNo == "")
        {
            json = "";
            json += "{";
            json += "\"status\":false";
            json += ",\"msg\":\"Enter AAdhar !\"";
            json += "}";
            result = json;
        }
        else
        {

            if (otp.Trim() == "")
            {
                json = "";
                json += "{";
                json += "\"status\":false";
                json += ",\"msg\":\"Enter OTP !\"";
                json += "}";
                result = json;

            }
            else
            {
                if (objCommonfun.isValidAadhaar(AadhaarNo))
                {
                   # region KYC
                    string ts = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

                    AuthKYC_OTP objAuthOTP = new AuthKYC_OTP();
                    
                    string pid = objAuthOTP.GenPIDXML_KYC_OTP(otp.Trim(), ts);
                    string publicKey = objCommonfun.readFicate();
                    string AuthXmlToSend = objAuthOTP.GenAuthXML_KYC_OTP(AadhaarNo, publicKey, pid);
                    string kycXmlToSend = objAuthOTP.GenKycXML(AuthXmlToSend, ts);

                    string ResponseXML = "";
                    string photo = "", residentName = "", dateOfBirth = "", gender = "", phone = "", emailId = "", careOf = "", landmark = "", locality = "", vtc = "", district = "", houseNumber = "", street = "", postOffice = "", subDistrict = "", state = "", pincode = "", residentNameLocal = "", careOfLocal = "", houseNumberLocal = "", streetLocal = "", landmarkLocal = "", localityLocal = "", vtcLocal = "", subDistrictLocal = "", districtLocal = "", stateLocal = "", pincodeLocal = "", postOfficeLocal = "",language="";
                    string HttpWebRequesturl = objCommonfun.getHttpWebRequesturl();
                    ResponseXML = objCommonfun.Postauthxml_OnAUA(kycXmlToSend, HttpWebRequesturl);
                    output = objCommonfun.ParseRespXML(ResponseXML);
                    if (output == "Y")
                    {
                        XmlDocument Doc = new XmlDocument();
                        Doc.LoadXml(ResponseXML);
                        XmlNode POI = Doc.SelectSingleNode("/KycRes/UidData/Poi");

                        if (!(POI.SelectSingleNode("@name") == null))
                            residentName = POI.SelectSingleNode("@name").Value.ToString();
                        else
                            residentName = "";

                        if (!(POI.SelectSingleNode("@dob") == null))
                            dateOfBirth = POI.SelectSingleNode("@dob").Value.ToString();
                        else
                            dateOfBirth = "";

                        if (!(POI.SelectSingleNode("@gender") == null))
                            gender = POI.SelectSingleNode("@gender").Value.ToString();
                        else
                            gender = "";


                        if (!(POI.SelectSingleNode("@email") == null))
                            emailId = POI.SelectSingleNode("@email").Value.ToString();
                        else
                            emailId = "";


                        if (!(POI.SelectSingleNode("@phone") == null))
                            phone = POI.SelectSingleNode("@phone").Value.ToString();
                        else
                            phone = "";


                        XmlNode POA = Doc.SelectSingleNode("/KycRes/UidData/Poa");


                        if (!(POA.SelectSingleNode("@co") == null))
                            careOf = POA.SelectSingleNode("@co").Value.ToString();
                        else
                            careOf = "";


                        if (!(POA.SelectSingleNode("@house") == null))
                            houseNumber = POA.SelectSingleNode("@house").Value.ToString();
                        else
                            houseNumber = "";

                        if (!(POA.SelectSingleNode("@street") == null))
                            street=POA.SelectSingleNode("@street").Value.ToString();
                        else
                            street="";

                        if (!(POA.SelectSingleNode("@loc") == null))
                            locality = POA.SelectSingleNode("@loc").Value.ToString();
                        else
                            locality = "";

                        if (!(POA.SelectSingleNode("@landmark") == null))
                            landmark = POA.SelectSingleNode("@landmark").Value.ToString();
                        else
                            landmark = "";

                        if (!(POA.SelectSingleNode("@vtc") == null))
                            vtc = POA.SelectSingleNode("@vtc").Value.ToString();
                        else
                            vtc = "";

                        if (!(POA.SelectSingleNode("@po") == null))
                            postOffice = POA.SelectSingleNode("@po").Value.ToString();
                        else
                            postOffice = "";

                        if (!(POA.SelectSingleNode("@subdist") == null))
                            subDistrict = POA.SelectSingleNode("@subdist").Value.ToString();
                        else
                            subDistrict = "";

                        if (!(POA.SelectSingleNode("@dist") == null))
                            district = POA.SelectSingleNode("@dist").Value.ToString();
                        else
                            district = "";

                        if (!(POA.SelectSingleNode("@state") == null))
                            state = POA.SelectSingleNode("@state").Value.ToString();
                        else
                            state = "";

                        if (!(POA.SelectSingleNode("@pc") == null))
                            pincode = POA.SelectSingleNode("@pc").Value.ToString();
                        else
                            pincode = "";
                      
                        XmlNode POL = Doc.SelectSingleNode("/KycRes/UidData/LData");

                        if (!(POL.SelectSingleNode("@name") == null))
                            residentNameLocal = POL.SelectSingleNode("@name").Value.ToString();
                        else
                            residentNameLocal = "";

                        if (!(POL.SelectSingleNode("@co") == null))
                            careOfLocal = POL.SelectSingleNode("@co").Value.ToString();
                        else
                            careOfLocal = "";


                        if (!(POL.SelectSingleNode("@house") == null))
                            houseNumberLocal = POL.SelectSingleNode("@house").Value.ToString();
                        else
                            houseNumberLocal = "";

                        if (!(POL.SelectSingleNode("@lang") == null))
                            language = POL.SelectSingleNode("@lang").Value.ToString();
                        else
                            language = "";

                        if (!(POL.SelectSingleNode("@street") == null))
                            streetLocal = POL.SelectSingleNode("@street").Value.ToString();
                        else
                            streetLocal = "";

                        if (!(POL.SelectSingleNode("@loc") == null))
                            localityLocal = POL.SelectSingleNode("@loc").Value.ToString();
                        else
                            localityLocal = "";

                        if (!(POL.SelectSingleNode("@landmark") == null))
                            landmarkLocal = POL.SelectSingleNode("@landmark").Value.ToString();
                        else
                            landmarkLocal = "";

                        if (!(POL.SelectSingleNode("@vtc") == null))
                            vtcLocal = POL.SelectSingleNode("@vtc").Value.ToString();
                        else
                            vtcLocal = "";

                        if (!(POL.SelectSingleNode("@po") == null))
                            postOfficeLocal = POL.SelectSingleNode("@po").Value.ToString();
                        else
                            postOfficeLocal = "";

                        if (!(POL.SelectSingleNode("@subdist") == null))
                            subDistrictLocal = POL.SelectSingleNode("@subdist").Value.ToString();
                        else
                            subDistrictLocal = "";

                        if (!(POL.SelectSingleNode("@dist") == null))
                            districtLocal = POL.SelectSingleNode("@dist").Value.ToString();
                        else
                            districtLocal = "";

                        if (!(POL.SelectSingleNode("@state") == null))
                            stateLocal = POL.SelectSingleNode("@state").Value.ToString();
                        else
                            stateLocal = "";

                        if (!(POL.SelectSingleNode("@pc") == null))
                            pincodeLocal = POL.SelectSingleNode("@pc").Value.ToString();
                        else
                            pincodeLocal = "";

                        XmlNode Pht = Doc.SelectSingleNode("/KycRes/UidData/Pht");

                        if (!(Pht == null))
                            photo = Pht.InnerText.ToString();
                        else
                            photo = "";

                        json = "";
                        json += "{";
                        json += "\"status\":true";
                        json += ",\"msg\":\"Your Authentication is Successful\"";
                        json += ",\"access_token\":\"" + access_token + "\"";
                        json += ",\"transaction_id\":\"" + transaction_id + "\"";
                        json += ",\"AadhaarNo\":\"" + AadhaarNo + "\"";
                        json += ",\"residentName\":\"" + residentName + "\"";
                        json += ",\"dateOfBirth\":\"" + dateOfBirth + "\"";
                        json += ",\"gender\":\"" + gender + "\"";
                        json += ",\"emailId\":\"" + emailId + "\"";
                        json += ",\"phone\":\"" + phone + "\"";
                        json += ",\"careOf\":\"" + careOf + "\"";
                        json += ",\"houseNumber\":\"" + houseNumber + "\"";
                        json += ",\"street\":\"" + street + "\"";
                        json += ",\"vtc\":\"" + vtc + "\"";
                        json += ",\"locality\":\"" + locality + "\"";
                        json += ",\"landmark\":\"" + landmark + "\"";
                        json += ",\"postOffice\":\"" + postOffice + "\"";
                        json += ",\"subDistrict\":\"" + subDistrict + "\"";
                        json += ",\"district\":\"" + district + "\"";
                        json += ",\"state\":\"" + state + "\"";
                        json += ",\"pincode\":\"" + pincode + "\"";
                        json += ",\"residentNameLocal\":\"" + residentNameLocal + "\"";
                        json += ",\"careOfLocal\":\"" + careOfLocal + "\"";
                        json += ",\"houseNumberLocal\":\"" + houseNumberLocal + "\"";
                        json += ",\"streetLocal\":\"" + streetLocal + "\"";
                        json += ",\"vtcLocal\":\"" + vtcLocal + "\"";
                        json += ",\"postOfficeLocal\":\"" + postOfficeLocal + "\"";
                        json += ",\"localityLocal\":\"" + localityLocal + "\"";
                        json += ",\"landmarkLocal\":\"" + landmarkLocal + "\"";
                        json += ",\"language\":\"" + language + "\"";
                        json += ",\"subDistrictLocal\":\"" + subDistrictLocal + "\"";
                        json += ",\"districtLocal\":\"" + districtLocal + "\"";
                        json += ",\"stateLocal\":\"" + stateLocal + "\"";
                        json += ",\"pincodeLocal\":\"" + pincodeLocal + "\"";
                        json += ",\"photo\":\"" + photo + "\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + otp + "," + access_token + "," + transaction_id + "," + "true" + "," + "Your Authentication is Successful" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                        File.AppendAllText(newFileName, clientDetails);
                    }
                    else if (output == "N")
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":false";
                        json += ",\"code\":101";
                        json += ",\"msg\":\"Your Authentication Failed\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + otp + "," + access_token + "," + transaction_id + "," + "false" + "," + "Your Authentication Failed" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                        File.AppendAllText(newFileName, clientDetails);
                    }
                    else
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":false";
                        json += ",\"code\":102";
                        json += ",\"msg\":\"" + output + "\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + otp + "," + access_token + "," + transaction_id + "," + "false" + "," + output + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                        File.AppendAllText(newFileName, clientDetails);
                    }

                  #endregion

                }
                else
                {
                    json = "";
                    json += "{";
                    json += "\"status\":false";
                    json += ",\"msg\":\"Invalid Aadhar !\"";
                    json += "}";
                    result = json;

                }
            }
        }

        return result;

        
    }

    /// <summary>
    /// Function for receiving KYC Authentication from UIDAI
    /// </summary>
    /// <param name="AadharNo">Aadhaar Number of the user</param>
    /// <param name="otp">OTP received from UIDAI</param>
    /// <param name="access_token">Access Token for internal usage</param>
    /// <param name="transaction_id">Unique Transaction ID (will be send to UIDAI)</param>
    /// <returns>JSON Data with status, message and relevant data</returns>
    [WebMethod]
    public string KYCAuthentication(string AadhaarNo, string otp, string access_token, string transaction_id)
    {
        string output = string.Empty;
        string result = "";
        string json = "";
        List<string> stringList = new List<string>();
        string newFileName = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["LogFile"].ToString());
        if (AadhaarNo == "")
        {
            json = "";
            json += "{";
            json += "\"status\":false";
            json += ",\"msg\":\"Enter AAdhar !\"";
            json += "}";
            result = json;
        }
        else
        {

            if (otp.Trim() == "")
            {
                json = "";
                json += "{";
                json += "\"status\":false";
                json += ",\"msg\":\"Enter OTP !\"";
                json += "}";
                result = json;

            }
            else
            {
                if (objCommonfun.isValidAadhaar(AadhaarNo))
                {
                    # region KYC
                    string ts = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

                    AuthKYC_OTP objAuthOTP = new AuthKYC_OTP();

                    string pid = objAuthOTP.GenPIDXML_KYC_OTP(otp.Trim(), ts);
                    string publicKey = objCommonfun.readFicate();
                    string AuthXmlToSend = objAuthOTP.GenAuthXML_KYC_OTP(AadhaarNo, publicKey, pid, transaction_id);
                    string kycXmlToSend = objAuthOTP.GenKycXML(AuthXmlToSend, ts);

                    string ResponseXML = "";
                    string HttpWebRequesturl = objCommonfun.getHttpWebRequesturl();
                    ResponseXML = objCommonfun.Postauthxml_OnAUA(kycXmlToSend, HttpWebRequesturl);
                    output = objCommonfun.ParseRespXML(ResponseXML);
                    if (output == "Y")
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":true";
                        json += ",\"msg\":\"Your Authentication is Successful\"";
                        json += ",\"access_token\":\"" + access_token + "\"";
                        json += ",\"transaction_id\":\"" + transaction_id + "\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + otp + "," + access_token + "," + transaction_id + "," + "true" + "," + "Your Authentication is Successful" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                        File.AppendAllText(newFileName, clientDetails);
                    }
                    else if (output == "N")
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":false";
                        json += ",\"code\":101";
                        json += ",\"msg\":\"Your Authentication Failed\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + otp + "," + access_token + "," + transaction_id + "," + "false" + "," + "Your Authentication Failed" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                        File.AppendAllText(newFileName, clientDetails);
                    }
                    else
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":false";
                        json += ",\"code\":102";
                        json += ",\"msg\":\"" + output + "\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + otp + "," + access_token + "," + transaction_id + "," + "false" + "," + output + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")  + Environment.NewLine;
                        File.AppendAllText(newFileName, clientDetails);
                    }

                    #endregion

                }
                else
                {
                    json = "";
                    json += "{";
                    json += "\"status\":false";
                    json += ",\"msg\":\"Invalid Aadhar !\"";
                    json += "}";
                    result = json;

                }
            }
        }

        return result;


    }

    /// <summary>
    /// Demographic Aadhaar Authentication
    /// </summary>
    /// <param name="AadhaarNo">Aadhaar Number of Applicant</param>
    /// <param name="PersonName">Full Name as in Aadhaar of Applicant (100%)</param>
    /// <param name="PersonGender">Gender of the Applicant</param>
    /// <param name="PersonDOB">Date of Birth of Applicant (YYYY-MM-DD)</param>
    /// <param name="PinCode">PIN Code of Applicant (Optional)</param>
    /// <param name="access_token">Access Token from OwnCloud</param>
    /// <param name="transaction_id">Unique Transaction ID from Application Server</param>
    /// <returns>JSON Data (True if Authentication is Successfull else False)</returns>
    [WebMethod]
    public string demographicAuthentication(string AadhaarNo, string PersonName, string PersonGender = "", string PersonDOB = "", string PinCode = "", string access_token = "Access Token", string transaction_id = "DGLKR")
    {
        string output = string.Empty;
        string result = "";
        string json = "";
        string newFileName = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DemographicLogFile"].ToString());
        if (AadhaarNo == "")
        {
            json = "";
            json += "{";
            json += "\"status\":false";
            json += ",\"msg\":\"Enter Aadhaar !\"";
            json += "}";
            result = json;
        }
        else if (PersonName == "")
        {
            json = "";
            json += "{";
            json += "\"status\":false";
            json += ",\"msg\":\"Enter Person Name !\"";
            json += "}";
            result = json;
        }
        else if (PersonGender == "")
        {
            json = "";
            json += "{";
            json += "\"status\":false";
            json += ",\"msg\":\"Enter Gender !\"";
            json += "}";
            result = json;
        }
        else if (PersonDOB == "")
        {
            json = "";
            json += "{";
            json += "\"status\":false";
            json += ",\"msg\":\"Enter Date of Birth !\"";
            json += "}";
            result = json;
        }
        else
            {
                if (objCommonfun.isValidAadhaar(AadhaarNo))
                {
                    # region DemographicAuth
                    string ts = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

                    AuthDemographic objAuthDemographic = new AuthDemographic();

                    string pid = objAuthDemographic.GenPIDXML_Demographic(PersonName, PersonGender, PersonDOB, "", "E", "100", PinCode);
                    string publicKey = objCommonfun.readFicate();
                    string AuthXmlToSend = objAuthDemographic.GenAuthXML_Demographic(AadhaarNo, PersonName, PinCode, publicKey, transaction_id, pid);

                    string ResponseXML = "";
                    string HttpWebRequesturl = objCommonfun.getHttpWebRequesturl();
                    ResponseXML = objCommonfun.Postauthxml_OnAUA(AuthXmlToSend, HttpWebRequesturl);
                    output = objCommonfun.demographicParseRespXML(ResponseXML);
                    if (output == "y")
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":true";
                        json += ",\"msg\":\"Your Authentication is Successful\"";
                        json += ",\"access_token\":\"" + access_token + "\"";
                        json += ",\"transaction_id\":\"" + transaction_id + "\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + access_token + "," + transaction_id + "," + "true" + "," + "Your Authentication is Successful" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine;
                       
                        File.AppendAllText(newFileName, clientDetails);
                    }
                    else if (output == "n")
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":false";
                        json += ",\"code\":101";
                        json += ",\"msg\":\"Your Authentication Failed\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + access_token + "," + transaction_id + "," + "false" + "," + "Your Authentication Failed" + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine;
                     
                        File.AppendAllText(newFileName, clientDetails);
                    }
                    else
                    {
                        json = "";
                        json += "{";
                        json += "\"status\":false";
                        json += ",\"code\":102";
                        json += ",\"msg\":\"" + output + "\"";
                        json += "}";
                        result = json;
                        string clientDetails = AadhaarNo + "," + access_token + "," + transaction_id + "," + "false" + "," + output + "," + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine;
                      
                        File.AppendAllText(newFileName, clientDetails);
                    }

                    #endregion

                }
                else
                {
                    json = "";
                    json += "{";
                    json += "\"status\":false";
                    json += ",\"msg\":\"Invalid Aadhar !\"";
                    json += "}";
                    result = json;

                }
            }
			return result;
        
	
       
    }
    


}
