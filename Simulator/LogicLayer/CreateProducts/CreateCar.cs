using LogicLayer.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.CreateProducts
{
    public class CreateCar : CreateProduct
    {
        public Product Create()
        {
            return new Car();
        }
    }
}
