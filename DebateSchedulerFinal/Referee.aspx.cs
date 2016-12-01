using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Collections.Specialized;
using System.Globalization;

namespace DebateSchedulerFinal
{
    public partial class Referee : System.Web.UI.Page
    {
        private const int nameCellWidth = 250;
        private const int statsCellWidth = 120;
        private const int dateCellWidth = 250;
        private const int vsCellWidth = 30;

        private bool includeVs = true;

        private readonly Color headerTableColor = Color.CornflowerBlue;
        private readonly Color headerTableTextColor = Color.White;
        private byte rowColor = 0;

        private OrderBy order = OrderBy.Ascending;
        private DebateOrderVar dOrder = DebateOrderVar.Date;

        private List<Debate> debates = new List<Debate>();

        private int debateSeasonID;

        private DateTime seasonStartDate;
        private int seasonLength;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(2);
            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null)
            {
                //Gathering query values
                NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                string orderString = queryValues.Get("Order");
                string debateOrderString = queryValues.Get("dOrder");

                if (orderString != null)
                {
                    order = (OrderBy)(int.Parse(orderString));
                }
                if (debateOrderString != null)
                {
                    dOrder = (DebateOrderVar)(int.Parse(debateOrderString));
                }

                debateSeasonID = Help.GetDebateSeasonID(Application);

                if (debateSeasonID == -1) //There is no debate season going on.
                {
                    Panel_Info.Visible = true;
                    Label_Info.Text = "There is currently no ongoing debate season to score.";
                    Panel1.Visible = false;
                    Panel_UpdateScores.Visible = false;
                    return; //We end the method here if there is no debate season/
                }
                debates = DatabaseHandler.GetDebateSeasonDebates(debateSeasonID);

                seasonStartDate = DatabaseHandler.GetDebateSeasonDateTime(debateSeasonID, out seasonLength);

                debates = Help.OrderDebates(order, dOrder, debates);

                TableRow header = CreateHeaderRow();
                Table1.Rows.Add(header);

                int rowNum = 1; // row 0 will be the header row.
                foreach (Debate d in debates)
                {
                    if (loggedUser.PermissionLevel == 2)
                    {
                        if (d.Team1Score == -1 && d.Team2Score == -1)
                        {
                            TableRow debateRow = CreateDebateRow(d, rowNum, loggedUser);
                            Table1.Rows.Add(debateRow);
                            rowNum++;
                        }
                    }
                    else if (loggedUser.PermissionLevel == 3)
                    {
                        TableRow debateRow = CreateDebateRow(d, rowNum, loggedUser);
                        Table1.Rows.Add(debateRow);
                        rowNum++;
                    }
                }

                if (Table1.Rows.Count == 1 && loggedUser.PermissionLevel < 3) //Only the header exists in the table. IE: There are no more debates to score.
                {
                    Panel_Info.Visible = true;
                    Label_Info.Text = "All debates have been scored, the season must now be ended by a " + Help.GetPermissionName(3);
                    Panel1.Visible = false;
                    Panel_UpdateScores.Visible = false;
                }

                //Creating the end debate season button.
                if (loggedUser.PermissionLevel >= 3 && DatabaseHandler.DebateSeasonScored(debateSeasonID))
                {
                    //The entire debate season has been scored, so let's add the option to end it.
                    Button endSeasonBut = new Button();
                    endSeasonBut.Width = UpdateButton.Width;
                    endSeasonBut.Height = 20;
                    endSeasonBut.Text = "End Debate Season";
                    endSeasonBut.Command += EndSeasonBut_Command;
                    Panel_UpdateScores.Controls.AddAt(Panel_UpdateScores.Controls.Count - 1, new LiteralControl("<br /> <br /> <br />"));
                    Panel_UpdateScores.Controls.AddAt(Panel_UpdateScores.Controls.Count - 1, endSeasonBut);
                    Panel_UpdateScores.Controls.AddAt(Panel_UpdateScores.Controls.Count - 1, new LiteralControl("<br />"));
                }


            }

        }

        private void EndSeasonBut_Command(object sender, CommandEventArgs e)
        {
            //The end debate season button has been pressed, now we end the debate season.
            DatabaseHandler.EndDebateSeason(Session, debateSeasonID);
            Help.SetDebateID(Application, -1);
            Response.Redirect(Request.RawUrl);
        }

        private TableRow CreateHeaderRow()
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            //TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();
            TableCell idCell = new TableCell();

            team1Cell.BackColor = headerTableColor;
            team1Cell.Width = nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;
            team1Cell.Text = "Team 1";

            team2Cell.BackColor = headerTableColor;
            team2Cell.Width = nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;
            team2Cell.Text = "Team 2";

            team1ScoreCell.BackColor = headerTableColor;
            team1ScoreCell.Width = statsCellWidth;
            team1ScoreCell.Text = "Team 1 Score";

            team2ScoreCell.BackColor = headerTableColor;
            team2ScoreCell.Width = statsCellWidth;
            team2ScoreCell.Text = "Team 2 Score";

            dateCell.HorizontalAlign = HorizontalAlign.Center;
            dateCell.BackColor = headerTableColor;
            dateCell.Width = dateCellWidth;
            dateCell.Text = "Date and Time";

            //morningCell.BackColor = headerTableColor;
            //morningCell.Width = dateCellWidth;
            //morningCell.Text = "Time";

            vsCell.BackColor = headerTableColor;
            vsCell.Width = vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;
            vsCell.Text = "vs.";

            idCell.BackColor = headerTableColor;
            idCell.Width = statsCellWidth;
            idCell.HorizontalAlign = HorizontalAlign.Center;
            idCell.Text = "ID";

            row.Cells.Add(team1Cell);
            if (includeVs)
                row.Cells.Add(vsCell);
            row.Cells.Add(team2Cell);
            row.Cells.Add(team1ScoreCell);
            row.Cells.Add(team2ScoreCell);
            row.Cells.Add(dateCell);
            //row.Cells.Add(morningCell);
            row.Cells.Add(idCell);

            return row;
        }

        private TableRow CreateDebateRow(Debate d, int rowNum, User loggedUser)
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            //TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();
            TableCell idCell = new TableCell();

            team1Cell.Width = nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;
            team2Cell.Width = nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;
            team1ScoreCell.Width = statsCellWidth;
            team1ScoreCell.HorizontalAlign = HorizontalAlign.Center;
            team2ScoreCell.Width = statsCellWidth;
            team2ScoreCell.HorizontalAlign = HorizontalAlign.Center;
            dateCell.Width = dateCellWidth;
            dateCell.HorizontalAlign = HorizontalAlign.Center;
            //morningCell.Width = dateCellWidth;
            vsCell.Width = vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;
            idCell.Width = statsCellWidth;
            idCell.HorizontalAlign = HorizontalAlign.Center;

            team1Cell.Text = d.Team1.Name;
            team2Cell.Text = d.Team2.Name;
            DropDownList ddl = new DropDownList();
            ddl.ID = "ddl" + rowNum;
            ddl.Items.Add(new ListItem("", "-1")); //Unscored value
            ddl.Items.Add(new ListItem("0", "0"));
            ddl.Items.Add(new ListItem("1", "1"));
            ddl.Items.Add(new ListItem("2", "2"));
            ddl.Items.Add(new ListItem("3", "3"));
            ddl.Items.Add(new ListItem("4", "4"));
            ddl.Items.Add(new ListItem("5", "5"));
            if (d.Team1Score == -1)
                ddl.SelectedIndex = 0;//0;
            else
                ddl.SelectedIndex = d.Team1Score + 1; //The + 2 is because of the 1 extra index items in ddl
            team1ScoreCell.Controls.Add(ddl);

            DropDownList ddl1 = new DropDownList();
            ddl1.ID = "ddl#" + rowNum;
            ddl1.Items.Add(new ListItem("", "-1")); //Unscored value
            ddl1.Items.Add(new ListItem("0", "0"));
            ddl1.Items.Add(new ListItem("1", "1"));
            ddl1.Items.Add(new ListItem("2", "2"));
            ddl1.Items.Add(new ListItem("3", "3"));
            ddl1.Items.Add(new ListItem("4", "4"));
            ddl1.Items.Add(new ListItem("5", "5"));
            if (d.Team2Score == -1)
                ddl1.SelectedIndex = 0;//0;
            else
                ddl1.SelectedIndex = d.Team2Score + 1; //The + 1 is because of the 1 extra index items in ddl
            team2ScoreCell.Controls.Add(ddl1);

            //Creating the date cell
            DropDownList dateTimeList = new DropDownList();
            dateTimeList.Visible = false;
            dateTimeList.ID = rowNum.ToString();
            Label dateLabel = new Label();
            dateLabel.ID = "L" + rowNum;
            if (d.MorningDebate)
                dateLabel.Text = d.Date.ToString("MM/dd/yy") + " during the morning.";
            else
                dateLabel.Text = d.Date.ToString("MM/dd/yy") + " during the afternoon.";
            dateCell.Controls.Add(dateLabel);
            dateCell.Controls.Add(dateTimeList);
            dateCell.Controls.Add(new LiteralControl("<br />"));

            if (d.Team1Score < 0 && d.Team2Score < 0 && loggedUser.PermissionLevel >= 3)
            {
                LinkButton rescheduleBut = new LinkButton();
                rescheduleBut.Text = "Reschedule";
                rescheduleBut.ID = "R" + rowNum;
                rescheduleBut.CommandArgument = CreateRescheduleString(d.ID, rowNum);
                rescheduleBut.Command += RescheduleBut_Command;
                dateCell.Controls.Add(rescheduleBut);
            }
            else
            {
                dateCell.Controls.Add(new LiteralControl("<br />"));
            }

            vsCell.Text = "vs";

            idCell.Text = "" + d.ID;

            row.Cells.Add(team1Cell);
            if (includeVs)
                row.Cells.Add(vsCell);
            row.Cells.Add(team2Cell);
            row.Cells.Add(team1ScoreCell);
            row.Cells.Add(team2ScoreCell);
            row.Cells.Add(dateCell);
            row.Cells.Add(idCell);

            rowColor++;
            if (rowColor % 2 != 0)
                row.BackColor = Help.rowColor2;
            else
                row.BackColor = Help.rowColor1;

            return row;
        }

        private string CreateRescheduleString(int debateID, int rowNumber)
        {
            return debateID + ":" + rowNumber;
        }

        private void ParseRescheduleString(string s, out int debateID, out int rowNumber)
        {
            string[] results = s.Split(':');
            debateID = int.Parse(results[0]);
            rowNumber = int.Parse(results[1]);
        }

        private string CreateDateTimeString(DateTime date, bool morning)
        {
            return Help.GetDateString(date) + "/" + morning;
        }

        private void ParseDateTimeString(string s, out DateTime date, out bool morning)
        {
            string[] results = s.Split('/');
            string dateString = results[0];
            date = Help.GetDate(dateString);
            morning = Convert.ToBoolean(results[1]);
        }

        private void RescheduleBut_Command(object sender, CommandEventArgs e)
        {
            int debateID;
            int rowNumber;
            ParseRescheduleString(e.CommandArgument as string, out debateID, out rowNumber);

            Debate debate = null;
            foreach (Debate d in debates)
            {
                if (d.ID == debateID)
                {
                    debate = d;
                    break;
                }
            }

            TableRow row = Table1.Rows[rowNumber];
            TableCell cell = row.Cells[5];

            LinkButton but = cell.FindControl("R" + rowNumber) as LinkButton;
            DropDownList dropDownList = cell.FindControl(rowNumber.ToString()) as DropDownList;
            dropDownList.Visible = true;
            if (but.Text == "Confirm Reschedule")
            {
                string value = dropDownList.SelectedValue;
                if (value != string.Empty)
                {
                    DateTime date;
                    bool morning;
                    ParseDateTimeString(value, out date, out morning);
                    debate.Date = date;
                    debate.MorningDebate = morning;
                    DatabaseHandler.UpdateDebate(Session, debate);
                }
                dropDownList.Enabled = false;
                dropDownList.Visible = false;
                but.Text = "Reschedule";
                Help.ForcePostBack(this);
            }
            else
            {
                TableCell resetCell;
                for (int i = 1; i < Table1.Rows.Count; i++) //We must now reset all other reschedulings so there is not any out of date possibilities.
                {
                    if (i != rowNumber) //We do not reset the clicked row.
                    {
                        resetCell = Table1.Rows[i].Cells[5];
                        DropDownList list = resetCell.FindControl(i.ToString()) as DropDownList;
                        Label label = resetCell.FindControl("L" + i) as Label;
                        LinkButton button = resetCell.FindControl("R" + i) as LinkButton;
                        if (list.Visible)
                        {
                            label.Visible = true;
                            button.Text = "Reschedule";
                            list.Visible = false;
                        }
                    }
                }
                but.Text = "Confirm Reschedule";
                List<DebateDate> dates = Help.GetAllAvailableDates(debates, debate, seasonStartDate, seasonLength);
                dropDownList.Items.Clear();
                dropDownList.Enabled = true;
                ListItem listItem = new ListItem();
                listItem.Text = "Do not reschedule.";
                listItem.Value = string.Empty;
                dropDownList.Items.Add(listItem);
                for (int i = 0; i < dates.Count; i++)
                {
                    listItem = new ListItem();
                    string morningAppend = "during the afternoon.";
                    if (dates[i].Morning)
                        morningAppend = "during the morning.";
                    listItem.Text = dates[i].Date.ToString("MM/dd/yy") + " " + morningAppend;
                    listItem.Value = CreateDateTimeString(dates[i].Date, dates[i].Morning);
                    dropDownList.Items.Add(listItem);
                }

                ViewState["id"] = debateID;
                ViewState["rowNumb"] = rowNumber;
                
                cell.Controls.Add(but);
            }
            //Help.ForcePostBack(this);
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);
            if (loggedUser != null)
            {
                ErrorLabel.Visible = false;
                for (int rowNum = 1; rowNum < Table1.Rows.Count; rowNum++) //Starts at row 1 since row 0 is header row.
                {
                    DropDownList TeamScore1Control = Table1.Rows[rowNum].Cells[3].FindControl("ddl" + rowNum) as DropDownList;
                    DropDownList TeamScore2Control = Table1.Rows[rowNum].Cells[4].FindControl("ddl#" + rowNum) as DropDownList;
                    int team1Score = int.Parse(TeamScore1Control.SelectedValue);
                    int team2Score = int.Parse(TeamScore2Control.SelectedValue);
                    int id;
                    bool success = int.TryParse(Table1.Rows[rowNum].Cells[6].Text, NumberStyles.Any, CultureInfo.InvariantCulture, out id);
                    Debate debate = debates[rowNum - 1];
                    if (team1Score >= 0 && team2Score >= 0)
                    {
                        debate.Team1Score = team1Score;
                        debate.Team2Score = team2Score;

                        bool result = DatabaseHandler.UpdateDebate(Session, debate);
                        for (int i = 0; i < debates.Count; i++)
                        {
                            if (debates[i].Team1.ID == debate.Team1.ID)
                                debates[i].Team1 = debate.Team1;
                            else if (debates[i].Team2.ID == debate.Team1.ID)
                                debates[i].Team2 = debate.Team1;
                            if (debates[i].Team1.ID == debate.Team2.ID)
                                debates[i].Team1 = debate.Team2;
                            else if (debates[i].Team2.ID == debate.Team2.ID)
                                debates[i].Team2 = debate.Team2;
                        }

                    }
                    else if (team1Score >= 0 || team2Score >= 0) //If this runs then both teams were not assigned a valid score and only one of them was.
                    {
                        
                        ErrorLabel.Text = "Both teams must be scored. Check Debate " + id;
                        ErrorLabel.Visible = true;
                        break; //Stops the loop so the user can fix the information before submitting the rest.
                    }
                    else //There was no score at all.
                    {
                        if (debate.Team1Score >= 0 && debate.Team2Score >= 0)
                        {
                            debate.Team1Score = team1Score;
                            debate.Team2Score = team2Score;

                            bool result = DatabaseHandler.UpdateDebate(Session, debate);
                            for (int i = 0; i < debates.Count; i++)
                            {
                                if (debates[i].Team1.ID == debate.Team1.ID)
                                    debates[i].Team1 = debate.Team1;
                                else if (debates[i].Team2.ID == debate.Team1.ID)
                                    debates[i].Team2 = debate.Team1;
                                if (debates[i].Team1.ID == debate.Team2.ID)
                                    debates[i].Team1 = debate.Team2;
                                else if (debates[i].Team2.ID == debate.Team2.ID)
                                    debates[i].Team2 = debate.Team2;
                            }

                        }
                    }

                }

                //Now we determine if there is any ties and if so we must generate a new match for each tie.
                if (DatabaseHandler.DebateSeasonScored(debateSeasonID))
                {
                    DebateSeason season = DatabaseHandler.GetDebateSeason(debateSeasonID);
                    List<Team> teams = season.Teams;
                    List<Team> rankedTeams = Help.RankTeams(teams);
                    Debate tieBreaker = null;
                    DateTime tieBreakerStartDate = seasonStartDate.AddDays((seasonLength) * 7);
                    bool seasonUpdated = false;
                    for (int i = 0; i < rankedTeams.Count; i++)
                    {
                        for (int j = i + 1; j < rankedTeams.Count; j++)
                        {
                            if (rankedTeams[i].Rank == 1 && rankedTeams[j].Rank == 1)
                            {
                                seasonUpdated = true;
                                tieBreaker = new Debate(0, rankedTeams[i], rankedTeams[j], -1, -1, seasonStartDate, true);
                                int id;
                                DatabaseHandler.AddDebate(Session, tieBreaker, out id);
                                tieBreaker.ID = id;
                                season.Debates.Add(tieBreaker);
                                List<DebateDate> availableDates = new List<DebateDate>();
                                while (availableDates.Count == 0)
                                {
                                    tieBreakerStartDate = tieBreakerStartDate.AddDays(7);
                                    availableDates = Help.GetAllAvailableDates(season.Debates, tieBreaker, tieBreakerStartDate, 1);
                                }
                                tieBreaker.MorningDebate = availableDates[0].Morning;
                                tieBreaker.Date = availableDates[0].Date;
                                DatabaseHandler.UpdateDebate(Session, tieBreaker);
                            }
                        }
                    }

                    if (seasonUpdated)
                    {
                        TimeSpan length = tieBreakerStartDate - seasonStartDate;
                        season.Length = (length.Days / 7);
                        DatabaseHandler.UpdateDebateSeason(Session, season);
                    }

                }

                if (loggedUser.PermissionLevel == 2)
                    Response.Redirect(Request.RawUrl);
                else
                {
                    //Do nothing because you should be the Super.
                    Help.ForcePostBack(this); //We force a post back to remove the reschedule button on scored debates.
                }
            }
        }
    }
}