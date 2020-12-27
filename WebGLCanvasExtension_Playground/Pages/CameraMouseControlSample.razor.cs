using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using Blazor.Extensions.Canvas;
using GLMatrixSharp;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebGLSharp;
using Microsoft.AspNetCore.Components.Web;

namespace WebGLCanvasExtension_Playground.Pages
{
    public partial class CameraMouseControlSample : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }

        BECanvasComponent _canvasReference;
        ShaderProgram _shaderProgram;
        Mesh _cylinderMesh;
        Light _light;

        bool _isDragging = false;
        double _lastX = 0, _lastY = 0;
        double _rotationX = 0, _rotationY = 0;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var gl = await this._canvasReference.CreateWebGLAsync(new WebGLContextAttributes
                {
                    PreserveDrawingBuffer = true,
                    PowerPreference = WebGLContextAttributes.POWER_PREFERENCE_HIGH_PERFORMANCE
                });
                await gl.ClearColorAsync(0.1f, 0.1f, 0.3f, 1);
                await gl.EnableAsync(EnableCap.DEPTH_TEST);
                await gl.ClearAsync(BufferBits.COLOR_BUFFER_BIT | BufferBits.DEPTH_BUFFER_BIT);
                await gl.EnableAsync(EnableCap.CULL_FACE);
                await gl.FrontFaceAsync(FrontFaceDirection.CCW);
                await gl.CullFaceAsync(Face.BACK);
                _shaderProgram = await ShaderProgram.InitShaderProgram(
            gl,
            await Http.GetStringAsync("/shaders/basic.vert"),
            await Http.GetStringAsync("/shaders/basic.frag"),
            new List<string>() { "position", "normal", "uv" },
            new List<string>() { "model", "projection", "ambientLight", "lightDirection", "diffuse" });

                var geometry = Geometry.ParseObjFile(await Http.GetStringAsync("/models/susan.obj"));
                var textureData = new int[40000];
                Random rnd = new Random();
                for (int i = 0; i < 40000; i = i + 4)
                {
                    //Colors between 0-255
                    textureData[i] = rnd.Next(40, 90);
                    textureData[i + 1] = 200;
                    textureData[i + 2] = 15;
                    textureData[i + 3] = 255;
                }
                var texture = await WebGLSharp.Texture.BuildAsync(gl, textureData);
                //var initialPosition = Mat4.Translate(Mat4.Create(), new float[] { -0.0f, 0.0f, -6f });
                _cylinderMesh = await Mesh.BuildAsync(gl, geometry, texture);
                _light = new Light();

                await DrawSceneAsync();
            }
        }

        private async Task DrawSceneAsync()
        {
            await _shaderProgram.GlContext.ClearAsync(BufferBits.COLOR_BUFFER_BIT | BufferBits.DEPTH_BUFFER_BIT);
            await _shaderProgram.GlContext.UseProgramAsync(_shaderProgram.Program);
            await _light.UseAsync(_shaderProgram);
            var projectionMatrix = Mat4.Perspective((float)(45 * Math.PI / 180), 1f, 0.1f, 100f);
            projectionMatrix = Mat4.Translate(projectionMatrix, new float[] { 0, 0, -6f });
            projectionMatrix = Mat4.Rotate(projectionMatrix, (float)_rotationX, new float[3] { 1f, 0, 0f });
            projectionMatrix = Mat4.Rotate(projectionMatrix, (float)_rotationY, new float[3] { 0, 1f, 0f });
            await _shaderProgram.GlContext.UniformMatrixAsync(_shaderProgram.Uniforms.GetValueOrDefault("projection"), false, projectionMatrix);
            await _cylinderMesh.DrawAsync(_shaderProgram);
        }

        private void MouseDown(MouseEventArgs e)
        {
            _isDragging = true;
            _lastX = e.ClientX;
            _lastY = e.ClientY;
        }

        private void MouseUp(MouseEventArgs e)
        {
            _isDragging = false;
        }

        private async Task MouseMove(MouseEventArgs e)
        {
            var x = e.ClientX;
            var y = e.ClientY;
            if (_isDragging)
            {
                var factor = 10d / _canvasReference.Height;
                var dx = factor * (x - _lastX);
                var dy = factor * (y - _lastY);
                _rotationX += dy;
                _rotationY += dx;
                await DrawSceneAsync();
            }
            _lastX = x;
            _lastY = y;
        }
    }
}
