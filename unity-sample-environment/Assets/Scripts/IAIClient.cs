using UnityEngine;
using System.Collections;

namespace MLPlayer {
	public interface IAIClient {
		void PushAIMessage(byte[] msg);
		void PushAgentState(State s);
		byte[] PopAIMessage();
		byte[] PopAgentState();
	}
}