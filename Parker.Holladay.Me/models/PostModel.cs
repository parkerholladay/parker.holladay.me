using System.Collections.Generic;

namespace Parker.Holladay.Me
{
    public class PostModel : BaseModel
    {
        public PostModel(string title) : base(title, isPost: true)
        { }

        public string Slug { get; set; }
        public string Image { get; set; }
        public string Date { get; set; }
        public bool HasDate { get { return !string.IsNullOrEmpty(Date); } }
        public List<string> Tags { get; set; }
    }
}
