using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace DebateSchedulerFinal
{
    /// <summary>
    /// Defines many helpful methods used to interact with the session and other areas of the application.
    /// </summary>
    public static class Help
    {
        private static readonly int MaxMorningsPerDay = 3;
        private static readonly int MaxAfternoonsPerDay = 2;
        private static readonly int MaximumTeams = 10;
        private static readonly int MaxTeamNameLength = 50;
        private static readonly int MinTeamNameLength = 3;
        private static readonly int UserCodeLength = 10;
        private static readonly string DateFormat = "O";
        public static readonly string scheduleURL = "Schedule.aspx";
        public static readonly string debateCreatorURL = "DebateCreator.aspx";
        public static readonly string defaultURL = "Default.aspx";
        public static readonly string letters = "abcdefghijklmnopqrstuvwxyz"; //ABCDEFGHIJKLMNOPQRSTUVWXYZ capitalized letters were removed for simpler codes.
        public static readonly string numbers = "1234567890";
        public static readonly string specialCharacters = "!@#$%^&*?/";
        public static readonly Random rand = new Random();


        /// <summary>
        /// Gets a random letter, this can be lower or upper case.
        /// </summary>
        /// <param name="amount">The amount of random letters to get.</param>
        /// <returns>Returns a string with the random letters.</returns>
        public static string GetRandomLetters(int amount)
        {
            StringBuilder strBuilder = new StringBuilder(amount);
            for (int i = 0; i < amount; i++)
            {
                strBuilder.Append(letters[rand.Next(0, letters.Length)]);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Gets a random number.
        /// </summary>
        /// <param name="amount">The amount of random numbers to get.</param>
        /// <returns>Returns a string with the random numbers.</returns>
        public static string GetRandomNumbers(int amount)
        {
            StringBuilder strBuilder = new StringBuilder(amount);
            for (int i = 0; i < amount; i++)
            {
                strBuilder.Append(numbers[rand.Next(0, numbers.Length)]);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Gets a random sepcial character.
        /// </summary>
        /// <param name="amount">The number of special characters to return.</param>
        /// <returns>Returns a string with the random special characters in it.</returns>
        public static string GetRandomSpecialCharacter(int amount)
        {
            StringBuilder strBuilder = new StringBuilder(amount);
            for (int i = 0; i < amount; i++)
            {
                strBuilder.Append(specialCharacters[rand.Next(0, specialCharacters.Length)]);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Generates a random user code, does not add it to the database.
        /// </summary>
        /// <returns>Returns a string with a generated user code.</returns>
        public static string GenerateUserCode()
        {
            StringBuilder strBuilder = new StringBuilder(UserCodeLength);
            bool character = true;
            for (int i = 0; i < UserCodeLength; i++)
            {
                if (rand.Next(0, 2) == 0)
                    character = true;
                else
                    character = false;

                if (character)
                    strBuilder.Append(GetRandomLetters(1));
                else
                    strBuilder.Append(GetRandomNumbers(1));
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Orders a list of debates by a given variable in a given order and returns the resulting list.
        /// </summary>
        /// <param name="order">The order of the returning list.</param>
        /// <param name="variable">The variable to order by.</param>
        /// <param name="debates">The list of debates being ordered.</param>
        /// <returns>Returns a list of debates properly ordered by the given variables.</returns>
        public static List<Debate> OrderDebates(OrderBy order, DebateOrderVar variable, List<Debate> debates)
        {

            switch (variable)
            {
                default:
                    if (order == OrderBy.Ascending)
                        return debates.OrderBy(o => o.Date).ToList();
                    else
                        return debates.OrderByDescending(o => o.Date).ToList();

                case DebateOrderVar.Team1Name:
                    {
                        int maxLength = debates.Max(o => o.Team1.Name.Length);
                        if (order == OrderBy.Ascending)
                            return debates.OrderBy(o => o.Team1.Name.PadLeft(maxLength, '0')).ToList(); //Not the most effecient way of sorting, but whatever it works.
                        else
                            return debates.OrderByDescending(o => o.Team1.Name.PadLeft(maxLength, '0')).ToList(); //Not the most effecient way of sorting, but whatever it works.
                    }
                case DebateOrderVar.Team2Name:
                    {
                        int maxLength = debates.Max(o => o.Team2.Name.Length);
                        if (order == OrderBy.Ascending)
                            return debates.OrderBy(o => o.Team2.Name.PadLeft(maxLength, '0')).ToList(); //Not the most effecient way of sorting, but whatever it works.
                        else
                            return debates.OrderByDescending(o => o.Team2.Name.PadLeft(maxLength, '0')).ToList(); //Not the most effecient way of sorting, but whatever it works.
                    }
                case DebateOrderVar.Team1Score:
                    if (order == OrderBy.Ascending)
                        return debates.OrderBy(o => o.Team1Score).ToList();
                    else
                        return debates.OrderByDescending(o => o.Team1Score).ToList();

                case DebateOrderVar.Team2Score:
                    if (order == OrderBy.Ascending)
                        return debates.OrderBy(o => o.Team2Score).ToList();
                    else
                        return debates.OrderByDescending(o => o.Team2Score).ToList();

                case DebateOrderVar.MorningDebate:
                    if (order == OrderBy.Ascending)
                        return debates.OrderBy(o => o.MorningDebate).ToList();
                    else
                        return debates.OrderByDescending(o => o.MorningDebate).ToList();
            }

        }

        /// <summary>
        /// Orders a list of teams by a given variable then returns the resulting list.
        /// </summary>
        /// <param name="order">The order to sort by.</param>
        /// <param name="variable">The variable to sort by.</param>
        /// <param name="teams">The list of teams.</param>
        public static List<Team> OrderTeams(OrderBy order, TeamOrderVar variable, List<Team> teams)
        {
            switch (variable)
            {
                default:
                    {
                        int maxLength = teams.Max(o => o.Name.Length);
                        if (order == OrderBy.Ascending)
                            return teams.OrderBy(o => o.Name.PadLeft(maxLength, '0')).ToList();
                        else
                            return teams.OrderByDescending(o => o.Name.PadLeft(maxLength, '0')).ToList();
                    }
                case TeamOrderVar.Wins:
                    if (order == OrderBy.Ascending)
                        return teams.OrderBy(o => o.Wins).ToList();
                    else
                        return teams.OrderByDescending(o => o.Wins).ToList();

                case TeamOrderVar.Ties:
                    if (order == OrderBy.Ascending)
                        return teams.OrderBy(o => o.Ties).ToList();
                    else
                        return teams.OrderByDescending(o => o.Ties).ToList();

                case TeamOrderVar.Losses:
                    if (order == OrderBy.Ascending)
                        return teams.OrderBy(o => o.Losses).ToList();
                    else
                        return teams.OrderByDescending(o => o.Losses).ToList();

                case TeamOrderVar.TotalScore:
                    if (order == OrderBy.Ascending)
                        return teams.OrderBy(o => o.TotalScore).ToList();
                    else
                        return teams.OrderByDescending(o => o.TotalScore).ToList();
                case TeamOrderVar.Rank:
                    if (order == OrderBy.Ascending)
                        return teams.OrderBy(o => o.Rank).ToList();
                    else
                        return teams.OrderByDescending(o => o.Rank).ToList();
            }
            

        }

        /// <summary>
        /// Gets the current debate season that is ongoing, or returns null if there isn't an ongoing season.
        /// </summary>
        /// <param name="application">The application variable, "Application" should nromally do it.</param>
        /// <returns>Returns null if there is no current debate season or an error occured getting the season from the database., otherwise returns a debate season.</returns>
        public static DebateSeason GetCurrentDebateSeason(HttpApplicationState application)
        {
            int debateID = GetDebateSeasonID(application);
            if (debateID != -1)
                return DatabaseHandler.GetDebateSeason(debateID);
            else
                return null;
        }

        /// <summary>
        /// Gets the debate id currently ongoing in the application.
        /// </summary>
        /// <param name="application">The application's state.</param>
        /// <returns>Returns -1 if no debate is ongoing, otherwise returns an id greator than -1.</returns>
        public static int GetDebateSeasonID(HttpApplicationState application)
        {
            object obj = application.Get("SeasonID");
            if (obj != null)
            {
                return (int)obj;
            }

            return -1;
        }

        /// <summary>
        /// Sets the id of the ongoing debate. Use -1 if there is no ongoing debate.
        /// </summary>
        /// <param name="application">The application state.</param>
        /// <param name="id">The id of the debate currently active.</param>
        public static void SetDebateID(HttpApplicationState application, int id)
        {
            if (application.Get("SeasonID") != null)
                application.Set("SeasonID", id);
            else
                application.Add("SeasonID", id);
        }

        /// <summary>
        /// Gets an integer that represents the maximum number of teams allowed in the scheduler.
        /// </summary>
        /// <returns>Returns an integer value representing the maximum teams allowed.</returns>
        public static int GetMaximumTeams()
        {
            return MaximumTeams;
        }

        /// <summary>
        /// Gets an integer that represents the maximum number of characters a team name can contain.
        /// </summary>
        public static int GetMaximumTeamNameSize()
        {
            return MaxTeamNameLength;
        }

        /// <summary>
        /// Gets an integer that represents the minimum number of characters a team name can contain.
        /// </summary>
        public static int GetMinimumTeamNameSize()
        {
            return MinTeamNameLength;
        }

        /// <summary>
        /// Gets the user object from the current session.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <returns>Returns a user object representative of the current user session. This will be null if there is no session.</returns>
        public static User GetUserSession(HttpSessionState session)
        {
            return session["User"] as User;
        }

        /// <summary>
        /// Adds a user object to the current session/
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="user">The user object the session will hold onto.</param>
        public static void AddUserSession(HttpSessionState session, User user)
        {
            session.Add("User", user);
        }

        /// <summary>
        /// Ends the current session.
        /// </summary>
        /// <param name="session">The session to clear.</param>
        public static void EndSession(HttpSessionState session)
        {
            session.RemoveAll();
        }

        /// <summary>
        /// Gets the name of the permission level based on the int value.
        /// </summary>
        /// <param name="permissionLevel">The permission level whose name will be returned.</param>
        /// <returns>Returns a string representation of the given permission level.</returns>
        public static string GetPermissionName(int permissionLevel)
        {
            switch (permissionLevel)
            {
                default: return "Regular User";
                case 2: return "Referee";
                case 3: return "Super Referee";
            }
        }

        /// <summary>
        /// Gets the int value of a permission level based on the name.
        /// </summary>
        /// <param name="permissionName">The name of the permission level.</param>
        /// <returns>Returns a number that represents the level of permissions the given name has.</returns>
        public static int GetPermissionLevel(string permissionName)
        {
            switch (permissionName)
            {
                default: return 0;
                case "Referee": return 2;
                case "Super Referee": return 3;
            }
        }

        /// <summary>
        /// Gets a string representation of a given date in a specific format type.
        /// </summary>
        /// <param name="date">The date to turn into a string.</param>
        /// <returns>Returns a string representation of the date.</returns>
        public static string GetDateString(DateTime date)
        {
            return date.ToString(DateFormat);
        }

        /// <summary>
        /// Gets a date from a given string.
        /// </summary>
        /// <param name="date">The string representation of the date.</param>
        /// <returns>Returns a date time parsed back from a string.</returns>
        public static DateTime GetDate(string date)
        {
            return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Gets a list of saturdays between two dates.
        /// </summary>
        /// <param name="StartDate">The DateTime that marks the beginning of the season.</param>
        /// <param name="EndDate">The DateTime that marks the end of the season.</param>
        /// <returns>Returns a list of every between the beginning and end of the season.</returns>
        public static List<DateTime> SatBetween(DateTime StartDate, DateTime EndDate)
        {
            if (StartDate.DayOfWeek != DayOfWeek.Saturday)
            {
                while (StartDate.DayOfWeek != DayOfWeek.Saturday)
                {
                    StartDate.AddDays(1);
                }
            }
            if (EndDate.DayOfWeek != DayOfWeek.Saturday)
            {
                while (EndDate.DayOfWeek != DayOfWeek.Saturday)
                {
                    EndDate.AddDays(-1);
                }

            }
            List<DateTime> dateList = new List<DateTime>();
            do
            {
                dateList.Add(StartDate);
                StartDate = StartDate.AddDays(7);
            } while (StartDate <= EndDate);
            return (dateList);
        }

        /// <summary>
        /// Gets the first available date found in the list of date pairs.
        /// </summary>
        /// <param name="datePairs">An array of lists where each list represents a new date.</param>
        /// <param name="d">The debate trying to be added to the date pairs.</param>
        /// <param name="listIndex">The index at which the debate can be added.</param>
        /// <param name="morning">Whether or not the debate added needs to be morning or afternoon.</param>
        /// <returns>Returns true if there was an available date, false otherwise.</returns>
        public static bool GetAvailableDate(List<Debate>[] datePairs, Debate d, out int listIndex, out bool morning)
        {
            return GetAvailableDate(datePairs, d, 0, datePairs.Length, out listIndex, out morning);
        }

        /// <summary>
        /// Gets the first available date found in the list of datepairs starting at a certain index.
        /// </summary>
        /// <param name="datePairs">An array of lists where each list represents a new date.</param>
        /// <param name="d">The debate trying to be added to the date pairs.</param>
        /// <param name="startIndex">The index at which the loop begins, it will end at this index as well once it loops around.</param>
        /// <param name="listIndex">The index at which the debate can be added.</param>
        /// <param name="morning">Whether or not the debate added needs to be morning or afternoon.</param>
        /// <returns>Returns true if there was an available date, false otherwise.</returns>
        public static bool GetAvailableDate(List<Debate>[] datePairs, Debate d, int startIndex, out int listIndex, out bool morning)
        {
            bool result = GetAvailableDate(datePairs, d, startIndex, datePairs.Length, out listIndex, out morning);
            if (result)
                return result;
            else
            {
                return GetAvailableDate(datePairs, d, 0, startIndex, out listIndex, out morning); 
            }
        }

        /// <summary>
        /// Gets the first available date found in the list of date pairs.
        /// </summary>
        /// <param name="datePairs">An array of lists where each list represents a new date.</param>
        /// <param name="d">The debate trying to be added to the date pairs.</param>
        /// <param name="startIndex">The index at which the loop begins.</param>
        /// <param name="endIndex">The end at which the loop stops.</param>
        /// <param name="listIndex">The index at which the debate can be added.</param>
        /// <param name="morning">Whether or not the debate added needs to be morning or afternoon.</param>
        /// <returns>Returns true if there was an available date, false otherwise.</returns>
        public static bool GetAvailableDate(List<Debate>[] datePairs, Debate d, int startIndex, int endIndex, out int listIndex, out bool morning)
        {
            int maxDebatesPerDay = MaxMorningsPerDay + MaxAfternoonsPerDay;
            listIndex = -1;
            morning = true;

            for (int k = startIndex; k < endIndex; k ++) // each (List<Debate> debateDay in datePairs)
            {
                List<Debate> debateDay = datePairs[k]; //The current day being looked at.
                
                if (debateDay.Count == 0) //If there is no debate on this day then we go ahead and use this day.
                {
                    listIndex = k;
                    return true;
                }
                else if (debateDay.Count < maxDebatesPerDay) //If there is an available day...
                {
                    bool morningTaken = false;
                    bool afternoonTaken = false;
                    bool conflict = false;
                    for (int i = 0; i < debateDay.Count; i++) //We go through each debate on this day..
                    {
                        Team team1 = debateDay[i].Team1; //Get the team 1
                        Team team2 = debateDay[i].Team2; //Get the team 2
                        if (team1.ID == d.Team1.ID //We check if the ids between team 1 and 2 on both debates conflict
                            || team1.ID == d.Team2.ID
                            || team2.ID == d.Team1.ID
                            || team2.ID == d.Team2.ID)
                        {
                            if (debateDay[i].MorningDebate) //We log whether it's a morning or afternoon that is taken.
                                morningTaken = true;
                            else
                                afternoonTaken = true;
                        }

                        if (morningTaken && afternoonTaken) //Once both are taken we consider this day impossible to be free.
                        {
                            conflict = true;
                            break; //And so we move on to the next day.
                        }
                    }

                    if (!conflict) //If there was no conflict (and so either morning or afternoon is available)
                    {
                        if (morningTaken) //We check if the morning is impossible
                        {
                            morning = false; //We assign an afternoon time.
                        }
                        else if (afternoonTaken) //But if it was the afternoon that is impossible
                        {
                            morning = true; //We assign the morning.
                        }
                        listIndex = k;
                        return true;
                    }

                }
            }

            return false;
        }

        /// <summary>
        /// creates every match for the season.
        /// </summary>
        /// <param name="Saturdays">The list of Saturdays in a season.</param>
        /// <param name="teamList">The list of teams in a season.</param>
        /// <returns>Returns a list of debates for the season.</returns>
        public static List<Debate> MatchMake(List<DateTime> Saturdays, List<Team> teamList)
        {
            //Assigning the pairs
            int n = teamList.Count;
            List<TeamPair> teamPairs = new List<TeamPair>((n * (n - 1)) / 2);
            for (int i = 0; i < teamList.Count; i++)
            {
                for (int j = i + 1; j < teamList.Count; j++)
                {
                    teamPairs.Add(new TeamPair(i, teamList[i], teamList[j], 0, 0, DateTime.Today, true, Guid.NewGuid()));
                }
            }

            teamPairs = teamPairs.OrderBy(a => a.PairID).ToList(); //We shuffle the list of team pairs.

            List<Debate>[] datePairs = new List<Debate>[Saturdays.Count];
            for (int i = 0; i < datePairs.Length; i++)
            {
                datePairs[i] = new List<Debate>(); //We initialize a new list for each debate day.
            }

            int startIndex = 0;
            //We now find places for the debates.
            foreach (Debate d in teamPairs)
            {
                bool morning;
                int index;
                bool result = GetAvailableDate(datePairs, d, startIndex, out index, out morning); //This returns an available slot the debate can be placed at.
                if (result) //There was an available slot.
                {
                    d.MorningDebate = morning; //We set whether it's a morning or afternoon slot.
                    d.Date = Saturdays[index]; //We set the date.
                    datePairs[index].Add(d); //We then add the debate to the list so future debates being added are influenced.
                    startIndex++;
                    if (startIndex >= datePairs.Length)
                        startIndex = 0;
                }
                else //There was not an available slot.
                {
                    //This is a problem... this is an impossible match up, too many teams or too few weeks.
                }
            }

            //We compile the debates into a single dimensional list.
            List<Debate> finalList = new List<Debate>((n * (n - 1)) / 2);
            foreach (List<Debate> pairList in datePairs)
            {
                foreach (Debate d in pairList)
                {
                    finalList.Add(d);
                }
            }

            return finalList;
        }

        /// <summary>
        /// Orders a list of teams by their wins and total score.
        /// </summary>
        /// <param name="teams">The list of teams.</param>
        /// <returns>Returns an ordered list.</returns>
        public static List<Team> RankTeams(List<Team> teams)
        {
            List<Team> result = teams.OrderBy(o => o.Wins).ThenBy(o => o.TotalScore).ToList(); //We order the list by their wins, then by their total score.
            result.Reverse(); //We reverse the list (so that the larger numbers are ranked toward the top).

            int currentRank = 1; //We begin by setting the smallest rank to 1.
            for (int i = 0; i < result.Count - 1; i++) //We skip the last result because there is no result after that so result[i + 1] will not work.
            {
                result[i].Rank = currentRank; //We assign the current rank to this team.
                if (result[i].Wins != result[i + 1].Wins ||
                        result[i].TotalScore != result[i + 1].TotalScore) //If the wins or the total score is not the same.
                {
                    currentRank++; //We increment the rank.
                }
            }

            //Now we order the last item whose rank has already been incremented in the loop above.
            result[result.Count - 1].Rank = currentRank;
            
            return result;
        }

        /// <summary>
        /// This writes a script, embeds it into the client's html and runs it causing a post back to occur (thus refreshing the page's contents).
        /// </summary>
        public static void ForcePostBack(System.Web.UI.Page page)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("<script>\n");
            sbScript.Append(page.ClientScript.GetPostBackEventReference(page, "PBArg") + ";\n");
            sbScript.Append("</script>\n");
            page.ClientScript.RegisterStartupScript(page.GetType(),"AutoPostBackScript", sbScript.ToString());
        }

    }
}