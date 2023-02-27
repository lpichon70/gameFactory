using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLogicLayer
{
    public class TestObervateur
    {
        [Fact]
        public void TestMaterials()
        {
            Enterprise e = new Enterprise();
            FakeObserveur obs = new FakeObserveur();
            e.Register(obs);
            int matos = e.Materials;
            e.BuyMaterials();
            Assert.True(obs.Materials > matos);
            Assert.Equal(obs.Materials, e.Materials);
        }

    }
}
