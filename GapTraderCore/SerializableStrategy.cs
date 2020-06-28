using System;

namespace GapTraderCore
{
    [Serializable]
    public class SerializableStrategy : SerializableBase
    {
        public string ShortName { get; protected set; }

        public SerializableStrategy(string name, string shortName) : base(name)
        {
            ShortName = shortName;
        }

        public SerializableStrategy()
        {
            
        }
    }
}