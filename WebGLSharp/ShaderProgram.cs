using Blazor.Extensions.Canvas.WebGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebGLSharp
{
    public class ShaderProgram
    {
        public WebGLProgram Program { get; set; }
        public Dictionary<string, int> Attributes { get; set; }
        public Dictionary<string, WebGLUniformLocation> Uniforms { get; set; }
        public WebGLContext GlContext { get; set; }

        public ShaderProgram(WebGLProgram program, WebGLContext gl, Dictionary<string, int> attributes, Dictionary<string, WebGLUniformLocation> uniforms)
        {
            Program = program;
            GlContext = gl;
            Attributes = attributes;
            Uniforms = uniforms;
        }

        public static async Task<ShaderProgram> InitShaderProgram(WebGLContext gl, string vsSource, string fsSource, List<string> attributesNames = null, List<string> uniformsNames = null)
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

            attributesNames ??= new List<string>();
            var attributesDict = new Dictionary<string, int>();
            foreach (var attribute in attributesNames)
            {
                attributesDict.Add(attribute, await gl.GetAttribLocationAsync(program, attribute));
            }

            uniformsNames ??= new List<string>();
            var uniformsDict = new Dictionary<string, WebGLUniformLocation>();
            foreach (var uniform in uniformsNames)
            {
                uniformsDict.Add(uniform, await gl.GetUniformLocationAsync(program, uniform));
            }

            return new ShaderProgram(program, gl, attributesDict, uniformsDict);
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
