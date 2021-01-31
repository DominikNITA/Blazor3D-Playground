# 3D world in Blazor - Playground

Live version is hosted on [Github Pages](https://dominiknita.github.io/Blazor3D-Playground/)

## How to clone project

TODO => merge submodules instructions

## About project & code structure


This project contains my latest tests & discoveries in domain of 3D rendering in Blazor and the code is available [here](https://github.com/DominikNITA/Blazor3D-Playground). I have decided to go with **WebGL** for now (check the list at the bottom for other possible technologies) because of it's maturity and wide adoption on any type of device. The objective is to write 3D apps with as few lines of JavaScript as possible.

My research in this topic started with [BlazorExtensions/Canvas](https://github.com/BlazorExtensions/Canvas) nuget package. However the package is **no longer supported or developed** and has a few **not working/bugged/not implemented** features - so I decided to **fork & modify** it to my needs and you can check the code right [here](https://github.com/DominikNITA/Canvas).

**3D graphics necessitates the use of matrices**, unfortunately none of availables math libraries provides functionnality similar to [GlMatrix.js](http://glmatrix.net/) - very popular library for WebGL written in JavaScript. I had to create my own simplified version of it. The library is very limited for now, so it's not published in it's own repository on Github and you can only see it in the [GLMatrixSharp folder in the main repo](https://github.com/DominikNITA/Blazor3D-Playground/tree/master/GLMatrixSharp).

During the development I have noticed that I need to write similar bites of code nearly all the time. For this reason I have created a **proof of concept for a project with helper classes** to create 3D world faster and with less bugs. For now it has classes for managing **loading models, textures, allocating buffers etc.** The code is located in the [WebGLSharp folder in the main repo](https://github.com/DominikNITA/Blazor3D-Playground/tree/master/WebGLSharp).


## Samples

List of currently available samples:

*   **Simple rotating cube** - quick animation created following Mozilla MSDN tutorial on WebGL
*   **Model loading** - loading custom mesh from .obj file using my custom library
*   **Camera control with mouse** - rotating the camera around an object using mouse events
*   **Data to texture (WIP)** - streaming data to an texture and displaying it in form of animation

## Todo List

*   Modify helper classes to make them less opinionated and shader dependend
*   Add inverse method to matrix library

## Other technologies to check

*   **Unity3d =>** custom interop layer with JavaScript
*   **WaveEngine =>** .net5 and compiles to webassembly, but is lacking documentation and is bugged and clearly not production ready
*   **WebGPU =>** still experimental and implementation could be similar to WebGL
*   **JavaScript 3D library =>** robust engine and plenty of solved issues, but necessitates a custom interop layer that could harm the performance
*   **And many many other solutions...**
