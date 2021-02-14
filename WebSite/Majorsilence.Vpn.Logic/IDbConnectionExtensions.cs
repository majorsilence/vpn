using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.OrmLite;

namespace System.Data
{
    public static class IDbConnectionExtensions
    {

        private static List<string> GetColumnNames(IDbConnection db, string tableName, IOrmLiteDialectProvider provider)
        {
            var columns = new List<string>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = getCommandText(tableName, provider);
                var tbl = new DataTable();
                tbl.Load(cmd.ExecuteReader());
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    columns.Add(tbl.Columns[i].ColumnName);
                }

            }
            return columns;
        }

        private static string getCommandText(string tableName, IOrmLiteDialectProvider provider)
        {

            if (provider == MySqlDialect.Provider)
            {
                return string.Format("select * from {0} limit 1", tableName);
            }
            else
            {
                return string.Format("select top 1 * from {0}", tableName);
            }
        }

        public static void AlterTable<T>(this IDbConnection db, IOrmLiteDialectProvider provider) where T : new()
        {
            var model = ModelDefinition<T>.Definition;
            var table = new T();
            var namingStrategy = provider.NamingStrategy;
            // just create the table if it doesn't already exist
            var tableName = namingStrategy.GetTableName(model.ModelName);
            if (db.TableExists(tableName) == false)
            {
                db.CreateTable<T>(overwrite: false);
                return;
            }

            // find each of the missing fields
            var columns = GetColumnNames(db, model.ModelName, provider);
            var missing = ModelDefinition<T>.Definition.FieldDefinitions
                                            .Where(field => columns.Contains(namingStrategy.GetColumnName(field.FieldName)) == false)
                                            .ToList();

            // add a new column for each missing field
            foreach (var field in missing)
            {
                var columnName = namingStrategy.GetColumnName(field.FieldName);
                var alterSql = string.Format("ALTER TABLE {0} ADD COLUMN {1} {2}",
                                             tableName,
                                             columnName,
                                             db.GetDialectProvider().GetColumnTypeDefinition(field.FieldType)
                    );
                Console.WriteLine(alterSql);
                db.ExecuteSql(alterSql);


                // custom code to make sure new string or boolean fields are not null

                var dbType = db.GetDialectProvider().GetColumnDbType(field.FieldType);
                if (dbType == DbType.String)
                {

                    var alterSqlRemoveNulls = string.Format("UPDATE TABLE {0} SET {1} = ''",
                                             tableName,
                                             columnName
                    );

                    Console.WriteLine(alterSqlRemoveNulls);
                    db.ExecuteSql(alterSqlRemoveNulls);

                }
                else if (dbType == DbType.Boolean)
                {
                    var alterSqlRemoveNulls = string.Format("UPDATE TABLE {0} SET {1} = 0",
                                            tableName,
                                            columnName
                   );

                    Console.WriteLine(alterSqlRemoveNulls);
                    db.ExecuteSql(alterSqlRemoveNulls);
                }

                if (dbType == DbType.String || dbType == DbType.Boolean)
                {
                    var alterSqlNotNull = string.Format("ALTER TABLE {0} ADD COLUMN {1} {2} NOT NULL",
                                                 tableName,
                                                 columnName,
                                                 db.GetDialectProvider().GetColumnTypeDefinition(field.FieldType)
                        );
                    Console.WriteLine(alterSqlNotNull);
                    db.ExecuteSql(alterSqlNotNull);
                }

            }
        }

    }
}
