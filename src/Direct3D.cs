using MicroCom.Runtime;

namespace HXE;

// TODO: CsWin32

public class Direct3D
{
    /// <summary>
    /// Based on d3d9helper.h
    /// </summary>
    public static class D3D9Helper
    {
        /// <summary>
        ///     <see href=""/>
        /// </summary>
        public interface IDirect3D9 : IUnknown
        {
            //public static unsafe HRESULT EnumAdapterModes(
            //    uint Adapter,
            //    D3DFormat Format,
            //    uint Mode,
            //    out D3DDISPLAYMODE* pMode
            //);
        }
    }
}
