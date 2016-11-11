<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Schedule.aspx.cs" Inherits="DebateSchedulerFinal.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <div id ="debateDisplay"> 
    <asp:Panel ID="Panel1" runat="server">
        <asp:Panel ID="Panel_Title" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label_Title" runat="server" Font-Size="X-Large" Text="Schedule"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="Panel_Searching" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label1" runat="server" Text="Search Options"></asp:Label>
            <br />
            <asp:Label ID="Label_TeamName" runat="server" Text="Team Name:"></asp:Label>
            <asp:DropDownList ID="DropDownList_TeamName" runat="server" Width="150px">
                <asp:ListItem Value="0">All</asp:ListItem>
            </asp:DropDownList>
            <br />
            <asp:Label ID="Label_Date" runat="server" Text="Date:"></asp:Label>
            <asp:DropDownList ID="DropDownList_Date" runat="server" Width="100px">
                <asp:ListItem Value="0">All</asp:ListItem>
            </asp:DropDownList>
            <br />
            <asp:Button ID="Button_Search" runat="server" OnClick="Button_Search_Click" Text="Search" />
        </asp:Panel>
        <br />
        <asp:Table ID="Table1" runat="server" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center">
        </asp:Table>
        
        <br />
    </asp:Panel>
    </div>

    <asp:Panel ID="Panel_NoDebate" runat="server" HorizontalAlign="Center" Visible="False">
        <asp:Label ID="Label_NoSchedule" runat="server" Font-Size="X-Large" Text="There is currently no ongoing debate season."></asp:Label>
        <br />
        </asp:Panel>
</asp:Content>
