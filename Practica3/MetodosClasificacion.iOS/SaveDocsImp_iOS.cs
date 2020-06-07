using MetodosClasificacion.iOS;
using MetodosClasificacion.Services;
using MobileCoreServices;

[assembly:Xamarin.Forms.Dependency(typeof(SaveDocsImp_iOS))]
namespace MetodosClasificacion.iOS
{
    public class SaveDocsImp_iOS : ISaveDocs
    {
        public string[] GetFileTypes()
        {
            return new[] {UTType.PlainText.ToString()};
        }
    }
}