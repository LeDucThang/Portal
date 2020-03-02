using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MProviderType
{
    public interface IProviderTypeValidator : IServiceScoped
    {
        Task<bool> Create(ProviderType ProviderType);
        Task<bool> Update(ProviderType ProviderType);
        Task<bool> Delete(ProviderType ProviderType);
        Task<bool> BulkDelete(List<ProviderType> ProviderTypes);
        Task<bool> Import(List<ProviderType> ProviderTypes);
    }

    public class ProviderTypeValidator : IProviderTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProviderTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ProviderType ProviderType)
        {
            ProviderTypeFilter ProviderTypeFilter = new ProviderTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ProviderType.Id },
                Selects = ProviderTypeSelect.Id
            };

            int count = await UOW.ProviderTypeRepository.Count(ProviderTypeFilter);
            if (count == 0)
                ProviderType.AddError(nameof(ProviderTypeValidator), nameof(ProviderType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ProviderType ProviderType)
        {
            return ProviderType.IsValidated;
        }

        public async Task<bool> Update(ProviderType ProviderType)
        {
            if (await ValidateId(ProviderType))
            {
            }
            return ProviderType.IsValidated;
        }

        public async Task<bool> Delete(ProviderType ProviderType)
        {
            if (await ValidateId(ProviderType))
            {
            }
            return ProviderType.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ProviderType> ProviderTypes)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ProviderType> ProviderTypes)
        {
            return true;
        }
    }
}
