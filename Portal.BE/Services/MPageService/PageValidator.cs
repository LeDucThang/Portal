using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MPage
{
    public interface IPageValidator : IServiceScoped
    {
        Task<bool> Create(Page Page);
        Task<bool> Update(Page Page);
        Task<bool> Delete(Page Page);
        Task<bool> BulkDelete(List<Page> Pages);
        Task<bool> Import(List<Page> Pages);
    }

    public class PageValidator : IPageValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PageValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Page Page)
        {
            PageFilter PageFilter = new PageFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Page.Id },
                Selects = PageSelect.Id
            };

            int count = await UOW.PageRepository.Count(PageFilter);
            if (count == 0)
                Page.AddError(nameof(PageValidator), nameof(Page.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Page Page)
        {
            return Page.IsValidated;
        }

        public async Task<bool> Update(Page Page)
        {
            if (await ValidateId(Page))
            {
            }
            return Page.IsValidated;
        }

        public async Task<bool> Delete(Page Page)
        {
            if (await ValidateId(Page))
            {
            }
            return Page.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Page> Pages)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Page> Pages)
        {
            return true;
        }
    }
}
