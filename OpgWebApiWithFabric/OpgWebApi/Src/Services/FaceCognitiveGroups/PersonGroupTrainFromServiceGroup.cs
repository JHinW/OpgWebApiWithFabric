using OpgWebApi.Src.Services.FaceCognitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitiveGroups
{
    public class PersonGroupTrainFromServiceGroup
    {
        private IEnumerable<FaceApiServiceGroup> _services;

        public PersonGroupTrainFromServiceGroup(IEnumerable<FaceApiServiceGroup> services)
        {
            _services = services;
        }

        public async Task TrainPersonGroup(string groupId)
        {
            foreach(var service in _services)
            {
                var trainer = new PersonGroupTrain(service);
                await trainer.TrainPersonGroup(groupId);
            }
            
        }

        public async Task<string> GetPersonGroupTrainingStatusAsync(string groupId)
        {
            foreach (var service in _services)
            {
                var trainer = new PersonGroupTrain(service);
                await trainer.TrainPersonGroup(groupId);
            }

            return "ok";

        }
    }
}
