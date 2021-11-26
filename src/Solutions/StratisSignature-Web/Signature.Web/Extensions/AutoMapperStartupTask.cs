using AutoMapper;
using Signature.Shared.Models;
using Signature.Web.Models;

namespace Signature.Web.Extensions
{
    public static class AutoMapperStartupTask
    {
        public static void Execute()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.ValidateInlineMaps = false;

                    cfg.CreateMap<ContactViewModel, Contact>()
                        .ReverseMap();

                });
        }
    }
}
