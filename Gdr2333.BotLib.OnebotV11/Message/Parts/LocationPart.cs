/*
   Copyright 2025 All contributors of Gdr2333.BotLib

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 位置分享消息段
/// </summary>
public class LocationPart : MessagePartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private LocationPayload? _data;

    /// <summary>
    /// 经度
    /// </summary>
    [JsonIgnore]
    public double Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    [JsonIgnore]
    public double Latitude { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [JsonIgnore]
    public string? Title { get; set; }

    /// <summary>
    /// 介绍
    /// </summary>
    [JsonIgnore]
    public string? Content { get; set; }

    [JsonConstructor]
    private LocationPart() : base("location")
    {
    }

    /// <summary>
    /// 构建一个位置分享消息段
    /// </summary>
    /// <param name="longitude">经度</param>
    /// <param name="latitude">纬度</param>
    /// <param name="title">标题</param>
    /// <param name="content">介绍</param>
    public LocationPart(double longitude, double latitude, string? title, string? content) : base("location")
    {
        Longitude = longitude;
        Latitude = latitude;
        Title = title;
        Content = content;
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        Longitude = _data.Longitude;
        Latitude = _data.Latitude;
        Title = _data.Title;
        Content = _data.Content;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new()
        {
            Longitude = Longitude,
            Latitude = Latitude,
            Title = Title,
            Content = Content
        };
    }

    /// <inheritdoc/>
    public override string ToString() =>
        $"[位置：{Title ?? Content ?? ($"经度：{Longitude}，纬度：{Latitude}")}]";
}
