using System;
using System.Collections.Generic;

namespace Parker.Holladay.Me
{
    public class AllPostsModel : BaseModel
    {
        public AllPostsModel(List<PostModel> posts) : base("Posts", isPost: true)
        {
            Posts = posts;
        }

        public List<PostModel> Posts { get; private set; }
    }
}
