using System.Collections.Generic;
using System.Linq;

namespace Parker.Holladay.Me
{
    public class IndexModel : BaseModel
    {
        private IndexModel(List<PostModel> posts) : base(isHome: true)
        {
            Posts = posts;
        }

        public List<PostModel> Posts { get; private set; }

        public static IndexModel Build(AllPostsModel allPosts)
        {
            return new IndexModel(allPosts.Posts.Take(4).ToList());
        }
    }
}
