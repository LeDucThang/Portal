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
    public interface IApplicationUserRepository
    {
        Task<int> Count(ApplicationUserFilter ApplicationUserFilter);
        Task<List<ApplicationUser>> List(ApplicationUserFilter ApplicationUserFilter);
        Task<ApplicationUser> Get(long Id);
        Task<bool> Create(ApplicationUser ApplicationUser);
        Task<bool> Update(ApplicationUser ApplicationUser);
        Task<bool> Delete(ApplicationUser ApplicationUser);
        Task<bool> BulkMerge(List<ApplicationUser> ApplicationUsers);
        Task<bool> BulkDelete(List<ApplicationUser> ApplicationUsers);
    }
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private DataContext DataContext;
        public ApplicationUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ApplicationUserDAO> DynamicFilter(IQueryable<ApplicationUserDAO> query, ApplicationUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Username != null)
                query = query.Where(q => q.Username, filter.Username);
            if (filter.Password != null)
                query = query.Where(q => q.Password, filter.Password);
            if (filter.DisplayName != null)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.UserStatusId != null)
                query = query.Where(q => q.UserStatusId, filter.UserStatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ApplicationUserDAO> OrFilter(IQueryable<ApplicationUserDAO> query, ApplicationUserFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<ApplicationUserDAO> initQuery = query.Where(q => false);
            foreach (ApplicationUserFilter ApplicationUserFilter in filter.OrFilter)
            {
                IQueryable<ApplicationUserDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Username != null)
                    queryable = queryable.Where(q => q.Username, filter.Username);
                if (filter.Password != null)
                    queryable = queryable.Where(q => q.Password, filter.Password);
                if (filter.DisplayName != null)
                    queryable = queryable.Where(q => q.DisplayName, filter.DisplayName);
                if (filter.Email != null)
                    queryable = queryable.Where(q => q.Email, filter.Email);
                if (filter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, filter.Phone);
                if (filter.UserStatusId != null)
                    queryable = queryable.Where(q => q.UserStatusId, filter.UserStatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ApplicationUserDAO> DynamicOrder(IQueryable<ApplicationUserDAO> query, ApplicationUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ApplicationUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ApplicationUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case ApplicationUserOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case ApplicationUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case ApplicationUserOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case ApplicationUserOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case ApplicationUserOrder.UserStatus:
                            query = query.OrderBy(q => q.UserStatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ApplicationUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ApplicationUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case ApplicationUserOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case ApplicationUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case ApplicationUserOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case ApplicationUserOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case ApplicationUserOrder.UserStatus:
                            query = query.OrderByDescending(q => q.UserStatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ApplicationUser>> DynamicSelect(IQueryable<ApplicationUserDAO> query, ApplicationUserFilter filter)
        {
            List<ApplicationUser> ApplicationUsers = await query.Select(q => new ApplicationUser()
            {
                Id = filter.Selects.Contains(ApplicationUserSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(ApplicationUserSelect.Username) ? q.Username : default(string),
                Password = filter.Selects.Contains(ApplicationUserSelect.Password) ? q.Password : default(string),
                DisplayName = filter.Selects.Contains(ApplicationUserSelect.DisplayName) ? q.DisplayName : default(string),
                Email = filter.Selects.Contains(ApplicationUserSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(ApplicationUserSelect.Phone) ? q.Phone : default(string),
                UserStatusId = filter.Selects.Contains(ApplicationUserSelect.UserStatus) ? q.UserStatusId : default(long),
                UserStatus = filter.Selects.Contains(ApplicationUserSelect.UserStatus) && q.UserStatus != null ? new UserStatus
                {
                    Id = q.UserStatus.Id,
                    Code = q.UserStatus.Code,
                    Name = q.UserStatus.Name,
                } : null,
            }).ToListAsync();
            return ApplicationUsers;
        }

        public async Task<int> Count(ApplicationUserFilter filter)
        {
            IQueryable<ApplicationUserDAO> ApplicationUsers = DataContext.ApplicationUser;
            ApplicationUsers = DynamicFilter(ApplicationUsers, filter);
            return await ApplicationUsers.CountAsync();
        }

        public async Task<List<ApplicationUser>> List(ApplicationUserFilter filter)
        {
            if (filter == null) return new List<ApplicationUser>();
            IQueryable<ApplicationUserDAO> ApplicationUserDAOs = DataContext.ApplicationUser;
            ApplicationUserDAOs = DynamicFilter(ApplicationUserDAOs, filter);
            ApplicationUserDAOs = DynamicOrder(ApplicationUserDAOs, filter);
            List<ApplicationUser> ApplicationUsers = await DynamicSelect(ApplicationUserDAOs, filter);
            return ApplicationUsers;
        }

        public async Task<ApplicationUser> Get(long Id)
        {
            ApplicationUser ApplicationUser = await DataContext.ApplicationUser.Where(x => x.Id == Id).Select(x => new ApplicationUser()
            {
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                DisplayName = x.DisplayName,
                Email = x.Email,
                Phone = x.Phone,
                UserStatusId = x.UserStatusId,
                UserStatus = x.UserStatus == null ? null : new UserStatus
                {
                    Id = x.UserStatus.Id,
                    Code = x.UserStatus.Code,
                    Name = x.UserStatus.Name,
                },
            }).FirstOrDefaultAsync();

            if (ApplicationUser == null)
                return null;
            ApplicationUser.Roles = await DataContext.UserRoleMapping
                .Where(x => x.ApplicationUserId == ApplicationUser.Id)
                .Select(x => new Role
                {
                    Id = x.Role.Id,
                    Name = x.Role.Name,
                }).ToListAsync();

            return ApplicationUser;
        }
        public async Task<bool> Create(ApplicationUser ApplicationUser)
        {
            ApplicationUserDAO ApplicationUserDAO = new ApplicationUserDAO();
            ApplicationUserDAO.Id = ApplicationUser.Id;
            ApplicationUserDAO.Username = ApplicationUser.Username;
            ApplicationUserDAO.Password = ApplicationUser.Password;
            ApplicationUserDAO.DisplayName = ApplicationUser.DisplayName;
            ApplicationUserDAO.Email = ApplicationUser.Email;
            ApplicationUserDAO.Phone = ApplicationUser.Phone;
            ApplicationUserDAO.UserStatusId = ApplicationUser.UserStatusId;
            ApplicationUserDAO.CreatedAt = StaticParams.DateTimeNow;
            ApplicationUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ApplicationUser.Add(ApplicationUserDAO);
            await DataContext.SaveChangesAsync();
            ApplicationUser.Id = ApplicationUserDAO.Id;
            await SaveReference(ApplicationUser);
            return true;
        }

        public async Task<bool> Update(ApplicationUser ApplicationUser)
        {
            ApplicationUserDAO ApplicationUserDAO = DataContext.ApplicationUser.Where(x => x.Id == ApplicationUser.Id).FirstOrDefault();
            if (ApplicationUserDAO == null)
                return false;
            ApplicationUserDAO.Id = ApplicationUser.Id;
            ApplicationUserDAO.Username = ApplicationUser.Username;
            ApplicationUserDAO.Password = ApplicationUser.Password;
            ApplicationUserDAO.DisplayName = ApplicationUser.DisplayName;
            ApplicationUserDAO.Email = ApplicationUser.Email;
            ApplicationUserDAO.Phone = ApplicationUser.Phone;
            ApplicationUserDAO.UserStatusId = ApplicationUser.UserStatusId;
            ApplicationUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ApplicationUser);
            return true;
        }

        public async Task<bool> Delete(ApplicationUser ApplicationUser)
        {
            await DataContext.UserRoleMapping.Where(x => x.ApplicationUserId == ApplicationUser.Id).DeleteFromQueryAsync();
            await DataContext.ApplicationUser.Where(x => x.Id == ApplicationUser.Id).UpdateFromQueryAsync(x => new ApplicationUserDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ApplicationUser> ApplicationUsers)
        {
            List<ApplicationUserDAO> ApplicationUserDAOs = new List<ApplicationUserDAO>();
            foreach (ApplicationUser ApplicationUser in ApplicationUsers)
            {
                ApplicationUserDAO ApplicationUserDAO = new ApplicationUserDAO();
                ApplicationUserDAO.Id = ApplicationUser.Id;
                ApplicationUserDAO.Username = ApplicationUser.Username;
                ApplicationUserDAO.Password = ApplicationUser.Password;
                ApplicationUserDAO.DisplayName = ApplicationUser.DisplayName;
                ApplicationUserDAO.Email = ApplicationUser.Email;
                ApplicationUserDAO.Phone = ApplicationUser.Phone;
                ApplicationUserDAO.UserStatusId = ApplicationUser.UserStatusId;
                ApplicationUserDAO.CreatedAt = StaticParams.DateTimeNow;
                ApplicationUserDAO.UpdatedAt = StaticParams.DateTimeNow;
                ApplicationUserDAOs.Add(ApplicationUserDAO);
            }
            await DataContext.BulkMergeAsync(ApplicationUserDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ApplicationUser> ApplicationUsers)
        {
            List<long> Ids = ApplicationUsers.Select(x => x.Id).ToList();
            await DataContext.UserRoleMapping.Where(x => Ids.Contains(x.ApplicationUserId)).DeleteFromQueryAsync();
            await DataContext.ApplicationUser.Where(x => Ids.Contains(x.Id)).UpdateFromQueryAsync(x => new ApplicationUserDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ApplicationUser ApplicationUser)
        {
            await DataContext.UserRoleMapping.Where(x => x.ApplicationUserId == ApplicationUser.Id).DeleteFromQueryAsync();
            if (ApplicationUser.Roles != null)
            {
                List<UserRoleMappingDAO> UserRoleMappingDAOs = ApplicationUser.Roles
                    .Select(x => new UserRoleMappingDAO
                    {
                        RoleId = x.Id,
                        ApplicationUserId = ApplicationUser.Id,
                    }).ToList();
                await DataContext.UserRoleMapping.BulkInsertAsync(UserRoleMappingDAOs);
            }
        }
    }
}
