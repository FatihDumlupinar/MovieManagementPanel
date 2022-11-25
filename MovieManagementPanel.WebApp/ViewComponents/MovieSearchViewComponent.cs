using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagementPanel.ApplicationService.Interfaces;
using MovieManagementPanel.Domain.Entities;
using MovieManagementPanel.WebApp.Models;

namespace MovieManagementPanel.WebApp.ViewComponents
{
    [ViewComponent(Name = "MovieSearch")]
    public class MovieSearchViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieSearchViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
        {
            ViewBag.Saloons = await GetItemsAsync(cancellationToken);
            return View("Default",new MovieListSearchModel());
        }

        private async Task<List<Saloon>> GetItemsAsync(CancellationToken cancellationToken)
        {
            var saloons = await _unitOfWork.Saloons.Find(i => i.IsActive)
                .AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

            return saloons;
        }
    }
}
