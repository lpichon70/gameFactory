﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    /// <summary>
    /// Part of company, who deal with clients needs
    /// </summary>
    public class ClientService : Subject
    {
        private Random r;
        private Dictionary<string, int> needs;
        private Dictionary<string , int> proba;
        public Dictionary<string, int> getProba()
        {
            return proba;
        }

        public ClientService()
        {
            needs = new Dictionary<string, int>();
            proba = new Dictionary<string , int>();
            r = new Random();
            Initialiser.initClients(this);
        }
        public int ProbaToClients(int proba)
        {
            return (int)(r.NextDouble() * proba);
        }
        
        public void UpdateClients()
        {
            // the values are the probability new clients want a type...
            foreach(string key in needs.Keys)
            {
                needs[key] += proba[key];
                this.NotifyClientNeed(key, needs[key]);
            }
        }
        /// <summary>
        /// Get clients needs
        /// </summary>
        /// <param name="type">type of product</param>
        /// <returns>number of potential clients</returns>
        /// <exception cref="ProductUnknown">If product is not known</exception>
        public int GetAskFor(string type)
        {            
            if (!needs.ContainsKey(type))
                throw new ProductUnknown();

            return needs[type];
        }

        /// <summary>
        /// Tells if a client want to buy the product
        /// </summary>
        /// <param name="type">kind of product</param>
        /// <returns>true if one client want to buy (and can buy)</returns>
        /// <exception cref="ProductUnknown">If type unknown</exception>
        public bool WantToBuy(string type)
        {
            if (!needs.ContainsKey(type))
                throw new ProductUnknown();
            return (r.NextDouble() * needs[type])*10 > 1;
            this.NotifyClientNeed(type, needs[type]);
        }

        /// <summary>
        /// A product is bought, so a client do not want to buy anymore
        /// </summary>
        /// <param name="type"></param>
        public void Buy(string type)
        {
            needs[type] -= 10;
            if (needs[type] < 0) needs[type] = 0;
            this.NotifyClientNeed(type,needs[type]);
        }

        public void InitProb(string type , int random)
        {
            proba[type] = random;
        }

        public void InitNeeds(string type, int need)
        {
            needs[type] = need;
        }
    }
}
