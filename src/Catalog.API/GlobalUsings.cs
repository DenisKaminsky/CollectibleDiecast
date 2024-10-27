﻿global using Asp.Versioning;
global using Asp.Versioning.Conventions;
global using CollectibleDiecast.Catalog.API;
global using CollectibleDiecast.Catalog.API.Infrastructure;
global using CollectibleDiecast.Catalog.API.Infrastructure.EntityConfigurations;
global using CollectibleDiecast.Catalog.API.Infrastructure.Exceptions;
global using CollectibleDiecast.Catalog.API.IntegrationEvents;
global using CollectibleDiecast.Catalog.API.IntegrationEvents.EventHandling;
global using CollectibleDiecast.Catalog.API.IntegrationEvents.Events;
global using CollectibleDiecast.Catalog.API.Model;
global using CollectibleDiecast.EventBus.Abstractions;
global using CollectibleDiecast.EventBus.Events;
global using CollectibleDiecast.IntegrationEventLogEF;
global using CollectibleDiecast.IntegrationEventLogEF.Services;
global using CollectibleDiecast.ServiceDefaults;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Options;
global using Npgsql;
