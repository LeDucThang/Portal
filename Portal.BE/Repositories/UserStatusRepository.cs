using Common;
using Portal.Entities;
using Portal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;

namespace Portal.Repositories
{
    public interface IUserStatusRepository
    {
        Task<int> Count(UserStatusFilter UserStatusFilter);
        Task<List<UserStatus>> List(UserStatusFilter UserStatusFilter);
        Task<UserStatus> Get(long Id);
    }
    public class UserStatusRepository : IUserStatusRepository
    {
        private DataContext DataContext;
        public UserStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<UserStatusDAO> DynamicFilter(IQueryable<UserStatusDAO> query, UserStatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<UserStatusDAO> OrFilter(IQueryable<UserStatusDAO> query, UserStatusFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<UserStatusDAO> initQuery = query.Where(q => false);
            foreach (UserStatusFilter UserStatusFilter in filter.OrFilter)
            {
                IQueryable<UserStatusDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<UserStatusDAO> DynamicOrder(IQueryable<UserStatusDAO> query, UserStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case UserStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case UserStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case UserStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case UserStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case UserStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case UserStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<UserStatus>> DynamicSelect(IQueryable<UserStatusDAO> query, UserStatusFilter filter)
        {
            List<UserStatus> UserStatuses = await query.Select(q => new UserStatus()
            {
                Id = filter.Selects.Contains(UserStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(UserStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(UserStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return UserStatuses;
        }

        public async Task<int> Count(UserStatusFilter filter)
        {
            IQueryable<UserStatusDAO> UserStatuss = DataContext.UserStatus;
            UserStatuss = DynamicFilter(UserStatuss, filter);
            return await UserStatuss.CountAsync();
        }

        public async Task<List<UserStatus>> List(UserStatusFilter filter)
        {
            if (filter == null) return new List<UserStatus>();
            IQueryable<UserStatusDAO> UserStatusDAOs = DataContext.UserStatus;
            UserStatusDAOs = DynamicFilter(UserStatusDAOs, filter);
            UserStatusDAOs = DynamicOrder(UserStatusDAOs, filter);
            List<UserStatus> UserStatuses = await DynamicSelect(UserStatusDAOs, filter);
            return UserStatuses;
        }

        public async Task<UserStatus> Get(long Id)
        {
            UserStatus UserStatus = await DataContext.UserStatus.Where(x => x.Id == Id).Select(x => new UserStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (UserStatus == null)
                return null;
            UserStatus.ApplicationUsers = await DataContext.ApplicationUser
                .Where(x => x.UserStatusId == UserStatus.Id)
                .Select(x => new ApplicationUser
                {
                    Id = x.Id,
                    Username = x.Username,
                    Password = x.Password,
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    Phone = x.Phone,
                    UserStatusId = x.UserStatusId,
                    RetryTime = x.RetryTime,
                    ProviderId = x.ProviderId,
                }).ToListAsync();

            return UserStatus;
        }
    }
}
