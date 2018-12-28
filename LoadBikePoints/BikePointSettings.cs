namespace LoadBikePoints
{
    public interface ITaskSettings
    {
        string Url { get; }
    }

    public class BikePointTaskSettings : ITaskSettings
    {
        public string Url { get => "#### path of the resource #######"; }

    }
}
