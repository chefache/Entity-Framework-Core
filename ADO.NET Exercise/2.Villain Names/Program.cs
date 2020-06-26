using Microsoft.Data.SqlClient;
using System;

namespace _2.Villain_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(
                "Server=DESKTOP-AMLLGJ5;DataBase=MinionsDB;Integrated Security=true");

            sqlConnection.Open();

            var printVillains = new SqlCommand("SELECT v.Name, COUNT(mv.MinionId) AS Count FROM Villains AS v " +
                                               "JOIN MinionsVillains AS mv ON mv.VillainId = v.Id  " +
                                               "GROUP BY v.Name, mv.VillainId " +
                                               "HAVING COUNT(mv.MinionId) > 3", sqlConnection);

            SqlDataReader reader = printVillains.ExecuteReader();

            using (reader)
            {
                while (reader.Read())
                {
                    var villianName = reader["Name"].ToString();
                    var minionsCount = (int)reader["Count"];

                    Console.WriteLine($"{villianName} - {minionsCount}");
                }
            }
        }
    }
}
