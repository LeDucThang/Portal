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


namespace Portal.Services.MSite
{
    public interface ISiteService :  IServiceScoped
    {
        Task<int> Count(SiteFilter SiteFilter);
        Task<List<Site>> List(SiteFilter SiteFilter);
        Task<Site> Get(long Id);
        Task<Site> Create(Site Site);
        Task<Site> Update(Site Site);
        Task<Site> Delete(Site Site);
        Task<List<Site>> BulkDelete(List<Site> Sites);
        Task<List<Site>> Import(DataFile DataFile);
        Task<DataFile> Export(SiteFilter SiteFilter);
    }

    public class SiteService : BaseService, ISiteService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ISiteValidator SiteValidator;

        public SiteService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ISiteValidator SiteValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.SiteValidator = SiteValidator;
        }
        public async Task<int> Count(SiteFilter SiteFilter)
        {
            try
            {
                int result = await UOW.SiteRepository.Count(SiteFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Site>> List(SiteFilter SiteFilter)
        {
            try
            {
                List<Site> Sites = await UOW.SiteRepository.List(SiteFilter);
                return Sites;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Site> Get(long Id)
        {
            Site Site = await UOW.SiteRepository.Get(Id);
            if (Site == null)
                return null;
            return Site;
        }
        public async Task<Site> Create(Site Site)
        {
            if (!await SiteValidator.Create(Site))
                return Site;

            try
            {
                await UOW.Begin();
                await UOW.SiteRepository.Create(Site);
                await UOW.Commit();

                await Logging.CreateAuditLog(Site, new { }, nameof(SiteService));
                return await UOW.SiteRepository.Get(Site.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Site> Update(Site Site)
        {
            if (!await SiteValidator.Update(Site))
                return Site;
            try
            {
                var oldData = await UOW.SiteRepository.Get(Site.Id);

                await UOW.Begin();
                await UOW.SiteRepository.Update(Site);
                await UOW.Commit();

                var newData = await UOW.SiteRepository.Get(Site.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(SiteService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Site> Delete(Site Site)
        {
            if (!await SiteValidator.Delete(Site))
                return Site;

            try
            {
                await UOW.Begin();
                await UOW.SiteRepository.Delete(Site);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Site, nameof(SiteService));
                return Site;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Site>> BulkDelete(List<Site> Sites)
        {
            if (!await SiteValidator.BulkDelete(Sites))
                return Sites;

            try
            {
                await UOW.Begin();
                await UOW.SiteRepository.BulkDelete(Sites);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Sites, nameof(SiteService));
                return Sites;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Site>> Import(DataFile DataFile)
        {
            List<Site> Sites = new List<Site>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Sites;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int URLColumn = 2 + StartColumn;
                int StatusColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string URLValue = worksheet.Cells[i + StartRow, URLColumn].Value?.ToString();
                    string StatusValue = worksheet.Cells[i + StartRow, StatusColumn].Value?.ToString();
                    Site Site = new Site();
                    Site.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    Site.Name = NameValue;
                    Site.URL = URLValue;
                    Site.Status = long.TryParse(StatusValue, out long Status) ? Status : 0;
                    Sites.Add(Site);
                }
            }
            
            if (!await SiteValidator.Import(Sites))
                return Sites;
            
            try
            {
                await UOW.Begin();
                await UOW.SiteRepository.BulkMerge(Sites);
                await UOW.Commit();

                await Logging.CreateAuditLog(Sites, new { }, nameof(SiteService));
                return Sites;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(SiteFilter SiteFilter)
        {
            List<Site> Sites = await UOW.SiteRepository.List(SiteFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Site);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int URLColumn = 2 + StartColumn;
                int StatusColumn = 3 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Site.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Site.Name);
                worksheet.Cells[1, URLColumn].Value = nameof(Site.URL);
                worksheet.Cells[1, StatusColumn].Value = nameof(Site.Status);

                for(int i = 0; i < Sites.Count; i++)
                {
                    Site Site = Sites[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Site.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Site.Name;
                    worksheet.Cells[i + StartRow, URLColumn].Value = Site.URL;
                    worksheet.Cells[i + StartRow, StatusColumn].Value = Site.Status;
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
        
        protected SiteFilter ToFilter(SiteFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<SiteFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                SiteFilter subFilter = new SiteFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.URL))
                    subFilter.URL = Map(subFilter.URL, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Status))
                    subFilter.Status = Map(subFilter.Status, currentFilter.Value);
            }
            return filter;
        }
    }
}
