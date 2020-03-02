using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MProvider;
using Portal.Services.MProviderType;


namespace Portal.Controllers.provider
{
    public class ProviderRoute : Root
    {
        public const string FE = "/provider";
        private const string Default = Base + FE;
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

        public const string SingleListProviderType = Default + "/single-list-provider-type";
    }
    
    public class ProviderController : ApiController
    {
        private IProviderTypeService ProviderTypeService;
        
        private IProviderService ProviderService;

        public ProviderController(
            IProviderTypeService ProviderTypeService,
            
            IProviderService ProviderService
        )
        {
            this.ProviderTypeService = ProviderTypeService;
            
            this.ProviderService = ProviderService;
        }

        [Route(ProviderRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Provider_ProviderFilterDTO Provider_ProviderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProviderFilter ProviderFilter = ConvertFilterDTOToFilterEntity(Provider_ProviderFilterDTO);
            return await ProviderService.Count(ProviderFilter);
        }

        [Route(ProviderRoute.List), HttpPost]
        public async Task<List<Provider_ProviderDTO>> List([FromBody] Provider_ProviderFilterDTO Provider_ProviderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProviderFilter ProviderFilter = ConvertFilterDTOToFilterEntity(Provider_ProviderFilterDTO);
            List<Provider> Providers = await ProviderService.List(ProviderFilter);
            return Providers.Select(c => new Provider_ProviderDTO(c)).ToList();
        }

        [Route(ProviderRoute.Get), HttpPost]
        public async Task<Provider_ProviderDTO> Get([FromBody]Provider_ProviderDTO Provider_ProviderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Provider Provider = await ProviderService.Get(Provider_ProviderDTO.Id);
            return new Provider_ProviderDTO(Provider);
        }

        [Route(ProviderRoute.Create), HttpPost]
        public async Task<ActionResult<Provider_ProviderDTO>> Create([FromBody] Provider_ProviderDTO Provider_ProviderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Provider Provider = ConvertDTOToEntity(Provider_ProviderDTO);
            Provider = await ProviderService.Create(Provider);
            Provider_ProviderDTO = new Provider_ProviderDTO(Provider);
            if (Provider.IsValidated)
                return Provider_ProviderDTO;
            else
                return BadRequest(Provider_ProviderDTO);
        }

        [Route(ProviderRoute.Update), HttpPost]
        public async Task<ActionResult<Provider_ProviderDTO>> Update([FromBody] Provider_ProviderDTO Provider_ProviderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Provider Provider = ConvertDTOToEntity(Provider_ProviderDTO);
            Provider = await ProviderService.Update(Provider);
            Provider_ProviderDTO = new Provider_ProviderDTO(Provider);
            if (Provider.IsValidated)
                return Provider_ProviderDTO;
            else
                return BadRequest(Provider_ProviderDTO);
        }

        [Route(ProviderRoute.Delete), HttpPost]
        public async Task<ActionResult<Provider_ProviderDTO>> Delete([FromBody] Provider_ProviderDTO Provider_ProviderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Provider Provider = ConvertDTOToEntity(Provider_ProviderDTO);
            Provider = await ProviderService.Delete(Provider);
            Provider_ProviderDTO = new Provider_ProviderDTO(Provider);
            if (Provider.IsValidated)
                return Provider_ProviderDTO;
            else
                return BadRequest(Provider_ProviderDTO);
        }

        [Route(ProviderRoute.Import), HttpPost]
        public async Task<List<Provider_ProviderDTO>> Import([FromBody] Provider_ProviderFilterDTO Provider_ProviderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProviderFilter ProviderFilter = ConvertFilterDTOToFilterEntity(Provider_ProviderFilterDTO);
            List<Provider> Providers = await ProviderService.List(ProviderFilter);
            return Providers.Select(c => new Provider_ProviderDTO(c)).ToList();
        }
        
        [Route(ProviderRoute.Export), HttpPost]
        public async Task<List<Provider_ProviderDTO>> Export([FromBody] Provider_ProviderFilterDTO Provider_ProviderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProviderFilter ProviderFilter = ConvertFilterDTOToFilterEntity(Provider_ProviderFilterDTO);
            List<Provider> Providers = await ProviderService.List(ProviderFilter);
            return Providers.Select(c => new Provider_ProviderDTO(c)).ToList();
        }

        public Provider ConvertDTOToEntity(Provider_ProviderDTO Provider_ProviderDTO)
        {
            Provider Provider = new Provider();
            Provider.Id = Provider_ProviderDTO.Id;
            Provider.Name = Provider_ProviderDTO.Name;
            Provider.ProviderTypeId = Provider_ProviderDTO.ProviderTypeId;
            Provider.Value = Provider_ProviderDTO.Value;
            Provider.IsDefault = Provider_ProviderDTO.IsDefault;
            Provider.ProviderType = Provider_ProviderDTO.ProviderType == null ? null : new ProviderType
            {
                Id = Provider_ProviderDTO.ProviderType.Id,
                Code = Provider_ProviderDTO.ProviderType.Code,
                Name = Provider_ProviderDTO.ProviderType.Name,
            };
            Provider.ApplicationUsers = Provider_ProviderDTO.ApplicationUsers?
                .Select(x => new ApplicationUser
                {
                    Id = x.Id,
                    Username = x.Username,
                    Password = x.Password,
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    Phone = x.Phone,
                    UserStatusId = x.UserStatusId,
                    RetryTime = x.RetryTime,
                    ProviderId = x.ProviderId,
                }).ToList();
            
            return Provider;
        }

        public ProviderFilter ConvertFilterDTOToFilterEntity(Provider_ProviderFilterDTO Provider_ProviderFilterDTO)
        {
            ProviderFilter ProviderFilter = new ProviderFilter();
            ProviderFilter.Selects = ProviderSelect.ALL;
            ProviderFilter.Skip = Provider_ProviderFilterDTO.Skip;
            ProviderFilter.Take = Provider_ProviderFilterDTO.Take;
            ProviderFilter.OrderBy = Provider_ProviderFilterDTO.OrderBy;
            ProviderFilter.OrderType = Provider_ProviderFilterDTO.OrderType;

            ProviderFilter.Id = Provider_ProviderFilterDTO.Id;
            ProviderFilter.Name = Provider_ProviderFilterDTO.Name;
            ProviderFilter.ProviderTypeId = Provider_ProviderFilterDTO.ProviderTypeId;
            ProviderFilter.Value = Provider_ProviderFilterDTO.Value;
            return ProviderFilter;
        }

        
        [Route(ProviderRoute.SingleListProviderType), HttpPost]
        public async Task<List<Provider_ProviderTypeDTO>> SingleListProviderType([FromBody] Provider_ProviderTypeFilterDTO Provider_ProviderTypeFilterDTO)
        {
            ProviderTypeFilter ProviderTypeFilter = new ProviderTypeFilter();
            ProviderTypeFilter.Skip = 0;
            ProviderTypeFilter.Take = 20;
            ProviderTypeFilter.OrderBy = ProviderTypeOrder.Id;
            ProviderTypeFilter.OrderType = OrderType.ASC;
            ProviderTypeFilter.Selects = ProviderTypeSelect.ALL;
            ProviderTypeFilter.Id = Provider_ProviderTypeFilterDTO.Id;
            ProviderTypeFilter.Code = Provider_ProviderTypeFilterDTO.Code;
            ProviderTypeFilter.Name = Provider_ProviderTypeFilterDTO.Name;

            List<ProviderType> ProviderTypes = await ProviderTypeService.List(ProviderTypeFilter);
            List<Provider_ProviderTypeDTO> Provider_ProviderTypeDTOs = ProviderTypes
                .Select(x => new Provider_ProviderTypeDTO(x)).ToList();
            return Provider_ProviderTypeDTOs;
        }
        

    }
}

