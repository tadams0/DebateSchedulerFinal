﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateSchedulerFinal
{
    public class TeamPair : Debate
    {
        public Guid PairID
        {
            get;
            private set;
        }

        public TeamPair(int id, Team team1, Team team2, int team1Score, int team2Score, DateTime date, bool morningDate, Guid pairid) : base(id, team1, team2, team1Score, team2Score, date, morningDate)
        {
            PairID = pairid;
        }

        

    }
}