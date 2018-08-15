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
    public static class TypeArmy {
        public static readonly BigInteger Infantry = 0;
        public static readonly BigInteger Archer = 1;
        public static readonly BigInteger Cavalry = 2;
        public static readonly BigInteger TypeCount = 4;
    }

    public static class StatusCity {
        public static readonly BigInteger Empty = 0;
        public static readonly BigInteger Occupied = 1;
        public static readonly BigInteger Seiging = 2;
    }

    public static class Const {
        public static readonly BigInteger fullPercentage = 100;

        public static readonly BigInteger numCities = 20;
        public static readonly int numCardsPerReg = 4;
        public static readonly int numCardsTotalReg = 10;
        public static readonly BigInteger numCellsOfCard = 9;       //cells count in card
        public static readonly BigInteger numCardsPerTimeOnReg = 5; //cards count at a time

        public static readonly string operationRegister = "register";
        public static readonly string operationRegCards = "regCards";
        public static readonly string operationMigrate  = "migrate";
        public static readonly string operationGetUsers = "getUsers";
        public static readonly string operationgetCities = "getCities";
        public static readonly string operationGetUserSeiges = "getUserSeiges";
        public static readonly string operationGetCitySeiges = "getCitySeiges";
        public static readonly string operationSeige = "seige";
        public static readonly string operationClain = "claim";
        public static readonly string operationForge = "forge";

        public static readonly string preCard = "c";
        public static readonly string preUser = "u";
        public static readonly string preWeather = "w";
        public static readonly string preServer = "S";
        public static readonly string preCity = "C";
        public static readonly string preSeige = "s";
    }


    public class Credential {
        public string email;
        public byte[] address;
        public string psw;
    }


    public static class TypeWeather {
        public static readonly BigInteger Sunny = 0;
        public static readonly BigInteger Rain = 1;
        public static readonly BigInteger Snow = 2;
        public static readonly BigInteger Fog = 4;
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
        public static Credential GenCred(string email, byte[] address, string psw) {
            Credential credential = new Credential();
            credential.email = email;
            credential.address = address;
            credential.psw = psw;
            return credential;
        }

    }



    public static class ErrCate {
        public static readonly byte[] Nothing = { 0 };
        public static readonly byte[] Account = { 1 };
        public static readonly byte[] Server = { 2 };
        public static readonly byte[] War = { 3 };
        public static readonly byte[] City = { 4 };
        public static readonly byte[] Card = { 5 };
        public static readonly byte[] User = { 6 };
        public static readonly byte[] Weather = { 7 };
    }

    public static class ErrType {
        public static readonly byte[] Nothing = { 0 };
        public static readonly byte[] Duplicated = { 1 };
        public static readonly byte[] LackGas = { 10 };

        public static readonly byte[] NotExist = { 20 };
        public static readonly byte[] AuthFail = { 25 };

    }

}
