using System;
using System.Numerics;
using Neunity.Tools;
using CarryBattleSC;
#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Helper = Neo.SmartContract.Framework.Helper;
#else
using Neunity.Adapters.Unity;
#endif



namespace Neunity.App {
    public class Contract : SmartContract
    {

        public static readonly byte[] Owner = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y".ToScriptHash();

        public static byte[] Random(byte[] salt, int size = 1)
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            Header bl = Blockchain.GetHeader(Blockchain.GetHeight());
            return Hash256(bl.Hash.Concat(tx.Hash).Concat(salt)).Range(0, size);
        }

        public static byte[] Rand(int size = 1){
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            Header bl = Blockchain.GetHeader(Blockchain.GetHeight());
            return Hash256(bl.Hash.Concat(tx.Hash)).Range(0, size);
        }

        public static object Main(string operation, params object[] args)
        {
            if (operation == "test")
            {
                byte o3 = 1;
                byte b3 = 12;
                byte e3 = 252;
                byte[] ba3 = new byte[3] { o3, b3, e3 };
                // byte[] ba3 = new byte[3]{0,12,252};
                Runtime.Notify(3, o3, b3, e3, ba3);

                byte o4 = 1;
                byte b4 = 12;
                byte e4 = 252;
                byte[] ba4 = new byte[3] { o4, b4, e4 };
                // byte[] ba4 = new byte[3]{0,12,252};
                Runtime.Notify(3, o4, b4, e4, ba4);
                return true;
            }
            if (Runtime.Trigger == TriggerType.Verification)
            {
                if (Owner.Length == 20)
                {
                    return Runtime.CheckWitness(Owner);
                }
                else if (Owner.Length == 33)
                {
                    byte[] signature = Op.String2Bytes(operation);
                    return VerifySignature(signature, Owner);
                }
                return true;
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == Const.operationGenesis)
                {
                    BigInteger serverId = (BigInteger)args[0];
                    return Genesis(serverId);
                }
                else if (operation == Const.operationFakeUsers)
                {
                    // <emails> = [S<email>*]
                    //<FakeUsers> = [S<user>*]
                    BigInteger serverId = (BigInteger)args[0];
                    byte[] emails = (byte[])args[1];
                    return FakeUsers(serverId, emails);
                }
                else if (operation == Const.operationFakeOccupied)
                {
                    //<FakeOccupies> = S<user>
                    BigInteger serverId = (BigInteger)args[0];
                    string email = (string)args[1];
                    BigInteger cityId = (BigInteger)args[2];
                    return FakeOccupies(serverId, email, cityId);
                }
                else if (operation == Const.operationSetWeather)
                {
                    byte[] weathers = (byte[])args[0];
                    return SetWeather(weathers);
                }
                else
                {
                    string email = (string)args[0];
                    string psw = (string)args[1];
                    byte[] address = (byte[])args[2];

                    Credential credential = GenCred(email, address, psw);

                    //<register> = S<header>
                    if (operation == Const.operationRegister)
                    {

                        BigInteger serverId = (BigInteger)args[3];
                        string nickName = (string)args[4];
                        BigInteger iconID = (BigInteger)args[5];
                        return Register(credential, serverId, nickName, iconID);
                    }

                    //<regCards> = [S<header>,S<user>,S[S<cards>*]]
                    if (operation == Const.operationRegCards)
                    {
                        BigInteger num = (BigInteger)args[3];
                        return RegCards(credential, (int)num);
                    }

                    if (operation == Const.operationMigrate)
                    {
                        BigInteger newServerId = (BigInteger)args[3];
                        return Migrate(credential, newServerId);
                    }

                    //<getUsers> = [S<header>,S<user>,S[S<cards>*]]
                    if (operation == Const.operationGetUser)
                    {
                        string emailU = (string)args[3];
                        return GetUser(credential, emailU);
                    }
                    //<getUsers> = [S<header>,S<user>,S[S<cards>*]]
                    if (operation == Const.operationGetUsers)
                    {
                        byte[] emails = (byte[])args[3];
                        return GetUsers(credential, emails);
                    }

                    if (operation == Const.operationgetCities)
                    {
                        BigInteger serverId = (BigInteger)args[3];
                        return GetCities(credential, serverId);

                    }

                    if (operation == Const.operationGetUserSeiges)
                    {
                        string userEmail = (string)args[3];
                        BigInteger max = (BigInteger)args[4];
                        return GetUserSeiges(credential, userEmail, max);

                    }

                    if (operation == Const.operationGetCitySeiges)
                    {
                        BigInteger serverId = (BigInteger)args[3];
                        BigInteger cityId = (BigInteger)args[4];
                        BigInteger max = (BigInteger)args[5];
                        return GetCitySeiges(credential, serverId, cityId, max);

                    }

                    if (operation == Const.operationSiege)
                    {
                        BigInteger serverId = (BigInteger)args[3];
                        BigInteger cityId = (BigInteger)args[4];
                        byte[] cardIds = (byte[])args[5];
                        return StartSiege(credential, serverId, cityId, cardIds);
                    }

                    if (operation == Const.operationClain)
                    {
                        string userEmail = (string)args[3];
                        return Claim(credential, userEmail);
                    }
                    if (operation == Const.operationForge)
                    {
                        BigInteger cardId = (BigInteger)args[3];
                        BigInteger score = (BigInteger)args[4];
                        return Forge(credential, cardId, score);
                    }

                }

            }
            return false;
        }

        /** Register the user onto a server. 
         * 
         * 用户注册/登录 （由于钱包功能尚不支持，Demo阶段暂时使用用户邮箱/密码作为认证方法。若有变动将在Global.VerifyUser()方法里变更实现）
         * 返回：该用户所有卡牌具体信息。若在该服务器上新账户，为用户新建若干卡牌。
         * 
         * Check psw/signature first, then
         * If the user does exist already, 
         *      If user exist on current server, consider as login in.
         *      If user does exist on another server, transfer user to this server.
         * Else, create cards for this user. Need attach some GAS...
         * 
         * Return: S<header> 

        */
        private static byte[] Register(Credential credential, BigInteger serverId, string nickName, BigInteger iconID)
        {
            byte[] userData = RW.FindDataUser(credential.email);
            if (userData.Length != 0)
            {   //Exists
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.Duplicated);

            }
            else
            {
                byte[] psdHash = Hash256(Op.String2Bytes(credential.psw));
                User user = new User();
                user.address = credential.address;
                user.email = credential.email;
                user.pswHash = psdHash;
                user.nickName = nickName;
                user.icon = iconID;
                user.serverID = serverId;
                user.warID = Op.Void();
                user.city = Const.numCities;
                //user.cardIDs = new byte[0];

                RW.SaveUser(user);
                return NuTP.RespDataSucWithBody(RW.User2Bytes(user));
            }
        }

        /* * NuTP:

         <RegCards> =  [S<header>, S[S<card>*]]
                      
         */
        private static byte[] RegCards(Credential credential, int num)
        {
            byte[] userData = RW.FindDataUser(credential.email);
            if (userData.Length == 0)
            {   //Account Not exist
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.NotExist);
            }
            else
            {   //Account does exist
                
                User user = RW.Bytes2User(userData);
                int numAlready = RW.NumCardsOfUser(user);
                int numPending = Const.numCardsTotalReg - numAlready;
                if (numPending <= 0)
                {
                    return NuTP.RespDataWithCode(ErrCate.Card, ErrType.Duplicated);
                }
                else
                {
                    CarryBattleSC.Card[] cardsNew = GenerateRandomCards(user, (numPending < Const.numCardsPerReg) ? numPending : Const.numCardsPerReg);
                    CarryBattleSC.Card[] cardsOrig = user.cards;
                    user.cards = new CarryBattleSC.Card[cardsOrig.Length + cardsNew.Length];


                    for (int i = 0; i < cardsOrig.Length; i++)
                    {
                        user.cards[i] = cardsOrig[i];
                    }

                    for (int j = 0; j < cardsNew.Length; j++)
                    {
                        user.cards[j + cardsOrig.Length] = cardsNew[j];
                    }
                    //更新玩家数据
                    RW.SaveUser(user);

                    byte[] body = RW.UserCards2Table(user);
                    return NuTP.RespDataSucWithBody(body);
                }
            }
        }


        /** Get basic information of an user
         * 
         *  获取一个玩家信息, 内容包括所在服务器、所占城池、
         * 返回：该用户所有卡牌具体信息。
         * 
         * The information includes:
         *     -  User email and address,
         *     -  User current server and city
         *     -  User score and unclaimed score
         *     -  User's cards (full cards data)
         * 
         * Return: Array of the User objects and cards
         
         *              
        */

        /* * NuTP:
         <GetUsers> =  [S<header>, S<user>, S[S<card>*]]

         */

        private static byte[] GetUser(Credential credential, string email)
        {
            // This method does not require credential

            User user = RW.FindUser(email);
            if (user != null)
            {
                byte[] body = NuSD.Seg(RW.User2Bytes(user)).AddSeg(RW.UserCards2Table(user));
                return NuTP.RespDataSucWithBody(body);
            }
            else
            {
                return NuTP.RespDataWithDetail(ErrCate.User, ErrType.NotExist, email, Op.Void());
            }
        }

        /** Get basic information of a list of users
         * 
         *  获取一系列玩家信息, 内容包括所在服务器、所占城池、
         * 返回：该用户所有卡牌具体信息。
         * 
         * For each user, the information includes:
         *     -  User email and address,
         *     -  User current server and city
         *     -  User score and unclaimed score
         *     -  User's cards (full cards data)
         * 
         * Return: Array of the User objects and cards
         
         *              
        */

        /* * NuTP:
         <emails> = S[S<email>*]
         
         <getUsers> =  [S<header>,
                       S[([S<user>,
                            S[S<card>*10]] 
                       )*]
         */

        private static byte[] GetUsers(Credential credential, byte[] emails)
        {
            // This method does not require credential

            int numEmails = emails.SizeTable();
            byte[] retBody = Op.Void();

            for (int i = 0; i < numEmails; i++)
            {
                string email = emails.SplitTblStr(i);
                User user = RW.FindUser(email);
                if (user != null)
                {
                    byte[] body = NuSD.Seg(RW.User2Bytes(user)).AddSeg(RW.UserCards2Table(user));
                    retBody.AddSeg(body);

                }
                else
                {
                    return NuTP.RespDataWithDetail(ErrCate.User, ErrType.NotExist, email, Op.Void());
                }

            }

            NuTP.Response response = new NuTP.Response
            {
                header = new NuTP.Header
                {
                    domain = NuTP.SysDom,
                    code = NuTP.Code.Success
                },

                body = retBody
            };
            return NuTP.Response2Bytes(response);

        }


        /** User migrate to another server. 
         * 玩家转服 
         * Return: bool for success/failure
        */
        private static byte[] Migrate(Credential credential, BigInteger serverId)
        {
            if (Global.VerifyUser(credential))
            {
                return NuTP.RespDataWithCode(NuTP.SysDom,NuTP.Code.ServiceUnavailable);
            }
            else
            {
                return NuTP.RespDataWithCode(NuTP.SysDom, NuTP.Code.Unauthorized);;
            }
        }

        /** All cities in a server with "maximum" recent items. 
         * 
         * 获取城市状态列表
         * 
         * Return: Array of War Object
        */
        private static byte[] GetCities(Credential credential, BigInteger serverId)
        {
            byte[] body = Op.Void();
            for (int i = 0; i < Const.numCities; i++)
            {
                body.AddSeg(RW.FindDataCity(serverId, i));

            }
            return NuTP.RespDataSucWithBody(body);
        }

        /** All the completed wars related with particular user with "maximum" recent items. 
		 * 
		 * 获取某用户已完成战役列表（最近的maximum个）。返回：一组War的列表
		 * 
		 * 
		 * Return: Array of War Object
        */
        private static byte[] GetUserSeiges(Credential credential, string email, BigInteger maximum)
        {
            User user = RW.FindUser(email);
            BigInteger tot = RW.NumUserSiegeHistory(user);
            BigInteger num = (tot > maximum) ? maximum : tot;
            return NuTP.RespDataWithCode(NuTP.SysDom, NuTP.Code.ServiceUnavailable);
        }

        /** All the completed wars related with particular user with "maximum" recent items. 
         * 
         * 获取某城市已完成战役列表（最近的maximum个）
         * 
         * 
         * Return: Array of War Object
        */
        private static byte[] GetCitySeiges(Credential credential, BigInteger serverId, BigInteger cityId, BigInteger maximum)
        {
            byte[] siegesData = RW.FindSiegesByCityHistory(serverId, cityId, maximum);
            return NuTP.RespDataSucWithBody(siegesData);
        }


        /** User starts a war against a city. If the city is empty now, it's gonna occupy it directly.
		 * 参战某城市, 
         * Return: An object of War. If failed to create, return null
        */
        /** User starts a war against a city. If the city is empty now, it's gonna occupy it directly.
         * 参战某城市, 
         * Return: An object of War. If failed to create, return null
        */
        public static byte[] StartSiege(Credential credential, BigInteger serverId, BigInteger cityId, byte[] cardIds)
        {
            if (Global.VerifyUser(credential))
            {
                if (cityId >= Const.numCities)
                {
                    return NuTP.RespDataWithCode(ErrCate.City, ErrType.NotExist);
                }
                else
                {   //City does exist
                    City city = RW.FindCity(serverId, cityId);
                    BigInteger status = RW.GetStatusCity(city);
                    if (status == StatusCity.Sieging)
                    {
                        return NuTP.RespDataWithCode(ErrCate.City, ErrType.Duplicated);
                    }
                    else
                    {
                        User user = RW.FindUser(credential.email);
                        if (user.warID != Op.Void())
                        {  //User does not allow to participate into multiple wars at the same time
                            return NuTP.RespDataWithCode(ErrCate.War, ErrType.Duplicated);
                        }
                        else
                        {
                            CarryBattleSC.Card[] cards = RW.Table2Cards(cardIds);
                            if (cards.Length != Const.numCardsSiege)
                            {  //Format of card array must be wrong
                                return NuTP.RespDataWithCode(NuTP.SysDom, NuTP.Code.BadRequest);
                            }

                            for (int i = 0; i < Const.numCardsSiege; i++)
                            {
                                if (cards[i].ownerEmail != user.email)
                                {
                                    return NuTP.RespDataWithCode(ErrCate.Card, ErrType.NotExist);
                                }
                            }

                            Siege siege = RW.CreateSiege(serverId, city, user, cards, Blockchain.GetHeight());

                            return NuTP.RespDataSucWithBody(RW.Seige2Bytes(siege));
                        }

                    }
                }

            }
            else
            {
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.AuthFail);
            }
        }

        /** User (of email) claims the prizes
         * email对应用户领取占领奖励
         * Return: An object of War. If failed to create, return null
        */
        public static byte[] Claim(Credential credential, string email)
        {
            if (Global.VerifyUser(credential))
            {
                return NuTP.RespDataWithCode(NuTP.SysDom, NuTP.Code.ServiceUnavailable);
            }
            else
            {
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.AuthFail);
            }
        }

        /** Forge a card with some score. With higher score, it may get higher chance make it better
        */
        public static byte[] Forge(Credential credential, BigInteger cardId, BigInteger score)
        {
            if (Global.VerifyUser(credential))
            {
                return NuTP.RespDataWithCode(NuTP.SysDom, NuTP.Code.ServiceUnavailable);
            }
            else
            {
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.AuthFail);
            }
        }


        #region Priviledge Operations
        public static byte[] Genesis(BigInteger serverID)
        {
            if (Runtime.CheckWitness(Owner))
            {
                Alg.GenesisCreateCities(serverID);
                //RW.GenesisCreateFake
                return NuTP.RespDataSuccess();
            }
            else
            {
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.AuthFail);
            }

        }


        public static byte[] SetWeather(byte[] weathers)
        {
            if (Runtime.CheckWitness(Owner))
            {
                RW.SaveWeathers(weathers);
                return NuTP.RespDataSuccess();
            }
            else
            {
                return NuTP.RespDataWithCode(NuTP.SysDom, NuTP.Code.Forbidden);
            }
        }

        // <emails> = [S<email>*]
        //<FakeUsers> = [S<user>*]
        private static byte[] FakeUsers(BigInteger serverId, byte[] emails)
        {
            byte[] body = Op.Void();

            if (Runtime.CheckWitness(Owner))
            {
                int numEmails = emails.SizeTable();
                for (int i = 0; i < numEmails; i++)
                {
                    string email = NuSD.SplitTblStr(emails, i);
                    User user = new User
                    {
                        address = Op.Void(),
                        email = email,
                        pswHash = Op.Void(),
                        nickName = "Fake",
                        icon = 0,
                        serverID = serverId,
                        warID = Op.Void(),
                        city = Const.numCities
                    };
                    user.cards = GenerateRandomCards(user, 10);
                    RW.SaveUser(user);
                    body.AddSeg(RW.User2Bytes(user));
                }

                return NuTP.RespDataSucWithBody(body);
            }
            else
            {
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.AuthFail);
            }
        }

        private static byte[] FakeOccupies(BigInteger serverId, string email, BigInteger cityID)
        {
            if (Runtime.CheckWitness(Owner))
            {
                if (cityID > Const.numCities || cityID <= 0)
                {
                    return NuTP.RespDataWithCode(ErrCate.City, ErrType.NotExist);
                }
                else
                {
                    User user = RW.FindUser(email);
                    if (user != null)
                    {
                        City city = RW.FindCity(serverId, cityID);
                        if (city.ownerID.Length == 0 || city.ownerID.Length == 1)    //something wrong with bytes2string/string2bytes
                        {   //not work when it's not occupied
                            city.ownerID = email;
                            RW.SaveCity(serverId, city);

                            user.city = cityID;
                            RW.SaveUser(user);
                            return NuTP.RespDataSuccess();
                        }
                        else
                        {
                            return NuTP.RespDataWithCode(ErrCate.City, ErrType.Duplicated);
                        }

                    }
                    else
                    {
                        return NuTP.RespDataWithCode(ErrCate.User, ErrType.NotExist);
                    }
                }

            }
            else
            {
                return NuTP.RespDataWithCode(ErrCate.Account, ErrType.AuthFail);
            }
        }
        #endregion

        #region Randomness

        /// <summary>
        /// 给指定用户产生随机初始卡
        /// 返回实际生成的卡牌数量
        /// </summary>
        public static Card[] GenerateRandomCards(User user, int num)
        {
            Card[] cards = new Card[num];
            int numCardsAlready = 0;
            if (user.cards != null && user.cards.Length > 0)
            {
                numCardsAlready = user.cards.Length;
            }

            byte[] dHeight = Op.BigInt2Bytes(Blockchain.GetHeight());
            byte[] salt = Rand();
            for (int i = 0; i < num; i++)
            {
                Card cardResult = new Card();
                byte[] dEmail = Op.String2Bytes(user.email);
                byte[] dNum = Op.BigInt2Bytes(i + numCardsAlready);

                cardResult.cardID = Random(Op.JoinByteArray(dEmail, dNum, dHeight), 10);

                cardResult.type = Op.Bytes2BigInt(Hash160(cardResult.cardID)) % TypeArmy.TypeCount;
                cardResult.lvls = Random(salt, Const.numCellsOfCard);

                cardResult.ownerEmail = user.email;
                cardResult.warPos = 0;
                cards[i] = cardResult;
                RW.SaveCard(cardResult);

            }

            return cards;
        }

       

        public static Credential GenCred(string email, byte[] address, string psw)
        {
            Credential credential = new Credential();
            credential.email = email;
            credential.address = address;
            credential.psw = psw;
            return credential;
        }

        public static byte[] Cred2Bytes(Credential credential)
        {
            return NuSD.SegString(credential.email)
                     .AddSeg(credential.address)
                     .AddSeg(Hash256(Op.String2Bytes(credential.psw)));
        }
        #endregion
    }
}
