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



namespace Portal.Controllers.provider
{
    public class ProviderRoute : Root
    {
        public const string Master = Module + "/provider/provider-master";
        public const string Detail = Module + "/provider/provider-detail";
        private const string Default = Rpc + Module + "/provider";
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
            { nameof(Provider.Id), FieldType.ID },
            { nameof(Provider.Name), FieldType.STRING },
            { nameof(Provider.GoogleRedirectUri), FieldType.STRING },
            { nameof(Provider.ADIP), FieldType.STRING },
            { nameof(Provider.ADUsername), FieldType.STRING },
            { nameof(Provider.ADPassword), FieldType.STRING },
            { nameof(Provider.GoogleClientId), FieldType.ID },
            { nameof(Provider.GoogleClientSecret), FieldType.STRING },
            { nameof(Provider.MicrosoftClientId), FieldType.ID },
            { nameof(Provider.MicrosoftClientSecret), FieldType.STRING },
            { nameof(Provider.MicrosoftRedirectUri), FieldType.STRING },
        };
    }

    public class ProviderController : ApiController
    {
        
        private IProviderService ProviderService;

        public ProviderController(
            
            IProviderService ProviderService
        )
        {
            
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
            Provider.GoogleRedirectUri = Provider_ProviderDTO.GoogleRedirectUri;
            Provider.ADIP = Provider_ProviderDTO.ADIP;
            Provider.ADUsername = Provider_ProviderDTO.ADUsername;
            Provider.ADPassword = Provider_ProviderDTO.ADPassword;
            Provider.GoogleClientId = Provider_ProviderDTO.GoogleClientId;
            Provider.GoogleClientSecret = Provider_ProviderDTO.GoogleClientSecret;
            Provider.MicrosoftClientId = Provider_ProviderDTO.MicrosoftClientId;
            Provider.MicrosoftClientSecret = Provider_ProviderDTO.MicrosoftClientSecret;
            Provider.MicrosoftRedirectUri = Provider_ProviderDTO.MicrosoftRedirectUri;

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
            ProviderFilter.GoogleRedirectUri = Provider_ProviderFilterDTO.GoogleRedirectUri;
            ProviderFilter.ADIP = Provider_ProviderFilterDTO.ADIP;
            ProviderFilter.ADUsername = Provider_ProviderFilterDTO.ADUsername;
            ProviderFilter.ADPassword = Provider_ProviderFilterDTO.ADPassword;
            ProviderFilter.GoogleClientId = Provider_ProviderFilterDTO.GoogleClientId;
            ProviderFilter.GoogleClientSecret = Provider_ProviderFilterDTO.GoogleClientSecret;
            ProviderFilter.MicrosoftClientId = Provider_ProviderFilterDTO.MicrosoftClientId;
            ProviderFilter.MicrosoftClientSecret = Provider_ProviderFilterDTO.MicrosoftClientSecret;
            ProviderFilter.MicrosoftRedirectUri = Provider_ProviderFilterDTO.MicrosoftRedirectUri;
            return ProviderFilter;
        }

        

    }
}

