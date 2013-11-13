using System.Web.Mvc;
using MongoDB.Driver.Builders;
using dondEducar.Models;
using dondEducar.ViewModel;

namespace dondEducar.Controllers
{
    public class EstablecimientoController : BaseController
    {
        [HttpGet]
        public ActionResult Establecimiento(string establecimientoId)
        {
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            var query = Query<Establecimiento>.EQ(e => e.Id, establecimientoId);
            var establecimiento = establecimientos.FindOne(query);

            var fourSquareVenue = BuscarAgregarEstablecimiento(establecimiento);

            establecimiento.FourSquareVenueId = fourSquareVenue.id;
            establecimiento.Likes = fourSquareVenue.likes.count;
            establecimientos.Save(establecimiento);

            var establecimientoViewModel = new EstablecimientoViewModel
                {
                    Establecimiento = establecimiento,
                    Venue = fourSquareVenue,
                };

            return View("Establecimiento", establecimientoViewModel);
        }

    }
}
