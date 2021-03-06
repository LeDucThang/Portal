﻿using System;
using System.Collections.Generic;

namespace Auth.Models
{
    public partial class Site
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public long Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
