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
    public interface IProviderRepository
    {
        Task<int> Count(ProviderFilter ProviderFilter);
        Task<List<Provider>> List(ProviderFilter ProviderFilter);
        Task<Provider> Get(long Id);
        Task<bool> Create(Provider Provider);
        Task<bool> Update(Provider Provider);
        Task<bool> Delete(Provider Provider);
        Task<bool> BulkMerge(List<Provider> Providers);
        Task<bool> BulkDelete(List<Provider> Providers);
    }
    public class ProviderRepository : IProviderRepository
    {
        private DataContext DataContext;
        public ProviderRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProviderDAO> DynamicFilter(IQueryable<ProviderDAO> query, ProviderFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ProviderTypeId != null)
                query = query.Where(q => q.ProviderTypeId, filter.ProviderTypeId);
            if (filter.Value != null)
                query = query.Where(q => q.Value, filter.Value);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ProviderDAO> OrFilter(IQueryable<ProviderDAO> query, ProviderFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<ProviderDAO> initQuery = query.Where(q => false);
            foreach (ProviderFilter ProviderFilter in filter.OrFilter)
            {
                IQueryable<ProviderDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.ProviderTypeId != null)
                    queryable = queryable.Where(q => q.ProviderTypeId, filter.ProviderTypeId);
                if (filter.Value != null)
                    queryable = queryable.Where(q => q.Value, filter.Value);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ProviderDAO> DynamicOrder(IQueryable<ProviderDAO> query, ProviderFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProviderOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProviderOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProviderOrder.ProviderType:
                            query = query.OrderBy(q => q.ProviderTypeId);
                            break;
                        case ProviderOrder.Value:
                            query = query.OrderBy(q => q.Value);
                            break;
                        case ProviderOrder.IsDefault:
                            query = query.OrderBy(q => q.IsDefault);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProviderOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProviderOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProviderOrder.ProviderType:
                            query = query.OrderByDescending(q => q.ProviderTypeId);
                            break;
                        case ProviderOrder.Value:
                            query = query.OrderByDescending(q => q.Value);
                            break;
                        case ProviderOrder.IsDefault:
                            query = query.OrderByDescending(q => q.IsDefault);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Provider>> DynamicSelect(IQueryable<ProviderDAO> query, ProviderFilter filter)
        {
            List<Provider> Providers = await query.Select(q => new Provider()
            {
                Id = filter.Selects.Contains(ProviderSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ProviderSelect.Name) ? q.Name : default(string),
                ProviderTypeId = filter.Selects.Contains(ProviderSelect.ProviderType) ? q.ProviderTypeId : default(long),
                Value = filter.Selects.Contains(ProviderSelect.Value) ? q.Value : default(string),
                IsDefault = filter.Selects.Contains(ProviderSelect.IsDefault) ? q.IsDefault : default(bool),
                ProviderType = filter.Selects.Contains(ProviderSelect.ProviderType) && q.ProviderType != null ? new ProviderType
                {
                    Id = q.ProviderType.Id,
                    Code = q.ProviderType.Code,
                    Name = q.ProviderType.Name,
                } : null,
            }).ToListAsync();
            return Providers;
        }

        public async Task<int> Count(ProviderFilter filter)
        {
            IQueryable<ProviderDAO> Providers = DataContext.Provider;
            Providers = DynamicFilter(Providers, filter);
            return await Providers.CountAsync();
        }

        public async Task<List<Provider>> List(ProviderFilter filter)
        {
            if (filter == null) return new List<Provider>();
            IQueryable<ProviderDAO> ProviderDAOs = DataContext.Provider;
            ProviderDAOs = DynamicFilter(ProviderDAOs, filter);
            ProviderDAOs = DynamicOrder(ProviderDAOs, filter);
            List<Provider> Providers = await DynamicSelect(ProviderDAOs, filter);
            return Providers;
        }

        public async Task<Provider> Get(long Id)
        {
            Provider Provider = await DataContext.Provider.Where(x => x.Id == Id).Select(x => new Provider()
            {
                Id = x.Id,
                Name = x.Name,
                ProviderTypeId = x.ProviderTypeId,
                Value = x.Value,
                IsDefault = x.IsDefault,
                ProviderType = x.ProviderType == null ? null : new ProviderType
                {
                    Id = x.ProviderType.Id,
                    Code = x.ProviderType.Code,
                    Name = x.ProviderType.Name,
                },
            }).FirstOrDefaultAsync();

            if (Provider == null)
                return null;
            Provider.ApplicationUsers = await DataContext.ApplicationUser
                .Where(x => x.ProviderId == Provider.Id)
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

            return Provider;
        }
        public async Task<bool> Create(Provider Provider)
        {
            ProviderDAO ProviderDAO = new ProviderDAO();
            ProviderDAO.Id = Provider.Id;
            ProviderDAO.Name = Provider.Name;
            ProviderDAO.ProviderTypeId = Provider.ProviderTypeId;
            ProviderDAO.Value = Provider.Value;
            ProviderDAO.IsDefault = Provider.IsDefault;
            DataContext.Provider.Add(ProviderDAO);
            await DataContext.SaveChangesAsync();
            Provider.Id = ProviderDAO.Id;
            await SaveReference(Provider);
            return true;
        }

        public async Task<bool> Update(Provider Provider)
        {
            ProviderDAO ProviderDAO = DataContext.Provider.Where(x => x.Id == Provider.Id).FirstOrDefault();
            if (ProviderDAO == null)
                return false;
            ProviderDAO.Id = Provider.Id;
            ProviderDAO.Name = Provider.Name;
            ProviderDAO.ProviderTypeId = Provider.ProviderTypeId;
            ProviderDAO.Value = Provider.Value;
            ProviderDAO.IsDefault = Provider.IsDefault;
            await DataContext.SaveChangesAsync();
            await SaveReference(Provider);
            return true;
        }

        public async Task<bool> Delete(Provider Provider)
        {
            await DataContext.ApplicationUser.Where(x => x.ProviderId == Provider.Id).UpdateFromQueryAsync(x => new ApplicationUserDAO { DeletedAt = StaticParams.DateTimeNow});
            await DataContext.Provider.Where(x => x.Id == Provider.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Provider> Providers)
        {
            List<ProviderDAO> ProviderDAOs = new List<ProviderDAO>();
            foreach (Provider Provider in Providers)
            {
                ProviderDAO ProviderDAO = new ProviderDAO();
                ProviderDAO.Id = Provider.Id;
                ProviderDAO.Name = Provider.Name;
                ProviderDAO.ProviderTypeId = Provider.ProviderTypeId;
                ProviderDAO.Value = Provider.Value;
                ProviderDAO.IsDefault = Provider.IsDefault;
                ProviderDAOs.Add(ProviderDAO);
            }
            await DataContext.BulkMergeAsync(ProviderDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Provider> Providers)
        {
            List<long> Ids = Providers.Select(x => x.Id).ToList();
            await DataContext.ApplicationUser.Where(x => Ids.Contains(x.ProviderId)).UpdateFromQueryAsync(x => new ApplicationUserDAO { DeletedAt = StaticParams.DateTimeNow});
            await DataContext.Provider.Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Provider Provider)
        {
            List<ApplicationUserDAO> ApplicationUserDAOs = await DataContext.ApplicationUser
                .Where(x => x.ProviderId == Provider.Id).ToListAsync();
            ApplicationUserDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Provider.ApplicationUsers != null)
            {
                foreach (ApplicationUser ApplicationUser in Provider.ApplicationUsers)
                {
                    ApplicationUserDAO ApplicationUserDAO = ApplicationUserDAOs
                        .Where(x => x.Id == ApplicationUser.Id && x.Id != 0).FirstOrDefault();
                    if (ApplicationUserDAO == null)
                    {
                        ApplicationUserDAO = new ApplicationUserDAO();
                        ApplicationUserDAO.Id = ApplicationUser.Id;
                        ApplicationUserDAO.Username = ApplicationUser.Username;
                        ApplicationUserDAO.Password = ApplicationUser.Password;
                        ApplicationUserDAO.DisplayName = ApplicationUser.DisplayName;
                        ApplicationUserDAO.Email = ApplicationUser.Email;
                        ApplicationUserDAO.Phone = ApplicationUser.Phone;
                        ApplicationUserDAO.UserStatusId = ApplicationUser.UserStatusId;
                        ApplicationUserDAO.RetryTime = ApplicationUser.RetryTime;
                        ApplicationUserDAO.ProviderId = ApplicationUser.ProviderId;
                        ApplicationUserDAOs.Add(ApplicationUserDAO);
                        ApplicationUserDAO.CreatedAt = StaticParams.DateTimeNow;
                        ApplicationUserDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ApplicationUserDAO.DeletedAt = null;
                    }
                    else
                    {
                        ApplicationUserDAO.Id = ApplicationUser.Id;
                        ApplicationUserDAO.Username = ApplicationUser.Username;
                        ApplicationUserDAO.Password = ApplicationUser.Password;
                        ApplicationUserDAO.DisplayName = ApplicationUser.DisplayName;
                        ApplicationUserDAO.Email = ApplicationUser.Email;
                        ApplicationUserDAO.Phone = ApplicationUser.Phone;
                        ApplicationUserDAO.UserStatusId = ApplicationUser.UserStatusId;
                        ApplicationUserDAO.RetryTime = ApplicationUser.RetryTime;
                        ApplicationUserDAO.ProviderId = ApplicationUser.ProviderId;
                        ApplicationUserDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ApplicationUserDAO.DeletedAt = null;
                    }
                }
                await DataContext.ApplicationUser.BulkMergeAsync(ApplicationUserDAOs);
            }
            
        }
    }
}
