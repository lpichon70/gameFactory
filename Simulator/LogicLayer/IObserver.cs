using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IObserver
    {
        public void MoneyChange(int money);

        public void StockChange(int stock);

        public void MaterialChange(int material);

        public void EmployeesChange(int free, int total);

        public void ClientNeedsChange(string type, int need);
    }
}
