using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MApplicationUser;
using Portal.Services.MUserStatus;
using Portal.Services.MRole;


namespace Portal.Controllers.application_user
{
    public class ApplicationUserRoute : Root
    {
        public const string Master = Module + "/application-user/application-user-master";
        public const string Detail = Module + "/application-user/application-user-detail";
        private const string Default = Rpc + Module + "/application-user";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public const string SingleListUserStatus = Default + "/single-list-user-status";
        public const string CountRole = Default + "/count-role";
        public const string ListRole = Default + "/list-role";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ApplicationUser.Id), FieldType.ID },
            { nameof(ApplicationUser.Username), FieldType.STRING },
            { nameof(ApplicationUser.Password), FieldType.STRING },
            { nameof(ApplicationUser.DisplayName), FieldType.STRING },
            { nameof(ApplicationUser.Email), FieldType.STRING },
            { nameof(ApplicationUser.Phone), FieldType.STRING },
            { nameof(ApplicationUser.UserStatusId), FieldType.ID },
        };
    }

    public class ApplicationUserController : ApiController
    {
        private IUserStatusService UserStatusService;
        
        private IRoleService RoleService;
        
        private IApplicationUserService ApplicationUserService;

        public ApplicationUserController(
            IUserStatusService UserStatusService,
            
            IRoleService RoleService,
            
            IApplicationUserService ApplicationUserService
        )
        {
            this.UserStatusService = UserStatusService;
            
            this.RoleService = RoleService;
            
            this.ApplicationUserService = ApplicationUserService;
        }

        [Route(ApplicationUserRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ApplicationUser_ApplicationUserFilterDTO ApplicationUser_ApplicationUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUserFilter ApplicationUserFilter = ConvertFilterDTOToFilterEntity(ApplicationUser_ApplicationUserFilterDTO);
            return await ApplicationUserService.Count(ApplicationUserFilter);
        }

        [Route(ApplicationUserRoute.List), HttpPost]
        public async Task<List<ApplicationUser_ApplicationUserDTO>> List([FromBody] ApplicationUser_ApplicationUserFilterDTO ApplicationUser_ApplicationUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUserFilter ApplicationUserFilter = ConvertFilterDTOToFilterEntity(ApplicationUser_ApplicationUserFilterDTO);
            List<ApplicationUser> ApplicationUsers = await ApplicationUserService.List(ApplicationUserFilter);
            return ApplicationUsers.Select(c => new ApplicationUser_ApplicationUserDTO(c)).ToList();
        }

        [Route(ApplicationUserRoute.Get), HttpPost]
        public async Task<ApplicationUser_ApplicationUserDTO> Get([FromBody]ApplicationUser_ApplicationUserDTO ApplicationUser_ApplicationUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUser ApplicationUser = await ApplicationUserService.Get(ApplicationUser_ApplicationUserDTO.Id);
            return new ApplicationUser_ApplicationUserDTO(ApplicationUser);
        }

        [Route(ApplicationUserRoute.Create), HttpPost]
        public async Task<ActionResult<ApplicationUser_ApplicationUserDTO>> Create([FromBody] ApplicationUser_ApplicationUserDTO ApplicationUser_ApplicationUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUser ApplicationUser = ConvertDTOToEntity(ApplicationUser_ApplicationUserDTO);
            ApplicationUser = await ApplicationUserService.Create(ApplicationUser);
            ApplicationUser_ApplicationUserDTO = new ApplicationUser_ApplicationUserDTO(ApplicationUser);
            if (ApplicationUser.IsValidated)
                return ApplicationUser_ApplicationUserDTO;
            else
                return BadRequest(ApplicationUser_ApplicationUserDTO);
        }

        [Route(ApplicationUserRoute.Update), HttpPost]
        public async Task<ActionResult<ApplicationUser_ApplicationUserDTO>> Update([FromBody] ApplicationUser_ApplicationUserDTO ApplicationUser_ApplicationUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUser ApplicationUser = ConvertDTOToEntity(ApplicationUser_ApplicationUserDTO);
            ApplicationUser = await ApplicationUserService.Update(ApplicationUser);
            ApplicationUser_ApplicationUserDTO = new ApplicationUser_ApplicationUserDTO(ApplicationUser);
            if (ApplicationUser.IsValidated)
                return ApplicationUser_ApplicationUserDTO;
            else
                return BadRequest(ApplicationUser_ApplicationUserDTO);
        }

        [Route(ApplicationUserRoute.Delete), HttpPost]
        public async Task<ActionResult<ApplicationUser_ApplicationUserDTO>> Delete([FromBody] ApplicationUser_ApplicationUserDTO ApplicationUser_ApplicationUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUser ApplicationUser = ConvertDTOToEntity(ApplicationUser_ApplicationUserDTO);
            ApplicationUser = await ApplicationUserService.Delete(ApplicationUser);
            ApplicationUser_ApplicationUserDTO = new ApplicationUser_ApplicationUserDTO(ApplicationUser);
            if (ApplicationUser.IsValidated)
                return ApplicationUser_ApplicationUserDTO;
            else
                return BadRequest(ApplicationUser_ApplicationUserDTO);
        }

        [Route(ApplicationUserRoute.Import), HttpPost]
        public async Task<List<ApplicationUser_ApplicationUserDTO>> Import([FromBody] ApplicationUser_ApplicationUserFilterDTO ApplicationUser_ApplicationUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUserFilter ApplicationUserFilter = ConvertFilterDTOToFilterEntity(ApplicationUser_ApplicationUserFilterDTO);
            List<ApplicationUser> ApplicationUsers = await ApplicationUserService.List(ApplicationUserFilter);
            return ApplicationUsers.Select(c => new ApplicationUser_ApplicationUserDTO(c)).ToList();
        }

        [Route(ApplicationUserRoute.Export), HttpPost]
        public async Task<List<ApplicationUser_ApplicationUserDTO>> Export([FromBody] ApplicationUser_ApplicationUserFilterDTO ApplicationUser_ApplicationUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ApplicationUserFilter ApplicationUserFilter = ConvertFilterDTOToFilterEntity(ApplicationUser_ApplicationUserFilterDTO);
            List<ApplicationUser> ApplicationUsers = await ApplicationUserService.List(ApplicationUserFilter);
            return ApplicationUsers.Select(c => new ApplicationUser_ApplicationUserDTO(c)).ToList();
        }

        public ApplicationUser ConvertDTOToEntity(ApplicationUser_ApplicationUserDTO ApplicationUser_ApplicationUserDTO)
        {
            ApplicationUser ApplicationUser = new ApplicationUser();
            ApplicationUser.Id = ApplicationUser_ApplicationUserDTO.Id;
            ApplicationUser.Username = ApplicationUser_ApplicationUserDTO.Username;
            ApplicationUser.Password = ApplicationUser_ApplicationUserDTO.Password;
            ApplicationUser.DisplayName = ApplicationUser_ApplicationUserDTO.DisplayName;
            ApplicationUser.Email = ApplicationUser_ApplicationUserDTO.Email;
            ApplicationUser.Phone = ApplicationUser_ApplicationUserDTO.Phone;
            ApplicationUser.UserStatusId = ApplicationUser_ApplicationUserDTO.UserStatusId;
            ApplicationUser.UserStatus = ApplicationUser_ApplicationUserDTO.UserStatus == null ? null : new UserStatus
            {
                Id = ApplicationUser_ApplicationUserDTO.UserStatus.Id,
                Code = ApplicationUser_ApplicationUserDTO.UserStatus.Code,
                Name = ApplicationUser_ApplicationUserDTO.UserStatus.Name,
            };

            return ApplicationUser;
        }

        public ApplicationUserFilter ConvertFilterDTOToFilterEntity(ApplicationUser_ApplicationUserFilterDTO ApplicationUser_ApplicationUserFilterDTO)
        {
            ApplicationUserFilter ApplicationUserFilter = new ApplicationUserFilter();
            ApplicationUserFilter.Selects = ApplicationUserSelect.ALL;
            ApplicationUserFilter.Skip = ApplicationUser_ApplicationUserFilterDTO.Skip;
            ApplicationUserFilter.Take = ApplicationUser_ApplicationUserFilterDTO.Take;
            ApplicationUserFilter.OrderBy = ApplicationUser_ApplicationUserFilterDTO.OrderBy;
            ApplicationUserFilter.OrderType = ApplicationUser_ApplicationUserFilterDTO.OrderType;

            ApplicationUserFilter.Id = ApplicationUser_ApplicationUserFilterDTO.Id;
            ApplicationUserFilter.Username = ApplicationUser_ApplicationUserFilterDTO.Username;
            ApplicationUserFilter.Password = ApplicationUser_ApplicationUserFilterDTO.Password;
            ApplicationUserFilter.DisplayName = ApplicationUser_ApplicationUserFilterDTO.DisplayName;
            ApplicationUserFilter.Email = ApplicationUser_ApplicationUserFilterDTO.Email;
            ApplicationUserFilter.Phone = ApplicationUser_ApplicationUserFilterDTO.Phone;
            ApplicationUserFilter.UserStatusId = ApplicationUser_ApplicationUserFilterDTO.UserStatusId;
            return ApplicationUserFilter;
        }

        
        [Route(ApplicationUserRoute.SingleListUserStatus), HttpPost]
        public async Task<List<ApplicationUser_UserStatusDTO>> SingleListUserStatus([FromBody] ApplicationUser_UserStatusFilterDTO ApplicationUser_UserStatusFilterDTO)
        {
            UserStatusFilter UserStatusFilter = new UserStatusFilter();
            UserStatusFilter.Skip = 0;
            UserStatusFilter.Take = 20;
            UserStatusFilter.OrderBy = UserStatusOrder.Id;
            UserStatusFilter.OrderType = OrderType.ASC;
            UserStatusFilter.Selects = UserStatusSelect.ALL;
            UserStatusFilter.Id = ApplicationUser_UserStatusFilterDTO.Id;
            UserStatusFilter.Code = ApplicationUser_UserStatusFilterDTO.Code;
            UserStatusFilter.Name = ApplicationUser_UserStatusFilterDTO.Name;

            List<UserStatus> UserStatuss = await UserStatusService.List(UserStatusFilter);
            List<ApplicationUser_UserStatusDTO> ApplicationUser_UserStatusDTOs = UserStatuss
                .Select(x => new ApplicationUser_UserStatusDTO(x)).ToList();
            return ApplicationUser_UserStatusDTOs;
        }
        

        [Route(ApplicationUserRoute.CountRole), HttpPost]
        public async Task<long> CountRole([FromBody] ApplicationUser_RoleFilterDTO ApplicationUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Id = ApplicationUser_RoleFilterDTO.Id;
            RoleFilter.Name = ApplicationUser_RoleFilterDTO.Name;

            return await RoleService.Count(RoleFilter);
        }

        [Route(ApplicationUserRoute.ListRole), HttpPost]
        public async Task<List<ApplicationUser_RoleDTO>> ListRole([FromBody] ApplicationUser_RoleFilterDTO ApplicationUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = ApplicationUser_RoleFilterDTO.Id;
            RoleFilter.Name = ApplicationUser_RoleFilterDTO.Name;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<ApplicationUser_RoleDTO> ApplicationUser_RoleDTOs = Roles
                .Select(x => new ApplicationUser_RoleDTO(x)).ToList();
            return ApplicationUser_RoleDTOs;
        }
    }
}

