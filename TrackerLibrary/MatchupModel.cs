using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary
{
    public class MatchupModel
    {
        /// <summary>
        /// Represents the list of teams which are participating in the matchup.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// Represents the matchup winner.
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represents the matchup round.
        /// </summary>
        public int MatchupRound { get; set; }
    }
}
