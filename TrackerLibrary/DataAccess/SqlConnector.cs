using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";

        // TODO -> Make the CreatePrize method actually save to the database
        /// <summary>
        /// Saves a new prize to the database.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                return model;
            }
        }

        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellphoneNumber);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                return model;
            }
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var t = new DynamicParameters();
                t.Add("@TeamName", model.TeamName);
                t.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", t, commandType: CommandType.StoredProcedure);

                model.Id = t.Get<int>("@id");

                foreach (PersonModel tm in model.TeamMembers)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", tm.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

                return model;
            }
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentTeams(connection, model);

                SaveTournamentRounds(connection, model);
            }
        }

        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var t = new DynamicParameters();
            t.Add("@TournamentName", model.TournamentName);
            t.Add("@EntryFee", model.EntryFee);
            t.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", t, commandType: CommandType.StoredProcedure);

            model.Id = t.Get<int>("@id");
        }

        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pm in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pm.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentTeams(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var t = new DynamicParameters();
                t.Add("@TournamentId", model.Id);
                t.Add("@TeamId", tm.Id);
                t.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", t, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var m = new DynamicParameters();
                    m.Add("@TournamentId", model.Id);
                    m.Add("@MatchupRound", matchup.MatchupRound);
                    m.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", m, commandType: CommandType.StoredProcedure);

                    matchup.Id = m.Get<int>("@id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        var me = new DynamicParameters();
                        me.Add("@MatchupId", matchup.Id);
                        
                        if (entry.ParentMatchup == null)
                        {
                            me.Add("@ParentMatchupId", null);
                        }
                        else
                        {
                            me.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                        }
                        
                        if (entry.TeamCompeting == null)
                        {
                            me.Add("@TeamCompetingId", null);
                        }
                        else
                        {
                            me.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        
                        me.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchupEntries_Insert", me, commandType: CommandType.StoredProcedure);

                        entry.Id = me.Get<int>("@id");
                    }
                }
            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

                foreach (TournamentModel t in output)
                {
                    var tId = new DynamicParameters();
                    tId.Add("@TournamentId", t.Id);

                    // Populate prizes
                    t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", tId, commandType: CommandType.StoredProcedure).ToList();

                    // Populate teams
                    t.EnteredTeams = connection.Query<TeamModel>("dbo.spTeam_GetByTournament", tId, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        var p = new DynamicParameters();
                        p.Add("@TeamId", team.Id);

                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }

                    
                    // Populate rounds
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", tId, commandType: CommandType.StoredProcedure).ToList();

                    foreach (MatchupModel m in matchups)
                    {
                        var p = new DynamicParameters();
                        p.Add("@MatchupId", m.Id);

                        m.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        List<TeamModel> allTeams = GetTeam_All();

                        foreach (var me in m.Entries)
                        {
                            if (me.TeamCompetingId > 0)
                            {
                                me.TeamCompeting = allTeams.Where(x => x.Id == me.TeamCompetingId).First();
                            }

                            if (me.ParentMatchupId > 0)
                            {
                                me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
                            }
                        }
                    }

                    List<MatchupModel> currRow = new List<MatchupModel>();
                    int currRound = 1;

                    foreach (MatchupModel m in matchups)
                    {
                        if (m.MatchupRound > currRound)
                        {
                            t.Rounds.Add(currRow);
                            currRow = new List<MatchupModel>();
                            currRound += 1;
                        }

                        currRow.Add(m);
                    }

                    t.Rounds.Add(currRow);
                }
            }

            return output;
        }
    }
}