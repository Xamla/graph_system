namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Graph.VideoComment")]
    public class VideoCommentModule
        : IFrameModuleBase
    {
        public VideoCommentModule(IGraphRuntime runtime)
            : base(runtime)
        {
            this.AddInputPin("Autoplay", PinDataTypeFactory.Create<bool>(), PropertyMode.Always);
        }
    }
}
