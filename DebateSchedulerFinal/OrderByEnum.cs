﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateSchedulerFinal
{
    public enum OrderBy
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }

    public enum TeamOrderVar
    {
        Name = 0,
        Wins = 1,
        Losses = 2,
        Ties = 3,
        TotalScore = 4,
        Rank = 5
    }

    public enum DebateOrderVar
    {
        Team1Score = 0,
        Team2Score = 1,
        Date = 2,
        MorningDebate = 3,
        Team1Name = 4,
        Team2Name = 5
    }


}