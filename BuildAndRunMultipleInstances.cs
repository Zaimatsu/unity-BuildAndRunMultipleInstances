using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEditor;
using UnityEngine;

public static class BuildAndRunMultipleInstances
{
	private static ResolutionDialogSetting currentResolutionDialogSetting;

	[MenuItem("Build/Build and Run/1 instance")]
	private static void PerformWin64Build1()
	{
		PerformWin64Build(1);
	}

	[MenuItem("Build/Build and Run/2 instances")]
	private static void PerformWin64Build2()
	{
		PerformWin64Build(2);
	}

	[MenuItem("Build/Build and Run/3 instances")]
	private static void PerformWin64Build3()
	{
		PerformWin64Build(3);
	}

	[MenuItem("Build/Build and Run/4 instances")]
	private static void PerformWin64Build4()
	{
		PerformWin64Build(4);
	}

	private static void PerformWin64Build(int playerCount)
	{
		PrepareBuildSettings();

		UnityEditor.Build.Reporting.BuildReport report = null;
		report = BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Win64/" + GetProjectName() + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

		for (int i = 0; i < playerCount; i++)
		{
			Process gameProcess = new Process();
			gameProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			gameProcess.StartInfo.FileName = report.summary.outputPath;
			gameProcess.Start();

			// TODO wait until windowHandle is of newly opened process
			// better windowHandle detection (maybe find and change name?)
			Thread.Sleep(1000);


			// TODO set window resoution from external configuration
			IntPtr windowHandle = GetWindowHandle();
			SetWindowPos(windowHandle, 0, (i % 2) * 680, (i / 2 % 2) * 520, 0, 0, 0x0001);
		}

		RestoreSettings();
	}

	private static void PrepareBuildSettings()
	{
		currentResolutionDialogSetting = PlayerSettings.displayResolutionDialog;
		PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
	}

	private static void RestoreSettings()
	{
		PlayerSettings.displayResolutionDialog = currentResolutionDialogSetting;
	}

	private static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];
	}

	private static string[] GetScenePaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];

		for (int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}

		return scenes;
	}

	public static System.IntPtr GetWindowHandle()
	{
		return GetForegroundWindow();
	}

	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	[DllImport("user32.dll")]
	static extern IntPtr GetForegroundWindow();
}