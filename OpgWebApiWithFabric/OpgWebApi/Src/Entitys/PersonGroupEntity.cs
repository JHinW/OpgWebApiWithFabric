using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Entitys
{
    public class PersonGroupEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string UserData { get; set; }

        public DateTime SentTime { get; set; }
    }
}
