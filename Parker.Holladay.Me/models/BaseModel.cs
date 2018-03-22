using System;

namespace Parker.Holladay.Me
{
    public abstract class BaseModel
    {
        const string SITE_TITLE = "parker.holladay.me";

        protected BaseModel(string pageTitle = null, bool isHome = false, bool isPost = false, bool isAbout = false)
        {
            Title = pageTitle != null ? $"{pageTitle} | {SITE_TITLE}" : SITE_TITLE;
            PageTitle = pageTitle ?? SITE_TITLE;
            IsHome = isHome;
            IsPost = isPost;
            IsAbout = isAbout;
        }

        public string Title { get; private set; }
        public string PageTitle { get; private set; }
        public bool IsHome { get; private set; }
        public bool IsPost { get; private set; }
        public bool IsAbout { get; private set; }
    }
}
