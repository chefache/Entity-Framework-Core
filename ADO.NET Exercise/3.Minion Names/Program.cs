using Microsoft.Data.SqlClient;
using System;
using System.Text;

namespace _3.Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(
                "Server=DESKTOP-AMLLGJ5;DataBase=MinionsDB;Integrated Security = true");

            sqlConnection.Open();

            var sb = new StringBuilder();

            string villainId = Console.ReadLine();

            using var getVillainName = new SqlCommand(@"SELECT v.Name FROM Villains AS v WHERE v.Id = @villainId", sqlConnection);
            getVillainName.Parameters.AddWithValue("@villainId", villainId);

            string villainName = getVillainName
                .ExecuteScalar()?
                .ToString();

            if (villainName == null)
            {
                sb.AppendLine($"No villain with ID {villainId} exist in the database.");
            }
            else
            {
                sb.AppendLine($"Villain: {villainName}");

                using var getMinionsInfo = new SqlCommand(@"SELECT m.[Name], m.Age FROM Minions as m
                                                            JOIN MinionsVillains AS mv ON mv.MinionId = m.Id
                                                            JOIN Villains AS v ON v.Id = mv.VillainId
                                                            WHERE v.[Name] = @villainName
                                                            ORDER BY m.[Name]", sqlConnection);

                getMinionsInfo.Parameters.AddWithValue("@villainName", villainName);

                SqlDataReader reader = getMinionsInfo.ExecuteReader();

                if (reader.HasRows)
                {
                    int rowNum = 1;
                    while (reader.Read())
                    {
                        string minionName = reader["Name"]?.ToString();
                        string minionAge = reader["Age"]?.ToString();

                        sb.AppendLine($"{rowNum}. {minionName} {minionAge}");
                        rowNum++;
                    }
                }
                else
                {
                    sb.AppendLine("(no minions)");
                }
            }

            var result = sb.ToString().TrimEnd();
            Console.WriteLine(result);
        }
    }
}
