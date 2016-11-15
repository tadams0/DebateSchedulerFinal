using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateSchedulerFinal
{
    public partial class DebateCreator : System.Web.UI.Page
    {
        private int currentTeam = 2;
        private int seasonID = -1;
        private bool nameError = false;

        List<TextBox> textBoxes = new List<TextBox>();
        List<Label> infoLabels = new List<Label>();
        List<Team> updateTeams = new List<Team>();

        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(3);
            seasonID = Help.GetDebateSeasonID(Application);
            if (seasonID != -1)
            {
                //Creating the end debate season button.
                if (DatabaseHandler.DebateSeasonScored(seasonID))
                {
                    //The entire debate season has been scored, so let's add the option to end it.
                    Button endSeasonBut = new Button();
                    endSeasonBut.Width = 180;
                    endSeasonBut.Height = 45;
                    endSeasonBut.Text = "End Debate Season";
                    endSeasonBut.Command += EndSeasonBut_Command;
                    Panel_OngoingSeason.Controls.AddAt(Panel_OngoingSeason.Controls.Count - 1, new LiteralControl("<br /> <br />"));
                    Panel_OngoingSeason.Controls.AddAt(Panel_OngoingSeason.Controls.Count - 1, endSeasonBut);
                    Panel_OngoingSeason.Controls.AddAt(Panel_OngoingSeason.Controls.Count - 1, new LiteralControl("<br />"));
                }

                bool scoreSet = DatabaseHandler.DebateSeasonHasAScore(seasonID);
                if (scoreSet) //The debate season has a score already set
                {
                    Panel_OngoingSeason.Visible = true;
                    Panel_Main.Visible = false;
                    return;
                }
                else if (!IsPostBack)
                {
                    //We can still edit the season since no scores have been set, so let's set up the schedule generator with the info.
                    Panel_RecreateSeason.Visible = true;
                    updateTeams = DatabaseHandler.GetDebateSeasonTeams(seasonID);
                    ViewState["teams"] = updateTeams.Count.ToString();
                    if (DropDownList_Teams.SelectedValue != currentTeam.ToString())
                    {
                        DropDownList_Teams.ClearSelection();
                        DropDownList_Teams.SelectedValue = currentTeam.ToString();
                    }
                    //Setting up the season parameters
                    int length;
                    DateTime date = DatabaseHandler.GetDebateSeasonDateTime(seasonID, out length);
                    DropDownList_Weeks.ClearSelection();
                    DropDownList_Weeks.SelectedValue = length.ToString();
                    Calendar_Start.SelectedDate = date;
                }
            }
             
            string teams = ViewState["teams"] as string;
            int numbTeams = -1;
            bool result = int.TryParse(teams, out numbTeams);

            if (numbTeams <= 2)
            {
                numbTeams = 2;
            }
            if (numbTeams >= Help.GetMaximumTeams())
            {
                numbTeams = Help.GetMaximumTeams();
            }

            currentTeam = numbTeams;
            for (int i = 1; i <= currentTeam; i++)
            {
                AddTeamButton(i);
            }

            if (!IsPostBack && currentTeam > 2)
            {
                DropDownList_Teams.ClearSelection();
                DropDownList_Teams.Items.FindByValue((currentTeam.ToString())).Selected = true;
            }
            //DropDownList_Teams.Items.FindByValue((numbTeams.ToString())).Selected = true;
        }

        private void EndSeasonBut_Command(object sender, CommandEventArgs e)
        {
            //The end debate season button has been pressed, now we end the debate season.
            DatabaseHandler.EndDebateSeason(Session, seasonID);
            Help.SetDebateID(Application, -1);
            Response.Redirect(Request.RawUrl);
        }

        protected void Calendar_Start_DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.Date.DayOfWeek != DayOfWeek.Saturday || (e.Day.Date.Year == DateTime.Now.Year && e.Day.Date.DayOfYear < DateTime.Now.DayOfYear) || e.Day.IsOtherMonth)
            {
                e.Day.IsSelectable = false;
                e.Cell.Enabled = false;
                e.Cell.BackColor = Color.LightGray;
                if (e.Day.IsOtherMonth)
                {
                    e.Cell.Text = "";
                }
            }
        }

        private void AddTeamButton(int teamNumber)
        {
            Label label = new Label();
            label.Text = "Team " + teamNumber + ": ";
            TextBox textBox = new TextBox();
            textBox.Width = 225;
            if (updateTeams.Count > 0)
            {
                textBox.Text = updateTeams[teamNumber - 1].Name;
            }
            Label infoLabel = new Label();
            infoLabel.Text = "Team names cannot repeat!";
            infoLabel.ForeColor = Color.Red;
            infoLabel.Visible = false;
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 1, label);
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 1, textBox);
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 1, infoLabel);
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 1, new LiteralControl("<br />"));

            textBoxes.Add(textBox);
            infoLabels.Add(infoLabel);
        }

        private List<Team> GetTeams()
        {
            List<Team> teams = new List<Team>();

            for (int i = 0; i < textBoxes.Count; i++)
            {
                string teamName = textBoxes[i].Text;
                Team newTeam = new Team(teamName, 0, 0, 0, 0, 0);
                teams.Add(newTeam);

                if (string.IsNullOrWhiteSpace(teamName))
                {
                    ShowTeamInfoLabel("Invalid team name. A team name cannot be empty or just spaces.", Color.Red, i );
                    nameError = true;
                }
                else if (teamName.Length > Help.GetMaximumTeamNameSize())
                {
                    ShowTeamInfoLabel("Invalid team name. The team name is too long.", Color.Red, i);
                    nameError = true;
                }
                else if (teamName.Length < Help.GetMinimumTeamNameSize())
                {
                    ShowTeamInfoLabel("Invalid team name. The team name is too short, team names must contain at least " + Help.GetMinimumTeamNameSize() + " characters.", Color.Red, i);
                    nameError = true;
                }
                else
                {
                    infoLabels[i].Visible = false;
                }
            }

            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    if (teams[i].Name.ToUpperInvariant() == teams[j].Name.ToUpperInvariant())
                    {
                        nameError = true;
                        if (!infoLabels[i].Visible)
                            ShowTeamInfoLabel("This team name conflicts with another.", Color.Red, i);
                        if (!infoLabels[j].Visible)
                            ShowTeamInfoLabel("This team name conflicts with another.", Color.Red, j);
                    }
                }
            }

            return teams;
        }

        private void ShowTeamInfoLabel(string message, Color color, int controlIndex)
        {
            Label label = infoLabels[controlIndex];
            label.Text = message;
            label.ForeColor = color;
            label.Visible = true;

        }

        protected void Button_CreateSchedule_Click(object sender, EventArgs e)
        {
            int currentSeasonID = Help.GetDebateSeasonID(Application);
            bool seasonHasScore = DatabaseHandler.DebateSeasonHasAScore(currentSeasonID);

            if (currentSeasonID == -1 || !seasonHasScore) //If the current debate season is -1 or if the current season does not have a score set we can create a schedule...
            {
                bool errorOccured = false;

                List<Team> teams = GetTeams();

                if (nameError)
                {
                    Label_ScheduleError.Text = "There are errors with the info given. Some team names are invalid.";
                    Label_ScheduleError.Visible = true;
                    errorOccured = true;
                }

                if (Calendar_Start.SelectedDates.Count <= 0)
                {
                    Label_ScheduleError.Text = "There are errors with the info given. There is no start date specified.";
                    Label_ScheduleError.Visible = true;
                    errorOccured = true;
                }

                if (!errorOccured)
                {
                    Label_ScheduleError.Visible = false;
                    //Generate schedule:
                    DateTime startDate = Calendar_Start.SelectedDate;
                    int seasonLength = int.Parse(DropDownList_Weeks.SelectedValue);
                    DateTime endDate = startDate.AddDays((seasonLength - 1) * 7);

                    //Adding the teams to the database
                    for (int i = 0; i < teams.Count; i++)
                    {
                        if (updateTeams.Count > 0 && i < updateTeams.Count)
                        {
                            teams[i].ID = updateTeams[i].ID;
                            DatabaseHandler.UpdateTeam(Session, teams[i]); //We update the team rather than creating a new one since these teams already exist in the database.
                        }
                        else
                        {
                            int id;
                            DatabaseHandler.AddTeam(Session, teams[i], out id);
                            teams[i].ID = id;
                        }
                    }

                    //Creating the actual debates
                    List<DateTime> saturdays = Help.SatBetween(startDate, endDate);
                    List<Debate> debates = Help.MatchMake(saturdays, teams);

                    if (debates != null) //Creating the debates was successful.
                    {
                        foreach (Debate d in debates)
                        {
                            int assignedID;
                            d.Team1Score = -1;
                            d.Team2Score = -1;
                            DatabaseHandler.AddDebate(Session, d, out assignedID);
                            d.ID = assignedID;
                        }

                        int seasonID = currentSeasonID;
                        DebateSeason newSeason = new DebateSeason(currentSeasonID, false, teams, debates, startDate, seasonLength);
                        
                        if (currentSeasonID == -1) //If the current season ID does not exist we create a new debate season.
                            DatabaseHandler.AddDebateSeason(Session, newSeason, out seasonID);
                        else //If the current season ID is a thing, we need to update the debate season instead.
                            DatabaseHandler.UpdateDebateSeason(Session, newSeason);

                        Help.SetDebateID(Application, seasonID);

                        Response.Redirect(Help.scheduleURL);
                    }
                    else
                    {
                        foreach (Team t in teams) //We must remove all the teams added to the database since there is no possible pairing.
                            DatabaseHandler.RemoveTeam(Session, t.ID, false);

                        Label_ScheduleError.Text = "A debate pairing was not found. There are too many teams for " + seasonLength + " weeks. Increase the season length.";
                        Label_ScheduleError.Visible = true;

                    }

                }

            }
            else
            {
                Panel_RecreateSeason.Visible = false;
                Response.Redirect(Request.RawUrl); //We refresh the page so that the correct view of the page is shown.
            }

        }

        protected void DropDownList_Teams_SelectedIndexChanged(object sender, EventArgs e)
        {
            int val = int.Parse(DropDownList_Teams.SelectedValue);
            currentTeam = val;

            ViewState["teams"] = currentTeam.ToString();
            Help.ForcePostBack(this);
        }

    }
}