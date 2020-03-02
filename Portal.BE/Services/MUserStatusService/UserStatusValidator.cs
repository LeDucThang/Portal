using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MUserStatus
{
    public interface IUserStatusValidator : IServiceScoped
    {
        Task<bool> Create(UserStatus UserStatus);
        Task<bool> Update(UserStatus UserStatus);
        Task<bool> Delete(UserStatus UserStatus);
        Task<bool> BulkDelete(List<UserStatus> UserStatuses);
        Task<bool> Import(List<UserStatus> UserStatuses);
    }

    public class UserStatusValidator : IUserStatusValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public UserStatusValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(UserStatus UserStatus)
        {
            UserStatusFilter UserStatusFilter = new UserStatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UserStatus.Id },
                Selects = UserStatusSelect.Id
            };

            int count = await UOW.UserStatusRepository.Count(UserStatusFilter);
            if (count == 0)
                UserStatus.AddError(nameof(UserStatusValidator), nameof(UserStatus.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(UserStatus UserStatus)
        {
            return UserStatus.IsValidated;
        }

        public async Task<bool> Update(UserStatus UserStatus)
        {
            if (await ValidateId(UserStatus))
            {
            }
            return UserStatus.IsValidated;
        }

        public async Task<bool> Delete(UserStatus UserStatus)
        {
            if (await ValidateId(UserStatus))
            {
            }
            return UserStatus.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<UserStatus> UserStatuses)
        {
            return true;
        }
        
        public async Task<bool> Import(List<UserStatus> UserStatuses)
        {
            return true;
        }
    }
}
