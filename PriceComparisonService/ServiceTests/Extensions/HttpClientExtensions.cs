﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceComarisonServiceTests.Extensions
{
    internal class HttpResponseMessageWithTiming
    {
        internal HttpResponseMessage Response { get; set; }

        internal TimeSpan Timing { get; set; }
    }

    internal static class HttpClientExtensions
    {
        internal static async Task<HttpResponseMessageWithTiming> GetWithTimingAsync(this HttpClient client, string requestUri)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            HttpResponseMessage response = await client.GetAsync(requestUri);
            TimeSpan timing = stopwatch.Elapsed;

            stopwatch.Stop();

            return new HttpResponseMessageWithTiming
            {
                Response = response,
                Timing = timing
            };
        }

        //internal static async Task<HttpResponseMessageWithTiming> PostWithTimingAsync(this HttpClient client, string requestUri)
        //{
        //    Stopwatch stopwatch = Stopwatch.StartNew();

        //    HttpResponseMessage response = await client.PostAsync(requestUri);
        //    TimeSpan timing = stopwatch.Elapsed;

        //    stopwatch.Stop();

        //    return new HttpResponseMessageWithTiming
        //    {
        //        Response = response,
        //        Timing = timing
        //    };
        //}
    }
}
