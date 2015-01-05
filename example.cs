private System.Data.DataSet ExecuteScript(string scriptPath)
        {
            var lines = ReadScript(scriptPath);

            var ds = new System.Data.DataSet();

            var connString = ConfigurationManager.ConnectionStrings["my_conn_string"].ConnectionString;

            using (var conn = new SqlCeConnection(connString))
            {
                conn.Open();

                var sb = new StringBuilder();

                foreach (var line in lines)
                {
                    var currLine = line.Trim();

                    if (currLine.Equals("GO", StringComparison.OrdinalIgnoreCase))
                    {
                        FillDataset(sb.ToString(), conn, ds);
                        sb.Remove(0, sb.Length);
                    }
                    else
                    {
                        sb.Append(currLine);
                        sb.Append(Environment.NewLine);
                    }
                }

                if (sb.Length != 0)
                    FillDataset(sb.ToString(), conn, ds);
            }

            return ds;
        }

        private IEnumerable<string> ReadScript(string scriptPath) {
            var basePath = Server.MapPath("~/App_Data/");
            var scriptServerPath = System.IO.Path.Combine(basePath, scriptPath);
            if (!System.IO.File.Exists(scriptServerPath))
                return Enumerable.Empty<string>();

            return System.IO.File.ReadAllLines(scriptServerPath);
        }

        private static System.Data.DataSet FillDataset(string query, SqlCeConnection conn, System.Data.DataSet ds)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.CommandType = System.Data.CommandType.Text;
                var adapter = new SqlCeDataAdapter(cmd);
                adapter.Fill(ds);
            }

            return ds;
        }
