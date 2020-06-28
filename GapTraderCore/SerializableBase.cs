using System;

namespace GapTraderCore
{
    [Serializable]
    public class SerializableBase
    {
        public string Name { get; protected set; }

        public SerializableBase()
        {
        }

        public SerializableBase(string name)
        {
            Name = name;
        }
    }
}
