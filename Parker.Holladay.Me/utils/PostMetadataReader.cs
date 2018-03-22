using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Parker.Holladay.Me
{
    public interface IPostMetadataReader
    {
        string ReadMetadataFromPost(string postSlug);
        string ReadMetadataFromAllPosts();
    }

    public class PostMetadataReader : IPostMetadataReader
    {
        public string ReadMetadataFromPost(string postSlug)
        {
            var postPath = Path.Combine(Directory.GetCurrentDirectory(), "views", "posts", $"{postSlug}.md");
            return ReadMetadataFromPostFile(postPath);
        }

        public string ReadMetadataFromPostFile(string postPath)
        {
            if (!File.Exists(postPath)) return string.Empty;

            var metdataBuilder = new StringBuilder();
            var keepReading = true;
            var captureMetadata = false;
            using (var reader = new StreamReader(postPath))
            {
                string line;
                while (keepReading && (line = reader.ReadLine()) != null)
                {
                    if (line == "@EndMetadata")
                        keepReading = false;
                    else if (line == "@Metadata")
                        captureMetadata = true;
                    else if (captureMetadata)
                        metdataBuilder.Append(line.Trim());
                }
            }

            return metdataBuilder.ToString();
        }

        public string ReadMetadataFromAllPosts()
        {
            var postsPath = Path.Combine(Directory.GetCurrentDirectory(), "views", "posts");
            if (!Directory.Exists(postsPath)) return "[]";

            var metadataBuilder = new StringBuilder("[");
            var postFiles = Directory.GetFiles(postsPath);
            foreach (var post in postFiles)
                metadataBuilder.AppendFormat("{0},", ReadMetadataFromPostFile(post) ?? "{}");

            return metadataBuilder.Append("]").ToString();
        }
    }
}
