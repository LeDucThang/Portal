using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MRole;

using Portal.Services.MApplicationUser;


namespace Portal.Controllers.role
{
    public class RoleRoute : Root
    {
        public const string FE = "/role";
        private const string Default = Base + FE;
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public const string CountApplicationUser = Default + "/count-application-user";
        public const string ListApplicationUser = Default + "/list-application-user";
    }
    
    public class RoleController : ApiController
    {
        
        private IApplicationUserService ApplicationUserService;
        
        private IRoleService RoleService;

        public RoleController(
            
            IApplicationUserService ApplicationUserService,
            
            IRoleService RoleService
        )
        {
            
            this.ApplicationUserService = ApplicationUserService;
            
            this.RoleService = RoleService;
        }

        [Route(RoleRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            return await RoleService.Count(RoleFilter);
        }

        [Route(RoleRoute.List), HttpPost]
        public async Task<List<Role_RoleDTO>> List([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            List<Role> Roles = await RoleService.List(RoleFilter);
            return Roles.Select(c => new Role_RoleDTO(c)).ToList();
        }

        [Route(RoleRoute.Get), HttpPost]
        public async Task<Role_RoleDTO> Get([FromBody]Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Role Role = await RoleService.Get(Role_RoleDTO.Id);
            return new Role_RoleDTO(Role);
        }

        [Route(RoleRoute.Create), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Create([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Create(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Update), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Update([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Update(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Delete), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Delete([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Delete(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Import), HttpPost]
        public async Task<List<Role_RoleDTO>> Import([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            List<Role> Roles = await RoleService.List(RoleFilter);
            return Roles.Select(c => new Role_RoleDTO(c)).ToList();
        }
        
        [Route(RoleRoute.Export), HttpPost]
        public async Task<List<Role_RoleDTO>> Export([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            List<Role> Roles = await RoleService.List(RoleFilter);
            return Roles.Select(c => new Role_RoleDTO(c)).ToList();
        }

        public Role ConvertDTOToEntity(Role_RoleDTO Role_RoleDTO)
        {
            Role Role = new Role();
            Role.Id = Role_RoleDTO.Id;
            Role.Name = Role_RoleDTO.Name;
            Role.Permissions = Role_RoleDTO.Permissions?
                .Select(x => new Permission
                {
                    Id = x.Id,
                    Name = x.Name,
                    RoleId = x.RoleId,
                }).ToList();
            
            return Role;
        }

        public RoleFilter ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Skip = Role_RoleFilterDTO.Skip;
            RoleFilter.Take = Role_RoleFilterDTO.Take;
            RoleFilter.OrderBy = Role_RoleFilterDTO.OrderBy;
            RoleFilter.OrderType = Role_RoleFilterDTO.OrderType;

            RoleFilter.Id = Role_RoleFilterDTO.Id;
            RoleFilter.Name = Role_RoleFilterDTO.Name;
            return RoleFilter;
        }

        

        [Route(RoleRoute.CountApplicationUser), HttpPost]
        public async Task<long> CountApplicationUser([FromBody] Role_ApplicationUserFilterDTO Role_ApplicationUserFilterDTO)
        {
            ApplicationUserFilter ApplicationUserFilter = new ApplicationUserFilter();
            ApplicationUserFilter.Id = Role_ApplicationUserFilterDTO.Id;
            ApplicationUserFilter.Username = Role_ApplicationUserFilterDTO.Username;
            ApplicationUserFilter.Password = Role_ApplicationUserFilterDTO.Password;
            ApplicationUserFilter.DisplayName = Role_ApplicationUserFilterDTO.DisplayName;
            ApplicationUserFilter.Email = Role_ApplicationUserFilterDTO.Email;
            ApplicationUserFilter.Phone = Role_ApplicationUserFilterDTO.Phone;
            ApplicationUserFilter.UserStatusId = Role_ApplicationUserFilterDTO.UserStatusId;
            ApplicationUserFilter.RetryTime = Role_ApplicationUserFilterDTO.RetryTime;
            ApplicationUserFilter.ProviderId = Role_ApplicationUserFilterDTO.ProviderId;

            return await ApplicationUserService.Count(ApplicationUserFilter);
        }

        [Route(RoleRoute.ListApplicationUser), HttpPost]
        public async Task<List<Role_ApplicationUserDTO>> ListApplicationUser([FromBody] Role_ApplicationUserFilterDTO Role_ApplicationUserFilterDTO)
        {
            ApplicationUserFilter ApplicationUserFilter = new ApplicationUserFilter();
            ApplicationUserFilter.Skip = 0;
            ApplicationUserFilter.Take = 20;
            ApplicationUserFilter.OrderBy = ApplicationUserOrder.Id;
            ApplicationUserFilter.OrderType = OrderType.ASC;
            ApplicationUserFilter.Selects = ApplicationUserSelect.ALL;
            ApplicationUserFilter.Id = Role_ApplicationUserFilterDTO.Id;
            ApplicationUserFilter.Username = Role_ApplicationUserFilterDTO.Username;
            ApplicationUserFilter.Password = Role_ApplicationUserFilterDTO.Password;
            ApplicationUserFilter.DisplayName = Role_ApplicationUserFilterDTO.DisplayName;
            ApplicationUserFilter.Email = Role_ApplicationUserFilterDTO.Email;
            ApplicationUserFilter.Phone = Role_ApplicationUserFilterDTO.Phone;
            ApplicationUserFilter.UserStatusId = Role_ApplicationUserFilterDTO.UserStatusId;
            ApplicationUserFilter.RetryTime = Role_ApplicationUserFilterDTO.RetryTime;
            ApplicationUserFilter.ProviderId = Role_ApplicationUserFilterDTO.ProviderId;

            List<ApplicationUser> ApplicationUsers = await ApplicationUserService.List(ApplicationUserFilter);
            List<Role_ApplicationUserDTO> Role_ApplicationUserDTOs = ApplicationUsers
                .Select(x => new Role_ApplicationUserDTO(x)).ToList();
            return Role_ApplicationUserDTOs;
        }
    }
}

