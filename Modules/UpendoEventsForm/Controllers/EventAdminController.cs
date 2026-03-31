using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Upendo.Modules.UpendoEventsForm.Components;
using Upendo.Modules.UpendoEventsForm.Constants;
using Upendo.Modules.UpendoEventsForm.Data;
using Upendo.Modules.UpendoEventsForm.Repository.Contract;

namespace Upendo.Modules.UpendoEventsForm.Controllers
{
    [SupportedModules(ModuleConstants.SupportedModules)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class EventAdminController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(EventAdminController));
        private readonly IEventAdminRepository _repository;

        public EventAdminController(IEventAdminRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ActionName("GetConfig")]
        public IHttpActionResult GetConfig(int portalId)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var result = _repository.GetConfig(effectivePortalId);
                result.IsSuperUser = isSuperUser;

                if (!isSuperUser)
                {
                    result.Portals = (result.Portals ?? Enumerable.Empty<PortalOptionDto>())
                        .Where(x => x.PortalId == effectivePortalId)
                        .ToList();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetFilesByFolder")]
        public IHttpActionResult GetFilesByFolder(int portalId, int folderId)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var result = _repository.GetFilesByFolder(effectivePortalId, folderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetFileById")]
        public IHttpActionResult GetFileById(int portalId, int fileId)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var result = _repository.GetFileById(effectivePortalId, fileId);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetEventForEdit")]
        public IHttpActionResult GetEventForEdit(int portalId, int eventId)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;

                var lookupPortalId = GetEffectivePortalId(portalId, isSuperUser);

                var result = _repository.GetEventForEdit(lookupPortalId, eventId);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("CheckSlugAvailability")]
        public IHttpActionResult CheckSlugAvailability(int portalId, string slug, int? eventId = null)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var isAvailable = _repository.IsSlugAvailable(effectivePortalId, slug, eventId);
                return Ok(new { IsAvailable = isAvailable });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public IHttpActionResult Save(EventFormSaveRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { Message = "The request is required." });
                }

                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(request.PortalId, isSuperUser);

                request.PortalId = effectivePortalId;

                var currentUserId = UserInfo != null ? UserInfo.UserID : -1;
                var eventId = _repository.SaveEvent(request, currentUserId);

                return Ok(new { EventId = eventId });
            }
            catch (ArgumentException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ActionName("CreateCategory")]
        public IHttpActionResult CreateCategory(CreateLookupRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { Message = "The request is required." });
                }

                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(request.PortalId, isSuperUser);

                var result = _repository.CreateCategory(effectivePortalId, request.Name);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ActionName("CreateTag")]
        public IHttpActionResult CreateTag(CreateLookupRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { Message = "The request is required." });
                }

                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(request.PortalId, isSuperUser);

                var result = _repository.CreateTag(effectivePortalId, request.Name);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("SearchVenueSuggestions")]
        public IHttpActionResult SearchVenueSuggestions(int portalId, string q, int limit = 8)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return Ok(Enumerable.Empty<LocationSuggestionDto>());
                }

                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var results = _repository.SearchVenueSuggestions(effectivePortalId, q, limit);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return InternalServerError(ex);
            }
        }

        #region Private Helper Methods

        private int GetEffectivePortalId(int requestedPortalId, bool isSuperUser)
        {
            var currentPortalId = PortalSettings != null ? PortalSettings.PortalId : requestedPortalId;

            if (isSuperUser)
            {
                // only superusers can override security of publishing across portal (until portal groups are implemented)  
                return requestedPortalId > 0 ? requestedPortalId : currentPortalId;
            }

            return currentPortalId;
        }

        private static void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);

                var ctlDnn = new DnnDataHelper();
                ctlDnn.LogErrorToAdminLog(ex);

                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }

        #endregion
    }
}