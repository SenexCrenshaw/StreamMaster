global using AutoMapper;

global using MediatR;

global using Microsoft.AspNetCore.SignalR;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Logging;

global using StreamMasterApplication.Common;
global using StreamMasterApplication.Common.Attributes;
global using StreamMasterApplication.Common.Events;
global using StreamMasterApplication.Common.Interfaces;
global using StreamMasterApplication.Hubs;
global using StreamMasterApplication.Settings.Queries;

global using StreamMasterDomain.Attributes;
global using StreamMasterDomain.Cache;
global using StreamMasterDomain.Common;
global using StreamMasterDomain.Dto;
global using StreamMasterDomain.Enums;
global using StreamMasterDomain.Repository;

global using X.PagedList;