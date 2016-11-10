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
    public partial class WebForm2 : System.Web.UI.Page
    {
        private const int nameCellWidth = 250;
        private const int statsCellWidth = 90;

        private readonly Color headerTableColor = Color.CornflowerBlue;
        private readonly Color headerTableTextColor = Color.White;

        private OrderBy order = OrderBy.Ascending;
        private TeamOrderVar teamOrder = TeamOrderVar.Name;

        protected void Page_Load(object sender, EventArgs e)
        {
            int currentDebateID = Help.GetDebateSeasonID(Application);

            if (currentDebateID != -1)
            {
                //Gathering query values
                NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                string orderString = queryValues.Get("Order");
                string teamOrderString = queryValues.Get("TeamOrder");

                if (orderString != null)
                {
                    order = (OrderBy)(int.Parse(orderString));
                }
                if (teamOrderString != null)
                {
                    teamOrder = (TeamOrderVar)(int.Parse(teamOrderString));
                }

                List<Team> teams = DatabaseHandler.GetDebateSeasonTeams(currentDebateID);

                teams = Help.OrderTeams(order, teamOrder, teams);

                TableRow header = CreateHeaderRow();
                Table1.Rows.Add(header);

                for (int i = 0; i < teams.Count; i++)
                {
                    TableRow teamRow = CreateTeamRow(teams[i], i);
                    Table1.Rows.Add(teamRow);
                }
            }
            else
            {
                Panel_NoDebates.Visible = true;
                Panel2.Visible = true;
            }

        }

        private TableRow CreateHeaderRow()
        {
            TableRow row = new TableRow();

            TableCell rankCell = new TableCell();
            TableCell nameCell = new TableCell();
            TableCell winCell = new TableCell();
            TableCell lossCell = new TableCell();
            TableCell tieCell = new TableCell();
            TableCell totalScore = new TableCell();

            rankCell.BackColor = headerTableColor;
            nameCell.BackColor = headerTableColor;
            winCell.BackColor = headerTableColor;
            lossCell.BackColor = headerTableColor;
            tieCell.BackColor = headerTableColor;
            totalScore.BackColor = headerTableColor;

            LinkButton rankButton = new LinkButton();
            rankButton.Command += RankButton_Command;
            rankButton.ForeColor = headerTableTextColor;
            rankButton.Text = "Rank";

            LinkButton nameButton = new LinkButton();
            nameButton.Command += NameButton_Command;
            nameButton.ForeColor = headerTableTextColor;
            nameButton.Text = "Name";

            LinkButton winButton = new LinkButton();
            winButton.Command += WinButton_Command;
            winButton.ForeColor = headerTableTextColor;
            winButton.Text = "Wins";

            LinkButton lossButton = new LinkButton();
            lossButton.Command += LossButton_Command;
            lossButton.ForeColor = headerTableTextColor;
            lossButton.Text = "Losses";

            LinkButton tieButton = new LinkButton();
            tieButton.Command += TieButton_Command;
            tieButton.ForeColor = headerTableTextColor;
            tieButton.Text = "Ties";

            LinkButton totalScoreButton = new LinkButton();
            totalScoreButton.Command += TotalScoreButton_Command;
            totalScoreButton.ForeColor = headerTableTextColor;
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
            throw new NotImplementedException();
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

        private void RedirectWithParameters(TeamOrderVar teamOrderVar)
        {
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            if (teamOrder == teamOrderVar && order != OrderBy.Descending) //If team order is already by name, then we flip them.
            {
                queryValues.Set("OrderTeams", ((int)teamOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Descending).ToString());
            }
            else
            {
                queryValues.Set("OrderTeams", ((int)teamOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Ascending).ToString());
            }

            string url = Request.Url.AbsolutePath;
            Response.Redirect(url + "?" + queryValues);
        }

        private TableRow CreateTeamRow(Team t, int rank )
        {
            TableRow row = new TableRow();

            TableCell rankCell = new TableCell();
            TableCell nameCell = new TableCell();
            TableCell winCell = new TableCell();
            TableCell lossCell = new TableCell();
            TableCell tieCell = new TableCell();
            TableCell totalScore = new TableCell();

            rankCell.Width = statsCellWidth;
            nameCell.Width = nameCellWidth;
            winCell.Width = statsCellWidth;
            lossCell.Width = statsCellWidth;
            tieCell.Width = statsCellWidth;
            totalScore.Width = statsCellWidth;

            rankCell.Text = rank.ToString();
            nameCell.Text = t.Name;
            winCell.Text = t.Wins.ToString();
            lossCell.Text = t.Losses.ToString();
            tieCell.Text = t.Ties.ToString();
            totalScore.Text = t.TotalScore.ToString();

            row.Cells.Add(rankCell);
            row.Cells.Add(nameCell);
            row.Cells.Add(winCell);
            row.Cells.Add(lossCell);
            row.Cells.Add(tieCell);
            row.Cells.Add(totalScore);

            return row;
        }

    }
}