using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Upendo.Modules.UpendoEventsForm.Data;

namespace Upendo.Modules.UpendoEventsForm.Components
{
    internal class DnnDataHelper
    {
        internal IList<PortalOptionDto> GetPortals()
        {
            var portals = PortalController.Instance
                .GetPortals()
                .Cast<PortalInfo>()
                .Where(p => p != null)
                .Where(p => !string.IsNullOrWhiteSpace(p.PortalName))
                .OrderBy(p => p.PortalName)
                .Select(p => new PortalOptionDto
                {
                    PortalId = p.PortalID,
                    PortalName = p.PortalName
                })
                .ToList();

            return portals;
        }

        internal IList<FolderOptionDto> GetFolders(int portalId)
        {
            if (portalId <= 0)
            {
                return new List<FolderOptionDto>();
            }

            var folders = FolderManager.Instance
                .GetFolders(portalId)
                .Where(f => f != null)
                .Where(f => !IsFolderInTrash(f))
                .OrderBy(f => NormalizeFolderPathForSort(f.FolderPath))
                .Select(f => new FolderOptionDto
                {
                    FolderId = f.FolderID,
                    PortalId = f.PortalID,
                    FolderName = GetFolderDisplayLabel(f),
                    FolderPath = f.FolderPath
                })
                .ToList();

            return folders;
        }

        internal IList<EventFileOptionDto> GetFiles(int portalId)
        {
            if (portalId <= 0)
            {
                return new List<EventFileOptionDto>();
            }

            var folders = FolderManager.Instance
                .GetFolders(portalId)
                .Where(f => f != null)
                .Where(f => !IsFolderInTrash(f))
                .ToList();

            var files = new List<EventFileOptionDto>();

            foreach (var folder in folders)
            {
                var folderFiles = FolderManager.Instance
                    .GetFiles(folder)
                    .Where(file => file != null)
                    .OrderBy(file => file.FileName);

                foreach (var file in folderFiles)
                {
                    files.Add(new EventFileOptionDto
                    {
                        FileId = file.FileId,
                        PortalId = portalId,
                        FolderId = folder.FolderID,
                        FolderName = GetFolderDisplayName(folder),
                        FolderPath = folder.FolderPath,
                        FileName = file.FileName,
                        RelativeUrl = FileManager.Instance.GetUrl(file)
                    });
                }
            }

            return files
                .OrderBy(f => NormalizeFolderPathForSort(f.FolderPath))
                .ThenBy(f => f.FileName)
                .ToList();
        }

        internal IList<EventFileOptionDto> GetFilesByFolder(int portalId, int folderId)
        {
            if (portalId <= 0 || folderId <= 0)
            {
                return new List<EventFileOptionDto>();
            }

            var folder = FolderManager.Instance.GetFolder(folderId);
            if (folder == null || folder.PortalID != portalId || IsFolderInTrash(folder))
            {
                return new List<EventFileOptionDto>();
            }

            return FolderManager.Instance
                .GetFiles(folder)
                .Where(file => file != null)
                .OrderBy(file => file.FileName)
                .Select(file => MapFileOption(file, folder, portalId))
                .ToList();
        }

        internal EventFileOptionDto GetFileById(int portalId, int fileId)
        {
            if (portalId <= 0 || fileId <= 0)
            {
                return null;
            }

            var file = FileManager.Instance.GetFile(fileId);
            if (file == null)
            {
                return null;
            }

            var folder = FolderManager.Instance.GetFolder(file.FolderId);
            if (folder == null || folder.PortalID != portalId || IsFolderInTrash(folder))
            {
                return null;
            }

            return MapFileOption(file, folder, portalId);
        }

