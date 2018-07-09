using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Helper = Neo.SmartContract.Framework.Helper;

using System.ComponentModel;
using System.Numerics;

/*
 * 1. When converting Int -> String, don't use ToString()!, use "" + int 
 * 2. BigInteger.Add not supported!, use `+`
 * 3. BigInteger: i++ not supported! use i+=1
 * 4. byte[]: can't init with: byte[] array = new byte[size], use: byte[] array = {0,0,0}
 * */


namespace CarryBattle
{
    

	public class CarryBattle : SmartContract
    {
		/** The Global Constants */
		public static class Const
        {
            public const int LenPlayer = 20;
			public const int NumCardsPerPlayer = 10;
			public const int SizeCard = 4;
			public const int NumRegisterCard = 10;
			public const byte NowarPos = 255;


            public const string NumCardLive = "nC";
            public const string NumCardDead = "nc";
            public const string NumWarLive = "nW";
            public const string NumWarDead = "nw";

            public const string PxCard = "C";
            public const string PxCardAct = "CA";
            public const string PxWar = "W";
			public const string PxUser = "U";

            public const string PxWarDead = "pw";
            public const string PxOwner = "po";
        }

		/** The result state of Storage.Put Operation */
        public static class State
        {
            public const byte Create = 0;
            public const byte Update = 1;
            public const byte Delete = 2;
            public const byte Unchanged = 3;
			public const byte Abort = 4;
            public const byte Invalid = 99;
        }

		/** The Utilities */
		public static class Utils
		{
            #region Helpers

            public static byte[] CreateCardInBytes() 
            {
                // Const.SizeCard = 4, 4*4=16
                // HACK: can't use new byte[16]
                byte[] array = {
                    0,0,0,0,
                    0,0,0,0,
                    0,0,0,0,
                    0,0,0,0,
                };
                return array;
            }
            #endregion

            #region Utils Storage
            /* ===========================================================
            * Storage functions are designed to support multi-segment keys
            * Eg. {Key = "seg1.seg2.seg3", Value = "someValue"}
            ==============================================================*/

            public static byte[] KeyPath(params string[] elements)
			{
				if (elements.Length == 0)
				{
					return new byte[0];
				}
				else
				{
					byte[] r = elements[0].AsByteArray();
					for (int i = 1; i < elements.Length; i++)
					{
						r = r.Concat(elements[i].AsByteArray());
					}
					return r;
				}

				//return string.Join(Const.Splitter, elements);
			}

			public static byte[] KeyPath(byte[] splitter, params string[] elements)
			{
				if (elements.Length == 0)
				{
					return new byte[0];
				}
				else
				{
					byte[] r = elements[0].AsByteArray();
					for (int i = 1; i < elements.Length; i++)
					{
						r = r.Concat(splitter).Concat(elements[i].AsByteArray());
					}
					return r;
				}
			}
			public static byte[] GetStorageWithKeyPath(params string[] elements)
			{
				return GetStorageWithKey(KeyPath(elements));
			}
			public static byte[] GetStorageWithKey(byte[] key)
			{
				return Storage.Get(Storage.CurrentContext, key);
			}
			public static byte[] GetStorageWithKey(string key)
			{
				return Storage.Get(Storage.CurrentContext, key);
			}

			public static byte SetStorageWithKeyPath(byte[] value, params string[] elements)
			{
				return SetStorageWithKey(KeyPath(elements), value);
			}
			public static byte SetStorageWithKey(string key, byte[] value)
			{
				return SetStorageWithKey(key.AsByteArray(), value);
			}

			public static byte SetStorageWithKey(byte[] key, byte[] value)
			{
				byte[] orig = GetStorageWithKey(key);
				if (orig == value) { return State.Unchanged; }

				if (value.Length == 0)
				{
					Storage.Delete(Storage.CurrentContext, key);
					return State.Delete;

				}
				else
				{
					Storage.Put(Storage.CurrentContext, key, value);
					return (orig.Length == 0) ? State.Create : State.Update;
				}
			}

			#endregion
		}
        
        

        #region LogicClasses
		/** The types of army */
		public static class TypeArmy
        {
            public const byte Infantry = 0;
            public const byte Archer = 1;
            public const byte Cavalry = 2;
            public const byte Invalid = 4;
        }


        
        [Serializable]
        public class Card
        {
            /**
             * Intrinsic Properties
            */
            public byte type;   //TypeArmy
            public byte[] lvls;    // Range: 0 - 255
			public BigInteger prtID1; // parent id
			public BigInteger prtID2;
			public BigInteger creator;
			public BigInteger owner;
            public BigInteger score;
            public BigInteger wins;
            public BigInteger wars;

        }



