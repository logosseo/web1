using System;
using System.Web.UI;
using System.Collections;
using System.Data.OracleClient;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Text;
using DIT.BizBase;
using System.Data.SqlClient;
using System.Xml;

public partial class Basic_Main_BasicMainXMLQuery : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "text/xml";
        Response.Expires = -1;
        Response.CacheControl = "no-cache";
        Response.Buffer = true;

        Response.Write("<?xml version='1.0' encoding='utf-8'?><response>");

        try
        {
            pSetData();
        }
        catch (Exception ex)
        {
            pHandleException(ex);
        }
        finally
        {
            Response.Write("</response>");
        }
    }


    /// <summary>
    /// Exception Handling
    /// </summary>
    /// <param name="ex">exception</param>
    /// <returns>XmlDocument</returns>
    private void pHandleException(Exception ex)
    {
        try
        {
            Response.Write("<error><![CDATA[" + ex.Message + "]]></error>");
            //Response.Write("<error><![CDATA[" + Covi.BizService.ApprovalService.ParseStackTrace(ex) + "]]></error>");
        }
        catch (Exception e)
        {
            Response.Write("<error><![CDATA[" + e.Message + "]]></error>");
        }
    }


    /// <summary>
    /// Request 값 xml 객체로 추출
    /// </summary>
    /// <returns>XmlDocument</returns>
    private XmlDocument pParseRequestBytes()
    {
        try
        {
            XmlDocument oXMLData = new XmlDocument();
            System.Text.Decoder oDecoder = System.Text.Encoding.UTF8.GetDecoder();
            System.Byte[] aBytes = Request.BinaryRead(Request.TotalBytes);
            long iCount = oDecoder.GetCharCount(aBytes, 0, aBytes.Length);
            System.Char[] aChars = new char[iCount];
            oDecoder.GetChars(aBytes, 0, aBytes.Length, aChars, 0);
            oXMLData.Load(new System.IO.StringReader(new String(aChars)));
            return oXMLData;
        }
        catch (Exception e)
        {
            throw new Exception(null, e);
        }
    }


    /// <summary>
    /// sql 파라메터 변경 처리
    /// </summary>
    /// <param name="oXML"></param>
    /// <param name="szQuery"></param>
    /// <returns></returns>
    private String GetReplaceParam(XmlDocument oXML, String szQuery)
    {
        // 파라미터 세팅
        System.Xml.XmlNodeList oParams = oXML.SelectNodes("Items/param");
        foreach (System.Xml.XmlNode oParam in oParams)
        {
            string paramName = oParam.SelectSingleNode("name").InnerText;
            string paramValue = oParam.SelectSingleNode("value").InnerText;

            szQuery = szQuery.Replace("$" + paramName, paramValue);
        }

        return szQuery;
    }


    /// <summary>
    /// sql query 실행
    /// </summary>
    private void pSetData()
    {
        XmlDocument oXML = new XmlDocument();

        try
        {
            oXML = pParseRequestBytes();
            string req_xml = oXML.OuterXml;

            string g_connectString = oXML.SelectSingleNode("Items/connectionname").InnerText;
            DataSet ds = null;

            try
            {
                ds = new DataSet();

                string szQuery = oXML.SelectSingleNode("Items/sql").InnerText;
                szQuery = GetQuery(szQuery);
                szQuery = GetReplaceParam(oXML, szQuery);

                using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings[g_connectString]))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(szQuery, connection))
                    {
                        if (oXML.SelectSingleNode("Items/type") != null && oXML.SelectSingleNode("Items/type").InnerText.ToLower() == "sp") { }

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                        {
                            sqlDataAdapter.Fill(ds);
                        }
                    }
                }

                Response.Write(ds.GetXml());
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(null, ex);
            }
            finally
            {
                //code
                oXML = null;
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
        }
    }


    public string GetQuery(string sql)
    {
        string pXML = "";

        switch (sql)
        {
            //NRL 라벨TYPE -> 국가
            case "NRL_Country":
                pXML += "SELECT ";
                pXML += "a.t_COUNTRYCODE, a.t_COUNTRYNAME, a.t_DIFFERENCE_USECOMPCODE ";
                pXML += "FROM NC302_KEYGROUP_COUNTRY a with(nolock) ";
                pXML += "WHERE a.t_DIFFERENCE_USECOMPCODE = '$nrl_labeltype_company' ";
                break;

            //NRL 라벨TYPE -> 거래처
            case "NRL_OrderCompany":
                pXML += "SELECT ";
                pXML += "a.t_ODERCOMPCODE, a.t_ODERCOMPNAME ";
                pXML += "FROM NC302_KEYGROUP_ODERCOMPANY a with(nolock) ";
                pXML += "WHERE a.t_DIFFERENCE_COUNTRYCODE = '$nrl_labeltype_a' ";
                break;

            //NRL 라벨TYPE -> 일련번호
            case "NRL_LabelDocument":
                pXML += "SELECT ";
                pXML += "a.t_IDVIEW, a.t_USECOMPCODE, a.t_ODERCOMPCODE, a.t_WTDIVISIONCODE, a.t_LABELDIVISIONCODE, a.t_LABELSIZECODE, ";
                pXML += "a.t_LABELSERIALNO, a.t_LANDSCAPE, a.t_DESC, a.t_ADDDATETIME, a.t_ADDER, a.t_DATAPROVIDERNAME, a.t_IsDelete, a.t_COUNTRYCODE, ";
                pXML += "(SELECT B.ERPCOMPCODE FROM NC302_KEYGROUP_USECOMPANY B WHERE B.t_USECOMPCODE = A.t_USECOMPCODE) AS ERPCOMPCODE, ";
                pXML += "(SELECT B.t_LABELSIZEVIEW FROM NC302_KEYGROUP_LABELSIZE B WHERE B.t_LABELSIZECODE = A.t_LABELSIZECODE) AS LABELSIZEVIEW, ";
                pXML += "REPLICATE('0',3-LEN(A.t_LABELSERIALNO)) + A.t_LABELSERIALNO AS LABELSERIALNO, ";
                pXML += "REPLICATE('0',3-LEN(A.t_IDVIEW)) + CONVERT(VARCHAR(20), A.t_IDVIEW) AS t_IDVIEW_DP ";
                pXML += "FROM NC302_LABELDOCUMENT a with(nolock) ";
                pXML += "WHERE a.t_ODERCOMPCODE = '$nrl_labeltype_b' ";
                pXML += "AND a.t_IsDelete = '0' ";
                break;
        }
        return pXML;
    }
}


