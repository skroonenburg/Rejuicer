namespace Rejuicer.Model
{
    internal class FileMatchModel
    {
        public FileMatchModel(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
        public string WildCard { get; set; }
    }
}
