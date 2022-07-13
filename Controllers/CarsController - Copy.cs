using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarSale.Data;
using CarSale.Models;
using Microsoft.AspNetCore.Authorization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static CarSale.Models.Enumerators;

namespace CarSale.Controllers
{
    public class CarsController : Controller
    {
        private static int maxNoImages = 6;
        private static int maxImageWidth = 1200;
        private static int maxImageHeight = 1000;


        private readonly CarSaleContext _context;

        public CarsController(CarSaleContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index( string sortOrder, string searchString, Make makeFilter)
        {
            ViewData["MakeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "make_desc" : "";
            ViewData["YearSortParm"] = sortOrder == "Date" ? "year_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;
            var car = from s in _context.Car
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                car = car.Where(s => s.Model.Contains(searchString)
                                       || s.Make.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_desc":
                    car = car.OrderByDescending(s => s.InDate);
                    break;
                case "Date":
                    car = car.OrderBy(s => s.InDate);
                    break;
                case "year_desc":
                    car = car.OrderByDescending(s => s.Year);
                    break;
                case "year":
                    car = car.OrderBy(s => s.Year);
                    break;
                default:
                    car = car.OrderBy(s => s.Model);
                    break;
            }
            return View(await car.AsNoTracking().ToListAsync());

            return _context.Car != null ?
                        View(await _context.Car.ToListAsync()) :
                        Problem("Entity set 'CarSaleContext.Car'  is null.");
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            //Get number of files in folder
            var cars = car;
            var path =Directory.GetCurrentDirectory() + "/wwwroot/cars";
          ViewBag.NoOfImageFiles= Directory.GetFiles(path, cars.Id.ToString() + "-*_m.jpg", SearchOption.TopDirectoryOnly).Length;
            //endfile
            return View(car);
        }

        // GET: Cars/Create

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Make,Model,Year,color,NewOrUsed,ImageLink,States,Price")] Car car)
        {
            if (ModelState.IsValid)
            {
                //ADD LOGGED IN USER NAME TO UPLOADED 

                car.UploadedBy = User.Identity.Name;

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Make,Model,Year,color,NewOrUsed,ImageLink,States,Price,InDate")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //CHANGE TO ALLOW ONLY UPLOADED USER EDIT
                if (car.UploadedBy.Equals(User.Identity.Name))
                {
                    try
                    {
                        _context.Update(car);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        if (!CarExists(car.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    return View("Unauthorized");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Car == null)
            {
                return Problem("Entity set 'CarSaleContext.Car'  is null.");
            }
            var car = await _context.Car.FindAsync(id);
            if (car.UploadedBy.Equals(User.Identity.Name))
            {
                if (car != null)
                {
                //ALLOW DELETE FROM UPLOADEDBY USER
                

                    _context.Car.Remove(car);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                return View("Unauthorized");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //Unauthorized

        public async Task<IActionResult> Unauthorized()
        {

            return View();


        }
        public IActionResult FileUploadPage(int? Id)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FileUpLoadPage(List<IFormFile> files, int? Id)
        {

            var car = await _context.Car.FindAsync(Id);
            if (car.UploadedBy.Equals(User.Identity.Name))
            {
              
            

            var fileSize = files.Sum(m => m.Length);
            var fileNames = new List<string>();
            ViewBag.Message ="files uploaded ";

            int fileNumber = 0;
            foreach (var file in files)
            {
                if (fileNumber < maxNoImages)
                {

                    fileNumber++;

                    string imageFolder = "\\wwwroot\\Cars";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory() + imageFolder, Id.ToString() + "-" + fileNumber.ToString() + "temp.jpg" );

                        //var filePath = Path.Combine(Directory.GetCurrentDirectory() + imageFolder, Id.ToString() + "-" + fileNumber.ToString() + ".jpg");

                        fileNames.Add(file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {

                        //mofifiction image sharp
                        using var image = Image.Load(file.OpenReadStream());
                        if(image.Width > maxImageWidth || image.Height > maxImageHeight)
                        {
                            return Ok("Image is too big (Width, Height)" + ", " + image.Height);
                        }

                        image.Mutate(x => x.Resize(200, 150));
                        image.SaveAsJpeg("wwwroot/cars/"+ Id.ToString() + "-" + fileNumber.ToString() + "_s.jpg");
                        image.Dispose();

                       // await file.CopyToAsync(stream);
                    }
                        var filePath2 = Path.Combine(Directory.GetCurrentDirectory() + imageFolder, "temp.jpg");

                        fileNames.Add(filePath);
                        using (var stream = new FileStream(filePath2, FileMode.Create))
                        {
                            using var image = Image.Load(file.OpenReadStream());
                            if (image.Width > maxImageWidth || image.Height > maxImageHeight)
                            {

                                return Ok("Image is too big (Width, Height)" + ", " + image.Height);
                            }
                            image.Mutate(x => x.Resize(1000, 600));
                            image.SaveAsJpeg("wwwroot/cars/" + Id.ToString() + "-" + fileNumber.ToString() + "_m.jpg");
                            image.Dispose();

                    }

                    ViewBag.Message += file.FileName + ", ";
                }

            }

            return View();


            }
            else
            {
                return View("Unauthorized");
            }
        }
    }    
}
