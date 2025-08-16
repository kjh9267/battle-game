﻿using UnityEngine;
using System.Collections;
using TWC;

namespace TWC.Demo
{
	public class CameraFollow : MonoBehaviour
	{
		public TileWorldCreator tileWorldCreator;
		public Vector3 offset;
		public float smoothTime = 0.3F;
		
		private Vector3 velocity = Vector3.zero;
		private Transform target;
		
		
		void OnEnable()
		{
			tileWorldCreator.OnBuildLayersComplete += MapReady;
		}
		
		void OnDisable()
		{
			tileWorldCreator.OnBuildLayersComplete -= MapReady;
		}
		
		
		// Assign player object to target after map has been built
		void MapReady(TileWorldCreator _twc)
		{
			PlayerMovement _player = null; 
			#if UNITY_2022_3_OR_NEWER
			_player = GameObject.FindAnyObjectByType<PlayerMovement>(); 
			#else
			_player = GameObject.FindObjectOfType<PlayerMovement>(); 
			#endif
			
			target = _player.transform;
		}
		
		void Update()
		{
			if (target == null)
				return;
				
				
			Vector3 targetPosition = target.position + offset;
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		}
	}
}