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
#endif



namespace CarryBattleSC {


    /** 
        NuSD: 
        <user> = [ S<email>
                   ,S<pswHash>
                   ,S<address>
                   ,S<nickName>
                   ,S<icon>
                   ,S<serverID>
                   ,S<city>
                   ,S<warID>
                   ,S[(S<cardId>)^10]
                 ]
    */
    public class User {
        public string email;
        public byte[] pswHash;
        public byte[] address;
        public string nickName;
        public BigInteger icon;
        public BigInteger serverID;
        public BigInteger city;    //The city the user occupied. If city = Const.numCities, the user is at large
        public byte[] warID;
        public Card[] cards;

    }


    /** 
        NuSD: 
        <card> = [  S<cardID>
                   ,S<type#1>
                   ,S<lvls#10>
                   ,S<ownerEmail>
                   ,S<warPos#1>
                 ]
    */
    public class Card {
        public byte[] cardID;
        public BigInteger type;   //TypeArmy
        public byte[] lvls;    // Range: 0 - 255
        public string ownerEmail;
        public BigInteger warPos;     //The Position of the war
        // N/S

    }



    public class CardLive {
        public BigInteger cardId;
    }


    public class City {
        public BigInteger status;
        public string ownerID;

        public BigInteger weather;
        public BigInteger windDirection;
        public BigInteger windStrength;
        //---- non-storage ----
        public BigInteger id;
    }

    public class Seige {
        public BigInteger serverID;
        public BigInteger city;
        public string ownerEmail;
        public string seigerEmail;
        public byte[] id;

    }


    public static class RW {
        public static byte[] Card2Bytes(Card card) {
            byte[] segId = NuSD.Seg(card.cardID);
            byte[] segtype = NuSD.SegInt(card.type);
            byte[] segLvl = NuSD.Seg(card.lvls);
            byte[] segOwner = NuSD.SegString(card.ownerEmail);
            byte[] segPos = NuSD.SegInt(card.warPos);
            return NuSD.JoinSegs2Table(segId, segtype, segLvl, segOwner, segPos);
        }


        public static Card Bytes2Card(byte[] data) {
            Card card = new Card();
            card.cardID = NuSD.DesegWithIdFromTable(data, 0);
            card.type = Op.Bytes2BigInt(NuSD.DesegWithIdFromTable(data, 1));
            card.lvls = NuSD.DesegWithIdFromTable(data, 2);
            card.ownerEmail = Op.Bytes2String(NuSD.DesegWithIdFromTable(data, 3));
            card.warPos = Op.Bytes2BigInt(NuSD.DesegWithIdFromTable(data, 4));

            return card;
        }

        public static Card ToCard(this byte[] data) => Bytes2Card(data);

        public static byte[] City2Bytes(City city) {
            return new byte[0];
        }
        public static City Bytes2City(byte[] data) {
            return new City();
        }


        public static byte[] Seige2Bytes(Seige war) {
            return new byte[0];
        }
        public static Seige Bytes2Seige(byte[] data) {
            return new Seige();
        }

        public static byte[] User2Bytes(User user) {
            byte[] segCardIds = Op.Empty;
            for (int i = 0; i < user.cards.Length;i++){
                segCardIds = segCardIds.AddSeg(user.cards[i].cardID);
                //segCardIds = Op.JoinTwoByteArray(segCardIds, NuSD.Seg(user.cards[i].cardID));
            }

            return Op.Empty
                     .AddSegStr(user.email)
                     .AddSeg(user.pswHash)
                     .AddSeg(user.address)
                     .AddSegStr(user.nickName)
                     .AddSegInt(user.icon)
                     .AddSegInt(user.serverID)
                     .AddSegInt(user.city)
                     .AddSeg(user.warID)
                     .AddSeg(segCardIds);
        }

        public static byte[] UserAndCards2Bytes(User user)
        {
            byte[] bytesUser = Op.Empty.AddSeg(User2Bytes(user));
            int num = user.cards.Length;

            byte[] bytesCards = Op.Empty;
            for (int i = 0; i < num;i++){
                byte[] cardData = Card2Bytes(user.cards[i]);
                bytesCards.AddSeg(cardData);
            }
            return bytesUser.AddSeg(bytesCards);

        }

