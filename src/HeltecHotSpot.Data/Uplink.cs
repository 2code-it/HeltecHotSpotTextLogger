namespace HeltecHotSpot.Data
{
	public class Uplink : Link
	{
		public byte chan { get; set; }
		public byte stat { get; set; }
		public double lsnr { get; set; }
		public short rssi { get; set; }
	}
}
