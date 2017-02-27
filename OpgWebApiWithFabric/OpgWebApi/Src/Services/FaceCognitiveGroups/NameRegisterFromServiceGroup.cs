using OpgWebApi.Src.Services.FaceCognitive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitiveGroups
{
    public class NameRegisterFromServiceGroup
    {
        private IEnumerable<FaceApiServiceGroup> _services;

        public NameRegisterFromServiceGroup(IEnumerable<FaceApiServiceGroup> services)
        {
            _services = services;
        }

        public async Task<string> RegisterYourNameWithImage(Stream stream, string uniqueId)
        {
            foreach(var service in _services)
            {
                stream.Position = 0;
                var register = new NameRegister(service);
                using(var mem = new MemoryStream())
                {
                    await stream.CopyToAsync(mem);
                    mem.Position = 0;
                    await register.RegisterYourNameWithImage(mem, uniqueId);
                }
                
            }

            return uniqueId;
        }

        public async Task<string> RegisterYourNameWithImages(Stream[] streams, string uniqueId)
        {
            foreach (var service in _services)
            {
                var register = new NameRegister(service);

                await register.RegisterYourNameWithImages(streams, uniqueId);
            }

            return uniqueId;
        }
    }
}
