using FluentValidation;
using Lab3.Features.Books;
using Lab3.Persistence;
using Lab3.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BooksManagementContext>(options =>
    options.UseSqlite("Data Source=bookmanagement.db"));
builder.Services.AddScoped<CreateBookHandler>();
builder.Services.AddScoped<GetAllBooksHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();
