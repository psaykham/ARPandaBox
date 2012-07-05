/*==============================================================================
            Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
            All Rights Reserved.
            Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraDeviceMenu : MonoBehaviour
{
    #region NESTED

    // Defines which menu to show.
    private enum MenuMode
    {
        MENU_OFF,            // Do not show a menu (default).
        MENU_CAMERA_OPTIONS, // Show camera device options.
        MENU_FOCUS_MODES     // Show focus mode menu.
    }

    #endregion // NESTED



    #region PRIVATE_MEMBER_VARIABLES

    // Currently active menu.
    private MenuMode mMenuToShow = MenuMode.MENU_OFF;

    // Check if a menu button has been pressed.
    private bool mButtonPressed = false;

    // Check if flash is currently enabled.
    private bool mFlashEnabled = false;

    // Contains the currently set auto focus mode.
    private CameraDevice.FocusMode mFocusMode =
        CameraDevice.FocusMode.FOCUS_MODE_NORMAL;

    // Contains the rectangle for the camera options menu.
    private Rect mAreaRect;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNTIY_MONOBEHAVIOUR_METHODS

    public void Start()
    {
        // Setup position and size of the camera menu.
        computePosition();
    }


    public void OnApplicationPause(bool pause)
    {
        // Upon resuming reactivate the torch if it was active before pausing:
        if (!pause)
        {
            if (mFlashEnabled)
            {
                mFlashEnabled = CameraDevice.Instance.SetFlashTorchMode(true);
            }
        }
    }

    
    public void Update()
    {		
        // If the touch event results from a button press it is ignored.
        if (!mButtonPressed)
        {
            // If finger is removed from screen.
            if (Input.GetMouseButtonUp(0))
            {
                // If menu is not rendered.
                if (mMenuToShow == MenuMode.MENU_OFF)
                {
                    // Show menu.
                    mMenuToShow = MenuMode.MENU_CAMERA_OPTIONS;
                }
                // If menu is already open.
                else
                {
                    // Close menu
                    mMenuToShow = MenuMode.MENU_OFF;
                }
            }
        }
        else
        {
            mButtonPressed = false;
        }
    }


    // Draw menus.
    public void OnGUI()
    {
        switch (mMenuToShow)
        {
            case MenuMode.MENU_CAMERA_OPTIONS:
                DrawMenu();
                break;

            case MenuMode.MENU_FOCUS_MODES:
                DrawFocusModes();
                break;

            default:
                break;
        }
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS

    // Draw menu to control camera device.
    private void DrawMenu()
    {
        computePosition();

        // Setup style for buttons.
        GUIStyle buttonGroupStyle = new GUIStyle(GUI.skin.button);
        buttonGroupStyle.stretchWidth = true;
        buttonGroupStyle.stretchHeight = true;

        GUILayout.BeginArea(mAreaRect);

        GUILayout.BeginHorizontal(buttonGroupStyle);

        // Turn flash on or off.
        if (GUILayout.Button("Toggle Flash", buttonGroupStyle))
        {
            if (!mFlashEnabled)
            {
                // Turn on flash if it is currently disabled.
                CameraDevice.Instance.SetFlashTorchMode(true);
                mFlashEnabled = true;
            }
            else
            {
                // Turn off flash if it is currently enabled.
                CameraDevice.Instance.SetFlashTorchMode(false);
                mFlashEnabled = false;
            }

            mMenuToShow = MenuMode.MENU_OFF;
            mButtonPressed = true;
        }

        // Triggers auto focus:
        if (GUILayout.Button("Autofocus", buttonGroupStyle))
        {
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO))
                mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO;

            mMenuToShow = MenuMode.MENU_OFF;
            mButtonPressed = true;
        }

        // Choose focus mode.
        if (GUILayout.Button("Focus Modes", buttonGroupStyle))
        {
            mMenuToShow = MenuMode.MENU_FOCUS_MODES;
            mButtonPressed = true;
        }
		
        if (GUILayout.Button("Target", buttonGroupStyle))
        {
			/*DataSetLoadBehaviour data = GetComponent<DataSetLoadBehaviour>();
			
			//print("NUM="+data.TrackerList["FlakesBox"].img.GetNumDataSets());
			
			if(data.DataSetToActivate == "Game")
			{	
				DataSetLoadBehaviour.ImageTrackedStruct imageTracked = data.TrackerList["FlakesBox"];
           		imageTracked.img.ActivateDataSet(imageTracked.data);
				data.DataSetToActivate = "FlakesBox";
			}
			else
			{	
				DataSetLoadBehaviour.ImageTrackedStruct imageTracked = data.TrackerList["Game"];
           		imageTracked.img.ActivateDataSet(imageTracked.data);
				data.DataSetToActivate = "Game";
			}*/
        }
		

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }


    // Draw menu to let user choose a focus mode.
    private void DrawFocusModes()
    {
        CameraDevice.FocusMode newMode;
        newMode = EnumOptionList(mFocusMode);

        // We set the new value only if the mode has changed.
        if (newMode != mFocusMode)
        {
            if (CameraDevice.Instance.SetFocusMode(newMode))
                mFocusMode = newMode;

            mMenuToShow = MenuMode.MENU_OFF;
            mButtonPressed = true;
        }
    }


    // Helper function to automatically create an option list of an enum object.
    private static CameraDevice.FocusMode EnumOptionList(
                                    CameraDevice.FocusMode setMode)
    {
        Type modeType = setMode.GetType();

        // Get possible enum values.
        CameraDevice.FocusMode[] modes =
            (CameraDevice.FocusMode[])Enum.GetValues(modeType);

        // Setup style for list.
        GUIStyle optionListStyle = new GUIStyle(GUI.skin.button);
        optionListStyle.stretchHeight = true;
        optionListStyle.stretchWidth = true;

        // Setup style for toggles.
        // We use "button" style as template because default toggles are too
        // small.
        GUIStyle toggleStyle = new GUIStyle(GUI.skin.button);
        toggleStyle.stretchHeight = true;
        toggleStyle.stretchWidth = true;
        toggleStyle.normal.textColor = Color.gray;
        toggleStyle.onNormal.textColor = Color.gray;
        toggleStyle.focused.textColor = Color.gray;
        toggleStyle.onFocused.textColor = Color.gray;
        toggleStyle.active.textColor = Color.gray;
        toggleStyle.onActive.textColor = Color.gray;
        toggleStyle.hover.textColor = Color.gray;
        toggleStyle.onHover.textColor = Color.gray;

        // Setup style for active toggle.
        // Setting active values for the toggle Style does not work so we create
        // another style.
        GUIStyle activeToggleStyle = new GUIStyle(toggleStyle);
        activeToggleStyle.normal.textColor = Color.white;
        activeToggleStyle.onNormal.textColor = Color.white;
        activeToggleStyle.focused.textColor = Color.white;
        activeToggleStyle.onFocused.textColor = Color.white;
        activeToggleStyle.active.textColor = Color.white;
        activeToggleStyle.onActive.textColor = Color.white;
        activeToggleStyle.hover.textColor = Color.white;
        activeToggleStyle.onHover.textColor = Color.white;


        CameraDevice.FocusMode newMode = setMode;

        // We render the menu over the full screen.
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        GUILayout.BeginVertical();

        foreach (CameraDevice.FocusMode mode in modes)
        {
            if (mode == setMode)
            {
                GUILayout.Toggle(true, mode.ToString(), activeToggleStyle);
            }
            else
            {
                if (GUILayout.Toggle(false, mode.ToString(), toggleStyle))
                {
                    newMode = mode;
                }
            }
        }

        GUILayout.EndVertical();

        GUILayout.EndArea();

        return newMode;
    }


    /// Compute the coordinates of the menu depending on the current orientation.
    private void computePosition()
    {
        int areaWidth = Screen.width;
        int areaHeight = (Screen.height / 5) * 2;
        int areaLeft = 0;
        int areaTop = Screen.height - areaHeight;
        mAreaRect = new Rect(areaLeft, areaTop, areaWidth, areaHeight);
    }

    #endregion // PRIVATE_METHODS
}
