using System;
using System.Numerics;
using Neunity.Tools;

#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
#endif


namespace CarryBattleSC {
    //军队类型
    public static class TypeArmy {
        public static readonly BigInteger Infantry  = 0;    //步
        public static readonly BigInteger Archer    = 1;    //弓
        public static readonly BigInteger Cavalry   = 2;    //骑
        public static readonly BigInteger TypeCount = 4;
    }

    //城市状态
    public static class StatusCity {
        public static readonly BigInteger Empty = 0;
        public static readonly BigInteger Occupied = 1;
        public static readonly BigInteger Sieging = 2;
    }

    public static class Const {
        public static readonly BigInteger Zero = 0;
        public static readonly BigInteger fullPercentage = 100;


        public static readonly BigInteger numCities = 8; //1,2,.....8
        public static readonly int numCardsPerReg = 4;
        public static readonly int numCardsTotalReg = 10;
        public static readonly int numCardsSiege = 10;
        public static readonly int numCellsOfCard = 9;       //cells count in card
        public static readonly BigInteger numCardsPerTimeOnReg = 5; //cards count at a time
        public static readonly BigInteger blocksForSiege = 10; 

        //创世
        public static readonly string operationGenesis = "genesis";
        public static readonly string operationFakeUsers = "fakeUsers";
        public static readonly string operationFakeOccupied = "fakeOccupies";
        public static readonly string operationSetWeather = "setWeather";

        //普通操作
        public static readonly string operationRegister = "register";
        public static readonly string operationRegCards = "regCards";
        public static readonly string operationMigrate = "migrate";
        public static readonly string operationGetUser = "getUser";
        public static readonly string operationGetUsers = "getUsers";
        public static readonly string operationgetCities = "getCities";
        public static readonly string operationGetUserSeiges = "getUserSeiges";
        public static readonly string operationGetCitySeiges = "getCitySeiges";
        public static readonly string operationSiege = "siege";
        public static readonly string operationClain = "claim";
        public static readonly string operationForge = "forge";

        public static readonly string preCard = "c";
        public static readonly string preUser = "u";
        public static readonly string preWeather = "w";
        public static readonly string preSeige = "s";
        public static readonly string preServer = "S";
        public static readonly string preCity = "C";
        public static readonly string keyTotSiege = "ts";
    }

    /* NuSD
        <credential> = [ S<email>
                         ,S<address>
                         ,S<pswHash>
                        ]
    */
    public class Credential {
        public string email;
        public byte[] address;
        public string psw;
    }



    //public class Weather{
    //	public BigInteger weather;
    //	public BigInteger windDirection;
    //	public BigInteger windStrength;
    //}



    public static class Global {
        //CardTotalCount used to give value to cardid
        public static BigInteger cardTotalCount = 1;

        public static bool VerifyUser(Credential credential) {
            
            return true;
        }

    }

    /// <summary>
    /// 地形和天气
    /// </summary>
    public static class TerrainAndWeather {

        //Terrain                                               步       弓       骑
        public static readonly BigInteger Plain = 0; //         125     100     150
        public static readonly BigInteger Valley = 1;//         100     150     125
        public static readonly BigInteger Forest = 2;//         150     125     100
        public static readonly BigInteger TerrainCount = 3;

        //Weather                                               步       弓      骑
        public static readonly BigInteger Sunny     = 0; //     100      100    100
        public static readonly BigInteger Rain      = 1; //     50       75     100
        public static readonly BigInteger Snow      = 2; //     75       100    50
        public static readonly BigInteger Wind      = 3; //     100      50     75
        public static readonly BigInteger WeatherCount = 4;

        public static BigInteger GetTerrain() {
            return 1;   //[TODO]
            //return Funcs.Random(TerrainCount);
        }
        public static BigInteger GetWeather() {
            return 1;   //[TODO]

            //return Funcs.Random(WeatherCount);
        }


