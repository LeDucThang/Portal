using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Portal.Entities;
using Portal;
using Portal.Repositories;

namespace Portal.Services.MView
{
    public interface IViewValidator : IServiceScoped
    {
        Task<bool> Create(View View);
        Task<bool> Update(View View);
        Task<bool> Delete(View View);
        Task<bool> BulkDelete(List<View> Views);
        Task<bool> Import(List<View> Views);
    }

    public class ViewValidator : IViewValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ViewValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(View View)
        {
            ViewFilter ViewFilter = new ViewFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = View.Id },
                Selects = ViewSelect.Id
            };

            int count = await UOW.ViewRepository.Count(ViewFilter);
            if (count == 0)
                View.AddError(nameof(ViewValidator), nameof(View.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(View View)
        {
            return View.IsValidated;
        }

        public async Task<bool> Update(View View)
        {
            if (await ValidateId(View))
            {
            }
            return View.IsValidated;
        }

        public async Task<bool> Delete(View View)
        {
            if (await ValidateId(View))
            {
            }
            return View.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<View> Views)
        {
            return true;
        }
        
        public async Task<bool> Import(List<View> Views)
        {
            return true;
        }
    }
}
