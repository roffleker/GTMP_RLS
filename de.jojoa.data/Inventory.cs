using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealifeGM.de.jojoa.data
{
    class Inventory
    {
        public int id = 0;
        public Dictionary<string, int> items = new Dictionary<string, int>();
        public Inventory(int ids)
        {
            id = ids;
            methods.getMethods.linv.Add(this);
        }
        public void addItem(string item, int amount)
        {
            if (items.ContainsKey(item))
            {
                int a = amount + items[item];
                items[item] = a;
            }
            else
            {
                items.Add(item, amount);
            }
            mysql.MySQL_InventoryData.Update(this);
        }

        public Boolean remItem(string item, int amount)
        {
            if (items.ContainsKey(item))
            {
                int a = items[item] - amount;
                if (a > 0)
                {
                    items[item] = a;
                } else
                {
                    items.Remove(item);
                }
                mysql.MySQL_InventoryData.Update(this);
                return true;
            } else
            {
                return false;
            }
        
        }

        public void showInventory()
        {
            //menu
        }

    }
}
