using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {
        // public fields in order to let EF Core get and set them
        public int Id { get; set; } // has to be named Id in order for EF core to recognize it
        public string UserName{ get; set; } // to avoid collison with AspNetCoreIdentity -> has Username field
        public byte[] PasswordHash{ get; set; }
        public byte[] PasswordSalt{ get; set; }
    }
}