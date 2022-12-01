using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagementPanel.ApplicationService.Interfaces;
using MovieManagementPanel.Domain.Entities;
using MovieManagementPanel.WebApp.Models;

namespace MovieManagementPanel.WebApp.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        #region Ctor&Fields

        private readonly ILogger<MovieController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public MovieController(ILogger<MovieController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var movies = await _unitOfWork.Movies.Find(i => i.IsActive)
                .Include(x => x.MoviesAndSaloons.Where(x => x.IsActive)).ThenInclude(x => x.Saloon)
                .AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            var listModel = movies.Select(x => new MovieListModel()
            {
                MovieId = x.Id,
                Name = x.Name,
                RealeseDate = x.ReleaseDate,
                Saloons = string.Join(",", x.MoviesAndSaloons.Select(x => x.Saloon.Name).ToList())
            });

            return View(listModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(MovieListSearchModel model, CancellationToken cancellationToken)
        {
            var iQueryableMovies = _unitOfWork.Movies.Find()
                .Include(x => x.MoviesAndSaloons.Where(i => i.IsActive)).ThenInclude(x => x.Saloon)
                .Where(i => i.IsActive);

            if (model.RealeseDate_Start.HasValue && model.RealeseDate_End.HasValue)
            {
                iQueryableMovies = iQueryableMovies.Where(i => i.ReleaseDate >= model.RealeseDate_Start && i.ReleaseDate <= model.RealeseDate_End);
            }
            if (model.SaloonId.HasValue && model.SaloonId != default)
            {
                #region 1.Yöntem

                var moviesAndSaloons = await _unitOfWork.MoviesAndSaloons
                           .Find(i => i.IsActive && i.Saloon.Id == model.SaloonId)
                           .Include(i => i.Movie)
                           .AsNoTrackingWithIdentityResolution()
                           .ToListAsync();

                var onlyMovieIds = moviesAndSaloons.Select(i => i.Movie.Id).ToList();

                iQueryableMovies = iQueryableMovies.Where(i => onlyMovieIds.Contains(i.Id));

                #endregion

                #region 2.Yöntem

                //iQueryableMovies = iQueryableMovies.Where(i => i.MoviesAndSaloons.Any(x => x.Saloon.Id == model.SaloonId));

                #endregion
            }

            var movies = await iQueryableMovies.AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            var listModel = movies.Select(x => new MovieListModel()
            {
                MovieId = x.Id,
                Name = x.Name,
                RealeseDate = x.ReleaseDate,
                Saloons = string.Join(",", x.MoviesAndSaloons.Select(x => x.Saloon.Name).ToList())
            });

            return View(listModel);
        }

        public async Task<ActionResult> Create(CancellationToken cancellationToken)
        {
            var saloons = await _unitOfWork.Saloons.Find(i => i.IsActive).AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            ViewBag.Saloons = saloons;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MovieCreateModel model, CancellationToken cancellationToken)
        {
            var movieEntity = await _unitOfWork.Movies.AddAsyncReturnEntity(new()
            {
                Name = model.Name,
                Description = model.Description,
                ReleaseDate = model.RealeseDate
            }, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            if (model.SaloonsId.Any())
            {
                var saloonsEntities = await _unitOfWork.Saloons.Find(i => i.IsActive && model.SaloonsId.Contains(i.Id)).ToListAsync(cancellationToken);

                var movieAndSalonList = saloonsEntities.Select(i => new MovieAndSaloon()
                {
                    Movie = movieEntity,
                    Saloon = i
                }).ToList();

                await _unitOfWork.MoviesAndSaloons.UpdateRangeAsync(movieAndSalonList, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var movie = await _unitOfWork.Movies.Find(i => i.IsActive && i.Id == id)
                .Include(i => i.MoviesAndSaloons.Where(i => i.IsActive)).ThenInclude(x => x.Saloon)
                .AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(cancellationToken);
            if (movie == null)
            {
                return Redirect(Url.Action("error", "errors") ?? nameof(Index));
            }

            var onlySaloonIds = movie.MoviesAndSaloons.Select(i => i.Saloon.Id).Distinct().ToList();

            var saloons = await _unitOfWork.Saloons.Find(i => i.IsActive)
                .AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            ViewBag.Saloons = saloons.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString(),
                Selected = onlySaloonIds.Any(x => x == i.Id)
            }).ToList();

            return View(new MovieEditModel()
            {
                Description = movie.Description,
                Id = id,
                Name = movie.Name,
                RealeseDate = movie.ReleaseDate,
                SaloonsId = onlySaloonIds

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MovieEditModel model, CancellationToken cancellationToken)
        {
            var movieEntity = await _unitOfWork.Movies.GetByIdAsync(model.Id, cancellationToken);
            if (movieEntity == null)
            {
                return Redirect(Url.Action("error", "errors") ?? nameof(Index));
            }

            movieEntity.Name = model.Name;
            movieEntity.Description = model.Description;
            movieEntity.ReleaseDate = model.RealeseDate;

            await _unitOfWork.Movies.UpdateAsync(movieEntity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var movieAndSaloonEntities = await _unitOfWork.MoviesAndSaloons
                .Find(i => i.IsActive && i.Movie.Id == model.Id)
                .ToListAsync(cancellationToken);
            if (movieAndSaloonEntities.Any())
            {
                await _unitOfWork.MoviesAndSaloons.DeleteRangeAsync(movieAndSaloonEntities, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            if (model.SaloonsId.Any())
            {
                var saloonsEntities = await _unitOfWork.Saloons.Find(i => i.IsActive && model.SaloonsId.Contains(i.Id)).ToListAsync(cancellationToken);
                if (saloonsEntities.Any())
                {
                    var addMoviveAndSaloonEntities = saloonsEntities.Select(i => new MovieAndSaloon()
                    {
                        Movie = movieEntity,
                        Saloon = i
                    });

                    await _unitOfWork.MoviesAndSaloons.AddRangeAsync(addMoviveAndSaloonEntities, cancellationToken);
                    await _unitOfWork.CommitAsync(cancellationToken);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == default)
            {
                return Redirect(Url.Action("error", "errors") ?? nameof(Index));
            }

            await _unitOfWork.Movies.DeleteAsync(movie, cancellationToken);

            var movieAndSaloons = await _unitOfWork.MoviesAndSaloons
                .Find(i => i.IsActive && i.Movie == movie)
                .Include(i => i.Movie)
                .ToListAsync(cancellationToken);
            if (movieAndSaloons.Any())
            {
                await _unitOfWork.MoviesAndSaloons.DeleteRangeAsync(movieAndSaloons, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

    }
}
