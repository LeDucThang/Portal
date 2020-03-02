using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MRole
{
    public interface IRoleValidator : IServiceScoped
    {
        Task<bool> Create(Role Role);
        Task<bool> Update(Role Role);
        Task<bool> Delete(Role Role);
        Task<bool> BulkDelete(List<Role> Roles);
        Task<bool> Import(List<Role> Roles);
    }

    public class RoleValidator : IRoleValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public RoleValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Role Role)
        {
            RoleFilter RoleFilter = new RoleFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Role.Id },
                Selects = RoleSelect.Id
            };

            int count = await UOW.RoleRepository.Count(RoleFilter);
            if (count == 0)
                Role.AddError(nameof(RoleValidator), nameof(Role.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Role Role)
        {
            return Role.IsValidated;
        }

        public async Task<bool> Update(Role Role)
        {
            if (await ValidateId(Role))
            {
            }
            return Role.IsValidated;
        }

        public async Task<bool> Delete(Role Role)
        {
            if (await ValidateId(Role))
            {
            }
            return Role.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Role> Roles)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Role> Roles)
        {
            return true;
        }
    }
}
