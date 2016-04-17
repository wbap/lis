using UnityEngine;
using System.Collections;
using System.IO;
using WebSocketSharp;
using System.Threading;

namespace MLPlayer {
	public class Agent : MonoBehaviour {
		[SerializeField] Camera camera;
		[SerializeField] Camera depthCamera;
		[SerializeField] Texture2D image;
		[SerializeField] Texture2D depth;

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
			state.image = GetCameraImage (camera, ref image);
			state.depth = GetCameraImage (depthCamera, ref depth);
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
			image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height,
				TextureFormat.RGB24, false);
			depth = new Texture2D(depthCamera.targetTexture.width, depthCamera.targetTexture.height,
				TextureFormat.RGB24, false);

			depthCamera.depthTextureMode = DepthTextureMode.Depth;
			depthCamera.SetReplacementShader(Shader.Find("Custom/ReplacementShader"), "");
		}


		public byte[] GetCameraImage(Camera cam, ref Texture2D tex) {
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = cam.targetTexture;
			cam.Render();
			tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
			tex.Apply();
			RenderTexture.active = currentRT;

			return tex.EncodeToPNG ();
		}
	}
}