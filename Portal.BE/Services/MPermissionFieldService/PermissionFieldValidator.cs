using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MPermissionField
{
    public interface IPermissionFieldValidator : IServiceScoped
    {
        Task<bool> Create(PermissionField PermissionField);
        Task<bool> Update(PermissionField PermissionField);
        Task<bool> Delete(PermissionField PermissionField);
        Task<bool> BulkDelete(List<PermissionField> PermissionFields);
        Task<bool> Import(List<PermissionField> PermissionFields);
    }

    public class PermissionFieldValidator : IPermissionFieldValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PermissionFieldValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(PermissionField PermissionField)
        {
            PermissionFieldFilter PermissionFieldFilter = new PermissionFieldFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = PermissionField.Id },
                Selects = PermissionFieldSelect.Id
            };

            int count = await UOW.PermissionFieldRepository.Count(PermissionFieldFilter);
            if (count == 0)
                PermissionField.AddError(nameof(PermissionFieldValidator), nameof(PermissionField.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(PermissionField PermissionField)
        {
            return PermissionField.IsValidated;
        }

        public async Task<bool> Update(PermissionField PermissionField)
        {
            if (await ValidateId(PermissionField))
            {
            }
            return PermissionField.IsValidated;
        }

        public async Task<bool> Delete(PermissionField PermissionField)
        {
            if (await ValidateId(PermissionField))
            {
            }
            return PermissionField.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<PermissionField> PermissionFields)
        {
            return true;
        }
        
        public async Task<bool> Import(List<PermissionField> PermissionFields)
        {
            return true;
        }
    }
}
