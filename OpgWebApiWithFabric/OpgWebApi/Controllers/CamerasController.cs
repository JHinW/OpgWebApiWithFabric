
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OpgWebApi.Src.Models.Camera;
using OpgWebApi.Src.Models;
using OpgWebApi.Src.Services.Storage;
using OpgWebApi.Src.Entitys.Camera;
using System;
using System.Threading.Tasks;
using OpgServices.OpgSrc.Models.Camera;
using OpgWebApi.Src.Services.FaceCognitive;
using System.IO;
using System.Linq;
using OpgWebApi.Src.Statics;
using OpgWebApi.Src.Services.FaceCognitiveGroups;
using OpgWebApi.Src.Entitys.Storage;

namespace OpgWebApi.Controllers
{
    [Route("api/[controller]")]
    public class CamerasController
    {
        private OpgContext _opgContext;

        private FaceApiService _faceApiService;

        private NameIdentity _nameIdentity;

        private NameIdentityFromServiceGroup _nameIdentityFromServiceGroup;

        private NameRegisterFromServiceGroup _nameRegisterFromServiceGroup;

        private PersonGroupCreateFromServiceGroup _personGroupCreateFromServiceGroup;

        private PersonGroupTrainFromServiceGroup _personGroupTrainFromServiceGroup;

        private BlobClient _blobClient;

        private TableClient _tableClient;


