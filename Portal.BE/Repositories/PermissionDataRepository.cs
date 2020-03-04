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
            if (filter.PermissionFieldId != null)
                query = query.Where(q => q.PermissionFieldId, filter.PermissionFieldId);
            if (filter.Value != null)
                query = query.Where(q => q.Value, filter.Value);
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
                if (filter.PermissionFieldId != null)
                    queryable = queryable.Where(q => q.PermissionFieldId, filter.PermissionFieldId);
                if (filter.Value != null)
                    queryable = queryable.Where(q => q.Value, filter.Value);
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
                        case PermissionDataOrder.PermissionField:
                            query = query.OrderBy(q => q.PermissionFieldId);
                            break;
                        case PermissionDataOrder.Value:
                            query = query.OrderBy(q => q.Value);
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
                        case PermissionDataOrder.PermissionField:
                            query = query.OrderByDescending(q => q.PermissionFieldId);
                            break;
                        case PermissionDataOrder.Value:
                            query = query.OrderByDescending(q => q.Value);
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
                PermissionFieldId = filter.Selects.Contains(PermissionDataSelect.PermissionField) ? q.PermissionFieldId : default(long),
                Value = filter.Selects.Contains(PermissionDataSelect.Value) ? q.Value : default(string),
                Permission = filter.Selects.Contains(PermissionDataSelect.Permission) && q.Permission != null ? new Permission
                {
                    Id = q.Permission.Id,
                    Name = q.Permission.Name,
                    RoleId = q.Permission.RoleId,
                } : null,
                PermissionField = filter.Selects.Contains(PermissionDataSelect.PermissionField) && q.PermissionField != null ? new PermissionField
                {
                    Id = q.PermissionField.Id,
                    Name = q.PermissionField.Name,
                    Type = q.PermissionField.Type,
                    ViewId = q.PermissionField.ViewId,
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
                PermissionFieldId = x.PermissionFieldId,
                Value = x.Value,
                Permission = x.Permission == null ? null : new Permission
                {
                    Id = x.Permission.Id,
                    Name = x.Permission.Name,
                    RoleId = x.Permission.RoleId,
                },
                PermissionField = x.PermissionField == null ? null : new PermissionField
                {
                    Id = x.PermissionField.Id,
                    Name = x.PermissionField.Name,
                    Type = x.PermissionField.Type,
                    ViewId = x.PermissionField.ViewId,
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
            PermissionDataDAO.PermissionFieldId = PermissionData.PermissionFieldId;
            PermissionDataDAO.Value = PermissionData.Value;
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
            PermissionDataDAO.PermissionFieldId = PermissionData.PermissionFieldId;
            PermissionDataDAO.Value = PermissionData.Value;
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
                PermissionDataDAO.PermissionFieldId = PermissionData.PermissionFieldId;
                PermissionDataDAO.Value = PermissionData.Value;
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
