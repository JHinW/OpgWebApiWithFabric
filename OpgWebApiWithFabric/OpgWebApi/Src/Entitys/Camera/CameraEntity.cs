using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Entitys.Camera
{
    public class CameraEntity
    {
        public string CampaignId { get; set; }

        public string CameraId { get; set; }

        [Key]
        public string VideoId { get; set; }

        public DateTime VideoUploadBeginTime { get; set; }

        public DateTime VideoUploadEndTime { get; set; }

        public string VideoUrl { get; set; }

        public DateTime DataArriveTime { get; set; }
    }
}
