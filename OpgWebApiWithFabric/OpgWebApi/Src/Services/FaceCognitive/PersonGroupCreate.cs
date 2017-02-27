using OpgWebApi.Src.Statics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitive
{
    public class PersonGroupCreate
    {
        private FaceApiService _service;

        public PersonGroupCreate(FaceApiService service)
        {
            _service = service;
        }

        public async Task CreatePersonGroup(string personGroupId, string name, string userData)
        {
            var personGroup = await _service.GetPersonGroupsByNameAsync(Configurations.PersonGroup);
            if(personGroup == null)
            {
                await _service.CreatePersonGroupAsync(personGroupId, name, userData);

            }
        }

    }
}
