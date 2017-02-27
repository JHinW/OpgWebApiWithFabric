using Microsoft.AspNetCore.Mvc;
using OpgWebApi.Src.Entitys.Account;
using OpgWebApi.Src.Entitys.Storage;
using OpgWebApi.Src.Models;
using OpgWebApi.Src.Services.FaceCognitive;
using OpgWebApi.Src.Services.FaceCognitiveGroups;
using OpgWebApi.Src.Services.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController
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


        public AccountsController(OpgContext db,
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

        [HttpPut("{name}")]
        public async Task<JsonResult> PutAccount(string name, [FromBody]string content)
        {
            int count = 0;
            try
            {
                var entity = _opgContext.AccountRegisterEntity.Where(x => x.Name == name).FirstOrDefault();
                if(entity == null)
                {
                    _opgContext.AccountRegisterEntity.Add(new AccountRegisterEntity
                    {
                        Id=Guid.NewGuid().ToString(),
                        Name = name,
                        Content = content,
                        CreatedTime = DateTime.UtcNow,
                        UpdatedTime = DateTime.UtcNow
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
                temp.RequestInfo = "Account Register";
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

        [HttpGet("{name}")]
        public async Task<JsonResult> GetAccount(string name)
        {
            int count = 0;
            try
            {
                var entities = _opgContext.AccountRegisterEntity.Where(x => x.Name == name);
                count = entities.Count();
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
                temp.RequestInfo = "Account Register";
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


    }
}
