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



namespace CarryBattleSC
{

    static public class Alg
    {



        //创世时, 产生城市数据
        public static bool GenesisCreateCities(BigInteger serverId)
        {
            for (int i = 0; i < Const.numCities; i++)
            {
                City city = RW.CreateCity(serverId, i);
                RW.SaveCity(serverId, city);
            }
            return true;
        }


        public static Siege StartSiege(BigInteger server, City city, User sieger, Card[] cards)
        {
            BigInteger height = Blockchain.GetHeight();
            Siege siege = RW.CreateSiege(server, city, sieger, cards, height);
            RW.AddSiegeCityHist(server, city.cityId, siege.id);
            return siege;
        }

        public static Siege FinishSiege(Siege siege,byte[]randFact)
        {
            if (siege.endHeight == 0)
            {   //尚未结算
                BigInteger height = Blockchain.GetHeight();
                if (height >= siege.startHeight + Const.blocksForSiege)
                {
                    BigInteger weather = RW.GetWeather(siege.cityID);
                    //[TODO] 添加结算算法

                    BigInteger scoreOwner = 123;
                    BigInteger scoreSieger = 456;

                    //玩家加分
                    User owner = RW.FindUser(siege.ownerEmail);
                    User sieger = RW.FindUser(siege.siegerEmail);
                    owner.score += scoreOwner;
                    sieger.score += scoreSieger;
                    RW.SaveUser(owner);
                    RW.SaveUser(sieger);

                    //战役转移
                    siege.endHeight = height;
                    RW.SaveSiege(siege);
                    return siege;
                }
                else
                {
                    return siege;
                }
            }
            else
            {
                return siege;
            }
        }

    }
}