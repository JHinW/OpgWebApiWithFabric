using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.FaceCognitive
{
    public class PersonGroupTrain
    {
        private FaceApiService _service;

        public PersonGroupTrain(FaceApiService service)
        {
            _service = service;
        }

        public async Task TrainPersonGroup(string groupId)
        {
            await _service.TrainPersonGroupAsync(groupId);
        }

        public async Task<string> GetPersonGroupTrainingStatusAsync(string groupId)
        {
            while (true)
            {
                var trainingStatus = await _service.GetPersonGroupTrainingStatusAsync(groupId);
                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }
                await Task.Delay(1000);
            }

            return "ok";
            
        }

    }
}
