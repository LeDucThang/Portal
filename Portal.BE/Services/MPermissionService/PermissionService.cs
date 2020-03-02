using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Portal;
using Portal.Helpers;
using Portal.Repositories;
using Portal.Entities;


namespace Portal.Services.MPermission
{
    public interface IPermissionService :  IServiceScoped
    {
        Task<int> Count(PermissionFilter PermissionFilter);
        Task<List<Permission>> List(PermissionFilter PermissionFilter);
        Task<Permission> Get(long Id);
        Task<Permission> Create(Permission Permission);
        Task<Permission> Update(Permission Permission);
        Task<Permission> Delete(Permission Permission);
        Task<List<Permission>> BulkDelete(List<Permission> Permissions);
        Task<List<Permission>> Import(DataFile DataFile);
        Task<DataFile> Export(PermissionFilter PermissionFilter);
    }

    public class PermissionService : BaseService, IPermissionService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPermissionValidator PermissionValidator;

        public PermissionService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPermissionValidator PermissionValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.PermissionValidator = PermissionValidator;
        }
        public async Task<int> Count(PermissionFilter PermissionFilter)
        {
            try
            {
                int result = await UOW.PermissionRepository.Count(PermissionFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Permission>> List(PermissionFilter PermissionFilter)
        {
            try
            {
                List<Permission> Permissions = await UOW.PermissionRepository.List(PermissionFilter);
                return Permissions;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Permission> Get(long Id)
        {
            Permission Permission = await UOW.PermissionRepository.Get(Id);
            if (Permission == null)
                return null;
            return Permission;
        }
        public async Task<Permission> Create(Permission Permission)
        {
            if (!await PermissionValidator.Create(Permission))
                return Permission;

            try
            {
                await UOW.Begin();
                await UOW.PermissionRepository.Create(Permission);
                await UOW.Commit();

                await Logging.CreateAuditLog(Permission, new { }, nameof(PermissionService));
                return await UOW.PermissionRepository.Get(Permission.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Permission> Update(Permission Permission)
        {
            if (!await PermissionValidator.Update(Permission))
                return Permission;
            try
            {
                var oldData = await UOW.PermissionRepository.Get(Permission.Id);

                await UOW.Begin();
                await UOW.PermissionRepository.Update(Permission);
                await UOW.Commit();

                var newData = await UOW.PermissionRepository.Get(Permission.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(PermissionService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Permission> Delete(Permission Permission)
        {
            if (!await PermissionValidator.Delete(Permission))
                return Permission;

            try
            {
                await UOW.Begin();
                await UOW.PermissionRepository.Delete(Permission);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Permission, nameof(PermissionService));
                return Permission;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Permission>> BulkDelete(List<Permission> Permissions)
        {
            if (!await PermissionValidator.BulkDelete(Permissions))
                return Permissions;

            try
            {
                await UOW.Begin();
                await UOW.PermissionRepository.BulkDelete(Permissions);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Permissions, nameof(PermissionService));
                return Permissions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Permission>> Import(DataFile DataFile)
        {
            List<Permission> Permissions = new List<Permission>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Permissions;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int RoleIdColumn = 2 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string RoleIdValue = worksheet.Cells[i + StartRow, RoleIdColumn].Value?.ToString();
                    Permission Permission = new Permission();
                    Permission.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    Permission.Name = NameValue;
                    Permission.RoleId = long.TryParse(RoleIdValue, out long RoleId) ? RoleId : 0;
                    Permissions.Add(Permission);
                }
            }
            
            if (!await PermissionValidator.Import(Permissions))
                return Permissions;
            
            try
            {
                await UOW.Begin();
                await UOW.PermissionRepository.BulkMerge(Permissions);
                await UOW.Commit();

                await Logging.CreateAuditLog(Permissions, new { }, nameof(PermissionService));
                return Permissions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(PermissionFilter PermissionFilter)
        {
            List<Permission> Permissions = await UOW.PermissionRepository.List(PermissionFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Permission);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int RoleIdColumn = 2 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Permission.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Permission.Name);
                worksheet.Cells[1, RoleIdColumn].Value = nameof(Permission.RoleId);

                for(int i = 0; i < Permissions.Count; i++)
                {
                    Permission Permission = Permissions[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Permission.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Permission.Name;
                    worksheet.Cells[i + StartRow, RoleIdColumn].Value = Permission.RoleId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Page),
                Content = MemoryStream,
            };
            return DataFile;
        }
        
        protected PermissionFilter ToFilter(PermissionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PermissionFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PermissionFilter subFilter = new PermissionFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.RoleId))
                    subFilter.RoleId = Map(subFilter.RoleId, currentFilter.Value);
            }
            return filter;
        }
    }
}
