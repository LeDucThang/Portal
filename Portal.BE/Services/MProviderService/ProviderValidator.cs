using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MProvider
{
    public interface IProviderValidator : IServiceScoped
    {
        Task<bool> Create(Provider Provider);
        Task<bool> Update(Provider Provider);
        Task<bool> Delete(Provider Provider);
        Task<bool> BulkDelete(List<Provider> Providers);
        Task<bool> Import(List<Provider> Providers);
    }

    public class ProviderValidator : IProviderValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProviderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Provider Provider)
        {
            ProviderFilter ProviderFilter = new ProviderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Provider.Id },
                Selects = ProviderSelect.Id
            };

            int count = await UOW.ProviderRepository.Count(ProviderFilter);
            if (count == 0)
                Provider.AddError(nameof(ProviderValidator), nameof(Provider.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Provider Provider)
        {
            return Provider.IsValidated;
        }

        public async Task<bool> Update(Provider Provider)
        {
            if (await ValidateId(Provider))
            {
            }
            return Provider.IsValidated;
        }

        public async Task<bool> Delete(Provider Provider)
        {
            if (await ValidateId(Provider))
            {
            }
            return Provider.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Provider> Providers)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Provider> Providers)
        {
            return true;
        }
    }
}