        [Serializable]
        public class War
        {
            public byte[] endHeight;
			public BigInteger creator;         
        }


		[Serializable]
        public class CardAct
        {
            public BigInteger cID;
            public BigInteger warID;
            public byte warPos; // order in the 10 list

        }

		[Serializable]
		public class User{
			public byte[] address;
			public byte[] name;
			public BigInteger warID;
		}

		[Serializable]
		public class Global{
			public BigInteger numCards; // total number
			public BigInteger highestLiveCardId; 
			public BigInteger numWars;
			public BigInteger highestLiveWarId;
			public BigInteger numUsers;
		}

        #endregion

		#region Entrance
		public static readonly byte[] Owner = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y".ToScriptHash();

		public static object Main(string operation, params object[] args)
        {
			if (Runtime.Trigger == TriggerType.Verification){
				if(Owner.Length == 20){
					return Runtime.CheckWitness(Owner);
				}
				else if ( Owner.Length == 33){
					byte[] signature = operation.AsByteArray();
                    return VerifySignature(signature, Owner);
				}
				return true;
			}
			else if(Runtime.Trigger == TriggerType.Application){
				if (operation == "userRegister")
                {// Register new user, will generate a few cards thereof

                    return UserRegister(args);
                }

				if (operation == "userRename")
                {
					return UserRename(args);
               
                }

				if (operation == "cardMerge")
                {
                    return CardMerge(args);
                }
                if (operation == "cardTransfer")
                {
                    return CardTransfer(args);
                }

                if (operation == "warStart")
                {
					return WarStart(args);
                }
                if (operation == "warJoin")
                {
					return WarJoin(args);

                }
                if (operation == "warRetreat")
                {
					return WarRetreat(args);

                }

                



                if (operation == "getNumUsers")
                {
					return GetNumUsers();
                }
                if (operation == "getNumCards")
                {
					return GetNumCards();
                }

				if (operation == "getCardInfo")
                {
					return true;
                }
				if(operation == "getUserIDByAddr"){
					if (args.Length < 1) return 0;
					byte[] addr = (byte[])args[0];
					return GetUserIDByAddr(addr);
				}



                byte[] r = new byte[1];

                return r;
			}
			else{   //Will elabrate for other cases
				return false;
			}

        }
		#endregion







        #region User

		/* =================================================
         * User Functions
         ====================================================*/

		private static User BytesToUser(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return null;
            }
            else
            {
                object[] objs = (object[])Helper.Deserialize(bytes);
                return (User)(object)objs;
            }
        }

        private static byte[] UserToBytes(User user)
        {
            return Helper.Serialize(user);
        }

        private static byte SetUser(BigInteger id, User user)
        {
            string idString = "" + id;
            return Utils.SetStorageWithKey(Const.PxUser + idString, UserToBytes(user));
        }

		public static bool UserRegister(params object[] args){
			if (args.Length < 2) return false;

			byte[] from = (byte[])args[0];
			if(!Runtime.CheckWitness(from)){
				return false;
			}
			else{
				if (GetUserIDByAddr(from)!=0){
					//User already registered
					return false;
				}
				//If logic goes here, this is a new user. 

				//Update global.
				Global global = GetGlobal();
				BigInteger newUserId = global.numUsers;
				global.numUsers += 1;
				global = SyncGlobal(global);

                //Put new user into storage
				User user = new User();
				user.address = from;
				user.name = (byte[])args[1];
                string idString = "" + newUserId;
                Utils.SetStorageWithKey(Const.PxUser + idString, UserToBytes(user));

                //Generate new user some basic cards
                // TODO: use hash(byte []) instead???
                uint timestamp = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
                int seed = (int)timestamp;

				for (int i = 0; i < Const.NumRegisterCard;i++){
					Card card = GenCard(newUserId,seed+i);
					BigInteger id = global.numCards + i;
					SetCard(id, card);
				}

				global.numCards += Const.NumRegisterCard;
				SetGlobal(global);
				return true;
			}
		}

		public static bool UserRename(params object[] args){
			if (args.Length < 2) return false;
			byte[] from = (byte[])args[0];
			if (!Runtime.CheckWitness(from))
            {
                return false;
            }
			else{
				BigInteger id = GetUserIDByAddr(from);
				User user = GetUserById(id);
				user.name = (byte[])args[1];
				SetUser(id,user);
				return true;
			}
		}      


        

