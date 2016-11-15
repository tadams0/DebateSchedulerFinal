<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="DebateSchedulerFinal.Archive" MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style2 {
            width: 274px;
        }
        .auto-style3 {
            width: 800px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    
    <asp:Panel ID="Panel_Title" runat="server" HorizontalAlign="Center">
        <asp:Label ID="Label_Title" runat="server" Text="Previous Debate Seasons" Font-Size="X-Large"></asp:Label>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel_NoSeason" runat="server" HorizontalAlign="Center" Visible="False">
        <asp:Label ID="Label_NoSeason" runat="server" Text="There is no previous season to view." Font-Size="Large"></asp:Label>
    </asp:Panel>
        <asp:Panel ID="Panel_Navigation" runat="server" Height="30px" HorizontalAlign="Center" BackColor="#CCCCCC">
            <table style="width:100%;" border="0">
                <tr>
                    <td class="auto-style2" style="width: 200px">
                        <asp:Button ID="Button_PreviousSeason" runat="server" Text="Previous Season" OnClick="Button_PreviousSeason_Click" />
                    </td>
                    <td class="auto-style3">
                        <asp:Label ID="Label_SeasonTitle" runat="server" Text="[Season Title]"></asp:Label>
                    </td>
                    <td style="width: 200px">
                        <asp:Button ID="Button_NextSeason" runat="server" Text="Next Season" OnClick="Button_NextSeason_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
    <asp:Panel ID="Panel_PagePanel" runat="server" HorizontalAlign="Center">
        <asp:Panel ID="Panel_RankingTitle" runat="server" HorizontalAlign="Center">
            <br />
            <asp:Label ID="Label_RankingsTitle" runat="server" Font-Size="X-Large" Text="Rankings"></asp:Label>
        </asp:Panel>
        <br />
        <asp:Table ID="Table_Rankings" runat="server" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center">
        </asp:Table>
        <br />
        <br />
        <br />
        <asp:Panel ID="Panel_Search" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label_ScheduleTitle" runat="server" Font-Size="X-Large" Text="Schedule"></asp:Label>
            <br />
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
        <asp:Panel ID="Panel_Main" runat="server">
        
        <br />
            <asp:Table ID="TableData" runat="server" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center">
            </asp:Table>
    </asp:Panel>
    
    <asp:Panel ID="Panel_NoSearch" runat="server" HorizontalAlign="Center" Visible="False">
        <asp:Label ID="Label_NoSearch" runat="server" Font-Size="X-Large" Text="No results match your search."></asp:Label>
    </asp:Panel>
    <br />
    <br />
    
    <br />
    
    </asp:Panel>
    <br />
</asp:Content>
