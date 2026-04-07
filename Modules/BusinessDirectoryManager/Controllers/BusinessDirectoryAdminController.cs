/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Upendo.Modules.BusinessDirectoryManager.Constants;
using Upendo.Modules.BusinessDirectoryManager.Data;
using Upendo.Modules.BusinessDirectoryManager.Repository.Contract;

namespace Upendo.Modules.BusinessDirectoryManager.Controllers
{
    [SupportedModules(ModuleConstants.SupportedModules)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class BusinessDirectoryAdminController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(BusinessDirectoryAdminController));
        private readonly IBusinessDirectoryAdminRepository _repository;

        public BusinessDirectoryAdminController(IBusinessDirectoryAdminRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ActionName("GetConfig")]
        public IHttpActionResult GetConfig(int portalId, bool includeCategories = true)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var result = _repository.GetConfig(effectivePortalId, includeCategories);
                result.IsSuperUser = isSuperUser;

                if (!isSuperUser)
                {
                    result.Portals = (result.Portals ?? Enumerable.Empty<PortalOptionDto>())
                        .Where(x => x.PortalId == effectivePortalId)
                        .ToList();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        [HttpPost]
        [ActionName("GetBusinessesPage")]
        public IHttpActionResult GetBusinessesPage(BusinessDirectoryListRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("The request is required.");
                }

                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(request.PortalId, isSuperUser);

                request.PortalId = effectivePortalId;
                request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                if (request.PageSize > 100)
                {
                    request.PageSize = 100;
                }

                var result = _repository.GetBusinessesPage(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("GetBusinessForEdit")]
        public IHttpActionResult GetBusinessForEdit(int portalId, int companyId)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var result = _repository.GetBusinessForEdit(effectivePortalId, companyId);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("CheckSlugAvailability")]
        public IHttpActionResult CheckSlugAvailability(int portalId, string slug, int? companyId = null)
        {
            try
            {
                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(portalId, isSuperUser);

                var result = _repository.CheckSlugAvailability(effectivePortalId, slug, companyId);
                return Ok(result);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public IHttpActionResult Save(BusinessDirectorySaveRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("The request is required.");
                }

                var isSuperUser = UserInfo != null && UserInfo.IsSuperUser;
                var effectivePortalId = GetEffectivePortalId(request.PortalId, isSuperUser);
                request.PortalId = effectivePortalId;

                var currentUserId = UserInfo != null ? UserInfo.UserID : -1;
                var companyId = _repository.SaveBusiness(request, currentUserId);

                return Ok(new { CompanyId = companyId });
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        #region Private Helper Methods 

        private int GetEffectivePortalId(int requestedPortalId, bool isSuperUser)
        {
            if (isSuperUser && requestedPortalId > 0)
            {
                return requestedPortalId;
            }

            return PortalSettings.PortalId;
        }

        private static void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);

                // TODO: Uncomment when we move the common logic to a shared library
                //var ctlDnn = new DnnDataHelper();
                //ctlDnn.LogErrorToAdminLog(ex);

                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }

        #endregion
    }
}