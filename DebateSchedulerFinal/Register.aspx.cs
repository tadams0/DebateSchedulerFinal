﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DebateSchedulerFinal
{
    public partial class Register : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(-1);
            User user = Help.GetUserSession(Session);
            if (user != null)
            {
                CreateUserWizard.Visible = false;
                Label_Title.Visible = false;
                Label_Code.Visible = false;
                TextBox1.Visible = false;
            }
            Label_UserCreated.Visible = false;
            Label_ErrorMessage.Visible = false;
        }
        
        protected void CreateUserWizard_CreatingUser(object sender, LoginCancelEventArgs e)
        {

            e.Cancel = true;

            if (CreateUserWizard.UserName.Contains('%'))
            {
                Label_ErrorMessage.Text = "Username cannot contain %";
                Label_ErrorMessage.Visible = true;
            }
            else
            {
                //Email verification:
                if (Help.IsValidEmail(CreateUserWizard.Email))
                {
                    string ipAddress = Request.UserHostAddress;
                    
                    string code = TextBox1.Text.Trim();
                    UserCodeError codeError;
                    bool result = DatabaseHandler.DeactivateUserCode(CreateUserWizard.UserName, code, out codeError);
                    if (result)
                    {
                        bool addUserResult = DatabaseHandler.AddUser(ipAddress, new User(2, CreateUserWizard.UserName, CreateUserWizard.Email, CreateUserWizard.Question, 0), CreateUserWizard.Password, CreateUserWizard.Answer);
                        if (addUserResult)
                        {
                            Label_UserCreated.Visible = true;
                            CreateUserWizard.Visible = false;
                            Label_ErrorMessage.Visible = false;
                            Label_Title.Visible = false;
                            Label_Code.Visible = false;
                            TextBox1.Visible = false;
                        }
                        else
                        {
                            Label_ErrorMessage.Text = "Username already exists.";
                            Label_ErrorMessage.Visible = true;
                        }
                    }
                    else if (codeError == UserCodeError.CodeExpired)
                    {
                        Label_ErrorMessage.Text = "The given code has expired.";
                        Label_ErrorMessage.Visible = true;
                    }
                    else if (codeError == UserCodeError.CodeUsed)
                    {
                        Label_ErrorMessage.Text = "The given code has already been used.";
                        Label_ErrorMessage.Visible = true;
                    }
                    else if (codeError == UserCodeError.CodeDoesntExist)
                    {
                        Label_ErrorMessage.Text = "The given code does not exist.";
                        Label_ErrorMessage.Visible = true;
                    }

                }
                else
                {
                    Label_ErrorMessage.Text = "Email is invalid.";
                    Label_ErrorMessage.Visible = true;
                }
                
            }
            
        }
        
    }
}