@Master['layout']
@Metadata
{
    "title": "Building a Blog on .NET Core with NancyFx",
    "slug": "building-a-blog-on-dotnet-core",
    "date": "2018-03-29",
    "tags": ["c#", ".net", ".net core", "nancyfx", "markdown"]
}
@EndMetadata

[dev-chat-tv]: https://devchat.tv
[dotnet-rocks]: https://dotnetrocks.com
[nancy-markdown]: https://github.com/jchannon/Nancy.ViewEngines.Markdown
[markdig]: https://github.com/lunet-io/markdig
[markdown-sharp]: https://www.nuget.org/packages/MarkdownSharp

@Section['content']

@Partial['post-header']

I've been meaning to start a blog for over a year, now. Then, about 6 months ago, our offices at Pluralsight moved a little farther away from my home. As a result of the longer commute, I had time, again, to listen to more podcasts, which I hadn't done in quite some time. I started listening to podcasts like Javascript Jabber and My Javascript Story on [DevChat.tv][dev-chat-tv] and to [.Net Rocks][dotnet-rocks]. I can't pinpoint exactly what it was, but these podcasts fueled me to start writing and sharing.

## The technology

### Why .NET Core?

I have been doing Node and Javascript/Typescript development full-time for the last two years. I thought about using React because that's what I've been doing for front-end development, but that felt a little heavy handed for a static site. That, and I still really enjoy C# as a language and .NET as a framework. So, I decided to branch out from what I'm doing every single day, and started building this simple blogging framework in .NET Core. And, inspite of my excitment for Microsoft's development stack and tools, I don't have a lot of love for Windows, as an operating system--I prefer to use Linux whenever I can. With .NET Core, I get to write the framework in C#, and develop and deploy it on Linux.

### Why NancyFx?

I've used Nancy in past projects, and I really appreciate how explicit it is as a web framework. One of the struggles I've had with ASP.NET MVC and WebApi are how magical they feel.

> We'll automagically build your routes from the names of your controllers and controller methods. It's magic. Oh, and don't forget the Global.asax and _ViewStart.cshtml. They're magic too. Etc. etc.
>
> --ASP.NET (paraphrased)
>
> --Me (sarcastically)

If you want a route in Nancy, you declare that route, and a handler for it. It's as simple as that. No need to do any dances with naming conventions for controllers and views.

### Why Markdown?

Markdown has become second nature, to me, for compiling notes or drafting a document, and, it has become more ubiquitous on the web, what with Stack Overflow and Github, etc. Besides, I would probably pull my hair out, due to excessive tedium, if I were to write all this in raw HTML. Basically, I just wanted to use Markdown.

## The code

I ran into a couple small issues as I set out to build my blog. First of all, while NancyFx is compatible with .NET core 2.0, they have yet to make their view engine libraries compatible. So, I simply borrowed some code from the [Nancy.ViewEngines.Markdown][nancy-markdown] library, and tweaked it to work with .NET Core. All this "view engine" was really doing was augmenting the built in Super Simple View Engine (SSVE), anyway. Thus, views can be written with a blend of HTML with SSVE expressions and Markdown. I had to pull in a dependency for [Markdig][markdig] rather than [MarkdownSharp][markdown-sharp] to parse the Markdown, as MarkdownSharp is also not yet compatible with .NET core.

### The view engine

The following code is a partial implementation of the Nancy `IViewEngineHost` needed for the Markdown view engine to work. The function `RemoveParagraphTagsFromSuperSimpleViewExpressions` does just that: removes `<p></p>` tags that get inserted by Markdig around the `@` expressions used by the SSVE.

```csharp
public string GetTemplate(string templateName, object model)
{
    // Get header html
    var body = ...; // Get html in between body tags

    var bodyHtmlFromMarkdown = Markdown.ToHtml(body);
    var bodyHtml = RemoveParagraphTagsFromSuperSimpleViewExpressions(rawBodyHtml);

    // Get footer html and return
}

static readonly Regex ExpressionsInPTags = new Regex("<p>(@[^<]*)</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

public string RemoveParagraphTagsFromSuperSimpleViewExpressions(string html)
{
    return ExpressionsInPTags.Replace(html, "$1");
}
```

This enables me to compose a shared layout file (which I chose to write in raw HTML) that is used for all my views. The views are then constructed with SSVE expressions and Markdown--ideally with more more Markdown than SSVE.

_NOTE: Here, I've intentially malformed the SSVE `@` expressions so they aren't actually rendered by Nancy as part of this page._

```markdown
@ Master['layout']

@ Section['content']

# @ Model.PageTitle

Bacon ipsum dolor amet tri-tip sirloin biltong, chicken **turducken**
t-bone ground round. _Bacon_ prosciutto doner drumstick salami.

## Looping over model fields

@ Each.EnumerableField

@ Partial['my-partial', Current]

@ EndEach

[See more >>](path-to-more)

```javascript
function () {
  console.log('All the markdown')
}
``
// Imagine that third backtick (`) is there
// If I include it, this view will be broken. =)

@ EndSection
```

### The posts

With each post, I include a custom `@Metadata` tag at the top that gets ignored by both the Markdown and SSVE. It contains a simple JSON object describing the post, and I use this to build the model for each post view.

```json
@Metadata
{
    "title": "My Totally Awesome Post Title",
    "slug": "totally-awesome-post",
    "image": "awesome.jpg",
    "date": "2018-03-31",
    "tags": ["blogs", "internets"]
}
@EndMetadata
```

```csharp
public PostModel BuildPostModel(string postSlug)
{
    var json = ReadMetadataFromPost(postSlug);
    var metadata = JsonConvert.DeserializeObject<PostMetadata>(json) ?? PostMetadata.Empty();

    return new PostModel(metadata.Title)
    {
        Slug = metadata.Slug,
        Image = metadata.Image,
        Date = metadata.Date?.ToString("yyyy-MM-dd"),
        Tags = metadata.Tags
    };
}

public string ReadMetadataFromPost(string postSlug)
{
    var postPath = Path.Combine(Directory.GetCurrentDirectory(), "views", "posts", $"{postSlug}.md")
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
```

## Takeaways

Ideally, I would prefer to have the posts written only in markdown with none of the SSVE expressions (perhaps still with the JSON metadata). But, as I explored that possibility, some of the limitations of Nancy's SSVE became apparent, and posed a large enough barrier that I've chosen to leave the posts as is. I'd like to come back and address this in the future so that I may keep the posts as clean and minimal as possible.

I've had a lot of fun toying with .NET Core as I've started this project. Even though this blog is relatively simple, it shows me the possiblities available with .NET Core, and leaves me wanting to experiment with larger, more connected applications.

@EndSection
