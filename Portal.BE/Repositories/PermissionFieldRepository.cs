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
    public interface IPermissionFieldRepository
    {
        Task<int> Count(PermissionFieldFilter PermissionFieldFilter);
        Task<List<PermissionField>> List(PermissionFieldFilter PermissionFieldFilter);
        Task<PermissionField> Get(long Id);
        Task<bool> Create(PermissionField PermissionField);
        Task<bool> Update(PermissionField PermissionField);
        Task<bool> Delete(PermissionField PermissionField);
        Task<bool> BulkMerge(List<PermissionField> PermissionFields);
        Task<bool> BulkDelete(List<PermissionField> PermissionFields);
    }
    public class PermissionFieldRepository : IPermissionFieldRepository
    {
        private DataContext DataContext;
        public PermissionFieldRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PermissionFieldDAO> DynamicFilter(IQueryable<PermissionFieldDAO> query, PermissionFieldFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Type != null)
                query = query.Where(q => q.Type, filter.Type);
            if (filter.ViewId != null)
                query = query.Where(q => q.ViewId, filter.ViewId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<PermissionFieldDAO> OrFilter(IQueryable<PermissionFieldDAO> query, PermissionFieldFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<PermissionFieldDAO> initQuery = query.Where(q => false);
            foreach (PermissionFieldFilter PermissionFieldFilter in filter.OrFilter)
            {
                IQueryable<PermissionFieldDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Type != null)
                    queryable = queryable.Where(q => q.Type, filter.Type);
                if (filter.ViewId != null)
                    queryable = queryable.Where(q => q.ViewId, filter.ViewId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PermissionFieldDAO> DynamicOrder(IQueryable<PermissionFieldDAO> query, PermissionFieldFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PermissionFieldOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PermissionFieldOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PermissionFieldOrder.Type:
                            query = query.OrderBy(q => q.Type);
                            break;
                        case PermissionFieldOrder.View:
                            query = query.OrderBy(q => q.ViewId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PermissionFieldOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PermissionFieldOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PermissionFieldOrder.Type:
                            query = query.OrderByDescending(q => q.Type);
                            break;
                        case PermissionFieldOrder.View:
                            query = query.OrderByDescending(q => q.ViewId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PermissionField>> DynamicSelect(IQueryable<PermissionFieldDAO> query, PermissionFieldFilter filter)
        {
            List<PermissionField> PermissionFields = await query.Select(q => new PermissionField()
            {
                Id = filter.Selects.Contains(PermissionFieldSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(PermissionFieldSelect.Name) ? q.Name : default(string),
                Type = filter.Selects.Contains(PermissionFieldSelect.Type) ? q.Type : default(string),
                ViewId = filter.Selects.Contains(PermissionFieldSelect.View) ? q.ViewId : default(long),
                View = filter.Selects.Contains(PermissionFieldSelect.View) && q.View != null ? new View
                {
                    Id = q.View.Id,
                    Name = q.View.Name,
                    Path = q.View.Path,
                    IsDeleted = q.View.IsDeleted,
                } : null,
            }).ToListAsync();
            return PermissionFields;
        }

        public async Task<int> Count(PermissionFieldFilter filter)
        {
            IQueryable<PermissionFieldDAO> PermissionFields = DataContext.PermissionField;
            PermissionFields = DynamicFilter(PermissionFields, filter);
            return await PermissionFields.CountAsync();
        }

        public async Task<List<PermissionField>> List(PermissionFieldFilter filter)
        {
            if (filter == null) return new List<PermissionField>();
            IQueryable<PermissionFieldDAO> PermissionFieldDAOs = DataContext.PermissionField;
            PermissionFieldDAOs = DynamicFilter(PermissionFieldDAOs, filter);
            PermissionFieldDAOs = DynamicOrder(PermissionFieldDAOs, filter);
            List<PermissionField> PermissionFields = await DynamicSelect(PermissionFieldDAOs, filter);
            return PermissionFields;
        }

        public async Task<PermissionField> Get(long Id)
        {
            PermissionField PermissionField = await DataContext.PermissionField.Where(x => x.Id == Id).Select(x => new PermissionField()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                ViewId = x.ViewId,
                View = x.View == null ? null : new View
                {
                    Id = x.View.Id,
                    Name = x.View.Name,
                    Path = x.View.Path,
                    IsDeleted = x.View.IsDeleted,
                },
            }).FirstOrDefaultAsync();

            if (PermissionField == null)
                return null;
            PermissionField.PermissionDatas = await DataContext.PermissionData
                .Where(x => x.PermissionFieldId == PermissionField.Id)
                .Select(x => new PermissionData
                {
                    Id = x.Id,
                    PermissionId = x.PermissionId,
                    PermissionFieldId = x.PermissionFieldId,
                    Value = x.Value,
                }).ToListAsync();

            return PermissionField;
        }
        public async Task<bool> Create(PermissionField PermissionField)
        {
            PermissionFieldDAO PermissionFieldDAO = new PermissionFieldDAO();
            PermissionFieldDAO.Id = PermissionField.Id;
            PermissionFieldDAO.Name = PermissionField.Name;
            PermissionFieldDAO.Type = PermissionField.Type;
            PermissionFieldDAO.ViewId = PermissionField.ViewId;
            DataContext.PermissionField.Add(PermissionFieldDAO);
            await DataContext.SaveChangesAsync();
            PermissionField.Id = PermissionFieldDAO.Id;
            await SaveReference(PermissionField);
            return true;
        }

        public async Task<bool> Update(PermissionField PermissionField)
        {
            PermissionFieldDAO PermissionFieldDAO = DataContext.PermissionField.Where(x => x.Id == PermissionField.Id).FirstOrDefault();
            if (PermissionFieldDAO == null)
                return false;
            PermissionFieldDAO.Id = PermissionField.Id;
            PermissionFieldDAO.Name = PermissionField.Name;
            PermissionFieldDAO.Type = PermissionField.Type;
            PermissionFieldDAO.ViewId = PermissionField.ViewId;
            await DataContext.SaveChangesAsync();
            await SaveReference(PermissionField);
            return true;
        }

        public async Task<bool> Delete(PermissionField PermissionField)
        {
            await DataContext.PermissionData.Where(x => x.PermissionFieldId == PermissionField.Id).DeleteFromQueryAsync();
            await DataContext.PermissionField.Where(x => x.Id == PermissionField.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PermissionField> PermissionFields)
        {
            List<PermissionFieldDAO> PermissionFieldDAOs = new List<PermissionFieldDAO>();
            foreach (PermissionField PermissionField in PermissionFields)
            {
                PermissionFieldDAO PermissionFieldDAO = new PermissionFieldDAO();
                PermissionFieldDAO.Id = PermissionField.Id;
                PermissionFieldDAO.Name = PermissionField.Name;
                PermissionFieldDAO.Type = PermissionField.Type;
                PermissionFieldDAO.ViewId = PermissionField.ViewId;
                PermissionFieldDAOs.Add(PermissionFieldDAO);
            }
            await DataContext.BulkMergeAsync(PermissionFieldDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PermissionField> PermissionFields)
        {
            List<long> Ids = PermissionFields.Select(x => x.Id).ToList();
            await DataContext.PermissionData.Where(x => Ids.Contains(x.PermissionFieldId)).DeleteFromQueryAsync();
            await DataContext.PermissionField.Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(PermissionField PermissionField)
        {
            await DataContext.PermissionData
                .Where(x => x.PermissionFieldId == PermissionField.Id)
                .DeleteFromQueryAsync();
            List<PermissionDataDAO> PermissionDataDAOs = new List<PermissionDataDAO>();
            if (PermissionField.PermissionDatas != null)
            {
                foreach (PermissionData PermissionData in PermissionField.PermissionDatas)
                {
                    PermissionDataDAO PermissionDataDAO = new PermissionDataDAO();
                    PermissionDataDAO.Id = PermissionData.Id;
                    PermissionDataDAO.PermissionId = PermissionData.PermissionId;
                    PermissionDataDAO.PermissionFieldId = PermissionData.PermissionFieldId;
                    PermissionDataDAO.Value = PermissionData.Value;
                    PermissionDataDAOs.Add(PermissionDataDAO);
                }
                await DataContext.PermissionData.BulkMergeAsync(PermissionDataDAOs);
            }
            
        }
    }
}
