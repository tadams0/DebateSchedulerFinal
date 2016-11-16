<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Referee.aspx.cs" Inherits="DebateSchedulerFinal.Referee"  MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    
    <div id ="debateDisplay"> 
        <asp:Panel ID="Panel_Title" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label1" runat="server" Font-Size="X-Large" Text="Assign Scores"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="Panel_Info" runat="server" HorizontalAlign="Center" Visible="False">
            <asp:Label ID="Label_Info" runat="server" Text="[Info]" Font-Size="Large"></asp:Label>
        </asp:Panel>
    <asp:Panel ID="Panel1" runat="server">
        <br />
            <asp:Table ID="Table1" runat="server" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center">
                </asp:Table>
        <br />
    </asp:Panel>
    </div>
    <asp:Panel ID="Panel_UpdateScores" runat="server" HorizontalAlign="Center">
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red" Height="20px" Text="Label" Visible="false"></asp:Label>
        <br />
        <asp:Button ID="UpdateButton" runat="server" Text="Update Scores"  Width="150px" Height="50px" OnClick="UpdateButton_Click"/>
        <br />
        <br />
    </asp:Panel>
</asp:Content>
