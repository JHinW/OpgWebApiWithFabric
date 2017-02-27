using Microsoft.ProjectOxford.Face.Contract;
using OpgWebApi.Src.Entitys.Camera;
using OpgWebApi.Src.Statics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitive
{
    public class NameIdentity
    {
        private FaceApiService _service;

        public NameIdentity(FaceApiService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<Tuple<Person, Face, Candidate>>> CheckNameFromImage(Stream stream)
        {
            var faces = await _service.DetectAsync(
                        stream,
                        returnFaceId: true,
                        returnFaceLandmarks: false,
                        returnFaceAttributes: FaceApiService.DefaultFaceAttributeTypes);
            var outs = new List<Tuple<Person, Face, Candidate>>();

            if (faces.Count() > 0)
            {
                var personGroup = await _service.GetPersonGroupsByNameAsync(Configurations.PersonGroup);
                var identityResults = await _service.IdentifyAsync(personGroup.PersonGroupId,
                        faces.Select(x => x.FaceId).ToArray());

                foreach (var face in faces)
                {
                    var result = identityResults.Where(x => x.FaceId == face.FaceId).First();
                    if(result.Candidates.Count() > 0)
                    {
                        var candidate = result.Candidates.OrderByDescending(x => x.Confidence).First();
                        var person = await _service.GetPersonAsync(personGroup.PersonGroupId, candidate.PersonId);
                        outs.Add(Tuple.Create(person, face, candidate));
                    }else
                    {
                        var candidate = new Candidate();
                        var person = new Person();
                        outs.Add(Tuple.Create(person, face, candidate));
                    }
                    //yield return face;
                }

                return outs;
            }
            return outs;
        }

    }
}
