using MetodosClasificacion.Droid;
using MetodosClasificacion.Services;

[assembly:Xamarin.Forms.Dependency(typeof(SaveDocsImp_Android))]
namespace MetodosClasificacion.Droid
{
    public class SaveDocsImp_Android : ISaveDocs
    {
        public string[] GetFileTypes()
        {
            return new[] {"text/plain"};
        }
    }
}