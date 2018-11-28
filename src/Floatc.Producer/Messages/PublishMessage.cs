namespace Floatc.Producer.Messages
{
    public class PublishMessage
    {
        public string Value { get; }

        public PublishMessage(string value)
        {
            Value = value;
        }
    }
}