//    private void InitDefaultData()
//    {
//        //품목코드 초기화
//        search_gubun.Items.Add(new ListItem("작지ID 조회", "P"));
//        search_gubun.Items.Add(new ListItem("품목코드 조회", "I"));

//        //공장선택 초기화
//        search_factory.Items.Add(new ListItem("안양공장", "P11"));
//        search_factory.Items.Add(new ListItem("칠서공장", "P12"));
//        search_factory.Items.Add(new ListItem("포승공장", "P13"));
//        search_factory.Items.Add(new ListItem("화성공장", "P21"));

//        //라벨TYPE-국가
//        nrl_labeltype_a.Items.Add(new ListItem("국가", ""));
//        DataTable dt1 = GetLabelCountry();
//        if (dt1 != null && dt1.Rows.Count > 0)
//        {
//            foreach (DataRow dataRow in dt1.Rows)
//            {
//                nrl_labeltype_a.Items.Add(new ListItem(ComUtils.GetDataRowValue(dataRow, "t_COUNTRYNAME"), ComUtils.GetDataRowValue(dataRow, "t_COUNTRYCODE")));
//            }
//        }

//        //라벨TYPE-거래처
//        nrl_labeltype_b.Items.Add(new ListItem("거래처", ""));
//        dt1 = GetLabelOrderCompany(GetSession("ORGANIZATION_ID"));
//        if (dt1 != null && dt1.Rows.Count > 0)
//        {
//            foreach (DataRow dataRow in dt1.Rows)
//            {
//                nrl_labeltype_b.Items.Add(new ListItem(ComUtils.GetDataRowValue(dataRow, "t_ODERCOMPNAME"), ComUtils.GetDataRowValue(dataRow, "t_ODERCOMPCODE")));
//            }
//        }

