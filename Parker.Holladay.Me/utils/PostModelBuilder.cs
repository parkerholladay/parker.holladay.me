using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Parker.Holladay.Me
{
    public interface IPostModelBuilder
    {
        PostModel Build(string postSlug);
        AllPostsModel BuildAll();
    }

    public class PostModelBuilder : IPostModelBuilder
    {
        readonly IPostMetadataReader reader;

        public PostModelBuilder(IPostMetadataReader reader)
        {
            this.reader = reader;
        }

        public PostModel Build(string postSlug)
        {
            var json = reader.ReadMetadataFromPost(postSlug);
            var metadata = JsonConvert.DeserializeObject<PostMetadata>(json) ?? PostMetadata.Empty();

            return BuildPostFromMetadata(metadata);
        }

        public AllPostsModel BuildAll()
        {
            var json = reader.ReadMetadataFromAllPosts();
            var metadatas = JsonConvert.DeserializeObject<List<PostMetadata>>(json);

            List<PostModel> posts = new List<PostModel>();
            foreach (var metadata in metadatas)
                posts.Add(BuildPostFromMetadata(metadata));

            return new AllPostsModel(posts.OrderByDescending(p => p.Date).ToList());
        }

        PostModel BuildPostFromMetadata(PostMetadata metadata)
        {
            return new PostModel(metadata.Title)
            {
                Slug = metadata.Slug,
                Image = metadata.Slug != null ? metadata.Slug + ".jpg" : null,
                Date = metadata.Date?.ToString("yyyy-MM-dd"),
                Tags = metadata.Tags
            };
        }
    }

    public class PostMetadata
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTimeOffset? Date { get; set; }
        public List<string> Tags { get; set; }

        public static PostMetadata Empty()
        {
            return new PostMetadata { Tags = new List<string>() };
        }
    }
}
