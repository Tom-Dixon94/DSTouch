﻿using UnityEngine;
using DS4Api;

public class DataDumpTest : MonoBehaviour
{
    private DS4 controller;

    private Rect windowRect = new Rect(300, 20, 470, 300);

    private DS4Data data;

    public Transform Visual;

    private myPoint lastPadPos;

    void Update()
    {
        if (!DS4Manager.HasWiimote()) { return; }

        controller = DS4Manager.Controllers[0];

        DS4Data tentative = data;
        do
        {
            data = tentative;
            tentative = controller.ReadDS4Data();
        } while (tentative != null);

        if (Visual != null)
            Visual.rotation = data.Orientation.Orientation;

        //if (data.TouchButton)
        //    MouseHook.SendClick();
    }

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 300, Screen.height), "");

        GUILayout.BeginVertical(GUILayout.Width(300));
        GUILayout.Label("DS4 Found: " + DS4Manager.HasWiimote());
        if (GUILayout.Button("Find DS4"))
            DS4Manager.FindWiimotes();

        if (GUILayout.Button("Cleanup"))
        {
            DS4Manager.Cleanup(controller);
            controller = null;
        }

        if (data == null)
            return;

        GUILayout.Label("Trackpad Button: " + data.TouchButton);
        GUILayout.Label("Trackpad Finger 1: (" + data.Touches[0, 0] + ", " + data.Touches[0, 1] + ")");
        GUILayout.Label("Trackpad Finger 2: (" + data.Touches[1, 0] + ", " + data.Touches[1, 1] + ")");
       
        GUILayout.Label("X: " + data.Cross);
        GUILayout.Label("\u25cb: " + data.Circle);
        GUILayout.Label("\u25a1: " + data.Square);
        GUILayout.Label("\u25b3: " + data.Triangle);

        GUILayout.Label("PS: " + data.PS);
        GUILayout.Label("Share: " + data.Share);
        GUILayout.Label("Options: " + data.Options);

        GUILayout.Label("D-Pad Up: " + data.DpadUp);
        GUILayout.Label("D-Pad Down: " + data.DpadDown);
        GUILayout.Label("D-Pad Left: " + data.DpadLeft);
        GUILayout.Label("D-Pad Right: " + data.DpadRight);

        GUILayout.Label("Left Stick: (" + data.lstick[0] + "," + data.lstick[1] + ")");
        GUILayout.Label("Right Stick: (" + data.rstick[0] + "," + data.rstick[1] + ")");

        GUILayout.Label("L1: " + data.L1);
        GUILayout.Label("R1: " + data.R1);
        GUILayout.Label("L2: " + data.L2 + " (" + data.L2_analog + ")");
        GUILayout.Label("R2: " + data.R2 + " (" + data.R2_analog + ")");
        GUILayout.Label("L3: " + data.L3);
        GUILayout.Label("R3: " + data.R3);
 //myPoint lastPos;
        
 //       //make center of touchpad center screen -- lastposition + (touched - max/2)
 //       int maxDiv = 4095 / 2;
 //       myPoint p = new myPoint(lastPadPos.x + (maxDiv - data.Touches[0, 0]), lastPadPos.y + (data.Touches[0, 1] - maxDiv));
 //       SetCursor.SetCursorPos(p.x, p.y);
 //       SetCursor.GetCursorPos(out lastPadPos);
        //print("cursor pos" + p);
        //GUILayout.Label("Trackpad Finger 3: (" + data.Touches[2, 0] + ", " + data.Touches[2, 1] + ")");

        GUILayout.Label("Gyro: " + data.Orientation.Gyro_Raw);
        GUILayout.Label("Rotation: " + data.Orientation.Orientation.eulerAngles);
        GUILayout.Label("Accel: " + data.Orientation.Accel_Raw);
        GUILayout.Label("Accel Standard Deviation: " + data.Orientation.Accel_Deviation);
        GUILayout.Label("Accel Magnitude: " + data.Orientation.Accel_Raw.magnitude);

        GUILayout.EndVertical();

        if (controller != null)
            windowRect = GUI.Window(0, windowRect, DataWindow, "Data");
    }

    private Vector2 scrollPosition = Vector2.zero;

    void DataWindow(int id)
    {
        byte[] data = controller.dump;

        GUILayout.BeginVertical(GUILayout.Width(470), GUILayout.Height(300));
        GUILayout.Space(20);

        GUILayout.BeginHorizontal(GUILayout.Height(25));
        GUILayout.Space(10);
        GUILayout.Label("##", GUILayout.Width(40));
        GUILayout.Label("Val", GUILayout.Width(40));
        for (int x = 7; x >= 0; x--)
            GUILayout.Label(x.ToString(), GUILayout.Width(40));
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(240));

        for (int x = 0; x < data.Length; x++)
        {
            byte val = data[x];

            GUILayout.BeginHorizontal(GUILayout.Height(25));
            GUILayout.Space(10);
            GUILayout.Label(x.ToString(), GUILayout.Width(40));
            GUILayout.Label(val.ToString("X2"), GUILayout.Width(40));
            byte bit = (byte)0x80;
            for (int i = 0; i < 8; i++)
            {
                bool flipped = (val & bit) == bit;
                GUILayout.Label(flipped ? "1" : "0", GUILayout.Width(40));

                bit = (byte)(bit >> 1);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}