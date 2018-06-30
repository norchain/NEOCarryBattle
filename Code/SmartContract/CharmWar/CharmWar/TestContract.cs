using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.SmartContract
{
    public class MapExample : Framework.SmartContract
    {
        public static byte[] Main(string op)
        {
            // Simple Map
            Map<string, int> m = new Map<string, int>();
            m["hello"] = 10;
            m["world"] = 15;
            Runtime.Notify(m["hello"]); // will print '10'
            Runtime.Notify(m["world"]); // will print '15'

            // StorageMap (create prefix for Storage)
            StorageMap user_sm = Storage.CurrentContext.CreateMap("user"); // 'user' prefix
            user_sm.Put("xyz", 20);
            user_sm.Put("wrd", 30);
            StorageMap tx_sm = Storage.CurrentContext.CreateMap("tx"); // 'tx' prefix
            tx_sm.Put("xyz", 40);
            tx_sm.Put("wrd", 50);

            Runtime.Notify(user_sm.Get("xyz").AsBigInteger()); // will print '20'
            Runtime.Notify(user_sm.Get("wrd").AsBigInteger()); // will print '30'
            Runtime.Notify(tx_sm.Get("xyz").AsBigInteger()); // will print '40'
            Runtime.Notify(tx_sm.Get("wrd").AsBigInteger()); // will print '50'
            Runtime.Notify(Storage.Get(Storage.CurrentContext, "user\x00xyz").AsBigInteger()); // will print '20'
			BigInteger r = Storage.Get(Storage.CurrentContext, "tx\u0000xyz").AsBigInteger();
            Runtime.Notify(r); // will print '40'
			return r.ToByteArray();
        }
    }
}