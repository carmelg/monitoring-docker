namespace Audified.Messages
{
    public class InspectContainer
    {
        public class Request
        {
            public string ContainerId { get; }

            public Request(string containerId)
            {
                ContainerId = containerId;
            }
        }

        public class Response
        {
            public string Id { get; }
            public string Name { get; }
            public string Image { get; }
            public bool IsRunning { get; }

            public Response(string id, string name, string image, bool isRunning)
            {
                Id = id;
                Name = name;
                Image = image;
                IsRunning = isRunning;
            }
        }
    }
}