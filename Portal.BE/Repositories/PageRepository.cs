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
    public interface IPageRepository
    {
        Task<int> Count(PageFilter PageFilter);
        Task<List<Page>> List(PageFilter PageFilter);
        Task<Page> Get(long Id);
        Task<bool> Create(Page Page);
        Task<bool> Update(Page Page);
        Task<bool> Delete(Page Page);
        Task<bool> BulkMerge(List<Page> Pages);
        Task<bool> BulkDelete(List<Page> Pages);
    }
    public class PageRepository : IPageRepository
    {
        private DataContext DataContext;
        public PageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PageDAO> DynamicFilter(IQueryable<PageDAO> query, PageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path);
            if (filter.ParentId != null)
                query = query.Where(q => q.ParentId, filter.ParentId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<PageDAO> OrFilter(IQueryable<PageDAO> query, PageFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<PageDAO> initQuery = query.Where(q => false);
            foreach (PageFilter PageFilter in filter.OrFilter)
            {
                IQueryable<PageDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Path != null)
                    queryable = queryable.Where(q => q.Path, filter.Path);
                if (filter.ParentId != null)
                    queryable = queryable.Where(q => q.ParentId, filter.ParentId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PageDAO> DynamicOrder(IQueryable<PageDAO> query, PageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PageOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PageOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case PageOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PageOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PageOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case PageOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Page>> DynamicSelect(IQueryable<PageDAO> query, PageFilter filter)
        {
            List<Page> Pages = await query.Select(q => new Page()
            {
                Id = filter.Selects.Contains(PageSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(PageSelect.Name) ? q.Name : default(string),
                Path = filter.Selects.Contains(PageSelect.Path) ? q.Path : default(string),
                ParentId = filter.Selects.Contains(PageSelect.Parent) ? q.ParentId : default(long?),
            }).ToListAsync();
            return Pages;
        }

        public async Task<int> Count(PageFilter filter)
        {
            IQueryable<PageDAO> Pages = DataContext.Page;
            Pages = DynamicFilter(Pages, filter);
            return await Pages.CountAsync();
        }

        public async Task<List<Page>> List(PageFilter filter)
        {
            if (filter == null) return new List<Page>();
            IQueryable<PageDAO> PageDAOs = DataContext.Page;
            PageDAOs = DynamicFilter(PageDAOs, filter);
            PageDAOs = DynamicOrder(PageDAOs, filter);
            List<Page> Pages = await DynamicSelect(PageDAOs, filter);
            return Pages;
        }

        public async Task<Page> Get(long Id)
        {
            Page Page = await DataContext.Page.Where(x => x.Id == Id).Select(x => new Page()
            {
                Id = x.Id,
                Name = x.Name,
                Path = x.Path,
                ParentId = x.ParentId,
            }).FirstOrDefaultAsync();

            if (Page == null)
                return null;
            Page.Permissions = await DataContext.PermissionPageMapping
                .Where(x => x.PageId == Page.Id)
                .Select(x => new Permission
                {
                    Id = x.Permission.Id,
                    Name = x.Permission.Name,
                    RoleId = x.Permission.RoleId,
                }).ToListAsync();

            return Page;
        }
        public async Task<bool> Create(Page Page)
        {
            PageDAO PageDAO = new PageDAO();
            PageDAO.Id = Page.Id;
            PageDAO.Name = Page.Name;
            PageDAO.Path = Page.Path;
            PageDAO.ParentId = Page.ParentId;
            DataContext.Page.Add(PageDAO);
            await DataContext.SaveChangesAsync();
            Page.Id = PageDAO.Id;
            await SaveReference(Page);
            return true;
        }

        public async Task<bool> Update(Page Page)
        {
            PageDAO PageDAO = DataContext.Page.Where(x => x.Id == Page.Id).FirstOrDefault();
            if (PageDAO == null)
                return false;
            PageDAO.Id = Page.Id;
            PageDAO.Name = Page.Name;
            PageDAO.Path = Page.Path;
            PageDAO.ParentId = Page.ParentId;
            await DataContext.SaveChangesAsync();
            await SaveReference(Page);
            return true;
        }

        public async Task<bool> Delete(Page Page)
        {
            await DataContext.PermissionPageMapping.Where(x => x.PageId == Page.Id).DeleteFromQueryAsync();
            await DataContext.Page.Where(x => x.Id == Page.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Page> Pages)
        {
            List<PageDAO> PageDAOs = new List<PageDAO>();
            foreach (Page Page in Pages)
            {
                PageDAO PageDAO = new PageDAO();
                PageDAO.Id = Page.Id;
                PageDAO.Name = Page.Name;
                PageDAO.Path = Page.Path;
                PageDAO.ParentId = Page.ParentId;
                PageDAOs.Add(PageDAO);
            }
            await DataContext.BulkMergeAsync(PageDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Page> Pages)
        {
            List<long> Ids = Pages.Select(x => x.Id).ToList();
            await DataContext.PermissionPageMapping.Where(x => Ids.Contains(x.PageId)).DeleteFromQueryAsync();
            await DataContext.Page.Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Page Page)
        {
            await DataContext.PermissionPageMapping.Where(x => x.PageId == Page.Id).DeleteFromQueryAsync();
            if (Page.Permissions != null)
            {
                List<PermissionPageMappingDAO> PermissionPageMappingDAOs = Page.Permissions
                    .Select(x => new PermissionPageMappingDAO
                    {
                        PermissionId = x.Id,
                        PageId = Page.Id,
                    }).ToList();
                await DataContext.PermissionPageMapping.BulkInsertAsync(PermissionPageMappingDAOs);
            }
        }
    }
}
