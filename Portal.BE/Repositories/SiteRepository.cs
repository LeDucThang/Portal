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
    public interface ISiteRepository
    {
        Task<int> Count(SiteFilter SiteFilter);
        Task<List<Site>> List(SiteFilter SiteFilter);
        Task<Site> Get(long Id);
        Task<bool> Create(Site Site);
        Task<bool> Update(Site Site);
        Task<bool> Delete(Site Site);
        Task<bool> BulkMerge(List<Site> Sites);
        Task<bool> BulkDelete(List<Site> Sites);
    }
    public class SiteRepository : ISiteRepository
    {
        private DataContext DataContext;
        public SiteRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SiteDAO> DynamicFilter(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.URL != null)
                query = query.Where(q => q.URL, filter.URL);
            if (filter.Status != null)
                query = query.Where(q => q.Status, filter.Status);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<SiteDAO> OrFilter(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            if (filter.OrFilter == null)
                return query;
            IQueryable<SiteDAO> initQuery = query.Where(q => false);
            foreach (SiteFilter SiteFilter in filter.OrFilter)
            {
                IQueryable<SiteDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.URL != null)
                    queryable = queryable.Where(q => q.URL, filter.URL);
                if (filter.Status != null)
                    queryable = queryable.Where(q => q.Status, filter.Status);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<SiteDAO> DynamicOrder(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SiteOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SiteOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case SiteOrder.URL:
                            query = query.OrderBy(q => q.URL);
                            break;
                        case SiteOrder.Status:
                            query = query.OrderBy(q => q.Status);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SiteOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SiteOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case SiteOrder.URL:
                            query = query.OrderByDescending(q => q.URL);
                            break;
                        case SiteOrder.Status:
                            query = query.OrderByDescending(q => q.Status);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Site>> DynamicSelect(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            List<Site> Sites = await query.Select(q => new Site()
            {
                Id = filter.Selects.Contains(SiteSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(SiteSelect.Name) ? q.Name : default(string),
                URL = filter.Selects.Contains(SiteSelect.URL) ? q.URL : default(string),
                Status = filter.Selects.Contains(SiteSelect.Status) ? q.Status : default(long),
            }).ToListAsync();
            return Sites;
        }

        public async Task<int> Count(SiteFilter filter)
        {
            IQueryable<SiteDAO> Sites = DataContext.Site;
            Sites = DynamicFilter(Sites, filter);
            return await Sites.CountAsync();
        }

        public async Task<List<Site>> List(SiteFilter filter)
        {
            if (filter == null) return new List<Site>();
            IQueryable<SiteDAO> SiteDAOs = DataContext.Site;
            SiteDAOs = DynamicFilter(SiteDAOs, filter);
            SiteDAOs = DynamicOrder(SiteDAOs, filter);
            List<Site> Sites = await DynamicSelect(SiteDAOs, filter);
            return Sites;
        }

        public async Task<Site> Get(long Id)
        {
            Site Site = await DataContext.Site.Where(x => x.Id == Id).Select(x => new Site()
            {
                Id = x.Id,
                Name = x.Name,
                URL = x.URL,
                Status = x.Status,
            }).FirstOrDefaultAsync();

            if (Site == null)
                return null;

            return Site;
        }
        public async Task<bool> Create(Site Site)
        {
            SiteDAO SiteDAO = new SiteDAO();
            SiteDAO.Id = Site.Id;
            SiteDAO.Name = Site.Name;
            SiteDAO.URL = Site.URL;
            SiteDAO.Status = Site.Status;
            SiteDAO.CreatedAt = StaticParams.DateTimeNow;
            SiteDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Site.Add(SiteDAO);
            await DataContext.SaveChangesAsync();
            Site.Id = SiteDAO.Id;
            await SaveReference(Site);
            return true;
        }

        public async Task<bool> Update(Site Site)
        {
            SiteDAO SiteDAO = DataContext.Site.Where(x => x.Id == Site.Id).FirstOrDefault();
            if (SiteDAO == null)
                return false;
            SiteDAO.Id = Site.Id;
            SiteDAO.Name = Site.Name;
            SiteDAO.URL = Site.URL;
            SiteDAO.Status = Site.Status;
            SiteDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Site);
            return true;
        }

        public async Task<bool> Delete(Site Site)
        {
            await DataContext.Site.Where(x => x.Id == Site.Id).UpdateFromQueryAsync(x => new SiteDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Site> Sites)
        {
            List<SiteDAO> SiteDAOs = new List<SiteDAO>();
            foreach (Site Site in Sites)
            {
                SiteDAO SiteDAO = new SiteDAO();
                SiteDAO.Id = Site.Id;
                SiteDAO.Name = Site.Name;
                SiteDAO.URL = Site.URL;
                SiteDAO.Status = Site.Status;
                SiteDAO.CreatedAt = StaticParams.DateTimeNow;
                SiteDAO.UpdatedAt = StaticParams.DateTimeNow;
                SiteDAOs.Add(SiteDAO);
            }
            await DataContext.BulkMergeAsync(SiteDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Site> Sites)
        {
            List<long> Ids = Sites.Select(x => x.Id).ToList();
            await DataContext.Site.Where(x => Ids.Contains(x.Id)).UpdateFromQueryAsync(x => new SiteDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Site Site)
        {
        }
    }
}
