using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;

namespace Obligatorio_BaseDeDatos2023
{
	public class FormulaUno
	{
		public FormulaUno()
		{
        }

        //Método para leer los datos de un archivo CSV y convertirlos en una DataTable.
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

        // Método para insertar los datos de una DataTable en una base de datos MySQL.
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


    }
}

