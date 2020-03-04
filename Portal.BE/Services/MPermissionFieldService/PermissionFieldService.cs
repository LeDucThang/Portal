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


namespace Portal.Services.MPermissionField
{
    public interface IPermissionFieldService :  IServiceScoped
    {
        Task<int> Count(PermissionFieldFilter PermissionFieldFilter);
        Task<List<PermissionField>> List(PermissionFieldFilter PermissionFieldFilter);
        Task<PermissionField> Get(long Id);
        Task<PermissionField> Create(PermissionField PermissionField);
        Task<PermissionField> Update(PermissionField PermissionField);
        Task<PermissionField> Delete(PermissionField PermissionField);
        Task<List<PermissionField>> BulkDelete(List<PermissionField> PermissionFields);
        Task<List<PermissionField>> Import(DataFile DataFile);
        Task<DataFile> Export(PermissionFieldFilter PermissionFieldFilter);
    }

    public class PermissionFieldService : BaseService, IPermissionFieldService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPermissionFieldValidator PermissionFieldValidator;

        public PermissionFieldService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPermissionFieldValidator PermissionFieldValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.PermissionFieldValidator = PermissionFieldValidator;
        }
        public async Task<int> Count(PermissionFieldFilter PermissionFieldFilter)
        {
            try
            {
                int result = await UOW.PermissionFieldRepository.Count(PermissionFieldFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionFieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<PermissionField>> List(PermissionFieldFilter PermissionFieldFilter)
        {
            try
            {
                List<PermissionField> PermissionFields = await UOW.PermissionFieldRepository.List(PermissionFieldFilter);
                return PermissionFields;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionFieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PermissionField> Get(long Id)
        {
            PermissionField PermissionField = await UOW.PermissionFieldRepository.Get(Id);
            if (PermissionField == null)
                return null;
            return PermissionField;
        }
        public async Task<PermissionField> Create(PermissionField PermissionField)
        {
            if (!await PermissionFieldValidator.Create(PermissionField))
                return PermissionField;

            try
            {
                await UOW.Begin();
                await UOW.PermissionFieldRepository.Create(PermissionField);
                await UOW.Commit();

                await Logging.CreateAuditLog(PermissionField, new { }, nameof(PermissionFieldService));
                return await UOW.PermissionFieldRepository.Get(PermissionField.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionFieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PermissionField> Update(PermissionField PermissionField)
        {
            if (!await PermissionFieldValidator.Update(PermissionField))
                return PermissionField;
            try
            {
                var oldData = await UOW.PermissionFieldRepository.Get(PermissionField.Id);

                await UOW.Begin();
                await UOW.PermissionFieldRepository.Update(PermissionField);
                await UOW.Commit();

                var newData = await UOW.PermissionFieldRepository.Get(PermissionField.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(PermissionFieldService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionFieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PermissionField> Delete(PermissionField PermissionField)
        {
            if (!await PermissionFieldValidator.Delete(PermissionField))
                return PermissionField;

            try
            {
                await UOW.Begin();
                await UOW.PermissionFieldRepository.Delete(PermissionField);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PermissionField, nameof(PermissionFieldService));
                return PermissionField;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionFieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<PermissionField>> BulkDelete(List<PermissionField> PermissionFields)
        {
            if (!await PermissionFieldValidator.BulkDelete(PermissionFields))
                return PermissionFields;

            try
            {
                await UOW.Begin();
                await UOW.PermissionFieldRepository.BulkDelete(PermissionFields);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PermissionFields, nameof(PermissionFieldService));
                return PermissionFields;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionFieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<PermissionField>> Import(DataFile DataFile)
        {
            List<PermissionField> PermissionFields = new List<PermissionField>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return PermissionFields;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int TypeColumn = 2 + StartColumn;
                int ViewIdColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string TypeValue = worksheet.Cells[i + StartRow, TypeColumn].Value?.ToString();
                    string ViewIdValue = worksheet.Cells[i + StartRow, ViewIdColumn].Value?.ToString();
                    PermissionField PermissionField = new PermissionField();
                    PermissionField.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    PermissionField.Name = NameValue;
                    PermissionField.Type = TypeValue;
                    PermissionField.ViewId = long.TryParse(ViewIdValue, out long ViewId) ? ViewId : 0;
                    PermissionFields.Add(PermissionField);
                }
            }
            
            if (!await PermissionFieldValidator.Import(PermissionFields))
                return PermissionFields;
            
            try
            {
                await UOW.Begin();
                await UOW.PermissionFieldRepository.BulkMerge(PermissionFields);
                await UOW.Commit();

                await Logging.CreateAuditLog(PermissionFields, new { }, nameof(PermissionFieldService));
                return PermissionFields;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionFieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(PermissionFieldFilter PermissionFieldFilter)
        {
            List<PermissionField> PermissionFields = await UOW.PermissionFieldRepository.List(PermissionFieldFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(PermissionField);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int TypeColumn = 2 + StartColumn;
                int ViewIdColumn = 3 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(PermissionField.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(PermissionField.Name);
                worksheet.Cells[1, TypeColumn].Value = nameof(PermissionField.Type);
                worksheet.Cells[1, ViewIdColumn].Value = nameof(PermissionField.ViewId);

                for(int i = 0; i < PermissionFields.Count; i++)
                {
                    PermissionField PermissionField = PermissionFields[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = PermissionField.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = PermissionField.Name;
                    worksheet.Cells[i + StartRow, TypeColumn].Value = PermissionField.Type;
                    worksheet.Cells[i + StartRow, ViewIdColumn].Value = PermissionField.ViewId;
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
        
        protected PermissionFieldFilter ToFilter(PermissionFieldFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PermissionFieldFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PermissionFieldFilter subFilter = new PermissionFieldFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Type))
                    subFilter.Type = Map(subFilter.Type, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ViewId))
                    subFilter.ViewId = Map(subFilter.ViewId, currentFilter.Value);
            }
            return filter;
        }
    }
}
