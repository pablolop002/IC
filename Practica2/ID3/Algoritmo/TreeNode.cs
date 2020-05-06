using System.Collections.Generic;

namespace ID3.Algoritmo
{
    public class TreeNode
    {
        public TreeNode(string nombre, int indice, Atributo nodo, string arista)
        {
            Nombre = nombre;
            Indice = indice;
            Nodo = nodo;
            Hijos = new List<TreeNode>();
            Arista = arista;
        }

        public TreeNode(bool isleaf, string name, string edge)
        {
            EsHoja = isleaf;
            Nombre = name;
            Arista = edge;
        }

        public string Nombre { get; }

        public string Arista { get; }

        public Atributo Nodo { get; }

        public List<TreeNode> Hijos { get; }

        public int Indice { get; }

        public bool EsHoja { get; }
    }
}