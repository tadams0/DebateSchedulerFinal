<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="DebateCreator.aspx.cs" Inherits="DebateSchedulerFinal.DebateCreator" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style5 {
            width: 250px;
        }
        .auto-style7 {
            height: 246px;
        }
        </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="DebateCreatorStyle.css" type ="text/css" /> <!--This is the stylesheet which is used by the debate scheduling page.-->
    <br />
    <asp:Panel ID="Panel_Title" runat="server" HorizontalAlign="Center">
        <asp:Label ID="Label_Title" runat="server" Text="Schedule Generation" Font-Size="X-Large"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="Panel_OngoingSeason" runat="server" HorizontalAlign="Center" Visible="False">
        <asp:Label ID="Label3" runat="server" Text="There is already an ongoing debate season, you must end it before starting a new season. You can end the season after setting scores for every debate." Font-Size="Large"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="Panel_RecreateSeason" runat="server" HorizontalAlign="Center" Visible="False">
        <asp:Label ID="Label4" runat="server" Text="You can still change the debate season until a score has been set."></asp:Label>
    </asp:Panel>
    <br />
    
    <div id ="contentPanel" >
        <asp:Panel ID="Panel_Main" runat="server" HorizontalAlign="Center" BackColor="White" BorderStyle="Solid">
            <div id ="mainTable">
                            <table border="1" style="width:100%;">
                                <tr>
                                    <td>Season Parameters</td>
                                    <td>Season Teams</td>
                                </tr>
                                <tr>
                                    <td class="auto-style7">
                                        <div id="calendars0" class="auto-style5" style="margin-left:auto; margin-right:auto; ">
                                            <br />
                                            <asp:Label ID="Label_StartDate0" runat="server" Text="Start Date"></asp:Label>
                                            <asp:Calendar ID="Calendar_Start" runat="server" OnDayRender="Calendar_Start_DayRender" Width="244px"></asp:Calendar>
                                            <br />
                                            <asp:Label ID="Label_EndDate0" runat="server" Text="Season Length"></asp:Label>
                                            <br />
                                            <asp:DropDownList ID="DropDownList_Weeks" runat="server" Width="60px">
                                                <asp:ListItem Selected="True" Value="2"></asp:ListItem>
                                                <asp:ListItem Value="3"></asp:ListItem>
                                                <asp:ListItem Value="4"></asp:ListItem>
                                                <asp:ListItem Value="5"></asp:ListItem>
                                                <asp:ListItem Value="6"></asp:ListItem>
                                                <asp:ListItem Value="7"></asp:ListItem>
                                                <asp:ListItem Value="8"></asp:ListItem>
                                                <asp:ListItem Value="9"></asp:ListItem>
                                                <asp:ListItem Value="10"></asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;<asp:Label ID="Label2" runat="server" Text="Weeks"></asp:Label>
                                            <br />
                                        </div>
                                    </td>
                                    <td class="auto-style7">
                                        <asp:Panel ID="Panel_TeamGenerating" runat="server">
                                            <br />
                                            <asp:DropDownList ID="DropDownList_Teams" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList_Teams_SelectedIndexChanged" Width="100px">
                                                <asp:ListItem Value="2"></asp:ListItem>
                                                <asp:ListItem Value="3"></asp:ListItem>
                                                <asp:ListItem Value="4"></asp:ListItem>
                                                <asp:ListItem Value="5"></asp:ListItem>
                                                <asp:ListItem Value="6"></asp:ListItem>
                                                <asp:ListItem Value="7"></asp:ListItem>
                                                <asp:ListItem Value="8"></asp:ListItem>
                                                <asp:ListItem Value="9"></asp:ListItem>
                                                <asp:ListItem Value="10"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="Label_Teams0" runat="server" Text="Teams"></asp:Label>
                                            <br />
                                            <br />
                                        </asp:Panel>
                                        <asp:Panel ID="Panel_Teams" runat="server">
                                            <br />
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                </div>
            
            <asp:Label ID="Label_ScheduleError" runat="server" ForeColor="Red" Text="There are errors with the info given." Visible="False"></asp:Label>
            <br />
            
            <asp:Button ID="Button_CreateSchedule" runat="server" Text="Create Schedule" OnClick="Button_CreateSchedule_Click" Height="35px" />
                            <br />
            <br />
    </asp:Panel>
        <br />
    </div>
    
</asp:Content>
