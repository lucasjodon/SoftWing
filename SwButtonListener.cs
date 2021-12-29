﻿using Android.Util;
using Android.Views;
using System;
using SoftWing.SwSystem;
using SoftWing.SwSystem.Messages;
using static Android.Views.View;
using Android.App;
using Xamarin.Essentials;

namespace SoftWing
{
    public class SwButtonListener : Java.Lang.Object, IOnTouchListener
    {
        private const String TAG = "SwButtonListener";
        private TimeSpan KEY_VIBRATION_TIME = TimeSpan.FromSeconds(0.01);
        private View button;
        private Keycode key = Keycode.Unknown;
        private bool vibrate_enabled = SwSettings.Default_Vibration_Enable;
        private MotionDescription motion = MotionDescription.InvalidMotion();
        private int motionId = MotionUpdateMessage.GetMotionId();
        private MessageDispatcher dispatcher;

        public SwButtonListener(View button_in, Keycode key_in, MotionDescription motion_in, bool vibrate_enable_in)
        {
            Log.Info(TAG, "SwButtonListener - key");
            button = button_in;
            key = key_in;
            motion = motion_in;
            dispatcher = MessageDispatcher.GetInstance(new Activity());
            vibrate_enabled = vibrate_enable_in;
        }

        ~SwButtonListener()
        {
            Log.Info(TAG, "~SwButtonListener");
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    Log.Info(TAG, "OnTouch - Down");
                    button.SetBackgroundColor(Android.Graphics.Color.SkyBlue);
                    if (vibrate_enabled)
                    {
                        Vibration.Vibrate(KEY_VIBRATION_TIME);
                    }
                    ReportEvent(ControlUpdateMessage.UpdateType.Pressed);
                    break;
                case MotionEventActions.Up:
                    Log.Info(TAG, "OnTouch - Up");
                    button.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    if (vibrate_enabled)
                    {
                        Vibration.Vibrate(KEY_VIBRATION_TIME);
                    }
                    ReportEvent(ControlUpdateMessage.UpdateType.Released);
                    break;
                default:
                    Log.Info(TAG, "OnTouch - Other");
                    break;
            }
            return true;
        }

        private void ReportEvent(ControlUpdateMessage.UpdateType update)
        {
            if (key != Keycode.Unknown)
            {
                dispatcher.Post(new ControlUpdateMessage(key, update));
                return;
            }
            dispatcher.Post(new MotionUpdateMessage(motionId, motion, update == ControlUpdateMessage.UpdateType.Released));
        }
    }
}
