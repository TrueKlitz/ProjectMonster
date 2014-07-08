﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

        public Camera(Core _core, Matrix4 _mCamera, Matrix4 _mProjection)
        {
            core = _core;
            fMovementSpeed = 0.14f;
            vPosition = new Vector3(-8.0f,-10.0f,-8.0f);
            mCamera = _mCamera;
            mProjection = _mProjection;
            fPitch = 100f;
            fYaw = 90f;
        }

        public void update()
        {
            vPosition += vMove;
            mCamera = Matrix4.CreateTranslation(vPosition) * Matrix4.CreateRotationY(core.helper.degToRad(fPitch)) * Matrix4.CreateRotationX(core.helper.degToRad(fYaw));

            SetCameraMatrix();
            SetProjectionMatrix();
        }

        public void SetShaderPointer(int _shaderProgramHandle)
        {
            uniformCameraMatrixPointer = GL.GetUniformLocation(_shaderProgramHandle, "camera_matrix");
            uniformProjectionMatrixPointer = GL.GetUniformLocation(_shaderProgramHandle, "projection_matrix");
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
