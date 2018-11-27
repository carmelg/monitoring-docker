using System;

namespace Audified.Messages
{
    public class StoreInspectContainerData
    {
        public string Id { get; }
        public string Name { get; }
        public string Image { get; }
        public bool IsRunning { get; }

        public StoreInspectContainerData(string id, string name, string image, bool isRunning)
        {
            Id = id;
            Name = name;
            Image = image;
            IsRunning = isRunning;
        }
    }
}