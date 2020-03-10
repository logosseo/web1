<%@ Page Title="" Language="C#" EnableEventValidation="true" AutoEventWireup="true" CodeFile="BasicMain.aspx.cs" Inherits="Basic_Main_BasicMain" %>

<!doctype html>
<html lang="ko">
<head>
	<!--#include virtual="/include/head.asp"-->
<script>
    $(function () {
        $("#btn_search").on("click", function () {
            <%= Page.GetPostBackEventReference(btnSearch) %>;
        });


        $("#search_code").on("keypress", function () {
            if (event.keyCode == 13) {
                $("#btn_search").trigger("click");
            }
        });


        $(".btnPrint").on("click", function () {
            var url = "BasicMainSave.aspx";
            $.ajax({
                type: "POST",
                url: url,
                data: $("form").serialize(),
                dataType: "html",
                async: false,
                success: function (data) {
                    //alert(data);
                    if ($(data).find("#resultSeq").val() != "") {
                        hiddenFrame.location.href = "ngls.stp://" + $(data).find("#resultSeq").val();
                    } else {
                        alert("저장오류");
                        //alert(data);
                    }
                }
            });

            return false;
        });


        $(".btnPreview").on("click", function () {
            alert("준비중입니다.");
            return false;
        });


        //NAC만 비중 소수점 2자리까지.
        $("#gravity").on("blur", function () {
            if ($("#search_factory").val() == "P21") {
                if (parseFloat(this.value)) {
                    //this.value = Math.round(parseFloat(this.value) * 100) / 100;
                    this.value = parseFloat(this.value).toFixed(2)
                } else {
                    this.value = "";
                }
            }
        }).trigger("blur");


        //NAC만 점도 소수점 2자리까지.
        $("#web_viscosity_20c").on("blur", function () {
            if ($("#search_factory").val() == "P21") {
                if (parseFloat(this.value)) {
                    //this.value = Math.round(parseFloat(this.value) * 100) / 100;
                    this.value = parseFloat(this.value).toFixed(2)
                } else {
                    this.value = "";
                }
            }
        }).trigger("blur");


        //NAC만 NV 소수점 1자리까지.
        $("#web_nv").on("blur", function () {
            if ($("#search_factory").val() == "P21") {
                if (parseFloat(this.value)) {
                    //this.value = Math.round(parseFloat(this.value) * 10) / 10;
                    this.value = parseFloat(this.value).toFixed(1)
                } else {
                    this.value = "";
                }
            }
        }).trigger("blur");


        //라벨색상 컬러표현
        $(".colorV span").css("background-color", "#" + $("#label_color_type_code").val());


        //LOT No, 원액LOT --> NAC만 대문자처리.
        $("#lot_number, #web_undiluted_lot").on("keyup", function () {
            if ($("#search_factory").val() == "P21") {
                this.value = this.value.toUpperCase();
            }
        })


/*====>ajax 사용안함. 주석처리
        $("#nrl_labeltype_company").on("change", function () {
            clearLabelType(this.name);

            if ($(this).val() != "") {
                var connectionname = "DarwinPortal_ConnectionString";
                var pXML = "NRL_Country";
                var aXML = "<param><name>nrl_labeltype_company</name><type>varchar</type><length>50</length><value><![CDATA[" + $(this).val() + "]]></value></param>";
                var sXML = "<Items><connectionname>" + connectionname + "</connectionname><xslpath></xslpath><sql><![CDATA[" + pXML + "]]></sql><type></type>" + aXML + "</Items>";

                var szURL = "/Basic/Main/BasicMainXMLQuery.aspx";
                $.post(szURL, sXML, function (data) {
                    receive_nrl_labeltype_company(data);
                }, "xml");
            }
        });


        $("#nrl_labeltype_a").on("change", function () {
            clearLabelType(this.name);

            if ($(this).val() != "") {
                var connectionname = "DarwinPortal_ConnectionString";
                var pXML = "NRL_OrderCompany";
                var aXML = "<param><name>nrl_labeltype_a</name><type>varchar</type><length>50</length><value><![CDATA[" + $(this).val() + "]]></value></param>";
                var sXML = "<Items><connectionname>" + connectionname + "</connectionname><xslpath></xslpath><sql><![CDATA[" + pXML + "]]></sql><type></type>" + aXML + "</Items>";

                var szURL = "/Basic/Main/BasicMainXMLQuery.aspx";
                $.post(szURL, sXML, function (data) {
                    receive_nrl_labeltype_a(data);
                }, "xml");
            }
        });


        $("#nrl_labeltype_b").on("change", function () {
            clearLabelType(this.name);

            if ($(this).val() != "") {
                var connectionname = "DarwinPortal_ConnectionString";
                var pXML = "NRL_LabelDocument";
                var aXML = "<param><name>nrl_labeltype_b</name><type>varchar</type><length>50</length><value><![CDATA[" + $(this).val() + "]]></value></param>";
                var sXML = "<Items><connectionname>" + connectionname + "</connectionname><xslpath></xslpath><sql><![CDATA[" + pXML + "]]></sql><type></type>" + aXML + "</Items>";

                var szURL = "/Basic/Main/BasicMainXMLQuery.aspx";
                $.post(szURL, sXML, function (data) {
                    receive_nrl_labeltype_b(data);
                }, "xml");
            }
        });
*/
    });


