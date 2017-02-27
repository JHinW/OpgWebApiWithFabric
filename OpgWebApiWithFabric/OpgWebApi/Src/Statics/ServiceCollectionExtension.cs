using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpgWebApi.Src.Services.FaceCognitive;
using OpgWebApi.Src.Services.FaceCognitiveGroups;
using OpgWebApi.Src.Services.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Statics
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAzureStorageServer(this IServiceCollection services, string connectionStr)
        {
            services.AddSingleton<AzureStorageClient>(new AzureStorageClient(connectionStr));

            services.AddSingleton<BlobClient>();

            services.AddSingleton<TableClient>();

            return services;
        }

        public static IServiceCollection AddFaceApiServices(this IServiceCollection services, IEnumerable<IConfigurationSection> configs)
        {
            foreach (var config in configs)
            {
                services.AddSingleton<FaceApiServiceGroup>(new FaceApiServiceGroup(config["ApiKey"], config["ApiRoot"]));
            }

            services.AddSingleton<NameIdentityFromServiceGroup>();

            services.AddSingleton<NameRegisterFromServiceGroup>();

            services.AddSingleton<PersonGroupCreateFromServiceGroup>();

            services.AddSingleton<PersonGroupTrainFromServiceGroup>();

            services.AddSingleton<NameIdentity>();

            return services;
        }


    }
}
