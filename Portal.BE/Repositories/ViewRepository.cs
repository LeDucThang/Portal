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
    public interface IViewRepository
    {
        Task<int> Count(ViewFilter ViewFilter);
        Task<List<View>> List(ViewFilter ViewFilter);
        Task<View> Get(long Id);
        Task<bool> Create(View View);
        Task<bool> Update(View View);
        Task<bool> Delete(View View);
        Task<bool> BulkMerge(List<View> Views);
        Task<bool> BulkDelete(List<View> Views);
    }
    public class ViewRepository : IViewRepository
    {
        private DataContext DataContext;
        public ViewRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ViewDAO> DynamicFilter(IQueryable<ViewDAO> query, ViewFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ViewDAO> OrFilter(IQueryable<ViewDAO> query, ViewFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<ViewDAO> initQuery = query.Where(q => false);
            foreach (ViewFilter ViewFilter in filter.OrFilter)
            {
                IQueryable<ViewDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Path != null)
                    queryable = queryable.Where(q => q.Path, filter.Path);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ViewDAO> DynamicOrder(IQueryable<ViewDAO> query, ViewFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ViewOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ViewOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ViewOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case ViewOrder.IsDeleted:
                            query = query.OrderBy(q => q.IsDeleted);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ViewOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ViewOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ViewOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case ViewOrder.IsDeleted:
                            query = query.OrderByDescending(q => q.IsDeleted);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<View>> DynamicSelect(IQueryable<ViewDAO> query, ViewFilter filter)
        {
            List<View> Views = await query.Select(q => new View()
            {
                Id = filter.Selects.Contains(ViewSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ViewSelect.Name) ? q.Name : default(string),
                Path = filter.Selects.Contains(ViewSelect.Path) ? q.Path : default(string),
                IsDeleted = filter.Selects.Contains(ViewSelect.IsDeleted) ? q.IsDeleted : default(bool),
            }).ToListAsync();
            return Views;
        }

        public async Task<int> Count(ViewFilter filter)
        {
            IQueryable<ViewDAO> Views = DataContext.View;
            Views = DynamicFilter(Views, filter);
            return await Views.CountAsync();
        }

        public async Task<List<View>> List(ViewFilter filter)
        {
            if (filter == null) return new List<View>();
            IQueryable<ViewDAO> ViewDAOs = DataContext.View;
            ViewDAOs = DynamicFilter(ViewDAOs, filter);
            ViewDAOs = DynamicOrder(ViewDAOs, filter);
            List<View> Views = await DynamicSelect(ViewDAOs, filter);
            return Views;
        }

        public async Task<View> Get(long Id)
        {
            View View = await DataContext.View.Where(x => x.Id == Id).Select(x => new View()
            {
                Id = x.Id,
                Name = x.Name,
                Path = x.Path,
                IsDeleted = x.IsDeleted,
            }).FirstOrDefaultAsync();

            if (View == null)
                return null;
            View.Pages = await DataContext.Page
                .Where(x => x.ViewId == View.Id)
                .Select(x => new Page
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                    ViewId = x.ViewId,
                    IsDeleted = x.IsDeleted,
                }).ToListAsync();
            View.PermissionFields = await DataContext.PermissionField
                .Where(x => x.ViewId == View.Id)
                .Select(x => new PermissionField
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    ViewId = x.ViewId,
                }).ToListAsync();

            return View;
        }
        public async Task<bool> Create(View View)
        {
            ViewDAO ViewDAO = new ViewDAO();
            ViewDAO.Id = View.Id;
            ViewDAO.Name = View.Name;
            ViewDAO.Path = View.Path;
            ViewDAO.IsDeleted = View.IsDeleted;
            DataContext.View.Add(ViewDAO);
            await DataContext.SaveChangesAsync();
            View.Id = ViewDAO.Id;
            await SaveReference(View);
            return true;
        }

        public async Task<bool> Update(View View)
        {
            ViewDAO ViewDAO = DataContext.View.Where(x => x.Id == View.Id).FirstOrDefault();
            if (ViewDAO == null)
                return false;
            ViewDAO.Id = View.Id;
            ViewDAO.Name = View.Name;
            ViewDAO.Path = View.Path;
            ViewDAO.IsDeleted = View.IsDeleted;
            await DataContext.SaveChangesAsync();
            await SaveReference(View);
            return true;
        }

        public async Task<bool> Delete(View View)
        {
            await DataContext.Page.Where(x => x.ViewId == View.Id).DeleteFromQueryAsync();
            await DataContext.PermissionField.Where(x => x.ViewId == View.Id).DeleteFromQueryAsync();
            await DataContext.View.Where(x => x.Id == View.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<View> Views)
        {
            List<ViewDAO> ViewDAOs = new List<ViewDAO>();
            foreach (View View in Views)
            {
                ViewDAO ViewDAO = new ViewDAO();
                ViewDAO.Id = View.Id;
                ViewDAO.Name = View.Name;
                ViewDAO.Path = View.Path;
                ViewDAO.IsDeleted = View.IsDeleted;
                ViewDAOs.Add(ViewDAO);
            }
            await DataContext.BulkMergeAsync(ViewDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<View> Views)
        {
            List<long> Ids = Views.Select(x => x.Id).ToList();
            await DataContext.Page.Where(x => Ids.Contains(x.ViewId)).DeleteFromQueryAsync();
            await DataContext.PermissionField.Where(x => Ids.Contains(x.ViewId)).DeleteFromQueryAsync();
            await DataContext.View.Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(View View)
        {
            await DataContext.Page
                .Where(x => x.ViewId == View.Id)
                .DeleteFromQueryAsync();
            List<PageDAO> PageDAOs = new List<PageDAO>();
            if (View.Pages != null)
            {
                foreach (Page Page in View.Pages)
                {
                    PageDAO PageDAO = new PageDAO();
                    PageDAO.Id = Page.Id;
                    PageDAO.Name = Page.Name;
                    PageDAO.Path = Page.Path;
                    PageDAO.ViewId = Page.ViewId;
                    PageDAO.IsDeleted = Page.IsDeleted;
                    PageDAOs.Add(PageDAO);
                }
                await DataContext.Page.BulkMergeAsync(PageDAOs);
            }
            
            await DataContext.PermissionField
                .Where(x => x.ViewId == View.Id)
                .DeleteFromQueryAsync();
            List<PermissionFieldDAO> PermissionFieldDAOs = new List<PermissionFieldDAO>();
            if (View.PermissionFields != null)
            {
                foreach (PermissionField PermissionField in View.PermissionFields)
                {
                    PermissionFieldDAO PermissionFieldDAO = new PermissionFieldDAO();
                    PermissionFieldDAO.Id = PermissionField.Id;
                    PermissionFieldDAO.Name = PermissionField.Name;
                    PermissionFieldDAO.Type = PermissionField.Type;
                    PermissionFieldDAO.ViewId = PermissionField.ViewId;
                    PermissionFieldDAOs.Add(PermissionFieldDAO);
                }
                await DataContext.PermissionField.BulkMergeAsync(PermissionFieldDAOs);
            }
            
        }
    }
}
