using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using GLMatrixSharp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using WebGLSharp;

namespace WebGLCanvasExtension_Playground.Pages
{
    public partial class DataToTextureSample : ComponentBase, IDisposable
    {
        [Inject] private ILogger<DataToTextureSample> _logger { get; set; }
        [Inject] private HttpClient Http { get; set; }

        BECanvasComponent _canvasReference;
        ShaderProgram _shaderProgram;
        Mesh _cylinderMesh;
        WebGLSharp.Texture _texture;
        Light _light;

        bool _isDragging = false;
        double _lastX = 0, _lastY = 0;
        double _rotationX = 0, _rotationY = 0;

        Dictionary<int, int[]> _textureContainer = new Dictionary<int, int[]>();

        System.Timers.Timer _animationTimer;
        int currentFrame = 0;

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
            await Http.GetStringAsync("shaders/basic.vert"),
            await Http.GetStringAsync("shaders/basic.frag"),
            new List<string>() { "position", "normal", "uv" },
            new List<string>() { "model", "projection", "ambientLight", "lightDirection", "diffuse" });

                var geometry = Geometry.ParseObjFile(await Http.GetStringAsync("models/plane.obj"));
                int size = 16;
                GenerateTextureData(size, size, 4, 8, 1.8f, 100, true);
                _texture = await WebGLSharp.Texture.BuildAsync(gl, _textureContainer.GetValueOrDefault(0), size, size);
                _cylinderMesh = await Mesh.BuildAsync(gl, geometry, _texture);
                _light = new Light();
                await DrawSceneAsync();
                _animationTimer = new System.Timers.Timer(70);
                _animationTimer.Elapsed += new ElapsedEventHandler(async (o, e) => await AnimationTick());
                _animationTimer.Start();
            }
        }

        private async Task AnimationTick()
        {
            currentFrame++;
            currentFrame = currentFrame % _textureContainer.Count;
            Console.WriteLine($"{currentFrame}:{_textureContainer.GetValueOrDefault(currentFrame).Where(x => x > 0).ToArray().Length}");
            await _texture.UpdateData(_textureContainer.GetValueOrDefault(currentFrame));
            if (!_isDragging) await DrawSceneAsync();
        }

        private async Task DrawSceneAsync()
        {
            await _shaderProgram.GlContext.ClearAsync(BufferBits.COLOR_BUFFER_BIT | BufferBits.DEPTH_BUFFER_BIT);
            await _shaderProgram.GlContext.UseProgramAsync(_shaderProgram.Program);
            await _light.UseAsync(_shaderProgram);
            var projectionMatrix = Mat4.Perspective((float)(45 * Math.PI / 180), 1f, 0.1f, 100f);
            projectionMatrix = Mat4.Translate(projectionMatrix, new float[] { 0, 0, -4f });
            projectionMatrix = Mat4.Rotate(projectionMatrix, (float)_rotationX + 90, new float[3] { 1f, 0, 0f });
            projectionMatrix = Mat4.Rotate(projectionMatrix, (float)_rotationY, new float[3] { 0, 1f, 0f });
            await _shaderProgram.GlContext.UniformMatrixAsync(_shaderProgram.Uniforms.GetValueOrDefault("projection"), false, projectionMatrix);
            await _cylinderMesh.DrawAsync(_shaderProgram);
        }

        void GenerateTextureData(int width, int height, float falloffStart, float falloffEnd, float speed, int framesCount, bool isAlphaBuffer)
        {
            //Determine random starting position and mouvement direction
            Random rnd = new Random();
            double x = rnd.Next(width / 4, width * 3 / 4);
            double y = rnd.Next(height / 4, height * 3 / 4);
            double directionAngle = rnd.NextDouble() * 360;
            //
            int valuesPerPoint = isAlphaBuffer ? 4 : 3;
            for (int f = 0; f < framesCount; f++)
            {
                //Update position
                x += speed * Math.Cos(directionAngle);
                if (x <= 0 || x >= width)
                {
                    directionAngle = (directionAngle - 90 % 360);
                    x = x <= 0 ? 0 : width;
                }

                y += speed * Math.Sin(directionAngle);
                if (y <= 0 || y >= height)
                {
                    directionAngle = (directionAngle - 90 % 360);
                    y = y <= 0 ? 0 : height;
                }

                //Create texture    
                var texture = new int[width * height * valuesPerPoint];
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var dist = Math.Sqrt(Math.Pow(i - x, 2) + Math.Pow(j - y, 2));
                        var redIndex = i * width * valuesPerPoint + j * valuesPerPoint;
                        var value = (int)(Math.Min(falloffEnd, (int)dist) / (falloffEnd) * 255);
                        texture[redIndex] = 255 - value;
                        texture[redIndex + 1] = 0;
                        texture[redIndex + 2] = value;
                        //texture[redIndex + 2] = 10;
                        //texture[redIndex ] = 220;
                        //texture[redIndex + 1] = 0;
                        if (isAlphaBuffer) texture[redIndex + 3] = 255;
                    }
                }
                _textureContainer.Add(f, texture);
            }
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

        public void Dispose()
        {
            _animationTimer.Stop();
            _animationTimer.Dispose();
        }
    }
}
