<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AdminPanel.aspx.cs" Inherits="DebateSchedulerFinal.AdminPanel" ValidateRequest="false" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="AdminPanelStyle.css" type ="text/css" /> <!--This is the stylesheet for the admin panel page.-->
    
    <div id ="NewsCreator">
        <asp:Panel ID="Panel_CreateNews" runat="server" BackColor="#CCCCCC" BorderStyle="Solid">

            <asp:Label ID="Label_NewsTitle" runat="server" Text="Create News"></asp:Label>
            <br />
            <FTB:FreeTextBox id="FreeTextBox1" runat="server" EnableHtmlMode="True" Width="790px" Height="500px" />
            <asp:Label ID="Label_NewsInfo" runat="server" Text="News Info" Visible="False"></asp:Label>
            <br />
            <asp:Button ID="Button_SubmitNews" runat="server" Text="Submit New" OnClick="Button_SubmitNews_Click" />
            <asp:Button ID="Button_UpdateNews" runat="server" OnClick="Button_UpdateNews_Click" Text="Update" />
            <asp:Button ID="Button_RemoveNews" runat="server" OnClick="Button_RemoveNews_Click" Text="Remove" />
        </asp:Panel>
    </div>
    <br />
     
    <br />
    <div id ="RefereeCreator">
         <asp:Panel ID="Panel_Codes" runat="server" BackColor="#CCCCCC" BorderStyle="Solid" HorizontalAlign="Center">
             <asp:Label ID="Label_RefereeCode" runat="server" Text="Referee Codes"></asp:Label>
             <br />
             <asp:Label ID="Label_CodeResult" runat="server" Text="Code Result" Visible="False"></asp:Label>
             <br />
             <asp:Button ID="Button_GenerateCode" runat="server" OnClick="Button_GenerateCode_Click" Text="Generate New Code" />
             <br />
             <asp:Label ID="Label1" runat="server" Text="Active Codes"></asp:Label>
             <asp:Panel ID="Panel_ActiveCodes" runat="server" HorizontalAlign="Center">
             </asp:Panel>
         </asp:Panel>
         <br />
        <br />
    
    <asp:Panel ID="Panel_CreateReferee" runat="server" BackColor="#CCCCCC" BorderStyle="Solid" HorizontalAlign="Center">
        <asp:Label ID="Label_CreateReferee" runat="server" Text="Add or Revoke Referee Status"></asp:Label>
        <br />
        <br />
    </asp:Panel>
         
         <br />
         
        </div>
    <div id ="LogViewer">
        <asp:Panel ID="Panel_ViewLog" runat="server" BackColor="#CCCCCC" BorderStyle="Solid">
            <asp:Panel ID="Panel_ViewLogTitle" runat="server" HorizontalAlign="Center">
                <asp:Label ID="Label_LogTitle" runat="server" Text="Log"></asp:Label>
                <br />
                <asp:Button ID="Button_ViewLog" runat="server" OnClick="Button_ViewLog_Click" Text="View Log" />
            </asp:Panel>
            <br />
        </asp:Panel>
    </div>
</asp:Content>
