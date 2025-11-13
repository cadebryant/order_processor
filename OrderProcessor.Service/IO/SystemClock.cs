namespace OrderProcessor.Service.IO
{
    public class SystemClock : IClock
    {
        public DateTime Today() => DateTime.Today;
    }
}
