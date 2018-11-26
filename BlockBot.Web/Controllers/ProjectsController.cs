﻿using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlockBot.Module.Aws.Models;
using BlockBot.Module.Aws.ServiceInterfaces;
using BlockBot.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlockBot.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IApiGatewayService _apiGatewayService;
        private readonly ApplicationDbContext _context;
        private readonly UrlEncoder _urlEncoder;
        private readonly ApplicationUserManager _userManager;

        public ProjectsController(ApplicationDbContext context,
            ApplicationUserManager userManager,
            UrlEncoder urlEncoder,
            IApiGatewayService apiGatewayService)
        {
            _context = context;
            _userManager = userManager;
            _urlEncoder = urlEncoder;
            _apiGatewayService = apiGatewayService;
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                project.OwnerId = user.Id;
                project.XML =
                    "<xml xmlns=\"http://www.w3.org/1999/xhtml\" id=\"workspaceBlocks\" style=\"display: none;\"></xml>";

                ApiGatewayRestApi result = await _apiGatewayService.CreateApiGateway(
                    _urlEncoder.Encode(user.NormalizedUserName + "-" + project.Name).ToLowerInvariant(),
                    "TODO add permalink to project");

                project.RestApiId = result.RestApiId;

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Name")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Project originalProject = await _context.Projects.FindAsync(id);

            if (originalProject == null)
            {
                return NotFound();
            }

            if (originalProject.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Project project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            // exception should be thrown if delete fails
            await _apiGatewayService.DeleteApiGateway(project.RestApiId);

            // TODO delete S3 and Lambda resources

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "Dashboard");
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}