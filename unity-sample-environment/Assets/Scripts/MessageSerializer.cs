using MsgPack;

namespace MLPlayer {
	
	/// <summary>
	/// MsgPack wrapper.
	/// </summary>
	public class MessageSerializer {
	  
#if UNITY_IOS
		ObjectPacker packer = new ObjectPacker();
#else
		CompiledPacker packer = new CompiledPacker();
#endif
		
		public byte[] Pack(object o) {
			return packer.Pack(o);
		}
		
		public T Unpack<T>(byte[] buf) {
			return packer.Unpack<T>(buf);
		}
	}
}
