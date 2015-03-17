using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace LabelPrintingInterface
{
    public class MainMenuBarDataSource
    {
        public MainMenuBarDataSource()
        {
            System.Configuration.Configuration rootWebConfig =
               System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/WebSiteName");
            //check if the AppSettings section has items
            if (rootWebConfig.AppSettings.Settings.Count > 0)
            {
                _connectionString = "Data Source=192.168.103.150\\INFLOWSQL;User ID=mws;Password=p@ssw0rd";
            }
        }

        string _connectionString =  "";



    }
}