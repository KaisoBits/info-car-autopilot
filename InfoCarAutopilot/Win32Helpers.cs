using System.Runtime.InteropServices;

namespace InfoCarAutopilot;

public static class Win32Helpers
{
    public static void ToggleOSSleepPrevention(bool preventFromSleeping)
    {
        if (preventFromSleeping)
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
        else
            SetThreadExecutionState(ES_CONTINUOUS);
    }

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;

    [DllImport("kernel32.dll")]
    private static extern uint SetThreadExecutionState(uint esFlags);
}
