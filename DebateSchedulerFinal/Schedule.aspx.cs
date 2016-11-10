using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
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

            if (currentDebateID != -1)
            {
                //Gathering query values
                NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                string orderString = queryValues.Get("Order");
                string debateOrderString = queryValues.Get("dOrder");
                string dateOrderString = queryValues.Get("date");
                string teamOrderString = queryValues.Get("team");

                if (orderString != null)
                {
                    order = (OrderBy)(int.Parse(orderString));
                }
                if (debateOrderString != null)
                {
                    dOrder = (DebateOrderVar)(int.Parse(debateOrderString));
                }

                debateSeason = DatabaseHandler.GetDebateSeason(currentDebateID);

                if (dateOrderString != null)
                {
                    int val;
                    bool result = int.TryParse(dateOrderString, out val);
                    if (result && val > 0 && val <= debateSeason.Length)
                    {
                        dateOrder = val;
                    }
                }
                if (teamOrderString != null)
                {
                    int val;
                    bool result = int.TryParse(teamOrderString, out val);
                    if (result && val > 0 && val <= debateSeason.Teams.Count)
                    {
                        teamOrder = val;
                    }
                }
                
                debates = debateSeason.Debates;

                debates = Help.OrderDebates(order, dOrder, debates);

                TableRow header = CreateHeaderRow();
                Table1.Rows.Add(header);

                seasonStart = DatabaseHandler.GetDebateSeasonDateTime(currentDebateID, out seasonLength);

                DateTime currentDate = seasonStart;
                for (int i = 1; i <= seasonLength; i++)
                {
                    string val = i.ToString();
                    ListItem dateItem = new ListItem(currentDate.ToString("MM/dd/yy"), val);
                    DropDownList_Date.Items.Add(dateItem);
                    currentDate = currentDate.AddDays(7);
                }

                teams = debateSeason.Teams;

                for (int i = 0; i < teams.Count; i++)
                {
                    string val = (i + 1).ToString();
                    ListItem teamItem = new ListItem(teams[i].Name, val);
                    DropDownList_TeamName.Items.Add(teamItem);
                }

                int addedRows = 0;
                //Adding the debates to the table
                if (!IsPostBack && (teamOrder > 0 || dateOrder > 0))
                {
                    if (teamOrder > 0)
                    {
                        searchName = teams[(teamOrder - 1)].Name;
                        DropDownList_TeamName.Items.FindByValue((teamOrder.ToString())).Selected = true;
                    }
                    if (dateOrder > 0)
                    {
                        searchDate = seasonStart.AddDays((dateOrder - 1) * 7);
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
                                TableRow debateRow = CreateDebateRow(d);
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

                        TableRow debateRow = CreateDebateRow(d);
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

        private TableRow CreateDebateRow(Debate d)
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();

            team1Cell.Width = nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;
            team2Cell.Width = nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;
            team1ScoreCell.Width = statsCellWidth;
            team2ScoreCell.Width = statsCellWidth;
            dateCell.Width = dateCellWidth;
            morningCell.Width = dateCellWidth;
            vsCell.Width = vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;

            team1Cell.Text = d.Team1.Name;
            team2Cell.Text = d.Team2.Name;
            if (d.Team1Score != -1)
                team1ScoreCell.Text = d.Team1Score.ToString();
            else
                team1ScoreCell.Text = noScoreDisplay;

            if (d.Team2Score != -1)
                team2ScoreCell.Text = d.Team2Score.ToString();
            else
                team2ScoreCell.Text = noScoreDisplay;

            dateCell.Text = d.Date.ToString("MM/dd/yy");

            vsCell.Text = "vs";

            if (d.MorningDebate)
                morningCell.Text = "Morning";
            else
                morningCell.Text = "Afternoon";

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

        private void RedirectWithParameters(DebateOrderVar debateOrderVar)
        {
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            if (dOrder == debateOrderVar && order != OrderBy.Descending) //If team order is already by name, then we flip them.
            {
                queryValues.Set("dOrder", ((int)debateOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Descending).ToString());
            }
            else
            {
                queryValues.Set("dOrder", ((int)debateOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Ascending).ToString());
            }

            queryValues.Set("date", dateOrder.ToString());
            queryValues.Set("team", teamOrder.ToString());

            string url = Request.Url.AbsolutePath;
            Response.Redirect(url + "?" + queryValues);
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Makes the day unselectable and its back color "Ghost White" if it is not a Saturday.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void DayRender(Object source, DayRenderEventArgs e)
        {

            if (e.Day.Date.DayOfWeek != DayOfWeek.Saturday)
            {
                e.Day.IsSelectable = false;
                e.Cell.BackColor = System.Drawing.Color.GhostWhite;
            }
        }

        protected void Button_Search_Click(object sender, EventArgs e)
        {
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());

            dateOrder = int.Parse(DropDownList_Date.SelectedValue);
            teamOrder = int.Parse(DropDownList_TeamName.SelectedValue);

            queryValues.Set("date", dateOrder.ToString());
            queryValues.Set("team", teamOrder.ToString());

            string url = Request.Url.AbsolutePath;
            Response.Redirect(url + "?" + queryValues);
        }
    }
}