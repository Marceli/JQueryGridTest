﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Entities
{
    public class Cat
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
        public virtual Person Person { get; set; }

     
    }
}
