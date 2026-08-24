using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace AquaMai.Mods.GameSystem.ExclusiveTouch;

public sealed class UsbDeviceIdentifierFinder : UsbDeviceFinder
{
    private readonly string identifier;
    private readonly UsbDeviceFinder vidPidFinder;
    private readonly UsbDeviceLocationFinder locationFinder;

    public UsbDeviceIdentifierFinder(int vid, int pid, string identifier)
        : base(vid, pid)
    {
        this.identifier = identifier?.Trim();
        vidPidFinder = new UsbDeviceFinder(vid, pid);
        locationFinder = new UsbDeviceLocationFinder(vid, pid, identifier);
    }

    public override bool Check(UsbRegistry usbRegistry)
    {
        return vidPidFinder.Check(usbRegistry) &&
               (MatchesSerial(GetRegistrySerial(usbRegistry)) || locationFinder.Check(usbRegistry));
    }

    public bool Check(UsbRegistry usbRegistry, string deviceSerial)
    {
        return Check(usbRegistry) ||
               (vidPidFinder.Check(usbRegistry) && MatchesSerial(deviceSerial));
    }

    public UsbDevice OpenUsbDevice(string diagnosticName, int playerNo)
    {
        var diagnosticsEnabled = ExclusiveTouchDiagnostics.Enabled;
        UsbRegDeviceList registries;
        try
        {
            registries = UsbDevice.AllDevices;
        }
        catch (Exception e)
        {
            if (diagnosticsEnabled)
            {
                ExclusiveTouchDiagnostics.Log(
                    "{0} player={1} device-scan error={2}", diagnosticName, playerNo + 1, e.Message);
            }
            return null;
        }

        var supportedCount = 0;
        if (diagnosticsEnabled)
        {
            ExclusiveTouchDiagnostics.Log(
                "{0} player={1} device-scan identifier={2} registry-count={3}",
                diagnosticName, playerNo + 1, identifier, registries.Count);
        }

        for (int i = 0; i < registries.Count; i++)
        {
            var registry = registries[i];
            if (diagnosticsEnabled)
            {
                var hardwareIds = registry[SPDRP.HardwareId] as string[];
                ExclusiveTouchDiagnostics.Log(
                    "{0} player={1} device-registry index={2} vid=0x{3:X4} pid=0x{4:X4} hardware-id={5} symbolic={6} device-id={7}",
                    diagnosticName, playerNo + 1, i, registry.Vid, registry.Pid,
                    hardwareIds == null ? "" : string.Join("|", hardwareIds),
                    registry.SymbolicName, registry["DeviceID"]);
            }
            if (!vidPidFinder.Check(registry)) continue;

            supportedCount++;
            var registrySerial = GetRegistrySerial(registry);
            var registrySerialMatch = MatchesSerial(registrySerial);
            var locationMatch = locationFinder.Check(registry);
            if (diagnosticsEnabled)
            {
                var locations = registry[SPDRP.LocationPaths] as string[];
                ExclusiveTouchDiagnostics.Log(
                    "{0} player={1} device-candidate index={2} registry-serial={3} registry-match={4} path-match={5} symbolic={6} device-id={7} locations={8}",
                    diagnosticName, playerNo + 1, i, registrySerial, registrySerialMatch,
                    locationMatch, registry.SymbolicName, registry["DeviceID"],
                    locations == null ? "" : string.Join("|", locations));
            }

            UsbDevice candidate = null;
            var selected = false;
            try
            {
                if (!registry.Open(out candidate) || candidate == null)
                {
                    if (diagnosticsEnabled)
                    {
                        ExclusiveTouchDiagnostics.Log(
                            "{0} player={1} device-candidate index={2} open=false error={3}",
                            diagnosticName, playerNo + 1, i, UsbDevice.LastErrorString);
                    }
                    continue;
                }

                string descriptorSerial = null;
                string descriptorError = null;
                try
                {
                    descriptorSerial = candidate.Info.SerialString?.Trim();
                }
                catch (Exception e)
                {
                    descriptorError = e.Message;
                }

                var descriptorSerialMatch = MatchesSerial(descriptorSerial);
                var matchSource = locationMatch ? "path" :
                    registrySerialMatch ? "registry-serial" :
                    descriptorSerialMatch ? "descriptor-serial" : "none";
                if (diagnosticsEnabled)
                {
                    ExclusiveTouchDiagnostics.Log(
                        "{0} player={1} device-candidate index={2} open=true descriptor-serial={3} descriptor-error={4} match={5}",
                        diagnosticName, playerNo + 1, i, descriptorSerial, descriptorError, matchSource);
                }

                if (!Check(registry, descriptorSerial)) continue;

                selected = true;
                return candidate;
            }
            catch (Exception e)
            {
                if (diagnosticsEnabled)
                {
                    ExclusiveTouchDiagnostics.Log(
                        "{0} player={1} device-candidate index={2} error={3}",
                        diagnosticName, playerNo + 1, i, e.Message);
                }
            }
            finally
            {
                if (!selected && candidate != null)
                {
                    try
                    {
                        candidate.Close();
                    }
                    catch (Exception e)
                    {
                        if (diagnosticsEnabled)
                        {
                            ExclusiveTouchDiagnostics.Log(
                                "{0} player={1} device-candidate index={2} close-error={3}",
                                diagnosticName, playerNo + 1, i, e.Message);
                        }
                    }
                }
            }
        }

        if (diagnosticsEnabled)
        {
            ExclusiveTouchDiagnostics.Log(
                "{0} player={1} device-scan no-match supported-count={2}",
                diagnosticName, playerNo + 1, supportedCount);
        }
        return null;
    }

    private bool MatchesSerial(string serial)
    {
        return !string.IsNullOrWhiteSpace(serial) &&
               string.Equals(identifier, serial.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static string GetRegistrySerial(UsbRegistry usbRegistry)
    {
        if (string.IsNullOrWhiteSpace(usbRegistry.SymbolicName)) return "";
        return UsbSymbolicName.Parse(usbRegistry.SymbolicName).SerialNumber?.Trim() ?? "";
    }
}
