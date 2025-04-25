using UnityEngine;

namespace Masot.Standard.Shaders
{
    public interface IComputeShaderWrapper
    {
        bool ShouldDispatch();
        void PreDispatch(ComputeShader shader, int kernel, RenderTexture texture, out Vector3Int threads);
        void AfterDispatch(ComputeShader shader, int kernel, RenderTexture texture, in Vector3Int threads);
        void RenderShader(ref RenderTexture output, bool clearFrame = true);
    }

    public abstract class ComputeShaderBase : IComputeShaderWrapper
    {
        private ComputeShader shader;
        private string shaderKernel;

        protected ComputeShaderBase(ComputeShader shaderFile, string shaderKernel)
        {
            this.shader = shaderFile;
            this.shaderKernel = shaderKernel;
        }

        public virtual void RenderShader(ref RenderTexture output, bool clearFrame = true)
        {
            if (!ShouldDispatch())
            {
                return;
            }

            if (clearFrame ||
                output is null)
            {
                if (output != null)
                    output.Release();

                output = new RenderTexture(Screen.width, Screen.height, 24);
                output.enableRandomWrite = true;
                output.Create();
            }

            Debug.Assert(output != null);

            int kernel = shader.FindKernel(shaderKernel);

            //resolution of the texture / screen
            shader.SetInts("_ResultResolution", Screen.width, Screen.height);

            //set texture
            shader.SetTexture(kernel, "_Result", output);

            Vector3Int threads;

            PreDispatch(shader, kernel, output, out threads);
            shader.Dispatch(kernel, threads.x, threads.y, threads.z);
            AfterDispatch(shader, kernel, output, threads);
        }

        public abstract bool ShouldDispatch();
        public abstract void AfterDispatch(ComputeShader shader, int kernel, RenderTexture texture, in Vector3Int threads);
        public abstract void PreDispatch(ComputeShader shader, int kernel, RenderTexture texture, out Vector3Int threads);
    }
}