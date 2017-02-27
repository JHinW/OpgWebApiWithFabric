using OpgWebApi.Src.Services.FaceCognitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitiveGroups
{
    public class FaceApiServiceGroup : FaceApiService
    {
        public string SubKey { get; set; }

        public FaceApiServiceGroup(string apiKey, string endpoint) :base(apiKey, endpoint)
        {
            SubKey = apiKey;
        }

    }
}
