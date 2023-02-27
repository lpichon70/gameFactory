using LogicLayer.Products;

namespace LogicLayer
{
    /// <summary>
    /// Enterprise simulation
    /// </summary>
    public class Enterprise : Subject , IObserver
    {
        #region associations
        private Workshop workshop;
        private Stock stock;
        private ClientService clients;
        private FactoryProduct factory;
        private Timer timer;
        private LogicLayer.ClientService clientService;

        #endregion

        #region Properties 

        /// <summary>
        /// Fait quelque chose à la fin du mois -> Timer (paye les employés, met à jour les clients...)
        /// </summary>
        /// <param name="state"></param>
        private void EndOfMonth(object? state)
        {
            PayEmployees();
            UpdateClients();
            this.NotifyClientNeed("car",GetAskClients("car"));
            this.NotifyClientNeed("scooter", GetAskClients("scooter"));
            this.NotifyClientNeed("bike", GetAskClients("bike"));
            this.NotifyClientNeed("telec", GetAskClients("telec"));

        }


        /// <summary>
        /// Gets the amount of money that enterprise disposes
        /// </summary>
        public int Money { get => money; }
        private int money;

        private int materials;
        /// <summary>
        /// Gets the amount of materials that enterprise disposes
        /// </summary>
        public int Materials { get => materials; }

        private int employees;
        /// <summary>
        /// Gets the number of employees
        /// </summary>
        public int Employees { get => employees; }

        /// <summary>
        /// Gets the number of free employees (they can work)
        /// </summary>
        public int FreeEmployees
        {
            get => employees - EmployeesWorkshop;
        }
       
        /// <summary>
        /// Gets the number of employees working in the workshop
        /// </summary>
        public int EmployeesWorkshop { get => workshop.NbEmployees; } 

