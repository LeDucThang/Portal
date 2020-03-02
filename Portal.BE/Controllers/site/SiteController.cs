using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Entities;
using Portal.Services.MSite;



namespace Portal.Controllers.site
{
    public class SiteRoute : Root
    {
        public const string FE = "/site";
        private const string Default = Base + FE;
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";

    }
    
    public class SiteController : ApiController
    {
        
        private ISiteService SiteService;

        public SiteController(
            
            ISiteService SiteService
        )
        {
            
            this.SiteService = SiteService;
        }

        [Route(SiteRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Site_SiteFilterDTO Site_SiteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SiteFilter SiteFilter = ConvertFilterDTOToFilterEntity(Site_SiteFilterDTO);
            return await SiteService.Count(SiteFilter);
        }

        [Route(SiteRoute.List), HttpPost]
        public async Task<List<Site_SiteDTO>> List([FromBody] Site_SiteFilterDTO Site_SiteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SiteFilter SiteFilter = ConvertFilterDTOToFilterEntity(Site_SiteFilterDTO);
            List<Site> Sites = await SiteService.List(SiteFilter);
            return Sites.Select(c => new Site_SiteDTO(c)).ToList();
        }

        [Route(SiteRoute.Get), HttpPost]
        public async Task<Site_SiteDTO> Get([FromBody]Site_SiteDTO Site_SiteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Site Site = await SiteService.Get(Site_SiteDTO.Id);
            return new Site_SiteDTO(Site);
        }

        [Route(SiteRoute.Create), HttpPost]
        public async Task<ActionResult<Site_SiteDTO>> Create([FromBody] Site_SiteDTO Site_SiteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Site Site = ConvertDTOToEntity(Site_SiteDTO);
            Site = await SiteService.Create(Site);
            Site_SiteDTO = new Site_SiteDTO(Site);
            if (Site.IsValidated)
                return Site_SiteDTO;
            else
                return BadRequest(Site_SiteDTO);
        }

        [Route(SiteRoute.Update), HttpPost]
        public async Task<ActionResult<Site_SiteDTO>> Update([FromBody] Site_SiteDTO Site_SiteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Site Site = ConvertDTOToEntity(Site_SiteDTO);
            Site = await SiteService.Update(Site);
            Site_SiteDTO = new Site_SiteDTO(Site);
            if (Site.IsValidated)
                return Site_SiteDTO;
            else
                return BadRequest(Site_SiteDTO);
        }

        [Route(SiteRoute.Delete), HttpPost]
        public async Task<ActionResult<Site_SiteDTO>> Delete([FromBody] Site_SiteDTO Site_SiteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Site Site = ConvertDTOToEntity(Site_SiteDTO);
            Site = await SiteService.Delete(Site);
            Site_SiteDTO = new Site_SiteDTO(Site);
            if (Site.IsValidated)
                return Site_SiteDTO;
            else
                return BadRequest(Site_SiteDTO);
        }

        [Route(SiteRoute.Import), HttpPost]
        public async Task<List<Site_SiteDTO>> Import([FromBody] Site_SiteFilterDTO Site_SiteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SiteFilter SiteFilter = ConvertFilterDTOToFilterEntity(Site_SiteFilterDTO);
            List<Site> Sites = await SiteService.List(SiteFilter);
            return Sites.Select(c => new Site_SiteDTO(c)).ToList();
        }
        
        [Route(SiteRoute.Export), HttpPost]
        public async Task<List<Site_SiteDTO>> Export([FromBody] Site_SiteFilterDTO Site_SiteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SiteFilter SiteFilter = ConvertFilterDTOToFilterEntity(Site_SiteFilterDTO);
            List<Site> Sites = await SiteService.List(SiteFilter);
            return Sites.Select(c => new Site_SiteDTO(c)).ToList();
        }

        public Site ConvertDTOToEntity(Site_SiteDTO Site_SiteDTO)
        {
            Site Site = new Site();
            Site.Id = Site_SiteDTO.Id;
            Site.Name = Site_SiteDTO.Name;
            Site.URL = Site_SiteDTO.URL;
            Site.Status = Site_SiteDTO.Status;
            
            return Site;
        }

        public SiteFilter ConvertFilterDTOToFilterEntity(Site_SiteFilterDTO Site_SiteFilterDTO)
        {
            SiteFilter SiteFilter = new SiteFilter();
            SiteFilter.Selects = SiteSelect.ALL;
            SiteFilter.Skip = Site_SiteFilterDTO.Skip;
            SiteFilter.Take = Site_SiteFilterDTO.Take;
            SiteFilter.OrderBy = Site_SiteFilterDTO.OrderBy;
            SiteFilter.OrderType = Site_SiteFilterDTO.OrderType;

            SiteFilter.Id = Site_SiteFilterDTO.Id;
            SiteFilter.Name = Site_SiteFilterDTO.Name;
            SiteFilter.URL = Site_SiteFilterDTO.URL;
            SiteFilter.Status = Site_SiteFilterDTO.Status;
            return SiteFilter;
        }

        

    }
}

