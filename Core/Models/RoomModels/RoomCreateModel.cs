using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.RoomModels
{
    public class RoomCreateModel
    {
        public RoomCreateModel()
        {
            this.Users = new List<int>();
            this.Roles = new List<int>();
        }

        public string Name { get; set; }

        public List<int> Users { get; set; }

        public List<int> Roles { get; set; }
    }
}
