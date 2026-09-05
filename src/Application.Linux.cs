#if LINUX
using System;

namespace HXE
{
    internal class Application
    {
#pragma warning disable HXE9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable CA1822 // Mark members as static
        public bool Run(Window window)
#pragma warning restore CA1822 // Mark members as static
        {
            if (window is Settings settings)
            {
                Settings.Save(settings);
                settings.Close();
            }
            else if (window is Positions positions)
            {
                // Positions.BrowseSource(positions);
                // Positions.BrowseTarget(positions);
                // Positions.Save(positions);
                Positions.Cancel(positions);

            }
            else throw new NotSupportedException();
            return true;
        }
#pragma warning disable CA1822 // Mark members as static
        public void Shutdown() { }
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore HXE9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
#endif
