using Blog.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public abstract class BaseController<TEntity> : Controller where TEntity : class
    {
        protected readonly IRepository<TEntity> Repository;

        // Constructor accepting the generic repository
        protected BaseController(IRepository<TEntity> repository)
        {
            Repository = repository;
        }

        // Additional shared functionality (e.g., helper methods) can be added here
    }
}