//        //라벨TYPE-전체
//        nrl_labeltype_c.Items.Add(new ListItem("일련번호", ""));
//        dt1 = GetLabelDocument(GetSession("ORGANIZATION_ID"));
//        if (dt1 != null && dt1.Rows.Count > 0)
//        {
//            foreach (DataRow dataRow in dt1.Rows)
//            {
//                // +  +  
//                nrl_labeltype_c.Items.Add(new ListItem(
//                    "NO:" + ComUtils.GetDataRowValue(dataRow, "t_LABELSERIALNO") + "/사이즈:" + ComUtils.GetDataRowValue(dataRow, "t_LABELSIZEVIEW") + "/설명:" + ComUtils.GetDataRowValue(dataRow, "t_DESC")
//                    , ComUtils.GetDataRowValue(dataRow, "t_IDVIEW")));
//            }
//        }
//    }


//    private void GetParameterData()
//    {
//        q_gubun.Value = _search_gubun = ComUtils.GetDefaultString(Request["search_gubun"], "");
//        q_code.Value = _search_code = ComUtils.GetDefaultString(Request["search_code"], "");
//        q_factory.Value = _search_factory = ComUtils.GetDefaultString(Request["search_factory"], "");
//    }


//    private void selectOracleData()
//    {
//        string szQuery = "EENG_MDM_LABEL_PKG.NGLS_LABEL_OUTPUT";

//        try
//        {
//            using (OracleConnection oracleConnection = new OracleConnection(ConfigurationSettings.AppSettings["GERP_ConnectionString"]))
//            {
//                oracleConnection.Open();

//                //한국어로 나와야 해서 실행함.
//                using (OracleCommand oracleCommand = new OracleCommand("ALTER SESSION SET nls_language = 'KOREAN'", oracleConnection))
//                {
//                    oracleCommand.ExecuteNonQuery();
//                }

//                    using (OracleCommand oracleCommand = new OracleCommand(szQuery, oracleConnection))
//                {
//                    //oracleCommand.CommandType = CommandType.Text;
//                    oracleCommand.CommandType = CommandType.StoredProcedure;

//                    oracleCommand.Parameters.Add("P_RETURN_STATUS", OracleType.VarChar).Value = ParameterDirection.Output;
//                    oracleCommand.Parameters.Add("P_ERROR_MESSAGE", OracleType.VarChar).Value = ParameterDirection.Output;
//                    oracleCommand.Parameters.Add("P_GUBUN", OracleType.VarChar).Value = _search_gubun; //"I";
//                    oracleCommand.Parameters.Add("P_SITE_CODE", OracleType.VarChar).Value = _search_factory; //"P11";
//                    oracleCommand.Parameters.Add("P_INPUT_CODE", OracleType.VarChar).Value = _search_code; //"PAA007321/18L";
//                    oracleCommand.Parameters.Add("P_RESULT", OracleType.Cursor).Direction = ParameterDirection.Output;

//                    using (OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand))
//                    {
//                        DataSet dataSet = new DataSet();
//                        oracleDataAdapter.Fill(dataSet);
//                        _erpDataTable = dataSet.Tables[0];
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            //오라클에서 조회된 결과가 없을 경우 exception 으로 빠지는데 임의로 빈 DataTable 을 생성하여 오류를 방지하고자 함.(조회된 데이터가 없을 때 화면 초기화 하는 역활도 함.)
//            //{"ORA-06502: PL/SQL: numeric or value error: character string buffer too small\nORA-06512: at \"APPS.EENG_MDM_LABEL_PKG\", line 283\nORA-06502: PL/SQL: numeric or value error: character string buffer too small\nORA-06512: at line 1\n"}
//            _erpDataTable = new DataTable();
//            _erpDataTable.Rows.Add(_erpDataTable.NewRow());
//            //throw ex;
//        }
//    }


//    protected void btnSearch_OnClick(object sender, EventArgs e)
//    {
//        GetParameterData();
//        SetDataBind();
//    }


//    private DataTable GetLabelCountry()
//    {
//        DataTable dt = null;

//        //최종쿼리생성
//        String query = string.Empty;
//        query += "SELECT t_COUNTRYCODE, t_COUNTRYNAME FROM NC302_KEYGROUP_COUNTRY ";

