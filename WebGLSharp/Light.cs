using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WebGLSharp
{
    public class Light
    {
        public Vector3 LightDirection { get; set; }
        public float AmbientLight { get; set; }

        public Light(Vector3 lightDirection, float ambientLight)
        {
            LightDirection = lightDirection;
            AmbientLight = ambientLight;
        }

        public Light() : this(new Vector3(5, 1, 1), 0f) { }

        public async Task UseAsync(ShaderProgram shaderProgram)
        {
            await shaderProgram.GlContext.UniformAsync(shaderProgram.Uniforms.GetValueOrDefault("lightDirection"),LightDirection.X, LightDirection.Y, LightDirection.Z);
            await shaderProgram.GlContext.UniformAsync(shaderProgram.Uniforms.GetValueOrDefault("ambientLight"), AmbientLight);
        }
    }
}
