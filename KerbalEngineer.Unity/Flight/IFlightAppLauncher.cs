namespace KerbalEngineer.Unity.Flight
{
    public interface IFlightAppLauncher
    {
        bool isOn { get; }

        bool controlBar { get; set; }

        bool showEngineer { get; set; }
    }
}