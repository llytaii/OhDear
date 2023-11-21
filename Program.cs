using System.Diagnostics;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;

namespace OhDear 
{
    class Program
    {
        private static Sdl2Window Window;
        private static GraphicsDevice GraphicsDevice;
        private static CommandList CommandList;
        private static ImGuiController ImGuiController;

        private static Vector3 ClearColor = new Vector3(0.45f, 0.55f, 0.6f);

        static void Main(string[] args)
        {
            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "ImGui.NET Sample Program"),
                new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
                out Window,
                out GraphicsDevice);
            Window.Resized += () =>
            {
                GraphicsDevice.MainSwapchain.Resize((uint)Window.Width, (uint)Window.Height);
                ImGuiController.WindowResized(Window.Width, Window.Height);
            };
            CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();
            ImGuiController = new ImGuiController(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, Window.Width, Window.Height);

            var stopwatch = Stopwatch.StartNew();
            float deltaTime = 0f;

            // Main application loop
            while (Window.Exists)
            {
                deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
                stopwatch.Restart();
                InputSnapshot snapshot = Window.PumpEvents();
                if (!Window.Exists) { break; }
                ImGuiController.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                SubmitUI();

                CommandList.Begin();
                CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
                CommandList.ClearColorTarget(0, new RgbaFloat(ClearColor.X, ClearColor.Y, ClearColor.Z, 1f));
                ImGuiController.Render(GraphicsDevice, CommandList);
                CommandList.End();
                GraphicsDevice.SubmitCommands(CommandList);
                GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
            }

            // Clean up Veldrid resources
            GraphicsDevice.WaitForIdle();
            ImGuiController.Dispose();
            CommandList.Dispose();
            GraphicsDevice.Dispose();
        }

        private static unsafe void SubmitUI()
        {
        }
    }
}
