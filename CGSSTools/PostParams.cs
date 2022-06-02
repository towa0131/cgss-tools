using MessagePack;

namespace CGSSTools
{
    [MessagePackObject]
    public class PostParams
    {
        [Key(0)]
        public string viewer_id { get; set; }

        [Key(1)]
        public string timezone { get; set; }
    }
}