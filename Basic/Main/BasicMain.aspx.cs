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
using System.Web;

public partial class Basic_Main_BasicMain : PageBase
{
    private string _token_string = "$";
    protected string _search_gubun = String.Empty;
    protected string _search_code = String.Empty;
    protected string _search_lang = String.Empty;
    protected string _search_factory = String.Empty;

    protected string _ghs_item_number = String.Empty; //GHS-MSDS 조회에 사용

    //DB조회결과처리용
    protected string _gerp_exception_yn = string.Empty;
    protected string _p_return_status = string.Empty;
    protected string _p_error_message = string.Empty;
    protected DataTable _erpDataTable = null;
    protected DataTable _ghsDataTable = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InitDefaultData();
                GetParameterData();
                SetDataBind();
            }
        }
        catch (Exception ex)
        {

        }
    }


    private void InitDefaultData()
    {
        DataTable dt1 = null;

        //품목코드 초기화
        search_gubun.Items.Add(new ListItem("작지ID 조회", "P"));
        search_gubun.Items.Add(new ListItem("품목코드 조회", "I"));

        //언어선택
        search_lang.Items.Add(new ListItem("한글", "KOREAN"));
        search_lang.Items.Add(new ListItem("영어", "AMERICAN"));

        //공장선택 초기화
        search_factory.Items.Add(new ListItem("NRP 안양공장", "P11"));
        search_factory.Items.Add(new ListItem("NRP 칠서공장", "P12"));
        search_factory.Items.Add(new ListItem("NRP 포승공장", "P13"));
        search_factory.Items.Add(new ListItem("NAC 화성공장", "P21"));
        search_factory.Items.Add(new ListItem("NRB 천안공장", "P31"));
        search_factory.Items.Add(new ListItem("NCH 안산공장", "P51"));
        search_factory.Items.Add(new ListItem("NCC 포항공장", "P61"));
        search_factory.Items.Add(new ListItem("NCC 안양공장", "P62"));
        search_factory.Items.Add(new ListItem("NRC 안산", "P71"));

        //라벨TYPE-회사코드
        nrl_labeltype_company.Items.Add(new ListItem("===회사===", ""));
        dt1 = GetLabelCompany();
        if (dt1 != null && dt1.Rows.Count > 0)
        {
            foreach (DataRow dataRow in dt1.Rows)
            {
                nrl_labeltype_company.Items.Add(new ListItem(ComUtils.GetDataRowValue(dataRow, "t_USECOMPNAME"), ComUtils.GetDataRowValue(dataRow, "t_USECOMPCODE")));
            }
        }

        //동적으로 표시하는 항목인데, 초기에 빈값으로 보이지 않도록 넣어줌.
        //nrl_labeltype_a.Items.Add(new ListItem("===국가===", ""));
        //nrl_labeltype_b.Items.Add(new ListItem("===거래처===", ""));
        //nrl_labeltype_c.Items.Add(new ListItem("===일련번호===", ""));

        //쿠키값으로 초기화
        GetInitLabelCookies();
    }


    private void GetParameterData()
    {
        q_gubun.Value = _search_gubun = ComUtils.GetDefaultString(Request["search_gubun"], "");
        q_code.Value = _search_code = ComUtils.GetDefaultString(Request["search_code"], "");
        q_lang.Value = _search_lang = ComUtils.GetDefaultString(Request["search_lang"], "");
        q_factory.Value = _search_factory = ComUtils.GetDefaultString(Request["search_factory"], "");
    }


    private void selectOracleData()
    {
        string szQuery = "EENG_MDM_LABEL_PKG.NGLS_LABEL_OUTPUT";

        try
        {
            using (OracleConnection oracleConnection = new OracleConnection(ConfigurationSettings.AppSettings["GERP_ConnectionString"]))
            {
                oracleConnection.Open();

                //한국어로 나와야 해서 실행함.
                using (OracleCommand oracleCommand = new OracleCommand("ALTER SESSION SET nls_language = '" + _search_lang + "'", oracleConnection))
                {
                    oracleCommand.ExecuteNonQuery();
                }

                using (OracleCommand oracleCommand = new OracleCommand(szQuery, oracleConnection))
                {
                    //oracleCommand.CommandType = CommandType.Text;
                    oracleCommand.CommandType = CommandType.StoredProcedure;

                    oracleCommand.Parameters.Add("P_RETURN_STATUS", OracleType.VarChar,  2000).Direction = ParameterDirection.Output;
                    oracleCommand.Parameters.Add("P_ERROR_MESSAGE", OracleType.VarChar, 2000).Direction = ParameterDirection.Output;
                    oracleCommand.Parameters.Add("P_GUBUN", OracleType.VarChar).Value = _search_gubun; //"I";
                    oracleCommand.Parameters.Add("P_SITE_CODE", OracleType.VarChar).Value = _search_factory; //"P11";
                    oracleCommand.Parameters.Add("P_INPUT_CODE", OracleType.VarChar).Value = _search_code; //"PAA007321/18L";
                    oracleCommand.Parameters.Add("P_RESULT", OracleType.Cursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand))
                    {
                        try
                        {
                            DataSet dataSet = new DataSet();
                            oracleDataAdapter.Fill(dataSet);
                            _erpDataTable = dataSet.Tables[0];
                            _p_return_status = ComUtils.GetNotNullToString(oracleCommand.Parameters["P_RETURN_STATUS"].Value);
                            _p_error_message = ComUtils.GetNotNullToString(oracleCommand.Parameters["P_ERROR_MESSAGE"].Value);

                        } catch(Exception ex)
                        {
                            _gerp_exception_yn = "Y";
                            //오라클에서 조회된 결과가 없을 경우 exception 으로 빠지는데 임의로 빈 DataTable 을 생성하여 오류를 방지하고자 함.(조회된 데이터가 없을 때 화면 초기화 하는 역활도 함.)
                            //{"ORA-06502: PL/SQL: numeric or value error: character string buffer too small\nORA-06512: at \"APPS.EENG_MDM_LABEL_PKG\", line 283\nORA-06502: PL/SQL: numeric or value error: character string buffer too small\nORA-06512: at line 1\n"}
                            //{"ORA-24338: statement handle not executed\n"}
                            _erpDataTable = new DataTable();
                            _erpDataTable.Rows.Add(_erpDataTable.NewRow());
                            _p_return_status = ComUtils.GetNotNullToString(oracleCommand.Parameters["P_RETURN_STATUS"].Value);
                            _p_error_message = ComUtils.GetNotNullToString(oracleCommand.Parameters["P_ERROR_MESSAGE"].Value);
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


    protected void btnSearch_OnClick(object sender, EventArgs e)
    {
        SetLabelCookies(ComUtils.GetDefaultString(Request["search_gubun"], ""), ComUtils.GetDefaultString(Request["search_lang"], ""), ComUtils.GetDefaultString(Request["search_factory"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_company"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_a"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_b"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_c"], ""));
        GetParameterData();
        SetDataBind();
    }


    /// <summary>
    /// 라벨TYPE-회사
    /// </summary>
    /// <returns></returns>
    private DataTable GetLabelCompany()
    {
        DataTable dt = null;

        //최종쿼리생성
        String query = string.Empty;
        query += "SELECT t_USECOMPCODE, t_USECOMPNAME, ORGANIZATION_ID, ERPCOMPCODE FROM NC302_KEYGROUP_USECOMPANY ORDER BY t_USECOMPCODE ";

        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    dt = dataSet.Tables[0];
                }
            }
        }

        return dt;
    }


    /// <summary>
    /// 라벨TYPE-국가
    /// </summary>
    /// <param name="p_nrl_labeltype_company"></param>
    /// <returns></returns>
    private DataTable GetLabelCountry(String p_nrl_labeltype_company)
    {
        DataTable dt = null;

        //최종쿼리생성
        String query = string.Empty;
        query += "SELECT ";
        query += "a.t_COUNTRYCODE, a.t_COUNTRYNAME, a.t_DIFFERENCE_USECOMPCODE ";
        query += "FROM NC302_KEYGROUP_COUNTRY a with(nolock) ";
        query += "WHERE a.t_DIFFERENCE_USECOMPCODE = '" + p_nrl_labeltype_company + "' ";
        query += "ORDER BY a.SortOrder, a.t_COUNTRYNAME ";

        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    dt = dataSet.Tables[0];
                }
            }
        }

        return dt;
    }


    /// <summary>
    /// 라벨TYPE-거래처
    /// </summary>
    /// <param name="p_nrl_labeltype_a"></param>
    /// <returns></returns>
    private DataTable GetLabelOrderCompany(String p_nrl_labeltype_a)
    {
        DataTable dt = null;

        //최종쿼리생성
        String query = string.Empty;
        query += "SELECT ";
        query += "a.t_ODERCOMPCODE, a.t_ODERCOMPNAME ";
        query += "FROM NC302_KEYGROUP_ODERCOMPANY a with(nolock) ";
        query += "WHERE a.t_DIFFERENCE_COUNTRYCODE = '" + p_nrl_labeltype_a + "' ";
        query += "ORDER BY a.SortOrder, a.t_ODERCOMPNAME ";

        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    dt = dataSet.Tables[0];
                }
            }
        }

        return dt;
    }


    /// <summary>
    /// 라벨TYPE-일련번호
    /// </summary>
    /// <param name="p_nrl_labeltype_b"></param>
    /// <returns></returns>
    private DataTable GetLabelDocument(String p_nrl_labeltype_b)
    {
        DataTable dt = null;

        //최종쿼리생성
        String query = string.Empty;
        query += "SELECT ";
        query += "a.t_IDVIEW, a.t_USECOMPCODE, a.t_ODERCOMPCODE, a.t_WTDIVISIONCODE, a.t_LABELDIVISIONCODE, a.t_LABELSIZECODE, ";
        query += "a.t_LABELSERIALNO, a.t_LANDSCAPE, a.t_DESC, a.t_ADDDATETIME, a.t_ADDER, a.t_DATAPROVIDERNAME, a.t_IsDelete, a.t_COUNTRYCODE, ";
        query += "(SELECT B.ERPCOMPCODE FROM NC302_KEYGROUP_USECOMPANY B WHERE B.t_USECOMPCODE = A.t_USECOMPCODE) AS ERPCOMPCODE, ";
        query += "(SELECT B.t_LABELSIZEVIEW FROM NC302_KEYGROUP_LABELSIZE B WHERE B.t_LABELSIZECODE = A.t_LABELSIZECODE) AS LABELSIZEVIEW, ";
        query += "REPLICATE('0',3-LEN(A.t_LABELSERIALNO)) + CONVERT(VARCHAR(20), A.t_LABELSERIALNO) AS LABELSERIALNO, ";
        query += "REPLICATE('0',4-LEN(A.t_IDVIEW)) + CONVERT(VARCHAR(20), A.t_IDVIEW) AS t_IDVIEW_DP ";
        query += "FROM NC302_LABELDOCUMENT a with(nolock) ";
        query += "WHERE a.t_ODERCOMPCODE = '" + p_nrl_labeltype_b + "' ";
        query += "AND a.t_IsDelete = '0' ";
        query += "ORDER BY a.SortOrder ";

        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    dt = dataSet.Tables[0];
                }
            }
        }

        return dt;
    }


    private void selectGhsMsds()
    {
        string prodcd = _ghs_item_number.Split('/')[0];

        //최종쿼리생성
        String query = string.Empty;
        query += "SELECT A.idx, A.sPRODCD, A.sLANG, A.sCOMPANY, A.sALT, A.HANDWORK, ";
        query += "B.HandWorkDtSeq, B.Revision, B.GubunCd, B.WordCd, B.WordValue ";
        query += "FROM PORTAL_GHSMSDS_RENEW_TBL A WITH(NOLOCK) JOIN PORTAL_GHSMSDS_HANDWORK_DT_TBL B WITH(NOLOCK) ON B.Revision = A.REVISION ";
        query += "WHERE A.idx = (SELECT TOP 1 idx FROM PORTAL_GHSMSDS_RENEW_TBL WHERE sPRODCD = '" + prodcd + "' AND sLANG = 'KO' AND HANDWORK = 'Y' AND DELYN = 'N' ORDER BY REGDATE DESC) ";
        query += "AND B.DelDate IS NULL ";

        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    _ghsDataTable = dataSet.Tables[0];
                }
            }
        }
    }


    private void SetDataBind()
    {
        try
        {
            if (_search_gubun != "" && _search_code != "" && _search_factory != "")
            {
                selectOracleData();

                if (_search_gubun == "I")
                {
                    _ghs_item_number = _search_code;
                }
                else
                {
                    if (_erpDataTable != null && _erpDataTable.Rows.Count > 0)
                    {
                        _ghs_item_number = ComUtils.GetDataRowValue(_erpDataTable.Rows[0], "item_number");
                    }
                }

                selectGhsMsds(); //GHS-MSDS 조회
                SetView();

                //if (_gerp_exception_yn == "Y")
                if (_p_return_status == "E")
                {
                    //화면 load 되기전에 실행(aspx 의 form 바로 아래 스크립트가 추가된다.)
                    //ClientScript.RegisterClientScriptBlock(GetType(), "message", string.Format("<script>alert('p_return_status:{0}\\np_error_message:{1}');</script>", _p_return_status, _p_error_message));
                    ClientScript.RegisterClientScriptBlock(GetType(), "message", string.Format("<script>alert('{0}');</script>", _p_error_message));
                }
            }

        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }


    private void SetView()
    {
        try
        {
            if (_erpDataTable != null && _erpDataTable.Rows.Count > 0)
            {
                DataRow dataRow = _erpDataTable.Rows[0];
                company_code.Value = ComUtils.GetDataRowValue(dataRow, "company_code"); //회사
                site_code.Value = ComUtils.GetDataRowValue(dataRow, "site_code"); //사이트
                item_number.Value = ComUtils.GetDataRowValue(dataRow, "item_number"); //품번
                lot_number.Value = ComUtils.GetDataRowValue(dataRow, "lot_number"); //로트번호
                prod_qty.Value = ComUtils.GetDataRowValue(dataRow, "prod_qty"); //생산수량
                prod_qty_kg.Value = ComUtils.GetDataRowValue(dataRow, "prod_qty_kg"); //생산수량(KG)
                so_qty.Value = ComUtils.GetDataRowValue(dataRow, "so_qty"); //주문수량
                so_qty_kg.Value = ComUtils.GetDataRowValue(dataRow, "so_qty_kg"); //주문수량(KG)
                mfg_date.Value = (ComUtils.GetDataRowValue(dataRow, "item_number") != "") ? DateTime.Now.ToString("yyyy-MM-dd") : ""; //제조일자(현재일자)
                alternateitem.Value = ComUtils.GetDataRowValue(dataRow, "alternateitem"); //대응코드
                salesrep_name.Value = ComUtils.GetDataRowValue(dataRow, "salesrep_name"); //판매자
                item_desc.Value = ComUtils.GetDataRowValue(dataRow, "item_desc"); //품명1
                item_desc2.Value = ComUtils.GetDataRowValue(dataRow, "item_desc2"); //품명2
                wip_item_desc.Value = ComUtils.GetDataRowValue(dataRow, "wip_item_desc"); //배합품품명
                mix_item_code.Value = ComUtils.GetDataRowValue(dataRow, "mix_item_code"); //사용희석제
                label_color_type_name.Value = ComUtils.GetDataRowValue(dataRow, "label_color_type_name"); //라벨색상
                label_color_type_code.Value = GetLabelColorTypeCode(label_color_type_name.Value); //라벨색상코드
                mix_rate.Value = ComUtils.GetDataRowValue(dataRow, "mix_rate"); //혼합비
                msds_code.Value = ComUtils.GetDataRowValue(dataRow, "msds_code"); //msds코드
                inwha_point.Value = ComUtils.GetDataRowValue(dataRow, "inwha_point"); //인화점
                risk_material.Value = ComUtils.GetDataRowValue(dataRow, "risk_material"); //위험물품명
                cautionclass.Value = ComUtils.GetDataRowValue(dataRow, "cautionclass"); //위험등급
                chemical_name.Value = ComUtils.GetDataRowValue(dataRow, "chemical_name"); //위험물질명
                include_rate.Value = ComUtils.GetDataRowValue(dataRow, "include_rate"); //위험물함량
                regulatory_year.Value = ComUtils.GetDataRowValue(dataRow, "regulatory_year"); //voc년도
                regulatory_lv_name.Value = ComUtils.GetDataRowValue(dataRow, "regulatory_lv1_name") + " " + ComUtils.GetDataRowValue(dataRow, "regulatory_lv2_name"); //voc적용기준1, 2 값은 합쳐서 표시
                regulatory_voc_value.Value = ComUtils.GetDataRowValue(dataRow, "regulatory_voc_value"); //voc기준값
                voc_value.Value = ComUtils.GetDataRowValue(dataRow, "voc_value"); //voc표시값
                water_diluent_code.Value = ComUtils.GetDataRowValue(dataRow, "water_diluent_code"); //voc희석제코드
                diluent_rate.Value = ComUtils.GetDataRowValue(dataRow, "diluent_rate"); //voc희석율
                car_type.Value = ComUtils.GetDataRowValue(dataRow, "car_type"); //차종
                kungwha_item_code.Value = ComUtils.GetDataRowValue(dataRow, "kungwha_item_code"); //경화제명
                type_code.Value = ComUtils.GetDataRowValue(dataRow, "type_code"); //종류
                used_type_code.Value = ComUtils.GetDataRowValue(dataRow, "used_type_code"); //용도
                dopo_dimension.Value = ComUtils.GetDataRowValue(dataRow, "dopo_dimension"); //도포면적
                label_type.Value = ComUtils.GetDataRowValue(dataRow, "label_type"); //라벨타입
                label_type_disp.Value = ComUtils.GetDataRowValue(dataRow, "label_type_disp"); //라벨타입명
                airback_exist_flag.Value = ComUtils.GetDataRowValue(dataRow, "airback_exist_flag"); //에어백유무
                available_period.Value = ComUtils.GetDataRowValue(dataRow, "available_period"); //유효기간
                actual_capacity.Value = ComUtils.GetDataRowValue(dataRow, "actual_capacity"); //실용량
                color_type.Value = ComUtils.GetDataRowValue(dataRow, "color_type"); //칼라타입
                remark.Value = ComUtils.GetDataRowValue(dataRow, "remark"); //추가사항
                customer_item_number.Value = ComUtils.GetDataRowValue(dataRow, "customer_item_number"); //거래처품번
                customer_factory.Value = ComUtils.GetDataRowValue(dataRow, "customer_factory"); //거래처공장
                packing_weight.Value = ComUtils.GetDataRowValue(dataRow, "packing_weight"); //포장중량
                can_weight.Value = ComUtils.GetDataRowValue(dataRow, "can_weight"); //용기중량
                color_code.Value = ComUtils.GetDataRowValue(dataRow, "color_code"); //컬러코드
                material_spec.Value = ComUtils.GetDataRowValue(dataRow, "material_spec"); //MATERIAL SPEC
                qr_color_code.Value = ComUtils.GetDataRowValue(dataRow, "qr_color_code"); //QR COLOR CODE
                qr_item_type.Value = ComUtils.GetDataRowValue(dataRow, "qr_item_type"); //QR공정구분
                gravity.Value = ComUtils.GetDataRowValue(dataRow, "gravity"); //비중
                toxic.Value = ComUtils.GetDataRowValue(dataRow, "toxic"); //유독물여부
                accident_contrast.Value = ComUtils.GetDataRowValue(dataRow, "accident_contrast"); //사고대비물질여부
                restricted.Value = ComUtils.GetDataRowValue(dataRow, "restricted"); //제한물질여부
                prohibited_handling.Value = ComUtils.GetDataRowValue(dataRow, "prohibited_handling"); //금지물질
                hallucinant_flag.Value = ComUtils.GetDataRowValue(dataRow, "hallucinant_flag"); //19세미만판매금지여부
                engineer_name.Value = ComUtils.GetDataRowValue(dataRow, "engineer_name"); //설계자
                un_number.Value = ComUtils.GetDataRowValue(dataRow, "un_number"); //유엔번호
                can_code.Value = ComUtils.GetDataRowValue(dataRow, "can_code"); //용기코드

                web_viscosity_sec.Value = ""; //점도(초)-----일회성입력
                web_viscosity_20c.Value = ""; //점도(20도)-----일회성입력
                web_undiluted_lot.Value = ""; //원액lot-----일회성입력
                web_nv.Value = ""; //NV-----일회성입력
                web_lot_output.Value = ""; //LOT생산량-----일회성입력
                web_coloring_heat.Value = ""; //착색열-----일회성입력
                web_grading.Value = ""; //입도-----일회성입력
                web_viscosity_standard.Value = ""; //점도기준-----일회성입력
                web_viscosity_real.Value = ""; //실점도-----일회성입력
                web_gloss.Value = ""; //광택-----일회성입력
                web_dft.Value = ""; //DFT-----일회성입력

                //picture.Value = ComUtils.GetDataRowValue(dataRow, "picture"); //그림
                divPictureList.Text = GetPictureHtml(ComUtils.GetDataRowValue(dataRow, "picture")); //그림

                signal.Value = (ComUtils.GetDataRowValue(dataRow, "signal").Split('$'))[0]; //신호어
                danger_expression.Value = ComUtils.GetDataRowValue(dataRow, "danger_expression").Replace("$", "\n"); //유해위험문구
                precaution_prevention.Value = ComUtils.GetDataRowValue(dataRow, "precaution_prevention").Replace("$", "\n"); //예방조치문구(예방)
                precaution_react.Value = ComUtils.GetDataRowValue(dataRow, "precaution_react").Replace("$", "\n"); //예방조치문구(대응)
                precaution_storage.Value = ComUtils.GetDataRowValue(dataRow, "precaution_storage").Replace("$", "\n"); //예방조치문구(저장)
                precaution_disuse.Value = ComUtils.GetDataRowValue(dataRow, "precaution_disuse").Replace("$", "\n"); //예방조치문구(폐기)

                //List 형태로 구성
                //mark_type.Value = ComUtils.GetDataRowValue(dataRow, "mark_type"); //인증종류
                //certification_no.Value = ComUtils.GetDataRowValue(dataRow, "certification_no"); //인증번호
                //certification_spec.Value = ComUtils.GetDataRowValue(dataRow, "certification_spec"); //인증규격
                divCertificationList.Text = GetCertificationHtml(ComUtils.GetDataRowValue(dataRow, "mark_type"), ComUtils.GetDataRowValue(dataRow, "certification_no"), ComUtils.GetDataRowValue(dataRow, "certification_spec"));
            }

            //GHS-MSDS 정보가 있으면 대체함.
            if (_ghsDataTable != null && _ghsDataTable.Rows.Count > 0)
            {
                divPictureList.Text = GetPictureHtml(GetGhsData(_ghsDataTable, "ghspic")); //그림
                signal.Value = (GetGhsData(_ghsDataTable, "ghssign").Split('$'))[0]; //신호어
                danger_expression.Value = GetGhsData(_ghsDataTable, "yhwh").Replace("$", "\n"); //유해위험문구
                precaution_prevention.Value = GetGhsData(_ghsDataTable, "ybyb").Replace("$", "\n"); //예방조치문구(예방)
                precaution_react.Value = GetGhsData(_ghsDataTable, "ybdu").Replace("$", "\n"); //예방조치문구(대응)
                precaution_storage.Value = GetGhsData(_ghsDataTable, "ybjj").Replace("$", "\n"); //예방조치문구(저장)
                precaution_disuse.Value = GetGhsData(_ghsDataTable, "ybpg").Replace("$", "\n"); //예방조치문구(폐기)
            }

        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }


    private String GetGhsData(DataTable table, string gubun)
    {
        String result = string.Empty;
        Boolean first = true;

        foreach (DataRow dataRow in table.Rows)
        {
            if (ComUtils.GetDataRowValue(dataRow, "GubunCd") == gubun)
            {
                if (first)
                {
                    result += ComUtils.GetDataRowValue(dataRow, "WordValue");
                    first = false;
                }
                else
                {
                    result += _token_string + ComUtils.GetDataRowValue(dataRow, "WordValue");
                }
            }
        }

        return result;
    }


    private String GetPictureHtml(String pic)
    {
        StringBuilder html = new StringBuilder(); ;
        html.AppendFormat("<li><p><img src='/images/GHS_느낌표.gif' /></p><input type='checkbox' name='picture' value='느낌표' {0} /><label>느낌표</label></li>", pic.IndexOf("느낌표") >= 0 ? "checked" : "");
        html.AppendFormat("<li><p><img src='/images/GHS_물고기.gif' /></p><input type='checkbox' name='picture' value='물고기' {0} /><label>물고기</label></li>", pic.IndexOf("물고기") >= 0 ? "checked" : "");
        html.AppendFormat("<li><p><img src='/images/GHS_부식.gif' /></p><input type='checkbox' name='picture' value='부식' {0} /><label>부식</label></li>", pic.IndexOf("부식") >= 0 ? "checked" : "");
        html.AppendFormat("<li><p><img src='/images/GHS_불꽃.gif' /></p><input type='checkbox' name='picture' value='불꽃' {0} /><label>불꽃</label></li>", pic.IndexOf("불꽃") >= 0 ? "checked" : "");
        html.AppendFormat("<li><p><img src='/images/GHS_사람.gif' /></p><input type='checkbox' name='picture' value='사람' {0} /><label>사람</label></li>", pic.IndexOf("사람") >= 0 ? "checked" : "");
        html.AppendFormat("<li><p><img src='/images/GHS_폭발.gif' /></p><input type='checkbox' name='picture' value='폭발' {0} /><label>폭발</label></li>", pic.IndexOf("폭발") >= 0 ? "checked" : "");
        html.AppendFormat("<li><p><img src='/images/GHS_해골.gif' /></p><input type='checkbox' name='picture' value='해골' {0} /><label>해골</label></li>", pic.IndexOf("해골") >= 0 ? "checked" : "");

        return html.ToString();
    }


    private String GetCertificationHtml(String mark_type, String certification_no, String certification_spec)
    {
        StringBuilder html = new StringBuilder(); ;

        string[] arr_mark_type = mark_type.Split('$');
        string[] arr_certification_no = certification_no.Split('$');
        string[] arr_certification_spec = certification_spec.Split('$');

        if (arr_mark_type != null && arr_mark_type.Length > 0)
        {
            for (int i = 0; i < arr_mark_type.Length; i++)
            {
                html.AppendFormat("<tr>");
                html.AppendFormat("<td class='align_c'>{0}</td>", i + 1);
                html.AppendFormat("<td><input type='text' name='mark_type' value='{0}' /></td>", arr_mark_type[i]);
                html.AppendFormat("<td><input type='text' name='certification_no' value='{0}' /></td>", arr_certification_no[i]);
                html.AppendFormat("<td><input type='text' name='certification_spec' value='{0}' /></td>", arr_certification_spec[i]);
                html.AppendFormat("</tr>");
            }
        }

        return html.ToString();
    }


    protected void nrl_labeltype_company_Changed(object sender, EventArgs e)
    {
        nrl_labeltype_a.Items.Clear();
        nrl_labeltype_b.Items.Clear();
        nrl_labeltype_c.Items.Clear();

        //라벨TYPE-국가
        nrl_labeltype_a.Items.Add(new ListItem("===국가===", ""));
        DataTable dt1 = GetLabelCountry(nrl_labeltype_company.SelectedValue);
        if (dt1 != null && dt1.Rows.Count > 0)
        {
            foreach (DataRow dataRow in dt1.Rows)
            {
                nrl_labeltype_a.Items.Add(new ListItem(ComUtils.GetDataRowValue(dataRow, "t_COUNTRYNAME"), ComUtils.GetDataRowValue(dataRow, "t_COUNTRYCODE")));
            }
        }
    }


    protected void nrl_labeltype_a_Changed(object sender, EventArgs e)
    {
        nrl_labeltype_b.Items.Clear();
        nrl_labeltype_c.Items.Clear();

        //라벨TYPE-거래처
        nrl_labeltype_b.Items.Add(new ListItem("===거래처===", ""));
        DataTable dt1 = GetLabelOrderCompany(nrl_labeltype_a.SelectedValue);
        if (dt1 != null && dt1.Rows.Count > 0)
        {
            foreach (DataRow dataRow in dt1.Rows)
            {
                nrl_labeltype_b.Items.Add(new ListItem(ComUtils.GetDataRowValue(dataRow, "t_ODERCOMPNAME"), ComUtils.GetDataRowValue(dataRow, "t_ODERCOMPCODE")));
            }
        }
    }


    protected void nrl_labeltype_b_Changed(object sender, EventArgs e)
    {
        nrl_labeltype_c.Items.Clear();

        //라벨TYPE-일련번호
        nrl_labeltype_c.Items.Add(new ListItem("===일련번호===", ""));
        DataTable dt1 = GetLabelDocument(nrl_labeltype_b.SelectedValue);
        if (dt1 != null && dt1.Rows.Count > 0)
        {
            foreach (DataRow dataRow in dt1.Rows)
            {
                nrl_labeltype_c.Items.Add(new ListItem(
                    ComUtils.GetDataRowValue(dataRow, "t_IDVIEW_DP") + " | " +
                    ComUtils.GetDataRowValue(dataRow, "ERPCOMPCODE") + "_" +
                    ComUtils.GetDataRowValue(dataRow, "t_WTDIVISIONCODE") + "_" +
                    ComUtils.GetDataRowValue(dataRow, "t_LABELDIVISIONCODE") + "_" +
                    ComUtils.GetDataRowValue(dataRow, "LABELSIZEVIEW") + "_" +
                    ComUtils.GetDataRowValue(dataRow, "LABELSERIALNO") + " | " +
                    ComUtils.GetDataRowValue(dataRow, "t_DESC")
                    , ComUtils.GetDataRowValue(dataRow, "t_IDVIEW")
                ));
            }
        }
    }


    protected void nrl_labeltype_c_Changed(object sender, EventArgs e)
    {
    }


    private void GetInitLabelCookies()
    {
        string cookie_gubun = GetCookie("search_gubun_save");
        string cookie_lang = GetCookie("search_lang_save");
        string cookie_factory = GetCookie("search_factory_save");

        string cookie_company = GetCookie("nrl_labeltype_company_save");
        string cookie_a = GetCookie("nrl_labeltype_a_save");
        string cookie_b = GetCookie("nrl_labeltype_b_save");
        string cookie_c = GetCookie("nrl_labeltype_c_save");

        //쿠키에서 마지막으로 조회/저장한 공장선택 값을 세팅해준다.
        search_gubun.SelectedValue = cookie_gubun;
        //아직 미공개 주석처리search_lang.SelectedValue = cookie_lang;
        search_factory.SelectedValue = cookie_factory;

        nrl_labeltype_company.SelectedValue = cookie_company;
        if (cookie_company != "")
        {
            nrl_labeltype_company_Changed(null, null);
        }

        nrl_labeltype_a.SelectedValue = cookie_a;
        if (cookie_a != "")
        {
            nrl_labeltype_a_Changed(null, null);
        }

        nrl_labeltype_b.SelectedValue = cookie_b;
        if (cookie_b != "")
        {
            nrl_labeltype_b_Changed(null, null);
        }

        nrl_labeltype_c.SelectedValue = cookie_c;
    }


    private String GetLabelColorTypeCode(string label_color_type_name)
    {
        string label_color_type_code = string.Empty;

        switch (label_color_type_name)
        {
            case "파랑":
                label_color_type_code = "0000ff";
                break;
            case "청색":
                label_color_type_code = "";
                break;
            case "녹색":
                label_color_type_code = "008000";
                break;
            case "띠_초록":
                label_color_type_code = "";
                break;
            case "연두":
                label_color_type_code = "";
                break;
            case "주황":
                label_color_type_code = "";
                break;
            case "띠_주황":
                label_color_type_code = "";
                break;
            case "빨강":
                label_color_type_code = "";
                break;
            case "띠_은색":
                label_color_type_code = "";
                break;
            case "흰색":
                label_color_type_code = "";
                break;
            case "띠_흰색":
                label_color_type_code = "";
                break;
            case "노랑":
                label_color_type_code = "FFFF00";
                break;
            case "띠_노랑":
                label_color_type_code = "";
                break;
            default:
                label_color_type_code = "";
                break;
        }

        return label_color_type_code;
    }

}
