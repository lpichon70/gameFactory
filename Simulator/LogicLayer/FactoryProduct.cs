using LogicLayer.CreateProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Products
{
    public class FactoryProduct
    {

        private Dictionary<string,CreateProduct> constructor = new Dictionary<string,CreateProduct>();
        public void setDico(string type , CreateProduct contructor)
        {
            this.constructor.Add(type, contructor);
        }

        public string[] Type
        {
            get { return constructor.Keys.ToArray(); }
        }

        public Product Create(string type)
        {
            if (!constructor.ContainsKey(type))
                throw new Exception("Ce type n'est pas valide");
            return constructor[type].Create();

        }

    }
}