//        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand(query, connection))
//            {
//                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
//                {
//                    DataSet dataSet = new DataSet();
//                    sqlDataAdapter.Fill(dataSet);
//                    dt = dataSet.Tables[0];
//                }
//            }
//        }

//        return dt;
//    }


//    private DataTable GetLabelOrderCompany(String organization_id)
//    {
//        DataTable dt = null;

//        //최종쿼리생성
//        String query = string.Empty;
//        query += "SELECT a.t_ODERCOMPCODE, a.t_ODERCOMPNAME, a.t_USECOMPCODE ";
//        query += "FROM NC302_KEYGROUP_ODERCOMPANY a with(nolock) join NC302_KEYGROUP_USECOMPANY b with(nolock) on b.t_USECOMPCODE = a.t_USECOMPCODE ";
//        query += "WHERE b.ORGANIZATION_ID = '" + organization_id + "' ";

//        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand(query, connection))
//            {
//                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
//                {
//                    DataSet dataSet = new DataSet();
//                    sqlDataAdapter.Fill(dataSet);
//                    dt = dataSet.Tables[0];
//                }
//            }
//        }

//        return dt;
//    }


//    private DataTable GetLabelDocument(String organization_id)
//    {
//        DataTable dt = null;

//        //최종쿼리생성
//        String query = string.Empty;
//        query += "SELECT ";
//        query += "a.t_IDVIEW, a.t_USECOMPCODE, a.t_ODERCOMPCODE, a.t_WTDIVISIONCODE, a.t_LABELDIVISIONCODE, a.t_LABELSIZECODE, ";
//        query += "a.t_LABELSERIALNO, a.t_LANDSCAPE, a.t_DESC, a.t_ADDDATETIME, a.t_ADDER, a.t_DATAPROVIDERNAME, a.t_IsDelete, a.t_COUNTRYCODE ";
//        query += "FROM NC302_LABELDOCUMENT a with(nolock) join NC302_KEYGROUP_USECOMPANY b with(nolock) on b.t_USECOMPCODE = a.t_USECOMPCODE ";
//        query += "WHERE b.ORGANIZATION_ID = '" + organization_id + "' ";
//        query += "AND a.t_IsDelete = '0' ";

//        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand(query, connection))
//            {
//                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
//                {
//                    DataSet dataSet = new DataSet();
//                    sqlDataAdapter.Fill(dataSet);
//                    dt = dataSet.Tables[0];
//                }
//            }
//        }

//        return dt;
//    }


//    private void selectGhsMsds()
//    {
//        string prodcd = _item_number.Split('/')[0];

//        //최종쿼리생성
//        String query = string.Empty;
//        query += "SELECT A.idx, A.sPRODCD, A.sLANG, A.sCOMPANY, A.sALT, A.HANDWORK, ";
//        query += "B.HandWorkDtSeq, B.Revision, B.GubunCd, B.WordCd, B.WordValue ";
//        query += "FROM PORTAL_GHSMSDS_RENEW_TBL A WITH(NOLOCK) JOIN PORTAL_GHSMSDS_HANDWORK_DT_TBL B WITH(NOLOCK) ON B.Revision = A.REVISION ";
//        query += "WHERE A.idx = (SELECT TOP 1 idx FROM PORTAL_GHSMSDS_RENEW_TBL WHERE sPRODCD = '" + prodcd  + "' AND sLANG = 'KO' AND HANDWORK = 'Y' AND DELYN = 'N' ORDER BY REGDATE DESC) ";
//        query += "AND B.DelDate IS NULL ";

//        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand(query, connection))
//            {
//                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
//                {
//                    DataSet dataSet = new DataSet();
//                    sqlDataAdapter.Fill(dataSet);
//                    _ghsDataTable = dataSet.Tables[0];
//                }
//            }
//        }
//    }


//    private void SetDataBind()
//    {
//        try
//        {
//            if (_search_gubun != "" && _search_code != "" && _search_factory != "")
//            {
//                selectOracleData();

//                if (_search_gubun == "I")
//                {
//                    _item_number = _search_code;
//                } else
//                {
//                    if (_erpDataTable != null && _erpDataTable.Rows.Count > 0)
//                    {
//                        _item_number = ComUtils.GetDataRowValue(_erpDataTable.Rows[0], "item_number");
//                    }
//                }

