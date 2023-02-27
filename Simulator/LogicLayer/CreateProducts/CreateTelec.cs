using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer.Products;


namespace LogicLayer.CreateProducts
{
    internal class CreateTelec : CreateProduct
    {
        public Product Create()
        {
            return new trotinette();
        }
    }
}
