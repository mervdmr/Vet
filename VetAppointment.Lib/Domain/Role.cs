using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetAppointment.Lib.Domain
{
    public class Role : BaseEntity<int>
    {
        public string Name { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }
}
