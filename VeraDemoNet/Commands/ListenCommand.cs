using System.Data.Common;
using System.Data.SqlClient;

namespace VeraDemoNet.Commands
{
    public class ListenCommand : BlabberCommandBase, IBlabberCommand, IListenCommand
    {
        private readonly string username;

        public ListenCommand(DbConnection connect, string username)
        {
            this.connect = connect;
            this.username = username;
        }

        public void Execute(string blabberUsername)
        {
            var listenerInsertQuery = "INSERT INTO listeners (blabber, listener, status) values (@blabber, @listener, 'Active');";
            logger.Info(listenerInsertQuery);

            using (var action = connect.CreateCommand())
            {

                action.CommandText = listenerInsertQuery;
                action.Parameters.Add(new SqlParameter {ParameterName = "@blabber", Value = blabberUsername});
                action.Parameters.Add(new SqlParameter {ParameterName = "@listener", Value = username});
                action.ExecuteNonQuery();
            }

            var blabNameQuery = "SELECT blab_name FROM users WHERE username = @username";
            logger.Info(blabNameQuery);
            string blabberName;

            using (var sqlStatement = connect.CreateCommand())
            {
                sqlStatement.CommandText = blabNameQuery;
                sqlStatement.Parameters.Add(new SqlParameter { ParameterName = "@username", Value = blabberUsername });

                using (var result = sqlStatement.ExecuteReader())
                {
                    result.Read();
                    blabberName = result.GetString((0));
                }
            }

            using (var sqlStatement = connect.CreateCommand())
            {
                var sqlQuery = "INSERT INTO users_history (blabber, event) VALUES (@username, @username + ' is now listening to ' + @blabberUsername)";
                sqlStatement.CommandText = sqlQuery;
                sqlStatement.Parameters.AddRange(new[] {
                    new SqlParameter("username", username),
                    new SqlParameter("blabberUsername", blabberUsername)
                    });
                logger.Info(sqlQuery);
                sqlStatement.ExecuteNonQuery();
            }

            /* END BAD CODE */
        }
    }
}