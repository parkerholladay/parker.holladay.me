using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Markdig;
using Nancy;
using Nancy.Responses;
using Nancy.ViewEngines;
using Nancy.ViewEngines.SuperSimpleViewEngine;

namespace Parker.Holladay.Me
{
    public class MarkdownEngine : IViewEngine
    {
        readonly SuperSimpleViewEngine engineWrapper;

        public MarkdownEngine(SuperSimpleViewEngine engineWrapper)
        {
            this.engineWrapper = engineWrapper;
        }

        public IEnumerable<string> Extensions
        {
            get { return new[] { "md" }; }
        }

        public void Initialize(ViewEngineStartupContext context)
        { }

        public Response RenderView(ViewLocationResult view, dynamic model, IRenderContext renderContext)
        {
            var response = new HtmlResponse();
            var viewEngineHost = new MarkdownViewEngineHost(new NancyViewEngineHost(renderContext), renderContext);

            var markdownHtml = renderContext.ViewCache.GetOrAdd(view, _ =>
                {
                    string markdown = view.Contents().ReadToEnd();
                    return Markdown.ToHtml(markdown);
                });
            var superSimpleViewHtml = MarkdownHelper.RemoveParagraphTagsFromSuperSimpleViewExpressions(markdownHtml);
            var renderedHtml = this.engineWrapper.Render(superSimpleViewHtml, model, viewEngineHost);

            response.Contents = stream =>
            {
                var writer = new StreamWriter(stream);
                writer.Write(renderedHtml);
                writer.Flush();
            };

            return response;
        }
    }
}
