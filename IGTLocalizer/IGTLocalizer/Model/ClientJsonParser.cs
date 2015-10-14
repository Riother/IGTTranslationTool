using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IGTLocalizer.Model
{
    public class ClientJsonParser
    {

        public ClientLibrary ClientLib { get; set; }

        public ClientJsonParser(ClientLibrary lib) {
            ClientLib = lib;
        }

        public ClientJsonParser(string json){
            deserializeClients(json);
        }

        public void deserializeClients(string json) {
            ClientLib = JsonConvert.DeserializeObject<ClientLibrary>(json);
        }

        public string serializeClients() {
            return JsonConvert.SerializeObject(ClientLib);
        }

    }
}
