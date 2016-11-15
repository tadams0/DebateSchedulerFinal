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
        private OrderBy order = OrderBy.Ascending;
        private TeamOrderVar teamOrder = TeamOrderVar.Rank;
        private List<Team> teams;

        protected void Page_Load(object sender, EventArgs e)
        {
            int currentDebateID = Help.GetDebateSeasonID(Application);

            if (currentDebateID != -1)
            {
                //Gathering query values
                NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                string orderString = queryValues.Get("Order");
                string teamOrderString = queryValues.Get("OrderTeams");

                if (orderString != null)
                {
                    order = (OrderBy)(int.Parse(orderString));
                }
                if (teamOrderString != null)
                {
                    teamOrder = (TeamOrderVar)(int.Parse(teamOrderString));
                }

                teams = DatabaseHandler.GetDebateSeasonTeams(currentDebateID);
                teams = Help.RankTeams(teams);

                teams = Help.OrderTeams(order, teamOrder, teams);

                TableRow header = CreateHeaderRow();
                Table1.Rows.Add(header);

                for (int i = 0; i < teams.Count; i++)
                {
                    TableRow teamRow = Help.CreateTeamRow(teams[i], i);
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

        

    }
}