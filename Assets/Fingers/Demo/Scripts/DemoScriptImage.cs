﻿//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    public class DemoScriptImage : MonoBehaviour
    {
        public FingersImageGestureHelperComponentScript ImageScript;
        public ParticleSystem MatchParticleSystem;
        public AudioSource AudioSourceOnMatch;

        private void LinesUpdated(object sender, System.EventArgs args)
        {
            //Debug.LogFormat("Lines updated, new point: {0},{1}", ImageScript.Gesture.FocusX, ImageScript.Gesture.FocusY);
        }

        private void LinesCleared(object sender, System.EventArgs args)
        {
            //Debug.LogFormat("Lines cleared!");
        }

        private void Start()
        {
            ImageScript.LinesUpdated += LinesUpdated;
            ImageScript.LinesCleared += LinesCleared;
        }

        private void LateUpdate()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                ImageScript.Reset();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                ImageGestureImage match = ImageScript.CheckForImageMatch();
                if (match != null)
                {
                    Debug.Log("Found image match: " + match.Name + ". Width: " + match.Width + ", Height: " + match.Height);
                    MatchParticleSystem.Play();
                    AudioSourceOnMatch.Play();
                }
                else
                {
                    Debug.Log("No match found!");
                }

                // TODO: Do something with the match
                // You could get a texture from it:
                // Texture2D texture = FingersImageAutomationScript.CreateTextureFromImageGestureImage(match);
            }
        }
    }
}
