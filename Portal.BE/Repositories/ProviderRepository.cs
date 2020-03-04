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
            if (filter.GoogleRedirectUri != null)
                query = query.Where(q => q.GoogleRedirectUri, filter.GoogleRedirectUri);
            if (filter.ADIP != null)
                query = query.Where(q => q.ADIP, filter.ADIP);
            if (filter.ADUsername != null)
                query = query.Where(q => q.ADUsername, filter.ADUsername);
            if (filter.ADPassword != null)
                query = query.Where(q => q.ADPassword, filter.ADPassword);
            if (filter.GoogleClientId != null)
                query = query.Where(q => q.GoogleClientId, filter.GoogleClientId);
            if (filter.GoogleClientSecret != null)
                query = query.Where(q => q.GoogleClientSecret, filter.GoogleClientSecret);
            if (filter.MicrosoftClientId != null)
                query = query.Where(q => q.MicrosoftClientId, filter.MicrosoftClientId);
            if (filter.MicrosoftClientSecret != null)
                query = query.Where(q => q.MicrosoftClientSecret, filter.MicrosoftClientSecret);
            if (filter.MicrosoftRedirectUri != null)
                query = query.Where(q => q.MicrosoftRedirectUri, filter.MicrosoftRedirectUri);
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
                if (filter.GoogleRedirectUri != null)
                    queryable = queryable.Where(q => q.GoogleRedirectUri, filter.GoogleRedirectUri);
                if (filter.ADIP != null)
                    queryable = queryable.Where(q => q.ADIP, filter.ADIP);
                if (filter.ADUsername != null)
                    queryable = queryable.Where(q => q.ADUsername, filter.ADUsername);
                if (filter.ADPassword != null)
                    queryable = queryable.Where(q => q.ADPassword, filter.ADPassword);
                if (filter.GoogleClientId != null)
                    queryable = queryable.Where(q => q.GoogleClientId, filter.GoogleClientId);
                if (filter.GoogleClientSecret != null)
                    queryable = queryable.Where(q => q.GoogleClientSecret, filter.GoogleClientSecret);
                if (filter.MicrosoftClientId != null)
                    queryable = queryable.Where(q => q.MicrosoftClientId, filter.MicrosoftClientId);
                if (filter.MicrosoftClientSecret != null)
                    queryable = queryable.Where(q => q.MicrosoftClientSecret, filter.MicrosoftClientSecret);
                if (filter.MicrosoftRedirectUri != null)
                    queryable = queryable.Where(q => q.MicrosoftRedirectUri, filter.MicrosoftRedirectUri);
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
                        case ProviderOrder.GoogleRedirectUri:
                            query = query.OrderBy(q => q.GoogleRedirectUri);
                            break;
                        case ProviderOrder.ADIP:
                            query = query.OrderBy(q => q.ADIP);
                            break;
                        case ProviderOrder.ADUsername:
                            query = query.OrderBy(q => q.ADUsername);
                            break;
                        case ProviderOrder.ADPassword:
                            query = query.OrderBy(q => q.ADPassword);
                            break;
                        case ProviderOrder.GoogleClient:
                            query = query.OrderBy(q => q.GoogleClientId);
                            break;
                        case ProviderOrder.GoogleClientSecret:
                            query = query.OrderBy(q => q.GoogleClientSecret);
                            break;
                        case ProviderOrder.MicrosoftClient:
                            query = query.OrderBy(q => q.MicrosoftClientId);
                            break;
                        case ProviderOrder.MicrosoftClientSecret:
                            query = query.OrderBy(q => q.MicrosoftClientSecret);
                            break;
                        case ProviderOrder.MicrosoftRedirectUri:
                            query = query.OrderBy(q => q.MicrosoftRedirectUri);
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
                        case ProviderOrder.GoogleRedirectUri:
                            query = query.OrderByDescending(q => q.GoogleRedirectUri);
                            break;
                        case ProviderOrder.ADIP:
                            query = query.OrderByDescending(q => q.ADIP);
                            break;
                        case ProviderOrder.ADUsername:
                            query = query.OrderByDescending(q => q.ADUsername);
                            break;
                        case ProviderOrder.ADPassword:
                            query = query.OrderByDescending(q => q.ADPassword);
                            break;
                        case ProviderOrder.GoogleClient:
                            query = query.OrderByDescending(q => q.GoogleClientId);
                            break;
                        case ProviderOrder.GoogleClientSecret:
                            query = query.OrderByDescending(q => q.GoogleClientSecret);
                            break;
                        case ProviderOrder.MicrosoftClient:
                            query = query.OrderByDescending(q => q.MicrosoftClientId);
                            break;
                        case ProviderOrder.MicrosoftClientSecret:
                            query = query.OrderByDescending(q => q.MicrosoftClientSecret);
                            break;
                        case ProviderOrder.MicrosoftRedirectUri:
                            query = query.OrderByDescending(q => q.MicrosoftRedirectUri);
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
                GoogleRedirectUri = filter.Selects.Contains(ProviderSelect.GoogleRedirectUri) ? q.GoogleRedirectUri : default(string),
                ADIP = filter.Selects.Contains(ProviderSelect.ADIP) ? q.ADIP : default(string),
                ADUsername = filter.Selects.Contains(ProviderSelect.ADUsername) ? q.ADUsername : default(string),
                ADPassword = filter.Selects.Contains(ProviderSelect.ADPassword) ? q.ADPassword : default(string),
                GoogleClientId = filter.Selects.Contains(ProviderSelect.GoogleClient) ? q.GoogleClientId : default(string),
                GoogleClientSecret = filter.Selects.Contains(ProviderSelect.GoogleClientSecret) ? q.GoogleClientSecret : default(string),
                MicrosoftClientId = filter.Selects.Contains(ProviderSelect.MicrosoftClient) ? q.MicrosoftClientId : default(string),
                MicrosoftClientSecret = filter.Selects.Contains(ProviderSelect.MicrosoftClientSecret) ? q.MicrosoftClientSecret : default(string),
                MicrosoftRedirectUri = filter.Selects.Contains(ProviderSelect.MicrosoftRedirectUri) ? q.MicrosoftRedirectUri : default(string),
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
                GoogleRedirectUri = x.GoogleRedirectUri,
                ADIP = x.ADIP,
                ADUsername = x.ADUsername,
                ADPassword = x.ADPassword,
                GoogleClientId = x.GoogleClientId,
                GoogleClientSecret = x.GoogleClientSecret,
                MicrosoftClientId = x.MicrosoftClientId,
                MicrosoftClientSecret = x.MicrosoftClientSecret,
                MicrosoftRedirectUri = x.MicrosoftRedirectUri,
            }).FirstOrDefaultAsync();

            if (Provider == null)
                return null;

            return Provider;
        }
        public async Task<bool> Create(Provider Provider)
        {
            ProviderDAO ProviderDAO = new ProviderDAO();
            ProviderDAO.Id = Provider.Id;
            ProviderDAO.Name = Provider.Name;
            ProviderDAO.GoogleRedirectUri = Provider.GoogleRedirectUri;
            ProviderDAO.ADIP = Provider.ADIP;
            ProviderDAO.ADUsername = Provider.ADUsername;
            ProviderDAO.ADPassword = Provider.ADPassword;
            ProviderDAO.GoogleClientId = Provider.GoogleClientId;
            ProviderDAO.GoogleClientSecret = Provider.GoogleClientSecret;
            ProviderDAO.MicrosoftClientId = Provider.MicrosoftClientId;
            ProviderDAO.MicrosoftClientSecret = Provider.MicrosoftClientSecret;
            ProviderDAO.MicrosoftRedirectUri = Provider.MicrosoftRedirectUri;
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
            ProviderDAO.GoogleRedirectUri = Provider.GoogleRedirectUri;
            ProviderDAO.ADIP = Provider.ADIP;
            ProviderDAO.ADUsername = Provider.ADUsername;
            ProviderDAO.ADPassword = Provider.ADPassword;
            ProviderDAO.GoogleClientId = Provider.GoogleClientId;
            ProviderDAO.GoogleClientSecret = Provider.GoogleClientSecret;
            ProviderDAO.MicrosoftClientId = Provider.MicrosoftClientId;
            ProviderDAO.MicrosoftClientSecret = Provider.MicrosoftClientSecret;
            ProviderDAO.MicrosoftRedirectUri = Provider.MicrosoftRedirectUri;
            await DataContext.SaveChangesAsync();
            await SaveReference(Provider);
            return true;
        }

        public async Task<bool> Delete(Provider Provider)
        {
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
                ProviderDAO.GoogleRedirectUri = Provider.GoogleRedirectUri;
                ProviderDAO.ADIP = Provider.ADIP;
                ProviderDAO.ADUsername = Provider.ADUsername;
                ProviderDAO.ADPassword = Provider.ADPassword;
                ProviderDAO.GoogleClientId = Provider.GoogleClientId;
                ProviderDAO.GoogleClientSecret = Provider.GoogleClientSecret;
                ProviderDAO.MicrosoftClientId = Provider.MicrosoftClientId;
                ProviderDAO.MicrosoftClientSecret = Provider.MicrosoftClientSecret;
                ProviderDAO.MicrosoftRedirectUri = Provider.MicrosoftRedirectUri;
                ProviderDAOs.Add(ProviderDAO);
            }
            await DataContext.BulkMergeAsync(ProviderDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Provider> Providers)
        {
            List<long> Ids = Providers.Select(x => x.Id).ToList();
            await DataContext.Provider.Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Provider Provider)
        {
        }
    }
}
