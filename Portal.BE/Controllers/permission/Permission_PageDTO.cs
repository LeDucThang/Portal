using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission
{
    public class Permission_PageDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Path { get; set; }
        
        public long? ParentId { get; set; }
        

        public Permission_PageDTO() {}
        public Permission_PageDTO(Page Page)
        {
            
            this.Id = Page.Id;
            
            this.Name = Page.Name;
            
            this.Path = Page.Path;
            
            this.ParentId = Page.ParentId;
            
        }
    }

    public class Permission_PageFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Path { get; set; }
        
        public IdFilter ParentId { get; set; }
        
        public PageOrder OrderBy { get; set; }
    }
}