﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Database
{
    public partial class DieElementInСonfiguration : IConfigElement
    {
        public long Id { get; set; }
        public long IdDie { get; set; }
        public long IdElement { get; set; }
        public long Number { get; set; }

        [NotMapped]
        public string Name { get { return IdElementNavigation.Name; } set { IdElementNavigation.Name = value; } }

        public virtual Die IdDieNavigation { get; set; }
        public virtual DieElement IdElementNavigation { get; set; }
    }
}
