using Nancy;

namespace Parker.Holladay.Me
{
    public class IndexModule : NancyModule
    {
        readonly IPostModelBuilder postVmBuilder;

        public IndexModule(IPostModelBuilder postVmBuilder)
        {
            this.postVmBuilder = postVmBuilder;

            Get("/", _ => GetIndex());
            Get("/about", _ => GetAbout());
            Get("/posts", _ => GetAllPosts());
            Get("/posts/{slug}", parameters => GetPost(parameters));
        }

        dynamic GetIndex()
        {
            return View[IndexModel.Build(postVmBuilder.BuildAll())];
        }

        dynamic GetAbout()
        {
            return View[AboutModel.Build()];
        }

        dynamic GetAllPosts()
        {
            return View["posts", postVmBuilder.BuildAll()];
        }

        dynamic GetPost(dynamic parameters)
        {
            string slug = parameters.slug;
            return View[$"posts/{slug}", postVmBuilder.Build(slug)];
        }
    }
}
