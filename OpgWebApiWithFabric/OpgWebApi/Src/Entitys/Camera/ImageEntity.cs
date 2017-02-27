using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Entitys.Camera
{
    public class ImageEntity
    {
        public string Id { get; set; }

        public string CampaignId { get; set; }

        public string CameraId { get; set; }

        public string VideoId { get; set; }

        public DateTime SentTime { get; set; }
    }
}
