using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AadhaarKYC : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Request.Form.Keys.Count > 0)
            {
                string aadhaar_no = Request.Form[0].ToString();
                string otp = Request.Form[1].ToString();
                string access_token = Request.Form[2].ToString();
                //Transaction ID from Application Server
                string transaction_id = Request.Form[3].ToString();
                //Response.Write("AadharNo:" + aadhaar_no);
                //Response.Write("OTP:" + otp);
                Response.Write(KYCdata(aadhaar_no, otp, access_token, transaction_id));
            }
            else
            {
                Response.Write("GET Method not allowed. Please POST required parameters as per API specification Document.");
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
    private string KYCdata(string strAdhaar, string strOTP, string straccess_token, string strtransaction_id)
    {
        string strKYCdata = null;
        Aadhaar aadhar = new Aadhaar();
        strKYCdata = aadhar.KYCData(strAdhaar, strOTP, straccess_token, strtransaction_id);
        return strKYCdata;
    }
}