        public static User Bytes2User(byte[] data) {
            if(data.SizeTable() != 8) {
                return null;
            }
            else{
                User user = new User
                {
                    email = data.SplitTblStr(0),
                    pswHash = data.SplitTbl(1),
                    address = data.SplitTbl(2),
                    nickName = data.SplitTblStr(3),
                    icon = data.SplitSegInt(4),
                    serverID = data.SplitSegInt(5),
                    city = data.SplitSegInt(6)
                };


                //Also need to read all cards 
                byte[] cardIds = data.SplitTbl(7);
                int num = cardIds.SizeTable();

                user.cards = new Card[num];

                for (int i = 0; i < num; i++)
                {
                    user.cards[i] = FindCard(cardIds.SplitTbl(i));

                }

                return user;
            }


        }



        public static byte[] FindDataUser(string email) {
            return IO.GetStorageWithKeyPath(Const.preUser, email);
        }

        public static User FindUser(string email) => Bytes2User(FindDataUser(email));


        public static byte SaveUser(User user) {
            return IO.SetStorageWithKeyPath(User2Bytes(user), Const.preUser, user.email);
        }

        public static byte SaveCard(Card card) { 
            return IO.SetStorageWithKeyPath(Card2Bytes(card), Const.preCard, card.cardID.ToString());
        }

        public static byte[] FindDataCard(byte[] cardID) {
            return IO.GetStorageWithKeyPath(Const.preCard, Op.Bytes2String(cardID));
        }

        public static Card FindCard(byte[] cardID) => Bytes2Card(FindDataCard(cardID));

        public static byte[] FindDataCity(BigInteger serverId, BigInteger cityId) {
            return IO.GetStorageWithKeyPath(
                Const.preServer,
                Op.BigInt2String(serverId),
                Const.preCity,
                Op.BigInt2String(cityId)
            );
        }

        public static byte[] FindDataSeige(BigInteger serverId, BigInteger seigeId) {
            return IO.GetStorageWithKeyPath(
                Const.preServer,
                Op.BigInt2String(serverId),
                Const.preSeige,
                Op.BigInt2String(seigeId)
            );
        }



        public static int NumCardsOfUser(User user) {
            return user.cards.Length;
        }

        /// <summary>
        /// 给指定用户产生随机初始卡
        /// 返回实际生成的卡牌数量
        /// </summary>
        public static Card[] GenerateRandomCards(User user, int num)
        {
            Card[] cards = new Card[num];
            int numCardsAlready = user.cards.Length;
            byte[] dHeight = Op.BigInt2Bytes(Blockchain.GetHeight());

            for (int i = 0; i < num; i++){
                Card cardResult = new Card();
                byte[] dEmail = Op.String2Bytes(user.email);
                byte[] dNum = Op.BigInt2Bytes(i + numCardsAlready);

                cardResult.cardID = Funcs.Rand(Op.JoinByteArray(dEmail,dNum,dHeight),10);

                cardResult.type = Op.Bytes2BigInt(Funcs.Rand(Op.JoinByteArray(dHeight,dNum,dEmail),1));
                cardResult.lvls = GenerateRandomCardLvs();//这算法你先前端用着吧，区块链上不行的

                cardResult.ownerEmail = user.email;
                cardResult.warPos = 0;
                SaveCard(cardResult);
                cards[i] = cardResult;
            }

            return cards;
        }

        /// <summary>
        /// 给指定用户产生随机初始卡,随机等级
        /// </summary>
        public static byte[] GenerateRandomCardLvs() {
            byte[] lvsResult = new byte[(int)Const.numCellsOfCard];
            for(int i = 0; i < Const.numCellsOfCard; i++) {
                //is empty or not (30% not empty)
                bool isEmpty = Funcs.Random(100) < 30;
                if(!isEmpty) {
                    int percentageValue = (int)Funcs.Random(Const.fullPercentage);
                    if(percentageValue < 50) {
                        lvsResult[i] = 1;
                    } else if(percentageValue < 70) {
                        lvsResult[i] = 2;
                    } else if(percentageValue < 90) {
                        lvsResult[i] = 3;
                    } else { 
                        lvsResult[i] = 4;
                    }
                } else {
                    lvsResult[i] = 0;
                }
            }
            return lvsResult;
        }
    }
}
