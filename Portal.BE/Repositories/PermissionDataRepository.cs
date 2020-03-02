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
    public interface IPermissionDataRepository
    {
        Task<int> Count(PermissionDataFilter PermissionDataFilter);
        Task<List<PermissionData>> List(PermissionDataFilter PermissionDataFilter);
        Task<PermissionData> Get(long Id);
        Task<bool> Create(PermissionData PermissionData);
        Task<bool> Update(PermissionData PermissionData);
        Task<bool> Delete(PermissionData PermissionData);
        Task<bool> BulkMerge(List<PermissionData> PermissionDatas);
        Task<bool> BulkDelete(List<PermissionData> PermissionDatas);
    }
    public class PermissionDataRepository : IPermissionDataRepository
    {
        private DataContext DataContext;
        public PermissionDataRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PermissionDataDAO> DynamicFilter(IQueryable<PermissionDataDAO> query, PermissionDataFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.PermissionId != null)
                query = query.Where(q => q.PermissionId, filter.PermissionId);
            if (filter.FilterName != null)
                query = query.Where(q => q.FilterName, filter.FilterName);
            if (filter.FilterType != null)
                query = query.Where(q => q.FilterType, filter.FilterType);
            if (filter.FilterValue != null)
                query = query.Where(q => q.FilterValue, filter.FilterValue);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<PermissionDataDAO> OrFilter(IQueryable<PermissionDataDAO> query, PermissionDataFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<PermissionDataDAO> initQuery = query.Where(q => false);
            foreach (PermissionDataFilter PermissionDataFilter in filter.OrFilter)
            {
                IQueryable<PermissionDataDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.PermissionId != null)
                    queryable = queryable.Where(q => q.PermissionId, filter.PermissionId);
                if (filter.FilterName != null)
                    queryable = queryable.Where(q => q.FilterName, filter.FilterName);
                if (filter.FilterType != null)
                    queryable = queryable.Where(q => q.FilterType, filter.FilterType);
                if (filter.FilterValue != null)
                    queryable = queryable.Where(q => q.FilterValue, filter.FilterValue);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PermissionDataDAO> DynamicOrder(IQueryable<PermissionDataDAO> query, PermissionDataFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PermissionDataOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PermissionDataOrder.Permission:
                            query = query.OrderBy(q => q.PermissionId);
                            break;
                        case PermissionDataOrder.FilterName:
                            query = query.OrderBy(q => q.FilterName);
                            break;
                        case PermissionDataOrder.FilterType:
                            query = query.OrderBy(q => q.FilterType);
                            break;
                        case PermissionDataOrder.FilterValue:
                            query = query.OrderBy(q => q.FilterValue);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PermissionDataOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PermissionDataOrder.Permission:
                            query = query.OrderByDescending(q => q.PermissionId);
                            break;
                        case PermissionDataOrder.FilterName:
                            query = query.OrderByDescending(q => q.FilterName);
                            break;
                        case PermissionDataOrder.FilterType:
                            query = query.OrderByDescending(q => q.FilterType);
                            break;
                        case PermissionDataOrder.FilterValue:
                            query = query.OrderByDescending(q => q.FilterValue);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PermissionData>> DynamicSelect(IQueryable<PermissionDataDAO> query, PermissionDataFilter filter)
        {
            List<PermissionData> PermissionDatas = await query.Select(q => new PermissionData()
            {
                Id = filter.Selects.Contains(PermissionDataSelect.Id) ? q.Id : default(long),
                PermissionId = filter.Selects.Contains(PermissionDataSelect.Permission) ? q.PermissionId : default(long),
                FilterName = filter.Selects.Contains(PermissionDataSelect.FilterName) ? q.FilterName : default(string),
                FilterType = filter.Selects.Contains(PermissionDataSelect.FilterType) ? q.FilterType : default(string),
                FilterValue = filter.Selects.Contains(PermissionDataSelect.FilterValue) ? q.FilterValue : default(string),
                Permission = filter.Selects.Contains(PermissionDataSelect.Permission) && q.Permission != null ? new Permission
                {
                    Id = q.Permission.Id,
                    Name = q.Permission.Name,
                    RoleId = q.Permission.RoleId,
                } : null,
            }).ToListAsync();
            return PermissionDatas;
        }

        public async Task<int> Count(PermissionDataFilter filter)
        {
            IQueryable<PermissionDataDAO> PermissionDatas = DataContext.PermissionData;
            PermissionDatas = DynamicFilter(PermissionDatas, filter);
            return await PermissionDatas.CountAsync();
        }

        public async Task<List<PermissionData>> List(PermissionDataFilter filter)
        {
            if (filter == null) return new List<PermissionData>();
            IQueryable<PermissionDataDAO> PermissionDataDAOs = DataContext.PermissionData;
            PermissionDataDAOs = DynamicFilter(PermissionDataDAOs, filter);
            PermissionDataDAOs = DynamicOrder(PermissionDataDAOs, filter);
            List<PermissionData> PermissionDatas = await DynamicSelect(PermissionDataDAOs, filter);
            return PermissionDatas;
        }

        public async Task<PermissionData> Get(long Id)
        {
            PermissionData PermissionData = await DataContext.PermissionData.Where(x => x.Id == Id).Select(x => new PermissionData()
            {
                Id = x.Id,
                PermissionId = x.PermissionId,
                FilterName = x.FilterName,
                FilterType = x.FilterType,
                FilterValue = x.FilterValue,
                Permission = x.Permission == null ? null : new Permission
                {
                    Id = x.Permission.Id,
                    Name = x.Permission.Name,
                    RoleId = x.Permission.RoleId,
                },
            }).FirstOrDefaultAsync();

            if (PermissionData == null)
                return null;

            return PermissionData;
        }
        public async Task<bool> Create(PermissionData PermissionData)
        {
            PermissionDataDAO PermissionDataDAO = new PermissionDataDAO();
            PermissionDataDAO.Id = PermissionData.Id;
            PermissionDataDAO.PermissionId = PermissionData.PermissionId;
            PermissionDataDAO.FilterName = PermissionData.FilterName;
            PermissionDataDAO.FilterType = PermissionData.FilterType;
            PermissionDataDAO.FilterValue = PermissionData.FilterValue;
            DataContext.PermissionData.Add(PermissionDataDAO);
            await DataContext.SaveChangesAsync();
            PermissionData.Id = PermissionDataDAO.Id;
            await SaveReference(PermissionData);
            return true;
        }

        public async Task<bool> Update(PermissionData PermissionData)
        {
            PermissionDataDAO PermissionDataDAO = DataContext.PermissionData.Where(x => x.Id == PermissionData.Id).FirstOrDefault();
            if (PermissionDataDAO == null)
                return false;
            PermissionDataDAO.Id = PermissionData.Id;
            PermissionDataDAO.PermissionId = PermissionData.PermissionId;
            PermissionDataDAO.FilterName = PermissionData.FilterName;
            PermissionDataDAO.FilterType = PermissionData.FilterType;
            PermissionDataDAO.FilterValue = PermissionData.FilterValue;
            await DataContext.SaveChangesAsync();
            await SaveReference(PermissionData);
            return true;
        }

        public async Task<bool> Delete(PermissionData PermissionData)
        {
            await DataContext.PermissionData.Where(x => x.Id == PermissionData.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PermissionData> PermissionDatas)
        {
            List<PermissionDataDAO> PermissionDataDAOs = new List<PermissionDataDAO>();
            foreach (PermissionData PermissionData in PermissionDatas)
            {
                PermissionDataDAO PermissionDataDAO = new PermissionDataDAO();
                PermissionDataDAO.Id = PermissionData.Id;
                PermissionDataDAO.PermissionId = PermissionData.PermissionId;
                PermissionDataDAO.FilterName = PermissionData.FilterName;
                PermissionDataDAO.FilterType = PermissionData.FilterType;
                PermissionDataDAO.FilterValue = PermissionData.FilterValue;
                PermissionDataDAOs.Add(PermissionDataDAO);
            }
            await DataContext.BulkMergeAsync(PermissionDataDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PermissionData> PermissionDatas)
        {
            List<long> Ids = PermissionDatas.Select(x => x.Id).ToList();
            await DataContext.PermissionData.Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(PermissionData PermissionData)
        {
        }
    }
}
