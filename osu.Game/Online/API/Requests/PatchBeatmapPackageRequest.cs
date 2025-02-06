// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Net.Http;
using osu.Framework.IO.Network;

namespace osu.Game.Online.API.Requests
{
    public class PatchBeatmapPackageRequest : APIUploadRequest
    {
        protected override string Uri
        {
            get
            {
                // can be removed once the service has been successfully deployed to production
                if (API!.EndpointConfiguration.BeatmapSubmissionServiceUrl == null)
                    throw new NotSupportedException("Beatmap submission not supported in this configuration!");

                return $@"{API!.EndpointConfiguration.BeatmapSubmissionServiceUrl!}/beatmapsets/{BeatmapSetID}";
            }
        }

        protected override string Target => throw new NotSupportedException();

        public uint BeatmapSetID { get; }

        // ReSharper disable once CollectionNeverUpdated.Global
        public Dictionary<string, byte[]> FilesChanged { get; } = new Dictionary<string, byte[]>();

        // ReSharper disable once CollectionNeverUpdated.Global
        public HashSet<string> FilesDeleted { get; } = new HashSet<string>();

        public PatchBeatmapPackageRequest(uint beatmapSetId)
        {
            BeatmapSetID = beatmapSetId;
        }

        protected override WebRequest CreateWebRequest()
        {
            var request = base.CreateWebRequest();
            request.Method = HttpMethod.Patch;

            foreach ((string filename, byte[] content) in FilesChanged)
                request.AddFile(@"filesChanged", content, filename);

            foreach (string filename in FilesDeleted)
                request.AddParameter(@"filesDeleted", filename, RequestParameterType.Form);

            request.Timeout = 60_000;
            return request;
        }
    }
}