        public CamerasController(OpgContext db,
            FaceApiService faceApiService,
            NameIdentity service,
            NameIdentityFromServiceGroup nameIdentityFromServiceGroup,
            NameRegisterFromServiceGroup nameRegisterFromServiceGroup,
            PersonGroupCreateFromServiceGroup personGroupCreateFromServiceGroup,
            PersonGroupTrainFromServiceGroup personGroupTrainFromServiceGroup,
            BlobClient blobClient,
            TableClient tableClient
            )
        {
            _opgContext = db;
            _faceApiService = faceApiService;
            _nameIdentity = service;

            _nameIdentityFromServiceGroup = nameIdentityFromServiceGroup;
            _nameRegisterFromServiceGroup = nameRegisterFromServiceGroup;
            _personGroupCreateFromServiceGroup = personGroupCreateFromServiceGroup;
            _personGroupTrainFromServiceGroup = personGroupTrainFromServiceGroup;

            _blobClient = blobClient;
            _tableClient = tableClient;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]UploadedVideoModel model)
        {
            int count = 0;
            try
            {
                _opgContext.CameraEntity.Add(new CameraEntity
                {
                    CampaignId = model.CampaignId,
                    CameraId = model.CameraId,
                    VideoId = model.VideoId,
                    VideoUploadBeginTime = model.VideoUploadBeginTime,
                    VideoUploadEndTime = model.VideoUploadEndTime,
                    VideoUrl = model.VideoUrl,
                    DataArriveTime = DateTime.UtcNow
                });

                count = await _opgContext.SaveChangesAsync();
                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "OK"
                });

            }
            catch (Exception e)
            {
                var temp = new AppServerErrEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                temp.ExceptionSource = e.Source;
                temp.RequestInfo = "Video Upload";
                temp.StackInfo = e.StackTrace;
                temp.Message = e.Message;
                await _tableClient.InsertTableInfo(temp);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Failed"
                });
            }
        }

        [HttpPut("image")]
        public async Task<JsonResult> PutImage([FromBody]ImageModel model)
        {
            int count = 0;
            try
            {
                // upload image to azure storage blob
                var frameBytes = Convert.FromBase64String(model.ImageBase64String);
                using (var stream = new MemoryStream(frameBytes))
                {
                    await _blobClient.UploadStreamAsImage(stream, model.Id);
                }

                return new JsonResult(new
                {
                    EffectLines = 1,
                    MessageStatus = "OK"
                });

            }
            catch (Exception e)
            {
                var temp = new AppServerErrEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                temp.ExceptionSource = e.Source;
                temp.RequestInfo = "Image upload";
                temp.StackInfo = e.StackTrace;
                temp.Message = e.Message;
                await _tableClient.InsertTableInfo(temp);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Failed"
                });
            }
        }

        [HttpPost("image")]
        public async Task<JsonResult> PostImage([FromBody]ImageModel model)
        {
            int count = 0;
            try
            {
                var frameBytes = Convert.FromBase64String(model.ImageBase64String);
                using (var stream = new MemoryStream(frameBytes))
                {
                    var outsWithSubKey = await _nameIdentityFromServiceGroup.CheckNameFromImage(stream);

                    foreach (var tuple in outsWithSubKey.Item1)
                    {
                        var face = tuple.Item2;
                        var person = tuple.Item1;
                        var candidate = tuple.Item3;
                        _opgContext.PersonDetails.Add(new PersonDetailsEntity
                        {
                            Id = Guid.NewGuid().ToString(),
                            MachineName = model.CameraId,
                            FaceId = face.FaceId.ToString(),
                            VideoId = model.VideoId,
                            ImageId = model.Id,
                            Name = person.Name,
                            Age = face.FaceAttributes.Age,
                            Confidence = candidate.Confidence,
                            Gender = face.FaceAttributes.Gender,
                            FacialHair = face.FaceAttributes.FacialHair?.Moustache.ToString(),
                            Glasses = face.FaceAttributes.Glasses.ToString(),
                            Smile = face.FaceAttributes.Smile.ToString(),
                            MonitorTime = model.SentTime,
                            UsedSubKey = outsWithSubKey.Item2
                        });
                        count++;
                    }

                    // upload image to azure storage blob
                    //await _blobClient.UploadStreamAsImage(stream, model.Id);
                }

                return new JsonResult(new
                {
                    EffectLines = 1,
                    MessageStatus = "OK"
                });

            }
            catch (Exception e)
            {
                var temp = new AppServerErrEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                temp.ExceptionSource = e.Source;
                temp.RequestInfo = "Image detect";
                temp.StackInfo = e.StackTrace;
                temp.Message = e.Message;
                await _tableClient.InsertTableInfo(temp);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Failed"
                });
            }
            finally
            {
                _opgContext.ImageEntity.Add(new ImageEntity
                {
                    Id = model.Id,
                    CampaignId = model.CampaignId,
                    CameraId = model.CameraId,
                    VideoId = model.VideoId,
                    SentTime = model.SentTime
                });

                await _opgContext.SaveChangesAsync();
            }
        }

        [HttpPost("Train")]
        public async Task<JsonResult> PostTraining([FromBody]string model)
        {
            int count = 0;
            try
            {
                var entity = _opgContext.PersonGroupEntity.Where(x => x.Name == Configurations.PersonGroup).First();
                await _personGroupTrainFromServiceGroup.TrainPersonGroup(entity.Id);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "OK"
                });

            }
            catch (Exception e)
            {
                var temp = new AppServerErrEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                temp.ExceptionSource = e.Source;
                temp.RequestInfo = "Group Training";
                temp.StackInfo = e.StackTrace;
                temp.Message = e.Message;
                await _tableClient.InsertTableInfo(temp);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Failed"
                });
            }

        }

        [HttpPost("FacesAdd")]
        public async Task<JsonResult> PostFaces([FromBody]ImageForNameModel model)
        {
            var count = 0;
            try
            {
                var frameBytes = Convert.FromBase64String(model.ImageBase64String);
                using (var stream = new MemoryStream(frameBytes))
                {
                    await _nameRegisterFromServiceGroup.RegisterYourNameWithImage(stream, model.name);
                    _opgContext.ImageForNameEntity.Add(new ImageForNameEntity
                    {
                        id = model.id,
                        name = model.name,
                        SentTime = model.SentTime
                    });

                    count = await _opgContext.SaveChangesAsync();

                    return new JsonResult(new
                    {
                        EffectLines = count,
                        MessageStatus = "OK"
                    });
                }
            }
            catch (Exception e)
            {
                var temp = new AppServerErrEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                temp.ExceptionSource = e.Source;
                temp.RequestInfo = "Group Faces Add";
                temp.StackInfo = e.StackTrace;
                temp.Message = e.Message;
                await _tableClient.InsertTableInfo(temp);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Failed"
                });
            }
        }



        [HttpPost("GroupCreation")]
        public async Task<JsonResult> PostGroupCreation([FromBody]string model)
        {
            int count = 0;
            try
            {
                var entity = _opgContext.PersonGroupEntity.Where(x => x.Name == Configurations.PersonGroup).First();
                await _personGroupCreateFromServiceGroup.CreatePersonGroup(entity.Id, entity.Name, entity.UserData);
                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Ok"
                });
            }
            catch (Exception e)
            {
                var temp = new AppServerErrEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                temp.ExceptionSource = e.Source;
                temp.RequestInfo = "Group Creation";
                temp.StackInfo = e.StackTrace;
                temp.Message = e.Message;
                await _tableClient.InsertTableInfo(temp);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Failed"
                });
            }
        }


        [HttpPut("Register/{machineName}")]
        public async Task<JsonResult> Put(string machineName)
        {
            int count = 0;
            try
            {
                var entity = _opgContext.CameraRegisterEntity.Where(e => e.MachineName == machineName).FirstOrDefault();


                /* var entity = from e in _opgContext.CameraRegisterEntity
                              where e.MachineName == machineName
                              select e;
                              */

                if (entity != null)
                {
                    entity.UpdatedTime = DateTime.UtcNow;
                    _opgContext.Update(entity);
                }
                else
                {
                    _opgContext.CameraRegisterEntity.Add(new CameraRegisterEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                        MachineName = machineName,
                        UpdatedTime = DateTime.UtcNow,
                        CreatedTime = DateTime.UtcNow,
                    });
                }

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Ok"
                });
            }
            catch (Exception e)
            {
                var temp = new AppServerErrEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                temp.ExceptionSource = e.Source;
                temp.RequestInfo = "Machine Register";
                temp.StackInfo = e.StackTrace;
                temp.Message = e.Message;
                await _tableClient.InsertTableInfo(temp);

                return new JsonResult(new
                {
                    EffectLines = count,
                    MessageStatus = "Failed"
                });
            }
            finally
            {
                await _opgContext.SaveChangesAsync();
            }

        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}