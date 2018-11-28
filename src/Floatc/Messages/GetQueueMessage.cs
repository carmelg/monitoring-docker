namespace Floatc.Messages
{
    public class GetQueueMessage
    {
        public class Request
        {
            public string VirtualHost { get; }
            public string Queue { get; }

            public Request(string queue)
                : this("%2F", queue)
            {

            }

            public Request(string virtualHost, string queue)
            {
                VirtualHost = virtualHost;
                Queue = queue;
            }
        }

        public class Response
        {
            public int Count { get; }

            public Response(int count)
            {
                Count = count;
            }
        }
    }
}