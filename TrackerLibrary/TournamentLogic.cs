using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        // Order the list of teams randomly
        // Check if it is big enough - if not, add in byes - 2*2*2*2 2^4
        // Create the first round of matchups
        // Create every round after that - 8 matchups - 4 matchups - 2 matchups - 1 matchup

        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = FindNumberOfByes(rounds, randomizedTeams.Count);
        }

        //private static List<MatchupEntryModel> CreateFirstRound(int byes, List<TeamModel> teams)
        //{
        //    List<MatchupModel> output = new List<MatchupModel>();
        //    MatchupModel curr = new MatchupModel;

        //    foreach (TeamModel team in teams)
        //    {
        //        curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

        //        if (byes > 0)
        //    }

        //    return output;
        //}

        private static int FindNumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - numberOfTeams;

            return output;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }

            return output;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            List<TeamModel> randomizedTeams = new List<TeamModel>();

            return randomizedTeams;
        }
    }
}
