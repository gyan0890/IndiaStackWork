using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DemographicAuth : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Request.Form.Keys.Count > 0)
            {
                string aadhaar_no = Request.Form[0].ToString();
                string person_name = Request.Form[1].ToString();
                string person_gender = Request.Form[2].ToString();
                string person_dob = Request.Form[3].ToString();
                string person_pin = Request.Form[4].ToString();
                string access_token = Request.Form[5].ToString();
                //Transaction ID from Application Server
                string transaction_id = Request.Form[6].ToString();
                Response.Write(demographicData(aadhaar_no, person_name, person_gender, person_dob, person_pin, access_token, transaction_id));
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

    private string demographicData(string aadhaar_no, string person_name, string person_gender, string person_dob, string person_pin, string access_token, string transaction_id)
    {
        string strDemographicData = null;
        Aadhaar aadhar = new Aadhaar();
        strDemographicData = aadhar.demographicAuthentication(aadhaar_no, person_name, person_gender, person_dob, person_pin, access_token, transaction_id);
        return strDemographicData;
    }
}