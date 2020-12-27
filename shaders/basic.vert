attribute vec3 position;
attribute vec3 normal;
attribute vec2 uv;
uniform mat4 model;
uniform mat4 projection;
varying vec3 vNormal;
varying vec2 vUv;

void main() {
    vUv = uv;
    vNormal = (model * vec4(normal, 0.)).xyz;
    gl_Position = projection * model * vec4(position, 1.);
}