using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Helper = Neo.SmartContract.Framework.Helper;

using System.ComponentModel;
using System.Numerics;


namespace CarryBattle
{
    

	public class CarryBattle : SmartContract
    {
		/** The Global Constants */
		public static class Const
        {
            public const char Sspliter = '\n'

			/** Pending Usage */
			public const int LenPlayer = 20;      
			/** Pending Usage */
			public const int NumCardsPerPlayer = 10;
			/** The border length of every card */
			public const int SizeCard = 4;
			/** The number of card sent for new user registeration */
			public const int NumRegisterCard = 10;
			/** If a card is not participating any war, in its related CardAct, put warPos = NowarPos */
			public const byte NowarPos = 255;
            
			/** Pending Usage */
            public const string NumCardLive = "nC";
			/** Pending Usage */
            public const string NumCardDead = "nc";
			/** Pending Usage */
            public const string NumWarLive = "nW";
			/** Pending Usage */
            public const string NumWarDead = "nw";

			/** Prefix of cards in Storage */
            public const string PxCard = "C";
			/** Prefix of live cards in Storage */
            public const string PxCardAct = "CA";
			/** Prefix of wars in Storage */
            public const string PxWar = "W";
			/** Prefix of users in Storage */
			public const string PxUser = "U";
			/** Prefix of live wars in Storage */
            public const string PxWarAct = "WA";
			/** Prefex of all owners */
            public const string PxOwner = "O";
        }

		/** The result state of Storage.Put Operation */
        public static class State
        {
			/** The Put action created a new item */
            public const byte Create = 0;
			/** The Put action updated an existing item */
            public const byte Update = 1;
			/** The Put action deleted an existing item */
            public const byte Delete = 2;
			/** The Put action did no effect */
            public const byte Unchanged = 3;
			/** The Put action is aborted for some reason*/
			public const byte Abort = 4;
			/** The Put action is invalid */
            public const byte Invalid = 99;
        }

		/** The Utilities */
		public static class Utils
		{
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

			//Serialize function is not supported by Unity. Use the customized one
			public static byte[] Serialize(Card card)
            {
				//byte type
				//byte[] lvls
				byte[] prtID1 = card.prtID1.ToByteArray();
				byte[] prtID2 = card.prtID2.ToByteArray();
				byte[] creator = card.creator.ToByteArray();
				byte[] owner = card.owner.ToByteArray();
				byte[] score = card.score.ToByteArray();
				byte[] wins = card.wins.ToByteArray();
				byte[] wars = card.wars.ToByteArray();

				int length = 1 + 
					Const.SizeCard* Const.SizeCard + 
				         prtID1.Length + 
				         prtID2.Length + 
				         creator.Length + 
				         owner.Length +

				byte[] c = new byte[];
            }

            public static Card Deserialize(byte[] data)
            {

            }

            public static byte[] Serialize(CardAct cardact)
            {

            }

            public static Card Deserialize(byte[] data)
            {

            }

			#endregion
		}
        
		public class CBInteger{
			private 
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
			/** Type of the Army */
            public byte type;  
			/** lvls includes SizeCard * SizeCard bytes. Holding the levels of each grid. Value 0 stands for empty grid.*/
            public byte[] lvls;    // Range: 0 - 255
			/** The parent card 1 */
			public BigInteger prtID1;
			/** The parent card 2 */
			public BigInteger prtID2;
			/** The creator user address of this card*/
			public BigInteger creator;
			/** The current owner address of this card */
			public BigInteger owner;
			/** The score of this card */
            public BigInteger score;
			/** The win wars of this card */
            public BigInteger wins;
			/** The number of wars of this card*/
            public BigInteger wars;

        }



        [Serializable]
        public class War
        {
			/** The blockchain height of the finalization of this war */
            public byte[] endHeight;
			/** The creator of this war */
			public BigInteger creator;         
        }

		/** CardAct belongs to another "list" in storage other than the Cards. 
		 * CardAct的设计存在是为了缩减在所有卡堆里搜索的时间。
		 * 具体而言，
		 * 1. 所有现存以及死去（被合并）的卡牌都以Card结构存在以PxCard为前缀的Storage里，而现存的Card同时以CardAct结构存在以PxCardAct为前缀的Storage里
		 * 2. 当协约函数需要遍历寻找一张现存的Card时，只需要在PxCardAct里搜索，然后通过cID字段直接定位到PxCard里一张牌的信息。
		 * 3. 当一张卡片死去时，将其从PxCardAct里置空即可。
		 * 4. 当新卡牌生成时，要在PxCard里以最高序号+1加入;同时在PxCardAct里在第一个空序号插入。
		 
         */
		[Serializable]
        public class CardAct
        {
			/** The index of the Card in PxCard */
            public BigInteger cID;
			/** The index of the war that the card is participating now */
            public BigInteger warID;
			/** The position of the card in the war it's participating */
            public byte warPos;

        }

