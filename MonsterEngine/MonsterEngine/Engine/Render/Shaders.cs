using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace MonsterEngine.Engine.Render
{
    class Shaders
    {
        public int S1_shaderProgramHandle, S2_shaderProgramHandle;

        /*
         * Shader 1 (S1) is being used for Terrain
         * Shader 2 (S2) is being used for Game Objects
        */

        public Vector3 directionalLight, ambient, directionalLightColor;

        public Shaders()
        {
            directionalLightColor = new Vector3(0.8f,0.9f,0.8f);
            directionalLight = Vector3.Normalize(new Vector3(0.8f, 0.7f, 0.2f));
            ambient = new Vector3(0.3f, 0.3f, 0.2f);
            
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
            GL.UseProgram(S2_shaderProgramHandle);
            GL.Uniform3(GL.GetUniformLocation(S2_shaderProgramHandle, "directionalLight"), directionalLight);
            GL.Uniform3(GL.GetUniformLocation(S2_shaderProgramHandle, "directionalLightColor"), directionalLightColor);
            GL.Uniform3(GL.GetUniformLocation(S2_shaderProgramHandle, "ambient"), ambient);
        }

        private void LoadShaderOne()
        {
            int S1_vertexHandle, S1_fragmentHandle;
            string vertexShaderSource = @"
            #version 140

            uniform mat4 camera_matrix;            
            uniform mat4 projection_matrix;

            in vec3 vertex_position;
            in vec3 vertex_normal;
            in vec2 vertex_texCoord;

            out vec3 normal;
            out vec2 TexCoord0;
            out float height;

            void main(void)
            {
              normal = (vec4( vertex_normal, 0 ) ).xyz;
              height = vertex_position.y;
              TexCoord0 = vertex_texCoord;
              gl_Position = projection_matrix * camera_matrix * vec4(vertex_position.xyz, 1 );
            }";
            string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            uniform sampler2D texGrass;
            uniform sampler2D texRock;
            uniform sampler2D texSand;
            uniform sampler2D texGrassRock;
            uniform sampler2D texDirt;
            uniform vec3 ambient;
            uniform vec3 directionalLight;
            uniform vec3 directionalLightColor;  
            
            in vec3 normal;
            in vec2 TexCoord0;
            in float height;

            out vec4 out_frag_color;

            void main(void)
            {
              vec4 texture = vec4(1,1,1,1);
              if(height >= 0.00f && height <= 1.0f){ texture = texture2D(texSand, TexCoord0.xy);}
              if(height >= 1.0f && height <= 1.25f){
                float lHeight = (height - 1.0f) * 4.0f;
                texture = ( ( texture2D(texGrass, TexCoord0.xy) * lHeight ) + ( texture2D(texSand, TexCoord0.xy) * (1.0f-lHeight) ) );
              }
              if(height >= 1.25f  && height <= 2.0f){ texture = texture2D(texGrass, TexCoord0.xy);}
              if(height >= 2.0f && height <= 2.25f){
                float lHeight = (height - 2.0f) * 4.0f;
                texture = ( ( texture2D(texGrassRock, TexCoord0.xy) * lHeight ) + ( texture2D(texGrass, TexCoord0.xy) * (1.0f-lHeight) ) );
              }
              if(height >= 2.25f && height <= 3.0f){ texture = texture2D(texGrassRock, TexCoord0.xy);}
              if(height >= 3.0f && height <= 3.25f){
                float lHeight = (height - 3.0f) * 4.0f;
                texture = ( ( texture2D(texRock, TexCoord0.xy) * lHeight ) + ( texture2D(texGrassRock, TexCoord0.xy) * (1.0f-lHeight) ) );
              }
              if(height >= 3.25f){ texture = texture2D(texRock, TexCoord0.xy);}
              if(TexCoord0.x < 0.0f || TexCoord0.y < 0.0f){
                texture = vec4(0,0,0,1);
              }   
              
              float diffuse = clamp( dot( directionalLight, normalize( normal ) ), 0.0, 1.0 );
              out_frag_color = texture * vec4( ambient + diffuse * directionalLightColor, 1.0 );
            }";

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
            Console.WriteLine("\n" + programInfoLog);

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
            string vertexShaderSource = @"
            #version 140

            uniform mat4 camera_matrix;            
            uniform mat4 projection_matrix;
            uniform mat4 modelview_matrix;

            in vec3 vertex_position;
            in vec3 vertex_normal;
            in vec2 vertex_texCoord0;
            in vec3 vertex_tangent;

            out mat3 tbnMatrix;
            out vec2 TexCoord0;
            out vec3 worldPos0;

            void main(void)
            { 
              worldPos0 = (modelview_matrix * vec4(vertex_position.xyz , 1.0 )).xyz;
              vec3 n = normalize((modelview_matrix * vec4(vertex_normal , 0.0 )).xyz);
              vec3 t = normalize((modelview_matrix * vec4(vertex_tangent , 0.0 )).xyz);
              t = normalize(t - dot(t, n) * n);

              vec3 biTangent = cross(t, n);

              tbnMatrix = mat3(t, biTangent, n);
              TexCoord0 = vertex_texCoord0;
              gl_Position = projection_matrix * camera_matrix * modelview_matrix * vec4(vertex_position.xyz , 1.0 );
            }";
            string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            uniform sampler2D texGround;
            uniform sampler2D texNormal;
            uniform vec3 ambient;
            uniform vec3 directionalLight;
            uniform vec3 directionalLightColor;  

            in mat3 tbnMatrix;
            in vec2 TexCoord0;
            in vec3 worldPos0;            

            out vec4 out_frag_color;

            void main(void)
            {
              vec3 normal = normalize(tbnMatrix * (2 * texture2D(texNormal, TexCoord0).xyz - 1));
              float diffuse = clamp( dot( directionalLight, normal ), 0.0, 1.0 );
              out_frag_color = texture2D( texGround , TexCoord0.xy ) * vec4( ambient + diffuse * directionalLightColor, 1.0 );
            }";

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
            Console.WriteLine("\n" + programInfoLog);

            GL.BindAttribLocation(S2_shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(S2_shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(S2_shaderProgramHandle, 2, "vertex_texCoord0");
            GL.BindAttribLocation(S2_shaderProgramHandle, 3, "vertex_tangent");
        }

        public void SetAttributesShaderTwo(int tGround,int tNormal)
        {
            Texture.BindTexture(ref tGround, TextureUnit.Texture0, "texGround", S2_shaderProgramHandle);
            Texture.BindTexture(ref tNormal, TextureUnit.Texture1, "texNormal", S2_shaderProgramHandle);
        }
    }
}
