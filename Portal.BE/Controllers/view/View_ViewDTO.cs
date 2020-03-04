using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.view
{
    public class View_ViewDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDeleted { get; set; }
        public List<View_PageDTO> Pages { get; set; }
        public List<View_PermissionFieldDTO> PermissionFields { get; set; }

        public View_ViewDTO() {}
        public View_ViewDTO(View View)
        {
            this.Id = View.Id;
            this.Name = View.Name;
            this.Path = View.Path;
            this.IsDeleted = View.IsDeleted;
            this.Pages = View.Pages?.Select(x => new View_PageDTO(x)).ToList();
            this.PermissionFields = View.PermissionFields?.Select(x => new View_PermissionFieldDTO(x)).ToList();
        }
    }

    public class View_ViewFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Path { get; set; }
        public ViewOrder OrderBy { get; set; }
    }
}
