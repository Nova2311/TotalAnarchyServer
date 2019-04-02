using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using TotalAnarchyGameServer.Weapons;


namespace TotalAnarchyGameServer {
    public class Serializer {
        public static void Serialize(List<BaseWeapon> weapons) {
            File.WriteAllText(@"C:\Users\q5058597\Source\test.json", JsonConvert.SerializeObject(weapons, Formatting.Indented));
        }

        public static void SendJSONFile(Clients client) {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(client.ip);
            
            
        }
    }
}
