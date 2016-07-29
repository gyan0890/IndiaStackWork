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
                string access_token = Request.Form[1].ToString();
                //Transaction ID from Application Server
                string transaction_id = Request.Form[2].ToString();

                Response.Write(GetAadhaarOTP(aadhaar_no, access_token, transaction_id));
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
    private string GetAadhaarOTP(string strAdhaar, string access_code, string transaction_id)
    {
        string strOTPdata = null;
        Aadhaar aadhar = new Aadhaar();
        strOTPdata = aadhar.GetAadhaarOTP(strAdhaar, access_code, transaction_id);
        return strOTPdata;
    }
}