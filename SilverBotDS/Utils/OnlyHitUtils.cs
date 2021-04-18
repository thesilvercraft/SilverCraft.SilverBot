using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Utils
{
    internal class OnlyHitUtils
    {
        public static async Task<CurrentSong> SearchAsync(HttpClient httpClient, string url = "https://api.onlyhit.us/fingerprinting/onlyhit.json")
        {
            var rm = await httpClient.GetAsync(url);
            var cursong = JsonSerializer.Deserialize<CurrentSong>(await rm.Content.ReadAsStringAsync());
            if (cursong is not null && cursong.Status.Msg == "Success")
            {
                return cursong;
            }
            throw new($"Requested yielded a message that isn't Success it is {cursong.Status.Msg}");
        }

        public class CurrentSong
        {
            [JsonPropertyName("status")]
            public Status Status { get; set; }

            [JsonPropertyName("result_type")]
            public int ResultType { get; set; }

            [JsonPropertyName("metadata")]
            public Metadata Metadata { get; set; }
        }

        public class Status
        {
            [JsonPropertyName("msg")]
            public string Msg { get; set; }

            [JsonPropertyName("code")]
            public int Code { get; set; }

            [JsonPropertyName("version")]
            public string Version { get; set; }
        }

        public class Metadata
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("timestamp_utc")]
            public string TimestampUtc { get; set; }

            [JsonPropertyName("music")]
            public Music[] Music { get; set; }
        }

        public class Music
        {
            [JsonPropertyName("album")]
            public Album Album { get; set; }

            [JsonPropertyName("play_offset_ms")]
            public int PlayOffsetMs { get; set; }

            [JsonPropertyName("sample_begin_time_offset_ms")]
            public int SampleBeginTimeOffsetMs { get; set; }

            [JsonPropertyName("contributors")]
            public Contributors Contributors { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("result_from")]
            public int ResultFrom { get; set; }

            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; }

            [JsonPropertyName("sample_end_time_offset_ms")]
            public int SampleEndTimeOffsetMs { get; set; }

            [JsonPropertyName("genres")]
            public Genre[] Genres { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("db_end_time_offset_ms")]
            public int DbEndTimeOffsetMs { get; set; }

            [JsonPropertyName("score")]
            public int Score { get; set; }

            [JsonPropertyName("db_begin_time_offset_ms")]
            public int DbBeginTimeOffsetMs { get; set; }

            [JsonPropertyName("artists")]
            public Artist2[] Artists { get; set; }

            [JsonPropertyName("duration_ms")]
            public int DurationMs { get; set; }

            [JsonPropertyName("external_ids")]
            public ExternalIds ExternalIds { get; set; }

            [JsonPropertyName("acrid")]
            public string Acrid { get; set; }

            [JsonPropertyName("external_metadata")]
            public ExternalMetadata ExternalMetadata { get; set; }
        }

        public class Album
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class Contributors
        {
            [JsonPropertyName("composers")]
            public string[] Composers { get; set; }

            [JsonPropertyName("lyricists")]
            public string[] Lyricists { get; set; }
        }

        public class ExternalIds
        {
            [JsonPropertyName("isrc")]
            public string Isrc { get; set; }

            [JsonPropertyName("upc")]
            public string Upc { get; set; }
        }

        public class ExternalMetadata
        {
            [JsonPropertyName("spotify")]
            public Spotify Spotify { get; set; }

            [JsonPropertyName("deezer")]
            public Deezer Deezer { get; set; }

            [JsonPropertyName("youtube")]
            public Youtube Youtube { get; set; }
        }

        public class Youtube
        {
            [JsonPropertyName("vid")]
            public string Vid { get; set; }
        }

        public class Spotify
        {
            [JsonPropertyName("album")]
            public Album1 Album { get; set; }

            [JsonPropertyName("track")]
            public Track Track { get; set; }

            [JsonPropertyName("artists")]
            public Artist[] Artists { get; set; }
        }

        public class Album1
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class Track
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }
        }

        public class Artist
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class Deezer
        {
            [JsonPropertyName("album")]
            public Album2 Album { get; set; }

            [JsonPropertyName("track")]
            public Track1 Track { get; set; }

            [JsonPropertyName("artists")]
            public Artist1[] Artists { get; set; }
        }

        public class Album2
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class Track1
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }
        }

        public class Artist1
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class Genre
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class Artist2
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}