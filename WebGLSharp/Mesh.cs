using ObjLoader.Loader.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.WebGL;
using GLMatrixSharp;

namespace WebGLSharp
{
    public class Mesh
    {
        VBO _positions, _normals, _uvs;
        Texture _texture;
        int _vertexCount;
        float[] _position;
        WebGLContext _gl;

        Mesh(VBO positions, VBO normals, VBO uvs, Texture texture, int vertexCount, float[] position, WebGLContext gl)
        {
            _positions = positions;
            _normals = normals;
            _uvs = uvs;
            _texture = texture;
            _vertexCount = vertexCount;
            _position = position;
            _gl = gl;
        }

        public async static Task<Mesh> BuildAsync(WebGLContext gl, Geometry geometry, Texture texture)
        {
            int vertexCount = geometry.GetVertexCount();
            return new Mesh( 
                await VBO.BuildAsync(gl, vertexCount, geometry.GetPositions()),
                await VBO.BuildAsync(gl, vertexCount, geometry.GetNormals()),
                await VBO.BuildAsync(gl, vertexCount, geometry.GetUvs()),
                texture,
                vertexCount,
                Mat4.Create(),
                gl);
        }

        public async Task DestroyAsync()
        {
            await _positions.DestroyAsync();
            await _normals.DestroyAsync();
            await _uvs.DestroyAsync();
        }

        public async Task DrawAsync(ShaderProgram shaderProgram)
        {
            //TODO: add attribute dictionary to the shader program
            await _positions.BindToAttributeAsync((uint)shaderProgram.Attributes.GetValueOrDefault("position"));
            await _normals.BindToAttributeAsync((uint)shaderProgram.Attributes.GetValueOrDefault("normal"));
            await _uvs.BindToAttributeAsync((uint)shaderProgram.Attributes.GetValueOrDefault("uv"));
            await _gl.UniformMatrixAsync(shaderProgram.Uniforms.GetValueOrDefault("model"), false, _position);
            await _texture.UseAsync( shaderProgram.Uniforms.GetValueOrDefault("diffuse"), 0);
            await _gl.DrawArraysAsync(Primitive.TRIANGLES, 0, _vertexCount);
        }

        public async Task<Mesh> LoadAsync(WebGLContext gl, string objFileContent, string textureFileContent)
        {
            return await BuildAsync(gl, Geometry.Parse(objFileContent), Texture.Load(textureFileContent));
        }

        //public async static Task<LoadResult> LoadModel(Stream objStream, Stream mtlStream)
        //{
        //    var objLoaderFactory = new ObjLoaderFactory();
        //    var objLoader = objLoaderFactory.Create(new CustomMaterialStreamProvider(mtlStream));
        //    //var fileStream = new FileStream(@"/models/cylinderWithRoof.obj", FileMode.Open);
        //    var res = objLoader.Load(objStream);
        //    res.Groups.Select(g => g.Faces.First().)
        //}

        //private class CustomMaterialStreamProvider : IMaterialStreamProvider
        //{
        //    Stream _stream;
        //    public CustomMaterialStreamProvider(Stream stream)
        //    {
        //        _stream = stream;
        //    }
        //    public Stream Open(string materialFile)
        //    {
        //        return _stream;
        //    }
        //}
    }
}
