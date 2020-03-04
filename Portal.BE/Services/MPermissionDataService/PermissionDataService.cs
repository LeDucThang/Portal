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


namespace Portal.Services.MPermissionData
{
    public interface IPermissionDataService :  IServiceScoped
    {
        Task<int> Count(PermissionDataFilter PermissionDataFilter);
        Task<List<PermissionData>> List(PermissionDataFilter PermissionDataFilter);
        Task<PermissionData> Get(long Id);
        Task<PermissionData> Create(PermissionData PermissionData);
        Task<PermissionData> Update(PermissionData PermissionData);
        Task<PermissionData> Delete(PermissionData PermissionData);
        Task<List<PermissionData>> BulkDelete(List<PermissionData> PermissionDatas);
        Task<List<PermissionData>> Import(DataFile DataFile);
        Task<DataFile> Export(PermissionDataFilter PermissionDataFilter);
    }

    public class PermissionDataService : BaseService, IPermissionDataService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPermissionDataValidator PermissionDataValidator;

        public PermissionDataService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPermissionDataValidator PermissionDataValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.PermissionDataValidator = PermissionDataValidator;
        }
        public async Task<int> Count(PermissionDataFilter PermissionDataFilter)
        {
            try
            {
                int result = await UOW.PermissionDataRepository.Count(PermissionDataFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionDataService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<PermissionData>> List(PermissionDataFilter PermissionDataFilter)
        {
            try
            {
                List<PermissionData> PermissionDatas = await UOW.PermissionDataRepository.List(PermissionDataFilter);
                return PermissionDatas;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionDataService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PermissionData> Get(long Id)
        {
            PermissionData PermissionData = await UOW.PermissionDataRepository.Get(Id);
            if (PermissionData == null)
                return null;
            return PermissionData;
        }
        public async Task<PermissionData> Create(PermissionData PermissionData)
        {
            if (!await PermissionDataValidator.Create(PermissionData))
                return PermissionData;

            try
            {
                await UOW.Begin();
                await UOW.PermissionDataRepository.Create(PermissionData);
                await UOW.Commit();

                await Logging.CreateAuditLog(PermissionData, new { }, nameof(PermissionDataService));
                return await UOW.PermissionDataRepository.Get(PermissionData.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionDataService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PermissionData> Update(PermissionData PermissionData)
        {
            if (!await PermissionDataValidator.Update(PermissionData))
                return PermissionData;
            try
            {
                var oldData = await UOW.PermissionDataRepository.Get(PermissionData.Id);

                await UOW.Begin();
                await UOW.PermissionDataRepository.Update(PermissionData);
                await UOW.Commit();

                var newData = await UOW.PermissionDataRepository.Get(PermissionData.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(PermissionDataService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionDataService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PermissionData> Delete(PermissionData PermissionData)
        {
            if (!await PermissionDataValidator.Delete(PermissionData))
                return PermissionData;

            try
            {
                await UOW.Begin();
                await UOW.PermissionDataRepository.Delete(PermissionData);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PermissionData, nameof(PermissionDataService));
                return PermissionData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionDataService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<PermissionData>> BulkDelete(List<PermissionData> PermissionDatas)
        {
            if (!await PermissionDataValidator.BulkDelete(PermissionDatas))
                return PermissionDatas;

            try
            {
                await UOW.Begin();
                await UOW.PermissionDataRepository.BulkDelete(PermissionDatas);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PermissionDatas, nameof(PermissionDataService));
                return PermissionDatas;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionDataService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<PermissionData>> Import(DataFile DataFile)
        {
            List<PermissionData> PermissionDatas = new List<PermissionData>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return PermissionDatas;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int PermissionIdColumn = 1 + StartColumn;
                int PermissionFieldIdColumn = 2 + StartColumn;
                int ValueColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string PermissionIdValue = worksheet.Cells[i + StartRow, PermissionIdColumn].Value?.ToString();
                    string PermissionFieldIdValue = worksheet.Cells[i + StartRow, PermissionFieldIdColumn].Value?.ToString();
                    string ValueValue = worksheet.Cells[i + StartRow, ValueColumn].Value?.ToString();
                    PermissionData PermissionData = new PermissionData();
                    PermissionData.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    PermissionData.PermissionId = long.TryParse(PermissionIdValue, out long PermissionId) ? PermissionId : 0;
                    PermissionData.PermissionFieldId = long.TryParse(PermissionFieldIdValue, out long PermissionFieldId) ? PermissionFieldId : 0;
                    PermissionData.Value = ValueValue;
                    PermissionDatas.Add(PermissionData);
                }
            }
            
            if (!await PermissionDataValidator.Import(PermissionDatas))
                return PermissionDatas;
            
            try
            {
                await UOW.Begin();
                await UOW.PermissionDataRepository.BulkMerge(PermissionDatas);
                await UOW.Commit();

                await Logging.CreateAuditLog(PermissionDatas, new { }, nameof(PermissionDataService));
                return PermissionDatas;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionDataService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(PermissionDataFilter PermissionDataFilter)
        {
            List<PermissionData> PermissionDatas = await UOW.PermissionDataRepository.List(PermissionDataFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(PermissionData);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int PermissionIdColumn = 1 + StartColumn;
                int PermissionFieldIdColumn = 2 + StartColumn;
                int ValueColumn = 3 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(PermissionData.Id);
                worksheet.Cells[1, PermissionIdColumn].Value = nameof(PermissionData.PermissionId);
                worksheet.Cells[1, PermissionFieldIdColumn].Value = nameof(PermissionData.PermissionFieldId);
                worksheet.Cells[1, ValueColumn].Value = nameof(PermissionData.Value);

                for(int i = 0; i < PermissionDatas.Count; i++)
                {
                    PermissionData PermissionData = PermissionDatas[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = PermissionData.Id;
                    worksheet.Cells[i + StartRow, PermissionIdColumn].Value = PermissionData.PermissionId;
                    worksheet.Cells[i + StartRow, PermissionFieldIdColumn].Value = PermissionData.PermissionFieldId;
                    worksheet.Cells[i + StartRow, ValueColumn].Value = PermissionData.Value;
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
        
        protected PermissionDataFilter ToFilter(PermissionDataFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PermissionDataFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PermissionDataFilter subFilter = new PermissionDataFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.PermissionId))
                    subFilter.PermissionId = Map(subFilter.PermissionId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.PermissionFieldId))
                    subFilter.PermissionFieldId = Map(subFilter.PermissionFieldId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Value))
                    subFilter.Value = Map(subFilter.Value, currentFilter.Value);
            }
            return filter;
        }
    }
}
