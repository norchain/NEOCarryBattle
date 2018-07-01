using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.SmartContract
{
	public class State{
		public const byte Create = 0;
		public const byte Update = 1;
		public const byte Delete = 2;
		public const byte Unchanged = 3;
		public const byte Invalid = 4;
	}

	public class Utils{
		public const string splitter = "";
        

		public static string KeyPath(params string[] elements)
        {
			return string.Join(splitter, elements);
        }
		public static byte[] GetStorageWithKeyPath(params string[] elements){
			return GetStorageWithKey(KeyPath(elements));
		}
		public static byte[] GetStorageWithKey(byte[] key){
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

		public static byte SetStorageWithKey(string key,byte[] value)
        {
			byte[] orig = GetStorageWithKey(key);
			if (orig == value) { return State.Unchanged; }

            if (value.Length == 0)
            {
				Storage.Delete(Storage.CurrentContext, key);
                return State.Delete;

            }
			else{
				Storage.Put(Storage.CurrentContext, key, value);
				return (orig.Length == 0)? State.Create: State.Update;
			} 
        }
	}


	public class Const {
		public const string numItems = "cNumItems";
		public const string prixItem = "i";
	}


    public class MapExample : Framework.SmartContract
    {      
		public static bool Main(string op,params object[] args)
        {
			if(op == "writeItem"){
				BigInteger currentNum = Utils.GetStorageWithKey(Const.numItems).AsBigInteger();

				byte[] key = (byte[])args[0];
				if (key[0]!='i'){
					return false;
				}
				else{
					byte[] val = (byte[])args[1];
					byte result = Utils.SetStorageWithKeyPath(val,Const.prixItem, key.ToString());
					switch (result) {
						case State.Create:
							{
								
								Utils.SetStorageWithKey(Const.numItems, BigInteger.Add(currentNum, 1).AsByteArray());
								break;
							}
						case State.Delete:{
								BigInteger currentNum = Utils.GetStorageWithKey(Const.numItems).AsBigInteger();
                                Utils.SetStorageWithKey(Const.numItems, BigInteger.Subtract(currentNum, 1).AsByteArray());
								break;
							}

							break;
						default:
							break;
					}
                 


					byte[] ex = Storage.Get(Storage.CurrentContext, key);
					if(ex.Length == 0){ //Not exist yet
						BigInteger currentNum = Storage.Get(Storage.CurrentContext, "cNumItems").AsBigInteger();
						Storage.Put(Storage.CurrentContext, "cNumItems", BigInteger.Add(currentNum, 1));
                        
					}
					Storage.Put(Storage.CurrentContext, key, val);
					return true;
				}
			}
            
			if(op == "assignItem"){
				byte[] itemId = (byte[])args[0];
				if (itemId[0] != 'i')
                {
                    return false;
                }
				else{
					byte[] userId = (byte[])args[1];
					Storage.Put(Storage.CurrentContext, "a"+, val);
				}
			}


        }
    }
}