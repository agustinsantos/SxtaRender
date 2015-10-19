using System.IO;

namespace ObjLoader.Loader.Loaders
{
    public class MaterialStreamProvider : IMaterialStreamProvider
    {
        private readonly string pathDir;

        public MaterialStreamProvider(string path = null)
        {
            pathDir = path;
        }

        public Stream Open(string materialFilePath)
        {
            if (pathDir != null && File.Exists(Path.Combine(pathDir, materialFilePath)))
                return File.Open(Path.Combine(pathDir, materialFilePath), FileMode.Open, FileAccess.Read);
            else
                return File.Open(materialFilePath, FileMode.Open, FileAccess.Read);
        }
    }

    public class MaterialNullStreamProvider : IMaterialStreamProvider
    {
        public Stream Open(string materialFilePath)
        {
            return null;
        }
    }
}