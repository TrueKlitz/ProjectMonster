using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.IO;

namespace MonsterEngine.Engine.Render
{
    class Shaders
    {
        public int S1_shaderProgramHandle, S2_shaderProgramHandle;

        /*
         * Shader 1 (S1) is being used for Terrain
         * Shader 2 (S2) is being used for Game Objects
        */

        public Vector3 directionalLight, ambient, directionalLightColor, gamma;

        public Shaders()
        {
            directionalLightColor = new Vector3(0.3f, 0.3f, 0.3f);
            directionalLight = Vector3.Normalize(new Vector3(0.2f, 0.5f, 0.45f));
            ambient = new Vector3(0.1f, 0.1f, 0.1f);
            gamma = new Vector3( 0.9f, 0.9f, 0.9f);
            LoadShaderOne();
            LoadShaderTwo();
            UpdateUniforms();
        }
       
        public void UpdateUniforms()
        {
            GL.UseProgram(S1_shaderProgramHandle);
            GL.Uniform3(GL.GetUniformLocation(S1_shaderProgramHandle, "directionalLight"), directionalLight);
            GL.Uniform3(GL.GetUniformLocation(S1_shaderProgramHandle, "directionalLightColor"), directionalLightColor);
            GL.Uniform3(GL.GetUniformLocation(S1_shaderProgramHandle, "ambient"),  ambient);
            GL.Uniform3(GL.GetUniformLocation(S1_shaderProgramHandle, "gamma"), gamma);
            GL.UseProgram(S2_shaderProgramHandle);
            GL.Uniform3(GL.GetUniformLocation(S2_shaderProgramHandle, "directionalLight"), directionalLight);
            GL.Uniform3(GL.GetUniformLocation(S2_shaderProgramHandle, "directionalLightColor"), directionalLightColor);
            GL.Uniform3(GL.GetUniformLocation(S2_shaderProgramHandle, "ambient"), ambient);
            GL.Uniform3(GL.GetUniformLocation(S2_shaderProgramHandle, "gamma"), gamma);
        }

        private void LoadShaderOne()
        {
            int S1_vertexHandle, S1_fragmentHandle;
            string vertexShaderSource = File.ReadAllText(".../.../Engine/Shaders/terrain.vsh");

            string fragmentShaderSource = File.ReadAllText(".../.../Engine/Shaders/terrain.fsh");

            S1_vertexHandle = GL.CreateShader(ShaderType.VertexShader);
            S1_fragmentHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(S1_vertexHandle, vertexShaderSource);
            GL.ShaderSource(S1_fragmentHandle, fragmentShaderSource);

            GL.CompileShader(S1_vertexHandle);
            GL.CompileShader(S1_fragmentHandle);

            S1_shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(S1_shaderProgramHandle, S1_vertexHandle);
            GL.AttachShader(S1_shaderProgramHandle, S1_fragmentHandle);

            GL.LinkProgram(S1_shaderProgramHandle);
            GL.UseProgram(S1_shaderProgramHandle);
            string programInfoLog;
            GL.GetProgramInfoLog(S1_shaderProgramHandle, out programInfoLog);
            if (programInfoLog != "") Console.WriteLine(programInfoLog); else Console.WriteLine("Shader 1 was successfully compiled");

            GL.BindAttribLocation(S1_shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(S1_shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(S1_shaderProgramHandle, 2, "vertex_texCoord");
        }

        public void SetAttributesShaderOne(int tGrass, int tRock,int tSand,int tGrassRock, int tDirt)
        {
            Texture.BindTexture(ref tGrass, TextureUnit.Texture0, "texGrass", S1_shaderProgramHandle);
            Texture.BindTexture(ref tRock, TextureUnit.Texture1, "texRock", S1_shaderProgramHandle);
            Texture.BindTexture(ref tSand, TextureUnit.Texture2, "texSand", S1_shaderProgramHandle);
            Texture.BindTexture(ref tGrassRock, TextureUnit.Texture3, "texGrassRock", S1_shaderProgramHandle);
            Texture.BindTexture(ref tDirt, TextureUnit.Texture4, "texDirt", S1_shaderProgramHandle);
        }

        private void LoadShaderTwo()
        {
            int S2_vertexHandle, S2_fragmentHandle;

            string vertexShaderSource = File.ReadAllText(".../.../Engine/Shaders/lightning.vsh");
            string fragmentShaderSource = File.ReadAllText(".../.../Engine/Shaders/lightning.fsh");

            S2_vertexHandle = GL.CreateShader(ShaderType.VertexShader);
            S2_fragmentHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(S2_vertexHandle, vertexShaderSource);
            GL.ShaderSource(S2_fragmentHandle, fragmentShaderSource);

            GL.CompileShader(S2_vertexHandle);
            GL.CompileShader(S2_fragmentHandle);

            S2_shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(S2_shaderProgramHandle, S2_vertexHandle);
            GL.AttachShader(S2_shaderProgramHandle, S2_fragmentHandle);

            GL.LinkProgram(S2_shaderProgramHandle);
            GL.UseProgram(S2_shaderProgramHandle);
            string programInfoLog;
            GL.GetProgramInfoLog(S2_shaderProgramHandle, out programInfoLog);
            if (programInfoLog != "") Console.WriteLine(programInfoLog); else Console.WriteLine("Shader 2 was successfully compiled");

            GL.BindAttribLocation(S2_shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(S2_shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(S2_shaderProgramHandle, 2, "vertex_texCoord0");
            GL.BindAttribLocation(S2_shaderProgramHandle, 3, "vertex_tangent");
        }

        public void SetAttributesShaderTwo(int tGround, int tNormal, bool normalMapping, float specluar)
        {
            Texture.BindTexture(ref tGround, TextureUnit.Texture0, "texGround", S2_shaderProgramHandle);
            Texture.BindTexture(ref tNormal, TextureUnit.Texture1, "texNormal", S2_shaderProgramHandle);
            GL.UseProgram(S2_shaderProgramHandle);
            GL.Uniform1(GL.GetUniformLocation(S2_shaderProgramHandle, "normalMapping"), normalMapping ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(S2_shaderProgramHandle, "specluar"), specluar);
        }
    }
}
