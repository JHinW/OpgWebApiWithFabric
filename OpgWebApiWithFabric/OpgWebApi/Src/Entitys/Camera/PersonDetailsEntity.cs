using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Entitys.Camera
{
    public class PersonDetailsEntity
    {
        [Key]
        public string Id { get; set; }

        public string MachineName { get; set; }

        public string FaceId { get; set; }

        public string VideoId { get; set; }

        public string ImageId { get; set; }

        public string Name { get; set; }

        public double Age { get; set; }

        public double Confidence { get; set; }

        public string Gender { get; set; }

        public string FacialHair { get; set; }

        public string Glasses { get; set; }

        public string Smile { get; set; }

        public DateTime MonitorTime { get; set; }

        public string UsedSubKey { get; set; }
    }
}
