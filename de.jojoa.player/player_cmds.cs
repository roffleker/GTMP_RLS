﻿using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using RealifeGM.de.jojoa.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealifeGM.de.jojoa.player
{
    class player_cmds : Script
    {
        #region teamcmds
     

        [Command("givemoney")]
        public void cmd_givemoney(Client p, string user, int amount)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            Account re = methods.getMethods.getAccountByName(user);
            if (a.rank["givemoney"])
            {
                if (re == null)
                {
                    API.sendChatMessageToPlayer(p, "User not found");
                    return;
                }
                re.money += amount;
                API.sendChatMessageToPlayer(p, "Du hast " + re.name + " " + amount + "$ gegeben.");
                API.sendChatMessageToPlayer(re.p, "Du hast von " + p.name + " " + amount + "$ bekommen.");
            }
        }

        [Command("createShop")]
        public void cmd_cshop(Client p, string clas)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["createShop"])
            {

                int i = mysql.MySQL_POIData.addShop(p.position, clas);
                API.sendChatMessageToPlayer(p, "ID: " + i);
            }
        }

        [Command("setShopSpawn")]
        public void cmd_sshopsp(Client p, int id)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["createShop"])
            {
                if (p.isInVehicle == false)
                {
                    API.sendChatMessageToPlayer(p, "Du musst in einem Auto sein.");
                    return;
                }
                Vehicle v = p.vehicle;
                if (mysql.MySQL_POIData.setSpawn(v.position, v.rotation, id))
                {
                    API.sendChatMessageToPlayer(p, "Spawn gesetzt.");
                } else
                {
                    API.sendChatMessageToPlayer(p, "Id not found");
                }

            }
        }


        [Command("createBank")]
        public void cmd_cbank(Client p)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["createBank"])
            {
                mysql.MySQL_POIData.addBank(p.position);
                API.sendChatMessageToPlayer(p, "Bank added");
            }
        }

        [Command("spawncar")]
        public void cmd_scar(Client p, string name)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["spawncar"])
            {
                VehicleHash hash = API.vehicleNameToModel(name);
                if(hash == 0)
                {
                    API.sendChatMessageToPlayer(p, "Auto nicht gefunden");
                    return;
                }
                Vehicle v = API.createVehicle(hash, p.position, p.rotation, 29, 29);
                v.numberPlate = "TEMP";
            }
        }

        [Command("createprop")]
        public void cmd_cprop(Client p, int type, int price)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["createprop"])
            {
                Property prop = new Property(p.position, price, type, mysql.MySQL_InventoryData.createInv().id);
                prop.Save();
                prop.show_prop();
                API.sendChatMessageToPlayer(p, "Du hast erfolgreich eine Imobilie erstellt. Preis: " + prop.price + "Typ: " + prop.typeName);
            }
        }


        [Command("removeprop")]
        public void cmd_rprop(Client p, int type, int price)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["removeprop"])
            {
                Property prop = new Property(new Vector3(), 0, 1,1);
                prop = methods.getMethods.getPropertyByPos(p.position);
                if(prop == null)
                {
                    API.sendChatMessageToPlayer(p, "Du musst bei einer Imobilie sein.");
                    return;
                }
                prop.Remove();

                API.sendChatMessageToPlayer(p, "Du hast erfolgreich eine Immobilie gelöscht. Preis: " + prop.price + "Typ: " + prop.typeName);
            }
        }

        [Command("kick")]
        public void cmd_kick(Client p, string user)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["kick"])
            {
                Client pl = API.getPlayerFromName(user);
                if(pl == null)
                {
                    API.sendChatMessageToPlayer(p, "Player not found");
                    return;
                }
                API.kickPlayer(pl);
                API.sendChatMessageToPlayer(p, "Du hast " + user + "gekickt");
                loggers.logger_teamcmd.log_cmd(p, "kick");
            }
        }

        [Command("fly")]
        public void cmd_fly(Client p)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["fly"]) 
            {
                API.triggerClientEvent(p, "fly");
                loggers.logger_teamcmd.log_cmd(p, "fly");
            }
        }

        [Command("god")]
        public void cmd_god(Client p)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["god"])
            {
                if (p.invincible == true)
                {
                    API.sendChatMessageToPlayer(p, "GOD OFF");
                    p.invincible = false;
                    loggers.logger_teamcmd.log_cmd(p, "god", "off");
                }
                else
                {
                    API.sendChatMessageToPlayer(p, "GOD ON");
                    p.invincible = true;
                    loggers.logger_teamcmd.log_cmd(p, "god", "on");
                }
            }

        }

        [Command("pos")]
        public void cmd_position(Client p)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            if (a.rank["pos"])
            {
                API.sendChatMessageToPlayer(p, "Position: X: " + p.position.X + " Y: " + p.position.Y + " Z: " + p.position.Z);
                API.sendChatMessageToPlayer(p, "Rotation: X: " + p.rotation.X + " Y: " + p.rotation.Y + " Z: " + p.rotation.Z);
                de.jojoa.loggers.logger_teamcmd.log_cmd(p, "pos", "Position: X: " + p.position.X + " Y: " + p.position.Y + " Z: " + p.position.Z);
            }
        }
        #endregion teamcmds

        #region propcmds
        [Command("setPropOwner")]
        public void cmd_soprop(Client p, string new_owner)
        {
            Account no = methods.getMethods.getAccountByName(new_owner);
            Account oo = methods.getMethods.getAccountByClient(p);
            Property prop = methods.getMethods.getPropertyByPos(p.position);
            if(no == null)
            {
                API.sendChatMessageToPlayer(p, "Player not found");
                return;
            }
            if (prop != null)
            {
                if (prop.owner == methods.getMethods.getAccountByClient(p))
                {

                    if (no.p.position.DistanceTo(prop.pos) < 7)
                    {
                        prop.owner = no;
                        prop.show_prop();
                        prop.Save();
                        oo.remProp(prop);
                        oo.redraw_acc();
                        no.addProp(prop);
                        no.redraw_acc();
                        API.sendChatMessageToPlayer(p, "Du hast das Haus erfogreich überschrieben.");
                        loggers.logger_usercmd.log_cmd(p, "setPropOwner", new_owner, prop.ID, 1, 1);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(p, "Der neue Inhaber muss in der Nähe sein.");
                        loggers.logger_usercmd.log_cmd(p, "setPropOwner", new_owner, prop.ID, 1, 2);
                    }
                }
                else if (prop.owner.name == "IM")
                {
                    if (oo.jobids.Contains(01))
                    {
                        if (no.p.position.DistanceTo(prop.pos) < 7)
                        {

                            oo.money -= Convert.ToInt32(Math.Round(prop.price * 0.95));
                            prop.owner = no;
                            prop.show_prop();
                            prop.Save();
                            oo.remProp(prop);
                            oo.redraw_acc();
                            no.addProp(prop);
                            no.redraw_acc();
                            API.sendChatMessageToPlayer(p, "Du hast das Haus erfogreich überschrieben. Preis: " + prop.price);
                            loggers.logger_usercmd.log_cmd(p, "setPropOwner", new_owner, prop.ID, 2, 1);
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(p, "Der neue Inhaber muss in der Nähe sein.");
                            loggers.logger_usercmd.log_cmd(p, "setPropOwner", new_owner, prop.ID, 2, 2);
                        }
                    }
                }
            }
            else
            {
                API.sendChatMessageToPlayer(p, "Du bist bei keiner Immobilie");
                loggers.logger_usercmd.log_cmd(p, "setPropOwner", new_owner, prop.ID, 1, 3);

            }
        }

        [Command("savespawn")]
        public void cmd_savespawn(Client p)
        {
            Account a = methods.getMethods.getAccountByClient(p);
            Property prop = methods.getMethods.getPropertyByPos(p.position);
            if(prop != null)
            {
                if (prop.owner == a)
                {
                    a.spawn = prop.ID;
                    mysql.MySQL_PlayerData.set(p, "spawnID", a.spawn);
                    API.sendChatMessageToPlayer(p, "Du spawnst ab jetzt hier.");
                    loggers.logger_usercmd.log_cmd(p, "savespawn", prop.ID, 1);
                } else
                {
                    API.sendChatMessageToPlayer(p,"Diese Immobilie gehört dir nicht.");
                    loggers.logger_usercmd.log_cmd(p, "savespawn", prop.ID, 2);
                }
            } else
            {
                API.sendChatMessageToPlayer(p,"Du musst bei einer Immobilie sein.");
                loggers.logger_usercmd.log_cmd(p, "savespawn", 0, 3);
            }
        }

        #endregion propcmds

        #region economycmds
        [Command("spend")]
        public void spend_cmd(Client p, int amount)
        {
            Account payer = methods.getMethods.getAccountByClient(p);
            if(payer.money >= amount)
            {
                payer.money -= amount;
                API.sendChatMessageToPlayer(p, "Du hast " + amount + "$ an den Server gezahlt.");
            }
        }

        [Command("/pay")]
        public void pay_cmd(Client p,string user,int amount)
        {
            Account payer = methods.getMethods.getAccountByClient(p);
            Account reciver = methods.getMethods.getAccountByName(user);
            if(reciver == null)
            {
                API.sendChatMessageToPlayer(p, "Player not found");
                return;
            }
            if(reciver.p != null)
            {
                if(reciver.p.position.DistanceTo(payer.p.position)< 8)
                {
                    if(payer.money >= amount)
                    {
                        payer.money -= amount;
                        API.sendChatMessageToPlayer(p, "Du hast  " + reciver.name + " " + amount + "$ gezahlt.");
                        reciver.money += amount;
                        API.sendChatMessageToPlayer(p, reciver.name + " hat dir " + amount + "$ gezahlt.");
                        loggers.logger_usercmd.log_cmd(p, "pay", user, amount, 1);
                    } else
                    {
                        API.sendChatMessageToPlayer(p, "Du hast zu wenig Geld");
                        loggers.logger_usercmd.log_cmd(p, "pay", user, amount, 2);
                    }
                } else
                {
                    API.sendChatMessageToPlayer(p, "Der Spieler ist zu weit entfernt");
                    loggers.logger_usercmd.log_cmd(p, "pay", user, amount, 3);
                }
            }
            else
            {
                API.sendChatMessageToPlayer(p, "Der Spieler ist nicht online");
                loggers.logger_usercmd.log_cmd(p, "pay", user, amount, 4);
            }

        }

        [Command("/bank")]
        public void bnk_cmd(Client p, int kntNR)
        {
            Boolean isatBank = false;
            foreach(Vector3 pos in mysql.MySQL_POIData.getBanks())
            {
                if (pos.DistanceTo(p.position) < 5)
                {
                    isatBank = true;
                }
            }
            if(isatBank == true)
            {
                Account a = methods.getMethods.getAccountByClient(p);
                Bank_Account ba = methods.getMethods.getBankByNumber(kntNR);
                if (ba == null)
                {
                    API.sendChatMessageToPlayer(p, "Konto nicht gefunden.");
                    loggers.logger_usercmd.log_cmd(p, "bank", kntNR, 2);

                } else if (ba.owner != a)
                {
                    loggers.logger_usercmd.log_cmd(p, "bank", kntNR, 3);
                    API.sendChatMessageToPlayer(p,"Du hast keinen Zugriff auf das Konto");
                } else
                {

                    loggers.logger_usercmd.log_cmd(p, "bank", kntNR, 1);
                    API.triggerClientEvent(p, "BankMenu",ba.money,ba.number);
                }
            } else
            {
                API.sendChatMessageToPlayer(p, "Du bist bei keiner Bank");
                loggers.logger_usercmd.log_cmd(p, "bank", kntNR, 4);
            }

        }

        [Command("/getbankaccounts")]
        public void bnkacc_get_cmd(Client p)
        {
            Boolean isatBank = false;
            Account a = methods.getMethods.getAccountByClient(p);
            foreach (Vector3 pos in mysql.MySQL_POIData.getBanks())
            {
                if (pos.DistanceTo(p.position) < 5)
                {
                    isatBank = true;
                }
            }
            if (isatBank == true)
            {
                loggers.logger_usercmd.log_cmd(p, "getbankaccounts", "1");
               foreach(Bank_Account ba in methods.getMethods.getBanksByUser(a))
                {
                    API.sendChatMessageToPlayer(p, "Kontonummer: " + ba.number);
                }
            }
            else
            {
                API.sendChatMessageToPlayer(p, "Du bist bei keiner Bank");
                loggers.logger_usercmd.log_cmd(p, "getbankaccounts", "2");
            }

        }

        [Command("/createbankaccount")]
        public void bnkacc_create_cmd(Client p)
        {
            Boolean isatBank = false;
            foreach (Vector3 pos in mysql.MySQL_POIData.getBanks())
            {
                if (pos.DistanceTo(p.position) < 5)
                {
                    isatBank = true;
                }
            }
            if (isatBank == true)
            {
                Account a = methods.getMethods.getAccountByClient(p);
                Bank_Account ba = mysql.MySQL_Bank.createAccount(a);
                API.sendChatMessageToPlayer(p, "Bankkonto erfogreich erstellt. Kontonummer: " + ba.number);
                loggers.logger_usercmd.log_cmd(p, "createbankaccount", ba.number,1);
            } else
            {
                API.sendChatMessageToPlayer(p, "Du bist bei keiner Bank");
                loggers.logger_usercmd.log_cmd(p, "createbankaccount", "00000", 2);
            }
        }

        [Command("/addbankuser")]
        public void bnkacc_au_cmd(Client p, string user)
        {
            API.sendChatMessageToPlayer(p, "Dieser Command wird später noch implementiert");
        }
        [Command("/rembankuser")]
        public void bnkacc_ru_cmd(Client p, string user)
        {
            API.sendChatMessageToPlayer(p, "Dieser Command wird später noch implementiert");
        }
        #endregion economycmds

        #region vehiclecmds
        [Command("shop")]
        public void cmd_vehshop(Client p)
        {
            Vector3 shop = null;
            foreach (Vector3 pos in mysql.MySQL_POIData.getShops())
            {
                if (pos.DistanceTo(p.position) < 5)
                {
                    shop = pos;
                }
            }
            if(shop == null)
            {
                API.sendChatMessageToPlayer(p, "Du bist bei keinem Shop");
                return;
            }
                string ap = string.Join(";" ,mysql.MySQL_POIData.getShopItems(shop).ToArray());
                API.triggerClientEvent(p, "ShopMenu", ap);
            
        }

        [Command("setcarspawn")]
        public void cmd_carspawn(Client p)
        {
            if(p.isInVehicle)
            {
                Account a = methods.getMethods.getAccountByClient(p);
                Vehicle v = p.vehicle;
                VehicleD vd = methods.getMethods.getVehiclebyVehicle(v);
                if(vd == null)
                {
                    API.sendChatMessageToPlayer(p,"Dieses Auto ist Temporär");
                    return;
                }
                if(vd.owner == a)
                {
                    mysql.MySQL_Vehicles.setSpawn(vd);
                    API.sendChatMessageToPlayer(p, "CarSpawn set");
                } else
                {
                    API.sendChatMessageToPlayer(p, "Dieses FAhrzeug gehört dir nicht");
                }
            }
        }
        #endregion vehiclecmds

    }
    }

