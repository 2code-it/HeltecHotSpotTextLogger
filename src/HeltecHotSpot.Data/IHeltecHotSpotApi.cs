
namespace HeltecHotSpot.Data
{
	public interface IHeltecHotSpotApi
	{
		Downlink[] GetLatestDownlinks(int count);
		Uplink[] GetLatestUplinks(int count);
		HotSpotStatus GetStatus();
		void ResetPassword(string newPassword, string newPasswordRepeat);
		MinerLogEntry[] GetMinerLogEntries();
	}
}