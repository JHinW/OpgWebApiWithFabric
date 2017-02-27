using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgServices.OpgSrc.Models.Camera
{
    public class UploadedVideoAssocitedJob
    {
        public string VideoId { get; set; }

        public string VideoUrl { get; set; }

        public Jobs JobType { get; set; }

        public DateTime JobSendTime { get; set; }
    }
}
