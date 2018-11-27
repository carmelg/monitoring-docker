namespace Audified.Messages
{
    public class InspectContainer
    {
        public string ContainerId { get; }

        public InspectContainer(string containerId)
        {
            ContainerId = containerId;
        }
    }
}