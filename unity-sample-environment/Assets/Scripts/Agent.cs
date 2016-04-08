using UnityEngine;
using System.Collections;
using System.IO;
using WebSocketSharp;
using System.Threading;

namespace MLPlayer {
	public class Agent : MonoBehaviour {
		[SerializeField] Camera camera;
		[SerializeField] Camera depthCamera;

		public Action action { set; get; }
		public State state { set; get; }

		public void AddReward (float reward)
		{
			if (!state.endEpisode) {
				state.reward += reward;
			}
		}

		public void UpdateState ()
		{
			state.image = GetCameraImage (camera);
			state.depth = GetCameraImage (depthCamera);
		}
		
		public void ResetState ()
		{
			state.Clear ();
		}

		public void StartEpisode ()
		{
			
		}

		public void EndEpisode ()
		{
			state.endEpisode = true;
		}

		public void Start() {
			action = new Action ();
			state = new State ();

			depthCamera.depthTextureMode = DepthTextureMode.Depth;
			depthCamera.SetReplacementShader(Shader.Find("Custom/ReplacementShader"), "");
		}


		public byte[] GetCameraImage(Camera cam) {
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = cam.targetTexture;
			cam.Render();
			Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height,
			                                TextureFormat.RGB24, false);
			image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
			image.Apply();
			RenderTexture.active = currentRT;
			byte[] bytes = image.EncodeToPNG ();
			Destroy (image);

			return bytes;
		}
	}
}