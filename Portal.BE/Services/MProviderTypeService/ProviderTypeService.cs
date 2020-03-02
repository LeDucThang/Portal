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


namespace Portal.Services.MProviderType
{
    public interface IProviderTypeService :  IServiceScoped
    {
        Task<int> Count(ProviderTypeFilter ProviderTypeFilter);
        Task<List<ProviderType>> List(ProviderTypeFilter ProviderTypeFilter);
        Task<ProviderType> Get(long Id);
    }

    public class ProviderTypeService : BaseService, IProviderTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProviderTypeValidator ProviderTypeValidator;

        public ProviderTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProviderTypeValidator ProviderTypeValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ProviderTypeValidator = ProviderTypeValidator;
        }
        public async Task<int> Count(ProviderTypeFilter ProviderTypeFilter)
        {
            try
            {
                int result = await UOW.ProviderTypeRepository.Count(ProviderTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ProviderType>> List(ProviderTypeFilter ProviderTypeFilter)
        {
            try
            {
                List<ProviderType> ProviderTypes = await UOW.ProviderTypeRepository.List(ProviderTypeFilter);
                return ProviderTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProviderTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ProviderType> Get(long Id)
        {
            ProviderType ProviderType = await UOW.ProviderTypeRepository.Get(Id);
            if (ProviderType == null)
                return null;
            return ProviderType;
        }
    }
}
