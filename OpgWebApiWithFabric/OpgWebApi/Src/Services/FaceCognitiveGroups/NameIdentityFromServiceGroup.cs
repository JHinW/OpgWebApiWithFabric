using Microsoft.ProjectOxford.Face.Contract;
using OpgWebApi.Src.Services.FaceCognitive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitiveGroups
{
    public class NameIdentityFromServiceGroup
    {
        public static volatile int selected = 0;

        private IEnumerable<FaceApiServiceGroup> _services;

        public NameIdentityFromServiceGroup(IEnumerable<FaceApiServiceGroup> services)
        {
            _services = services;
        }

        public async Task<Tuple<IEnumerable<Tuple<Person, Face, Candidate>>, string>> CheckNameFromImage(Stream stream)
        {
            var count = _services.Count();
            var faceServer = _services.ToArray()[NameIdentityFromServiceGroup.selected++ % count];
            var identitier = new NameIdentity(faceServer);
            var detail = await identitier.CheckNameFromImage(stream);
            return Tuple.Create(detail, faceServer.SubKey);
        }

    }
}
