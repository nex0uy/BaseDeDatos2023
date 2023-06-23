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

        public DataTable ReadFile(string path, string tableName)
        {

            DataTable table = new DataTable(tableName);

            using (StreamReader csvReader = new StreamReader(path))
            {
                string[] headers = csvReader.ReadLine().Split(',');

                foreach (string header in headers)
                {
                    table.Columns.Add(header);
                }

                while (!csvReader.EndOfStream)
                {
                    string[] lineValues = csvReader.ReadLine().Split(',');

                    DataRow tableRow = table.NewRow();

                    for (int i = 0; i < headers.Length; i++)
                    {
                        string value = lineValues[i].Trim('\"');

                        tableRow[i] = value == "\\N" ? DBNull.Value : (object)value;
                    }
                    table.Rows.Add(tableRow);

                }

            }
            return table;
        }

        public void InsertData(MySqlConnection connection, DataTable table)
        {

            try
            {
                string tableName = table.TableName;
                var columnNames = table.Columns;
                var columnsRows = table.Rows;

                StringBuilder columnNamesConcad = new StringBuilder();

                foreach (DataColumn column in columnNames)
                {
                    if (columnNamesConcad.Length > 0)
                    {
                        columnNamesConcad.Append(',');
                    }
                    columnNamesConcad.Append(column.ColumnName);
                }

                //string columns = columnNamesConcad.ToString();

                string columns = "`" + columnNamesConcad.ToString().Replace(",", "`,`") + "`";


                foreach (DataRow row in columnsRows)
                {
                    StringBuilder parameters = new StringBuilder();
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;

                        foreach (DataColumn column in columnNames)
                        {
                            if (parameters.Length > 0)
                            {
                                parameters.Append(',');
                            }

                            string parameterName = $"@{column.ColumnName}";
                            parameters.Append(parameterName);

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

