using System.Data;

namespace ID3.Algoritmo
{
    public class Arbol
    {
        public static Tree CreateTreeAndHandleUserOperation(DataTable dato)
        {
            return new Tree
            {
                Root = Tree.Aprender(dato, "")
            };
        }
    }
}