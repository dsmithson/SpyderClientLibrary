﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Notifications
{
    public enum ServerEventType
    {
        DSCPing,
        Warning,				
        Error,					
        Failure,				
        Success,
        Information,
        InputConfigLoaded,
        OutputConfigLoaded,
        PixelSpacesChanged,
        AutoSyncComplete,
        EffectComplete,
        DataObjectChanged,
        PlayItemsChanged,
        StillLoaded,
        InvalidDataInScript,
        DrawingDataChanged,
        SourceSelected,
        PresetRecallComplete,
        PresetOnComplete,
        ScriptRun,
        ScriptModified,
        FileChange,
        FrontPanelChange,
        UserDiagnosticsRefreshed,
        FrameConfigsChanged,
        RoutersChanged,
        MultiviewerConfigsChanged,
        UserEdidsChanged,
        InputEdidsChanged,
        IP4AddressChanged,
        FrontPanelCommandKeysChanged,
        FrontPanelFunctionKeysChanged,
        FrontPanelTestPatternsChanged,
        FrontPanelLockUpdated
    };
    
}
