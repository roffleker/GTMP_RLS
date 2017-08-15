using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealifeGM.de.jojoa.data
{
    class Property
    {
        public Vector3 pos;
        public string ID;
        public int price;
        public Inventory inv;
        public int type;
        public string typeName;
        public Account owner;
        public string street;
        public string zone;
        public Marker mark;
        public TextLabel lbl1;
        public TextLabel lbl2;
        public TextLabel lbl3;
        public TextLabel lbl4;



        public Property(Vector3 posI,int priceI,int typeI,int invid)
        {
            pos = posI;
            price = priceI;
            type = typeI;
            getTypeName();
            owner = methods.getMethods.getAccountByName("IM");
            inv = methods.getMethods.getInvById(invid);

        }

        public void show_prop()
        {
            Property prop = this;
            if (prop.mark != null)
            {
                prop.mark.delete();
                prop.lbl1.delete();
                prop.lbl2.delete();
                prop.lbl3.delete();
                prop.lbl4.delete();
            }
            int r = 255;
            int g = 0;
            string lbl3_text = "Besitzer: " + prop.owner.p.name;
            if (prop.owner == de.jojoa.methods.getMethods.getAccountByName("ImmobilienManagement"))
            {
                lbl3_text = prop.price + "$";
                g = 255;
            }
            int b = 0;

            prop.mark = API.shared.createMarker(0, prop.pos, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 150, r, g, b);
            prop.lbl1 = API.shared.createTextLabel(mysql.MySQL_PropertyData.getString(prop, "street"), prop.pos.Add(new Vector3(0, -1.5, 0)), 8, 10);
            prop.lbl2 = API.shared.createTextLabel(mysql.MySQL_PropertyData.getString(prop, "zone"), prop.pos.Add(new Vector3(0, -2.5, 0)), 8, 10);
            prop.lbl3 = API.shared.createTextLabel(lbl3_text, prop.pos.Add(new Vector3(0, -4.5, 0)), 8, 10);
            prop.lbl4 = API.shared.createTextLabel(prop.typeName, prop.pos.Add(new Vector3(0, -3.5, 0)), 8, 1);

        }

        public void getTypeName()
        {
            switch (type)
            {
                case 1:
                    typeName = "Kleine Wohnung";
                    break;
                case 2:
                    typeName = "Große Wohnung";
                    break;
                case 3:
                    typeName = "Kleines Haus";
                    break;
                case 4:
                    typeName = "Großes Haus";
                    break;
                case 5:
                    typeName = "Kleine Villa";
                    break;
                case 6:
                    typeName = "Große Villa";
                    break;
            }
        }

       

        public void Save()
        {
            mysql.MySQL_PropertyData.saveProperty(this);
        }
        
        public void Remove()
        {
            mysql.MySQL_PropertyData.RemoveProp(this);
        }
    }
}
