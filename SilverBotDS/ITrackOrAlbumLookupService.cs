using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Sdk;
using SilverBot.Shared.Objects;

namespace SilverBotDS
{
    public interface ITrackOrAlbumLookupService
    {
        public Task<List<string>?> TryGettingTrackOrAlbum(string name);
    }

    public class JellyFinLookupService : ITrackOrAlbumLookupService
    {
        private readonly IItemsClient _itemsClient;
        private readonly Config _config;
        public JellyFinLookupService(HttpClient client, Config config)
        {
            _config = config;
            // Create the settings object
            var sdkClientSettings = new SdkClientSettings
            {
                BaseUrl = _config.ExtraParams["JellyFinLookupService.BaseUrlNoSlash"],
                AccessToken = _config.ExtraParams["JellyFinLookupService.ApiKey"],
                DeviceId = "silverbotlol"
            };
            _itemsClient = new ItemsClient(sdkClientSettings, client);
        }

        public async Task<List<string>?> TryGettingTrackOrAlbum(string name)
        {
            BaseItemDtoQueryResult? itemsResponse;
            if (Guid.TryParse(name, out var x))
            {
                 itemsResponse = await _itemsClient.GetItemsAsync(userId: Guid.Parse(_config.ExtraParams["JellyFinLookupService.ParentUserId"]),
                    recursive: true, parentId: Guid.Parse( _config.ExtraParams["JellyFinLookupService.ParentLibId"]), ids: new []{x},
                    includeItemTypes: new List<BaseItemKind>
                    {
                        BaseItemKind.Audio, BaseItemKind.MusicAlbum
                    }, sortBy: new []{"ParentIndexNumber","IndexNumber","SortName"}).ConfigureAwait(false);
            }
            else
            {
                itemsResponse= await _itemsClient.GetItemsAsync(userId: Guid.Parse(_config.ExtraParams["JellyFinLookupService.ParentUserId"]),
                    searchTerm: name, recursive: true, parentId: Guid.Parse( _config.ExtraParams["JellyFinLookupService.ParentLibId"]),
                    includeItemTypes: new List<BaseItemKind>
                    {
                        BaseItemKind.Audio, BaseItemKind.MusicAlbum
                    }).ConfigureAwait(false);
            }
 
            switch (itemsResponse.Items[0].Type)
            {
                case BaseItemKind.MusicAlbum:
                    itemsResponse = await _itemsClient.GetItemsAsync(userId: Guid.Parse(_config.ExtraParams["JellyFinLookupService.ParentUserId"]),
                        parentId: itemsResponse.Items[0].Id, includeItemTypes: new List<BaseItemKind>
                        {
                            BaseItemKind.Audio
                        }, sortBy: new []{"ParentIndexNumber","IndexNumber","SortName"}).ConfigureAwait(false);
                    return itemsResponse.Items.Select(s => _config.ExtraParams["JellyFinLookupService.BaseUrlNoSlash"]+ "/Items/" + s.Id + "/Download?api_key=" + _config.ExtraParams["JellyFinLookupService.ApiKey"]+"&ignoreplsindex="+s.IndexNumber).ToList();
                case BaseItemKind.Audio:
                    return new List<string>()
                    {
                        _config.ExtraParams["JellyFinLookupService.BaseUrlNoSlash"]+"/Items/" + itemsResponse.Items[0].Id + "/Download?api_key=" + _config.ExtraParams["JellyFinLookupService.ApiKey"]
                    };
                case BaseItemKind.AggregateFolder:
                case BaseItemKind.AudioBook:
                case BaseItemKind.BasePluginFolder:
                case BaseItemKind.Book:
                case BaseItemKind.BoxSet:
                case BaseItemKind.Channel:
                case BaseItemKind.ChannelFolderItem:
                case BaseItemKind.CollectionFolder:
                case BaseItemKind.Episode:
                case BaseItemKind.Folder:
                case BaseItemKind.Genre:
                case BaseItemKind.ManualPlaylistsFolder:
                case BaseItemKind.Movie:
                case BaseItemKind.LiveTvChannel:
                case BaseItemKind.LiveTvProgram:
                case BaseItemKind.MusicArtist:
                case BaseItemKind.MusicGenre:
                case BaseItemKind.MusicVideo:
                case BaseItemKind.Person:
                case BaseItemKind.Photo:
                case BaseItemKind.PhotoAlbum:
                case BaseItemKind.Playlist:
                case BaseItemKind.PlaylistsFolder:
                case BaseItemKind.Program:
                case BaseItemKind.Recording:
                case BaseItemKind.Season:
                case BaseItemKind.Series:
                case BaseItemKind.Studio:
                case BaseItemKind.Trailer:
                case BaseItemKind.TvChannel:
                case BaseItemKind.TvProgram:
                case BaseItemKind.UserRootFolder:
                case BaseItemKind.UserView:
                case BaseItemKind.Video:
                case BaseItemKind.Year:
                default:
                    return null;
            }
        }
    }
}