        /// <summary>
        /// Gets the total amount of stock
        /// </summary>
        public int TotalStock { get => stock.TotalStock; }


        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the enterprise
        /// </summary>
        public Enterprise()
        {
            money = 300000;
            employees = 4;
            materials = 100;  
            workshop = new Workshop();
            stock = new Stock();
            clients = new ClientService();
            this.factory = new FactoryProduct();

            Initialiser.initProducts(factory);
            Initialiser.initClients(clients);

            timer = new Timer(EndOfMonth);
            timer.Change(0, Constants.MONTH_TIME);

            this.NotifyEmployeesChange(FreeEmployees, employees);
            this.NotifyMaterialChange(materials);
            this.NotifyMoneyChange(money);
            this.NotifyStockChange(TotalStock);

            clientService = new LogicLayer.ClientService();
            clientService.Register(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Buy some materials
        /// </summary>
        /// <exception cref="NotEnoughMoney">If insufisant funds</exception>
        public void BuyMaterials()
        {
            int cost = Constants.MATERIALS * Constants.COST_MATERIALS;
            if (money < cost)
                throw new NotEnoughMoney();
            money -= cost;
            this.NotifyMoneyChange(money);
            materials += Constants.MATERIALS;
            this.NotifyMaterialChange(materials);
        }

        /// <summary>
        /// Hire a new emloyee
        /// </summary>        
        public void Hire()
        {
            ++employees;
            this.NotifyEmployeesChange(FreeEmployees, employees);
        }

        /// <summary>
        /// DIsmiss an employee
        /// </summary>
        /// <exception cref="NoEmployee">If no employee to dismiss</exception>
        /// <exception cref="NotEnoughMoney">If not enough money to pay the bonus</exception>
        /// <exception cref="EmployeeWorking">If all employees worked, no dismiss is possible</exception>
        public void Dismiss()
        {
            if (employees < 1) throw new NoEmployee();
            int cost = Constants.BONUS;
            if (money < cost)
                throw new NotEnoughMoney();
            if (FreeEmployees < 1)
                throw new EmployeeWorking();
            money -= cost;
            employees--;
            this.NotifyMoneyChange(money);
            this.NotifyEmployeesChange(FreeEmployees, employees);
        }

        /// <summary>
        /// Start a product production
        /// </summary>
        /// <param name="type">a string identifying kind of product</param>
        /// <exception cref="ProductUnknown">the type is unknown</exception>
        /// <exception cref="NotEnoughMaterials">Not enough materials to build</exception>
        /// <exception cref="NoEmployee">Not enough employee to build</exception>
        public void MakeProduct(string type)
        {
            
            Product p;
            p = this.factory.Create(type);

            // test if the product can be build
            if (materials < p.MaterialsNeeded)
                throw new NotEnoughMaterials();
            if (employees - EmployeesWorkshop < p.EmployeesNeeded)
                throw new NoEmployee();

            materials -= p.MaterialsNeeded; // consume materials
            // start the building...
            workshop.StartProduction(p);
            this.NotifyMaterialChange(materials);
            this.NotifyEmployeesChange(FreeEmployees, employees);
        }

        /// <summary>
        /// Update the productions & the stock
        /// </summary>
        /// <exception cref="UnableToStock">If stock is full</exception>
        public void UpdateProductions()
        {
            // update informations about productions
            var list = workshop.ProductsDone(); 
            // add finish products in stock
            foreach(var product in list)
            {
                stock.Add(product);
                workshop.Remove(product);
            }
           
            this.NotifyMaterialChange(materials);
            this.NotifyStockChange(TotalStock);
        }

        /// <summary>
        /// Get the numbers of products of a type workshop build
        /// </summary>
        /// <param name="v">kind of product</param>
        /// <returns>number of products building</returns>        
        public int GetProduction(string v)
        {
            return workshop.InProduction(v);
            
        }

        /// <summary>
        /// Gets the number of products stocked
        /// </summary>
        /// <param name="v">type of product</param>
        /// <returns>number stocked</returns>
        public int GetStock(string v)
        {
            this.NotifyEmployeesChange(FreeEmployees, employees);
            this.NotifyMaterialChange(materials);
            this.NotifyMoneyChange(money);
            this.NotifyStockChange(TotalStock);
            return stock.GetNbOfType(v);
        }

        /// <summary>
        /// Pay all the employees
        /// </summary>
        /// <exception cref="NotEnoughMoney">if money is not enough !</exception>
        public void PayEmployees()
        {
            int cost = employees * Constants.SALARY;
            if (cost > money)
                throw new NotEnoughMoney();
            money -= cost;
            this.NotifyMoneyChange(money);
        }

        /// <summary>
        /// Update the buying status
        /// </summary>
        public void UpdateBuying()
        {            
            if(clients.WantToBuy("bike"))
            {
                TrySell("bike");
            }
            else if(clients.WantToBuy("scooter"))
            {
                TrySell("scooter");
                
            }
            else if(clients.WantToBuy("car"))
            {
                TrySell("car");
             
            }
            else if (clients.WantToBuy("telec"))
            {
                TrySell("telec");

            }
        }

        /// <summary>
        /// Permet de vendre un produit qui se trouve dans Stock
        /// </summary>
        /// <param name="type"></param>
        private void TrySell(string type)
        {
            Product? p = stock.Find(type);
            if(p!=null)
            {
                stock.Remove(p);
                money += p.Price;
                clients.Buy(type);
            }
            this.NotifyMoneyChange(money);
        }

        /// <summary>
        /// update client needs
        /// </summary>
        public void UpdateClients()
        {            
            clients.UpdateClients();
        }

        /// <summary>
        /// Get clients needs
        /// </summary>
        /// <param name="type">type of product clients wanted</param>
        /// <returns>number of potential clients</returns>
        /// <exception cref="ProductUnknown">If type unknown</exception>
        public int GetAskClients(string type)
        {
            return clients.GetAskFor(type); 
        }

        public void MoneyChange(int money)
        {
            this.NotifyMoneyChange(money);
        }

        public void StockChange(int stock)
        {
            this.NotifyStockChange(TotalStock);
        }

        public void MaterialChange(int material)
        {
            this.NotifyMaterialChange(materials);
        }

        public void EmployeesChange(int free, int total)
        {
            this.NotifyEmployeesChange(FreeEmployees, employees);
        }

        public void ClientNeedsChange(string type, int need)
        {
            this.NotifyClientNeed("car", GetAskClients("car"));
            this.NotifyClientNeed("scooter", GetAskClients("scooter"));
            this.NotifyClientNeed("bike", GetAskClients("bike"));
            this.NotifyClientNeed("telec", GetAskClients("telec"));

        }
        #endregion

        /// <summary>
        /// Permet de récupérer la liste des produits qui vont être affichés dans la fenêtre IHM
        /// </summary>
        public string[] NamesOfProducts
        {
            get => factory.Type;
        }


        /// <summary>
        /// Destructor of the class
        /// </summary>
        ~Enterprise()
        {
            timer.Dispose();
        }

    }
}