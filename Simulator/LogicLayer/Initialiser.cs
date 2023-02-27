using LogicLayer.CreateProducts;
using LogicLayer.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Initialiser
    {
        public static void initProducts(FactoryProduct factory)
        {
            factory.setDico("car", new CreateCar());
            factory.setDico("scooter", new CreateScooter());
            factory.setDico("bike", new CreateBike());
            factory.setDico("telec", new CreateTelec());
        }

        public static void initClients(ClientService client)
        {
            client.InitNeeds("car",0);
            client.InitNeeds("scooter", 0);
            client.InitNeeds("bike", 0);
            client.InitNeeds("telec", 0);

            client.InitProb("car", client.ProbaToClients(10));
            client.InitProb("bike", client.ProbaToClients(20));
            client.InitProb("scooter", client.ProbaToClients(14));
            client.InitProb("telec", client.ProbaToClients(26));
        }


    }
}
