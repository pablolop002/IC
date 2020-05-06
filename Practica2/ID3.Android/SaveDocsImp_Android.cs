using ID3.Droid;
using ID3.Services;

[assembly:Xamarin.Forms.Dependency(typeof(SaveDocsImp_Android))]
namespace ID3.Droid
{
    public class SaveDocsImp_Android : ISaveDocs
    {
        public string[] GetFileTypes()
        {
            return new[] {"text/plain"};
        }
    }
}