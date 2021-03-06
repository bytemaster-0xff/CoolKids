﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.PlatformSupport
{
    public enum Locations
    {
        Local,
        Roaming,
        Temp
    }

    public interface IStorageService
    {
        Task<Uri> StoreAsync(Stream stream, Locations location, String fileName, String folder = "");

        Task<Stream> Get(Uri rui);

        Task<Stream> Get(Locations location, String fileName, String folder = "");

        Task<T> GetKVPAsync<T>(String key, T defaultValue = default(T)) where T : class;

        Task StoreKVP<T>(String key, T value) where T : class;

        Task ClearKVP(String key);
    }
}
