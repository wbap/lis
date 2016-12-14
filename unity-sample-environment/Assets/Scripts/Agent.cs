using UnityEngine;
using System.Collections.Generic;
using System.IO;
using WebSocketSharp;
using System.Threading;

namespace MLPlayer {
	public class Agent : MonoBehaviour {
		[SerializeField] List<Camera> rgbCameras;
		[SerializeField] List<Camera> depthCameras;
		[SerializeField] List<Texture2D> rgbImages;
		[SerializeField] List<Texture2D> depthImages;

		public Action action { set; get; }
		public State state { set; get;}


		public void AddReward (float reward)
		{
			if (!state.endEpisode) {
				state.reward += reward;
			}
		}

		public void UpdateState ()
		{
			state.image = new byte[rgbCameras.Count][];
			for (int i=0; i<rgbCameras.Count; i++) {
				Texture2D txture = rgbImages [i];
				state.image[i] = GetCameraImage (rgbCameras[i], ref txture);
			}
			state.depth = new byte[depthCameras.Count][];
			for (int i=0; i<depthCameras.Count; i++) {
				Texture2D txture = depthImages [i];
				state.depth[i] = GetCameraImage (depthCameras[i], ref txture);
			}
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
		
			rgbImages = new List<Texture2D> (rgbCameras.Capacity);
			foreach (var cam in rgbCameras) {
				rgbImages.Add (new Texture2D (cam.targetTexture.width, cam.targetTexture.height,
					TextureFormat.RGB24, false));
			}
			depthImages = new List<Texture2D> (rgbCameras.Capacity);
			foreach (var cam in depthCameras) {
				depthImages.Add(new Texture2D (cam.targetTexture.width, cam.targetTexture.height,
					TextureFormat.RGB24, false));
			}

			foreach (var cam in depthCameras) {
				cam.depthTextureMode = DepthTextureMode.Depth;
				cam.SetReplacementShader (Shader.Find ("Custom/ReplacementShader"), "");
			}
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
