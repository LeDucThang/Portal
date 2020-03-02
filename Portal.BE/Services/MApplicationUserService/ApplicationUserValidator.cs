using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MApplicationUser
{
    public interface IApplicationUserValidator : IServiceScoped
    {
        Task<bool> Create(ApplicationUser ApplicationUser);
        Task<bool> Update(ApplicationUser ApplicationUser);
        Task<bool> Delete(ApplicationUser ApplicationUser);
        Task<bool> BulkDelete(List<ApplicationUser> ApplicationUsers);
        Task<bool> Import(List<ApplicationUser> ApplicationUsers);
    }

    public class ApplicationUserValidator : IApplicationUserValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ApplicationUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ApplicationUser ApplicationUser)
        {
            ApplicationUserFilter ApplicationUserFilter = new ApplicationUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ApplicationUser.Id },
                Selects = ApplicationUserSelect.Id
            };

            int count = await UOW.ApplicationUserRepository.Count(ApplicationUserFilter);
            if (count == 0)
                ApplicationUser.AddError(nameof(ApplicationUserValidator), nameof(ApplicationUser.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ApplicationUser ApplicationUser)
        {
            return ApplicationUser.IsValidated;
        }

        public async Task<bool> Update(ApplicationUser ApplicationUser)
        {
            if (await ValidateId(ApplicationUser))
            {
            }
            return ApplicationUser.IsValidated;
        }

        public async Task<bool> Delete(ApplicationUser ApplicationUser)
        {
            if (await ValidateId(ApplicationUser))
            {
            }
            return ApplicationUser.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ApplicationUser> ApplicationUsers)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ApplicationUser> ApplicationUsers)
        {
            return true;
        }
    }
}
