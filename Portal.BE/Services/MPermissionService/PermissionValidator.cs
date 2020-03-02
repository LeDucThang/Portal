using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MPermission
{
    public interface IPermissionValidator : IServiceScoped
    {
        Task<bool> Create(Permission Permission);
        Task<bool> Update(Permission Permission);
        Task<bool> Delete(Permission Permission);
        Task<bool> BulkDelete(List<Permission> Permissions);
        Task<bool> Import(List<Permission> Permissions);
    }

    public class PermissionValidator : IPermissionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PermissionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Permission Permission)
        {
            PermissionFilter PermissionFilter = new PermissionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Permission.Id },
                Selects = PermissionSelect.Id
            };

            int count = await UOW.PermissionRepository.Count(PermissionFilter);
            if (count == 0)
                Permission.AddError(nameof(PermissionValidator), nameof(Permission.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Permission Permission)
        {
            return Permission.IsValidated;
        }

        public async Task<bool> Update(Permission Permission)
        {
            if (await ValidateId(Permission))
            {
            }
            return Permission.IsValidated;
        }

        public async Task<bool> Delete(Permission Permission)
        {
            if (await ValidateId(Permission))
            {
            }
            return Permission.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Permission> Permissions)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Permission> Permissions)
        {
            return true;
        }
    }
}
