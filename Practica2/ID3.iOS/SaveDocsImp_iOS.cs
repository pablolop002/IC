using ID3.iOS;
using ID3.Services;
using MobileCoreServices;

[assembly:Xamarin.Forms.Dependency(typeof(SaveDocsImp_iOS))]
namespace ID3.iOS
{
    public class SaveDocsImp_iOS : ISaveDocs
    {
        public string[] GetFileTypes()
        {
            return new[] {UTType.PlainText.ToString()};
        }
    }
}