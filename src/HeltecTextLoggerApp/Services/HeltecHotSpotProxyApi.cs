using HeltecHotSpot.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Services
{
	public class HeltecHotSpotProxyApi : IHeltecHotSpotApi
	{
		public HeltecHotSpotProxyApi(string host, string user, string password)
		{
			_host = host;
			_credential = new NetworkCredential(user, password);
		}

		private string _host;
		private NetworkCredential _credential;

		public HotSpotStatus GetStatus()
		{
			JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
			jsonOptions.Converters.Add(new StringToByteJsonConverter());
			jsonOptions.Converters.Add(new StringToInt32JsonConverter());
			jsonOptions.Converters.Add(new StringToDoubleJsonConverter());

			string json = DownloadString("apply.php");
			return JsonSerializer.Deserialize<HotSpotStatus>(json, jsonOptions);
		}

		public Uplink[] GetLatestUplinks(int count)
		{
			LinkRequest linkRequest = new LinkRequest();
			linkRequest.apply = "loralog";
			linkRequest.cnt = count.ToString();
			linkRequest.tr = "up";
			string data = UploadString("apply.php", JsonSerializer.Serialize(linkRequest));
			return data.Trim().Split("<br>", StringSplitOptions.RemoveEmptyEntries).Select(x => GetUplinkFromTextLine(x)).ToArray();
		}

		public Downlink[] GetLatestDownlinks(int count)
		{
			LinkRequest linkRequest = new LinkRequest();
			linkRequest.apply = "loralog";
			linkRequest.cnt = count.ToString();
			linkRequest.tr = "down";
			string data = UploadString("apply.php", JsonSerializer.Serialize(linkRequest));
			return data.Trim().Split("<br>", StringSplitOptions.RemoveEmptyEntries).Select(x => GetDownlinkFromTextLine(x)).ToArray();
		}

		public void ResetPassword(string newPassword, string newPasswordRepeat)
		{
			throw new NotImplementedException();
		}

		public MinerLogEntry[] GetMinerLogEntries()
		{
			//2021-12-18 00:14:48.630 6 [error] <0.7356.7>@miner_onion_server:send_witness:207 failed to send witness, max retry
			string data = DownloadString("beacon.php");
			Regex regexChunkStart = new Regex(@"^\d{4}\-\d{2}\-\d{2}");
			data = data.Replace("<br>", "$$");
			data = Regex.Replace(data, "<[^>]*>", string.Empty);
			string[] chunks = data.Split("\n", StringSplitOptions.RemoveEmptyEntries).Where(x => regexChunkStart.IsMatch(x)).ToArray();
			data = string.Join(string.Empty, chunks);
			var entries =  data.Split("$$", StringSplitOptions.RemoveEmptyEntries).Select(x => GetMinerLogEntryFromTextLine(x)).ToArray();
			return entries;
		}

		private Uplink GetUplinkFromTextLine(string line)
		{
			string jsonString = line.Substring(line.IndexOf("{"));
			if (!jsonString.EndsWith("]}"))
			{
				jsonString = jsonString.Substring(0, jsonString.IndexOf("]")) + "]}";
			}
			Uplink link = JsonSerializer.Deserialize<UplinkResponse>(jsonString).rxpk[0];
			link.created = GetLinkDateTimeFromLinkLine(line);
			return link;
		}

		private Downlink GetDownlinkFromTextLine(string line)
		{
			string jsonString = line.Substring(line.IndexOf("{"));
			Downlink link = JsonSerializer.Deserialize<DownlinkResponse>(jsonString).txpk;
			link.created = GetLinkDateTimeFromLinkLine(line);
			return link;
		}

		private DateTime GetLinkDateTimeFromLinkLine(string line)
		{
			//Dec 9 12:24:24
			string dateTimeString = line.Substring(0, line.IndexOf(" JSON "));
			string[] monthNames = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec".Split(',');
			string[] parts = dateTimeString.Split(' ');
			int month = Array.IndexOf<string>(monthNames, parts[0]) + 1;
			int day = Convert.ToInt32(parts[1]);
			TimeSpan time = TimeSpan.ParseExact(parts[2], "h':'mm':'ss", CultureInfo.InvariantCulture);
			DateTime dateTime = new DateTime(DateTime.Now.Year, month, day, time.Hours, time.Minutes, time.Seconds);
			if (dateTime.Subtract(DateTime.Now).Days > 180)
			{
				dateTime = dateTime.AddYears(-1);
			}
			return dateTime.ToLocalTime();
		}

		private MinerLogEntry GetMinerLogEntryFromTextLine(string line)
		{
			string[] parts = line.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
			MinerLogEntry logEntry = new MinerLogEntry();
			logEntry.created = DateTime.ParseExact($"{parts[0]} {parts[1]}:{parts[2]}:{parts[3]}", "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture).ToLocalTime();
			logEntry.category = Convert.ToByte(parts[4]);
			logEntry.severity = parts[5].Trim('[', ']');
			logEntry.origin = parts[6];
			logEntry.source = parts[7];
			logEntry.code = Convert.ToInt32(parts[8]);
			logEntry.message = string.Join(' ', parts.Skip(9));
			return logEntry;
		}

		private string UploadString(string path, string data)
		{
			using (WebClient client = new WebClient())
			{
				string address = $"http://{_host}/{path}";
				client.Credentials = _credential;
				return client.UploadString(address, data ?? string.Empty);
			}
		}

		private string DownloadString(string path)
		{
			using (WebClient client = new WebClient())
			{
				string address = $"http://{_host}/{path}";
				client.Credentials = _credential;
				return client.DownloadString(address);
			}
		}

		private class StringToByteJsonConverter : JsonConverter<Byte>
		{
			public override Byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.String)
				{
					return Convert.ToByte(reader.GetString());
				}
				else
				{
					return reader.GetByte();
				}
			}

			public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
			{
				throw new NotImplementedException();
			}
		}

		private class StringToInt32JsonConverter : JsonConverter<int>
		{
			public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.String)
				{
					return Convert.ToInt32(reader.GetString());
				}
				else
				{
					return reader.GetInt32();
				}
			}

			public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
			{
				throw new NotImplementedException();
			}
		}

		private class StringToDoubleJsonConverter : JsonConverter<double>
		{
			public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.String)
				{
					return Convert.ToDouble(reader.GetString());
				}
				else
				{
					return reader.GetDouble();
				}
			}

			public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
			{
				throw new NotImplementedException();
			}
		}
	}
}
