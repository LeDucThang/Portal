using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MPermissionField;
using Portal.Services.MView;


namespace Portal.Controllers.permission_field
{
    public class PermissionFieldRoute : Root
    {
        public const string Master = Module + "/permission-field/permission-field-master";
        public const string Detail = Module + "/permission-field/permission-field-detail";
        private const string Default = Rpc + Module + "/permission-field";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public const string SingleListView = Default + "/single-list-view";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(PermissionField.Id), FieldType.ID },
            { nameof(PermissionField.Name), FieldType.STRING },
            { nameof(PermissionField.Type), FieldType.STRING },
            { nameof(PermissionField.ViewId), FieldType.ID },
        };
    }

    public class PermissionFieldController : ApiController
    {
        private IViewService ViewService;
        
        private IPermissionFieldService PermissionFieldService;

        public PermissionFieldController(
            IViewService ViewService,
            
            IPermissionFieldService PermissionFieldService
        )
        {
            this.ViewService = ViewService;
            
            this.PermissionFieldService = PermissionFieldService;
        }

        [Route(PermissionFieldRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] PermissionField_PermissionFieldFilterDTO PermissionField_PermissionFieldFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFieldFilter PermissionFieldFilter = ConvertFilterDTOToFilterEntity(PermissionField_PermissionFieldFilterDTO);
            return await PermissionFieldService.Count(PermissionFieldFilter);
        }

        [Route(PermissionFieldRoute.List), HttpPost]
        public async Task<List<PermissionField_PermissionFieldDTO>> List([FromBody] PermissionField_PermissionFieldFilterDTO PermissionField_PermissionFieldFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFieldFilter PermissionFieldFilter = ConvertFilterDTOToFilterEntity(PermissionField_PermissionFieldFilterDTO);
            List<PermissionField> PermissionFields = await PermissionFieldService.List(PermissionFieldFilter);
            return PermissionFields.Select(c => new PermissionField_PermissionFieldDTO(c)).ToList();
        }

        [Route(PermissionFieldRoute.Get), HttpPost]
        public async Task<PermissionField_PermissionFieldDTO> Get([FromBody]PermissionField_PermissionFieldDTO PermissionField_PermissionFieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionField PermissionField = await PermissionFieldService.Get(PermissionField_PermissionFieldDTO.Id);
            return new PermissionField_PermissionFieldDTO(PermissionField);
        }

        [Route(PermissionFieldRoute.Create), HttpPost]
        public async Task<ActionResult<PermissionField_PermissionFieldDTO>> Create([FromBody] PermissionField_PermissionFieldDTO PermissionField_PermissionFieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionField PermissionField = ConvertDTOToEntity(PermissionField_PermissionFieldDTO);
            PermissionField = await PermissionFieldService.Create(PermissionField);
            PermissionField_PermissionFieldDTO = new PermissionField_PermissionFieldDTO(PermissionField);
            if (PermissionField.IsValidated)
                return PermissionField_PermissionFieldDTO;
            else
                return BadRequest(PermissionField_PermissionFieldDTO);
        }

        [Route(PermissionFieldRoute.Update), HttpPost]
        public async Task<ActionResult<PermissionField_PermissionFieldDTO>> Update([FromBody] PermissionField_PermissionFieldDTO PermissionField_PermissionFieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionField PermissionField = ConvertDTOToEntity(PermissionField_PermissionFieldDTO);
            PermissionField = await PermissionFieldService.Update(PermissionField);
            PermissionField_PermissionFieldDTO = new PermissionField_PermissionFieldDTO(PermissionField);
            if (PermissionField.IsValidated)
                return PermissionField_PermissionFieldDTO;
            else
                return BadRequest(PermissionField_PermissionFieldDTO);
        }

        [Route(PermissionFieldRoute.Delete), HttpPost]
        public async Task<ActionResult<PermissionField_PermissionFieldDTO>> Delete([FromBody] PermissionField_PermissionFieldDTO PermissionField_PermissionFieldDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionField PermissionField = ConvertDTOToEntity(PermissionField_PermissionFieldDTO);
            PermissionField = await PermissionFieldService.Delete(PermissionField);
            PermissionField_PermissionFieldDTO = new PermissionField_PermissionFieldDTO(PermissionField);
            if (PermissionField.IsValidated)
                return PermissionField_PermissionFieldDTO;
            else
                return BadRequest(PermissionField_PermissionFieldDTO);
        }

        [Route(PermissionFieldRoute.Import), HttpPost]
        public async Task<List<PermissionField_PermissionFieldDTO>> Import([FromBody] PermissionField_PermissionFieldFilterDTO PermissionField_PermissionFieldFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFieldFilter PermissionFieldFilter = ConvertFilterDTOToFilterEntity(PermissionField_PermissionFieldFilterDTO);
            List<PermissionField> PermissionFields = await PermissionFieldService.List(PermissionFieldFilter);
            return PermissionFields.Select(c => new PermissionField_PermissionFieldDTO(c)).ToList();
        }

        [Route(PermissionFieldRoute.Export), HttpPost]
        public async Task<List<PermissionField_PermissionFieldDTO>> Export([FromBody] PermissionField_PermissionFieldFilterDTO PermissionField_PermissionFieldFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFieldFilter PermissionFieldFilter = ConvertFilterDTOToFilterEntity(PermissionField_PermissionFieldFilterDTO);
            List<PermissionField> PermissionFields = await PermissionFieldService.List(PermissionFieldFilter);
            return PermissionFields.Select(c => new PermissionField_PermissionFieldDTO(c)).ToList();
        }

        public PermissionField ConvertDTOToEntity(PermissionField_PermissionFieldDTO PermissionField_PermissionFieldDTO)
        {
            PermissionField PermissionField = new PermissionField();
            PermissionField.Id = PermissionField_PermissionFieldDTO.Id;
            PermissionField.Name = PermissionField_PermissionFieldDTO.Name;
            PermissionField.Type = PermissionField_PermissionFieldDTO.Type;
            PermissionField.ViewId = PermissionField_PermissionFieldDTO.ViewId;
            PermissionField.View = PermissionField_PermissionFieldDTO.View == null ? null : new View
            {
                Id = PermissionField_PermissionFieldDTO.View.Id,
                Name = PermissionField_PermissionFieldDTO.View.Name,
                Path = PermissionField_PermissionFieldDTO.View.Path,
                IsDeleted = PermissionField_PermissionFieldDTO.View.IsDeleted,
            };
            PermissionField.PermissionDatas = PermissionField_PermissionFieldDTO.PermissionDatas?
                .Select(x => new PermissionData
                {
                    Id = x.Id,
                    PermissionId = x.PermissionId,
                    PermissionFieldId = x.PermissionFieldId,
                    Value = x.Value,
                }).ToList();

            return PermissionField;
        }

        public PermissionFieldFilter ConvertFilterDTOToFilterEntity(PermissionField_PermissionFieldFilterDTO PermissionField_PermissionFieldFilterDTO)
        {
            PermissionFieldFilter PermissionFieldFilter = new PermissionFieldFilter();
            PermissionFieldFilter.Selects = PermissionFieldSelect.ALL;
            PermissionFieldFilter.Skip = PermissionField_PermissionFieldFilterDTO.Skip;
            PermissionFieldFilter.Take = PermissionField_PermissionFieldFilterDTO.Take;
            PermissionFieldFilter.OrderBy = PermissionField_PermissionFieldFilterDTO.OrderBy;
            PermissionFieldFilter.OrderType = PermissionField_PermissionFieldFilterDTO.OrderType;

            PermissionFieldFilter.Id = PermissionField_PermissionFieldFilterDTO.Id;
            PermissionFieldFilter.Name = PermissionField_PermissionFieldFilterDTO.Name;
            PermissionFieldFilter.Type = PermissionField_PermissionFieldFilterDTO.Type;
            PermissionFieldFilter.ViewId = PermissionField_PermissionFieldFilterDTO.ViewId;
            return PermissionFieldFilter;
        }

        
        [Route(PermissionFieldRoute.SingleListView), HttpPost]
        public async Task<List<PermissionField_ViewDTO>> SingleListView([FromBody] PermissionField_ViewFilterDTO PermissionField_ViewFilterDTO)
        {
            ViewFilter ViewFilter = new ViewFilter();
            ViewFilter.Skip = 0;
            ViewFilter.Take = 20;
            ViewFilter.OrderBy = ViewOrder.Id;
            ViewFilter.OrderType = OrderType.ASC;
            ViewFilter.Selects = ViewSelect.ALL;
            ViewFilter.Id = PermissionField_ViewFilterDTO.Id;
            ViewFilter.Name = PermissionField_ViewFilterDTO.Name;
            ViewFilter.Path = PermissionField_ViewFilterDTO.Path;

            List<View> Views = await ViewService.List(ViewFilter);
            List<PermissionField_ViewDTO> PermissionField_ViewDTOs = Views
                .Select(x => new PermissionField_ViewDTO(x)).ToList();
            return PermissionField_ViewDTOs;
        }
        

    }
}

