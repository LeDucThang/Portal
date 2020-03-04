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


namespace Portal.Services.MPage
{
    public interface IPageService :  IServiceScoped
    {
        Task<int> Count(PageFilter PageFilter);
        Task<List<Page>> List(PageFilter PageFilter);
        Task<Page> Get(long Id);
        Task<Page> Create(Page Page);
        Task<Page> Update(Page Page);
        Task<Page> Delete(Page Page);
        Task<List<Page>> BulkDelete(List<Page> Pages);
        Task<List<Page>> Import(DataFile DataFile);
        Task<DataFile> Export(PageFilter PageFilter);
    }

    public class PageService : BaseService, IPageService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPageValidator PageValidator;

        public PageService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPageValidator PageValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.PageValidator = PageValidator;
        }
        public async Task<int> Count(PageFilter PageFilter)
        {
            try
            {
                int result = await UOW.PageRepository.Count(PageFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Page>> List(PageFilter PageFilter)
        {
            try
            {
                List<Page> Pages = await UOW.PageRepository.List(PageFilter);
                return Pages;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Page> Get(long Id)
        {
            Page Page = await UOW.PageRepository.Get(Id);
            if (Page == null)
                return null;
            return Page;
        }
        public async Task<Page> Create(Page Page)
        {
            if (!await PageValidator.Create(Page))
                return Page;

            try
            {
                await UOW.Begin();
                await UOW.PageRepository.Create(Page);
                await UOW.Commit();

                await Logging.CreateAuditLog(Page, new { }, nameof(PageService));
                return await UOW.PageRepository.Get(Page.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Page> Update(Page Page)
        {
            if (!await PageValidator.Update(Page))
                return Page;
            try
            {
                var oldData = await UOW.PageRepository.Get(Page.Id);

                await UOW.Begin();
                await UOW.PageRepository.Update(Page);
                await UOW.Commit();

                var newData = await UOW.PageRepository.Get(Page.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(PageService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Page> Delete(Page Page)
        {
            if (!await PageValidator.Delete(Page))
                return Page;

            try
            {
                await UOW.Begin();
                await UOW.PageRepository.Delete(Page);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Page, nameof(PageService));
                return Page;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Page>> BulkDelete(List<Page> Pages)
        {
            if (!await PageValidator.BulkDelete(Pages))
                return Pages;

            try
            {
                await UOW.Begin();
                await UOW.PageRepository.BulkDelete(Pages);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Pages, nameof(PageService));
                return Pages;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Page>> Import(DataFile DataFile)
        {
            List<Page> Pages = new List<Page>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Pages;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PathColumn = 2 + StartColumn;
                int ViewIdColumn = 3 + StartColumn;
                int IsDeletedColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PathValue = worksheet.Cells[i + StartRow, PathColumn].Value?.ToString();
                    string ViewIdValue = worksheet.Cells[i + StartRow, ViewIdColumn].Value?.ToString();
                    string IsDeletedValue = worksheet.Cells[i + StartRow, IsDeletedColumn].Value?.ToString();
                    Page Page = new Page();
                    Page.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    Page.Name = NameValue;
                    Page.Path = PathValue;
                    Page.ViewId = long.TryParse(ViewIdValue, out long ViewId) ? ViewId : 0;
                    Pages.Add(Page);
                }
            }
            
            if (!await PageValidator.Import(Pages))
                return Pages;
            
            try
            {
                await UOW.Begin();
                await UOW.PageRepository.BulkMerge(Pages);
                await UOW.Commit();

                await Logging.CreateAuditLog(Pages, new { }, nameof(PageService));
                return Pages;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(PageFilter PageFilter)
        {
            List<Page> Pages = await UOW.PageRepository.List(PageFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Page);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PathColumn = 2 + StartColumn;
                int ViewIdColumn = 3 + StartColumn;
                int IsDeletedColumn = 4 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Page.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Page.Name);
                worksheet.Cells[1, PathColumn].Value = nameof(Page.Path);
                worksheet.Cells[1, ViewIdColumn].Value = nameof(Page.ViewId);
                worksheet.Cells[1, IsDeletedColumn].Value = nameof(Page.IsDeleted);

                for(int i = 0; i < Pages.Count; i++)
                {
                    Page Page = Pages[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Page.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Page.Name;
                    worksheet.Cells[i + StartRow, PathColumn].Value = Page.Path;
                    worksheet.Cells[i + StartRow, ViewIdColumn].Value = Page.ViewId;
                    worksheet.Cells[i + StartRow, IsDeletedColumn].Value = Page.IsDeleted;
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
        
        protected PageFilter ToFilter(PageFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PageFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PageFilter subFilter = new PageFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Path))
                    subFilter.Path = Map(subFilter.Path, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ViewId))
                    subFilter.ViewId = Map(subFilter.ViewId, currentFilter.Value);
            }
            return filter;
        }
    }
}
