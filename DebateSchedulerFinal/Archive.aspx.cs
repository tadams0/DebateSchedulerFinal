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
    public partial class Archive : System.Web.UI.Page
    {
        private const string noScoreDisplay = "";

        private bool includeVs = true;
        
        private OrderBy teamOrderType = OrderBy.Ascending;
        private OrderBy debateOrderType = OrderBy.Ascending;
        private DebateOrderVar debateOrderVar = DebateOrderVar.Date;
        private TeamOrderVar teamOrderVar = TeamOrderVar.Rank;

        private DebateSeason debateSeason;
        private List<Debate> debates = new List<Debate>();
        private List<Team> teams = new List<Team>();

        private int seasonLength;
        private DateTime seasonStart;

        private int dateOrder = 0;
        private DateTime searchDate;
        private int teamOrder = 0;
        private string searchName = string.Empty;

        private int seasonIndex = 1;
        private int minimumSeasonIndex = 1;
        private int currentDebateSeason = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            int currentDebateID = Help.GetDebateSeasonID(Application);

            if (currentDebateID != -1)
                minimumSeasonIndex = 2;


            //Gathering query values
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            string pageString = queryValues.Get("page");
            if (pageString != null)
            {
                bool result = int.TryParse(pageString, out seasonIndex);
                if (!result) //If we could not parse the string then we reset the index.
                    seasonIndex = minimumSeasonIndex;
            }

            if (seasonIndex <= minimumSeasonIndex)
            {
                seasonIndex = minimumSeasonIndex;
                Button_PreviousSeason.Visible = false;
            }

            Panel_Search.Visible = true;
            Panel_NoSearch.Visible = false;
            Button_NextSeason.Enabled = true;
            Button_PreviousSeason.Enabled = true;

            object teamOrderTypeObj = ViewState["tOrderType"]; //Team order type
            object debateOrderTypeObj = ViewState["dOrderType"]; //Debate order type
            object teamOrderVarObj = ViewState["teamOrder"]; //The order of teams
            object debateOrderVarObj = ViewState["debateOrder"]; //The order of debates.
            object dateOrderObj = ViewState["date"];
            object teamOrderObj = ViewState["team"];
            object seasonObj = ViewState["season"];

            if (debateOrderTypeObj != null)
            {
                debateOrderType = (OrderBy)(int.Parse(debateOrderTypeObj.ToString()));
            }
            if (debateOrderVarObj != null)
            {
                debateOrderVar = (DebateOrderVar)(int.Parse(debateOrderVarObj.ToString()));
            }
            if (teamOrderTypeObj != null)
            {
                teamOrderType = (OrderBy)(int.Parse(teamOrderTypeObj.ToString()));
            }
            if (teamOrderVarObj != null)
            {
                teamOrderVar = (TeamOrderVar)(int.Parse(teamOrderVarObj.ToString()));
            }

            if (seasonObj == null)
            {
                List<DebateSeason> debateSeasons = DatabaseHandler.GetDebateSeasons(seasonIndex, seasonIndex + 1);
                if (debateSeasons.Count > 0)
                    debateSeason = debateSeasons[0];
                else  //No debates were found.. ut oh.
                {
                    Button_NextSeason.Visible = false;
                    Panel_PagePanel.Visible = false;
                    Label_SeasonTitle.Text = "No debate seasons here!";
                }
            }
            else
                debateSeason = seasonObj as DebateSeason;

            ViewState["season"] = debateSeason;

            if (debateSeason != null) //We do not display the current ongoing debate season.
            {
                if (debateSeason.ID != currentDebateID)
                {
                    //Setting up the enviroment
                    currentDebateSeason = debateSeason.ID;
                    DateTime startDate = debateSeason.StartDate;
                    DateTime endDate = debateSeason.StartDate.AddDays(debateSeason.Length * 7);
                    Label_SeasonTitle.Text = "Debate Season that started on " + startDate.ToString("MM/dd/yy") + " and ended on " + endDate.ToString("MM/dd/yy");

                    //Generating the team rankings table
                    teams = debateSeason.Teams;
                    teams = Help.RankTeams(teams);

                    teams = Help.OrderTeams(teamOrderType, teamOrderVar, teams);

                    TableRow header = CreateTeamHeaderRow();
                    Table_Rankings.Rows.Add(header);

                    for (int i = 0; i < teams.Count; i++)
                    {
                        TableRow teamRow = Help.CreateTeamRow(teams[i], i);
                        Table_Rankings.Rows.Add(teamRow);
                    }

                    //Generating the search options and debate table...
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

                    debates = Help.OrderDebates(debateOrderType, debateOrderVar, debates);

                    TableRow debateHeader = CreateDebateHeaderRow();
                    TableData.Rows.Add(debateHeader);

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
                                    TableRow debateRow = Help.CreateDebateRow(d, includeVs);
                                    TableData.Rows.Add(debateRow);
                                    addedRows++;
                                }
                            }

                        }
                    }
                    else
                    {
                        foreach (Debate d in debates)
                        {

                            TableRow debateRow = Help.CreateDebateRow(d, includeVs);
                            TableData.Rows.Add(debateRow);
                            addedRows++;
                        }
                    }
                    if (addedRows <= 0)
                    {
                        TableData.Visible = false;
                        Panel_NoSearch.Visible = true;
                        Label_NoSearch.Text = "No results match your search.";
                    }
                    else
                    {
                        TableData.Visible = true;
                        Panel_NoSearch.Visible = false;
                    }
                }
            }


        }

        private TableRow CreateTeamHeaderRow()
        {
            TableRow row = new TableRow();

            TableCell rankCell = new TableCell();
            TableCell nameCell = new TableCell();
            TableCell winCell = new TableCell();
            TableCell lossCell = new TableCell();
            TableCell tieCell = new TableCell();
            TableCell totalScore = new TableCell();

            rankCell.BackColor = Help.headerTableColor;
            nameCell.BackColor = Help.headerTableColor;
            winCell.BackColor = Help.headerTableColor;
            lossCell.BackColor = Help.headerTableColor;
            tieCell.BackColor = Help.headerTableColor;
            totalScore.BackColor = Help.headerTableColor;

            LinkButton rankButton = new LinkButton();
            rankButton.Command += RankButton_Command;
            rankButton.ForeColor = Help.headerTableTextColor;
            rankButton.Text = "Rank";

            LinkButton nameButton = new LinkButton();
            nameButton.Command += NameButton_Command;
            nameButton.ForeColor = Help.headerTableTextColor;
            nameButton.Text = "Name";

            LinkButton winButton = new LinkButton();
            winButton.Command += WinButton_Command;
            winButton.ForeColor = Help.headerTableTextColor;
            winButton.Text = "Wins";

            LinkButton lossButton = new LinkButton();
            lossButton.Command += LossButton_Command;
            lossButton.ForeColor = Help.headerTableTextColor;
            lossButton.Text = "Losses";

            LinkButton tieButton = new LinkButton();
            tieButton.Command += TieButton_Command;
            tieButton.ForeColor = Help.headerTableTextColor;
            tieButton.Text = "Ties";

            LinkButton totalScoreButton = new LinkButton();
            totalScoreButton.Command += TotalScoreButton_Command;
            totalScoreButton.ForeColor = Help.headerTableTextColor;
            totalScoreButton.Text = "Total Score";

            rankCell.Controls.Add(rankButton);
            nameCell.Controls.Add(nameButton);
            winCell.Controls.Add(winButton);
            lossCell.Controls.Add(lossButton);
            tieCell.Controls.Add(tieButton);
            totalScore.Controls.Add(totalScoreButton);

            row.Cells.Add(rankCell);
            row.Cells.Add(nameCell);
            row.Cells.Add(winCell);
            row.Cells.Add(lossCell);
            row.Cells.Add(tieCell);
            row.Cells.Add(totalScore);

            return row;
        }

        private void RankButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Rank);
        }

        private void TotalScoreButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.TotalScore);
        }

        private void TieButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Ties);
        }

        private void LossButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Losses);
        }

        private void WinButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Wins);
        }

        private void NameButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Name);
        }

        private TableRow CreateDebateHeaderRow()
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();

            team1Cell.BackColor = Help.headerTableColor;
            team1Cell.Width = Help.nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;

            team2Cell.BackColor = Help.headerTableColor;
            team2Cell.Width = Help.nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;

            team1ScoreCell.BackColor = Help.headerTableColor;
            team1ScoreCell.Width = Help.statsCellWidth;

            team2ScoreCell.BackColor = Help.headerTableColor;
            team2ScoreCell.Width = Help.statsCellWidth;

            dateCell.BackColor = Help.headerTableColor;
            dateCell.Width = Help.dateCellWidth;

            morningCell.BackColor = Help.headerTableColor;
            morningCell.Width = Help.dateCellWidth;

            vsCell.BackColor = Help.headerTableColor;
            vsCell.Width = Help.vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;

            LinkButton team1Button = new LinkButton();
            team1Button.Command += Team1Button_Command;
            team1Button.ForeColor = Help.headerTableTextColor;
            team1Button.Text = "Team 1";

            LinkButton team2Button = new LinkButton();
            team2Button.Command += Team2Button_Command;
            team2Button.ForeColor = Help.headerTableTextColor;
            team2Button.Text = "Team 2";

            LinkButton team1ScoreButton = new LinkButton();
            team1ScoreButton.Command += Team1ScoreButton_Command;
            team1ScoreButton.ForeColor = Help.headerTableTextColor;
            team1ScoreButton.Text = "Team 1 Score";

            LinkButton team2ScoreButton = new LinkButton();
            team2ScoreButton.Command += Team2ScoreButton_Command;
            team2ScoreButton.ForeColor = Help.headerTableTextColor;
            team2ScoreButton.Text = "Team 2 Score";

            LinkButton dateButton = new LinkButton();
            dateButton.Command += DateButton_Command;
            dateButton.ForeColor = Help.headerTableTextColor;
            dateButton.Text = "Date";

            LinkButton morningButton = new LinkButton();
            morningButton.Command += MorningButton_Command;
            morningButton.ForeColor = Help.headerTableTextColor;
            morningButton.Text = "Time";

            vsCell.ForeColor = Help.headerTableColor;
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

        

        /// <summary>
        /// Refreshes the page with a new order for the table on page load.
        /// </summary>
        /// <param name="debateOrderVar">The variable to order debates by.</param>
        private void RedirectWithParameters(DebateOrderVar debateOrderVar)
        {
            if (this.debateOrderVar == debateOrderVar && debateOrderType != OrderBy.Descending) //If team order is already by name, then we flip them.
            {
                ViewState["dOrderType"] = (int)OrderBy.Descending;
            }
            else
            {
                ViewState["dOrderType"] = (int)OrderBy.Ascending;
            }

            ViewState["debateOrder"] = (int)teamOrderVar;

            dateOrder = int.Parse(DropDownList_Date.SelectedValue);
            teamOrder = int.Parse(DropDownList_TeamName.SelectedValue);

            ViewState["date"] = dateOrder;
            ViewState["team"] = teamOrder;
            ViewState["seasonID"] = seasonIndex;
            ViewState["tOrderType"] = (int)debateOrderType;
            ViewState["debateOrder"] = (int)debateOrderVar;
            ViewState["prevSeasonID"] = seasonIndex;

            Help.ForcePostBack(this);
        }

        /// <summary>
        /// Refreshes the page with a new order for the table on page load.
        /// </summary>
        /// <param name="teamOrderVar">The variable to order teams by.</param>
        private void RedirectWithParameters(TeamOrderVar teamOrderVar)
        {
            if (this.teamOrderVar == teamOrderVar && teamOrderType != OrderBy.Descending) //If team order is already by name, then we flip them.
            {
                ViewState["tOrderType"] = (int)OrderBy.Descending;
                
            }
            else
            {
                ViewState["tOrderType"] = (int)OrderBy.Ascending;
            }

            ViewState["teamOrder"] = (int)teamOrderVar;

            dateOrder = int.Parse(DropDownList_Date.SelectedValue);
            teamOrder = int.Parse(DropDownList_TeamName.SelectedValue);

            ViewState["date"] = dateOrder;
            ViewState["team"] = teamOrder;
            ViewState["dOrderType"] = (int)debateOrderType;
            ViewState["debateOrder"] = (int)debateOrderVar;

            Help.ForcePostBack(this);
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

        protected void Button_PreviousSeason_Click(object sender, EventArgs e)
        {
            Button_NextSeason.Enabled = false;
            Button_PreviousSeason.Enabled = false;

            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            queryValues.Set("page", (seasonIndex - 1).ToString());
            string url = Request.Url.AbsolutePath;
            Response.Redirect(url + "?" + queryValues);
        }

        protected void Button_NextSeason_Click(object sender, EventArgs e)
        {
            Button_NextSeason.Enabled = false;
            Button_PreviousSeason.Enabled = false;

            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            queryValues.Set("page", (seasonIndex + 1).ToString());
            string url = Request.Url.AbsolutePath;
            Response.Redirect(url + "?" + queryValues);
        }

        protected void Button_Search_Click(object sender, EventArgs e)
        {
            dateOrder = int.Parse(DropDownList_Date.SelectedValue);
            teamOrder = int.Parse(DropDownList_TeamName.SelectedValue);
            
            ViewState["date"] = dateOrder;
            ViewState["team"] = teamOrder;

            Help.ForcePostBack(this);
        }
    }
}