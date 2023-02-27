using LogicLayer.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLogicLayer
{
    public class TestFabriqueAbstraite
    {
        [Fact]
        public void TestCreatePoduct()
        {
            //Create a factory
            FactoryProduct usine = new FactoryProduct();

            //Initialisation of the dictionary
            Initialiser.initProducts(usine);

            //Create a new car
            Product car = usine.Create("car");

            Assert.Equal("car",car.Name);
        }

        
    }
}
