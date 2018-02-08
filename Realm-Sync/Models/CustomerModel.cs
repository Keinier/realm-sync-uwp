using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realm_Sync.Models
{
    public class CustomerModel:RealmObject
    {

       
        public string No { get; set; }


        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
