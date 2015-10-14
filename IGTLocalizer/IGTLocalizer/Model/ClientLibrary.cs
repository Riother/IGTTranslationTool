using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGTLocalizer.Model
{
    public class ClientLibrary
    {
        public List<Client> Clients { get; set; }

        public ClientLibrary() {
            Clients = new List<Client>();
        }

        public ClientLibrary(List<Client> clients)
        {
            this.Clients = clients;
        }

    }
}