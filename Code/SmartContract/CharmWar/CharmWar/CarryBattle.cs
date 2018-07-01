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
        #region AssistClasses
		/** The Global Constants */
		public static class Const
        {
            public const int LenPlayer = 20;
			public const int NumCardsPerPlayer = 10;
			public const int SizeCard = 4;
			public const int NumRegisterCard = 10;


            public const string NumCardLive = "nC";
            public const string NumCardDead = "nc";
            public const string NumWarLive = "nW";
            public const string NumWarDead = "nw";

            public const string PxCardLive = "pC";
            public const string PxCardDead = "pc";
            public const string PxWarLive = "pW";
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
            public const byte Invalid = 4;
        }

        /** The Utilities */
        public static class Utils
        {
            
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
				if (elements.Length == 0 ){
					return new byte[0];
				}
				else{
					byte[] r = elements[0].AsByteArray();
					for (int i = 1; i < elements.Length; i++){
						r = r.Concat(splitter).Concat(elements[i].AsByteArray());
					}
					return r;
				}

				//return string.Join(Const.Splitter, elements);
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

            
        }
        
        #endregion

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
			public BigInteger prtID1;
			public BigInteger prtID2;
			public byte[] creatorID;


        }

		[Serializable]
		public class CardDynamic{
			/**
             * Dynamic Properties
            */
			public BigInteger cID;
            public byte[] ownerID;
            public BigInteger childID;
            public BigInteger score;
            public BigInteger wins;
            public BigInteger loss;
            public BigInteger warID;
		}

        [Serializable]
        public class War
        {
			/**
             * Intrinsic Properties
            */
            public byte[] endHeight;
			public byte[] creator;


        }

		[Serializable]
		public class WarDynamic{
			public BigInteger wID;

            public byte[] cardIDs;
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
				if (operation == "register")
                {// Register new user, will generate a few cards thereof

                    return Register(args);
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

                if (operation == "cardMerge")
                {
					return CardMerge(args);
                }
                if (operation == "cardTransfer")
                {
					return CardTransfer(args);
                }
                if (operation == "getNumPlayers")
                {
					return true;
                }
                if (operation == "getNumCards")
                {// Register new user, will generate a few cards thereof
					return GetNumCards();
                }

				if (operation == "getCardInfo")
                {// Register new user, will generate a few cards thereof
					return true;
                }

                byte[] r = new byte[1];

                return r;
			}
			else{   //Will elabrate for other cases
				return false;
			}

        }
		#endregion

		#region BasicInformation


		#endregion
        
		private static Card _GenCardWithOwner(byte[] owner,int seed){
			Card card = new Card();
            /*
              TBD: The algorithm to generate card  
            */

			card.type = Convert.ToByte(seed % 3);
			card.creatorID = owner;
			int grids = Const.SizeCard * Const.SizeCard;
			byte[] levels = new byte[grids];
			for (int i = 0; i < grids; i++){
				levels[i] = Convert.ToByte((seed << i) % 2);
			}
			card.lvls = levels;
			return card;
   
		}

		public static BigInteger GetNumPlayers(){
			return Utils.GetStorageWithKey("Gp").AsBigInteger();
		}

		public static BigInteger GetNumCards()
        {
            return Utils.GetStorageWithKey("GC").AsBigInteger();
        }

		public static BigInteger GetHighestCardOrder()
        {
            return Utils.GetStorageWithKey("Gc").AsBigInteger();
        }

		public static byte[] PlayerAddress(BigInteger id)
        {
			return Utils.GetStorageWithKeyPath("P"+id.ToString(),"a");
        }

		/* =================================================
         * Player Registeration: Will give 10 cards for free
         ====================================================*/
		public static Object Register(params object[] args){
			if (args.Length < 1) return false;

			byte[] from = (byte[])args[0];
			if(!Runtime.CheckWitness(from)){
				return false;
			}
			else{
				BigInteger numPlayers = GetNumPlayers();
				for (BigInteger i = 0; i < numPlayers;i++){
					if(PlayerAddress(i) == from){
						return false;
					}
				}
				//If logic goes here, this is a new user. record it.
				Utils.SetStorageWithKeyPath(from, "P" + numPlayers.ToString(), "a");
				Utils.SetStorageWithKey("Gp", BigInteger.Add(numPlayers,1).AsByteArray());
				int hash = Blockchain.GetHeader(Blockchain.GetHeight()).GetHashCode();
				for (int i = 0; i < Const.NumRegisterCard;i++){
					Card card = _GenCardWithOwner(from,hash);
					BigInteger id = BigInteger.Add(GetNumCards(), 1);
					byte[] rawCard = CardToBytes(card);
					Utils.SetStorageWithKeyPath(rawCard,"C", id.ToString());
				}
				return true;
			}
		}


		/* =================================================
         * Card Related Functions
         ====================================================*/
		
		public static Object CardMerge(params object[] args)
        {
            return false;
        }

		public static Object CardTransfer(params object[] args)
        {
            return false;
        }

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

		private static Card BytesToCard(byte[] bytes){
			if(bytes.Length ==0){
				return null;
			}
			else{
				object[] objs = (object[])Helper.Deserialize(bytes);
                return (Card)(object)objs;
			}
		}

		private static byte[] CardToBytes(Card card)
        {
			return Helper.Serialize(card);
        }

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
