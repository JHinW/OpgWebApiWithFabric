using OpgWebApi.Src.Statics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitive
{
    public class NameRegister
    {
        private FaceApiService _service;

        public NameRegister(FaceApiService service)
        {
            _service = service;
        }

        public async Task<string> RegisterYourNameWithImage(Stream stream, string uniqueId)
        {

            var personGroup = await _service.GetPersonGroupsByNameAsync(Configurations.PersonGroup);
            var psersons = await _service.GetPersonsAsync(personGroup.PersonGroupId);
            if(psersons.Any(x=> x.Name == uniqueId))
            {
                var person = psersons.Where(x => x.Name == uniqueId).First();
                await _service.AddPersonFaceAsync(personGroup.PersonGroupId, person.PersonId, stream, uniqueId, null);

            }else
            {
                var result = await _service.CreatePersonWithResultAsync(personGroup.PersonGroupId, uniqueId);
                await _service.AddPersonFaceAsync(personGroup.PersonGroupId, result.PersonId, stream, uniqueId, null);
            }

            //await _service.TrainPersonGroupAsync(personGroup.PersonGroupId);

            return uniqueId;
        }

        public async Task<string> RegisterYourNameWithImages(Stream[] streams, string uniqueId)
        {
            var personGroup = await _service.GetPersonGroupsByNameAsync(Configurations.PersonGroup);
            var psersons = await _service.GetPersonsAsync(personGroup.PersonGroupId);
            if (psersons.Any(x => x.Name == uniqueId))
            {
                var person = psersons.Where(x => x.Name == uniqueId).First();
                foreach(var stream in streams)
                {
                    await _service.AddPersonFaceAsync(personGroup.PersonGroupId, person.PersonId, stream, uniqueId, null);
                }
            }
            else
            {
                var result = await _service.CreatePersonWithResultAsync(personGroup.PersonGroupId, uniqueId);
                foreach(var stream in streams)
                {
                    await _service.AddPersonFaceAsync(personGroup.PersonGroupId, result.PersonId, stream, uniqueId, null);
                }
            }

            //await _service.TrainPersonGroupAsync(personGroup.PersonGroupId);

            return uniqueId;
        }

    }
}
