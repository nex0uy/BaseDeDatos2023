using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Obligatorio_BaseDeDatos2023
{
	public class FormulaUno
	{
		public FormulaUno()
		{
        }

        public DataTable ReadFile(string path, string tableName)
        {
            DataTable table = new DataTable(tableName);

            using (StreamReader csvReader = new StreamReader(path))
            {
                // Leer la primera línea del archivo CSV (los encabezados de las columnas) y dividirla en una matriz.
                string[] headers = csvReader.ReadLine().Split(',');

                // Añadir cada encabezado como una nueva columna en la DataTable.
                foreach (string header in headers)
                {
                    table.Columns.Add(header);
                }

                // Leer el archivo CSV línea por línea hasta que no queden más líneas.
                while (!csvReader.EndOfStream)
                {
                    // Leer la línea actual y dividirla en una matriz.
                    string[] lineValues = csvReader.ReadLine().Split(',');

                    // Crear una nueva fila para la DataTable.
                    DataRow tableRow = table.NewRow();

                    // Añadir cada valor de la línea a la fila de la DataTable.
                    for (int i = 0; i < headers.Length; i++)
                    {
                        string value = lineValues[i].Trim('\"');

                        // Reemplazar cualquier valor "\\N" con DBNull para representar un valor nulo en la base de datos.
                        tableRow[i] = value == "\\N" ? DBNull.Value : (object)value;
                    }
                    // Añadir la fila completa a la DataTable.
                    table.Rows.Add(tableRow);
                }
            }
            // Devolver la DataTable completa.
            return table;
        }


        public void InsertData(MySqlConnection connection, DataTable table)
        {
            try
            {
                string tableName = table.TableName;
                var columnNames = table.Columns;
                var columnsRows = table.Rows;

                // Utilizar un StringBuilder para crear una lista de nombres de columnas separadas por comas.
                StringBuilder columnNamesConcad = new StringBuilder();

                // Añadir cada nombre de columna al StringBuilder.
                foreach (DataColumn column in columnNames)
                {
                    if (columnNamesConcad.Length > 0)
                    {
                        columnNamesConcad.Append(',');
                    }
                    columnNamesConcad.Append(column.ColumnName);
                }

                // Encerrar cada nombre de columna entre acentos graves para que sea válido en SQL.
                string columns = "`" + columnNamesConcad.ToString().Replace(",", "`,`") + "`";

                foreach (DataRow row in columnsRows)
                {
                    StringBuilder parameters = new StringBuilder();
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;

                        // Crear un parámetro para cada valor en la fila y añadirlo al comando.
                        foreach (DataColumn column in columnNames)
                        {
                            if (parameters.Length > 0)
                            {
                                parameters.Append(',');
                            }

                            // Crear el nombre del parámetro basándose en el nombre de la columna.
                            string parameterName = $"@{column.ColumnName}";
                            parameters.Append(parameterName);

                            // Añadir el valor de la fila como un parámetro en el comando, 
                            // utilizando DBNull si el valor es null.
                            command.Parameters.AddWithValue(parameterName, row[column.ColumnName] ?? DBNull.Value);
                        }

                        command.CommandText = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private DataTable ExecuteQuery(MySqlConnection connection, string query)
        {
            DataTable dt = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
            adapter.Fill(dt);
            return dt;
        }

        //1.Qué piloto es el que ha ganado más campeonatos mundiales? (Si hay fanáticos, podrán validar los 7 campeonatos de Lewis y de Michael)
        public DataTable PilotoMasCampeonatosMundiales(MySqlConnection connection)
        {
            string query = @"
                    SELECT piloto, COUNT(*) AS total_campeonatos
                    FROM (
                    SELECT r.year, CONCAT(d.forename, ' ', d.surname) AS piloto, ds.points AS max_points
                    FROM races r
                    JOIN driver_standings ds ON r.raceId = ds.raceId
                    JOIN drivers d ON ds.driverId = d.driverId
                    WHERE (r.year, r.round) IN (
                    SELECT year, MAX(round)
                    FROM races
                    GROUP BY year
                    )
                    AND ds.points = (
                    SELECT MAX(ds2.points)
                    FROM driver_standings ds2
                    WHERE ds2.raceId = ds.raceId
                    )
                    ) AS tmp
                    GROUP BY piloto
                    ORDER BY total_campeonatos DESC
                    LIMIT 2";
            return ExecuteQuery(connection, query);
        }

        //2. Qué escudería ha ganado el campeonato de constructores más veces en la historia? 
        public DataTable EscuderiaMasCampeonatos(MySqlConnection connection)
        {
            string query = @"
                SELECT ganador, COUNT(*) as total_victorias
                FROM (
                SELECT carreras.year, constructores.name as ganador, MAX(cs.points) as puntos
                FROM constructor_standings cs
                JOIN races carreras ON cs.raceId = carreras.raceId
                JOIN constructors constructores ON cs.constructorId = constructores.constructorId
                WHERE (carreras.year, cs.points) IN (
                SELECT carreras.year, MAX(cs.points)
                FROM constructor_standings cs
                JOIN races carreras ON cs.raceId = carreras.raceId
                GROUP BY carreras.year
                )
                GROUP BY carreras.year, constructores.name
                ) as campeonatos
                GROUP BY ganador
                ORDER BY total_victorias DESC
                LIMIT 1";
            return ExecuteQuery(connection, query);
        }

        //3. Qué piloto ganó más veces un Gran Premio?
        public DataTable PilotoQueGanoMasUnGranPremio(MySqlConnection connection)
        {
            string query = @"
                        SELECT
                        pilotos.driverId,
                        pilotos.forename,
                        pilotos.surname,
                        COUNT(*) as ganadas
                        FROM results resultados
                        JOIN drivers pilotos on resultados.driverId = pilotos.driverId
                        WHERE positionOrder = 1
                        GROUP BY pilotos.driverId
                        ORDER BY ganadas DESC
                        LIMIT 1";
            return ExecuteQuery(connection, query);
        }

        //4. Qué grandes premios tuvieron los campeonatos del 1996 – 1999. 
        public DataTable GrandesPremiosCampeonatos(MySqlConnection connection)
        {
            string query = @"
                        SELECT
                        carreras.year,
                        circuitos.name
                        FROM
                        races carreras
                        JOIN
                        circuits circuitos ON carreras.circuitId = circuitos.circuitId
                        WHERE
                        carreras.year BETWEEN 1996 AND 1999
                        ORDER BY
                        carreras.year, circuitos.name";
            return ExecuteQuery(connection, query);
        }

        //5. Quién ganó el Gran Premio de Suzuka, de 1997? 
        public DataTable GanadorSuzuka1997(MySqlConnection connection)
        {
            string query = @"
                        SELECT
                        pilotos.driverId,
                        pilotos.forename,
                        pilotos.surname
                        FROM
                        results resultados
                        JOIN
                        drivers pilotos ON resultados.driverId = pilotos.driverId
                        WHERE
                        resultados.raceId = (
                        SELECT
                        carreras.raceId
                        FROM
                        races carreras
                        JOIN
                        circuits circuitos ON carreras.circuitId = circuitos.circuitId
                        WHERE
                        circuitos.name = 'Suzuka Circuit' AND carreras.year = 1997
                        )
                        AND resultados.positionOrder = 1";
            return ExecuteQuery(connection, query);
        }

        //6. Cuántas carreras ganó Jacques Villenueve? 
        public DataTable CarrerasGanadasVilleneuve(MySqlConnection connection)
        {
            string query = @"
                    SELECT
                    pilotos.driverId,
                    pilotos.forename,
                    pilotos.surname,
                    COUNT(*) as ganadas
                    FROM results resultados
                    JOIN drivers pilotos on resultados.driverId = pilotos.driverId
                    WHERE pilotos.surname = 'Villeneuve' AND pilotos.forename = 'Jacques' AND positionOrder = 1
                    GROUP BY pilotos.driverId
                    ORDER BY ganadas DESC
                    LIMIT 1";
            return ExecuteQuery(connection, query);
        }

        //7. Quién ganó más carreras saliendo desde una posición que no era la Pole Position?
        public DataTable MasGanadorNoPole(MySqlConnection connection)
        {
            string query = @"
                        SELECT
                        pilotos.driverId,
                        pilotos.forename,
                        pilotos.surname,
                        COUNT(*) as ganadas
                        FROM results resultados
                        JOIN drivers pilotos on resultados.driverId = pilotos.driverId
                        WHERE grid != 1 AND positionOrder = 1
                        GROUP BY pilotos.driverId
                        ORDER BY ganadas DESC
                        LIMIT 1";
            return ExecuteQuery(connection, query);
        }

        //8. Y quién saliendo desde ahí? 
        public DataTable MasGanadorDesdePole(MySqlConnection connection)
        {
            string query = @"
                        SELECT
                        pilotos.driverId,
                        pilotos.forename,
                        pilotos.surname,
                        COUNT(*) as ganadas
                        FROM results resultados
                        JOIN drivers pilotos on resultados.driverId = pilotos.driverId
                        WHERE grid = 1 AND positionOrder = 1
                        GROUP BY pilotos.driverId
                        ORDER BY ganadas DESC
                        LIMIT 1";
            return ExecuteQuery(connection, query);
        }
    }
}

