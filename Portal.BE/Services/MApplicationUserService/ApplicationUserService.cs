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


namespace Portal.Services.MApplicationUser
{
    public interface IApplicationUserService :  IServiceScoped
    {
        Task<int> Count(ApplicationUserFilter ApplicationUserFilter);
        Task<List<ApplicationUser>> List(ApplicationUserFilter ApplicationUserFilter);
        Task<ApplicationUser> Get(long Id);
        Task<ApplicationUser> Create(ApplicationUser ApplicationUser);
        Task<ApplicationUser> Update(ApplicationUser ApplicationUser);
        Task<ApplicationUser> Delete(ApplicationUser ApplicationUser);
        Task<List<ApplicationUser>> BulkDelete(List<ApplicationUser> ApplicationUsers);
        Task<List<ApplicationUser>> Import(DataFile DataFile);
        Task<DataFile> Export(ApplicationUserFilter ApplicationUserFilter);
    }

    public class ApplicationUserService : BaseService, IApplicationUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IApplicationUserValidator ApplicationUserValidator;

        public ApplicationUserService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IApplicationUserValidator ApplicationUserValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ApplicationUserValidator = ApplicationUserValidator;
        }
        public async Task<int> Count(ApplicationUserFilter ApplicationUserFilter)
        {
            try
            {
                int result = await UOW.ApplicationUserRepository.Count(ApplicationUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ApplicationUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ApplicationUser>> List(ApplicationUserFilter ApplicationUserFilter)
        {
            try
            {
                List<ApplicationUser> ApplicationUsers = await UOW.ApplicationUserRepository.List(ApplicationUserFilter);
                return ApplicationUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ApplicationUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ApplicationUser> Get(long Id)
        {
            ApplicationUser ApplicationUser = await UOW.ApplicationUserRepository.Get(Id);
            if (ApplicationUser == null)
                return null;
            return ApplicationUser;
        }
        public async Task<ApplicationUser> Create(ApplicationUser ApplicationUser)
        {
            if (!await ApplicationUserValidator.Create(ApplicationUser))
                return ApplicationUser;

            try
            {
                await UOW.Begin();
                await UOW.ApplicationUserRepository.Create(ApplicationUser);
                await UOW.Commit();

                await Logging.CreateAuditLog(ApplicationUser, new { }, nameof(ApplicationUserService));
                return await UOW.ApplicationUserRepository.Get(ApplicationUser.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ApplicationUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ApplicationUser> Update(ApplicationUser ApplicationUser)
        {
            if (!await ApplicationUserValidator.Update(ApplicationUser))
                return ApplicationUser;
            try
            {
                var oldData = await UOW.ApplicationUserRepository.Get(ApplicationUser.Id);

                await UOW.Begin();
                await UOW.ApplicationUserRepository.Update(ApplicationUser);
                await UOW.Commit();

                var newData = await UOW.ApplicationUserRepository.Get(ApplicationUser.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ApplicationUserService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ApplicationUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ApplicationUser> Delete(ApplicationUser ApplicationUser)
        {
            if (!await ApplicationUserValidator.Delete(ApplicationUser))
                return ApplicationUser;

            try
            {
                await UOW.Begin();
                await UOW.ApplicationUserRepository.Delete(ApplicationUser);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ApplicationUser, nameof(ApplicationUserService));
                return ApplicationUser;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ApplicationUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ApplicationUser>> BulkDelete(List<ApplicationUser> ApplicationUsers)
        {
            if (!await ApplicationUserValidator.BulkDelete(ApplicationUsers))
                return ApplicationUsers;

            try
            {
                await UOW.Begin();
                await UOW.ApplicationUserRepository.BulkDelete(ApplicationUsers);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ApplicationUsers, nameof(ApplicationUserService));
                return ApplicationUsers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ApplicationUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ApplicationUser>> Import(DataFile DataFile)
        {
            List<ApplicationUser> ApplicationUsers = new List<ApplicationUser>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return ApplicationUsers;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int PasswordColumn = 2 + StartColumn;
                int DisplayNameColumn = 3 + StartColumn;
                int EmailColumn = 4 + StartColumn;
                int PhoneColumn = 5 + StartColumn;
                int UserStatusIdColumn = 6 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string UsernameValue = worksheet.Cells[i + StartRow, UsernameColumn].Value?.ToString();
                    string PasswordValue = worksheet.Cells[i + StartRow, PasswordColumn].Value?.ToString();
                    string DisplayNameValue = worksheet.Cells[i + StartRow, DisplayNameColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i + StartRow, EmailColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i + StartRow, PhoneColumn].Value?.ToString();
                    string UserStatusIdValue = worksheet.Cells[i + StartRow, UserStatusIdColumn].Value?.ToString();
                    ApplicationUser ApplicationUser = new ApplicationUser();
                    ApplicationUser.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    ApplicationUser.Username = UsernameValue;
                    ApplicationUser.Password = PasswordValue;
                    ApplicationUser.DisplayName = DisplayNameValue;
                    ApplicationUser.Email = EmailValue;
                    ApplicationUser.Phone = PhoneValue;
                    ApplicationUser.UserStatusId = long.TryParse(UserStatusIdValue, out long UserStatusId) ? UserStatusId : 0;
                    ApplicationUsers.Add(ApplicationUser);
                }
            }
            
            if (!await ApplicationUserValidator.Import(ApplicationUsers))
                return ApplicationUsers;
            
            try
            {
                await UOW.Begin();
                await UOW.ApplicationUserRepository.BulkMerge(ApplicationUsers);
                await UOW.Commit();

                await Logging.CreateAuditLog(ApplicationUsers, new { }, nameof(ApplicationUserService));
                return ApplicationUsers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ApplicationUserService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(ApplicationUserFilter ApplicationUserFilter)
        {
            List<ApplicationUser> ApplicationUsers = await UOW.ApplicationUserRepository.List(ApplicationUserFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(ApplicationUser);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int PasswordColumn = 2 + StartColumn;
                int DisplayNameColumn = 3 + StartColumn;
                int EmailColumn = 4 + StartColumn;
                int PhoneColumn = 5 + StartColumn;
                int UserStatusIdColumn = 6 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(ApplicationUser.Id);
                worksheet.Cells[1, UsernameColumn].Value = nameof(ApplicationUser.Username);
                worksheet.Cells[1, PasswordColumn].Value = nameof(ApplicationUser.Password);
                worksheet.Cells[1, DisplayNameColumn].Value = nameof(ApplicationUser.DisplayName);
                worksheet.Cells[1, EmailColumn].Value = nameof(ApplicationUser.Email);
                worksheet.Cells[1, PhoneColumn].Value = nameof(ApplicationUser.Phone);
                worksheet.Cells[1, UserStatusIdColumn].Value = nameof(ApplicationUser.UserStatusId);

                for(int i = 0; i < ApplicationUsers.Count; i++)
                {
                    ApplicationUser ApplicationUser = ApplicationUsers[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = ApplicationUser.Id;
                    worksheet.Cells[i + StartRow, UsernameColumn].Value = ApplicationUser.Username;
                    worksheet.Cells[i + StartRow, PasswordColumn].Value = ApplicationUser.Password;
                    worksheet.Cells[i + StartRow, DisplayNameColumn].Value = ApplicationUser.DisplayName;
                    worksheet.Cells[i + StartRow, EmailColumn].Value = ApplicationUser.Email;
                    worksheet.Cells[i + StartRow, PhoneColumn].Value = ApplicationUser.Phone;
                    worksheet.Cells[i + StartRow, UserStatusIdColumn].Value = ApplicationUser.UserStatusId;
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
        
        protected ApplicationUserFilter ToFilter(ApplicationUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ApplicationUserFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ApplicationUserFilter subFilter = new ApplicationUserFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Username))
                    subFilter.Username = Map(subFilter.Username, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Password))
                    subFilter.Password = Map(subFilter.Password, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.DisplayName))
                    subFilter.DisplayName = Map(subFilter.DisplayName, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Email))
                    subFilter.Email = Map(subFilter.Email, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Phone))
                    subFilter.Phone = Map(subFilter.Phone, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.UserStatusId))
                    subFilter.UserStatusId = Map(subFilter.UserStatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
