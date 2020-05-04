﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Netptune.Core.Services;
using Netptune.Models;

namespace Netptune.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardService BoardService;
        private readonly UserManager<AppUser> UserManager;

        public BoardsController(IBoardService boardService, UserManager<AppUser> userManager)
        {
            BoardService = boardService;
            UserManager = userManager;
        }

        // GET: api/boards
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json", Type = typeof(List<Board>))]
        public async Task<IActionResult> GetBoards(int projectId)
        {
            var result = await BoardService.GetBoards(projectId);

            return Ok(result);
        }

        // GET: api/boards/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json", Type = typeof(Board))]
        public async Task<IActionResult> GetBoard([FromRoute] int id)
        {
            var result = await BoardService.GetBoard(id);

            return Ok(result);
        }

        // PUT: api/boards/5
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json", Type = typeof(Board))]
        public async Task<IActionResult> PutBoard([FromBody] Board board)
        {
            var result = await BoardService.UpdateBoard(board);

            return Ok(result);
        }

        // POST: api/boards
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json", Type = typeof(Board))]
        public async Task<IActionResult> PostBoard([FromBody] Board board)
        {
            var result = await BoardService.AddBoard(board);

            return Ok(result);
        }

        // DELETE: api/ProjectBoards/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json", Type = typeof(Board))]
        public async Task<IActionResult> DeleteBoard([FromRoute] int id)
        {
            var user = await UserManager.GetUserAsync(User);
            var result = await BoardService.DeleteBoard(id, user);

            return Ok(result);
        }
    }
}