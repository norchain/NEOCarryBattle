using System;
using System.Numerics;
using Neunity.Tools;

#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
using Org.BouncyCastle.Asn1.Ocsp;
#endif



namespace CarryBattleSC {
    public class CarryBattleSC : SmartContract {

        public static readonly byte[] Owner = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y".ToScriptHash();

        public static object Main(string operation, params object[] args) {
            if(operation == "test") {
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
            if(Runtime.Trigger == TriggerType.Verification) {
                if(Owner.Length == 20) {
                    return Runtime.CheckWitness(Owner);
                } else if(Owner.Length == 33) {
                    byte[] signature = Op.String2Bytes(operation);
                    return VerifySignature(signature, Owner);
                }
                return true;
            } else if(Runtime.Trigger == TriggerType.Application) {
                if(operation == "genesis") {
                    return Genesis();
                } else if(operation == "fakeUsers") {
                    BigInteger serverId = (BigInteger)args[0];
                    byte[] emails = (byte[])args[1];
                    return FakeUsers(serverId, emails);
                } else if(operation == "fakeOccupies") {
                    BigInteger serverId = (BigInteger)args[0];
                    string email = (string)args[1];
                    BigInteger cityId = (BigInteger)args[2];
                    return FakeOccupies(serverId, email, cityId);
                } else if(operation == "setWeather") {
                    byte[] weathers = (byte[])args[0];
                    return SetWeather(weathers);
                } else {
                    string email = (string)args[0];
                    string psw = (string)args[1];
                    byte[] address = (byte[])args[2];

                    Credential credential = Global.GenCred(email, address, psw);

                    if(operation == Const.operationRegister) {
                        BigInteger serverId = (BigInteger)args[3];
                        return Register(credential, serverId);
                    }

                    if(operation == Const.operationRegCards) {
                        BigInteger num = (BigInteger)args[3];
                        return RegCards(credential, (int)num);
                    }

                    if(operation == Const.operationMigrate) {
                        BigInteger newServerId = (BigInteger)args[3];
                        return Migrate(credential, newServerId);
                    }

                    if(operation == Const.operationGetUsers) {
                        byte[] emails = (byte[])args[3];
                        return GetUsers(credential, emails);
                    }

                    if(operation == Const.operationgetCities) {
                        BigInteger serverId = (BigInteger)args[3];
                        return GetCities(credential, serverId);

                    }

                    if(operation == Const.operationGetUserSeiges) {
                        string userEmail = (string)args[3];
                        BigInteger max = (BigInteger)args[4];
                        return GetUserSeiges(credential, userEmail, max);

                    }

                    if(operation == Const.operationGetCitySeiges) {
                        BigInteger serverId = (BigInteger)args[3];
                        BigInteger cityId = (BigInteger)args[4];
                        BigInteger max = (BigInteger)args[5];
                        return GetCitySeiges(credential, serverId, cityId, max);

                    }

                    if(operation == Const.operationSeige) {
                        BigInteger serverId = (BigInteger)args[3];
                        BigInteger cityId = (BigInteger)args[4];
                        byte[] cardIds = (byte[])args[5];
                        return Seige(credential, serverId, cityId, cardIds);
                    }

                    if(operation == Const.operationClain) {
                        string userEmail = (string)args[3];
                        return Claim(credential, userEmail);
                    }
                    if(operation == Const.operationForge) {
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
         * Return: Array of the Card objects owned by the user 

        */
        private static byte[] Register(Credential credential, BigInteger serverId) {
            byte[] userData = RW.FindDataUser(credential.email);
            if(userData.Length != 0) {   //Exists
                return NuTP.DomCode(ErrCate.Account, ErrType.Duplicated);

            } else {
                byte[] psdHash = Funcs.Hash(Op.String2Bytes(credential.psw));
                User user = new User();
                user.address = credential.address;
                user.email = credential.email;
                user.pswHash = psdHash;

                user.serverID = serverId;
                user.city = Const.numCities;
                //user.cardIDs = new byte[0];

                RW.SaveUser(user);
                return NuTP.Success;
            }
        }

        /* * NuTP:

         <getUsers> =  [S<header>,
                       ,S[([S<user>,
                            S[S<card>^10]] 
                       )^]
         */
        private static byte[] RegCards(Credential credential, int num) {
            byte[] userData = RW.FindDataUser(credential.email);
            if(userData.Length == 0) {   //Account Not exist
                return NuTP.DomCode(ErrCate.Account, ErrType.NotExist);
            } else {   //Account does exist
                
                User user = RW.Bytes2User(userData);
                int numAlready = RW.NumCardsOfUser(user);
                int numPending = Const.numCardsTotalReg - numAlready;
                if( numPending < 0 ){
                    return NuTP.DomCode(ErrCate.Card, ErrType.Duplicated);
                }
                else{
                    Card[] cards = RW.GenerateRandomCards(user, (numPending < Const.numCardsPerReg)? numPending:Const.numCardsPerReg);
                    for (int i = numAlready, j=0 ; i < numAlready + cards.Length; i++, j++){
                        user.cards[i] = cards[j];
                    }
                    //更新玩家数据
                    RW.SaveUser(user);
                    NuTP.Response response = new NuTP.Response
                    {
                        header = new NuTP.Header
                        {
                            domain = NuTP.SysDom,
                            code = NuTP.Code.Success
                        },

                        body = RW.UserAndCards2Bytes(user)
                    };
                    return NuTP.Response2Bytes(response);
                }
            }
        }

        /** Get basic information of a list of users
         * 
         *  获取一系列玩家信息, 内容包括所在服务器、所占城池、
         * 返回：该用户所有卡牌具体信息。若在该服务器上新账户，为用户新建若干卡牌。
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
         <emails> = S[S<email>^]
         
         <getUsers> =  [S<header>,
                       S[([S<user>,
                            S[S<card>^10]] 
                       )^]
         */

        private static byte[] GetUsers(Credential credential, byte[] emails) {
            // This method does not require credential

            int numEmails = emails.SizeTable();
            byte[] retBody = Op.Empty;

            for (int i = 0; i < numEmails; i ++) {
                string email = emails.SplitTblStr(i);
                User user = RW.FindUser(email);
                if (user!=null ){
                    retBody.AddSeg(RW.UserAndCards2Bytes(user));

                }
                else{
                    return NuTP.DomCodeWithDesp(ErrCate.User, ErrType.NotExist, Op.String2Bytes(email));
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
        private static byte[] Migrate(Credential credential, BigInteger serverId) {
            if(Global.VerifyUser(credential)) {
                return new byte[0];
            } else {
                return new byte[0];
            }
        }

        /** All cities in a server with "maximum" recent items. 
         * 
         * 获取城市状态列表
         * 
         * Return: Array of War Object
        */
        private static byte[] GetCities(Credential credential, BigInteger serverId) {
            return new byte[0];
        }

        /** All the completed wars related with particular user with "maximum" recent items. 
		 * 
		 * 获取某用户已完成战役列表（最近的maximum个）。返回：一组War的列表
		 * 
		 * 
		 * Return: Array of War Object
        */
        private static byte[] GetUserSeiges(Credential credential, string email, BigInteger maximum) {
            return new byte[0];
        }

        /** All the completed wars related with particular user with "maximum" recent items. 
         * 
         * 获取某城市已完成战役列表（最近的maximum个）
         * 
         * 
         * Return: Array of War Object
        */
        private static byte[] GetCitySeiges(Credential credential, BigInteger serverId, BigInteger cityId, BigInteger maximum) {
            return new byte[0];
        }


        /** User starts a war against a city. If the city is empty now, it's gonna occupy it directly.
		 * 参战某城市, 
         * Return: An object of War. If failed to create, return null
        */
        private static byte[] Seige(Credential credential, BigInteger serverId, BigInteger cityId, byte[] cardIds) {
            if(Global.VerifyUser(credential)) {
                return new byte[0];
            } else {
                return new byte[0];
            }
        }

        /** User (of email) claims the prizes
         * email对应用户领取占领奖励
         * Return: An object of War. If failed to create, return null
        */
        private static byte[] Claim(Credential credential, string email) {
            if(Global.VerifyUser(credential)) {
                return new byte[0];
            } else {
                return new byte[0];
            }
        }

        /** Forge a card with some score. With higher score, it may get higher chance make it better
        */
        private static byte[] Forge(Credential credential, BigInteger cardId, BigInteger score) {
            if(Global.VerifyUser(credential)) {
                return new byte[0];
            } else {
                return new byte[0];
            }
        }


        #region Priviledge Operations
        private static bool Genesis() {
            if(Runtime.CheckWitness(Owner)) {
                return true;
            } else {
                return false;
            }

        }


        private static bool SetWeather(byte[] weathers) {
            if(Runtime.CheckWitness(Owner)) {
                return true;
            } else {
                return false;
            }
        }

        private static byte[] FakeUsers(BigInteger serverId, byte[] emails) {
            if(Runtime.CheckWitness(Owner)) {
                return new byte[0];
            } else {
                return new byte[0];
            }
        }

        private static byte[] FakeOccupies(BigInteger serverId, string email, BigInteger city) {
            if(Runtime.CheckWitness(Owner)) {
                return new byte[0];
            } else {
                return new byte[0];
            }
        }
        #endregion
    }
}
