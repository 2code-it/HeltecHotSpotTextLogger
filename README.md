## HeltecHotSpotTextLogger
Gets the data from a heltec hotspot and saves it to text format


## PreReqs
dotnet5 desktop runtime

## Config
HeliumHotSpotAddress= (not used)
LocalIPAddress=192.168.1.2 (hotspot local network address)
LoginName=HT-M2808 (default dashboard login name)
LoginPassword=a0b0c0 (dashboard login password)
LoraLogDownlinkAmount=4 (amount of lora downlinks to fetch for the specified period)
LoraLogUplinkAmount=20 (amount of lora uplinks to fetch for the specified period)
LoraLogIntervalInMinutes=10 (lora log interval)
StatusLogIntervalInMinutes=10 (hotspot status log interval)
MinerLogIntervalInMinutes=10 (miner log interval from beacons.php)
