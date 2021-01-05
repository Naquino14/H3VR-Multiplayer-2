using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate int AVOpenCallback(AVFormatContext* s, AVIOContext** pb, [MarshalAs(UnmanagedType.LPStr)] string url, int flags, AVIOInterruptCB* int_cb, AVDictionary** options);
}
