using System;
using System.Numerics;
using Neunity.Tools;
using IO = Neunity.Tools.NuIO;
#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
using System.Linq;
#endif



namespace CarryBattleSC
{


    /** 
    NuSD: <card> = [ S<cardID>,S<type#1>,S<lvls#9>,S<ownerEmail>,S<warPos#1> ]
*/
    public class Card
    {
        public byte[] cardID;
        public BigInteger type;   //TypeArmy
        public byte[] lvls;    // Range: 0 - 255
        public string ownerEmail;
        public BigInteger warPos;     //The Position of the war
        // N/S

    }

    /** 
        NuSD: <user> = [ S<email>,S<pswHash>,S<address>,S<nickName>,S<icon>,
                   S<score>,S<serverID>,S<city>,S<warID>,S[(S<cardId>)*10]]
    */
    public class User
    {
        public string email;
        public byte[] pswHash;
        public byte[] address;
        public string nickName;
        public BigInteger icon;
        public BigInteger score;
        public BigInteger serverID;
        public BigInteger city;    //The city the user occupied. If city = Const.zero, the user is free
        public byte[] warID;
        public Card[] cards;

    }




    /**
     * NuSD: <city> = [S<cityId>,S<ownerId>,S<siegeId>]
    */
    public class City
    {
        public BigInteger cityId;
        public BigInteger serverID;
        //public BigInteger status;
        public string ownerID;  //email
        public byte[] siegeID;
        public Card[] ownerCards;
    }

    public class Siege
    {
        public byte[] id;
        public BigInteger serverID;
        public BigInteger cityID;
        public string ownerEmail;
        public string siegerEmail;
        public BigInteger startHeight;
        public BigInteger endHeight;
        public BigInteger ownerScore;
        public BigInteger siegerScore;
        public Card[] ownerCards;
        public Card[] siegeCards;
    }


    /**
        The basic read, write, search actions for basic classes
    */

    public static class RW
    {
        #region Cards
        public static byte[] Card2Bytes(Card card)
        {
            if (card == null)
            {
                return Op.Void();
            }
            else
            {
                return NuSD.Seg(card.cardID)
                       .AddSegInt(card.type)
                       .AddSeg(card.lvls)
                       .AddSegStr(card.ownerEmail)
                       .AddSegInt(card.warPos);
            }
        }


        public static Card Bytes2Card(byte[] data)
        {
            if (data.Length == 0)
            {
                return null;
            }
            else
            {
                Card card = new Card
                {
                    cardID = data.SplitTbl(0),
                    type = data.SplitTblInt(1),
                    lvls = data.SplitTbl(2),
                    ownerEmail = data.SplitTblStr(3),
                    warPos = data.SplitTblInt(4)
                };
                return card;
            }

        }

        public static Card[] Table2Cards(byte[] table)
        {

            int num = table.SizeTable();
            Card[] cards = new Card[num];
            for (int i = 0; i < num; i++)
            {
                cards[i] = Bytes2Card(NuSD.SplitTbl(table, i));

            }
            return cards;
        }

        public static byte[] CardIDs2Table(Card[] cards)
        {
            int num = cards.Length;
            byte[] bytesCards = Op.Void();
            for (int i = 0; i < num; i++)
            {
                bytesCards = bytesCards.AddSeg(cards[i].cardID);
            }
            return bytesCards;
        }

        public static Card[] Table2CardIDs(byte[] table)
        {
            int num = table.SizeTable();
            Card[] cards = new Card[num];
            for (int i = 0; i < num; i++)
            {
                byte[] cid = NuSD.SplitTbl(table, i);
                cards[i] = FindCard(cid);
            }
            return cards;
        }

        public static byte SaveCard(Card card)
        {
            return IO.SetStorageWithKeyPath(Card2Bytes(card), Const.preCard, Op.Bytes2String(card.cardID));
        }

        public static byte[] FindDataCard(byte[] cardID)
        {
            return IO.GetStorageWithKeyPath(Const.preCard, Op.Bytes2String(cardID));
        }

        public static Card FindCard(byte[] cardID) => Bytes2Card(FindDataCard(cardID));


        #endregion

