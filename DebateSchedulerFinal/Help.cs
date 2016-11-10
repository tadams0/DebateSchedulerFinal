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
        private static readonly int MaximumTeams = 10;
        private static readonly int MaxTeamNameLength = 50;
        private static readonly int MinTeamNameLength = 3;
        private static readonly int UserCodeLength = 10;
        private static readonly string DateFormat = "O";
        public static readonly string scheduleURL = "Schedule.aspx";
        public static readonly string debateCreatorURL = "DebateCreator.aspx";
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

        private static void FindConflict(List<TeamPair>[] pairs, out int pairIndex, out int teamPairIndex)
        {
            pairIndex = -1;
            teamPairIndex = -1;
            for (int i  = 0; i < pairs.Length; i++)
            {
                List<TeamPair> datePair = pairs[i];

                for (int j = 0; j < datePair.Count; j++)
                {
                    for (int k = 0; k < datePair.Count; k++)
                    {
                        if (k != j)
                        {
                            if (datePair[j].MorningDebate == datePair[k].MorningDebate)
                            if (datePair[j].Team1.ID == datePair[k].Team1.ID ||
                                datePair[j].Team1.ID == datePair[k].Team2.ID ||
                                datePair[j].Team2.ID == datePair[k].Team1.ID ||
                                datePair[j].Team2.ID == datePair[k].Team2.ID)
                            {
                                pairIndex = i;
                                teamPairIndex = j;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static bool ReplaceConflict(List<TeamPair>[] pairs, int pairIndex, int teamPairIndex)
        {
            int maximumSize = pairs[0].Count;//test for now
            TeamPair conflictingPair = pairs[pairIndex][teamPairIndex];

            for (int i = 0; i < pairs.Length; i++)
            {
                List<TeamPair> datePair = pairs[i];
                if (datePair.Count < maximumSize)
                {
                    datePair.Add(conflictingPair);
                    pairs[pairIndex].RemoveAt(teamPairIndex);
                }
                else
                {
                    for (int j = 0; j < datePair.Count; j++)
                    {
                        //if (datePair[j].MorningDebate != conflictingPair.MorningDebate)
                        //{
                            if (datePair[j].Team1.ID == conflictingPair.Team1.ID &&
                                datePair[j].Team2.ID == conflictingPair.Team1.ID &&
                                datePair[j].Team1.ID == conflictingPair.Team2.ID &&
                                datePair[j].Team2.ID == conflictingPair.Team2.ID)
                            {
                                break;
                            }
                            else if (j == datePair.Count - 1)
                            {
                                TeamPair temp = datePair[j];
                                DateTime tempDate = datePair[j].Date;
                                datePair[j] = conflictingPair;
                                pairs[pairIndex][teamPairIndex] = temp;
                                pairs[pairIndex][teamPairIndex].Date = conflictingPair.Date;
                                datePair[j].Date = tempDate;
                                return true;
                            }
                        //}
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
        public static List<TeamPair> MatchMake(List<DateTime> Saturdays, List<Team> teamList)
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

            teamPairs = teamPairs.OrderBy(a => a.PairID).ToList();

            //Organizing the teams based on their date...
            List<TeamPair>[] datePairs = new List<TeamPair>[Saturdays.Count];
            //Assigning the dates...
            int currentSat = 0;
            for (int i = 0; i < teamPairs.Count; i++)
            {
                teamPairs[i].Date = Saturdays[currentSat];
                if (datePairs[currentSat] == null)
                    datePairs[currentSat] = new List<TeamPair>();
                datePairs[currentSat].Add(teamPairs[i]);
                currentSat++;
                if (currentSat >= Saturdays.Count)
                    currentSat = 0; 
            }

            //Assigning morning/afernoon debates alternating.
            bool morning = true;
            foreach (List<TeamPair> pairList in datePairs)
            {
                foreach (TeamPair p in pairList)
                {
                    p.MorningDebate = morning;
                    morning = !morning;
                }
            }

           //int datePairIndex = -1;
           // int pos1 = -1;
           // do
           // {
           //     FindConflict(datePairs, out datePairIndex, out pos1);
           //     if (datePairIndex != -1)
           //     {
           //         bool replaced = ReplaceConflict(datePairs, datePairIndex, pos1);
           //         if (!replaced)
           //         {
           //             datePairs[datePairIndex][pos1].MorningDebate = !datePairs[datePairIndex][pos1].MorningDebate;
           //             bool result = ReplaceConflict(datePairs, datePairIndex, pos1);
           //             if (!result)
           //                 break; //Test
           //         }
           //     }
           // } while (datePairIndex != -1);

            teamPairs.Clear();
            teamPairs = new List<TeamPair>((n * (n - 1)) / 2);
            foreach (List<TeamPair> pairList in datePairs)
            {
                foreach (TeamPair p in pairList)
                {
                    teamPairs.Add(p);
                }
            }

            //Dictionary<int, int> prevTeams = new Dictionary<int, int>();
            //int sat = 0;
            //    foreach (TeamPair tp in teamPairs)
            //    {
            //        sat = 0;
            //        int teamA = tp.Team1.ID;
            //        int teamB = tp.Team2.ID;


            //        if (teamA > 10) //This function makes sure the lists used below will function properly
            //        {
            //            teamA %= 10;
            //            if (teamA > 0)
            //                teamA -= 1;
            //        }

            //        if (teamB > 10) //This function makes sure the lists used below will function properly
            //        {
            //            teamB %= 10;
            //            if (teamB > 0)
            //                teamB -= 1;
            //        }


            //        if (prevTeams.ContainsKey(teamA))
            //            prevTeams[teamA] += 1;
            //        else
            //            prevTeams[teamA] = 1;

            //        if (prevTeams.ContainsKey(teamB))
            //            prevTeams[teamB] += 1;
            //        else
            //            prevTeams[teamB] = 1;

            //        int aVal = prevTeams[teamA] % 2;
            //        int bVal = prevTeams[teamB] % 2;
            //        if (aVal == 0)
            //            aVal = 2;
            //        if (bVal == 0)
            //            bVal = 2;

            //        if ((aVal + bVal) % 3 == 0)
            //        {
            //            tp.MorningDebate = false;
            //        }
            //        else if ((aVal + bVal) % 3 == 1)
            //        {
            //            tp.MorningDebate = false;
            //        }
            //        else if ((aVal + bVal) % 3 == 2)
            //        {
            //            tp.MorningDebate = true;
            //        }

            //        if (prevTeams[teamA] >= 3 || prevTeams[teamB] >= 3)
            //        {
            //            if (!((aVal + bVal) == 0))
            //            {
            //                if (aVal < bVal)
            //                    sat += prevTeams[teamB];
            //                else
            //                    sat += prevTeams[teamA];
            //            }
            //        }

            //        if (sat >= 10)
            //        {
            //            sat %= 10;
            //        }
            //        tp.Date = Saturdays[sat];
            //    }
            return (teamPairs);
        }

        /// <summary>
        /// Sorts a list of teams by score.
        /// </summary>
        /// <param name="matchupList">The list of matches in a season.</param>
        /// <param name="teamList">The list of teams in a season.</param>
        /// <returns>Returns an ordered list of teams from highest to lowest score.</returns>
        public static List<Team> SortScore(List<Debate> matchupList, List<Team> teamList)
        {
            foreach (TeamPair i in matchupList)
            {
                int j = 0, k = 0;
                while (j <= teamList.Count)
                {
                    if (i.Team1.ID == teamList[j].ID)
                    {
                        teamList[j].TotalScore += i.Team1Score;
                        break;
                    }
                    else
                        j++;
                }
                while (k <= teamList.Count)
                {
                    if (i.Team2.ID == teamList[k].ID)
                    {
                        teamList[k].TotalScore += i.Team2Score;
                        break;
                    }
                    else
                        j++;
                }
            }
            teamList = teamList.OrderBy(a => a.TotalScore).ToList();
            return teamList;
        }
        /// <summary>
        /// assigns wins, losses, and ties to each team.
        /// </summary>
        /// <param name="matchupList">The list of matches in a season.</param>
        /// <param name="teamList">The list of teams in a season.</param>
        /// <returns>Returns a list of teams with updated wins, losses, and ties.</returns>
        public static List<Team> AssignResults(List<Debate> matchupList, List<Team> teamList)
        {
            foreach (TeamPair i in matchupList)
            {
                if (i.Team1Score > i.Team2Score)
                {
                    int j = 0, k = 0;
                    while (j <= teamList.Count)
                    {
                        if (i.Team1.ID == teamList[j].ID)
                        {
                            teamList[j].Wins++;
                            break;
                        }
                        else
                            j++;
                    }
                    while (k <= teamList.Count)
                    {
                        if (i.Team2.ID == teamList[k].ID)
                        {
                            teamList[k].Losses++;
                            break;
                        }
                        else
                            j++;
                    }
                }

                else if (i.Team1Score < i.Team2Score)
                {
                    int j = 0, k = 0;
                    while (j <= teamList.Count)
                    {
                        if (i.Team1.ID == teamList[j].ID)
                        {
                            teamList[j].Losses++;
                            break;
                        }
                        else
                            j++;
                    }
                    while (k <= teamList.Count)
                    {
                        if (i.Team2.ID == teamList[k].ID)
                        {
                            teamList[k].Wins++;
                            break;
                        }
                        else
                            j++;
                    }
                }
                else if (i.Team1Score == i.Team2Score)
                {
                    int j = 0, k = 0;
                    while (j <= teamList.Count)
                    {
                        if (i.Team1.ID == teamList[j].ID)
                        {
                            teamList[j].Ties++;
                            break;
                        }
                        else
                            j++;
                    }
                    while (k <= teamList.Count)
                    {
                        if (i.Team2.ID == teamList[k].ID)
                        {
                            teamList[k].Ties++;
                            break;
                        }
                        else
                            j++;
                    }
                }
            }
            return teamList;
        }
        /// <summary>
        /// Determines who wins based on score and wins.
        /// </summary>
        /// <param name="matchupList">The list of matches in a season.</param>
        /// <param name="teamList">The list of teams in a season.</param>
        /// <returns>Returns a list of teams ordered primarily by scores and secondarily by wins</returns>
        public static List<Team> WhoWins(List<Debate> matchupList,ref List<Team> teamList)
        {
            AssignResults(matchupList,teamList);
            SortScore(matchupList, teamList);
            for (int i = 0; i <= teamList.Count; i++)
            {
                if (teamList[i].TotalScore == teamList[i+1].TotalScore)
                    if (teamList[i+1].Wins > teamList[i].Wins)
                    {
                        Team temp = teamList[i];
                        teamList[i] = teamList[i + 1];
                        teamList[i + 1] = temp;
                    }

            }
            return teamList;
        }

        /// <summary>
        /// Orders a list of teams by their wins and total score.
        /// </summary>
        /// <param name="teams">The list of teams.</param>
        /// <returns>Returns an ordered list.</returns>
        public static List<Team> RankTeams(List<Team> teams)
        {
            List<Team> result = teams.OrderBy(o => o.Wins).ThenBy(o => o.TotalScore).ToList();
            result.Reverse();

            int currentRank = 1;
            for (int i = 0; i < result.Count - 1; i++)
            {
                result[i].Rank = currentRank;
                if (result[i].Wins != result[i + 1].Wins ||
                        result[i].TotalScore != result[i + 1].TotalScore) //If the wins or the total score is not the same.
                {
                    currentRank++;
                }
            }

            //Now we order the last item
            result[result.Count - 1].Rank = currentRank;


            return result;
        }

    }
}