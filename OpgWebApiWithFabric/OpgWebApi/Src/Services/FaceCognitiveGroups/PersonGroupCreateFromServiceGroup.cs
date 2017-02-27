using OpgWebApi.Src.Services.FaceCognitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitiveGroups
{
    public class PersonGroupCreateFromServiceGroup
    {
        private IEnumerable<FaceApiServiceGroup> _services;

        public PersonGroupCreateFromServiceGroup(IEnumerable<FaceApiServiceGroup> services)
        {
            _services = services;
        }

        public async Task CreatePersonGroup(string personGroupId, string name, string userData)
        {
            foreach(var service in _services)
            {
                var creator = new PersonGroupCreate(service);
                await creator.CreatePersonGroup(personGroupId, name, userData);
            }
        }

    }
}
