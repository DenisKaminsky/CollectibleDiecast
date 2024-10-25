﻿global using Asp.Versioning;
global using Asp.Versioning.Conventions;
global using System.Data.Common;
global using Npgsql;
global using System.Runtime.Serialization;
global using FluentValidation;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using CollectibleDiecast.EventBus.Abstractions;
global using CollectibleDiecast.EventBus.Events;
global using CollectibleDiecast.EventBus.Extensions;
global using CollectibleDiecast.IntegrationEventLogEF;
global using CollectibleDiecast.IntegrationEventLogEF.Services;
global using CollectibleDiecast.Ordering.API;
global using CollectibleDiecast.Ordering.API.Application.Behaviors;
global using CollectibleDiecast.Ordering.API.Application.Commands;
global using CollectibleDiecast.Ordering.API.Application.IntegrationEvents;
global using CollectibleDiecast.Ordering.API.Application.IntegrationEvents.EventHandling;
global using CollectibleDiecast.Ordering.API.Application.IntegrationEvents.Events;
global using CollectibleDiecast.Ordering.API.Application.Models;
global using CollectibleDiecast.Ordering.API.Application.Queries;
global using CollectibleDiecast.Ordering.API.Application.Validations;
global using CollectibleDiecast.Ordering.API.Extensions;
global using CollectibleDiecast.Ordering.API.Infrastructure;
global using CollectibleDiecast.Ordering.API.Infrastructure.Services;
global using CollectibleDiecast.Ordering.Domain.AggregatesModel.BuyerAggregate;
global using CollectibleDiecast.Ordering.Domain.AggregatesModel.OrderAggregate;
global using CollectibleDiecast.Ordering.Domain.Events;
global using CollectibleDiecast.Ordering.Domain.Exceptions;
global using CollectibleDiecast.Ordering.Domain.SeedWork;
global using CollectibleDiecast.Ordering.Infrastructure;
global using CollectibleDiecast.Ordering.Infrastructure.Idempotency;
global using CollectibleDiecast.Ordering.Infrastructure.Repositories;
global using Microsoft.Extensions.Options;
global using Polly;
global using Polly.Retry;
global using CollectibleDiecast.ServiceDefaults;