using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateSchedulerFinal
{
    public partial class UserPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(1);
            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null)
            {

                if (loggedUser.PermissionLevel >= Help.GetPermissionLevel("Super Referee"))
                {
                    Panel_ChangeUsername.Visible = true;
                    Panel_DeleteAccount.Visible = false;
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
            

        }

        protected void Button_ChangeUsername_Click(object sender, EventArgs e)
        {
            string newUsername = TextBox_Username.Text;
            if (string.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 3)
            {
                Label_UsernameError.Visible = true;
                Label_UsernameError.Text = "Username must contain at least three letters, numbers, or symbols.";
                return;
            }

            if (newUsername.Length > 50)
            {
                Label_UsernameError.Visible = true;
                Label_UsernameError.Text = "Username must be 50 characters or fewer.";
                return;
            }

            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null)
            {

                User resultingUser = DatabaseHandler.AuthenticateUsernamePassword(loggedUser.Username, TextBox_PasswordUsername.Text);
                if (resultingUser != null)
                {
                    loggedUser.Username = newUsername;
                    bool result = DatabaseHandler.UpdateUser(Session, loggedUser);
                    if (result)
                    {
                        Help.EndSession(Session);
                        Help.AddUserSession(Session, loggedUser);
                        ((MasterPage)Master).RefreshLogout();
                        Label_UsernameError.Visible = true;
                        Label_UsernameError.ForeColor = System.Drawing.Color.Green;
                        Label_UsernameError.Text = "Username was successfully changed.";
                    }
                    else
                    {
                        Label_UsernameError.Visible = true;
                        Label_UsernameError.Text = "An unknown error occured while updating the database.";
                    }
                }
                else
                {
                    Label_UsernameError.Visible = true;
                    Label_UsernameError.Text = "Password did not match.";
                }
                
                
            }

        }

        protected void Button_ChangePassword_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null)
            {
                string oldPassword = ChangePassword.CurrentPassword;
                string newPassword = ChangePassword.NewPassword;

                User storedUser = DatabaseHandler.AuthenticateUsernamePassword(loggedUser.Username, oldPassword);
                if (storedUser == null)
                {
                    Label_PasswordError.Visible = true;
                    Label_PasswordError.Text = "Password is wrong.";
                    return;
                }

                bool result = DatabaseHandler.ChangeUserPassword(Session, loggedUser, oldPassword, newPassword, false); //We do not log password changes.
                if (result)
                {
                    Label_PasswordError.Visible = true;
                    Label_PasswordError.ForeColor = System.Drawing.Color.Green;
                    Label_PasswordError.Text = "Password was successfully changed!";
                }
                else
                {
                    Label_PasswordError.Visible = true;
                    Label_PasswordError.Text = "An unknown error occured while trying to change the password.";
                }

            }
        }

        protected void Button_DeleteAccount_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);
            if (loggedUser != null)
            {
                User resultUser = DatabaseHandler.AuthenticateUsernamePassword(loggedUser.Username, TextBox_DeleteAccount.Text);
                if (resultUser != null) //The resulting user was not deleted.
                {
                    Panel_Confirm.Visible = true;
                    Label_DeleteAccountError.Visible = true;
                    Label_DeleteAccountError.Text = "Are you sure you want to permanently removed your account?";
                }
                else
                {
                    Label_DeleteAccountError.Visible = true;
                    Label_DeleteAccountError.Text = "Password is invalid or does not match.";
                }
            }
        }

        protected void Button_NoDelete_Click(object sender, EventArgs e)
        {
            Panel_Confirm.Visible = false;
            Label_DeleteAccountError.Visible = false;
        }

        protected void Button_YesDelete_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);
            if (loggedUser != null)
            {
                bool result = DatabaseHandler.RemoveUser(Session, loggedUser.ID);
                if (result)
                {
                    Help.EndSession(Session);
                    ((MasterPage)Master).RefreshLogout();
                    Response.Redirect(Help.defaultURL);
                }
                else
                {
                    Label_DeleteAccountError.Visible = true;
                    Label_DeleteAccountError.Text = "An error occured while removing account.";
                }
            }
        }


    }
}