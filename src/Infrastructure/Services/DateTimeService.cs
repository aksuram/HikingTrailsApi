﻿using HikingTrailsApi.Application.Common.Interfaces;
using System;

namespace HikingTrailsApi.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
