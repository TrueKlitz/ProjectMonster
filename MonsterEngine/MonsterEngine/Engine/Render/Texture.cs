using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Diagnostics;

namespace MonsterEngine.Engine.Render
{
    class Texture
    {
       
        public Texture()
        {
        }

        public static int LoadTextureFromFile(string filename)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if ( String.IsNullOrEmpty(filename) | !File.Exists(filename))
                filename = ".../.../Engine/Render/standartTexture.png";

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);
            bmp.Dispose();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            
            sw.Stop();
            Console.WriteLine("Texture " + id + " has being loaded in " + sw.ElapsedMilliseconds + "ms");
            return id;
        }

        public static int CreateTexture(int width, int height, Color color)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(2, 2);
            bmp.SetPixel(0, 0, color);
            bmp.SetPixel(1, 0, color);
            bmp.SetPixel(1, 1, color);
            bmp.SetPixel(0, 1, color);
            bmp = new Bitmap(bmp, width,height);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);
            bmp.Dispose();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            
            sw.Stop();
            Console.WriteLine("Texture " + id + " has being created in " + sw.ElapsedMilliseconds + "ms");
            return id;
        }

        public static void BindTexture(ref int textureId, TextureUnit textureUnit, string UniformName, int _shaderProgrammHandle)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(GL.GetUniformLocation(_shaderProgrammHandle, UniformName), textureUnit - TextureUnit.Texture0);
        }

    }
}