//                selectGhsMsds(); //GHS-MSDS 조회
//                SetView();
//            }

//        }
//        catch (Exception ex)
//        {
//            //throw ex;
//        }
//    }


//    private void SetView()
//    {
//        try
//        {
//            if (_erpDataTable != null && _erpDataTable.Rows.Count > 0)
//            {
//                DataRow dataRow = _erpDataTable.Rows[0];
//                company_code.Value = ComUtils.GetDataRowValue(dataRow, "company_code"); //회사
//                site_code.Value = ComUtils.GetDataRowValue(dataRow, "site_code"); //사이트
//                item_number.Value = ComUtils.GetDataRowValue(dataRow, "item_number"); //품번
//                lot_number.Value = ComUtils.GetDataRowValue(dataRow, "lot_number"); //로트번호
//                mfg_date.Value = DateTime.Now.ToString("yyyy-MM-dd"); //제조일자
//                alternateitem.Value = ComUtils.GetDataRowValue(dataRow, "alternateitem"); //대응코드
//                item_desc.Value = ComUtils.GetDataRowValue(dataRow, "item_desc"); //품명1
//                item_desc2.Value = ComUtils.GetDataRowValue(dataRow, "item_desc2"); //품명2
//                mix_item_code.Value = ComUtils.GetDataRowValue(dataRow, "mix_item_code"); //사용희석제
//                label_color_type_name.Value = ComUtils.GetDataRowValue(dataRow, "label_color_type_name"); //라벨색상
//                mix_rate.Value = ComUtils.GetDataRowValue(dataRow, "mix_rate"); //혼합비
//                msds_code.Value = ComUtils.GetDataRowValue(dataRow, "msds_code"); //msds코드
//                inwha_point.Value = ComUtils.GetDataRowValue(dataRow, "inwha_point"); //인화점
//                risk_material.Value = ComUtils.GetDataRowValue(dataRow, "risk_material"); //위험물품명
//                cautionclass.Value = ComUtils.GetDataRowValue(dataRow, "cautionclass"); //위험등급
//                chemical_name.Value = ComUtils.GetDataRowValue(dataRow, "chemical_name"); //위험물질명
//                include_rate.Value = ComUtils.GetDataRowValue(dataRow, "include_rate"); //위험물함량
//                regulatory_year.Value = ComUtils.GetDataRowValue(dataRow, "regulatory_year"); //voc년도
//                regulatory_lv_name.Value = ComUtils.GetDataRowValue(dataRow, "regulatory_lv1_name") + " " + ComUtils.GetDataRowValue(dataRow, "regulatory_lv2_name"); //voc적용기준1, 2 값은 합쳐서 표시
//                regulatory_voc_value.Value = ComUtils.GetDataRowValue(dataRow, "regulatory_voc_value"); //voc기준값
//                voc_value.Value = ComUtils.GetDataRowValue(dataRow, "voc_value"); //voc표시값
//                car_type.Value = ComUtils.GetDataRowValue(dataRow, "car_type"); //차종
//                kungwha_item_code.Value = ComUtils.GetDataRowValue(dataRow, "kungwha_item_code"); //경화제명
//                type_code.Value = ComUtils.GetDataRowValue(dataRow, "type_code"); //종류
//                used_type_code.Value = ComUtils.GetDataRowValue(dataRow, "used_type_code"); //용도
//                dopo_dimension.Value = ComUtils.GetDataRowValue(dataRow, "dopo_dimension"); //도포면적
//                label_type_disp.Value = ComUtils.GetDataRowValue(dataRow, "label_type_disp"); //라벨타입
//                airback_exist_flag.Value = ComUtils.GetDataRowValue(dataRow, "airback_exist_flag"); //에어백유무
//                available_period.Value = ComUtils.GetDataRowValue(dataRow, "available_period"); //유효기간
//                actual_capacity.Value = ComUtils.GetDataRowValue(dataRow, "actual_capacity"); //실용량
//                color_type.Value = ComUtils.GetDataRowValue(dataRow, "color_type"); //칼라타입
//                remark.Value = ComUtils.GetDataRowValue(dataRow, "remark"); //추가사항
//                customer_item_number.Value = ComUtils.GetDataRowValue(dataRow, "customer_item_number"); //거래처품번
//                customer_factory.Value = ComUtils.GetDataRowValue(dataRow, "customer_factory"); //거래처공장
//                packing_weight.Value = ComUtils.GetDataRowValue(dataRow, "packing_weight"); //포장중량
//                can_weight.Value = ComUtils.GetDataRowValue(dataRow, "can_weight"); //용기중량
//                color_code.Value = ComUtils.GetDataRowValue(dataRow, "color_code"); //컬러코드
//                gravity.Value = ComUtils.GetDataRowValue(dataRow, "gravity"); //비중
//                toxic.Value = ComUtils.GetDataRowValue(dataRow, "toxic"); //유독물여부
//                accident_contrast.Value = ComUtils.GetDataRowValue(dataRow, "accident_contrast"); //사고대비물질여부
//                restricted.Value = ComUtils.GetDataRowValue(dataRow, "restricted"); //제한물질여부
//                prohibited_handling.Value = ComUtils.GetDataRowValue(dataRow, "prohibited_handling"); //금지물질

