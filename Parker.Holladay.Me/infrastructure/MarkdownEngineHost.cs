using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig;
using Nancy.ViewEngines;
using Nancy.ViewEngines.SuperSimpleViewEngine;

namespace Parker.Holladay.Me
{
    public class MarkdownViewEngineHost : IViewEngineHost
    {
        readonly IViewEngineHost viewEngineHost;
        readonly IRenderContext renderContext;
        static readonly IEnumerable<string> validExtensions = new[] { "md", "markdown" };

        public MarkdownViewEngineHost(IViewEngineHost viewEngineHost, IRenderContext renderContext)
        {
            this.viewEngineHost = viewEngineHost;
            this.renderContext = renderContext;
            this.Context = this.renderContext.Context;
        }

        public object Context { get; private set; }

        public string HtmlEncode(string input)
        {
            return this.viewEngineHost.HtmlEncode(input);
        }

        public string GetTemplate(string templateName, object model)
        {
            var view = this.renderContext.LocateView(templateName, model);
            if (view == null)
                return "[ERR!]";

            var viewContents = view.Contents.Invoke().ReadToEnd();

            if (!validExtensions.Any(x => x.Equals(view.Extension, StringComparison.OrdinalIgnoreCase)))
                return viewContents;

            if (view.Name.ToLower() == "master")
            {
                var headerHtml = viewContents.Substring(
                    viewContents.IndexOf("<!DOCTYPE html>", StringComparison.OrdinalIgnoreCase),
                    viewContents.IndexOf("<body>", StringComparison.OrdinalIgnoreCase) + 6);

                var body = viewContents.Substring(
                        viewContents.IndexOf("<body>", StringComparison.OrdinalIgnoreCase) + 6,
                        (viewContents.IndexOf("</body>", StringComparison.OrdinalIgnoreCase) - 7) -
                        (viewContents.IndexOf("<body>", StringComparison.OrdinalIgnoreCase)));
                var rawBodyHtml = Markdown.ToHtml(body);
                var bodyHtml = MarkdownHelper.RemoveParagraphTagsFromSuperSimpleViewExpressions(rawBodyHtml);

                var footerHtml = viewContents.Substring(viewContents.IndexOf("</body>", StringComparison.OrdinalIgnoreCase));

                return string.Concat(headerHtml, bodyHtml, footerHtml);
            }

            return Markdown.ToHtml(viewContents);
        }

        public string GetUriString(string name, params string[] parameters)
        {
            return this.viewEngineHost.GetUriString(name, parameters);
        }

        public string ExpandPath(string path)
        {
            return this.viewEngineHost.ExpandPath(path);
        }

        public string AntiForgeryToken()
        {
            return this.viewEngineHost.AntiForgeryToken();
        }
    }
}
