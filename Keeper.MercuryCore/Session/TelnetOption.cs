﻿namespace Keeper.MercuryCore.Session
{
    public enum TelnetOption
        : byte
    {
        BinaryTransmission = 0,
        Echo = 1,
        Reconnection = 2,
        SuppressGoAhead = 3,
        ApproxMessageSizeNegotiation = 4,
        Status = 5,
        TimingMark = 6,
        RemoteControlledTransAndEcho = 7,
        OutputLineWidth = 8,
        OutputPageSize = 9,
        OutputCarriageReturnDisposition = 10,
        OutputHorizontalTabStops = 11,
        OutputHorizontalTabDisposition = 12,
        OutputFormfeedDisposition = 13,
        OutputVerticalTabStops = 14,
        OutputVerticalTabDisposition = 15,
        OutputLinefeedDisposition = 16,
        ExtendedAscii = 17,
        Logout = 18,
        ByteMacro = 19,
        DataEntryTerminal = 20,
        Supdup = 21,
        SupdupOutput = 22,
        SendLocation = 23,
        TerminalType = 24,
        EndOfRecord = 25,
        TacacsUserIdentification = 26,
        OutputMarking = 27,
        TerminalLocationNumber = 28,
        Telnet3270Regime = 29,
        X3Pad = 30,
        NegotiateAboutWindowSize = 31,
        TerminalSpeed = 32,
        RemoteFlowControl = 33,
        Linemode = 34,
        XDisplayLocation = 35,
        EnvironmentOption = 36,
        AuthenticationOption = 37,
        EncryptionOption = 38,
        NewEnvironmentOption = 39,
        Tn3270E = 40,
        Xauth = 41,
        Charset = 42,
        TelnetRemoteSerialPort = 43,
        ComPortControlOption = 44,
        TelnetSuppressLocalEcho = 45,
        TelnetStartTls = 46,
        Kermit = 47,
        SendUrl = 48,
        ForwardX = 49,
        Msdp = 69,
        Mssp = 70,
        Mcp = 85,
        Mccp = 86,
        Msp = 90,
        Mxp = 91,
        TeloptPragmaLogon = 138,
        TeloptSspiLogon = 139,
        TeloptPragmaHeartbeat = 140,
        Atcp = 200,
        Gmcp = 201,
        ExtendedOptionsList = 255
    }
}
