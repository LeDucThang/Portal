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
    public interface IProviderTypeRepository
    {
        Task<int> Count(ProviderTypeFilter ProviderTypeFilter);
        Task<List<ProviderType>> List(ProviderTypeFilter ProviderTypeFilter);
        Task<ProviderType> Get(long Id);
    }
    public class ProviderTypeRepository : IProviderTypeRepository
    {
        private DataContext DataContext;
        public ProviderTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProviderTypeDAO> DynamicFilter(IQueryable<ProviderTypeDAO> query, ProviderTypeFilter filter)
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

         private IQueryable<ProviderTypeDAO> OrFilter(IQueryable<ProviderTypeDAO> query, ProviderTypeFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<ProviderTypeDAO> initQuery = query.Where(q => false);
            foreach (ProviderTypeFilter ProviderTypeFilter in filter.OrFilter)
            {
                IQueryable<ProviderTypeDAO> queryable = query;
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

        private IQueryable<ProviderTypeDAO> DynamicOrder(IQueryable<ProviderTypeDAO> query, ProviderTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProviderTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProviderTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProviderTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProviderTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProviderTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProviderTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ProviderType>> DynamicSelect(IQueryable<ProviderTypeDAO> query, ProviderTypeFilter filter)
        {
            List<ProviderType> ProviderTypes = await query.Select(q => new ProviderType()
            {
                Id = filter.Selects.Contains(ProviderTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProviderTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ProviderTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ProviderTypes;
        }

        public async Task<int> Count(ProviderTypeFilter filter)
        {
            IQueryable<ProviderTypeDAO> ProviderTypes = DataContext.ProviderType;
            ProviderTypes = DynamicFilter(ProviderTypes, filter);
            return await ProviderTypes.CountAsync();
        }

        public async Task<List<ProviderType>> List(ProviderTypeFilter filter)
        {
            if (filter == null) return new List<ProviderType>();
            IQueryable<ProviderTypeDAO> ProviderTypeDAOs = DataContext.ProviderType;
            ProviderTypeDAOs = DynamicFilter(ProviderTypeDAOs, filter);
            ProviderTypeDAOs = DynamicOrder(ProviderTypeDAOs, filter);
            List<ProviderType> ProviderTypes = await DynamicSelect(ProviderTypeDAOs, filter);
            return ProviderTypes;
        }

        public async Task<ProviderType> Get(long Id)
        {
            ProviderType ProviderType = await DataContext.ProviderType.Where(x => x.Id == Id).Select(x => new ProviderType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (ProviderType == null)
                return null;
            ProviderType.Providers = await DataContext.Provider
                .Where(x => x.ProviderTypeId == ProviderType.Id)
                .Select(x => new Provider
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProviderTypeId = x.ProviderTypeId,
                    Value = x.Value,
                    IsDefault = x.IsDefault,
                }).ToListAsync();

            return ProviderType;
        }
    }
}