        #region User
        public static byte[] User2Bytes(User user)
        {
            if (user == null) return Op.Void();
            byte[] segCardIds = Op.Void();
            if (user.cards != null && user.cards.Length > 0)
            {
                for (int i = 0; i < user.cards.Length; i++)
                {
                    //no cards data
                    segCardIds = segCardIds.AddSeg(user.cards[i].cardID);
                }
            }

            byte[] ret = NuSD.SegString(user.email)
                     .AddSeg(user.pswHash)
                     .AddSeg(user.address)
                     .AddSegStr(user.nickName)
                     .AddSegInt(user.icon)
                     .AddSegInt(user.score)
                     .AddSegInt(user.serverID)
                     .AddSegInt(user.city)
                     .AddSeg(user.warID)
                     .AddSeg(segCardIds);

            return ret;

        }


        public static User Bytes2User(byte[] data)
        {
            if (data.Length == 0)
            {
                return null;
            }
            else
            {
                User user = new User
                {
                    email = data.SplitTblStr(0),
                    pswHash = data.SplitTbl(1),
                    address = data.SplitTbl(2),
                    nickName = data.SplitTblStr(3),
                    icon = data.SplitTblInt(4),
                    score = data.SplitTblInt(5),
                    serverID = data.SplitTblInt(6),
                    city = data.SplitTblInt(7),
                    warID = data.SplitTbl(8)
                };


                //Also need to read all cards 
                byte[] cardIds = data.SplitTbl(9);
                int num = cardIds.SizeTable();

                user.cards = new Card[num];

                for (int i = 0; i < num; i++)
                {
                    user.cards[i] = FindCard(cardIds.SplitTbl(i));

                }
                return user;
            }
        }

        public static byte[] UserCards2Table(User user)
        {
            if (user == null) return Op.Void();
            int num = user.cards.Length;
            byte[] bytesCards = Op.Void();
            for (int i = 0; i < num; i++)
            {
                byte[] cardData = Card2Bytes(user.cards[i]);
                bytesCards = bytesCards.AddSeg(cardData);
            }
            return bytesCards;
        }

        public static int NumCardsOfUser(User user)
        {
            if (user.cards == null)
            {
                return 0;
            }
            else
            {
                return user.cards.Length;
            }
        }


        public static byte SaveUser(User user)
        {
            return IO.SetStorageWithKeyPath(User2Bytes(user), Const.preUser, user.email);
        }

        public static byte[] FindDataUser(string email)
        {
            return IO.GetStorageWithKeyPath(Const.preUser, email);
        }

        public static User FindUser(string email) => Bytes2User(FindDataUser(email));


        #endregion


        #region City
        public static byte[] City2Bytes(City city)
        {
            if (city == null) return Op.Void();

            byte[] data = NuSD.SegInt(city.serverID)
                              .AddSegInt(city.cityId)
                              .AddSegStr(city.ownerID)
                              .AddSeg(city.siegeID);
            if (city.ownerID != "")
            {
                data = data.AddSeg(CardIDs2Table(city.ownerCards));
            }
            return data;

        }
        public static City Bytes2City(byte[] data)
        {
            City city = new City
            {
                serverID = data.SplitTblInt(0),
                cityId = data.SplitTblInt(1),
                ownerID = data.SplitTblStr(2),
                siegeID = data.SplitSeg(3)
            };

            if (city.ownerID != "")
            {
                byte[] cardIds = data.SplitSeg(4);
                city.ownerCards = Table2CardIDs(cardIds);
            }

            return city;
        }

        public static City CreateCity(BigInteger serverID, BigInteger cityID)
        {
            City city = new City
            {
                cityId = cityID,
                serverID = serverID
            };


            return city;
        }

        public static BigInteger GetStatusCity(City city)
        {
            if (city.ownerID == "")
            {
                return StatusCity.Empty;
            }
            else if (city.siegeID == Op.Void())
            {
                return StatusCity.Occupied;
            }
            else
            {
                return StatusCity.Sieging;
            }
        }

        public static byte SaveCity(BigInteger serverID, City city)
        {
            return IO.SetStorageWithKeyPath(City2Bytes(city)
                                            , Const.preServer
                                            , Op.BigInt2String(serverID)
                                            , Const.preCity
                                            , Op.BigInt2String(city.cityId)
                                           );
        }

        //public static byte[] FindDataCity(BigInteger cityID)
        //{
        //    return IO.GetStorageWithKeyPath(Const.preCity, cityID.ToString());
        //}

        public static City FindCity(BigInteger serverID, BigInteger cityID)
        {
            return Bytes2City(FindDataCity(serverID, cityID));
        }

        public static byte[] FindDataCity(BigInteger serverId, BigInteger cityId)
        {
            return IO.GetStorageWithKeyPath(
                Const.preServer,
                Op.BigInt2String(serverId),
                Const.preCity,
                Op.BigInt2String(cityId)
            );
        }

