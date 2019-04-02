using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace TotalAnarchyGameServer.Network.Database {
    public class Database {

        static SqlConnection myConnection = new SqlConnection("user id=username;" + "password=password;server=serverurl;" + "Trusted_Connection=yes;" + "database=database;" + "connection timeout=30");

        

        public static void StartDatabase() {
            try {
                myConnection.Open();
            } catch (Exception e) {

                Logger.WriteError(e.ToString());
            }
        }

    }
}
