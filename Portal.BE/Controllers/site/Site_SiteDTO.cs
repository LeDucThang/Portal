using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.site
{
    public class Site_SiteDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public long Status { get; set; }

        public Site_SiteDTO() {}
        public Site_SiteDTO(Site Site)
        {
            this.Id = Site.Id;
            this.Name = Site.Name;
            this.URL = Site.URL;
            this.Status = Site.Status;
        }
    }

    public class Site_SiteFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter URL { get; set; }
        public LongFilter Status { get; set; }
        public SiteOrder OrderBy { get; set; }
    }
}
