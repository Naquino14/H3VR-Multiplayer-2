using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate int av_format_control_message(AVFormatContext* s, int type, void* data, ulong data_size);
}
