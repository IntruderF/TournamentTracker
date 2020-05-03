using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Represents the team's list of members.
        /// </summary>

        public List<PersonModel> teamMembers { get; set; } = new List<PersonModel>();
        /// <summary>
        /// Represents the team name.
        /// </summary>
        public string teamName { get; set; }
    }
}
