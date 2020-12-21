using Blazor.Extensions.Canvas.WebGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebGLSharp
{
    public class ShaderProgram
    {
        public static async Task<WebGLProgram> InitShaderProgram(WebGLContext gl, string vsSource, string fsSource)
        {
            var vertexShader = await LoadShaderAsync(gl, ShaderType.VERTEX_SHADER, vsSource);
            var fragmentShader = await LoadShaderAsync(gl, ShaderType.FRAGMENT_SHADER, fsSource);

            var program = await gl.CreateProgramAsync();
            await gl.AttachShaderAsync(program, vertexShader);
            await gl.AttachShaderAsync(program, fragmentShader);
            await gl.LinkProgramAsync(program);

            await gl.DeleteShaderAsync(vertexShader);
            await gl.DeleteShaderAsync(fragmentShader);

            if (!await gl.GetProgramParameterAsync<bool>(program, ProgramParameter.LINK_STATUS))
            {
                string info = await gl.GetProgramInfoLogAsync(program);
                throw new Exception("An error occured while linking the program: " + info);
            }
            return program;
        }

        async static Task<WebGLShader> LoadShaderAsync(WebGLContext gl, ShaderType type, string source)
        {
            var shader = await gl.CreateShaderAsync(type);
            await gl.ShaderSourceAsync(shader, source);
            await gl.CompileShaderAsync(shader);

            if (!await gl.GetShaderParameterAsync<bool>(shader, ShaderParameter.COMPILE_STATUS))
            {
                string info = await gl.GetShaderInfoLogAsync(shader);
                await gl.DeleteShaderAsync(shader);
                throw new Exception("An error occured while compiling the shader: " + info);
            }

            return shader;
        }
    }
}
