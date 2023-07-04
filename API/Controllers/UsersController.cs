﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // it becomes GET .../api/users
    public class UsersController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public UsersController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<AppUser>>> GetUsers()
        { 
        
            var users = await _dataContext.Users.ToListAsync();

            return users;
        
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {

            return await _dataContext.Users.FindAsync(id);
        }

    }
}
