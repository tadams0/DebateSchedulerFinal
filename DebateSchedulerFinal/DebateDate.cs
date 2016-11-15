using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateSchedulerFinal
{
    /// <summary>
    /// Defines an object that holds date information and a boolean for when the debate should take place.
    /// </summary>
    public class DebateDate
    {
        /// <summary>
        /// The date the debate should take place on.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// If true the date given is for the morning, if false it is for the afternoon.
        /// </summary>
        public bool Morning { get; set; }

        /// <summary>
        /// Creates a debate date object.
        /// </summary>
        /// <param name="date">The date the debate should take place on.</param>
        /// <param name="morning">Whether or not the debate takes place in the morning (true) or afternoon (false).</param>
        public DebateDate(DateTime date, bool morning)
        {
            Date = date;
            Morning = morning;
        }
    }
}