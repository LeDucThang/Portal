using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MView;



namespace Portal.Controllers.view
{
    public class ViewRoute : Root
    {
        public const string Master = Module + "/view/view-master";
        public const string Detail = Module + "/view/view-detail";
        private const string Default = Rpc + Module + "/view";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(View.Id), FieldType.ID },
            { nameof(View.Name), FieldType.STRING },
            { nameof(View.Path), FieldType.STRING },
        };
    }

    public class ViewController : ApiController
    {
        
        private IViewService ViewService;

        public ViewController(
            
            IViewService ViewService
        )
        {
            
            this.ViewService = ViewService;
        }

        [Route(ViewRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] View_ViewFilterDTO View_ViewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ViewFilter ViewFilter = ConvertFilterDTOToFilterEntity(View_ViewFilterDTO);
            return await ViewService.Count(ViewFilter);
        }

        [Route(ViewRoute.List), HttpPost]
        public async Task<List<View_ViewDTO>> List([FromBody] View_ViewFilterDTO View_ViewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ViewFilter ViewFilter = ConvertFilterDTOToFilterEntity(View_ViewFilterDTO);
            List<View> Views = await ViewService.List(ViewFilter);
            return Views.Select(c => new View_ViewDTO(c)).ToList();
        }

        [Route(ViewRoute.Get), HttpPost]
        public async Task<View_ViewDTO> Get([FromBody]View_ViewDTO View_ViewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            View View = await ViewService.Get(View_ViewDTO.Id);
            return new View_ViewDTO(View);
        }

        [Route(ViewRoute.Create), HttpPost]
        public async Task<ActionResult<View_ViewDTO>> Create([FromBody] View_ViewDTO View_ViewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            View View = ConvertDTOToEntity(View_ViewDTO);
            View = await ViewService.Create(View);
            View_ViewDTO = new View_ViewDTO(View);
            if (View.IsValidated)
                return View_ViewDTO;
            else
                return BadRequest(View_ViewDTO);
        }

        [Route(ViewRoute.Update), HttpPost]
        public async Task<ActionResult<View_ViewDTO>> Update([FromBody] View_ViewDTO View_ViewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            View View = ConvertDTOToEntity(View_ViewDTO);
            View = await ViewService.Update(View);
            View_ViewDTO = new View_ViewDTO(View);
            if (View.IsValidated)
                return View_ViewDTO;
            else
                return BadRequest(View_ViewDTO);
        }

        [Route(ViewRoute.Delete), HttpPost]
        public async Task<ActionResult<View_ViewDTO>> Delete([FromBody] View_ViewDTO View_ViewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            View View = ConvertDTOToEntity(View_ViewDTO);
            View = await ViewService.Delete(View);
            View_ViewDTO = new View_ViewDTO(View);
            if (View.IsValidated)
                return View_ViewDTO;
            else
                return BadRequest(View_ViewDTO);
        }

        [Route(ViewRoute.Import), HttpPost]
        public async Task<List<View_ViewDTO>> Import([FromBody] View_ViewFilterDTO View_ViewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ViewFilter ViewFilter = ConvertFilterDTOToFilterEntity(View_ViewFilterDTO);
            List<View> Views = await ViewService.List(ViewFilter);
            return Views.Select(c => new View_ViewDTO(c)).ToList();
        }

        [Route(ViewRoute.Export), HttpPost]
        public async Task<List<View_ViewDTO>> Export([FromBody] View_ViewFilterDTO View_ViewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ViewFilter ViewFilter = ConvertFilterDTOToFilterEntity(View_ViewFilterDTO);
            List<View> Views = await ViewService.List(ViewFilter);
            return Views.Select(c => new View_ViewDTO(c)).ToList();
        }

        public View ConvertDTOToEntity(View_ViewDTO View_ViewDTO)
        {
            View View = new View();
            View.Id = View_ViewDTO.Id;
            View.Name = View_ViewDTO.Name;
            View.Path = View_ViewDTO.Path;
            View.IsDeleted = View_ViewDTO.IsDeleted;
            View.Pages = View_ViewDTO.Pages?
                .Select(x => new Page
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                    ViewId = x.ViewId,
                    IsDeleted = x.IsDeleted,
                }).ToList();
            View.PermissionFields = View_ViewDTO.PermissionFields?
                .Select(x => new PermissionField
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    ViewId = x.ViewId,
                }).ToList();

            return View;
        }

        public ViewFilter ConvertFilterDTOToFilterEntity(View_ViewFilterDTO View_ViewFilterDTO)
        {
            ViewFilter ViewFilter = new ViewFilter();
            ViewFilter.Selects = ViewSelect.ALL;
            ViewFilter.Skip = View_ViewFilterDTO.Skip;
            ViewFilter.Take = View_ViewFilterDTO.Take;
            ViewFilter.OrderBy = View_ViewFilterDTO.OrderBy;
            ViewFilter.OrderType = View_ViewFilterDTO.OrderType;

            ViewFilter.Id = View_ViewFilterDTO.Id;
            ViewFilter.Name = View_ViewFilterDTO.Name;
            ViewFilter.Path = View_ViewFilterDTO.Path;
            return ViewFilter;
        }

        

    }
}

