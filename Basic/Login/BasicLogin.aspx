<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="BasicLogin.aspx.cs" Inherits="Basic_Login_BasicLogin" %>

<!doctype html>
<html lang="ko">
<head>
	<!--#include virtual="/include/head.asp"-->
<script>
    $(function () {
        //id가 있을 경우 기본 이미지를 지워주기 위함.
        if ($("#uid").val() != "") {
            $("#uid").focus();
            $("#upw").focus();
        }

        $("#loginBtn").on("click", function () {
            if ($("#uid").val() == "") {
                $("#uid").focus();
                return false;
            } else if ($("#upw").val() == "") {
                $("#upw").focus();
                return false;
            }

            <%= Page.GetPostBackEventReference(btnLogin) %>;
        });
    });


    function login() {
        $("#loginBtn").trigger("click");
    }
</script>
</head>

<body class="loginBody">
<%--
[테스트 로그인 계정]
DPI025506/sangmin1
137817953101/welcome
--%>
<form id="aform" method="post" runat="server">
	<div id="loginWrap">
		<h1 class="title"><strong>NOROO</strong>Label System</h1>
		<div class="loginBox">
			<h2 class="logo">(주)노루페인트</h2>
			<fieldset  class="login_form">
				<legend>로그인</legend>
				<div class="input_id login_spr">
					<input type="text" id="uid" runat="server" value="" class="input_text" style="ime-mode:inactive;" title="아이디" onkeypress="if (event.keyCode==13) login();" onkeyup="this.value = this.value.toUpperCase();" onfocus="this.className='input_text focus'" />
				</div>
				<div class="input_pw login_spr">
					<input type="password" id="upw" runat="server" value="" class="input_text" title="비밀번호" onkeypress="if (event.keyCode==13) login();" onfocus="this.className='input_text focus'" />
				</div>
				<div class="loign_btn">
					<a href="#" class="login_spr" id="loginBtn" >Login</a>
				</div>
			</fieldset >
			<div class="check_info"><input type="checkbox" id="uidSave" runat="server" value="Y" checked="checked" /><label for="checkbox1">아이디 저장</label></div>
			<div class="login_txt">노루그룹 라벨관리 시스템입니다.<br />로그인 이전에 아래의 출력 클라이언트를<br />다운로드 후 설치해 주십시오.(최초 1회)</div>
            <div class="clientBtn"><a href="/nrl_webservice/setup/glms_Setup.exe">출력 클라이언트 다운로드</a></div>
		</div>
	</div>


    <!-- ==== 히든필드 시작 ===== -->
    <div style="display:none">
        <asp:Button id="btnLogin" OnClick="btnLogin_OnClick" runat="server" style="display:none;"></asp:Button>
    </div>
    <!-- ==== 히든필드 종료 ===== -->
</form>
</body>
</html>
