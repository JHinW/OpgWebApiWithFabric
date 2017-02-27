using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using OpgWebApi.Src.Statics;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;

namespace OpgWebApi.Src.Services.FaceCognitive
{
    public class FaceApiService
    {
        public static FaceAttributeType[] DefaultFaceAttributeTypes = new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.HeadPose };

        public int RetryCountOnQuotaLimitError = 6;
        public int RetryDelayOnQuotaLimitError = 500;

        private FaceServiceClient faceClient;

        public Action Throttled;

        public FaceApiService(string apiKey, string endpoint)
        {
            faceClient = new FaceServiceClient(apiKey, endpoint);
        }


        private async Task<TResponse> RunTaskWithAutoRetryOnQuotaLimitExceededError<TResponse>(Func<Task<TResponse>> action)
        {
            int retriesLeft = RetryCountOnQuotaLimitError;
            int delay = RetryDelayOnQuotaLimitError;

            TResponse response = default(TResponse);

            while (true)
            {
                try
                {
                    response = await action();
                    break;
                }
                catch (FaceAPIException exception) when (exception.HttpStatus == (System.Net.HttpStatusCode)429 && retriesLeft > 0)
                {
                    ErrorTrackingHelper.TrackException(exception, "Face API throttling error");
                    if (retriesLeft == 1 && Throttled != null)
                    {
                        Throttled();
                    }

                    await Task.Delay(delay);
                    retriesLeft--;
                    delay *= 2;
                    Console.WriteLine("delay: " + delay);
                    Console.WriteLine("retriesLeft: " + retriesLeft);
                    continue;
                }
                catch (FaceAPIException exception) when (exception.HttpStatus != (System.Net.HttpStatusCode)429 || retriesLeft <= 0)
                {
                    throw exception;
                }
            }

            return response;
        }

        private async Task RunTaskWithAutoRetryOnQuotaLimitExceededError(Func<Task> action)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError<object>(async () => { await action(); return null; });
        }

        public async Task CreatePersonGroupAsync(string personGroupId, string name, string userData)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreatePersonGroupAsync(personGroupId, name, userData));
        }

        public async Task<Person[]> GetPersonsAsync(string personGroupId)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Person[]>(() => faceClient.GetPersonsAsync(personGroupId));
        }

        public async Task<Face[]> DetectAsync(Stream stream, bool returnFaceId = true, bool returnFaceLandmarks = false, IEnumerable<FaceAttributeType> returnFaceAttributes = null)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Face[]>(async () => await faceClient.DetectAsync(stream, returnFaceId, returnFaceLandmarks, returnFaceAttributes));
        }

        public async Task<Face[]> DetectAsync(Func<Task<Stream>> imageStreamCallback, bool returnFaceId = true, bool returnFaceLandmarks = false, IEnumerable<FaceAttributeType> returnFaceAttributes = null)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Face[]>(async () => await faceClient.DetectAsync(await imageStreamCallback(), returnFaceId, returnFaceLandmarks, returnFaceAttributes));
        }

        public async Task<Face[]> DetectAsync(string url, bool returnFaceId = true, bool returnFaceLandmarks = false, IEnumerable<FaceAttributeType> returnFaceAttributes = null)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Face[]>(() => faceClient.DetectAsync(url, returnFaceId, returnFaceLandmarks, returnFaceAttributes));
        }

        public async Task<PersonFace> GetPersonFaceAsync(string personGroupId, Guid personId, Guid face)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<PersonFace>(() => faceClient.GetPersonFaceAsync(personGroupId, personId, face));
        }

        public async Task<IEnumerable<PersonGroup>> GetPersonGroupsAsync(string userDataFilter = null)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<PersonGroup[]>(() => faceClient.ListPersonGroupsAsync())).Where(group => string.IsNullOrEmpty(userDataFilter) || string.Equals(group.UserData, userDataFilter));
        }

        public async Task<PersonGroup> GetPersonGroupsByNameAsync(string groupName = null)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<PersonGroup[]>(() => faceClient.ListPersonGroupsAsync())).Where(group => group.Name == groupName).First();
        }

        public async Task<IEnumerable<FaceListMetadata>> GetFaceListsAsync(string userDataFilter = null)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<FaceListMetadata[]>(() => faceClient.ListFaceListsAsync())).Where(list => string.IsNullOrEmpty(userDataFilter) || string.Equals(list.UserData, userDataFilter));
        }

        public async Task<SimilarPersistedFace[]> FindSimilarAsync(Guid faceId, string faceListId, int maxNumOfCandidatesReturned = 1)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<SimilarPersistedFace[]>(() => faceClient.FindSimilarAsync(faceId, faceListId, maxNumOfCandidatesReturned)));
        }

        public async Task<AddPersistedFaceResult> AddFaceToFaceListAsync(string faceListId, Func<Task<Stream>> imageStreamCallback, FaceRectangle targetFace)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<AddPersistedFaceResult>(async () => await faceClient.AddFaceToFaceListAsync(faceListId, await imageStreamCallback(), null, targetFace)));
        }

        public async Task CreateFaceListAsync(string faceListId, string name, string userData)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreateFaceListAsync(faceListId, name, userData));
        }

        public async Task DeleteFaceListAsync(string faceListId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeleteFaceListAsync(faceListId));
        }

        public async Task UpdatePersonGroupsAsync(string personGroupId, string name, string userData)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.UpdatePersonGroupAsync(personGroupId, name, userData));
        }

        public async Task AddPersonFaceAsync(string personGroupId, Guid personId, string imageUrl, string userData, FaceRectangle targetFace)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.AddPersonFaceAsync(personGroupId, personId, imageUrl, userData, targetFace));
        }

        public async Task AddPersonFaceAsync(string personGroupId, Guid personId, Func<Task<Stream>> imageStreamCallback, string userData, FaceRectangle targetFace)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(async () => await faceClient.AddPersonFaceAsync(personGroupId, personId, await imageStreamCallback(), userData, targetFace));
        }

        public async Task AddPersonFaceAsync(string personGroupId, Guid personId, Stream stream, string userData, FaceRectangle targetFace)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(async () => await faceClient.AddPersonFaceAsync(personGroupId, personId, stream, userData, targetFace));
        }

        public async Task<IdentifyResult[]> IdentifyAsync(string personGroupId, Guid[] detectedFaceIds)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<IdentifyResult[]>(() => faceClient.IdentifyAsync(personGroupId, detectedFaceIds));
        }

        public async Task DeletePersonAsync(string personGroupId, Guid personId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeletePersonAsync(personGroupId, personId));
        }

        public async Task CreatePersonAsync(string personGroupId, string name)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreatePersonAsync(personGroupId, name));
        }

        public async Task<CreatePersonResult> CreatePersonWithResultAsync(string personGroupId, string name)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreatePersonAsync(personGroupId, name));
        }

        public async Task<Person> GetPersonAsync(string personGroupId, Guid personId)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Person>(() => faceClient.GetPersonAsync(personGroupId, personId));
        }

        public async Task TrainPersonGroupAsync(string personGroupId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.TrainPersonGroupAsync(personGroupId));
        }

        public async Task DeletePersonGroupAsync(string personGroupId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeletePersonGroupAsync(personGroupId));
        }

        public async Task DeletePersonFaceAsync(string personGroupId, Guid personId, Guid faceId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeletePersonFaceAsync(personGroupId, personId, faceId));
        }

        public async Task<TrainingStatus> GetPersonGroupTrainingStatusAsync(string personGroupId)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<TrainingStatus>(() => faceClient.GetPersonGroupTrainingStatusAsync(personGroupId));
        }

    }
}
