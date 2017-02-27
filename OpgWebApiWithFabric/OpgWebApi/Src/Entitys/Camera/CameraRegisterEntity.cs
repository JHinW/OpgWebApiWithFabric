using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Entitys.Camera
{
    public class CameraRegisterEntity
    {
        public string Id { get; set; }

        public string MachineName { get; set; }

        public string LocationAddr { get; set; }

        public DateTime UpdatedTime { get; set; }

        public DateTime CreatedTime { get; set; }

    }
}
