<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserPage.aspx.cs" Inherits="DebateSchedulerFinal.UserPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style2 {
            width: 216px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="UserPageStyle.css" type ="text/css" /> <!--This is the stylesheet for the user page.-->
    &nbsp;&nbsp;&nbsp;<asp:Panel ID="Panel_Title" runat="server" HorizontalAlign="Center">
    <asp:Label ID="Label_Title" runat="server" Text="User Page" Font-Size="X-Large"></asp:Label>
</asp:Panel>
<br />
&nbsp;<div id ="mainPanel">
        <asp:Panel ID="Panel_Main" runat="server" HorizontalAlign="Center" BackColor="#CCCCCC" BorderStyle="Solid" DefaultButton="Button_ChangePassword">
            <asp:ChangePassword ID="ChangePassword" runat="server" Width="400px">
                <ChangePasswordTemplate>
                    <table cellpadding="1" cellspacing="0" style="border-collapse:collapse;">
                        <tr>
                            <td>
                                <table cellpadding="0" style="width:400px;">
                                    <tr>
                                        <td align="center" colspan="2">Change Your Password</td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Password:</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword" ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="ChangePassword" ForeColor="Red">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="NewPassword" runat="server" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword" ErrorMessage="New Password is required." ToolTip="New Password is required." ValidationGroup="ChangePassword" ForeColor="Red">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword" ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required." ValidationGroup="ChangePassword" ForeColor="Red">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry." ValidationGroup="ChangePassword" ForeColor="Red"></asp:CompareValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2" style="color:Red;">
                                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </ChangePasswordTemplate>
            </asp:ChangePassword>
            <asp:Button ID="Button_ChangePassword" runat="server" CommandName="ChangePassword" Text="Change Password" ValidationGroup="ChangePassword" OnClick="Button_ChangePassword_Click" />
            <br />
            <asp:Label ID="Label_PasswordError" runat="server" ForeColor="Red" Text="Error" Visible="False"></asp:Label>
            <br />
        </asp:Panel>
    </div>
&nbsp;&nbsp;&nbsp;
    <br />
    <div id ="usernamePanel">
    <asp:Panel ID="Panel_ChangeUsername" runat="server" BackColor="#CCCCCC" BorderStyle="Solid" DefaultButton="Button_ChangeUsername" HorizontalAlign="Center" Visible="False">
        <asp:Label ID="Label1" runat="server" Font-Size="Medium" Text="Change Your Username"></asp:Label>
        <br />
        <asp:Label ID="Label2" runat="server" Text="New Username:"></asp:Label>
        <asp:TextBox ID="TextBox_Username" runat="server"></asp:TextBox>
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label_Password" runat="server" Text="Password:"></asp:Label>
        <asp:TextBox ID="TextBox_PasswordUsername" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Button ID="Button_ChangeUsername" runat="server" OnClick="Button_ChangeUsername_Click" Text="Change Username" />
        <br />
        <asp:Label ID="Label_UsernameError" runat="server" ForeColor="Red" Text="Error" Visible="False"></asp:Label>
    </asp:Panel>
        </div>
<br />
</asp:Content>
