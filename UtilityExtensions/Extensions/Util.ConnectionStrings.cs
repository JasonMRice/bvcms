using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace UtilityExtensions
{
    public static partial class Util
    {
        private static string _host;
        public static string Host
        {
            get
            {
                if (_host.HasValue())
                {
                    return _host;
                }
                var h = ConfigurationManager.AppSettings["host"];
                if (h.HasValue())
                {
                    return h;
                }

                try
                {
                    if (HttpContextFactory.Current?.Request != null)
                    {
                        return HttpContextFactory.Current.Request.Url.Authority.SplitStr(".:")[0];
                    }
                }
                catch
                {
                }
                return null;
            }

            set => _host = value;
        }
        public static string DbServer
        {
            get
            {
                var settings = ConfigurationManager.ConnectionStrings["CMS"];
                if (settings != null)
                {
                    return new SqlConnectionStringBuilder(settings.ConnectionString).DataSource;
                }
                return null;
            }
        }

        public static bool IsHosted
        {
            get
            {
                if (IsDebug())
                {
                    return false;
                }

                return ConfigurationManager.AppSettings["INSERT_X-FORWARDED-PROTO"] == "true";
            }
        }

        public static string ParseEnv(string value)
        {
            return Environment.ExpandEnvironmentVariables(value);
        }

        public static string GetConnectionString(string host)
        {
            var cs = ConnectionStringSettings(host) ?? ConfigurationManager.ConnectionStrings["CMS"];
            var cb = new SqlConnectionStringBuilder(cs?.ConnectionString ?? "Data Source=(local);Integrated Security=True");
            var a = host.Split('.', ':');
            cb.InitialCatalog = $"CMS_{a[0]}";
            return ParseEnv(cb.ConnectionString);
        }

        public static string GetConnectionString2(string db, int? timeout = null)
        {
            var cs = ConnectionStringSettings(db) ?? ConfigurationManager.ConnectionStrings["CMS"];
            var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
            if (timeout.HasValue)
            {
                cb.ConnectTimeout = timeout.Value;
            }
            cb.InitialCatalog = db;
            return ParseEnv(cb.ConnectionString);
        }

        private static ConnectionStringSettings ConnectionStringSettings(string host)
        {
            var h2 = ConfigurationManager.AppSettings["CmsHosted2"];
            if (h2.HasValue())
            {
                var a = h2.Split(',');
                if (a.Contains(host))
                {
                    return ConfigurationManager.ConnectionStrings["CMS2"];
                }
            }
            return ConfigurationManager.ConnectionStrings["CMS"];
        }

        private const string STR_ConnectionString = "ConnectionString";
        public static string ConnectionString
        {
            get
            {
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                    {
                        if (HttpContextFactory.Current.Session[STR_ConnectionString] != null)
                        {
                            return HttpContextFactory.Current.Session[STR_ConnectionString].ToString();
                        }
                    }
                }

                var cs = ConnectionStringSettings(Host);
                var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
                cb.InitialCatalog = $"CMS_{Host}";
                return ParseEnv(cb.ConnectionString);
            }
            set
            {
                if (HttpContextFactory.Current != null)
                {
                    HttpContextFactory.Current.Session[STR_ConnectionString] = value;
                }
            }
        }

        public static string ReadOnlyConnectionString(string host = null, bool finance = false)
        {
            var pw = ConfigurationManager.AppSettings["readonlypassword"];
            if (!pw.HasValue())
            {
                return ConnectionString;
            }

            var cs = ConnectionStringSettings(host ?? Host);
            var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
            cb.InitialCatalog = $"CMS_{host ?? Host}";
            cb.IntegratedSecurity = false;
            cb.UserID = (finance ? $"ro-{cb.InitialCatalog}-finance" : $"ro-{cb.InitialCatalog}");
            cb.Password = pw;
            return ParseEnv(cb.ConnectionString);
        }

        public static string ConnectionStringReadOnly => ReadOnlyConnectionString();

        public static string ConnectionStringReadOnlyFinance => ReadOnlyConnectionString(finance: true);

        public static string ConnectionStringImage
        {
            get
            {
                var cs = ConnectionStringSettings(Host);
                var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
                var a = Host.Split('.', ':');
                cb.InitialCatalog = $"CMSi_{a[0]}";
                return ParseEnv(cb.ConnectionString);
            }
        }
        public static string GetConnectionString2(string cs, string db)
        {
            var connectionStr = new SqlConnectionStringBuilder(cs)
            { InitialCatalog = db }.ConnectionString;
            return ParseEnv(connectionStr);
        }
    }
}

