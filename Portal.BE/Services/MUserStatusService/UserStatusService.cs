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


namespace Portal.Services.MUserStatus
{
    public interface IUserStatusService :  IServiceScoped
    {
        Task<int> Count(UserStatusFilter UserStatusFilter);
        Task<List<UserStatus>> List(UserStatusFilter UserStatusFilter);
        Task<UserStatus> Get(long Id);
    }

    public class UserStatusService : BaseService, IUserStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IUserStatusValidator UserStatusValidator;

        public UserStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IUserStatusValidator UserStatusValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.UserStatusValidator = UserStatusValidator;
        }
        public async Task<int> Count(UserStatusFilter UserStatusFilter)
        {
            try
            {
                int result = await UOW.UserStatusRepository.Count(UserStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UserStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UserStatus>> List(UserStatusFilter UserStatusFilter)
        {
            try
            {
                List<UserStatus> UserStatuss = await UOW.UserStatusRepository.List(UserStatusFilter);
                return UserStatuss;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UserStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<UserStatus> Get(long Id)
        {
            UserStatus UserStatus = await UOW.UserStatusRepository.Get(Id);
            if (UserStatus == null)
                return null;
            return UserStatus;
        }
    }
}