//                //web_viscosity_sec.Value = ComUtils.GetDataRowValue(dataRow, "web_viscosity_sec"); //점도(초)-----일회성입력
//                //web_viscosity_20c.Value = ComUtils.GetDataRowValue(dataRow, "web_viscosity_20c"); //점도(20도)-----일회성입력
//                //web_undiluted_lot.Value = ComUtils.GetDataRowValue(dataRow, "web_undiluted_lot"); //원액lot-----일회성입력
//                //web_coloring_heat.Value = ComUtils.GetDataRowValue(dataRow, "web_coloring_heat"); //착색열-----일회성입력
//                //web_grading.Value = ComUtils.GetDataRowValue(dataRow, "web_grading"); //입도-----일회성입력

//                //picture.Value = ComUtils.GetDataRowValue(dataRow, "picture"); //그림
//                divPictureList.Text = GetPictureHtml(ComUtils.GetDataRowValue(dataRow, "picture")); //그림

//                signal.Value = (ComUtils.GetDataRowValue(dataRow, "signal").Split('$'))[0]; //신호어
//                danger_expression.Value = ComUtils.GetDataRowValue(dataRow, "danger_expression").Replace("$", "\n"); //유해위험문구
//                precaution_prevention.Value = ComUtils.GetDataRowValue(dataRow, "precaution_prevention").Replace("$", "\n"); //예방조치문구(예방)
//                precaution_react.Value = ComUtils.GetDataRowValue(dataRow, "precaution_react").Replace("$", "\n"); //예방조치문구(대응)
//                precaution_storage.Value = ComUtils.GetDataRowValue(dataRow, "precaution_storage").Replace("$", "\n"); //예방조치문구(저장)
//                precaution_disuse.Value = ComUtils.GetDataRowValue(dataRow, "precaution_disuse").Replace("$", "\n"); //예방조치문구(폐기)

//                //List 형태로 구성
//                //mark_type.Value = ComUtils.GetDataRowValue(dataRow, "mark_type"); //인증종류
//                //certification_no.Value = ComUtils.GetDataRowValue(dataRow, "certification_no"); //인증번호
//                //certification_spec.Value = ComUtils.GetDataRowValue(dataRow, "certification_spec"); //인증규격
//                divCertificationList.Text = GetCertificationHtml(ComUtils.GetDataRowValue(dataRow, "mark_type"), ComUtils.GetDataRowValue(dataRow, "certification_no"), ComUtils.GetDataRowValue(dataRow, "certification_spec"));
//            }

//            //GHS-MSDS 정보가 있으면 대체함.
//            if (_ghsDataTable != null && _ghsDataTable.Rows.Count > 0)
//            {
//                divPictureList.Text = GetPictureHtml(GetGhsData(_ghsDataTable, "ghspic")); //그림
//                signal.Value = (GetGhsData(_ghsDataTable, "ghssign").Split('$'))[0]; //신호어
//                danger_expression.Value = GetGhsData(_ghsDataTable, "yhwh").Replace("$", "\n"); //유해위험문구
//                precaution_prevention.Value = GetGhsData(_ghsDataTable, "ybyb").Replace("$", "\n"); //예방조치문구(예방)
//                precaution_react.Value = GetGhsData(_ghsDataTable, "ybdu").Replace("$", "\n"); //예방조치문구(대응)
//                precaution_storage.Value = GetGhsData(_ghsDataTable, "ybjj").Replace("$", "\n"); //예방조치문구(저장)
//                precaution_disuse.Value = GetGhsData(_ghsDataTable, "ybpg").Replace("$", "\n"); //예방조치문구(폐기)
//            }

