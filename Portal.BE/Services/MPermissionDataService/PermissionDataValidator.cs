using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MPermissionData
{
    public interface IPermissionDataValidator : IServiceScoped
    {
        Task<bool> Create(PermissionData PermissionData);
        Task<bool> Update(PermissionData PermissionData);
        Task<bool> Delete(PermissionData PermissionData);
        Task<bool> BulkDelete(List<PermissionData> PermissionDatas);
        Task<bool> Import(List<PermissionData> PermissionDatas);
    }

    public class PermissionDataValidator : IPermissionDataValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PermissionDataValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(PermissionData PermissionData)
        {
            PermissionDataFilter PermissionDataFilter = new PermissionDataFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = PermissionData.Id },
                Selects = PermissionDataSelect.Id
            };

            int count = await UOW.PermissionDataRepository.Count(PermissionDataFilter);
            if (count == 0)
                PermissionData.AddError(nameof(PermissionDataValidator), nameof(PermissionData.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(PermissionData PermissionData)
        {
            return PermissionData.IsValidated;
        }

        public async Task<bool> Update(PermissionData PermissionData)
        {
            if (await ValidateId(PermissionData))
            {
            }
            return PermissionData.IsValidated;
        }

        public async Task<bool> Delete(PermissionData PermissionData)
        {
            if (await ValidateId(PermissionData))
            {
            }
            return PermissionData.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<PermissionData> PermissionDatas)
        {
            return true;
        }
        
        public async Task<bool> Import(List<PermissionData> PermissionDatas)
        {
            return true;
        }
    }
}
