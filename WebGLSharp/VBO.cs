using Blazor.Extensions.Canvas.WebGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebGLSharp
{
    public class VBO
    {
        WebGLContext _gl;
        int _size;
        int _count;
        WebGLBuffer _dataBuffer;

        public async static Task<VBO> BuildAsync(WebGLContext gl, int count, float[] data)
        {
            var dataBuffer = await gl.CreateBufferAsync();
            await gl.BindBufferAsync(BufferType.ARRAY_BUFFER, dataBuffer);
            await gl.BufferDataAsync(BufferType.ARRAY_BUFFER, data, BufferUsageHint.STATIC_DRAW);
            return new VBO(gl, count, data, dataBuffer);
        }
        private VBO(WebGLContext gl, int count,float[] data, WebGLBuffer dataBuffer)
        {
            _gl = gl;
            _count = count;
            _size = data.Length / count;
            _dataBuffer = dataBuffer;
        }

        public async Task DestroyAsync()
        {
            await _gl.DeleteBufferAsync(_dataBuffer);
        }

        internal static object BuildAsync(WebGLContext gl, int vertexCount, object positions)
        {
            throw new NotImplementedException();
        }

        public async Task BindToAttributeAsync(uint attribute)
        {
            await _gl.BindBufferAsync(BufferType.ARRAY_BUFFER, _dataBuffer);
            await _gl.EnableVertexAttribArrayAsync(attribute);
            await _gl.VertexAttribPointerAsync(attribute, _size, DataType.FLOAT, false, 0, 0);
        }
    }
}
