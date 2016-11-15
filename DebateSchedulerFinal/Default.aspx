<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DebateSchedulerFinal.News" MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="NewsStyle.css" type ="text/css" /> <!--This is the stylesheet which is used by the news page.-->

    <div id ="NewsArea" runat ="server">

        <asp:Panel ID="Panel_Title" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label_Title" runat="server" Font-Size="X-Large" Text="News"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="Panel_NoNews" runat="server" HorizontalAlign="Center" Visible="False">
            <asp:Label ID="Label_NoNews" runat="server" Text="There is currently no news to view." Font-Size="Large"></asp:Label>
        </asp:Panel>

        <br />

        <asp:Panel ID="Panel_News" runat="server">
        </asp:Panel>

    </div>
</asp:Content>
