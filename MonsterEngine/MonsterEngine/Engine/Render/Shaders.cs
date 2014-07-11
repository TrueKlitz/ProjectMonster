using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace MonsterEngine.Engine.Render
{
    class Shaders
    {
        public int S1_shaderProgramHandle, S2_shaderProgramHandle;

        /*
         * Shader 1 (S1) is being used for Terrain
         * Shader 2 (S2) is being used for Game Objects
        */

        public Shaders()
        {
            LoadShaderOne();
            LoadShaderTwo();
        }
       
        private void LoadShaderOne()
        {
            int S1_vertexHandle, S1_fragmentHandle;
            string vertexShaderSource = @"
            #version 140
 
            // object space to camera space transformation
            uniform mat4 camera_matrix;            
 
            // camera space to clip coordinates
            uniform mat4 projection_matrix;

            // incoming vertex position
            in vec3 vertex_position;
 
            // incoming vertex normal
            in vec3 vertex_normal;
 
            in vec2 vertex_texCoord;

            // transformed vertex normal
            out vec3 normal;
            out vec2 TexCoord0;
            out float height;

            void main(void)
            {
              //not a proper transformation if modelview_matrix involves non-uniform scaling
              normal = (vec4( vertex_normal, 0 ) ).xyz;
              height = vertex_position.y;
              TexCoord0 = vertex_texCoord;
              // transforming the incoming vertex position
                gl_Position = projection_matrix * camera_matrix * vec4(vertex_position.xyz, 1 );
            }";
            string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            const vec3 ambient = vec3( 0.1, 0.1, 0.1 );
            const vec3 lightVecNormalized = normalize( vec3( 0.3, 0.25, 0.5 ) );
            const vec3 lightColor = vec3( 0.6, 0.6, 0.6 );
 
            uniform sampler2D texGrass;
            uniform sampler2D texRock;
            uniform sampler2D texSand;
            uniform sampler2D texGrassRock;
            uniform sampler2D texDirt;

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
              
              float diffuse = clamp( dot( lightVecNormalized, normalize( normal ) ), 0.0, 1.0 );
              out_frag_color = texture * vec4( ambient + diffuse * lightColor, 1.0 );
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
 
            // object space to camera space transformation
            uniform mat4 camera_matrix;            
 
            // camera space to clip coordinates
            uniform mat4 projection_matrix;
    
            //Object Position/Rotation/Scale...
            uniform mat4 modelview_matrix;

            // incoming vertex position
            in vec3 vertex_position;
 
            // incoming vertex normal
            in vec3 vertex_normal;
 
            in vec2 vertex_texCoord;

            // transformed vertex normal
            out vec3 normal;
            out vec2 TexCoord0;

            void main(void)
            {
              //not a proper transformation if modelview_matrix involves non-uniform scaling
              normal = (vec4( vertex_normal, 0 ) ).xyz;
              TexCoord0 = vertex_texCoord;
              // transforming the incoming vertex position
                gl_Position = projection_matrix * camera_matrix * modelview_matrix * vec4(vertex_position.xyz, 0.2 );
            }";
            string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            const vec3 ambient = vec3( 0.1, 0.1, 0.1 );
            const vec3 lightVecNormalized = normalize( vec3( 0.3, 0.25, 0.5 ) );
            const vec3 lightColor = vec3( 0.6, 0.6, 0.6 );
 
            uniform sampler2D texGround;

            in vec3 normal;
            in vec2 TexCoord0;

            out vec4 out_frag_color;

            void main(void)
            {
              vec4 texture = vec4(1,1,1,1);
              texture = texture2D(texGround, TexCoord0.xy ) ;
              
              float diffuse = clamp( dot( lightVecNormalized, normalize( normal ) ), 0.0, 1.0 );
              out_frag_color = texture * vec4( ambient + diffuse * lightColor, 1.0 );
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
            GL.BindAttribLocation(S2_shaderProgramHandle, 2, "vertex_texCoord");

        }

        public void SetAttributesShaderTwo(int tGround)
        {
            Texture.BindTexture(ref tGround, TextureUnit.Texture0, "texGround", S2_shaderProgramHandle);
        }
    }
}
