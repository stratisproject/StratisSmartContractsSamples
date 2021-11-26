using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Signature.BAL.Interface;
using Signature.Web.Extensions;
using Signature.Web.Models;

namespace Signature.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDocumentService _documentService;

        public DashboardController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var documentCounts = await _documentService.GetDocumentCounts(User.GetUserId());

            var unsignedDocumentCount = new DashboardViewModel
            {
                 AwaitingSignCount = documentCounts.AwaitingSignCount,
                 CompletedCount = documentCounts.CompletedCount
            };
            return View(unsignedDocumentCount);
        }
    }
}