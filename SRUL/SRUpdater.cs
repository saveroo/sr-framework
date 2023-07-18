// // using AutoUpdaterDotNET;
// using Newtonsoft.Json;
//
// namespace SRUL
// {
//     public class SRUpdater
//     {
//         private void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
//         {
//             dynamic json = JsonConvert.DeserializeObject(args.RemoteData);
//             args.UpdateInfo = new UpdateInfoEventArgs
//             {
//                 CurrentVersion = json.version,
//                 ChangelogURL = json.changelog,
//                 DownloadURL = json.url,
//                 Mandatory = new Mandatory
//                 {
//                     Value = json.mandatory.value,
//                     UpdateMode = json.mandatory.mode,
//                     MinimumVersion = json.mandatory.minVersion
//                 },
//                 CheckSum = new CheckSum
//                 {
//                     Value = json.checksum.value,
//                     HashingAlgorithm = json.checksum.hashingAlgorithm
//                 }
//             };
//         }
//     }
// }