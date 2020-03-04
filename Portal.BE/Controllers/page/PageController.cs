using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MPage;
using Portal.Services.MView;
using Portal.Services.MPermission;


namespace Portal.Controllers.page
{
    public class PageRoute : Root
    {
        public const string Master = Module + "/page/page-master";
        public const string Detail = Module + "/page/page-detail";
        private const string Default = Rpc + Module + "/page";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public const string SingleListView = Default + "/single-list-view";
        public const string CountPermission = Default + "/count-permission";
        public const string ListPermission = Default + "/list-permission";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(Page.Id), FieldType.ID },
            { nameof(Page.Name), FieldType.STRING },
            { nameof(Page.Path), FieldType.STRING },
            { nameof(Page.ViewId), FieldType.ID },
        };
    }

    public class PageController : ApiController
    {
        private IViewService ViewService;
        
        private IPermissionService PermissionService;
        
        private IPageService PageService;

        public PageController(
            IViewService ViewService,
            
            IPermissionService PermissionService,
            
            IPageService PageService
        )
        {
            this.ViewService = ViewService;
            
            this.PermissionService = PermissionService;
            
            this.PageService = PageService;
        }

        [Route(PageRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Page_PageFilterDTO Page_PageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = ConvertFilterDTOToFilterEntity(Page_PageFilterDTO);
            return await PageService.Count(PageFilter);
        }

        [Route(PageRoute.List), HttpPost]
        public async Task<List<Page_PageDTO>> List([FromBody] Page_PageFilterDTO Page_PageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = ConvertFilterDTOToFilterEntity(Page_PageFilterDTO);
            List<Page> Pages = await PageService.List(PageFilter);
            return Pages.Select(c => new Page_PageDTO(c)).ToList();
        }

        [Route(PageRoute.Get), HttpPost]
        public async Task<Page_PageDTO> Get([FromBody]Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Page Page = await PageService.Get(Page_PageDTO.Id);
            return new Page_PageDTO(Page);
        }

        [Route(PageRoute.Create), HttpPost]
        public async Task<ActionResult<Page_PageDTO>> Create([FromBody] Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Page Page = ConvertDTOToEntity(Page_PageDTO);
            Page = await PageService.Create(Page);
            Page_PageDTO = new Page_PageDTO(Page);
            if (Page.IsValidated)
                return Page_PageDTO;
            else
                return BadRequest(Page_PageDTO);
        }

        [Route(PageRoute.Update), HttpPost]
        public async Task<ActionResult<Page_PageDTO>> Update([FromBody] Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Page Page = ConvertDTOToEntity(Page_PageDTO);
            Page = await PageService.Update(Page);
            Page_PageDTO = new Page_PageDTO(Page);
            if (Page.IsValidated)
                return Page_PageDTO;
            else
                return BadRequest(Page_PageDTO);
        }

        [Route(PageRoute.Delete), HttpPost]
        public async Task<ActionResult<Page_PageDTO>> Delete([FromBody] Page_PageDTO Page_PageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Page Page = ConvertDTOToEntity(Page_PageDTO);
            Page = await PageService.Delete(Page);
            Page_PageDTO = new Page_PageDTO(Page);
            if (Page.IsValidated)
                return Page_PageDTO;
            else
                return BadRequest(Page_PageDTO);
        }

        [Route(PageRoute.Import), HttpPost]
        public async Task<List<Page_PageDTO>> Import([FromBody] Page_PageFilterDTO Page_PageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = ConvertFilterDTOToFilterEntity(Page_PageFilterDTO);
            List<Page> Pages = await PageService.List(PageFilter);
            return Pages.Select(c => new Page_PageDTO(c)).ToList();
        }

        [Route(PageRoute.Export), HttpPost]
        public async Task<List<Page_PageDTO>> Export([FromBody] Page_PageFilterDTO Page_PageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PageFilter PageFilter = ConvertFilterDTOToFilterEntity(Page_PageFilterDTO);
            List<Page> Pages = await PageService.List(PageFilter);
            return Pages.Select(c => new Page_PageDTO(c)).ToList();
        }

        public Page ConvertDTOToEntity(Page_PageDTO Page_PageDTO)
        {
            Page Page = new Page();
            Page.Id = Page_PageDTO.Id;
            Page.Name = Page_PageDTO.Name;
            Page.Path = Page_PageDTO.Path;
            Page.ViewId = Page_PageDTO.ViewId;
            Page.IsDeleted = Page_PageDTO.IsDeleted;
            Page.View = Page_PageDTO.View == null ? null : new View
            {
                Id = Page_PageDTO.View.Id,
                Name = Page_PageDTO.View.Name,
                Path = Page_PageDTO.View.Path,
                IsDeleted = Page_PageDTO.View.IsDeleted,
            };

            return Page;
        }

        public PageFilter ConvertFilterDTOToFilterEntity(Page_PageFilterDTO Page_PageFilterDTO)
        {
            PageFilter PageFilter = new PageFilter();
            PageFilter.Selects = PageSelect.ALL;
            PageFilter.Skip = Page_PageFilterDTO.Skip;
            PageFilter.Take = Page_PageFilterDTO.Take;
            PageFilter.OrderBy = Page_PageFilterDTO.OrderBy;
            PageFilter.OrderType = Page_PageFilterDTO.OrderType;

            PageFilter.Id = Page_PageFilterDTO.Id;
            PageFilter.Name = Page_PageFilterDTO.Name;
            PageFilter.Path = Page_PageFilterDTO.Path;
            PageFilter.ViewId = Page_PageFilterDTO.ViewId;
            return PageFilter;
        }

        
        [Route(PageRoute.SingleListView), HttpPost]
        public async Task<List<Page_ViewDTO>> SingleListView([FromBody] Page_ViewFilterDTO Page_ViewFilterDTO)
        {
            ViewFilter ViewFilter = new ViewFilter();
            ViewFilter.Skip = 0;
            ViewFilter.Take = 20;
            ViewFilter.OrderBy = ViewOrder.Id;
            ViewFilter.OrderType = OrderType.ASC;
            ViewFilter.Selects = ViewSelect.ALL;
            ViewFilter.Id = Page_ViewFilterDTO.Id;
            ViewFilter.Name = Page_ViewFilterDTO.Name;
            ViewFilter.Path = Page_ViewFilterDTO.Path;

            List<View> Views = await ViewService.List(ViewFilter);
            List<Page_ViewDTO> Page_ViewDTOs = Views
                .Select(x => new Page_ViewDTO(x)).ToList();
            return Page_ViewDTOs;
        }
        

        [Route(PageRoute.CountPermission), HttpPost]
        public async Task<long> CountPermission([FromBody] Page_PermissionFilterDTO Page_PermissionFilterDTO)
        {
            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter.Id = Page_PermissionFilterDTO.Id;
            PermissionFilter.Name = Page_PermissionFilterDTO.Name;
            PermissionFilter.RoleId = Page_PermissionFilterDTO.RoleId;

            return await PermissionService.Count(PermissionFilter);
        }

        [Route(PageRoute.ListPermission), HttpPost]
        public async Task<List<Page_PermissionDTO>> ListPermission([FromBody] Page_PermissionFilterDTO Page_PermissionFilterDTO)
        {
            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter.Skip = 0;
            PermissionFilter.Take = 20;
            PermissionFilter.OrderBy = PermissionOrder.Id;
            PermissionFilter.OrderType = OrderType.ASC;
            PermissionFilter.Selects = PermissionSelect.ALL;
            PermissionFilter.Id = Page_PermissionFilterDTO.Id;
            PermissionFilter.Name = Page_PermissionFilterDTO.Name;
            PermissionFilter.RoleId = Page_PermissionFilterDTO.RoleId;

            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            List<Page_PermissionDTO> Page_PermissionDTOs = Permissions
                .Select(x => new Page_PermissionDTO(x)).ToList();
            return Page_PermissionDTOs;
        }
    }
}

