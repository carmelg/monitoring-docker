using System.Collections.Generic;

namespace Audified.Messages
{
    public class ListContainers
    {
        public class Request
        {

        }

        public class Response
        {
            public IReadOnlyCollection<string> ContainersId { get; }

            public Response(IReadOnlyCollection<string> containersId)
            {
                ContainersId = containersId;
            }
        }
    }
}