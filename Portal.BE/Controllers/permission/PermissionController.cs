using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MPermission;
using Portal.Services.MRole;
using Portal.Services.MPage;


namespace Portal.Controllers.permission
{
    public class PermissionRoute : Root
    {
        public const string Master = Module + "/permission/permission-master";
        public const string Detail = Module + "/permission/permission-detail";
        private const string Default = Rpc + Module + "/permission";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public const string SingleListRole = Default + "/single-list-role";
        public const string CountPage = Default + "/count-page";
        public const string ListPage = Default + "/list-page";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(Permission.Id), FieldType.ID },
            { nameof(Permission.Name), FieldType.STRING },
            { nameof(Permission.RoleId), FieldType.ID },
        };
    }

    public class PermissionController : ApiController
    {
        private IRoleService RoleService;
        
        private IPageService PageService;
        
        private IPermissionService PermissionService;

        public PermissionController(
            IRoleService RoleService,
            
            IPageService PageService,
            
            IPermissionService PermissionService
        )
        {
            this.RoleService = RoleService;
            
            this.PageService = PageService;
            
            this.PermissionService = PermissionService;
        }

        [Route(PermissionRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO);
            return await PermissionService.Count(PermissionFilter);
        }

        [Route(PermissionRoute.List), HttpPost]
        public async Task<List<Permission_PermissionDTO>> List([FromBody] Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO);
            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            return Permissions.Select(c => new Permission_PermissionDTO(c)).ToList();
        }

        [Route(PermissionRoute.Get), HttpPost]
        public async Task<Permission_PermissionDTO> Get([FromBody]Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Permission Permission = await PermissionService.Get(Permission_PermissionDTO.Id);
            return new Permission_PermissionDTO(Permission);
        }

        [Route(PermissionRoute.Create), HttpPost]
        public async Task<ActionResult<Permission_PermissionDTO>> Create([FromBody] Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Permission Permission = ConvertDTOToEntity(Permission_PermissionDTO);
            Permission = await PermissionService.Create(Permission);
            Permission_PermissionDTO = new Permission_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Permission_PermissionDTO;
            else
                return BadRequest(Permission_PermissionDTO);
        }

        [Route(PermissionRoute.Update), HttpPost]
        public async Task<ActionResult<Permission_PermissionDTO>> Update([FromBody] Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Permission Permission = ConvertDTOToEntity(Permission_PermissionDTO);
            Permission = await PermissionService.Update(Permission);
            Permission_PermissionDTO = new Permission_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Permission_PermissionDTO;
            else
                return BadRequest(Permission_PermissionDTO);
        }

        [Route(PermissionRoute.Delete), HttpPost]
        public async Task<ActionResult<Permission_PermissionDTO>> Delete([FromBody] Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Permission Permission = ConvertDTOToEntity(Permission_PermissionDTO);
            Permission = await PermissionService.Delete(Permission);
            Permission_PermissionDTO = new Permission_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Permission_PermissionDTO;
            else
                return BadRequest(Permission_PermissionDTO);
        }

        [Route(PermissionRoute.Import), HttpPost]
        public async Task<List<Permission_PermissionDTO>> Import([FromBody] Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO);
            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            return Permissions.Select(c => new Permission_PermissionDTO(c)).ToList();
        }

        [Route(PermissionRoute.Export), HttpPost]
        public async Task<List<Permission_PermissionDTO>> Export([FromBody] Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO);
            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            return Permissions.Select(c => new Permission_PermissionDTO(c)).ToList();
        }

        public Permission ConvertDTOToEntity(Permission_PermissionDTO Permission_PermissionDTO)
        {
            Permission Permission = new Permission();
            Permission.Id = Permission_PermissionDTO.Id;
            Permission.Name = Permission_PermissionDTO.Name;
            Permission.RoleId = Permission_PermissionDTO.RoleId;
            Permission.Role = Permission_PermissionDTO.Role == null ? null : new Role
            {
                Id = Permission_PermissionDTO.Role.Id,
                Name = Permission_PermissionDTO.Role.Name,
            };
            Permission.PermissionDatas = Permission_PermissionDTO.PermissionDatas?
                .Select(x => new PermissionData
                {
                    Id = x.Id,
                    PermissionId = x.PermissionId,
                    PermissionFieldId = x.PermissionFieldId,
                    Value = x.Value,
                }).ToList();

            return Permission;
        }

        public PermissionFilter ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter.Selects = PermissionSelect.ALL;
            PermissionFilter.Skip = Permission_PermissionFilterDTO.Skip;
            PermissionFilter.Take = Permission_PermissionFilterDTO.Take;
            PermissionFilter.OrderBy = Permission_PermissionFilterDTO.OrderBy;
            PermissionFilter.OrderType = Permission_PermissionFilterDTO.OrderType;

            PermissionFilter.Id = Permission_PermissionFilterDTO.Id;
            PermissionFilter.Name = Permission_PermissionFilterDTO.Name;
            PermissionFilter.RoleId = Permission_PermissionFilterDTO.RoleId;
            return PermissionFilter;
        }

        
        [Route(PermissionRoute.SingleListRole), HttpPost]
        public async Task<List<Permission_RoleDTO>> SingleListRole([FromBody] Permission_RoleFilterDTO Permission_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = Permission_RoleFilterDTO.Id;
            RoleFilter.Name = Permission_RoleFilterDTO.Name;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<Permission_RoleDTO> Permission_RoleDTOs = Roles
                .Select(x => new Permission_RoleDTO(x)).ToList();
            return Permission_RoleDTOs;
        }
        

        [Route(PermissionRoute.CountPage), HttpPost]
        public async Task<long> CountPage([FromBody] Permission_PageFilterDTO Permission_PageFilterDTO)
        {
            PageFilter PageFilter = new PageFilter();
            PageFilter.Id = Permission_PageFilterDTO.Id;
            PageFilter.Name = Permission_PageFilterDTO.Name;
            PageFilter.Path = Permission_PageFilterDTO.Path;
            PageFilter.ViewId = Permission_PageFilterDTO.ViewId;

            return await PageService.Count(PageFilter);
        }

        [Route(PermissionRoute.ListPage), HttpPost]
        public async Task<List<Permission_PageDTO>> ListPage([FromBody] Permission_PageFilterDTO Permission_PageFilterDTO)
        {
            PageFilter PageFilter = new PageFilter();
            PageFilter.Skip = 0;
            PageFilter.Take = 20;
            PageFilter.OrderBy = PageOrder.Id;
            PageFilter.OrderType = OrderType.ASC;
            PageFilter.Selects = PageSelect.ALL;
            PageFilter.Id = Permission_PageFilterDTO.Id;
            PageFilter.Name = Permission_PageFilterDTO.Name;
            PageFilter.Path = Permission_PageFilterDTO.Path;
            PageFilter.ViewId = Permission_PageFilterDTO.ViewId;

            List<Page> Pages = await PageService.List(PageFilter);
            List<Permission_PageDTO> Permission_PageDTOs = Pages
                .Select(x => new Permission_PageDTO(x)).ToList();
            return Permission_PageDTOs;
        }
    }
}

