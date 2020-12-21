using ObjLoader.Loader.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebGLSharp
{
    public class MeshLoader
    {
        public async static Task<LoadResult> LoadModel(Stream objStream, Stream mtlStream)
        {
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create(new CustomMaterialStreamProvider(mtlStream));
            //var fileStream = new FileStream(@"/models/cylinderWithRoof.obj", FileMode.Open);
            return objLoader.Load(objStream);
        }

        private class CustomMaterialStreamProvider : IMaterialStreamProvider
        {
            Stream _stream;
            public CustomMaterialStreamProvider(Stream stream)
            {
                _stream = stream;
            }
            public Stream Open(string materialFile)
            {
                return _stream;
            }
        }
    }
}
