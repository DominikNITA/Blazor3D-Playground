using Blazor.Extensions.Canvas.WebGL;
using System;
using System.Threading.Tasks;

namespace WebGLSharp
{
    public class Texture
    {
        WebGLContext _gl;
        WebGLTexture _texture;

        public async Task<Texture> BuildAsync(WebGLContext gl, uint[] image)
        {
            var texture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, texture);
            //TODO: Create a new version without width&height
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, 500, 500, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, image);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, (int)TextureParameterValue.LINEAR);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, (int)TextureParameterValue.LINEAR);
            return new Texture(gl, texture);
        }

        internal Texture(WebGLContext gl, WebGLTexture texture)
        {
            _gl = gl;
            _texture = texture;
        }

        internal async Task UseAsync(WebGLUniformLocation uniform, int binding)
        {
            await _gl.ActiveTextureAsync((Blazor.Extensions.Canvas.WebGL.Texture)Enum.Parse(typeof(Blazor.Extensions.Canvas.WebGL.Texture),"TEXTURE"+binding));
            await _gl.BindTextureAsync(TextureType.TEXTURE_2D, _texture);
            await _gl.UniformAsync(uniform, binding);
        }

        internal static Texture Load(string textureFileContent)
        {
            throw new NotImplementedException();
        }
    }
}