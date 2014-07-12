using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace MonsterEngine.Engine.Render
{
    class Camera
    {
        Core core;
        
        public Vector3 vMove;
        public Vector3 vPosition;
        public Matrix4 mCamera, mProjection;

        public float fMovementSpeed, fPitch, fYaw;
        public int uniformCameraMatrixPointer, uniformProjectionMatrixPointer;

        public Stopwatch sw = new Stopwatch();

        public Camera(Core _core, Matrix4 _mCamera, Matrix4 _mProjection)
        {
            core = _core;
            fMovementSpeed = 0.005f;
            vPosition = new Vector3(-8.0f,-10.0f,-8.0f);
            mCamera = _mCamera;
            mProjection = _mProjection;
            fPitch = 100f;
            fYaw = 90f;
        }

        public void update()
        {
            sw.Restart();
            vPosition += vMove * (float)core.dDeltaTime;
            mCamera = Matrix4.CreateTranslation(vPosition) * Matrix4.CreateRotationY(Helper.degToRad(fPitch)) * Matrix4.CreateRotationX(Helper.degToRad(fYaw));

            SetShaderPointer();
            sw.Stop(); 
        }

        public void SetShaderPointer()
        {
            GL.UseProgram(Core.game.shader.S1_shaderProgramHandle);
            uniformCameraMatrixPointer = GL.GetUniformLocation(Core.game.shader.S1_shaderProgramHandle, "camera_matrix");
            uniformProjectionMatrixPointer = GL.GetUniformLocation(Core.game.shader.S1_shaderProgramHandle, "projection_matrix");
            SetCameraMatrix();
            SetProjectionMatrix();
            GL.UseProgram(Core.game.shader.S2_shaderProgramHandle);
            uniformCameraMatrixPointer = GL.GetUniformLocation(Core.game.shader.S2_shaderProgramHandle, "camera_matrix");
            uniformProjectionMatrixPointer = GL.GetUniformLocation(Core.game.shader.S2_shaderProgramHandle, "projection_matrix");
            SetCameraMatrix();
            SetProjectionMatrix();  
        }

        public void SetCameraMatrix()
        {
            GL.UniformMatrix4(uniformCameraMatrixPointer, false, ref mCamera);
        }

        public void SetProjectionMatrix()
        {
            GL.UniformMatrix4(uniformProjectionMatrixPointer, false, ref mProjection);
        }

    }
}
