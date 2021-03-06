﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateSchedulerFinal
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        private const int nameCellWidth = 250;
        private const int statsCellWidth = 80;
        private const int dateCellWidth = 150;
        private const int vsCellWidth = 30;
        private const string noScoreDisplay = "";

        private bool includeVs = true;

        private readonly Color headerTableColor = Color.CornflowerBlue;
        private readonly Color headerTableTextColor = Color.White;

        private OrderBy order = OrderBy.Ascending;
        private DebateOrderVar dOrder = DebateOrderVar.Date;

        private DebateSeason debateSeason;
        private List<Debate> debates = new List<Debate>();
        private List<Team> teams = new List<Team>();

        private int seasonLength;
        private DateTime seasonStart;

        private int dateOrder = 0;
        private DateTime searchDate;
        private int teamOrder = 0;
        private string searchName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            int currentDebateID = Help.GetDebateSeasonID(Application);
            Panel_Searching.Visible = true;
            Panel_NoDebate.Visible = false;

            if (currentDebateID != -1)
            {
                object orderObj = ViewState["Order"];
                object debateOrderObj = ViewState["dOrder"];
                object dateOrderObj = ViewState["date"];
                object teamOrderObj = ViewState["team"];
                
                if (orderObj != null)
                {
                    order = (OrderBy)(int.Parse(orderObj.ToString()));
                }
                if (debateOrderObj != null)
                {
                    dOrder = (DebateOrderVar)(int.Parse(debateOrderObj.ToString()));
                }
                
                object savedSeason = ViewState["Season"];

                if (savedSeason == null)
                {
                    debateSeason = DatabaseHandler.GetDebateSeason(currentDebateID);
                }
                else
                    debateSeason = savedSeason as DebateSeason;

                ViewState["Season"] = debateSeason;

                if (dateOrderObj != null)
                {
                    int val;
                    bool result = int.TryParse(dateOrderObj.ToString(), out val);
                    if (result && val > 0 && val <= debateSeason.Length)
                    {
                        dateOrder = val;
                    }
                }
                if (teamOrderObj != null)
                {
                    int val;
                    bool result = int.TryParse(teamOrderObj.ToString(), out val);
                    if (result && val > 0 && val <= debateSeason.Teams.Count)
                    {
                        teamOrder = val;
                    }
                }
                
                debates = debateSeason.Debates;

                debates = Help.OrderDebates(order, dOrder, debates);

                TableRow header = CreateHeaderRow();
                Table1.Rows.Add(header);

                seasonStart = debateSeason.StartDate;
                seasonLength = debateSeason.Length;

                DateTime currentDate = seasonStart;
                
                teams = debateSeason.Teams;

                if (!IsPostBack) //We do not do this on page post backs (IE: After searches).
                {
                    //Creating the team drop down
                    for (int i = 0; i < teams.Count; i++)
                    {
                        string val = (i + 1).ToString();
                        ListItem teamItem = new ListItem(teams[i].Name, val);
                        DropDownList_TeamName.Items.Add(teamItem);
                    }

                    //Creating the date drop down
                    for (int i = 1; i <= seasonLength; i++)
                    {
                        string val = i.ToString();
                        ListItem dateItem = new ListItem(currentDate.ToString("MM/dd/yy"), val);
                        DropDownList_Date.Items.Add(dateItem);
                        currentDate = currentDate.AddDays(7);
                    }
                }

                int addedRows = 0;
                //Adding the debates to the table
                if ((teamOrder > 0 || dateOrder > 0))
                {

                    if (teamOrder > 0)
                    {
                        searchName = teams[(teamOrder - 1)].Name;
                        if (!IsPostBack)
                            DropDownList_TeamName.Items.FindByValue((teamOrder.ToString())).Selected = true;
                    }
                    if (dateOrder > 0)
                    {
                        searchDate = seasonStart.AddDays((dateOrder - 1) * 7);
                        if (!IsPostBack)
                            DropDownList_Date.Items.FindByValue((dateOrder.ToString())).Selected = true;
                    }

                    foreach (Debate d in debates)
                    {
                        if (dateOrder == 0 || (d.Date.Month == searchDate.Month &&
                            d.Date.Day == searchDate.Day &&
                            d.Date.Year == searchDate.Year))
                        {
                            if (teamOrder == 0 || (d.Team1.Name == searchName ||
                                d.Team2.Name == searchName))
                            {
                                TableRow debateRow = Help.CreateDebateRow(d, true, addedRows);
                                Table1.Rows.Add(debateRow);
                                addedRows++;
                            }
                        }
                        
                    }
                }
                else
                {
                    foreach (Debate d in debates)
                    {

                        TableRow debateRow = Help.CreateDebateRow(d, true, addedRows);
                        Table1.Rows.Add(debateRow);
                        addedRows++;
                    }
                }
                if (addedRows <= 0)
                {
                    Table1.Visible = false;
                    Panel_NoDebate.Visible = true;
                    Label_NoSchedule.Text = "No results match your search.";
                }
                else
                {
                    Table1.Visible = true;
                    Panel_NoDebate.Visible = false;
                }
            }
            else
            {
                Panel_Searching.Visible = false;
                Panel_NoDebate.Visible = true;
            }

        }

        private TableRow CreateHeaderRow()
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();

            team1Cell.BackColor = headerTableColor;
            team1Cell.Width = nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;

            team2Cell.BackColor = headerTableColor;
            team2Cell.Width = nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;

            team1ScoreCell.BackColor = headerTableColor;
            team1ScoreCell.Width = statsCellWidth;

            team2ScoreCell.BackColor = headerTableColor;
            team2ScoreCell.Width = statsCellWidth;

            dateCell.BackColor = headerTableColor;
            dateCell.Width = dateCellWidth;

            morningCell.BackColor = headerTableColor;
            morningCell.Width = dateCellWidth;

            vsCell.BackColor = headerTableColor;
            vsCell.Width = vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;

            LinkButton team1Button = new LinkButton();
            team1Button.Command += Team1Button_Command;
            team1Button.ForeColor = headerTableTextColor;
            team1Button.Text = "Team 1";

            LinkButton team2Button = new LinkButton();
            team2Button.Command += Team2Button_Command;
            team2Button.ForeColor = headerTableTextColor;
            team2Button.Text = "Team 2";

            LinkButton team1ScoreButton = new LinkButton();
            team1ScoreButton.Command += Team1ScoreButton_Command;
            team1ScoreButton.ForeColor = headerTableTextColor;
            team1ScoreButton.Text = "Team 1 Score";

            LinkButton team2ScoreButton = new LinkButton();
            team2ScoreButton.Command += Team2ScoreButton_Command;
            team2ScoreButton.ForeColor = headerTableTextColor;
            team2ScoreButton.Text = "Team 2 Score";

            LinkButton dateButton = new LinkButton();
            dateButton.Command += DateButton_Command;
            dateButton.ForeColor = headerTableTextColor;
            dateButton.Text = "Date";

            LinkButton morningButton = new LinkButton();
            morningButton.Command += MorningButton_Command;
            morningButton.ForeColor = headerTableTextColor;
            morningButton.Text = "Time";

            vsCell.ForeColor = headerTableColor;
            vsCell.Text = "Versus";

            team1Cell.Controls.Add(team1Button);
            team2Cell.Controls.Add(team2Button);
            team1ScoreCell.Controls.Add(team1ScoreButton);
            team2ScoreCell.Controls.Add(team2ScoreButton);
            dateCell.Controls.Add(dateButton);
            morningCell.Controls.Add(morningButton);

            row.Cells.Add(team1Cell);
            if (includeVs)
                row.Cells.Add(vsCell);
            row.Cells.Add(team2Cell);
            row.Cells.Add(team1ScoreCell);
            row.Cells.Add(team2ScoreCell);
            row.Cells.Add(dateCell);
            row.Cells.Add(morningCell);

            return row;
        }

        private void MorningButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.MorningDebate);
        }

        private void DateButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Date);
        }

        private void Team2ScoreButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team2Score);
        }

        private void Team1ScoreButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team1Score);
        }

        private void Team2Button_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team2Name);
        }

        private void Team1Button_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team1Name);
        }

        /// <summary>
        /// Refreshes the page with a new order for the table on page load.
        /// </summary>
        /// <param name="debateOrderVar"></param>
        private void RedirectWithParameters(DebateOrderVar debateOrderVar)
        {
            if (dOrder == debateOrderVar && order != OrderBy.Descending) //If team order is already by name, then we flip them.
            {
                ViewState["Order"] = (int)OrderBy.Descending;
            }
            else
            {
                ViewState["Order"] = (int)OrderBy.Ascending;
            }

            dateOrder = int.Parse(DropDownList_Date.SelectedValue);
            teamOrder = int.Parse(DropDownList_TeamName.SelectedValue);
            
            ViewState["dOrder"] = (int)debateOrderVar;
            ViewState["date"] = dateOrder;
            ViewState["team"] = teamOrder;

            Help.ForcePostBack(this);
        }

        protected void Button_Search_Click(object sender, EventArgs e)
        {
            dateOrder = int.Parse(DropDownList_Date.SelectedValue);
            teamOrder = int.Parse(DropDownList_TeamName.SelectedValue);

            ViewState["Order"] = (int)order;
            ViewState["dOrder"] = (int)dOrder;
            ViewState["date"] = dateOrder;
            ViewState["team"] = teamOrder;

            Help.ForcePostBack(this);
        }
    }
}