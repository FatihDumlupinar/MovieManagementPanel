using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagementPanel.ApplicationService.Interfaces;
using MovieManagementPanel.Domain.Entities;
using MovieManagementPanel.WebApp.Models;

namespace MovieManagementPanel.WebApp.Controllers
{
    [Authorize]
    public class SaloonController : Controller
    {
        #region Ctor&Fields

        private readonly ILogger<SaloonController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SaloonController(ILogger<SaloonController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var saloons = await _unitOfWork.Saloons
                .Find(i => i.IsActive)
                .Include(i => i.MoviesAndSaloons.Where(x => x.IsActive)).ThenInclude(i => i.Movie)
                .AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            var listModel = saloons.Select(i => new SaloonListModel()
            {
                Id = i.Id,
                Name = i.Name,
                Movies = string.Join(",", i.MoviesAndSaloons.Select(i => i.Movie.Name).ToList())

            }).ToList();

            return View(listModel);
        }

        public async Task<ActionResult> Create(CancellationToken cancellationToken)
        {
            var movies = await _unitOfWork.Movies.Find(i => i.IsActive)
                .AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            ViewBag.Movies = movies;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaloonCreateModel model, CancellationToken cancellationToken)
        {
            var saloonEntity = await _unitOfWork.Saloons.AddAsyncReturnEntity(new()
            {
                Name = model.Name,
            }, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            if (model.MovieIds.Any())
            {
                var movies = await _unitOfWork.Movies.
                    Find(i => i.IsActive && model.MovieIds.Contains(i.Id))
                    .AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

                var movieAndSaloonList = movies.Select(i => new MovieAndSaloon()
                {
                    Movie = i,
                    Saloon = saloonEntity
                });

                await _unitOfWork.MoviesAndSaloons.AddRangeAsync(movieAndSaloonList, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var saloonEntity = await _unitOfWork.Saloons
                .Find(i => i.IsActive && i.Id == id)
                .Include(i => i.MoviesAndSaloons.Where(i=>i.IsActive)).ThenInclude(i => i.Movie)
                .AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(cancellationToken);
            if (saloonEntity == null)
            {
                return Redirect(Url.Action("error", "errors") ?? nameof(Index));
            }

            var onlyMovieIds = saloonEntity.MoviesAndSaloons.Select(i => i.Movie.Id).ToList();

            var movies = await _unitOfWork.Movies.Find(i => i.IsActive)
                .AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            ViewBag.Movies = movies.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString(),
                Selected = onlyMovieIds.Any(x => x == i.Id)
            }).ToList();

            return View(new SaloonEditModel()
            {
                Id = id,
                Name = saloonEntity.Name,
                MovieIds = onlyMovieIds

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SaloonEditModel model, CancellationToken cancellationToken)
        {
            var saloonEntity = await _unitOfWork.Saloons.GetByIdAsync(model.Id, cancellationToken);
            if (saloonEntity == null)
            {
                return Redirect(Url.Action("error", "errors") ?? nameof(Index));
            }

            saloonEntity.Name = model.Name;

            await _unitOfWork.Saloons.UpdateAsync(saloonEntity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var movieAndSaloonEntities = await _unitOfWork.MoviesAndSaloons
               .Find(i => i.IsActive && i.Saloon.Id == model.Id)
               .ToListAsync(cancellationToken);
            if (movieAndSaloonEntities.Any())
            {
                await _unitOfWork.MoviesAndSaloons.DeleteRangeAsync(movieAndSaloonEntities, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            if (model.MovieIds.Any())
            {
                var movies = await _unitOfWork.Movies.
                    Find(i => i.IsActive && model.MovieIds.Contains(i.Id)).ToListAsync(cancellationToken);

                var movieAndSaloonList = movies.Select(i => new MovieAndSaloon()
                {
                    Movie = i,
                    Saloon = saloonEntity,
                });

                await _unitOfWork.MoviesAndSaloons.AddRangeAsync(movieAndSaloonList, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var saloonEntity = await _unitOfWork.Saloons.GetByIdAsync(id, cancellationToken);
            if (saloonEntity == null)
            {
                return Redirect(Url.Action("error", "errors") ?? nameof(Index));
            }

            await _unitOfWork.Saloons.DeleteAsync(saloonEntity, cancellationToken);

            var movieAndSaloonEntities = await _unitOfWork.MoviesAndSaloons
                .Find(i => i.IsActive && i.Saloon == saloonEntity)
                .Include(i => i.Saloon)
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync(cancellationToken);
            if (movieAndSaloonEntities.Any())
            {
                await _unitOfWork.MoviesAndSaloons.DeleteRangeAsync(movieAndSaloonEntities, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
    }
}
