@Master['layout']

[pluralsight]: https://pluralsight.com
[csharp]: https://www.google.com/search?q=csharp+language
[dotnet-core]: https://docs.microsoft.com/dotnet/core
[nancy]: http://nancyfx.org
[markdown]: https://www.google.com/search?q=markdown
[nancy-markdown]: https://github.com/jchannon/Nancy.ViewEngines.Markdown
[blog-guide]: posts/building-a-blog-with-nancyfx

@Section['content']

# @Model.PageTitle

<div class="about-me">
    <img src="content/images/me.png" alt="Me" />
</div>

```js
function init() {
    // Hello World!
    console.log('Hello World!')
}
```

## New blog, who dis?

I am a human. I just want to be clear on that up front. I'm not the best human in the world, most likely, but I try to be the best human I can be. I am the father of four wonderful, loving, frustrating, curious, and intelligent children. My wife of more than 10 years is the love of my life, the wisdom to my insanity, and the encouragement to my self-doubt. I try to be the same for her.

I am a software developer by trade, and tinkerer by compulsion. I currently work at [Pluralsight][pluralsight] building things that I think are pretty awesome. Not only that, I truly believe in our mission of elevating the human condition through technology learning. I love learning new tools and processes, and I started this blog to begin sharing more of what I learn, in the hope that it will be useful to some.

## Under the hood

```csharp
public BlogEngine GetEngine()
{
    var blog = new Blog("parker.holladay.me");
    return blog.TheHood
               .Peek(); // ;-)
}
```

This blog was written in [C#][csharp] on [.NET Core][dotnet-core] using the [NancyFx][nancy] framework and a custom [Markdown][markdown] view engine based on the official Nancy [markdown view engine][nancy-markdown]. See my post [here][blog-guide] for more information.

@EndSection
