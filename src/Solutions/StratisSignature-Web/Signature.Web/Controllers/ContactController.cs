using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Signature.BAL.Interface;
using Signature.Shared.Models;
using Signature.Utility;
using Signature.Web.Extensions;
using Signature.Web.Models;

namespace Signature.Web.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        #region Properties

        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly IToastNotification _toastNotification;

        #endregion

        #region Constructor

        public ContactController(IUserService userService,
            IContactService contactService,
            IToastNotification toastNotification)
        {
            _userService = userService;
            _contactService = contactService;
            _toastNotification = toastNotification;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index()
        {
            var contacts = await _contactService.GetContacts(User.GetUserId());
            return View(contacts.ToList().ToListModel());
        }

        public IActionResult AddContact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmail(model.Email);
                if (!user.UserId.IsValidGuid())
                {
                    _toastNotification.AddErrorToastMessage(Constants.ContactUserMustbeRegistered);
                    return View();
                }

                var contact = new Contact
                {
                    UserId = user.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    RequestedBy = User.GetUserId(),
                };

                var result = await _contactService.AddContact(contact);
                if (result > 0)
                {
                    _toastNotification.AddSuccessToastMessage(Constants.ContactAddSuccess);
                    return RedirectToAction("Index");
                }
                _toastNotification.AddErrorToastMessage(Constants.RequestProcessFailure);
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var rowAffected = await _contactService.DeleteContact(id);
            if (rowAffected > 0)
            {
                _toastNotification.AddSuccessToastMessage(Constants.ContactDeleteSuccess);
                return RedirectToAction("Index");
            }

            _toastNotification.AddErrorToastMessage(Constants.RequestProcessFailure);
            return View();
        }

        #endregion
    }
}