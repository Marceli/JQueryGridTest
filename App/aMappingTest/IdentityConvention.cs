using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Core.Data
{
    public class IdGenerationConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.GeneratedBy.HiLo("1000");
        }
    }
    public class CollectionConvention : ICollectionConvention
    {
        public void Apply(ICollectionInstance instance)
        {
            instance.Cascade.SaveUpdate();
            instance.Inverse();
        }

       
    }

}