        #endregion

        #region Seige
        public static byte[] Seige2Bytes(Siege war)
        {
            return new byte[0];
        }
        public static Siege Bytes2Seige(byte[] data)
        {
            return new Siege();
        }

        public static Siege CreateSiege(BigInteger server, City city, User sieger, Card[] cards, BigInteger startHeight)
        {

            Siege siege = new Siege
            {
                serverID = server,
                cityID = city.cityId,
                ownerEmail = city.ownerID,
                siegerEmail = sieger.email,
                startHeight = startHeight,
                ownerCards = city.ownerCards,
                siegeCards = cards
            };
            return siege;
        }

        public static byte[] FindDataSiege(BigInteger serverId, byte[] seigeId)
        {
            return IO.GetStorageWithKeyPath(
                Const.preServer,
                Op.BigInt2String(serverId),
                Const.preSeige,
                Op.Bytes2String(seigeId)
            );
        }

        public static Siege FindSiege(BigInteger serverId, byte[] seigeId)
        {
            return Bytes2Seige(FindDataSiege(serverId, seigeId));
        }

        public static byte[] FindSiegeDataByCityHistId(BigInteger serverId, BigInteger cityId, BigInteger histId)
        {
            byte[] siegeId = IO.GetStorageWithKeyPath(
                Const.preServer, Op.BigInt2String(serverId),
                Const.preCity, Op.BigInt2String(cityId),
                Const.preSeige, Op.BigInt2String(histId)
            );

            return FindDataSiege(serverId, siegeId);
        }

        public static BigInteger AddSiegeCityHist(BigInteger serverId, BigInteger cityId, byte[] siegeId)
        {
            BigInteger histId = NumCitySiegeHistory(serverId, cityId);
            IO.SetStorageWithKeyPath(siegeId,
                Const.preServer, Op.BigInt2String(serverId),
                Const.preCity, Op.BigInt2String(cityId),
                Const.preSeige, Op.BigInt2String(histId)
            );

            IO.SetStorageWithKeyPath(Op.BigInt2Bytes(histId + 1),
                    Const.preServer, Op.BigInt2String(serverId),
                    Const.preCity, Op.BigInt2String(cityId),
                    Const.keyTotSiege
            );
            return histId;
        }


        public static byte SaveSiege(Siege siege)
        {
            byte[] data = Seige2Bytes(siege);
            return IO.SetStorageWithKeyPath(data,
                Const.preServer,
                Op.BigInt2String(siege.serverID),
                Const.preCity,
                Op.BigInt2String(siege.cityID)
            );
        }

        public static BigInteger NumCitySiegeHistory(BigInteger serverId, BigInteger cityId)
        {
            return Op.Bytes2BigInt(
                IO.GetStorageWithKeyPath(
                    Const.preServer, Op.BigInt2String(serverId),
                    Const.preCity, Op.BigInt2String(cityId),
                    Const.keyTotSiege
                )
            );
        }


        public static byte[] FindSiegesByCityHistory(BigInteger serverId, BigInteger cityId, BigInteger numLatest)
        {

            BigInteger tot = NumCitySiegeHistory(serverId, cityId);
            BigInteger num = (tot > numLatest) ? numLatest : tot;
            byte[] ret = Op.Void();
            for (BigInteger i = 0; i < num; i = i + 1)
            {
                BigInteger histId = tot - i - 1;
                byte[] siegeData = FindSiegeDataByCityHistId(serverId, cityId, histId);
                ret.AddSeg(siegeData);
            }
            return ret;
        }

        public static BigInteger NumUserSiegeHistory(User user)
        {
            return Op.Bytes2BigInt(
                IO.GetStorageWithKeyPath(
                    Const.preUser, user.email,
                    Const.keyTotSiege
                )
            );
        }


        #endregion


        #region Weather
        public static byte SaveWeathers(byte[] weathers)
        {
            return IO.SetStorageWithKey(Const.preWeather, weathers);
        }

        public static byte[] ReadWeathers()
        {
            return IO.GetStorageWithKey(Const.preWeather);
        }

        public static BigInteger GetWeather(BigInteger cityID)
        {
            return 1;   //TBD
            //byte[] weathers = ReadWeathers();
            //if (cityID > Const.numCities)
            //{
            //    return 0;
            //}
            //else
            //{
            //    return Op.Byte2BigInt(weathers[(int)cityID]);
            //}

        }
        #endregion

    }
}
