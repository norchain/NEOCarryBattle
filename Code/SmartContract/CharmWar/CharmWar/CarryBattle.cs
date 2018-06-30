using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Helper = Neo.SmartContract.Framework.Helper;

using System.ComponentModel;
using System.Numerics;


namespace CarryBattle
{
	[Serializable]
	public class Card
	{
		//public BigInteger id;
		public byte type;   //0: Infantry, 1: Archer, 2: Cavalry
		public byte[] levels;    // Range: 0 - 255
		public byte[] warID;   //0: Dead 1: Normal
		public byte[] playerID;
	}

	[Serializable]
	public class War{
		//public BigInteger id;
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

			StorageMap map = Storage.CurrentContext.CreateMap();

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
