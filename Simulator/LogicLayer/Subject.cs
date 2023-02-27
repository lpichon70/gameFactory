using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Subject
    {
        private List<IObserver> observers = new List<IObserver>();

        public void Register(IObserver obs)
        {
            observers.Add(obs);
        }

        public void UnRegister(IObserver obs)
        {
            observers.Remove(obs);
        }

        protected void NotifyMoneyChange(int money)
        {
            foreach(IObserver o in observers)
            {
                o.MoneyChange(money);
            }
        }

        protected void NotifyStockChange(int stock)
        {
            foreach (IObserver o in observers)
            {
                o.StockChange(stock);
            }
        }

        protected void NotifyMaterialChange(int material)
        {
            foreach (IObserver o in observers)
            {
                o.MaterialChange(material);
            }
        }

        protected void NotifyEmployeesChange(int free, int total)
        {
            foreach (IObserver o in observers)
            {
                o.EmployeesChange(free,total);
            }
        }

        protected void NotifyClientNeed(string type, int nedd)
        {
            foreach (IObserver o in observers)
            {
                o.ClientNeedsChange(type, nedd);
            }
        }
    }
}
