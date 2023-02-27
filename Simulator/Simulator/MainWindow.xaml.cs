using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IObserver
    {
        private LogicLayer.Enterprise enterprise;
        private Timer timerSecond;
        private Timer timerWeek;
        public MainWindow()
        {
            InitializeComponent();
            enterprise = new LogicLayer.Enterprise();
            enterprise.Register(this);
              
            DataContext = enterprise;
            timerSecond = new Timer(TimerSecondTick);
            timerSecond.Change(0, LogicLayer.Constants.TIME_SLICE); 
            timerWeek = new Timer(TimerWeekTick);
            timerWeek.Change(0, LogicLayer.Constants.WEEK_TIME);


            InitPanelBuild();
            InitProd();
            InitPanelStock();
            InitPanelClient();

        }

        /// <summary>
        /// Se produit toutes les secondes
        /// </summary>
        /// <param name="data"></param>
        private void TimerSecondTick(object? data)
        {
            Dispatcher.Invoke(() =>
            {
                // every second, to update screen
                UpdateScreen();
            });
            
        }

        /// <summary>
        /// Se produit toutes les semaines 
        /// </summary>
        /// <param name="data"></param>
        private void TimerWeekTick(object? data)
        {
            Dispatcher.Invoke(() =>
            {
                // nothing to do every week...
            });
        }

        /// <summary>
        /// Met din au programme
        /// </summary>
        private void EndOfSimulation()
        {
            MessageBox.Show("END OF SIMULATION");
            Close();
        }


        /// <summary>
        /// Actualise l'écrant (IHM)
        /// </summary>
        private void UpdateScreen()
        {
            enterprise.UpdateProductions();
            enterprise.UpdateBuying();

            string[] temp = enterprise.NamesOfProducts;

            
            foreach (string type in temp)
            {
                UpdateProd(type);
                ProductionDone(type);
                UpdatePanelClient(type);
            }
        }

        /// <summary>
        /// Achète une quantitée de matériel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyMaterials(object sender, RoutedEventArgs e)
        {
            try
            {
                enterprise.BuyMaterials();
                UpdateScreen();
            }
            catch(LogicLayer.NotEnoughMoney)
            {
                MessageBox.Show("Not enough money to buy materials !");
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        /// <summary>
        /// Ajoute un employé à l'entreprise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hire(object sender, RoutedEventArgs e)
        {
            try
            {
                enterprise.Hire();
                UpdateScreen();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        /// <summary>
        /// Enlève un employé à l'entreprise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dismiss(object sender, RoutedEventArgs e)
        {
            try
            {
                enterprise.Dismiss();
                UpdateScreen();
            }
            catch(LogicLayer.NoEmployee)
            {
                MessageBox.Show("There is no employee to dismiss");
            }
            catch(LogicLayer.NotEnoughMoney)
            {
                MessageBox.Show("There is not enough money to puy dismiss bonus");
            }
            catch(LogicLayer.EmployeeWorking)
            {
                MessageBox.Show("You can't dismiss no : employees working");
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        /// <summary>
        /// Crée un produit -> affiche un message si vous n'avez pas assez d'employés, si le produit n'existe pas ou si vous manquez de 
        /// matériels.
        /// </summary>
        /// <param name="s">nom du produit</param>
        private void BuildProduct(string s)
        {
            try
            {
                enterprise.MakeProduct(s);
                UpdateScreen();
            }
            catch (LogicLayer.ProductUnknown)
            {
                MessageBox.Show("I don't know how to make " + s);
            }
            catch (LogicLayer.NotEnoughMaterials)
            {
                MessageBox.Show("You do not have suffisent materials to build a "+s);
            }
            catch (LogicLayer.NoEmployee)
            {
                MessageBox.Show("You do not have enough employees to build a "+s);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }
        
        /// <summary>
        /// Change la valeur afficher à l'écran lorsque l'utilisateur utilise de l'argent suite à une notification (observateur)
        /// </summary>
        /// <param name="m"></param>
        public void MoneyChange(int m)
        {
            Dispatcher.Invoke(() =>
            {
                // code pour MAJ l'affichage
                money.Content = m.ToString("C");
            });

            
        }

        /// <summary>
        /// Change la valeur afficher à l'écran lorsque le stock change suite à une notification (observateur)
        /// </summary>
        /// <param name="stock"></param>
        public void StockChange(int stock)
        {
            Dispatcher.Invoke(() =>
            {
                // code pour MAJ l'affichage
                totalStock.Content = stock.ToString() + " %";
            });
            
        }

        /// <summary>
        /// Change la valeur afficher à l'écran lorsque l'utilisateur achète ou utilise des matériaux suite à une notification (observateur)
        /// </summary>
        /// <param name="material"></param>
        public void MaterialChange(int material)
        {
            Dispatcher.Invoke(() =>
            {
                // code pour MAJ l'affichage
                materials.Content = material.ToString();

            });
           
        }

        /// <summary>
        /// Change la valeur afficher à l'écran lorsque le nombre d'employés change suite à une notification (observateur)
        /// </summary>
        /// <param name="free"></param>
        /// <param name="total"></param>
        public void EmployeesChange(int free, int total)
        {
            Dispatcher.Invoke(() =>
            {
                // code pour MAJ l'affichage
                employees.Content = free.ToString() + " / " + total.ToString();
            });
            
        }

        /// <summary>
        /// Change la valeur afficher à l'écran lorsque le mois se termine et que les clients changant suite à une notification (observateur)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="need"></param>
        public void ClientNeedsChange(string type, int need)
        {
            
            Dispatcher.Invoke(() =>
            {
               try
                {
                    enterprise.UpdateClients();
                }
                    catch (LogicLayer.NotEnoughMoney)
                {
                    timerSecond.Dispose();

                    MessageBox.Show("Not enough money to pay employees !");
                    EndOfSimulation();
                }
                UpdatePanelClient(type);
            });
            
        }


        /// <summary>
        /// Permet d'initialiser la partie qui va contenir les boutons
        /// </summary>
        private void InitPanelBuild()
        {
            string[] temp = enterprise.NamesOfProducts;

            foreach(string type in temp)
            {
                // create a button, with a static style
                Button button = new Button();
                button.Style = Application.Current.TryFindResource("resBtn") as Style;
                // when the button is clicked, we call BuildProduct with the good type
                button.Click += (sender, args) => { BuildProduct(type); };
                // create the stack panel inside the button
                var panel = new StackPanel();
                button.Content = panel;
                // create an image with resources, and file with same name than product, and add to the panel
                Image image = new Image();
                string path = string.Format("pack://application:,,,/Simulator;component/Images/{0}.png", type);
                BitmapImage bmp = new BitmapImage(new Uri(path));
                image.Source = bmp;
                panel.Children.Add(image);
                // create a label, with the good style and add to the panel
                Label label = new Label();
                label.Content = "Build a " + type;
                label.Style = Application.Current.TryFindResource("legend") as Style;
                panel.Children.Add(label);
                // add the button to the parent panel
                panelBuild.Children.Add(button);
            }
        }

        /// <summary>
        /// Initialise le panneau IHM de production de produits
        /// </summary>
        private void InitProd()
        {
            string[] temps = enterprise.NamesOfProducts;

            foreach (string type in temps)
            {

                Border border = new Border();
                
                var panel = new StackPanel();
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness =  new Thickness(1);
                border.Margin = new Thickness(2);
                border.Child = panel;

                // create an image with resources, and file with same name than product, and add to the panel
                Image image = new Image();
                image.Width = 40;
                string path = string.Format("pack://application:,,,/Simulator;component/Images/{0}.png", type);
                BitmapImage bmp = new BitmapImage(new Uri(path));
                image.Source = bmp;
                panel.Children.Add(image);


                // create a label, with the good style and add to the panel
                Label label = new Label();
                label.Name = type+"sProd";
                label.Content = "0";
                label.Style = Application.Current.TryFindResource("legend") as Style;
                panel.Children.Add(label);
                panelProd.Children.Add(border);
            }
        }

        /// <summary>
        /// Initialise le panneau des stock de la partie IHM
        /// </summary>
        private void InitPanelStock()
        {
            string[] temps = enterprise.NamesOfProducts;

            foreach (string type in temps)
            {

                Border border = new Border();

                var panel = new StackPanel();
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(1);
                border.Margin = new Thickness(2);
                border.Child = panel;


                // create an image with resources, and file with same name than product, and add to the panel
                Image image = new Image();
                image.Width = 40;
                string path = string.Format("pack://application:,,,/Simulator;component/Images/{0}.png", type);
                BitmapImage bmp = new BitmapImage(new Uri(path));
                image.Source = bmp;
                panel.Children.Add(image);


                // create a label, with the good style and add to the panel
                Label label = new Label();
                label.Name = type+"sStock";
                label.Content = "0";
                label.Style = Application.Current.TryFindResource("legend") as Style;
                panel.Children.Add(label);
                panelStock.Children.Add(border);
            }
        }

        /// <summary>
        /// Initialise la partie client de l'IHM
        /// </summary>
        private void InitPanelClient()
        {
            string[] temps = enterprise.NamesOfProducts;

            foreach (string type in temps)
            {

                Border border = new Border();

                var panel = new StackPanel();
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(1);
                border.Margin = new Thickness(2);
                border.Child = panel;


                // create an image with resources, and file with same name than product, and add to the panel
                Image image = new Image();
                image.Width = 40;
                string path = string.Format("pack://application:,,,/Simulator;component/Images/{0}.png", type);
                BitmapImage bmp = new BitmapImage(new Uri(path));
                image.Source = bmp;
                panel.Children.Add(image);


                // create a label, with the good style and add to the panel
                Label label = new Label();
                label.Name = type + "sClient";
                label.Content = "0";
                label.Style = Application.Current.TryFindResource("legend") as Style;
                panel.Children.Add(label);
                panelClient.Children.Add(border);
            }
        }

        /// <summary>
        /// Met à jour le stockage (IHM)
        /// </summary>
        /// <param name="p"></param>
        private void ProductionDone(string p)
        {
            string name = p + "sStock";
            Dispatcher.Invoke(() =>
            {
                var test = UIChildFinder.FindChild(panelStock, name, typeof(Label));

                if (test is Label label)
                {
                    label.Content = enterprise.GetStock(p);
                }
            });
        }


        /// <summary>
        /// Met à jour la production (IHM)
        /// </summary>
        /// <param name="p"></param>
        private void UpdateProd(string p)
        {
            string name = p + "sProd";
            Dispatcher.Invoke(() =>
            {
                var test = UIChildFinder.FindChild(panelProd, name, typeof(Label));

                if (test is Label label)
                {
                    label.Content = enterprise.GetProduction(p).ToString();
                }
            });
        }

        /// <summary>
        /// Met à jour la partie client (IHM)
        /// </summary>
        /// <param name="p"></param>
        private void UpdatePanelClient(string p)
        {
            string name = p+"sClient";
            Dispatcher.Invoke(() =>
            {
                var test = UIChildFinder.FindChild(panelClient, name, typeof(Label));

                if (test is Label label)
                {
                    label.Content = enterprise.GetAskClients(p);
                }
            });
        }

    }
}