		[Serializable]
		public class User{
			/** The address of the user */
			public byte[] address;
			/** The arbitary name of the user*/
			public byte[] name;
			/** The war the player is participating */
			public BigInteger warID;
		}

		[Serializable]
		public class Global{
			/** Total number of the cards */
			public BigInteger numCards;
			/** The highest index of the PxCardAct */
			public BigInteger highestLiveCardId;
			/** Total number of the wars */
			public BigInteger numWars;
			/** The highest index of the PxWarAct */
			public BigInteger highestLiveWarId;
			/** Total number of users */
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
				// Register new user, will generate a few cards thereof
				if (operation == "userRegister")
                {

                    return UserRegister(args);
                }
				// Rename a new user
				if (operation == "userRename")
                {
					return UserRename(args);
               
                }
				// Merage two cards.
				if (operation == "cardMerge")
                {
                    return CardMerge(args);
                }
				// Change the owner of a card
                if (operation == "cardTransfer")
                {
                    return CardTransfer(args);
                }
				// Start a war
                if (operation == "warStart")
                {
					return WarStart(args);
                }
				// Join a war
                if (operation == "warJoin")
                {
					return WarJoin(args);

                }
				// Retreat from a war
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
			return Utils.SetStorageWithKey(Const.PxUser + id.ToString(), UserToBytes(user));
        }


		/**  */
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
				global.numUsers = BigInteger.Add(global.numUsers,1);
				global = SyncGlobal(global);

                //Put new user into storage
				User user = new User();
				user.address = from;
				user.name = (byte[])args[1];
				Utils.SetStorageWithKey(Const.PxUser + newUserId.ToString(), UserToBytes(user));

                //Generate new user some basic cards
				int seed = Blockchain.GetHeader(Blockchain.GetHeight()).GetHashCode();

				for (int i = 0; i < Const.NumRegisterCard;i++){
					Card card = GenCard(newUserId,seed+i);
					BigInteger id = BigInteger.Add(global.numCards, i);
					SetCard(id, card);
				}

				global.numCards = BigInteger.Add(global.numCards, Const.NumRegisterCard);
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
			for (BigInteger i = 1; i < GetGlobal().numUsers; i++){
				if(GetUserById(i).address == addr){
					return i;
				}
			}
			return 0;
        }

		public static User GetUserByAddr(byte[] addr){
			for (BigInteger i = 1; i < GetGlobal().numUsers; i++)
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
			return Utils.GetStorageWithKey(Const.PxUser + uid.ToString());
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
			return Utils.SetStorageWithKey(Const.PxCard + cid.ToString(), CardToBytes(card));
		}

		private static Card GenCard(BigInteger owner, int seed)
        {
            Card card = new Card();
            /*
              TBD: The algorithm to generate card. Below is the temporary implementation
            */

            card.type = Convert.ToByte(seed % 3);
            card.creator = owner;
            int grids = Const.SizeCard * Const.SizeCard;
            byte[] levels = new byte[grids];
            for (int i = 0; i < grids; i++)
            {
                levels[i] = Convert.ToByte((seed << i) % 2);
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
			if( !force){//If (force == true), the caller should insure that the cID is valid and corresponding card exists
				Card card = GetCardById(cID);
				if (card == null){
					return State.Invalid;
				}
			}


			Global global = GetGlobal();
			BigInteger insertAId = global.highestLiveCardId;

			for (BigInteger i = 1; i < global.highestLiveCardId; i++){
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

			Utils.SetStorageWithKey(Const.PxCardAct + insertAId.ToString(), CardActToBytes(cardAct));

			if(insertAId == global.highestLiveCardId ){
				global.highestLiveCardId = BigInteger.Add(global.highestLiveCardId , 1);
				SetGlobal(global);
				return State.Create;
			}
			else{
				return State.Update;
			}
		}
        
		private static byte DiscardCard(BigInteger cardID){
			Global global = GetGlobal();
			for (BigInteger i = 1; i < global.highestLiveCardId; i++)
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
			return Utils.SetStorageWithKey(Const.PxCardAct + caId.ToString(), CardActToBytes(cardAct));
		}

		public static byte[] GetCardRawById(BigInteger cId)
        {
			return Utils.GetStorageWithKey(Const.PxCard + cId.ToString());
        }

    

		public static Card GetCardById(BigInteger cid){
			return BytesToCard(GetCardRawById(cid));
		}



		public static byte[] GetCardActRawById(BigInteger caId){
			return Utils.GetStorageWithKey(Const.PxCardAct + caId.ToString());
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

                for (BigInteger i = 0; i < GetHighestCardOrder(); i++)
                {
                    byte[] cardRaw = Utils.GetStorageWithKeyPath("pC", i.ToString(), "c");

                }
				return false;
            }

        }

        public static Object CardTransfer(params object[] args)
        {
            return false;
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