/*====>ajax 사용안함. 주석처리
    function clearLabelType(typeName) {
        if (typeName == "nrl_labeltype_company") {
            $("#nrl_labeltype_a option").remove();
            $("#nrl_labeltype_b option").remove();
            $("#nrl_labeltype_c option").remove();
        } else if (typeName == "nrl_labeltype_a") {
            $("#nrl_labeltype_b option").remove();
            $("#nrl_labeltype_c option").remove();
        } else if (typeName == "nrl_labeltype_b") {
            $("#nrl_labeltype_c option").remove();
        }
    }


    function receive_nrl_labeltype_company(dataresponseXML) {
        //alert(CFN_XmlToString(dataresponseXML).toString());
        //console.log(CFN_XmlToString(dataresponseXML).toString());
        var rows = $(dataresponseXML).find("Table");
        if (rows.length == 0) {
            alert("조회된 데이터가 없습니다.");
            //console.log("조회된 데이터가 없습니다.(receive_loadData1)");

        } else {
            $("#nrl_labeltype_a").append("<option value=''>===국가===</option>");
            rows.each(function (i, elm) {
                $("#nrl_labeltype_a").append("<option value='" + $(elm).find("t_COUNTRYCODE").text() + "'>" + $(elm).find("t_COUNTRYNAME").text() + "</option>");
            });
        }
    }


    function receive_nrl_labeltype_a(dataresponseXML) {
        //alert(CFN_XmlToString(dataresponseXML).toString());
        //console.log(CFN_XmlToString(dataresponseXML).toString());
        var rows = $(dataresponseXML).find("Table");
        if (rows.length == 0) {
            alert("조회된 데이터가 없습니다.");
            //console.log("조회된 데이터가 없습니다.(receive_loadData1)");

        } else {
            $("#nrl_labeltype_b").append("<option value=''>===거래처===</option>");
            rows.each(function (i, elm) {
                $("#nrl_labeltype_b").append("<option value='" + $(elm).find("t_ODERCOMPCODE").text() + "'>" + $(elm).find("t_ODERCOMPNAME").text() + "</option>");
            });
        }
    }


    function receive_nrl_labeltype_b(dataresponseXML) {
        //alert(CFN_XmlToString(dataresponseXML).toString());
        //console.log(CFN_XmlToString(dataresponseXML).toString());
        var rows = $(dataresponseXML).find("Table");
        if (rows.length == 0) {
            alert("조회된 데이터가 없습니다.");
            //console.log("조회된 데이터가 없습니다.(receive_loadData1)");

        } else {
            $("#nrl_labeltype_c").append("<option value=''>===일련번호===</option>");
            rows.each(function (i, elm) {
                $("#nrl_labeltype_c").append("<option value='" + $(elm).find("t_IDVIEW").text() + "'>"
                    + $(elm).find("t_IDVIEW_DP").text() + " | "
                    + $(elm).find("ERPCOMPCODE").text() + "_"
                    + $(elm).find("t_WTDIVISIONCODE").text() + "_"
                    + $(elm).find("t_LABELDIVISIONCODE").text() + "_"
                    + $(elm).find("LABELSIZEVIEW").text() + "_"
                    + $(elm).find("LABELSERIALNO").text() + " | "
                    + $(elm).find("t_DESC").text()
                    + "</option>");
            });
        }
    }


    var g_XMLSerializer; // g_XMLSerializer 전역객체
    // Xml을 문자열로 반환
    function CFN_XmlToString(pXml) {
        if (pXml.xml) { // IE 에서
            return pXml.xml;
        } else {
            if (g_XMLSerializer != null || g_XMLSerializer == undefined) {
                g_XMLSerializer = new XMLSerializer();
            }
            return g_XMLSerializer.serializeToString(pXml);
        }
    }
*/
</script>
</head>

