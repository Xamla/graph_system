using Xamla.Graph.MethodModule;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Graph.Controls.ViewString")]
    class ViewStringModule
        : SingleInstanceMethodModule
    {
        public ViewStringModule(IGraphRuntime runtime)
            : base(runtime, ModuleKind.ViewModule)
        {
        }

        [ModuleMethod]
        public void View(
            [InputPin(PropertyMode = PropertyMode.Never, Editor = WellKnownEditors.Hidden)] string data,
            [InputPin(PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.Hidden)] string input,
            [InputPin(PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.Hidden)] int textAreaWidth = 100,
            [InputPin(PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.Hidden)] int textAreaHeight = 40,
            [InputPin(PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.IntNumber)] int fontSize = 14
        )
        {
            this.Properties.SetValue("input", data);
        }
    }
}
