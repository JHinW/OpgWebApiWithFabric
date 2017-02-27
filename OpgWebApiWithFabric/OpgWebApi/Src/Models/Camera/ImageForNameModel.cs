using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Models.Camera
{
    public class ImageForNameModel
    {
        public string id { get; set; }

        public string ImageBase64String { get; set; }

        public string ImageUrl { get; set; }

        public string name { get; set; }

        public DateTime SentTime { get; set; }
    }
}
