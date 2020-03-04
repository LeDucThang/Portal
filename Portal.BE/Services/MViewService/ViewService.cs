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


namespace Portal.Services.MView
{
    public interface IViewService :  IServiceScoped
    {
        Task<int> Count(ViewFilter ViewFilter);
        Task<List<View>> List(ViewFilter ViewFilter);
        Task<View> Get(long Id);
        Task<View> Create(View View);
        Task<View> Update(View View);
        Task<View> Delete(View View);
        Task<List<View>> BulkDelete(List<View> Views);
        Task<List<View>> Import(DataFile DataFile);
        Task<DataFile> Export(ViewFilter ViewFilter);
    }

    public class ViewService : BaseService, IViewService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IViewValidator ViewValidator;

        public ViewService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IViewValidator ViewValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ViewValidator = ViewValidator;
        }
        public async Task<int> Count(ViewFilter ViewFilter)
        {
            try
            {
                int result = await UOW.ViewRepository.Count(ViewFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ViewService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<View>> List(ViewFilter ViewFilter)
        {
            try
            {
                List<View> Views = await UOW.ViewRepository.List(ViewFilter);
                return Views;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ViewService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<View> Get(long Id)
        {
            View View = await UOW.ViewRepository.Get(Id);
            if (View == null)
                return null;
            return View;
        }
        public async Task<View> Create(View View)
        {
            if (!await ViewValidator.Create(View))
                return View;

            try
            {
                await UOW.Begin();
                await UOW.ViewRepository.Create(View);
                await UOW.Commit();

                await Logging.CreateAuditLog(View, new { }, nameof(ViewService));
                return await UOW.ViewRepository.Get(View.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ViewService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<View> Update(View View)
        {
            if (!await ViewValidator.Update(View))
                return View;
            try
            {
                var oldData = await UOW.ViewRepository.Get(View.Id);

                await UOW.Begin();
                await UOW.ViewRepository.Update(View);
                await UOW.Commit();

                var newData = await UOW.ViewRepository.Get(View.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ViewService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ViewService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<View> Delete(View View)
        {
            if (!await ViewValidator.Delete(View))
                return View;

            try
            {
                await UOW.Begin();
                await UOW.ViewRepository.Delete(View);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, View, nameof(ViewService));
                return View;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ViewService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<View>> BulkDelete(List<View> Views)
        {
            if (!await ViewValidator.BulkDelete(Views))
                return Views;

            try
            {
                await UOW.Begin();
                await UOW.ViewRepository.BulkDelete(Views);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Views, nameof(ViewService));
                return Views;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ViewService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<View>> Import(DataFile DataFile)
        {
            List<View> Views = new List<View>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Views;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PathColumn = 2 + StartColumn;
                int IsDeletedColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PathValue = worksheet.Cells[i + StartRow, PathColumn].Value?.ToString();
                    string IsDeletedValue = worksheet.Cells[i + StartRow, IsDeletedColumn].Value?.ToString();
                    View View = new View();
                    View.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    View.Name = NameValue;
                    View.Path = PathValue;
                    Views.Add(View);
                }
            }
            
            if (!await ViewValidator.Import(Views))
                return Views;
            
            try
            {
                await UOW.Begin();
                await UOW.ViewRepository.BulkMerge(Views);
                await UOW.Commit();

                await Logging.CreateAuditLog(Views, new { }, nameof(ViewService));
                return Views;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ViewService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(ViewFilter ViewFilter)
        {
            List<View> Views = await UOW.ViewRepository.List(ViewFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(View);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PathColumn = 2 + StartColumn;
                int IsDeletedColumn = 3 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(View.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(View.Name);
                worksheet.Cells[1, PathColumn].Value = nameof(View.Path);
                worksheet.Cells[1, IsDeletedColumn].Value = nameof(View.IsDeleted);

                for(int i = 0; i < Views.Count; i++)
                {
                    View View = Views[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = View.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = View.Name;
                    worksheet.Cells[i + StartRow, PathColumn].Value = View.Path;
                    worksheet.Cells[i + StartRow, IsDeletedColumn].Value = View.IsDeleted;
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
        
        protected ViewFilter ToFilter(ViewFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ViewFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ViewFilter subFilter = new ViewFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Path))
                    subFilter.Path = Map(subFilter.Path, currentFilter.Value);
            }
            return filter;
        }
    }
}
