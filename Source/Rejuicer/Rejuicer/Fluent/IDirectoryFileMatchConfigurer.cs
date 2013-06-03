namespace Rejuicer
{
    public interface IDirectoryFileMatchConfigurer
    {
        ICompactorConfigurer Matching(string wildcard);
        ICompactorConfigurer All { get; }
        IDirectoryFileMatchConfigurer Excluding(string filename);
    }
}
