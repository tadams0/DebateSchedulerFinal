using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Threading;

namespace DebateSchedulerFinal
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        private NewsPost editingPost = null;
        private Color codeColor = Color.ForestGreen;

        private Color refereeColor = Color.DarkRed;
        private Color nonRefereeColor = Color.Black;

        private const int usersPerPage = 5000;
        private int currentUserPage = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(3);
            User loggedUser = Help.GetUserSession(Session);
            if (loggedUser != null && loggedUser.PermissionLevel >= 3)
            {
                List<string> activeCodes = DatabaseHandler.GetActiveCodes(Session);

                foreach (string s in activeCodes)
                {
                    Label codeLabel = new Label();
                    codeLabel.ForeColor = codeColor;
                    codeLabel.Text = s;
                    Button deactiveButton = new Button();
                    deactiveButton.Text = "Deactivate";
                    deactiveButton.CommandArgument = s;
                    deactiveButton.Command += DeactiveButton_Command;
                    Panel_ActiveCodes.Controls.Add(codeLabel);
                    Panel_ActiveCodes.Controls.Add(deactiveButton);
                    Panel_ActiveCodes.Controls.Add(new LiteralControl("<br />"));
                }

                List<User> users = DatabaseHandler.GetUsers(Session,currentUserPage, currentUserPage + usersPerPage + 1);
                int count = users.Count;
                if (users.Count > usersPerPage)
                    count -= 1;
                for (int i  = 0; i < count; i++)
                {
                    User u = users[i];
                    if (u.PermissionLevel <= 2) //We do not want to list super referees.
                    {
                        Label l = new Label();
                        l.Text = u.Username + "  "; //Two spaces for some padding against the button.
                        Button but = new Button();
                        bool isRevoke = false;
                        if (u.PermissionLevel >= 2)
                        {
                            l.ForeColor = refereeColor;
                            but.Text = "Revoke";
                            isRevoke = true;
                        }
                        else
                        {
                            l.ForeColor = nonRefereeColor;
                            but.Text = "Add";
                        }

                        but.CommandArgument = CreateAddRevokeArgument(u.ID, isRevoke);
                        but.Command += But_Command;

                        Panel_CreateReferee.Controls.Add(l);
                        Panel_CreateReferee.Controls.Add(but);
                        Panel_CreateReferee.Controls.Add(new LiteralControl("<br />"));
                    }
                }

                if (users.Count > currentUserPage + usersPerPage)
                {
                    Button nextButton = new Button();

                }
                

                //Slow loading this is a temporary solution
                //System.Threading.Thread.Sleep(1000);

                //Checking if data needs to be filled in...
                string editID = Request.QueryString["editID"];
                if (editID != null)
                {
                    int postID = int.Parse(editID);
                    NewsPost post = DatabaseHandler.GetNewsPost(postID);
                    if (post != null && post.Creator.ID == loggedUser.ID)
                    {
                        editingPost = post;
                        FreeTextBox1.Text = post.Data; //Server.HtmlDecode(post.Data);
                        FreeTextBox1.UpdateToolbar = true;
                    }
                }
            }
        }

        private void DeactiveButton_Command(object sender, CommandEventArgs e)
        {
            string code = e.CommandArgument as string;
            UserCodeError error;
            DatabaseHandler.DeactivateUserCode(code, out error);
            Response.Redirect(Request.RawUrl);
        }

        /// <summary>
        /// Creates an add revoke argument used for button presses.
        /// </summary>
        /// <param name="userID">The user's id to store.</param>
        /// <param name="isRevoke">Whether when the button is clicked referee status is revoke (if true) or added (if false).</param>
        /// <returns>Returns an argument string.</returns>
        private string CreateAddRevokeArgument(int userID, bool isRevoke)
        {
            int val = 0;
            if (isRevoke)
                val = 1;
            return userID + ":" + val;
        }

        /// <summary>
        /// Parses an add revoke argument used for button presses.
        /// </summary>
        /// <param name="value">The add revoke argument.</param>
        /// <param name="userID">The resultant user id stored in the argument.</param>
        /// <param name="isRevoke">The resultant boolean stored in the argument 
        /// which if true means the button should revoke referee status, if false it should add it.</param>
        private void ParseAddRevokeArgument(string value, out int userID, out bool isRevoke)
        {
            string[] result = value.Split(':');
            userID = int.Parse(result[0]);
            int val = int.Parse(result[1]);
            if (val == 1)
                isRevoke = true;
            else
                isRevoke = false;
        }

        private void But_Command(object sender, CommandEventArgs e)
        {
            int userID;
            bool revokeStatus;
            ParseAddRevokeArgument(e.CommandArgument as string, out userID, out revokeStatus);

            User user = DatabaseHandler.GetUser(Session, userID);
            if (revokeStatus)
            {
                user.PermissionLevel = 1;
            }
            else
            {
                user.PermissionLevel = 2;
            }

            DatabaseHandler.UpdateUser(Session, user);
            Response.Redirect(Request.RawUrl);
        }

        private void ShowNewsInfo(string message, Color color)
        {
            Label_NewsInfo.ForeColor = color;
            Label_NewsInfo.Text = message;
            Label_NewsInfo.Visible = true;
        }

        protected void Button_SubmitNews_Click(object sender, EventArgs e)
        {
            User creator = Help.GetUserSession(Session);
            if (creator != null)
            {
                NewsPost newPost = new NewsPost(0, creator, DateTime.Now, "", FreeTextBox1.Text);
                bool result = DatabaseHandler.AddNewsPost(Session, newPost);
                if (result)
                {
                    ShowNewsInfo("A new news post was created!", Color.Green);
                }
                else
                {
                    ShowNewsInfo("Error occured while trying to create a news post.", Color.Red);
                }
            }

        }

        protected void Button_RemoveNews_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null && editingPost != null && loggedUser.ID == editingPost.Creator.ID)
            { //If there is a logged in user, and a post being editted, and the person logged in created the post...
                bool result = DatabaseHandler.RemoveNewsPost(Session, editingPost.ID);
                if (result)
                {
                    ShowNewsInfo("News post has been permanently removed.", Color.Red);
                }
                else
                {
                    ShowNewsInfo("An error occured while removing the news post, it was not removed.", Color.Red);
                }
            }
            else
            {
                if (editingPost == null)
                {
                    ShowNewsInfo("This is not a post yet, press Submit New instead.", Color.Red);
                }
            }
        }

        protected void Button_UpdateNews_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null && editingPost != null && loggedUser.ID == editingPost.Creator.ID)
            { //If there is a logged in user, and a post being editted, and the person logged in created the post...
                editingPost.Data = FreeTextBox1.ViewStateText;
                bool result = DatabaseHandler.UpdateNewsPost(Session, editingPost);
                if (result)
                {
                    ShowNewsInfo("News post successfully updated!", Color.Green);
                }
                else
                {
                    ShowNewsInfo("Error occured while trying to update the news post.", Color.Red);
                }
            }
            else
            {
                if (editingPost == null)
                {
                    ShowNewsInfo("This is not a post yet, press Submit New instead.", Color.Red);
                }
            }
        }

        protected void Button_GenerateCode_Click(object sender, EventArgs e)
        {
            string code;
            bool result = DatabaseHandler.AddUserCode(Session, out code);
            if (result)
            {
                Label_CodeResult.Visible = true;
                Label_CodeResult.Text = "Your new code is: " + code;
                Label_CodeResult.ForeColor = Color.Green;
            }
            else
            {
                Label_CodeResult.Visible = true;
                Label_CodeResult.Text = "There has been an error generating a new code.";
                Label_CodeResult.ForeColor = Color.Red;
            }
        }

        protected void Button_ViewLog_Click(object sender, EventArgs e)
        {
            Button_ViewLog.Visible = false;

            string[] logLines = DatabaseHandler.GetLog(Session);

            Label_LogTitle.Text = "Logs as of " + DateTime.Now;

            if (logLines != null)
            {
                foreach (string s in logLines)
                {
                    Label l = new Label();
                    l.Text = s;
                    Panel_ViewLog.Controls.AddAt(2, l);
                    Panel_ViewLog.Controls.AddAt(2, new LiteralControl("<br /> <br />"));
                }
            }
        }
    }
}