<body>
<%--
-임시:포승,품목코드,PUE007101/4L<br />
-임시:안양,품목코드,PAA007321/18L<br />
-임시:화성,작지번호,7161752<br />
--%>
<form id="aform" method="post" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<div id="wrap">
		<div id="header">
			<div class="inner">
				<div class="idSearch">
                    <asp:DropDownList id="search_gubun" CssClass="idSlt" runat="server"></asp:DropDownList>
					<input type="text" id="search_code" class="idInp" runat="server" onkeyup="this.value = this.value.toUpperCase();" />
                    <a href="#" id="btn_search" class="btn"></a>
				</div>
                <div class="langWrap">
                    언어 선택
                    <asp:DropDownList id="search_lang" CssClass="langSlt" runat="server"></asp:DropDownList>
                </div>
				<div class="selectFactory">
					<strong>공장 선택</strong>
                    <asp:DropDownList id="search_factory" runat="server"></asp:DropDownList>
				</div>
			</div>
		</div>
		<div id="contents">
			<div class="boxA">
				<table summary="" cellpadding="0" cellspacing="0" class="tableA">
					<caption></caption>
					<colgroup>
						<col width="16%" />
						<col width="32%" />
						<col width="16%" />
						<col width="*" />
					</colgroup>
					<tbody>
						<tr>
							<th scope="col">품목코드</th>
							<td><input type="text" id="item_number" runat="server" /></td>
							<th scope="col">LOT No.</th>
							<td><input type="text" id="lot_number" runat="server" /></td>
						</tr>
                        <tr>
                            <th scope="col">제조일자</th>
							<td><input type="text" id="mfg_date" runat="server" /></td>
							<th scope="col">대응코드</th>
							<td><input type="text" id="alternateitem" runat="server" /></td>
                        </tr>
						<tr>
							<th scope="col">품명1</th>
							<td><input type="text" id="item_desc" runat="server" /></td>
							<th scope="col">품명2</th>
							<td><input type="text" id="item_desc2" runat="server" /></td>
						</tr>
                        <tr>
							<th scope="col">배합품명</th>
							<td><input type="text" id="wip_item_desc" runat="server" /></td>
              <th scope="col">인쇄여부</th>
							<td><input type="text" /></td>
						</tr>
					</tbody>
				</table>
				<table summary="" cellpadding="0" cellspacing="0" class="tableA marT01">
					<caption></caption>
					<colgroup>
						<col width="16%" />
						<col width="16%" />
						<col width="16%" />
						<col width="12%" />
            <col width="8%" />
						<col width="16%" />
						<col width="*" />
					</colgroup>
					<tbody>
						<tr>
                            <th scope="col">사용희석제</th>
							<td><input type="text" id="mix_item_code" runat="server" /></td>
							<th scope="col">라벨색상</th>
							<td><input type="text" id="label_color_type_name" runat="server" /></td>
                            <td class="colorV"><span style="background-color:#fff"></span></td>
							<th scope="col">혼합비</th>
							<td><input type="text" id="mix_rate" runat="server" /></td>
						</tr>
					</tbody>
				</table>
			</div>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="boxB02 marT10">
                        <span>라벨 TYPE</span>
                        <ul>
                            <li><asp:DropDownList id="nrl_labeltype_company" runat="server" AutoPostBack="true" OnSelectedIndexChanged="nrl_labeltype_company_Changed"></asp:DropDownList></li>
                            <li><asp:DropDownList id="nrl_labeltype_a" runat="server" AutoPostBack="true" OnSelectedIndexChanged="nrl_labeltype_a_Changed"></asp:DropDownList></li>
                            <li><asp:DropDownList id="nrl_labeltype_b" runat="server" AutoPostBack="true" OnSelectedIndexChanged="nrl_labeltype_b_Changed"></asp:DropDownList></li>
                            <li><asp:DropDownList id="nrl_labeltype_c" runat="server" AutoPostBack="true" OnSelectedIndexChanged="nrl_labeltype_c_Changed"></asp:DropDownList></li>
                        </ul>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="boxA marT10">
      				<table summary="" cellpadding="0" cellspacing="0" class="tableB tableB03">
      					<caption></caption>
      					<colgroup>
      						<col width="10%" />
      						<col width="16%" />
      						<col width="16%" />
      						<col width="*" />
      					</colgroup>
      					<thead>
      						<tr>
      							<th scope="col" class="t">순번</th>
      							<th scope="col">표준(인증)종류</th>
      							<th scope="col">인증번호</th>
      							<th scope="col">인증규격</th>
      						</tr>
      					</thead>
      					<tbody>
                              <asp:Label ID="divCertificationList" runat="server" />
      <%--
      						<tr>
      							<td class="align_c">1</td>
      							<td></td>
      							<td></td>
      							<td></td>
      						</tr>
      --%>
      					</tbody>
      				</table>
      			</div>
            <div class="btnWrap marT20">
                <a href="#" class="btnPrint">출력하기<em></em></a>
                <a href="#" class="btnPreview">미리보기<em></em></a>
            </div>
			<div class="boxF marT10">
				<div class="leftB">
					<table summary="" cellpadding="0" cellspacing="0" class="tableB">
						<caption></caption>
						<colgroup>
							<col width="20%" />
							<col width="30%" />
							<col width="20%" />
							<col width="30%" />
						</colgroup>
						<tbody>
							<tr>
								<th scope="col">위험물품명</th>
								<td><input type="text" id="risk_material" runat="server" /></td>
								<th scope="col">위험등급</th>
								<td><input type="text" id="cautionclass" runat="server" /></td>
							</tr>
							<tr>
								<th scope="col">위험물질명</th>
								<td><input type="text" id="chemical_name" runat="server" /></td>
								<th scope="col">위험물함량</th>
								<td><input type="text" id="include_rate" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="row">VOC적용기준</th>
								<td colspan="3"><input type="text" id="regulatory_lv_name" runat="server" /></td>
							</tr>
						</tbody>
					</table>
                    <table summary="" cellpadding="0" cellspacing="0" class="tableBL">
						<caption></caption>
						<colgroup>
							<col width="20%" />
							<col width="14%" />
							<col width="20%" />
							<col width="13%" />
                            <col width="20%" />
							<col width="13%" />
						</colgroup>
						<tbody>
                            <tr>
								<th scope="col">VOC년도</th>
								<td><input type="text" id="regulatory_year" runat="server" /></td>
								<th scope="col">VOC기준값</th>
								<td><input type="text" id="regulatory_voc_value" runat="server" /></td>
								<th scope="col">VOC표시값</th>
								<td><input type="text" id="voc_value" runat="server" /></td>
							</tr>
						</tbody>
					</table>
                    <table summary="" cellpadding="0" cellspacing="0" class="tableBL">
                        <caption></caption>
                        <colgroup>
							<col width="20%" />
							<col width="80%" />
						</colgroup>
                        <tbody>
                            <tr>
                                <th scope="col">VOC희석율</th>
                                <td><input type="text" id="diluent_rate" runat="server" /></td>
                            </tr>
                        </tbody>
                    </table>
					<table summary="" cellpadding="0" cellspacing="0" class="tableB tableB02">
						<caption></caption>
						<colgroup>
							<col width="20%" />
							<col width="30%" />
							<col width="20%" />
							<col width="30%" />
						</colgroup>
						<tbody>
							<tr>
								<th scope="row">차종</th>
								<td colspan="3"><input type="text" id="car_type" runat="server" /></td>
                            </tr>
                            <tr>
								<th scope="row">경화제명</th>
								<td colspan="3"><input type="text" id="kungwha_item_code" runat="server" /></td>
							</tr>
							<tr>
								<th scope="row">종류</th>
								<td colspan="3"><input type="text" id="type_code" runat="server" /></td>
                            </tr>
                            <tr>
								<th scope="row">용도</th>
								<td colspan="3"><input type="text" id="used_type_code" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="row">도포면적</th>
								<td colspan="3"><input type="text" id="dopo_dimension" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="row">라벨타입명</th>
								<td colspan="3"><input type="text" id="label_type_disp" runat="server" /></td>
							</tr>
							<tr>
								<th scope="col">에어백유무</th>
								<td><input type="text" id="airback_exist_flag" runat="server" /></td>
                                <th scope="col">유효기간</th>
                                <td><input type="text" id="available_period" runat="server" /></td>
							</tr>
							<tr>
                                <th scope="col">실용량</th>
								<td><input type="text" id="actual_capacity" runat="server" /></td>
								<th scope="col">칼라타입</th>
								<td><input type="text" id="color_type" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="row">추가사항</th>
								<td colspan="3"><input type="text" id="remark" runat="server" /></td>
							</tr>
                            <tr>
                                <th scope="col">생산수량</th>
								<td><input type="text" id="prod_qty" runat="server" /></td>
                                <th scope="col">생산수량(KG)</th>
								<td><input type="text" id="prod_qty_kg" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="col">판매자</th>
								<td colspan="3"><input type="text" id="salesrep_name" runat="server" /></td>
							</tr>
                        </tbody>
                    </table>
                    <table summary="" cellpadding="0" cellspacing="0" class="tableB tableB02">
						<caption></caption>
						<colgroup>
							<col width="20%" />
							<col width="30%" />
							<col width="20%" />
							<col width="30%" />
						</colgroup>
						<tbody>
							<tr>
								<th scope="col">거래처품번</th>
								<td><input type="text" id="customer_item_number" runat="server" /></td>
								<th scope="col">거래처공장</th>
								<td><input type="text" id="customer_factory" runat="server" /></td>
							</tr>
							<tr>
								<th scope="col">포장중량</th>
								<td><input type="text" id="packing_weight" runat="server" /></td>
								<th scope="col">용기중량</th>
								<td><input type="text" id="can_weight" runat="server" /></td>
							</tr>
							<tr>
								<th scope="col">컬러코드</th>
								<td><input type="text" id="color_code" runat="server" /></td>
                                <th scope="col">비중</th>
								<td><input type="text" id="gravity" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="col">MATERIAL SPEC</th>
								<td><input type="text" id="material_spec" runat="server" /></td>
                                <th scope="col">QR COLOR CODE</th>
								<td><input type="text" id="qr_color_code" runat="server" /></td>
							</tr>
                            <tr>
                                <th scope="col">QR공정구분</th>
                                <td colspan="3"><input type="text" id="qr_item_type" runat="server" /></td>
                            </tr>
                            <tr style="display:none;"><%--현업요청으로 안보이게만 처리함(DB에 값은 저장해야함)--%>
								<th scope="col">유독물여부</th>
								<td><input type="text" id="toxic" runat="server" /></td>
                                <th scope="col">사고대비<br />물질여부</th>
								<td><input type="text" id="accident_contrast" runat="server" /></td>
							</tr>
                            <tr style="display:none;"><%--현업요청으로 안보이게만 처리함(DB에 값은 저장해야함)--%>
								<th scope="col">제한물질여부</th>
								<td><input type="text" id="restricted" runat="server" /></td>
                                <th scope="col">금지물질</th>
								<td><input type="text" id="prohibited_handling" runat="server" /></td>
							</tr>
						</tbody>
					</table>
          <p class="s_t marT10">일회성 Data</p>
					<table summary="" cellpadding="0" cellspacing="0" class="tableB">
						<caption></caption>
						<colgroup>
							<col width="20%" />
							<col width="30%" />
							<col width="20%" />
							<col width="30%" />
						</colgroup>
						<tbody>
                            <tr>
								<th scope="col">점도기준</th>
								<td><input type="text" id="web_viscosity_standard" runat="server" /></td>
								<th scope="col">실점도</th>
								<td><input type="text" id="web_viscosity_real" runat="server" /></td>
							</tr>
							<tr>
								<th scope="col">점도(초/20℃)</th>
								<td><input type="text" id="web_viscosity_20c" runat="server" /></td>
                                <th scope="col">NV</th>
                                <td><input type="text" id="web_nv" runat="server" /></td>
							</tr>
							<tr>
								<th scope="row">원액LOT</th>
								<td><input type="text" id="web_undiluted_lot" runat="server" /></td>
                                <th scope="col">LOT생산량</th>
                                <td><input type="text" id="web_lot_output" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="col">착색열</th>
								<td><input type="text" id="web_coloring_heat" runat="server" /></td>
								<th scope="col">입도</th>
								<td><input type="text" id="web_grading" runat="server" /></td>
							</tr>
                            <tr>
								<th scope="col">광택</th>
								<td><input type="text" id="web_gloss" runat="server" /></td>
								<th scope="col">D.F.T</th>
								<td><input type="text" id="web_dft" runat="server" /></td>
							</tr>
						</tbody>
					</table>
				</div>
				<div class="rightB">
                    <table summary="" cellpadding="0" cellspacing="0" class="tableB">
						<caption></caption>
						<colgroup>
							<col width="10%" />
                            <col width="10%" />
							<col width="80%" />
						</colgroup>
						<tbody>
							<tr>
								<th scope="row" colspan="2">그림문자</th>
								<td>
									<div class="imgText">
										<ul>
                                            <asp:Label ID="divPictureList" runat="server" />