        //地形加成
        //public static float GetTerrainAdditionRate(BigInteger armyType, BigInteger terrain) {
        //    float result = 1.0f;
        //    if(armyType == TypeArmy.Infantry) {
        //        if(terrain == Plain) {
        //            result = 1.25f;
        //        } else if(terrain == Valley) {
        //            result = 1.0f;
        //        } else if(terrain == Forest) {
        //            result = 1.5f;
        //        }
        //    } else if(armyType == TypeArmy.Archer) {
        //        if(terrain == Plain) {
        //            result = 1.0f;
        //        } else if(terrain == Valley) {
        //            result = 1.5f;
        //        } else if(terrain == Forest) {
        //            result = 1.25f;
        //        }
        //    } else if(armyType == TypeArmy.Cavalry) {
        //        if(terrain == Plain) {
        //            result = 1.5f;
        //        } else if(terrain == Valley) {
        //            result = 1.25f;
        //        } else if(terrain == Forest) {
        //            result = 1.0f;
        //        }
        //    }
        //    return result;
        //}

        //天气加成
        //public static float GetWeatherdditionRate(BigInteger armyType, BigInteger weather) {
        //    float result = 1.0f;
        //    if(armyType == TypeArmy.Infantry) {
        //        if(weather == Rain) {
        //            result = 0.5f;
        //        } else if(weather == Snow) {
        //            result = 0.75f;
        //        } else if(weather == Wind) {
        //            result = 1.0f;
        //        }
        //    } else if(armyType == TypeArmy.Archer) {
        //        if(weather == Rain) {
        //            result = 0.75f;
        //        } else if(weather == Snow) {
        //            result = 1.0f;
        //        } else if(weather == Wind) {
        //            result = 0.5f;
        //        }
        //    } else if(armyType == TypeArmy.Cavalry) {
        //        if(weather == Rain) {
        //            result = 1.0f;
        //        } else if(weather == Snow) {
        //            result = 0.5f;
        //        } else if(weather == Wind) {
        //            result = 0.75f;
        //        }
        //    }
        //    return result;
        //}
        /// <summary>
        /// Gets the advantage addition rate.
        /// </summary>
        /// <returns>The advantage addition rate of armyType1 when vs. armyType2 </returns>
        /// <param name="armyType1">Army type1.</param>
        /// <param name="armyType2">Army type2.</param>
        //public static float GetAdvantageAdditionRate(BigInteger armyType1, BigInteger armyType2) {
        //    float result = 1.0f;
        //    if(armyType1 == TypeArmy.Infantry) {
        //        if(armyType2 == TypeArmy.Cavalry) {
        //            result = 0.75f;
        //        } else if(armyType2 == TypeArmy.Infantry) {
        //            result = 1f;
        //        } else if(armyType2 == TypeArmy.Archer) {
        //            result = 1.5f;
        //        }
        //    } else if(armyType1 == TypeArmy.Archer) {
        //        if(armyType2 == TypeArmy.Infantry) {
        //            result = 0.75f;
        //        } else if(armyType2 == TypeArmy.Archer) {
        //            result = 1f;
        //        } else if(armyType2 == TypeArmy.Cavalry) {
        //            result = 1.5f;
        //        }
        //    } else if(armyType1 == TypeArmy.Cavalry) {
        //        if(armyType2 == TypeArmy.Archer) {
        //            result = 0.75f;
        //        } else if(armyType2 == TypeArmy.Cavalry) {
        //            result = 1f;
        //        } else if(armyType2 == TypeArmy.Infantry) {
        //            result = 1.5f;
        //        }
        //    }
        //    return result;
        //}
    }



    public static class ErrCate {
        public static readonly BigInteger Nothing = 0;
        public static readonly BigInteger Account = 1;
        public static readonly BigInteger Server = 2;
        public static readonly BigInteger War = 3;
        public static readonly BigInteger City = 4;
        public static readonly BigInteger Card = 5;
        public static readonly BigInteger User = 6;
        public static readonly BigInteger Weather = 7;
    }

    public static class ErrType {
        public static readonly BigInteger Nothing = 0;
        public static readonly BigInteger Duplicated = 1;
        public static readonly BigInteger LackGas = 10;

        public static readonly BigInteger NotExist = 20;
        public static readonly BigInteger AuthFail = 25;

    }

}
