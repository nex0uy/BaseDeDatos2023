using System;
using System.Data;
using Obligatorio_BaseDeDatos2023;
using MySql.Data.MySqlClient;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FormulaUno formulaUno = new FormulaUno();
            string connectionString = "server=localhost;user=root;password=bernardo;database=formulaUno";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    bool continuar = true;
                    while (true)
                    {
                        Console.WriteLine("---- Menú ----");
                        Console.WriteLine("1. Insertar todos los datos.");
                        Console.WriteLine("2. Ver piloto con más campeonatos mundiales.");
                        Console.WriteLine("3. Ver escudería con más campeonatos.");
                        Console.WriteLine("4. Ver piloto que ganó más veces un Gran Premio.");
                        Console.WriteLine("5. Ver Grandes Premios 1996-1999.");
                        Console.WriteLine("6. Ver ganador Gran Premio de Suzuka 1997.");
                        Console.WriteLine("7. Ver carreras ganadas por Jacques Villenueve.");
                        Console.WriteLine("8. Ver quien ganó más carreras sin estar en Pole Position.");
                        Console.WriteLine("9. Ver quien ganó más carreras desde la Pole Position.");
                        Console.WriteLine("0. Salir.");
                        Console.WriteLine("Elige una opción:");

                        var opcion = Console.ReadLine();

                        switch (opcion)
                        {
                            case "1":
                                InsertData(formulaUno, connection);
                                break;
                            case "2":
                                DataTable dt = formulaUno.PilotoMasCampeonatosMundiales(connection);
                                PrintDataTable(dt);
                                break;
                            case "3":
                                DataTable dt2 = formulaUno.EscuderiaMasCampeonatos(connection);
                                PrintDataTable(dt2);
                                break;
                            case "4":
                                DataTable dt3 = formulaUno.PilotoQueGanoMasUnGranPremio(connection);
                                PrintDataTable(dt3);
                                break;
                            case "5":
                                DataTable dt4 = formulaUno.GrandesPremiosCampeonatos(connection);
                                PrintDataTable(dt4);
                                break;
                            case "6":
                                DataTable dt5 = formulaUno.GanadorSuzuka1997(connection);
                                PrintDataTable(dt5);
                                break;
                            case "7":
                                DataTable dt6 = formulaUno.CarrerasGanadasVilleneuve(connection);
                                PrintDataTable(dt6);
                                break;
                            case "8":
                                DataTable dt7 = formulaUno.MasGanadorNoPole(connection);
                                PrintDataTable(dt7);
                                break;
                            case "9":
                                DataTable dt8 = formulaUno.MasGanadorDesdePole(connection);
                                PrintDataTable(dt8);
                                break;
                            case "0":
                                Console.WriteLine("Adiós!");
                                return;
                            default:
                                Console.WriteLine("Opción no válida. Inténtalo de nuevo.");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void PrintDataTable(DataTable table)
        {
            Console.WriteLine("**************************");
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Console.Write($"{row[column]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("**************************");
        }

        static void InsertData(FormulaUno formulaUno, MySqlConnection connection)
        {
            DataTable drivers = formulaUno.ReadFile("/Users/nexo/Downloads/archive/drivers.csv", "drivers");
            formulaUno.InsertData(connection, drivers);

            DataTable constructors = formulaUno.ReadFile("/Users/nexo/Downloads/archive/constructors.csv", "constructors");
            formulaUno.InsertData(connection, constructors);

            DataTable status = formulaUno.ReadFile("/Users/nexo/Downloads/archive/status.csv", "status");
            formulaUno.InsertData(connection, status);

            DataTable seasons = formulaUno.ReadFile("/Users/nexo/Downloads/archive/seasons.csv", "seasons");
            formulaUno.InsertData(connection, seasons);

            DataTable circuits = formulaUno.ReadFile("/Users/nexo/Downloads/archive/circuits.csv", "circuits");
            formulaUno.InsertData(connection, circuits);

            DataTable races = formulaUno.ReadFile("/Users/nexo/Downloads/archive/races.csv", "races");
            formulaUno.InsertData(connection, races);

            DataTable constructor_results = formulaUno.ReadFile("/Users/nexo/Downloads/archive/constructor_results.csv", "constructor_results");
            formulaUno.InsertData(connection, constructor_results);

            DataTable constructor_standings = formulaUno.ReadFile("/Users/nexo/Downloads/archive/constructor_standings.csv", "constructor_standings");
            formulaUno.InsertData(connection, constructor_standings);

            DataTable driver_standings = formulaUno.ReadFile("/Users/nexo/Downloads/archive/driver_standings.csv", "driver_standings");
            formulaUno.InsertData(connection, driver_standings);

            DataTable lap_times = formulaUno.ReadFile("/Users/nexo/Downloads/archive/lap_times.csv", "lap_times");
            formulaUno.InsertData(connection, lap_times);

            DataTable pit_stops = formulaUno.ReadFile("/Users/nexo/Downloads/archive/pit_stops.csv", "pit_stops");
            formulaUno.InsertData(connection, pit_stops);

            DataTable qualifying = formulaUno.ReadFile("/Users/nexo/Downloads/archive/qualifying.csv", "qualifying");
            formulaUno.InsertData(connection, qualifying);

            DataTable results = formulaUno.ReadFile("/Users/nexo/Downloads/archive/results.csv", "results");
            formulaUno.InsertData(connection, results);

            DataTable sprint_results = formulaUno.ReadFile("/Users/nexo/Downloads/archive/sprint_results.csv", "sprint_results");
            formulaUno.InsertData(connection, sprint_results);
        }
    }
}