<%--
											<li><p><img src="/images/GHS_느낌표.gif" /></p><input type="checkbox"><label>느낌표</label></li>
											<li><p><img src="/images/GHS_물고기.gif" /></p><input type="checkbox"><label>물고기</label></li>
											<li><p><img src="/images/GHS_부식.gif" /></p><input type="checkbox"><label>부식</label></li>
											<li><p><img src="/images/GHS_불꽃.gif" /></p><input type="checkbox"><label>불꽃</label></li>
											<li><p><img src="/images/GHS_사람.gif" /></p><input type="checkbox"><label>사람</label></li>
											<li><p><img src="/images/GHS_폭발.gif" /></p><input type="checkbox"><label>폭발</label></li>
											<li><p><img src="/images/GHS_해골.gif" /></p><input type="checkbox"><label>해골</label></li>
--%>
										</ul>
									</div>
								</td>
							</tr>
                            <tr>
								<th scope="row" colspan="2">신호어</th>
								<td><input type="text" id="signal" runat="server" /></td>
							</tr>
							<tr>
								<th scope="row" colspan="2">유해위험문구</th>
								<td class="textA01"><textarea id="danger_expression" runat="server"></textarea></td>
							</tr>
							<tr>
								<th scope="col" rowspan="4">예방조치문구</th>
                                <th scope="col">예방</th>
								<td class="textA02"><textarea id="precaution_prevention" runat="server"></textarea></td>
							</tr>
                            <tr>
								<th scope="row">대응</th>
								<td class="textA02"><textarea id="precaution_react" runat="server"></textarea></td>
							</tr>
                            <tr>
								<th scope="row">저장</th>
								<td class="textA02"><textarea id="precaution_storage" runat="server"></textarea></td>
							</tr>
                            <tr>
								<th scope="row">폐기</th>
								<td class="textA02"><textarea id="precaution_disuse" runat="server"></textarea></td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>

		</div>
		<div class="btnWrap">
			<a href="#" class="btnPrint">출력하기<em></em></a>
            <a href="#" class="btnPreview">미리보기<em></em></a>
		</div>
		<div id="footer">
			<!--#include virtual="/include/footer.asp"-->
		</div>
	</div>


    <!-- ==== 히든필드 시작 ===== -->
    <div style="display:none">
        <iframe id="hiddenFrame" name="hiddenFrame" src="about:blank" style="display:none;"></iframe>
        <asp:Button id="btnSearch" OnClick="btnSearch_OnClick" runat="server" style="display:none;"></asp:Button><!-- 조회를 위한 버튼 -->
        조회조건(구분)<input type="text" id="q_gubun" runat="server" /><!--hidden-->
        조회조건(코드)<input type="text" id="q_code" runat="server" /><!--hidden-->
        조회조건(언어)<input type="text" id="q_lang" runat="server" /><!--hidden-->
        조회조건(공장)<input type="text" id="q_factory" runat="server" /><!--hidden-->

        회사<input type="text" id="company_code" runat="server" /><!--hidden-->
        사이트<input type="text" id="site_code" runat="server" /><!--hidden-->
        msds코드<input type="text" id="msds_code" runat="server" /><!--hidden-->
        인화점<input type="text" id="inwha_point" runat="server" /><!--hidden-->
        VOC희석제<input type="text" id="water_diluent_code" runat="server" /><!--hidden-->

        <%--일회성입력(화면에서는 삭제됨)--%>
        점도(초)<input type="text" id="web_viscosity_sec" runat="server" /><!--hidden-->

        <%--2019-12-19; 추가항목--%>
        19세미만판매금지여부<input type="text" id="hallucinant_flag" runat="server" /><!--hidden-->
        설계자<input type="text" id="engineer_name" runat="server" /><!--hidden-->
        유엔번호<input type="text" id="un_number" runat="server" /><!--hidden-->
        용기코드<input type="text" id="can_code" runat="server" /><!--hidden-->

        <%--2020-01-03; 추가항목--%>
        <input type="text" id="label_type" runat="server" /><!--hidden-->

        <%--2020-01-31; 추가항목--%>
        <input type="text" id="so_qty" runat="server" /><!--주문수량--><!--hidden-->
        <input type="text" id="so_qty_kg" runat="server" /><!--주문수량(KG)--><!--hidden-->
        <input type="text" id="label_color_type_code" runat="server" /><!--라벨타입코드--><!--hidden-->
    </div>
    <!-- ==== 히든필드 종료 ===== -->
</form>
</body>
</html>
