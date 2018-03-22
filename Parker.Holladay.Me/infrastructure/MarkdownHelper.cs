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
    static class MarkdownHelper
    {
        /*
        <p>      - matches the literal string "<p>"
        (        - creates a capture group, so that we can get the text back by backreferencing in our replacement string
        @        - matches the literal string "@"
        [^<]*    - matches any character other than the "<" character and does this any amount of times
        )        - ends the capture group
        </p>     - matches the literal string "</p>"
        */
        static readonly Regex ExpressionsInPTags = new Regex("<p>(@[^<]*)</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string RemoveParagraphTagsFromSuperSimpleViewExpressions(string html)
        {
            return ExpressionsInPTags.Replace(html, "$1");
        }
    }
}
