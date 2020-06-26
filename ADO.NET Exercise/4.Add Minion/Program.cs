using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Text;

namespace _4.Add_Minion
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(
                "Server=DESKTOP-AMLLGJ5;DataBase=MinionsDB;Integrated Security = true;");

            sqlConnection.Open();

            var minionInput = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            var minionInfo = minionInput[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            var villainsInfo = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string result = AddMinionsToDatabase(sqlConnection, minionInfo, villainsInfo);

            return result;
        }

        private static string AddMinionsToDatabase(SqlConnection sqlConnection, string[] minionInfo, string[] villainsInfo)
        {
            var output = new StringBuilder();

            string minionName = minionInfo[0];
            string minionAge = minionInfo[1];
            string minionTown = minionInfo[2];

            string villainName = villainsInfo[1];

            string townId = EnsureTownExist(sqlConnection, minionTown, output);

            string villainId = EnsureVillainExist(sqlConnection, villainName, output);

            using var insertMinionCommand = new SqlCommand(@"SELECT Id FROM EvilnessFactors AS e
                                                       WHERE e.Name = 'Evil'
                                                       INSERT INTO Minions([Name], Age, TownId)
                                                       VALUES(@minionName, @minionAge, @townId)", sqlConnection);

            insertMinionCommand.Parameters.AddRange(new[]
            {
                new SqlParameter("@minionName", minionName),
                new SqlParameter("@minionAge", minionAge),
                new SqlParameter("@townId", townId)
            });

            insertMinionCommand.ExecuteNonQuery();

            using var getMinionId = new SqlCommand(@"SELECT Id FROM Minions 
                                                     WHERE [Name] = @minionName", sqlConnection);
            getMinionId.Parameters.AddWithValue("@minionName", minionName);

            string minionId = getMinionId.ExecuteScalar().ToString();

            using var insertIntoMappingCommand = new SqlCommand(@"INSERT INTO MinionsVillains (MinionId, VillainId)
                                                                  VALUES(@minionId, @villainId)", sqlConnection);

            insertIntoMappingCommand.Parameters.AddRange(new[]
            {
                new SqlParameter("@minionId", minionId),
                new SqlParameter("@villainId", villainId)
            });

            insertIntoMappingCommand.ExecuteNonQuery();

            output.AppendLine($"Successfully added {minionName} to be minion of {villainName}");

            return output.ToString().TrimEnd();
        }

        private static string EnsureVillainExist(SqlConnection sqlConnection, string villainName, StringBuilder output)
        {
            using var getVillainIdCommand = new SqlCommand(@"SELECT Id FROM Villains WHERE [Name] = @villainName", sqlConnection);
            getVillainIdCommand.Parameters.AddWithValue("@villainName", villainName);
            string villainId = getVillainIdCommand.ExecuteScalar()?.ToString();

            if (villainId == null)
            {
                using var getFactorId = new SqlCommand(@"SELECT Id FROM EvilnessFactors AS e
                                                         WHERE e.Name = @villainName", sqlConnection);
                getFactorId.Parameters.AddWithValue("@villainName", villainName);

                string factorId = getFactorId.ExecuteScalar()?.ToString();

                using var insertVillainCommand = new SqlCommand(@"INSERT INTO Villains([Name], EvilnessFactorId)
	                                                              VALUES(@villainName, @factorId)", sqlConnection);
                insertVillainCommand.Parameters.AddWithValue("@villainName", villainName);
                insertVillainCommand.Parameters.AddWithValue("@factorId", factorId);

                insertVillainCommand.ExecuteNonQuery();

                villainId = getVillainIdCommand.ExecuteScalar()?.ToString();

                output
                    .AppendLine($"Villain {villainName} was added to the database.");

                    return output.ToString().TrimEnd();
            }
            else
            {
                return villainId;
            }
        }



        private static string EnsureTownExist(SqlConnection sqlConnection, string minionTown, StringBuilder output)
        {
            using var GetTownIdCommand = new SqlCommand(@"SELECT t.Id from Towns as t
                                                     WHERE t.[Name] = @minionTown", sqlConnection);

            GetTownIdCommand.Parameters.AddWithValue("@minionTown", minionTown);

            string townId = GetTownIdCommand.ExecuteScalar()?.ToString();
            
            if (townId == null)
            {
                using var insertTownAndCountry = new SqlCommand(@"INSERT INTO Towns([Name], CountryCode)
	                                                    VALUES(@townName, 'Bulgaria')", sqlConnection);
                insertTownAndCountry.Parameters.AddWithValue("@townName", minionTown);

                insertTownAndCountry.ExecuteNonQuery();

                townId = GetTownIdCommand.ExecuteScalar().ToString();

                output.AppendLine($"Town {minionTown} was added to the database.");
            }

            return townId;
        }
    }
}
