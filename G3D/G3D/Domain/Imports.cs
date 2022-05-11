using System.IO;

namespace G3D.Domain
{
    internal class Imports
    {
        public int Count { get; set; }
        public string Title { get; set; }
        public string Extrnsion { get; set; }
        public string Size { get; set; }

        public Imports(int count, string path)
        {
            Count = count;
            Title = System.IO.Path.GetFileNameWithoutExtension(path);
            Extrnsion = System.IO.Path.GetExtension(path);
            var f = new FileInfo(path);
            Size = string.Format("{0} Kb", (f.Length / 1024).ToString("N2"));
        }
    }
}
