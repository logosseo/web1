using System;
using System.Web.UI;
using System.Collections;
using System.Data.OracleClient;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using DIT.BizBase;

public partial class Basic_Login_BasicLogout : WebBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                RemoveSession();
                Response.Redirect("/");
            }
        }
        catch (Exception ex)
        {

        }
    }

}
