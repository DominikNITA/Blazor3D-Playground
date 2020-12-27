using Blazor.Extensions.Canvas.WebGL;
using System;
using System.Threading.Tasks;

namespace WebGLSharp
{
    public class Texture
    {
        WebGLContext _gl;
        WebGLTexture _texture;
        int _width, _height;

        public static async Task<Texture> BuildAsync(WebGLContext gl, int[] image, int width = 100, int height = 100)
        {
            var texture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, texture);
            //TODO: Create a new version without width&height
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, width, height, 0, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, image);
            // WebGL1 has different requirements for power of 2 images vs non power of 2 images
            //if ((width & (width - 1)) == 0 && (height & (height - 1)) == 0)
            //{
            //    await gl.GenerateMipmapAsync(TextureType.TEXTURE_2D);
            //}
            //else
            //{
            //await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, (int)TextureParameterValue.LINEAR);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, (int)TextureParameterValue.LINEAR);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, (int)TextureParameterValue.CLAMP_TO_EDGE);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, (int)TextureParameterValue.CLAMP_TO_EDGE);
            return new Texture(gl, texture, width, height);
        }

        public async Task UpdateData(int[] image)
        {
            await _gl.BindTextureAsync(TextureType.TEXTURE_2D, _texture);
            await _gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, _width, _height, 0, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, image);
            //await _gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, (int)TextureParameterValue.LINEAR);
            //await _gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, (int)TextureParameterValue.CLAMP_TO_EDGE);
            //await _gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, (int)TextureParameterValue.CLAMP_TO_EDGE);
        }

        internal Texture(WebGLContext gl, WebGLTexture texture, int width, int height)
        {
            _gl = gl;
            _texture = texture;
            _width = width;
            _height = height;
        }

        internal async Task UseAsync(WebGLUniformLocation uniform, int binding)
        {
            await _gl.ActiveTextureAsync((Blazor.Extensions.Canvas.WebGL.Texture)Enum.Parse(typeof(Blazor.Extensions.Canvas.WebGL.Texture), "TEXTURE" + binding));
            await _gl.BindTextureAsync(TextureType.TEXTURE_2D, _texture);
            await _gl.UniformAsync(uniform, binding);
        }

        internal static Texture Load(string textureFileContent)
        {
            throw new NotImplementedException();
        }
    }
}