		public static BigInteger GetUserIDByAddr(byte[] addr)
        {
			for (BigInteger i = 1; i < GetGlobal().numUsers; i+=1){
				if(GetUserById(i).address == addr){
					return i;
				}
			}
			return 0;
        }

		public static User GetUserByAddr(byte[] addr){
			for (BigInteger i = 1; i < GetGlobal().numUsers; i+=1)
            {
				User u = GetUserById(i);
                if (u.address == addr)
                {
                    return u;
                }
            }
			return null; 
		}

		public static byte[] GetUserRawById(BigInteger uid)
        {
            string idString = "" + uid;
            return Utils.GetStorageWithKey(Const.PxUser + idString);
        }

		public static User GetUserById(BigInteger uid){
			return BytesToUser(GetUserRawById(uid));
		}



        #endregion

        #region Card
		/* =================================================
         * Card Related Functions
         ====================================================*/



		private static Card BytesToCard(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return null;
            }
            else
            {
                object[] objs = (object[])Helper.Deserialize(bytes);
                return (Card)(object)objs;
            }
        }

        private static byte[] CardToBytes(Card card)
        {
            return Helper.Serialize(card);
        }

		private static byte SetCard(BigInteger cid, Card card){
            string idString = "" + cid;
            return Utils.SetStorageWithKey(Const.PxCard + idString, CardToBytes(card));
		}

		private static Card GenCard(BigInteger owner, int seed)
        {
            Card card = new Card();
            /*
              TBD: The algorithm to generate card. Below is the temporary implementation
            */

            card.type = (byte)(seed % 3);
            card.creator = owner;
            int grids = Const.SizeCard * Const.SizeCard;
            byte[] levels = Utils.CreateCardInBytes();
            for (int i = 0; i < grids; i++)
            {
                levels[i] = (byte)((seed << i) % 2);
            }
            card.lvls = levels;
            return card;

        }

		private static CardAct BytesToCardAct(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return null;
            }
            else
            {
                object[] objs = (object[])Helper.Deserialize(bytes);
				return (CardAct)(object)objs;
            }
        }

		private static byte[] CardActToBytes(CardAct cardAct)
        {
			return Helper.Serialize(cardAct);
        }

		      

		private static byte ActivateCard(BigInteger cID, bool force = false) {
			if (!force) {//If (force == true), the caller should insure that the cID is valid and corresponding card exists
				Card card = GetCardById(cID);
				if (card == null){
					return State.Invalid;
				}
			}


			Global global = GetGlobal();
			BigInteger insertAId = global.highestLiveCardId;

			for (BigInteger i = 1; i < global.highestLiveCardId; i+=1){
				CardAct ca = GetCardActById(i);
				if (ca == null ){
					if (insertAId == 0){    //Record the 1st empty one
						insertAId = i;
					}
				}
				else{
					if (ca.cID == 0){
						if (insertAId == 0)
                        {    //Record the 1st empty one
                            insertAId = i;
                        }
					}
					else if(ca.cID == cID){// act already exist. Abort activating...
						return State.Abort;
					}
				}
			}

			CardAct cardAct = new CardAct();
			cardAct.cID = cID;

            string idString = "" + insertAId;
            Utils.SetStorageWithKey(Const.PxCardAct + idString, CardActToBytes(cardAct));

			if(insertAId == global.highestLiveCardId ){
                global.highestLiveCardId += 1;
				SetGlobal(global);
				return State.Create;
			}
			else{
				return State.Update;
			}
		}
        
		private static byte DiscardCard(BigInteger cardID){
			Global global = GetGlobal();
			for (BigInteger i = 1; i < global.highestLiveCardId; i+=1)
            {
                CardAct ca = GetCardActById(i);
                if (ca != null)
                {
					if (ca.cID == cardID){
						return SetCardAct(i, null);
					}
                }
                
            }
			return State.Abort;
		}

		private static byte SetCardAct(BigInteger caId, CardAct cardAct){
            string idString = "" + caId;
            return Utils.SetStorageWithKey(Const.PxCardAct + idString, CardActToBytes(cardAct));
		}

		public static byte[] GetCardRawById(BigInteger cId)
        {
            string idString = "" + cId;
            return Utils.GetStorageWithKey(Const.PxCard + idString);
        }

    

		public static Card GetCardById(BigInteger cid){
			return BytesToCard(GetCardRawById(cid));
		}



		public static byte[] GetCardActRawById(BigInteger caId){
            string idString = "" + caId;
            return Utils.GetStorageWithKey(Const.PxCardAct + idString);
		}

		public static CardAct GetCardActById(BigInteger caId)
        {
			return BytesToCardAct(GetCardActRawById(caId));
        }


		public static Object CardMerge(params object[] args)
        {
            if (args.Length < 3) return false;

            byte[] from = (byte[])args[0];
            if (!Runtime.CheckWitness(from))
            {
                return false;
            }
            else
            {
                BigInteger parent1 = (BigInteger)args[1];
                BigInteger parent2 = (BigInteger)args[2];

                bool isP1Auth = false;
                bool isP2Auth = false;

                BigInteger numCards = GetNumCards();

                for (BigInteger i = 0; i < GetHighestCardOrder(); i+=1)
                {
                    string idString = "" + i;
                    byte[] cardRaw = Utils.GetStorageWithKeyPath("pC", idString, "c");

                }
				return true;
            }

        }

        /*
         * first argu: Card current owner id: BigInteger
         * second argu: Card receiver id: BigInteger
         * third argu: Card id to be transfered: BigInteger
         * */
        public static Object CardTransfer(params object[] args)
        {
            if (args.Length < 3) return false;

            BigInteger currentOwnerId = (BigInteger)args[0];
            BigInteger receiverId = (BigInteger)args[1];
            BigInteger cardId = (BigInteger)args[2];

            User currentOwner = GetUserById(currentOwnerId);
            User receiver = GetUserById(receiverId);
            Card card = GetCardById(cardId);

            // check input validity
            if (currentOwner != null && receiver != null & card != null) 
            {
                // check if its legal to transfer
                if (card.owner == currentOwnerId)
                {
                    card.owner = receiverId;
                    return true;
                }
                else
                {
                    return false;
                }
            } else 
            {
                // input not valid
                return false;
            }
        }

        #endregion

        #region War
		/* =================================================
         * War Related Functions
         ====================================================*/

		public static Object WarStart(params object[] args){
			return false;
		}

		public static Object WarJoin(params object[] args)
        {
            return false;
        }

		public static Object WarRetreat(params object[] args)
        {
            return false;
        }
        #endregion


        #region Global
		/* =================================================
         * Global Related Functions
         ====================================================*/

		public static Global GetGlobal(){
			byte[] raw = Utils.GetStorageWithKey("G");
			if(raw.Length == 0){
				Global g = new Global();
				SetGlobal(g);
				return g;
        
			}
			else{
				object[] objs = (object[])Helper.Deserialize(raw);
				return (Global)(object)objs;
			}
		}

		public static byte SetGlobal(Global global){
			return Utils.SetStorageWithKey("G",Helper.Serialize(global));
		}

		public static Global SyncGlobal(Global global){
			Utils.SetStorageWithKey("G", Helper.Serialize(global));
			return global;
		}

		public static BigInteger GetNumUsers()
        {
			return GetGlobal().numUsers;
        }

        public static BigInteger GetNumCards()
        {
			return GetGlobal().numCards;
        }

        public static BigInteger GetHighestCardOrder()
        {
			return GetGlobal().highestLiveCardId;
        }
        #endregion
		/* ================
        * Basic Get Information
        ===================*/



		public static byte[] GetCardRaw(byte[] cardID){
			return Storage.Get(Storage.CurrentContext, cardID);
		}

		public static byte[] GetWarRaw(byte[] warID)
        {
            return Storage.Get(Storage.CurrentContext, warID);
        }
        
		public static byte[] GetCardsByUserId(byte[] playerId){
			
			//byte[] cardsID = GetCardRaw()
			byte[] r = new byte[1];
            return r; 
		}




		//public static byte[] OpenRoom(byte[] playerID, BigInteger[] cardIDs)
  //      {
            

  //      }

		//public static byte[] JoinRoom(byte[] playerID, BigInteger roomID, BigInteger[] cardIDs)
        //{

        //}



		private static Card _GetCard(byte[] cardID){
			byte[] raw = GetCardRaw(cardID);
			if(raw.Length == 0){
				return null;
			}
			else{
				object[] objs = (object[])Helper.Deserialize(raw);
				return (Card)(object)objs;

			}
		}



      
		private static War _GetWar(byte[] warID){
			byte[] raw = GetWarRaw(warID);
            if (raw.Length == 0){
                return null;
            }
            else{
                object[] objs = (object[])Helper.Deserialize(raw);
				return (War)(object)objs;
            }
        }
        
        


    }
}
