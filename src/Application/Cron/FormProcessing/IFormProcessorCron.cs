﻿using System;
using System.Threading.Tasks;
using Application.Helpers;

namespace Application.Services.Cron
{
    public interface IFormProcessorCron: IAutoDependencyService
    {
       Task DoWork();
    }
}

