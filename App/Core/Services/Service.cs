using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class Service:IService
    {

        public int[] GetNumbers()
        {
            return new int[]{1,2,3};
        }

       
    }
}
