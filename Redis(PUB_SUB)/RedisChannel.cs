namespace Redis_PUB_SUB_
{
    public class RedisChannel
    {
        public string ChannelName { get; set; }
        public List<string> Messages { get; set; }=new List<string>();  
    }
}
