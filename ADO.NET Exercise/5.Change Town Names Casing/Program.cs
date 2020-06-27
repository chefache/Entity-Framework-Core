using Microsoft.Data.SqlClient;
using System;
using System.Linq.Expressions;
using System.Text;

namespace _5.Change_Town_Names_Casing
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(
               "Server=DESKTOP-AMLLGJ5;DataBase=MinionsDB;Integrated Security = true");

            sqlConnection.Open();

            string countryName = Console.ReadLine();

            using var getCountryId = new SqlCommand(@"SELECT Id FROM Countries
                                                      WHERE Name = @countryName", sqlConnection);
            getCountryId.Parameters.AddWithValue("@countryName", countryName);

            var countryId = getCountryId.ExecuteScalar();

            using var changeTownsNamesToUpper = new SqlCommand(@"UPDATE Towns
                                                                 SET Name = UPPER(Name)
                                                                 WHERE CountryCode = @countryId", sqlConnection);
            changeTownsNamesToUpper.Parameters.AddWithValue("@countryId", countryId);
            changeTownsNamesToUpper.ExecuteNonQuery();


            using var getCountTownsAfected = new SqlCommand(@"SELECT COUNT(Name) FROM Towns
                                                              GROUP BY CountryCode
                                                              HAVING CountryCode = @countryId", sqlConnection);
            getCountTownsAfected.Parameters.AddWithValue("@countryId", countryId);
            var countTownsAffected = (int)getCountTownsAfected?.ExecuteScalar();

            using var getNamesOfAffectedTowns = new SqlCommand(@"SELECT * FROM Towns
                                                                 WHERE CountryCode = @countryId", sqlConnection);
            getNamesOfAffectedTowns.Parameters.AddWithValue("@countryId", countryId);

            var sb = new StringBuilder();
            SqlDataReader reader = getNamesOfAffectedTowns.ExecuteReader();
            using (reader)
            {
                while (reader.Read())
                {
                    var townName = reader["Name"].ToString();
                    sb.Append($"{townName}, ");
                }
            }
            
            if (countTownsAffected < 1)
            {
                Console.WriteLine("No town names were affected.");
            }

            Console.WriteLine($"{countTownsAffected} town names were affected [{sb.ToString().TrimEnd()}].");
        }
    }
}
