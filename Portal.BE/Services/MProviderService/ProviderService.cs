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


namespace Portal.Services.MProvider
{
    public interface IProviderService :  IServiceScoped
    {
        Task<int> Count(ProviderFilter ProviderFilter);
        Task<List<Provider>> List(ProviderFilter ProviderFilter);
        Task<Provider> Get(long Id);
        Task<Provider> Create(Provider Provider);
        Task<Provider> Update(Provider Provider);
        Task<Provider> Delete(Provider Provider);
        Task<List<Provider>> BulkDelete(List<Provider> Providers);
        Task<List<Provider>> Import(DataFile DataFile);
        Task<DataFile> Export(ProviderFilter ProviderFilter);
    }

    public class ProviderService : BaseService, IProviderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProviderValidator ProviderValidator;

        public ProviderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProviderValidator ProviderValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ProviderValidator = ProviderValidator;
        }
        public async Task<int> Count(ProviderFilter ProviderFilter)
        {
            try
            {
                int result = await UOW.ProviderRepository.Count(ProviderFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Provider>> List(ProviderFilter ProviderFilter)
        {
            try
            {
                List<Provider> Providers = await UOW.ProviderRepository.List(ProviderFilter);
                return Providers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Provider> Get(long Id)
        {
            Provider Provider = await UOW.ProviderRepository.Get(Id);
            if (Provider == null)
                return null;
            return Provider;
        }
        public async Task<Provider> Create(Provider Provider)
        {
            if (!await ProviderValidator.Create(Provider))
                return Provider;

            try
            {
                await UOW.Begin();
                await UOW.ProviderRepository.Create(Provider);
                await UOW.Commit();

                await Logging.CreateAuditLog(Provider, new { }, nameof(ProviderService));
                return await UOW.ProviderRepository.Get(Provider.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Provider> Update(Provider Provider)
        {
            if (!await ProviderValidator.Update(Provider))
                return Provider;
            try
            {
                var oldData = await UOW.ProviderRepository.Get(Provider.Id);

                await UOW.Begin();
                await UOW.ProviderRepository.Update(Provider);
                await UOW.Commit();

                var newData = await UOW.ProviderRepository.Get(Provider.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProviderService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Provider> Delete(Provider Provider)
        {
            if (!await ProviderValidator.Delete(Provider))
                return Provider;

            try
            {
                await UOW.Begin();
                await UOW.ProviderRepository.Delete(Provider);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Provider, nameof(ProviderService));
                return Provider;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Provider>> BulkDelete(List<Provider> Providers)
        {
            if (!await ProviderValidator.BulkDelete(Providers))
                return Providers;

            try
            {
                await UOW.Begin();
                await UOW.ProviderRepository.BulkDelete(Providers);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Providers, nameof(ProviderService));
                return Providers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Provider>> Import(DataFile DataFile)
        {
            List<Provider> Providers = new List<Provider>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Providers;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int ProviderTypeIdColumn = 2 + StartColumn;
                int ValueColumn = 3 + StartColumn;
                int IsDefaultColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ProviderTypeIdValue = worksheet.Cells[i + StartRow, ProviderTypeIdColumn].Value?.ToString();
                    string ValueValue = worksheet.Cells[i + StartRow, ValueColumn].Value?.ToString();
                    string IsDefaultValue = worksheet.Cells[i + StartRow, IsDefaultColumn].Value?.ToString();
                    Provider Provider = new Provider();
                    Provider.Id = long.TryParse(IdValue, out long Id) ? Id : 0;
                    Provider.Name = NameValue;
                    Provider.ProviderTypeId = long.TryParse(ProviderTypeIdValue, out long ProviderTypeId) ? ProviderTypeId : 0;
                    Provider.Value = ValueValue;
                    Providers.Add(Provider);
                }
            }
            
            if (!await ProviderValidator.Import(Providers))
                return Providers;
            
            try
            {
                await UOW.Begin();
                await UOW.ProviderRepository.BulkMerge(Providers);
                await UOW.Commit();

                await Logging.CreateAuditLog(Providers, new { }, nameof(ProviderService));
                return Providers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(ProviderFilter ProviderFilter)
        {
            List<Provider> Providers = await UOW.ProviderRepository.List(ProviderFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Provider);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int ProviderTypeIdColumn = 2 + StartColumn;
                int ValueColumn = 3 + StartColumn;
                int IsDefaultColumn = 4 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Provider.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Provider.Name);
                worksheet.Cells[1, ProviderTypeIdColumn].Value = nameof(Provider.ProviderTypeId);
                worksheet.Cells[1, ValueColumn].Value = nameof(Provider.Value);
                worksheet.Cells[1, IsDefaultColumn].Value = nameof(Provider.IsDefault);

                for(int i = 0; i < Providers.Count; i++)
                {
                    Provider Provider = Providers[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Provider.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Provider.Name;
                    worksheet.Cells[i + StartRow, ProviderTypeIdColumn].Value = Provider.ProviderTypeId;
                    worksheet.Cells[i + StartRow, ValueColumn].Value = Provider.Value;
                    worksheet.Cells[i + StartRow, IsDefaultColumn].Value = Provider.IsDefault;
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
        
        protected ProviderFilter ToFilter(ProviderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProviderFilter>();
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProviderFilter subFilter = new ProviderFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ProviderTypeId))
                    subFilter.ProviderTypeId = Map(subFilter.ProviderTypeId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Value))
                    subFilter.Value = Map(subFilter.Value, currentFilter.Value);
            }
            return filter;
        }
    }
}