//        }
//        catch (Exception ex)
//        {
//            //throw ex;
//        }
//    }


//    private String GetGhsData(DataTable table, string gubun)
//    {
//        String result = string.Empty;
//        Boolean first = true;

//        foreach (DataRow dataRow in table.Rows)
//        {
//            if (ComUtils.GetDataRowValue(dataRow, "GubunCd") == gubun)
//            {
//                if (first)
//                {
//                    result += ComUtils.GetDataRowValue(dataRow, "WordValue");
//                    first = false;
//                }
//                else
//                {
//                    result += _token_string + ComUtils.GetDataRowValue(dataRow, "WordValue");
//                }
//            }
//        }

//        return result;
//    }


//    private String GetPictureHtml(String pic)
//    {
//        StringBuilder html = new StringBuilder(); ;
//        html.AppendFormat("<li><p><img src='/images/GHS_느낌표.gif' /></p><input type='checkbox' name='picture' value='느낌표' {0} /><label>느낌표</label></li>", pic.IndexOf("느낌표") >= 0 ? "checked" : "");
//        html.AppendFormat("<li><p><img src='/images/GHS_물고기.gif' /></p><input type='checkbox' name='picture' value='물고기' {0} /><label>물고기</label></li>", pic.IndexOf("물고기") >= 0 ? "checked" : "");
//        html.AppendFormat("<li><p><img src='/images/GHS_부식.gif' /></p><input type='checkbox' name='picture' value='부식' {0} /><label>부식</label></li>", pic.IndexOf("부식") >= 0 ? "checked" : "");
//        html.AppendFormat("<li><p><img src='/images/GHS_불꽃.gif' /></p><input type='checkbox' name='picture' value='불꽃' {0} /><label>불꽃</label></li>", pic.IndexOf("불꽃") >= 0 ? "checked" : "");
//        html.AppendFormat("<li><p><img src='/images/GHS_사람.gif' /></p><input type='checkbox' name='picture' value='사람' {0} /><label>사람</label></li>", pic.IndexOf("사람") >= 0 ? "checked" : "");
//        html.AppendFormat("<li><p><img src='/images/GHS_폭발.gif' /></p><input type='checkbox' name='picture' value='폭발' {0} /><label>폭발</label></li>", pic.IndexOf("폭발") >= 0 ? "checked" : "");
//        html.AppendFormat("<li><p><img src='/images/GHS_해골.gif' /></p><input type='checkbox' name='picture' value='해골' {0} /><label>해골</label></li>", pic.IndexOf("해골") >= 0 ? "checked" : "");

//        return html.ToString();
//    }


//    private String GetCertificationHtml(String mark_type, String certification_no, String certification_spec)
//    {
//        StringBuilder html = new StringBuilder(); ;

//        string[] arr_mark_type = mark_type.Split('$');
//        string[] arr_certification_no = certification_no.Split('$');
//        string[] arr_certification_spec = certification_spec.Split('$');

//        if(arr_mark_type != null && arr_mark_type.Length > 0)
//        {
//            for(int i = 0; i < arr_mark_type.Length; i++)
//            {
//                html.AppendFormat("<tr>");
//                html.AppendFormat("<td class='align_c'>{0}</td>", i+1);
//                html.AppendFormat("<td><input type='text' name='mark_type' value='{0}' /></td>", arr_mark_type[i]);
//                html.AppendFormat("<td><input type='text' name='certification_no' value='{0}' /></td>", arr_certification_no[i]);
//                html.AppendFormat("<td><input type='text' name='certification_spec' value='{0}' /></td>", arr_certification_spec[i]);
//                html.AppendFormat("</tr>");
//            }
//        }

//        return html.ToString();
//    }

//}
