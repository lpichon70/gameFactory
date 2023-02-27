using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLogicLayer
{
    public class FakeObserveur : IObserver
    {
        private int materials = 0;
        public int Materials { get { return materials; } }

        public void EmployeesChange(int free, int total)
        {
        }

        public void MaterialChange(int material)
        {
            this.materials = material;
        }

        public void MoneyChange(int money)
        {
        }

        public void StockChange(int stock)
        {
        }
    }
}
