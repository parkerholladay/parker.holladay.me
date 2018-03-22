namespace Parker.Holladay.Me
{
    public class AboutModel : BaseModel
    {
        private AboutModel() : base("About", isAbout: true)
        { }

        public static AboutModel Build()
        {
            return new AboutModel();
        }
    }
}
