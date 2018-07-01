using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Helper = Neo.SmartContract.Framework.Helper;

using System.ComponentModel;
using System.Numerics;


namespace CarryBattle
{
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
		#region StorageInteraction
		public const string splitter = "";


		public static string KeyPath(params string[] elements)
		{
			return string.Join(splitter, elements);
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
    
	/** The Global Constants */
	public static class Const
    {
		public const int LenPlayer = 20;


        public const string NumCardLive = "ncl";
		public const string NumCardDead = "ncd";
		public const string NumWarLive = "nwl";
		public const string NumWarDead = "nwd";

		public const string PrixCardLive = "pcl";
		public const string PrixCardDead = "pcd";
		public const string PrixWarLive = "pwl";
		public const string PrixWarDead = "pwd";
		public const string PrixOwner = "po";
    }

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
		public byte type;   //TypeArmy
		public byte[] levels;    // Range: 0 - 255
		public byte[] warID;   //0: Free
		public byte[] ownerID;
	}

	[Serializable]
	public class War{
		public byte[] endHeight;
		public byte[] cardIDs;  
	}
    
	public class Constant{
		static int bytesCardID = 20;
	}

	public class CarryBattle : SmartContract
    {
		public static byte[] Main(string operation, byte[] paras)
        {
			if (operation == "war"){
				
			}
			byte[] r = new byte[1];

			return r;
        }
       

		/* ================
        * Basic Get Information
        ===================*/

		public static byte[] SetMapSample(byte[]value, params object[] path){

			StorageMap map = Storage.CurrentContext.CreateMap("kitty");


		}

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




		public static byte[] OpenRoom(byte[] playerID, BigInteger[] cardIDs)
        {
            

        }

		public static byte[] JoinRoom(byte[] playerID, BigInteger roomID, BigInteger[] cardIDs)
        {

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
