global using AutoMapper;
global using AutoMapper.QueryableExtensions;

global using MediatR;

global using Microsoft.AspNetCore.SignalR;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using Reinforced.Typings.Attributes;

global using StreamMaster.Application.Common;
global using StreamMaster.Application.Common.Events;
global using StreamMaster.Application.Common.Extensions;
global using StreamMaster.Application.Hubs;
global using StreamMaster.Application.Interfaces;
global using StreamMaster.Application.Profiles.Queries;
global using StreamMaster.Domain.API;
global using StreamMaster.Domain.Attributes;
global using StreamMaster.Domain.Common;
global using StreamMaster.Domain.Configuration;
global using StreamMaster.Domain.Dto;
global using StreamMaster.Domain.Enums;
global using StreamMaster.Domain.Extensions;
global using StreamMaster.Domain.Helpers;
global using StreamMaster.Domain.Logging;
global using StreamMaster.Domain.Models;
global using StreamMaster.Domain.Pagination;
global using StreamMaster.Domain.Repository;
global using StreamMaster.Domain.Services;
global using StreamMaster.PlayList;
global using StreamMaster.PlayList.Models;
global using StreamMaster.SchedulesDirect.Domain.Dto;
global using StreamMaster.SchedulesDirect.Domain.Interfaces;
global using StreamMaster.SchedulesDirect.Domain.JsonClasses;
global using StreamMaster.SchedulesDirect.Domain.Models;
global using StreamMaster.SchedulesDirect.Domain.XmltvXml;
global using StreamMaster.Streams.Domain.Interfaces;
global using StreamMaster.Streams.Domain.Models;
global using StreamMaster.Streams.Domain.Statistics;

global using X.PagedList;