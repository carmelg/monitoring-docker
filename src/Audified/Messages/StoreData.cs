using System;

namespace Audified.Messages
{
    public class StoreData<T>
    {
        public T Payload { get; }

        public StoreData(T payload)
        {
            Payload = payload;
        }
    }
}