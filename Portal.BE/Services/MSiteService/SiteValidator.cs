using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MSite
{
    public interface ISiteValidator : IServiceScoped
    {
        Task<bool> Create(Site Site);
        Task<bool> Update(Site Site);
        Task<bool> Delete(Site Site);
        Task<bool> BulkDelete(List<Site> Sites);
        Task<bool> Import(List<Site> Sites);
    }

    public class SiteValidator : ISiteValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public SiteValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Site Site)
        {
            SiteFilter SiteFilter = new SiteFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Site.Id },
                Selects = SiteSelect.Id
            };

            int count = await UOW.SiteRepository.Count(SiteFilter);
            if (count == 0)
                Site.AddError(nameof(SiteValidator), nameof(Site.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Site Site)
        {
            return Site.IsValidated;
        }

        public async Task<bool> Update(Site Site)
        {
            if (await ValidateId(Site))
            {
            }
            return Site.IsValidated;
        }

        public async Task<bool> Delete(Site Site)
        {
            if (await ValidateId(Site))
            {
            }
            return Site.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Site> Sites)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Site> Sites)
        {
            return true;
        }
    }
}
