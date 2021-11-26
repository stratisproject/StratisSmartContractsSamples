using System.Collections.Generic;
using AutoMapper;
using Signature.Shared.Models;
using Signature.Web.Models;

namespace Signature.Web.Extensions
{
    public static class MappingExtensions
    {
        public static List<Contact> ToSharedListModel(this List<ContactViewModel> model)
        {
            return Mapper.Map<List<ContactViewModel>, List<Contact>>(model);
        }

        public static List<ContactViewModel> ToListModel (this List<Contact> model)
        {
            return Mapper.Map<List<Contact>, List<ContactViewModel>>(model);
        }

        public static IEnumerable<DocumentViewModel> ToListModel(this IEnumerable<Document> model)
        {
            return Mapper.Map<IEnumerable<Document>, IEnumerable<DocumentViewModel>>(model);
        }

        public static DocumentDetailViewModel ToSharedModel(this DocumentDetail dto)
        {
            return Mapper.Map<DocumentDetail, DocumentDetailViewModel>(dto);
        }       
    }
}
