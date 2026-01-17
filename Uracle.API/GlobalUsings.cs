global using Carter;
global using Mapster;
global using MediatR;
global using Uracle.API;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using SharedKernel.Exceptions.Handler;
global using Uracle.Application;
global using Uracle.Infrastructure;
global using Uracle.Infrastructure.Data.Extensions;
global using Uracle.Application.DTOs;
global using Uracle.API.Middleware;
global using Microsoft.AspNetCore.Authentication.JwtBearer;

global using Microsoft.IdentityModel.Tokens;
global using System.Text;
global using Microsoft.AspNetCore.Http;
global using Uracle.Application.Commands.ContestsCommand;
global using Uracle.Application.DTOs.ContestDto;
global using Uracle.Application.Queries.UsersQuery;
global using Uracle.Application.Queries.ContestsQuery;

global using Uracle.Application.Queries.GroupsQuery;

global using Microsoft.Extensions.Options;
global using Uracle.Application.Commands.UsersCommand;
global using Uracle.Infrastructure.Options;

global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;