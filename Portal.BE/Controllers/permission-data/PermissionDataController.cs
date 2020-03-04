using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MPermissionData;
using Portal.Services.MPermission;using Portal.Services.MPermissionField;


namespace Portal.Controllers.permission_data
{
    public class PermissionDataRoute : Root
    {
        public const string Master = Module + "/permission-data/permission-data-master";
        public const string Detail = Module + "/permission-data/permission-data-detail";
        private const string Default = Rpc + Module + "/permission-data";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public const string SingleListPermission = Default + "/single-list-permission";
        public const string SingleListPermissionField = Default + "/single-list-permission-field";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(PermissionData.Id), FieldType.ID },
            { nameof(PermissionData.PermissionId), FieldType.ID },
            { nameof(PermissionData.PermissionFieldId), FieldType.ID },
            { nameof(PermissionData.Value), FieldType.STRING },
        };
    }

    public class PermissionDataController : ApiController
    {
        private IPermissionService PermissionService;
        private IPermissionFieldService PermissionFieldService;
        
        private IPermissionDataService PermissionDataService;

        public PermissionDataController(
            IPermissionService PermissionService,
            IPermissionFieldService PermissionFieldService,
            
            IPermissionDataService PermissionDataService
        )
        {
            this.PermissionService = PermissionService;
            this.PermissionFieldService = PermissionFieldService;
            
            this.PermissionDataService = PermissionDataService;
        }

        [Route(PermissionDataRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] PermissionData_PermissionDataFilterDTO PermissionData_PermissionDataFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionDataFilter PermissionDataFilter = ConvertFilterDTOToFilterEntity(PermissionData_PermissionDataFilterDTO);
            return await PermissionDataService.Count(PermissionDataFilter);
        }

        [Route(PermissionDataRoute.List), HttpPost]
        public async Task<List<PermissionData_PermissionDataDTO>> List([FromBody] PermissionData_PermissionDataFilterDTO PermissionData_PermissionDataFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionDataFilter PermissionDataFilter = ConvertFilterDTOToFilterEntity(PermissionData_PermissionDataFilterDTO);
            List<PermissionData> PermissionDatas = await PermissionDataService.List(PermissionDataFilter);
            return PermissionDatas.Select(c => new PermissionData_PermissionDataDTO(c)).ToList();
        }

        [Route(PermissionDataRoute.Get), HttpPost]
        public async Task<PermissionData_PermissionDataDTO> Get([FromBody]PermissionData_PermissionDataDTO PermissionData_PermissionDataDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionData PermissionData = await PermissionDataService.Get(PermissionData_PermissionDataDTO.Id);
            return new PermissionData_PermissionDataDTO(PermissionData);
        }

        [Route(PermissionDataRoute.Create), HttpPost]
        public async Task<ActionResult<PermissionData_PermissionDataDTO>> Create([FromBody] PermissionData_PermissionDataDTO PermissionData_PermissionDataDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionData PermissionData = ConvertDTOToEntity(PermissionData_PermissionDataDTO);
            PermissionData = await PermissionDataService.Create(PermissionData);
            PermissionData_PermissionDataDTO = new PermissionData_PermissionDataDTO(PermissionData);
            if (PermissionData.IsValidated)
                return PermissionData_PermissionDataDTO;
            else
                return BadRequest(PermissionData_PermissionDataDTO);
        }

        [Route(PermissionDataRoute.Update), HttpPost]
        public async Task<ActionResult<PermissionData_PermissionDataDTO>> Update([FromBody] PermissionData_PermissionDataDTO PermissionData_PermissionDataDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionData PermissionData = ConvertDTOToEntity(PermissionData_PermissionDataDTO);
            PermissionData = await PermissionDataService.Update(PermissionData);
            PermissionData_PermissionDataDTO = new PermissionData_PermissionDataDTO(PermissionData);
            if (PermissionData.IsValidated)
                return PermissionData_PermissionDataDTO;
            else
                return BadRequest(PermissionData_PermissionDataDTO);
        }

        [Route(PermissionDataRoute.Delete), HttpPost]
        public async Task<ActionResult<PermissionData_PermissionDataDTO>> Delete([FromBody] PermissionData_PermissionDataDTO PermissionData_PermissionDataDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionData PermissionData = ConvertDTOToEntity(PermissionData_PermissionDataDTO);
            PermissionData = await PermissionDataService.Delete(PermissionData);
            PermissionData_PermissionDataDTO = new PermissionData_PermissionDataDTO(PermissionData);
            if (PermissionData.IsValidated)
                return PermissionData_PermissionDataDTO;
            else
                return BadRequest(PermissionData_PermissionDataDTO);
        }

        [Route(PermissionDataRoute.Import), HttpPost]
        public async Task<List<PermissionData_PermissionDataDTO>> Import([FromBody] PermissionData_PermissionDataFilterDTO PermissionData_PermissionDataFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionDataFilter PermissionDataFilter = ConvertFilterDTOToFilterEntity(PermissionData_PermissionDataFilterDTO);
            List<PermissionData> PermissionDatas = await PermissionDataService.List(PermissionDataFilter);
            return PermissionDatas.Select(c => new PermissionData_PermissionDataDTO(c)).ToList();
        }

        [Route(PermissionDataRoute.Export), HttpPost]
        public async Task<List<PermissionData_PermissionDataDTO>> Export([FromBody] PermissionData_PermissionDataFilterDTO PermissionData_PermissionDataFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionDataFilter PermissionDataFilter = ConvertFilterDTOToFilterEntity(PermissionData_PermissionDataFilterDTO);
            List<PermissionData> PermissionDatas = await PermissionDataService.List(PermissionDataFilter);
            return PermissionDatas.Select(c => new PermissionData_PermissionDataDTO(c)).ToList();
        }

        public PermissionData ConvertDTOToEntity(PermissionData_PermissionDataDTO PermissionData_PermissionDataDTO)
        {
            PermissionData PermissionData = new PermissionData();
            PermissionData.Id = PermissionData_PermissionDataDTO.Id;
            PermissionData.PermissionId = PermissionData_PermissionDataDTO.PermissionId;
            PermissionData.PermissionFieldId = PermissionData_PermissionDataDTO.PermissionFieldId;
            PermissionData.Value = PermissionData_PermissionDataDTO.Value;
            PermissionData.Permission = PermissionData_PermissionDataDTO.Permission == null ? null : new Permission
            {
                Id = PermissionData_PermissionDataDTO.Permission.Id,
                Name = PermissionData_PermissionDataDTO.Permission.Name,
                RoleId = PermissionData_PermissionDataDTO.Permission.RoleId,
            };
            PermissionData.PermissionField = PermissionData_PermissionDataDTO.PermissionField == null ? null : new PermissionField
            {
                Id = PermissionData_PermissionDataDTO.PermissionField.Id,
                Name = PermissionData_PermissionDataDTO.PermissionField.Name,
                Type = PermissionData_PermissionDataDTO.PermissionField.Type,
                ViewId = PermissionData_PermissionDataDTO.PermissionField.ViewId,
            };

            return PermissionData;
        }

        public PermissionDataFilter ConvertFilterDTOToFilterEntity(PermissionData_PermissionDataFilterDTO PermissionData_PermissionDataFilterDTO)
        {
            PermissionDataFilter PermissionDataFilter = new PermissionDataFilter();
            PermissionDataFilter.Selects = PermissionDataSelect.ALL;
            PermissionDataFilter.Skip = PermissionData_PermissionDataFilterDTO.Skip;
            PermissionDataFilter.Take = PermissionData_PermissionDataFilterDTO.Take;
            PermissionDataFilter.OrderBy = PermissionData_PermissionDataFilterDTO.OrderBy;
            PermissionDataFilter.OrderType = PermissionData_PermissionDataFilterDTO.OrderType;

            PermissionDataFilter.Id = PermissionData_PermissionDataFilterDTO.Id;
            PermissionDataFilter.PermissionId = PermissionData_PermissionDataFilterDTO.PermissionId;
            PermissionDataFilter.PermissionFieldId = PermissionData_PermissionDataFilterDTO.PermissionFieldId;
            PermissionDataFilter.Value = PermissionData_PermissionDataFilterDTO.Value;
            return PermissionDataFilter;
        }

        
        [Route(PermissionDataRoute.SingleListPermission), HttpPost]
        public async Task<List<PermissionData_PermissionDTO>> SingleListPermission([FromBody] PermissionData_PermissionFilterDTO PermissionData_PermissionFilterDTO)
        {
            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter.Skip = 0;
            PermissionFilter.Take = 20;
            PermissionFilter.OrderBy = PermissionOrder.Id;
            PermissionFilter.OrderType = OrderType.ASC;
            PermissionFilter.Selects = PermissionSelect.ALL;
            PermissionFilter.Id = PermissionData_PermissionFilterDTO.Id;
            PermissionFilter.Name = PermissionData_PermissionFilterDTO.Name;
            PermissionFilter.RoleId = PermissionData_PermissionFilterDTO.RoleId;

            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            List<PermissionData_PermissionDTO> PermissionData_PermissionDTOs = Permissions
                .Select(x => new PermissionData_PermissionDTO(x)).ToList();
            return PermissionData_PermissionDTOs;
        }
        
        [Route(PermissionDataRoute.SingleListPermissionField), HttpPost]
        public async Task<List<PermissionData_PermissionFieldDTO>> SingleListPermissionField([FromBody] PermissionData_PermissionFieldFilterDTO PermissionData_PermissionFieldFilterDTO)
        {
            PermissionFieldFilter PermissionFieldFilter = new PermissionFieldFilter();
            PermissionFieldFilter.Skip = 0;
            PermissionFieldFilter.Take = 20;
            PermissionFieldFilter.OrderBy = PermissionFieldOrder.Id;
            PermissionFieldFilter.OrderType = OrderType.ASC;
            PermissionFieldFilter.Selects = PermissionFieldSelect.ALL;
            PermissionFieldFilter.Id = PermissionData_PermissionFieldFilterDTO.Id;
            PermissionFieldFilter.Name = PermissionData_PermissionFieldFilterDTO.Name;
            PermissionFieldFilter.Type = PermissionData_PermissionFieldFilterDTO.Type;
            PermissionFieldFilter.ViewId = PermissionData_PermissionFieldFilterDTO.ViewId;

            List<PermissionField> PermissionFields = await PermissionFieldService.List(PermissionFieldFilter);
            List<PermissionData_PermissionFieldDTO> PermissionData_PermissionFieldDTOs = PermissionFields
                .Select(x => new PermissionData_PermissionFieldDTO(x)).ToList();
            return PermissionData_PermissionFieldDTOs;
        }
        

    }
}

