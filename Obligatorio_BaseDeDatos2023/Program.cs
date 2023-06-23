using System.Data;

using System;
using Obligatorio_BaseDeDatos2023;
using MySql.Data.MySqlClient;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {

            try
            {
                FormulaUno formulaUno = new FormulaUno();
                string connectionString = "server=localhost;user=root;password=bernardo;database=formulaUno";

                using (MySqlConnection conecction = new MySqlConnection(connectionString))
                {
                    conecction.Open();

                    DataTable drivers = formulaUno.ReadFile("/Users/nexo/Downloads/archive/drivers.csv", "drivers");
                    formulaUno.InsertData(conecction, drivers);

                    DataTable constructors = formulaUno.ReadFile("/Users/nexo/Downloads/archive/constructors.csv", "constructors");
                    formulaUno.InsertData(conecction, constructors);

                    DataTable status = formulaUno.ReadFile("/Users/nexo/Downloads/archive/status.csv", "status");
                    formulaUno.InsertData(conecction, status);

                    DataTable seasons = formulaUno.ReadFile("/Users/nexo/Downloads/archive/seasons.csv", "seasons");
                    formulaUno.InsertData(conecction, seasons);

                    DataTable circuits = formulaUno.ReadFile("/Users/nexo/Downloads/archive/circuits.csv", "circuits");
                    formulaUno.InsertData(conecction, circuits);

                    DataTable races = formulaUno.ReadFile("/Users/nexo/Downloads/archive/races.csv", "races");
                    formulaUno.InsertData(conecction, races);

                    DataTable constructor_results = formulaUno.ReadFile("/Users/nexo/Downloads/archive/constructor_results.csv", "constructor_results");
                    formulaUno.InsertData(conecction, constructor_results);

                    DataTable constructor_standings = formulaUno.ReadFile("/Users/nexo/Downloads/archive/constructor_standings.csv", "constructor_standings");
                    formulaUno.InsertData(conecction, constructor_standings);

                    DataTable driver_standings = formulaUno.ReadFile("/Users/nexo/Downloads/archive/driver_standings.csv", "driver_standings");
                    formulaUno.InsertData(conecction, driver_standings);

                    DataTable lap_times = formulaUno.ReadFile("/Users/nexo/Downloads/archive/lap_times.csv", "lap_times");
                    formulaUno.InsertData(conecction, lap_times);

                    DataTable pit_stops = formulaUno.ReadFile("/Users/nexo/Downloads/archive/pit_stops.csv", "pit_stops");
                    formulaUno.InsertData(conecction, pit_stops);

                    DataTable qualifying = formulaUno.ReadFile("/Users/nexo/Downloads/archive/qualifying.csv", "qualifying");
                    formulaUno.InsertData(conecction, qualifying);

                    DataTable results = formulaUno.ReadFile("/Users/nexo/Downloads/archive/results.csv", "results");
                    formulaUno.InsertData(conecction, results);

                    DataTable sprint_results = formulaUno.ReadFile("/Users/nexo/Downloads/archive/sprint_results.csv", "sprint_results");
                    formulaUno.InsertData(conecction, sprint_results);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}

