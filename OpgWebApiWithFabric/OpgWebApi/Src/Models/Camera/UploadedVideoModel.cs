using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgServices.OpgSrc.Models.Camera
{
    public class UploadedVideoModel
    {   
        public string CampaignId { get; set; }

        public string CameraId { get; set; }

        public string VideoId { get; set; }

        public DateTime VideoUploadBeginTime { get; set; }

        public DateTime VideoUploadEndTime { get; set; }

        public string VideoUrl { get; set; }
    }
}
