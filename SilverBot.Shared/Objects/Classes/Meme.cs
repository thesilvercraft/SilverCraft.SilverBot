/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using System.Text.Json.Serialization;

namespace SilverBotDS.Objects;

public class Meme
{
    /// <summary>
    ///     the link of the post
    /// </summary>
    [JsonPropertyName("postLink")]
    public string PostLink { get; set; }

    /// <summary>
    ///     the subreddit of the post
    /// </summary>
    [JsonPropertyName("subreddit")]
    public string Subreddit { get; set; }

    /// <summary>
    ///     The title of the post
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    ///     url address of the post
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    ///     bool if the post is nsfw or not
    /// </summary>
    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }

    /// <summary>
    ///     bool if the post is a spoiler or not
    /// </summary>
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }

    /// <summary>
    ///     author of post
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; set; }

    /// <summary>
    ///     number of upvotes of post
    /// </summary>
    [JsonPropertyName("ups")]
    public int Ups { get; set; }
}