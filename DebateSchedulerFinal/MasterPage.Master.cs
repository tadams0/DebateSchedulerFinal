﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateSchedulerFinal
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        public int PermissionLevel { get; private set; } = 0;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            //Setting up the server side backend for database related stuffs
            AppDomain.CurrentDomain.SetData("DataDirectory", DatabaseHandler.GetAppDataPath());
            User user = Help.GetUserSession(Session);
            if (user != null)
                FillLogout();
            
            if (!Page.IsPostBack)
                CheckPermissions(user);
        }

        public void SetPagePermissionLevel(int permissionLevel)
        {
            PermissionLevel = permissionLevel;
        }

        private MenuItem MakeAdminButton()
        {
            MenuItem but = new MenuItem();
            but.NavigateUrl = "~/AdminPanel.aspx";
            but.Text = "Admin Panel";
            but.Value = "A";
            return but;
        }

        private MenuItem MakeDebateCreatorButton()
        {
            MenuItem but = new MenuItem();
            but.NavigateUrl = "~/DebateCreator.aspx";
            but.Text = "Create Debate Season";
            but.Value = "D";
            return but;
        }

        private MenuItem MakeRefereeButton()
        {
            MenuItem but = new MenuItem();
            but.NavigateUrl = "~/Referee.aspx";
            but.Text = "Assign Scores";
            but.Value = "R";
            return but;
        }

        private MenuItem MakeUserPageButton()
        {
            MenuItem but = new MenuItem();
            but.NavigateUrl = "~/UserPage.aspx";
            but.Text = "User Page";
            but.Value = "U";
            return but;
        }

        public void RemoveButton(string val)
        {
            MenuItem adminBut = Menu1.FindItem(val);
            if (adminBut != null)
            {
                Menu1.Items.Remove(adminBut);
            }
        }

        private void CheckPermissions(User user)
        {
            RemoveButton("U");
            RemoveButton("A"); //While this is not effecient, it works.
            RemoveButton("D");
            RemoveButton("R");

            if (user != null)
            {
                Menu1.Items.Add(MakeUserPageButton());

                if (user.PermissionLevel >= 2)
                {
                    Menu1.Items.Add(MakeRefereeButton());
                }

                if (user.PermissionLevel >= 3)
                {
                    Menu1.Items.Add(MakeDebateCreatorButton());
                    Menu1.Items.Add(MakeAdminButton());
                }
            }

            //If the user is not logged in and the permission level of the page is greator than 1...
            //Or if the user is logged in but their permission level is less than the page's permission level..
            if ((user == null && PermissionLevel >= 1) || (user != null && user.PermissionLevel < PermissionLevel))
            {
                if (Request.Url.AbsolutePath.ToUpperInvariant() != ("/" + Help.defaultURL).ToUpperInvariant())
                    Response.Redirect(Help.defaultURL);
            }
            
            if (user != null && PermissionLevel == -1)
            {
                if (Request.Url.AbsolutePath.ToUpperInvariant() != ("/" + Help.defaultURL).ToUpperInvariant())
                    Response.Redirect(Help.defaultURL);
            }
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            User newUser = DatabaseHandler.AuthenticateUsernamePassword(Login1.UserName, Login1.Password);
            if (newUser != null) //If the new user is not null then the login did not fail.
            {
                Help.AddUserSession(Session, newUser);
                
                FillLogout();

                CheckPermissions(newUser);
            }
            else
            {
                //Error occured logging in..
            }
        }

        /// <summary>
        /// Fills out the logout panel with information pertaining to the logged in user.
        /// </summary>
        private void FillLogout()
        {
            User user = Help.GetUserSession(Session);
            if (user != null)
            {
                Login1.Visible = false;
                Panel_logout.Visible = true;
                HyperLink1.Text = user.Username;
                Label_Permissions.Text = Help.GetPermissionName(user.PermissionLevel);
            }
        }

        /// <summary>
        /// Refreshes the log out panel on the master page.
        /// </summary>
        public void RefreshLogout()
        {
            FillLogout();
        }

        protected void Button_Logout_Click(object sender, EventArgs e)
        {
            Help.EndSession(Session);
            Panel_logout.Visible = false;
            Login1.Visible = true;
            CheckPermissions(null);       
        }
    }
}