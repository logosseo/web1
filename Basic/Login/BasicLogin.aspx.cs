using System;
using System.Web.UI;
using System.Collections;
using System.Data.OracleClient;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using DIT.BizBase;

public partial class Basic_Login_BasicLogin : WebBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InitDefaultData();
            }
        }
        catch (Exception ex)
        {

        }
    }


    private void InitDefaultData()
    {
        //쿠키값이 있다면 id 세팅
        uid.Value = GetCookie("LOGIN_ID_SAVE");

        //이미 로그인이 되어 있다면.
        if (ComUtils.GetNotNullToString(Session["LOGIN_ID"]) != "")
        {
            Response.Redirect("/Basic/Main/BasicMain.aspx");
        }
    }


    protected void btnLogin_OnClick(object sender, EventArgs e)
    {
        String _uid = ComUtils.GetDefaultString(Request["uid"], "");
        String _upw = ComUtils.GetDefaultString(Request["upw"], "");
        String _uidSave = ComUtils.GetDefaultString(Request["uidSave"], "");

        string result = CheckIdPw(_uid, _upw);
        if(result == "Y")
        {
            DataTable userTable = null;
            Regex r = new Regex("[A-Z]");
            if(r.IsMatch(_uid))
            {
                //영어가 포함된 id는 직원
                userTable = GetEmpInfo(_uid);
            } else
            {
                //숫자로된 id는 공급업체
                userTable = GetSupplyInfo(_uid);
            }

            if (userTable != null && userTable.Rows.Count > 0)
            {
                SetSession(userTable);
                SetCookieIdSave(userTable, _uidSave);
            }

            Response.Redirect("/Basic/Main/BasicMain.aspx");
        }

        //화면 load 되기전에 실행(aspx 의 form 바로 아래 스크립트가 추가된다.)
        ClientScript.RegisterClientScriptBlock(GetType(), "message", "<script>alert('ID/PW 를 확인해 주세요.');</script>");
    }


    /// <summary>
    /// 직원 정보 조회
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    private DataTable GetEmpInfo(string userid)
    {
        DataTable dataTable = null;
        string szQuery = "";
        szQuery += "        SELECT USER_TBL.USER_NAME                                                AS LOGIN_ID ";
        szQuery += "               , USER_TBL.USER_ID                                                AS USER_ID ";
        szQuery += "               , (CASE WHEN (SYSDATE BETWEEN USER_TBL.START_DATE AND NVL(USER_TBL.END_DATE, SYSDATE + 1)) THEN 'Y' ELSE 'N' END) AS ENABLE_DATE ";
        szQuery += "               , PEOLPE_TBL.FULL_NAME                                            AS USER_NAME_KR ";
        szQuery += "               , USER_TBL.EMAIL_ADDRESS                                          AS EMAIL ";
        szQuery += "               , PEOLPE_TBL.PERSON_ID                                            AS PERSON_ID ";
        szQuery += "               , ASSIGN_TBL.ASS_ATTRIBUTE1                                       AS DEPT_CODE ";
        szQuery += "               , (EFIN_COMMON_PKG.GET_SEG_DESC(ASSIGN_TBL.SET_OF_BOOKS_ID, 3, ASSIGN_TBL.ASS_ATTRIBUTE1)) AS DEPT_NAME ";
        szQuery += "               , ASSIGN_TBL.LOCATION_ID                                          AS LOCATION_ID ";
        szQuery += "               , LOCATION_TBL.LOCATION_CODE                                      AS LOCATION_NAME ";
        szQuery += "               , USER_TBL.EMPLOYEE_ID                                            AS EMPLOYEE_ID ";
        szQuery += "               , USER_TBL.PERSON_PARTY_ID                                        AS PERSON_PARTY_ID ";
        szQuery += "               , PEOLPE_TBL.EMPLOYEE_NUMBER                                      AS EMPLOYEE_NUMBER ";
        szQuery += "               , ASSIGN_TBL.ORGANIZATION_ID ";
        szQuery += "        FROM   FND_USER                   USER_TBL ";
        szQuery += "               , PER_ALL_PEOPLE_F         PEOLPE_TBL ";
        szQuery += "               , PER_ALL_ASSIGNMENTS_F    ASSIGN_TBL ";
        szQuery += "               , HR_LOCATIONS_ALL         LOCATION_TBL ";
        szQuery += "        WHERE  USER_TBL.EMPLOYEE_ID     = PEOLPE_TBL.PERSON_ID ";
        szQuery += "           AND USER_TBL.PERSON_PARTY_ID = PEOLPE_TBL.PARTY_ID ";
        szQuery += "           AND PEOLPE_TBL.PERSON_ID     = ASSIGN_TBL.PERSON_ID ";
        szQuery += "           AND TRUNC(SYSDATE) BETWEEN PEOLPE_TBL.EFFECTIVE_START_DATE AND PEOLPE_TBL.EFFECTIVE_END_DATE ";
        szQuery += "           AND TRUNC(SYSDATE) BETWEEN ASSIGN_TBL.EFFECTIVE_START_DATE AND ASSIGN_TBL.EFFECTIVE_END_DATE ";
        szQuery += "           AND ASSIGN_TBL.LOCATION_ID   = LOCATION_TBL.LOCATION_ID(+) ";
        szQuery += "           AND USER_TBL.USER_NAME       = '" + userid + "' ";
        szQuery += "           AND ROWNUM = 1 ";

        try
        {
            using (OracleConnection oracleConnection = new OracleConnection(ConfigurationSettings.AppSettings["GERP_ConnectionString"]))
            {
                oracleConnection.Open();
                using (OracleCommand oracleCommand = new OracleCommand(szQuery, oracleConnection))
                {
                    using (OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand))
                    {
                        DataSet dataSet = new DataSet();
                        oracleDataAdapter.Fill(dataSet);
                        dataTable = dataSet.Tables[0];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //throw ex;
        }

        return dataTable;
    }


    /// <summary>
    /// 공급업체 정보 조회
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    private DataTable GetSupplyInfo(string userid)
    {
        DataTable dataTable = null;
        string szQuery = "";
        szQuery += "        SELECT ";
        szQuery += "            USER_TBL.USER_NAME AS LOGIN_ID ";
        szQuery += "            , USER_TBL.USER_ID AS USER_ID ";
        szQuery += "            , (CASE WHEN(SYSDATE BETWEEN NVL(USER_TBL.START_DATE, SYSDATE + 1) AND NVL(USER_TBL.END_DATE, SYSDATE + 1)) THEN 'Y' ELSE 'N' END) AS ENABLE_DATE ";
        szQuery += "           , SP_USER_TBL.NAME AS USER_NAME_KR ";
        szQuery += "            , SP_USER_TBL.EMAIL AS EMAIL ";
        szQuery += "            , SP_COMPANY_TBL.COMPANY_REG_NO AS COMPANY_REG_NO ";
        szQuery += "            , SP_COMPANY_TBL.COMPANY_NAME AS COMPANY_NAME ";
        szQuery += "            , SP_COMPANY_TBL.COUNTRY AS COUNTRY_CODE ";
        szQuery += "            , NVL((SELECT NAME FROM PA_COUNTRY_V WHERE COUNTRY_CODE = SP_COMPANY_TBL.COUNTRY), '') AS COUNTRY_NAME ";
        szQuery += "            , SP_COMPANY_TBL.ATTACH_FILE_MAX_COUNT ";
        szQuery += "            , SP_COMPANY_TBL.ATTACH_FILE_MAX_SIZE ";
        szQuery += "            , RESPONSE_TBL.AUTH_CODE as RESPONSIBILITY_KEY ";
        szQuery += "            , RESPONSE_TBL.AUTH_TEXT as RESPONSIBILITY_NAME ";
        szQuery += "        FROM FND_USER                          USER_TBL, ";
        szQuery += "            EPO.EPO_PORTAL_USER_MST SP_USER_TBL, ";
        szQuery += "            EPO.EPO_PORTAL_COMPANY_MST SP_COMPANY_TBL ";
        szQuery += "            , EPO_PORTALN_AUTH_USER USERRESP_TBL ";
        szQuery += "            , EPO_PORTALN_AUTH_MASTER RESPONSE_TBL ";
        szQuery += "        WHERE USER_TBL.USER_NAME = '" + userid + "' ";
        szQuery += "            AND USER_TBL.USER_NAME = SP_USER_TBL.USER_NAME ";
        szQuery += "            AND SP_USER_TBL.COMPANY_REG_NO = SP_COMPANY_TBL.COMPANY_REG_NO ";
        szQuery += "            AND USER_TBL.USER_NAME = USERRESP_TBL.EMPNO ";
        szQuery += "            AND USERRESP_TBL.AUTH_CODE = RESPONSE_TBL.AUTH_CODE ";
        szQuery += "            AND USERRESP_TBL.DELYN = 'N' ";
        szQuery += "            AND RESPONSE_TBL.AUTH_CODE LIKE 'WEBSP_%_RESP' ";
        szQuery += "            AND ROWNUM = 1 ";

        try
        {
            using (OracleConnection oracleConnection = new OracleConnection(ConfigurationSettings.AppSettings["GERP_ConnectionString"]))
            {
                oracleConnection.Open();
                using (OracleCommand oracleCommand = new OracleCommand(szQuery, oracleConnection))
                {
                    using (OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand))
                    {
                        DataSet dataSet = new DataSet();
                        oracleDataAdapter.Fill(dataSet);
                        dataTable = dataSet.Tables[0];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //throw ex;
        }

        return dataTable;
    }


    /// <summary>
    /// id,pw 가 맞는지 검증
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="userpw"></param>
    /// <returns></returns>
    private string CheckIdPw(String userid, String userpw)
    {
        string result = "N";
        string szQuery = String.Format("SELECT FND_WEB_SEC.VALIDATE_LOGIN('{0}', '{1}') AS LOGINRESULT FROM DUAL", userid, userpw);

        try
        {
            using (OracleConnection oracleConnection = new OracleConnection(ConfigurationSettings.AppSettings["GERP_ConnectionString"]))
            {
                oracleConnection.Open();
                using (OracleCommand oracleCommand = new OracleCommand(szQuery, oracleConnection))
                {
                    using (OracleDataReader reader = oracleCommand.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            result = reader["LOGINRESULT"].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //throw ex;
        }

        return result;
    }


    #region //2019-12-05; 오라클연결 조회 참고용 자료
    private void CheckId_오라클연결성공_조회1()
    {
        //★패키지에 파라메터를 넘길때는 패키지의 파라메터명과 일치시켜야 함에 주의 한다.
        //패키지 선언부 : FUNCTION validate_login(p_user IN VARCHAR2, p_pwd  IN VARCHAR2) return VARCHAR2;
        string szQuery = "SELECT FND_WEB_SEC.VALIDATE_LOGIN(:p_user, :p_pwd) AS LOGINRESULT FROM DUAL";

        try
        {
            using (OracleConnection oracleConnection = new OracleConnection(ConfigurationSettings.AppSettings["GERP_ConnectionString"]))
            {
                oracleConnection.Open();
                using (OracleCommand oracleCommand = new OracleCommand(szQuery, oracleConnection))
                {
                    oracleCommand.CommandType = CommandType.Text;
                    //oracleCommand.CommandType = CommandType.StoredProcedure;

                    oracleCommand.Parameters.Add("p_user", OracleType.VarChar).Value = "DPI025506"; // uid;
                    oracleCommand.Parameters.Add("p_pwd", OracleType.VarChar).Value = "sangmin1"; // upw;

                    using (OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand))
                    {
                        DataSet dataSet = new DataSet();
                        oracleDataAdapter.Fill(dataSet);
                        DataTable dataTable = dataSet.Tables[0];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }


    private void CheckId_오라클연결성공_조회2()
    {
        //조회조건을 파라메터로 넘기는 방식은 잘 안되는데.. 나중에 시간 있으면 테스트 할 것.
        string szQuery = "SELECT COMPANY_REG_NO, COUNTRY, COMPANY_NAME ";
        szQuery += "FROM EPO_PORTAL_COMPANY_MST WHERE COMPANY_REG_NO LIKE '999990000%' ORDER BY COMPANY_REG_NO ";

        try
        {
            using (OracleConnection oracleConnection = new OracleConnection(ConfigurationSettings.AppSettings["GERP_ConnectionString"]))
            {
                oracleConnection.Open();
                using (OracleCommand oracleCommand = new OracleCommand(szQuery, oracleConnection))
                {
                    //oracleCommand.CommandType = CommandType.Text; //기본값은 Text 라서 생략가능
                    //oracleCommand.CommandType = CommandType.StoredProcedure;

                    using (OracleDataReader reader = oracleCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string company_reg_no = reader["company_reg_no"].ToString();
                            string country = reader["COUNTRY"].ToString();
                            string company_name = reader["COMPANY_NAME"].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }
    #endregion

}
