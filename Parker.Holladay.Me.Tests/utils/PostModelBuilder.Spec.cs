using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using NUnit.Specifications;
using Parker.Holladay.Me;

namespace Parker.Holladay.Me.Tests
{
    public class when_getting_a_post
    {
        static readonly IPostMetadataReader reader = Substitute.For<IPostMetadataReader>();
        static readonly PostModelBuilder subject = new PostModelBuilder(reader);

        static string postSlug;
        static PostMetadata metadata;
        static PostModel expected;
        static PostModel actual;

        public class when_metadata_is_available : ContextSpecification
        {
            Establish context = () =>
            {
                postSlug = "hots-is-life";
                metadata = new PostMetadata
                {
                    Title = "HoTS is Life",
                    Slug = postSlug,
                    Date = DateTimeOffset.Now,
                    Tags = new List<string> { "blizzard", "starcraft", "diablo", "warcraft", "overwatch" }
                };
                expected = Helper.BuildPostFromMetadata(metadata);

                reader.ReadMetadataFromPost(postSlug).Returns(JsonConvert.SerializeObject(metadata));
            };

            Because of = () => actual = subject.Build(postSlug);

            It returns_the_expected_post_model = () =>
            {
                Assert.AreEqual(expected.Title, actual.Title);
                Assert.AreEqual(expected.PageTitle, actual.PageTitle);
                Assert.AreEqual(expected.Slug, actual.Slug);
                Assert.AreEqual(expected.Date, actual.Date);
                Assert.AreEqual(expected.Image, actual.Image);
                Assert.AreEqual(expected.Tags, actual.Tags);
            };
        }

        public class when_metadata_is_not_available : ContextSpecification
        {
            Establish context = () =>
            {
                postSlug = "the-nexus";
                reader.ReadMetadataFromPost(postSlug).Returns(string.Empty);
            };

            Because of = () => actual = subject.Build(postSlug);

            It returns_an_empty_post_model = () =>
            {
                Assert.IsNotNull(actual.Title);
                Assert.IsNotNull(actual.PageTitle);
                Assert.IsTrue(actual.IsPost);
                Assert.IsNull(actual.Slug);
                Assert.IsNull(actual.Image);
                Assert.IsNull(actual.Date);
                Assert.IsEmpty(actual.Tags);
            };
        }
    }

    public class when_getting_all_posts
    {
        static readonly IPostMetadataReader reader = Substitute.For<IPostMetadataReader>();
        static readonly PostModelBuilder subject = new PostModelBuilder(reader);

        static List<PostMetadata> metadatas;
        static AllPostsModel expected;
        static AllPostsModel actual;

        public class when_posts_are_available : ContextSpecification
        {
            Establish context = () =>
            {
                var metadata1 = new PostMetadata
                {
                    Title = "StarCraft",
                    Slug = "starcraft",
                    Date = DateTimeOffset.Now.AddDays(-2),
                    Tags = new List<string> { "blizzard", "starcraft" }
                };
                var metadata2 = new PostMetadata
                {
                    Title = "WarCraft",
                    Slug = "warcraft",
                    Date = DateTimeOffset.Now,
                    Tags = new List<string> { "blizzard", "warcraft" }
                };
                metadatas = new List<PostMetadata> { metadata1, metadata2 };

                var postModels = metadatas.Select(m => Helper.BuildPostFromMetadata(m)).ToList();
                expected = new AllPostsModel(postModels);

                reader.ReadMetadataFromAllPosts().Returns(JsonConvert.SerializeObject(metadatas));
            };

            Because of = () => actual = subject.BuildAll();

            It returns_the_expected_all_posts_model = () =>
            {
                Assert.AreEqual(expected.Title, actual.Title);
                Assert.AreEqual(expected.PageTitle, actual.PageTitle);

                var expectedPost = expected.Posts.First();
                var actualPost = actual.Posts.First(p => p.Slug == expectedPost.Slug);
                Assert.AreEqual(expectedPost.PageTitle, actualPost.PageTitle);
                Assert.AreEqual(expectedPost.Date, actualPost.Date);
                Assert.AreEqual(expectedPost.Image, actualPost.Image);
                Assert.AreEqual(expectedPost.Tags, actualPost.Tags);
            };

            It returns_the_posts_ordered_by_date = () => Assert.Greater(actual.Posts.First().Date, actual.Posts.Last().Date);
        }

        public class when_posts_are_not_available : ContextSpecification
        {
            Establish context = () =>
            {
                expected = new AllPostsModel(new List<PostModel>());
                reader.ReadMetadataFromAllPosts().Returns("[]");
            };

            Because of = () => actual = subject.BuildAll();

            It returns_the_model_with_empty_posts = () =>
            {
                Assert.AreEqual(expected.Title, actual.Title);
                Assert.AreEqual(expected.PageTitle, actual.PageTitle);
                Assert.IsEmpty(actual.Posts);
            };
        }
    }

    internal static class Helper
    {
        internal static PostModel BuildPostFromMetadata(PostMetadata metadata)
        {
            return new PostModel(metadata.Title)
            {
                Slug = metadata.Slug,
                Image = metadata.Slug + ".jpg",
                Date = metadata.Date?.ToString("yyyy-MM-dd"),
                Tags = metadata.Tags
            };
        }
    }
}
