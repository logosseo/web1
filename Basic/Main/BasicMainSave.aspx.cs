using System;
using System.Web.UI;
using System.Collections;
using System.Data.OracleClient;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using DIT.BizBase;

public partial class Basic_Main_BasicMainSave : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SetLabelCookies(ComUtils.GetDefaultString(Request["search_gubun"], ""), ComUtils.GetDefaultString(Request["search_lang"], ""), ComUtils.GetDefaultString(Request["search_factory"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_company"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_a"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_b"], ""), ComUtils.GetDefaultString(Request["nrl_labeltype_c"], ""));
                SaveData();
            }
        }
        catch (Exception ex)
        {
        }
    }


    private void SaveData()
    {
        Hashtable hash = new Hashtable();
        StringBuilder columnSql = new StringBuilder();

        //파라메터 처리
        makeQueryDataValue(columnSql, hash, "@company_code", ComUtils.GetDefaultString(Request["company_code"], ""));
        makeQueryDataValue(columnSql, hash, "@site_code", ComUtils.GetDefaultString(Request["site_code"], ""));
        makeQueryDataValue(columnSql, hash, "@item_number", ComUtils.GetDefaultString(Request["item_number"], ""));
        makeQueryDataValue(columnSql, hash, "@lot_number", ComUtils.GetDefaultString(Request["lot_number"], ""));
        makeQueryDataValue(columnSql, hash, "@prod_qty", ComUtils.GetDefaultString(Request["prod_qty"], ""));
        makeQueryDataValue(columnSql, hash, "@prod_qty_kg", ComUtils.GetDefaultString(Request["prod_qty_kg"], ""));
        makeQueryDataValue(columnSql, hash, "@so_qty", ComUtils.GetDefaultString(Request["so_qty"], ""));
        makeQueryDataValue(columnSql, hash, "@so_qty_kg", ComUtils.GetDefaultString(Request["so_qty_kg"], ""));
        makeQueryDataValue(columnSql, hash, "@mfg_date", ComUtils.GetDefaultString(Request["mfg_date"], ""));
        makeQueryDataValue(columnSql, hash, "@alternateitem", ComUtils.GetDefaultString(Request["alternateitem"], ""));
        makeQueryDataValue(columnSql, hash, "@salesrep_name", ComUtils.GetDefaultString(Request["salesrep_name"], ""));
        makeQueryDataValue(columnSql, hash, "@item_desc", ComUtils.GetDefaultString(Request["item_desc"], ""));
        makeQueryDataValue(columnSql, hash, "@item_desc2", ComUtils.GetDefaultString(Request["item_desc2"], ""));
        makeQueryDataValue(columnSql, hash, "@wip_item_desc", ComUtils.GetDefaultString(Request["wip_item_desc"], ""));
        makeQueryDataValue(columnSql, hash, "@mix_item_code", ComUtils.GetDefaultString(Request["mix_item_code"], ""));
        makeQueryDataValue(columnSql, hash, "@label_color_type_name", ComUtils.GetDefaultString(Request["label_color_type_name"], ""));
        makeQueryDataValue(columnSql, hash, "@label_color_type_code", ComUtils.GetDefaultString(Request["label_color_type_code"], ""));
        makeQueryDataValue(columnSql, hash, "@mix_rate", ComUtils.GetDefaultString(Request["mix_rate"], ""));
        makeQueryDataValue(columnSql, hash, "@msds_code", ComUtils.GetDefaultString(Request["msds_code"], ""));
        makeQueryDataValue(columnSql, hash, "@inwha_point", ComUtils.GetDefaultString(Request["inwha_point"], ""));
        makeQueryDataValue(columnSql, hash, "@risk_material", ComUtils.GetDefaultString(Request["risk_material"], ""));
        makeQueryDataValue(columnSql, hash, "@cautionclass", ComUtils.GetDefaultString(Request["cautionclass"], ""));
        makeQueryDataValue(columnSql, hash, "@chemical_name", ComUtils.GetDefaultString(Request["chemical_name"], ""));
        makeQueryDataValue(columnSql, hash, "@include_rate", ComUtils.GetDefaultString(Request["include_rate"], ""));
        makeQueryDataValue(columnSql, hash, "@regulatory_year", ComUtils.GetDefaultString(Request["regulatory_year"], ""));
        makeQueryDataValue(columnSql, hash, "@regulatory_lv_name", ComUtils.GetDefaultString(Request["regulatory_lv_name"], ""));
        makeQueryDataValue(columnSql, hash, "@regulatory_lv1_name", ComUtils.GetDefaultString(Request["regulatory_lv1_name"], ""));
        makeQueryDataValue(columnSql, hash, "@regulatory_lv2_name", ComUtils.GetDefaultString(Request["regulatory_lv2_name"], ""));
        makeQueryDataValue(columnSql, hash, "@regulatory_voc_value", ComUtils.GetDefaultString(Request["regulatory_voc_value"], ""));
        makeQueryDataValue(columnSql, hash, "@voc_value", ComUtils.GetDefaultString(Request["voc_value"], ""));
        makeQueryDataValue(columnSql, hash, "@water_diluent_code", ComUtils.GetDefaultString(Request["water_diluent_code"], ""));
        makeQueryDataValue(columnSql, hash, "@diluent_rate", ComUtils.GetDefaultString(Request["diluent_rate"], ""));
        makeQueryDataValue(columnSql, hash, "@car_type", ComUtils.GetDefaultString(Request["car_type"], ""));
        makeQueryDataValue(columnSql, hash, "@kungwha_item_code", ComUtils.GetDefaultString(Request["kungwha_item_code"], ""));
        makeQueryDataValue(columnSql, hash, "@type_code", ComUtils.GetDefaultString(Request["type_code"], ""));
        makeQueryDataValue(columnSql, hash, "@used_type_code", ComUtils.GetDefaultString(Request["used_type_code"], ""));
        makeQueryDataValue(columnSql, hash, "@dopo_dimension", ComUtils.GetDefaultString(Request["dopo_dimension"], ""));
        makeQueryDataValue(columnSql, hash, "@label_type", ComUtils.GetDefaultString(Request["label_type"], ""));
        makeQueryDataValue(columnSql, hash, "@label_type_disp", ComUtils.GetDefaultString(Request["label_type_disp"], ""));
        makeQueryDataValue(columnSql, hash, "@airback_exist_flag", ComUtils.GetDefaultString(Request["airback_exist_flag"], ""));
        makeQueryDataValue(columnSql, hash, "@available_period", ComUtils.GetDefaultString(Request["available_period"], ""));
        makeQueryDataValue(columnSql, hash, "@actual_capacity", ComUtils.GetDefaultString(Request["actual_capacity"], ""));
        makeQueryDataValue(columnSql, hash, "@color_type", ComUtils.GetDefaultString(Request["color_type"], ""));
        makeQueryDataValue(columnSql, hash, "@remark", ComUtils.GetDefaultString(Request["remark"], ""));
        makeQueryDataValue(columnSql, hash, "@customer_item_number", ComUtils.GetDefaultString(Request["customer_item_number"], ""));
        makeQueryDataValue(columnSql, hash, "@customer_factory", ComUtils.GetDefaultString(Request["customer_factory"], ""));
        makeQueryDataValue(columnSql, hash, "@packing_weight", ComUtils.GetDefaultString(Request["packing_weight"], ""));
        makeQueryDataValue(columnSql, hash, "@can_weight", ComUtils.GetDefaultString(Request["can_weight"], ""));
        makeQueryDataValue(columnSql, hash, "@color_code", ComUtils.GetDefaultString(Request["color_code"], ""));
        makeQueryDataValue(columnSql, hash, "@material_spec", ComUtils.GetDefaultString(Request["material_spec"], ""));
        makeQueryDataValue(columnSql, hash, "@qr_color_code", ComUtils.GetDefaultString(Request["qr_color_code"], ""));
        makeQueryDataValue(columnSql, hash, "@qr_item_type", ComUtils.GetDefaultString(Request["qr_item_type"], ""));
        makeQueryDataValue(columnSql, hash, "@gravity", ComUtils.GetDefaultString(Request["gravity"], ""));
        makeQueryDataValue(columnSql, hash, "@toxic", ComUtils.GetDefaultString(Request["toxic"], ""));
        makeQueryDataValue(columnSql, hash, "@accident_contrast", ComUtils.GetDefaultString(Request["accident_contrast"], ""));
        makeQueryDataValue(columnSql, hash, "@restricted", ComUtils.GetDefaultString(Request["restricted"], ""));
        makeQueryDataValue(columnSql, hash, "@prohibited_handling", ComUtils.GetDefaultString(Request["prohibited_handling"], ""));
        makeQueryDataValue(columnSql, hash, "@hallucinant_flag", ComUtils.GetDefaultString(Request["hallucinant_flag"], ""));
        makeQueryDataValue(columnSql, hash, "@engineer_name", ComUtils.GetDefaultString(Request["engineer_name"], ""));
        makeQueryDataValue(columnSql, hash, "@un_number", ComUtils.GetDefaultString(Request["un_number"], ""));
        makeQueryDataValue(columnSql, hash, "@can_code", ComUtils.GetDefaultString(Request["can_code"], ""));
        makeQueryDataValue(columnSql, hash, "@web_viscosity_sec", ComUtils.GetDefaultString(Request["web_viscosity_sec"], ""));
        makeQueryDataValue(columnSql, hash, "@web_viscosity_20c", ComUtils.GetDefaultString(Request["web_viscosity_20c"], ""));
        makeQueryDataValue(columnSql, hash, "@web_undiluted_lot", ComUtils.GetDefaultString(Request["web_undiluted_lot"], ""));
        makeQueryDataValue(columnSql, hash, "@web_nv", ComUtils.GetDefaultString(Request["web_nv"], ""));
        makeQueryDataValue(columnSql, hash, "@web_lot_output", ComUtils.GetDefaultString(Request["web_lot_output"], ""));
        makeQueryDataValue(columnSql, hash, "@web_coloring_heat", ComUtils.GetDefaultString(Request["web_coloring_heat"], ""));
        makeQueryDataValue(columnSql, hash, "@web_grading", ComUtils.GetDefaultString(Request["web_grading"], ""));
        makeQueryDataValue(columnSql, hash, "@web_gloss", ComUtils.GetDefaultString(Request["web_gloss"], ""));
        makeQueryDataValue(columnSql, hash, "@web_dft", ComUtils.GetDefaultString(Request["web_dft"], ""));
        makeQueryDataValue(columnSql, hash, "@web_viscosity_standard", ComUtils.GetDefaultString(Request["web_viscosity_standard"], ""));
        makeQueryDataValue(columnSql, hash, "@web_viscosity_real", ComUtils.GetDefaultString(Request["web_viscosity_real"], ""));
        makeQueryDataValue(columnSql, hash, "@picture", ComUtils.GetDefaultString(Request["picture"], ""));
        makeQueryDataValue(columnSql, hash, "@signal", ComUtils.GetDefaultString(Request["signal"], ""));
        makeQueryDataValue(columnSql, hash, "@danger_expression", ComUtils.GetDefaultString(Request["danger_expression"], ""));
        makeQueryDataValue(columnSql, hash, "@precaution_prevention", ComUtils.GetDefaultString(Request["precaution_prevention"], ""));
        makeQueryDataValue(columnSql, hash, "@precaution_react", ComUtils.GetDefaultString(Request["precaution_react"], ""));
        makeQueryDataValue(columnSql, hash, "@precaution_storage", ComUtils.GetDefaultString(Request["precaution_storage"], ""));
        makeQueryDataValue(columnSql, hash, "@precaution_disuse", ComUtils.GetDefaultString(Request["precaution_disuse"], ""));
        makeQueryDataValue(columnSql, hash, "@mark_type", ComUtils.GetDefaultString(Request["mark_type"], ""));
        makeQueryDataValue(columnSql, hash, "@certification_no", ComUtils.GetDefaultString(Request["certification_no"], ""));
        makeQueryDataValue(columnSql, hash, "@certification_spec", ComUtils.GetDefaultString(Request["certification_spec"], ""));
        makeQueryDataValue(columnSql, hash, "@nrl_labeltype_company", ComUtils.GetDefaultString(Request["nrl_labeltype_company"], ""));
        makeQueryDataValue(columnSql, hash, "@nrl_labeltype_a", ComUtils.GetDefaultString(Request["nrl_labeltype_a"], ""));
        makeQueryDataValue(columnSql, hash, "@nrl_labeltype_b", ComUtils.GetDefaultString(Request["nrl_labeltype_b"], ""));
        makeQueryDataValue(columnSql, hash, "@nrl_labeltype_c", ComUtils.GetDefaultString(Request["nrl_labeltype_c"], ""));
        makeQueryDataValue(columnSql, hash, "@print_yn", "N");
        //=====makeQueryDataValue(columnSql, hash, "@print_date", ""); //데이터 안넘김(인쇄할때 업데이트)
        makeQueryDataValue(columnSql, hash, "@regid", ComUtils.GetNotNullToString(Session["LOGIN_ID"]));
        makeQueryDataValue(columnSql, hash, "@regname", ComUtils.GetNotNullToString(Session["USER_NAME_KR"]));
        //=====makeQueryDataValue(columnSql, hash, "@regdate", ""); //데이터 안넘김(기본값이 getdate())
        makeQueryDataValue(columnSql, hash, "@q_gubun", ComUtils.GetDefaultString(Request["q_gubun"], ""));
        makeQueryDataValue(columnSql, hash, "@q_code", ComUtils.GetDefaultString(Request["q_code"], ""));
        makeQueryDataValue(columnSql, hash, "@q_lang", ComUtils.GetDefaultString(Request["q_lang"], ""));
        makeQueryDataValue(columnSql, hash, "@q_factory", ComUtils.GetDefaultString(Request["q_factory"], ""));


        //최종쿼리생성
        String query = string.Empty;
        query += "INSERT INTO GLMS_LABELINFO_TBL ( ";
        query += columnSql.ToString().Replace("@", "");
        query += ") VALUES ( ";
        query += columnSql.ToString();
        query += "); ";
        query += "SELECT SCOPE_IDENTITY() ";


        using (SqlConnection connection = new SqlConnection(ConfigurationSettings.AppSettings["DarwinPortal_ConnectionString"]))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach(DictionaryEntry pair in hash)
                {
                    command.Parameters.AddWithValue(pair.Key.ToString(), pair.Value);
                }

                //resultSeq.Value = "" + command.ExecuteNonQuery();
                resultSeq.Value = "" + command.ExecuteScalar();
            }
        }
    }


    private void makeQueryDataValue(StringBuilder columnSql, Hashtable hash, string columnHead, string columlValue)
    {
        if (columnSql.Length == 0)
        {
            columnSql.AppendFormat(columnHead);
        }
        else
        {
            columnSql.AppendFormat(", " + columnHead);
        }
        hash.Add(columnHead, columlValue);
    }

}