        internal List<UserOptionDto> GetPortalUsers(int portalId)
        {
            var results = new List<UserOptionDto>();

            if (portalId <= 0)
            {
                return results;
            }

            var users = UserController.GetUsers(portalId);

            if (users == null)
            {
                return results;
            }

            results = users
                .Cast<UserInfo>()
                .Where(x => x != null && !x.IsDeleted)
                .Select(x => new UserOptionDto
                {
                    UserId = x.UserID,
                    DisplayName = !string.IsNullOrWhiteSpace(x.DisplayName)
                        ? x.DisplayName.Trim()
                        : (!string.IsNullOrWhiteSpace(x.Username) ? x.Username.Trim() : string.Empty),
                    Email = x.Email
                })
                .OrderBy(x => x.DisplayName)
                .ThenBy(x => x.Email)
                .ToList();

            return results;
        }

        internal void LogErrorToAdminLog(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            try
            {
                var portalSettings = PortalSettings.Current;
                var logEntry = new LogInfo
                {
                    LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString(),
                    LogPortalID = portalSettings?.PortalId ?? -1,
                    LogUserID = portalSettings?.UserId ?? -1,
                    BypassBuffering = true,
                };

                logEntry.AddProperty("ExceptionType", ex.GetType().FullName ?? string.Empty);
                logEntry.AddProperty("Message", ex.Message ?? string.Empty);
                logEntry.AddProperty("StackTrace", ex.StackTrace ?? string.Empty);
                logEntry.AddProperty("Source", ex.Source ?? string.Empty);

                if (ex.InnerException != null)
                {
                    logEntry.AddProperty("InnerException", ex.InnerException.ToString());
                }

                EventLogController.Instance.AddLog(logEntry);
            }
            catch (Exception loggingException)
            {
                Exceptions.LogException(loggingException);
            }
        }

        internal bool FileBelongsToPortal(int portalId, int? fileId)
        {
            if (portalId <= 0 || !fileId.HasValue || fileId.Value <= 0)
            {
                return false;
            }

            return GetFileById(portalId, fileId.Value) != null;
        }

        private static bool IsFolderInTrash(IFolderInfo folder)
        {
            if (folder == null)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(folder.FolderPath) &&
                folder.FolderPath.IndexOf("_Trash", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        private static string GetFolderDisplayName(IFolderInfo folder)
        {
            if (folder == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(folder.FolderName))
            {
                return folder.FolderName;
            }

            if (!string.IsNullOrWhiteSpace(folder.FolderPath))
            {
                var trimmed = folder.FolderPath.Trim('/').Trim();
                return string.IsNullOrWhiteSpace(trimmed) ? folder.FolderPath : trimmed;
            }

            return string.Empty;
        }

        private static string GetFolderSortName(IFolderInfo folder)
        {
            return GetFolderDisplayName(folder) ?? string.Empty;
        }

        private static string NormalizeFolderPathForSort(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return "/";
            }

            var normalized = folderPath.Replace("\\", "/").Trim();

            if (!normalized.StartsWith("/"))
            {
                normalized = string.Concat("/", normalized);
            }

            if (normalized.Length > 1 && normalized.EndsWith("/"))
            {
                normalized = normalized.TrimEnd('/');
            }

            return normalized;
        }

        private static EventFileOptionDto MapFileOption(IFileInfo file, IFolderInfo folder, int portalId)
        {
            return new EventFileOptionDto
            {
                FileId = file.FileId,
                PortalId = portalId,
                FolderId = folder.FolderID,
                FolderName = GetFolderDisplayName(folder),
                FolderPath = folder.FolderPath,
                FileName = file.FileName,
                RelativeUrl = FileManager.Instance.GetUrl(file)
            };
        }

        private static string GetFolderDisplayLabel(IFolderInfo folder)
        {
            if (folder == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(folder.FolderPath))
            {
                return "[Root Folder]";
            }

            var normalized = folder.FolderPath.Replace("\\", "/").Trim();

            if (normalized.EndsWith("/"))
            {
                normalized = normalized.TrimEnd('/');
            }

            return string.IsNullOrWhiteSpace(normalized)
                ? "Root Folder"
                : normalized;
        }
    }
}