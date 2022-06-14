using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Data;
using WarehouseManagementSystem.Models;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;

namespace WarehouseManagementSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WarehouseManagementSystemContext _context;

        public ProductsController(WarehouseManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int warehouseId)
        {
            if (_context.Products != null)
            {
                var products = await _context.Products.ToListAsync();
                var filteredProducts = products.Where(m => m.WarehouseId == warehouseId);
                ViewData["id"] = warehouseId;
                return View(filteredProducts);
            }
            return Problem("Entity set 'WarehouseManagementSystemContext.Product'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Products/Create/{warehouseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,Quantity,RefCode,Image,WarehouseId")] Product product, 
            [FromForm(Name = "file")] IFormFile file, int warehouseId)
        {
            var clientAccessKey = "DefaultEndpointsProtocol=https;AccountName=productstorageao;AccountKey=JIxeL8+FvIzEPkMnYwFM6I4JhnWpe/edw8u1EF0Z1V3HiaLdxDgQ2YzNmvV4d7kEyjxF9g1hs/1P+AStIuNJOw==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(clientAccessKey);
            var client = storageAccount.CreateCloudBlobClient();
            var imagesContainer = client.GetContainerReference("images");

            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');

            var blockBlob = imagesContainer.GetBlockBlobReference(fileName);
            var stream = file.OpenReadStream();
            await blockBlob.UploadFromStreamAsync(stream);
            product.Image = blockBlob.Uri;
            product.WarehouseId = warehouseId;
            ViewData["id"] = warehouseId;
            _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new {warehouseId=warehouseId});
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Quantity,RefCode,Image,WarehouseId")] Product product, [FromForm(Name = "file")] IFormFile file)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            var clientAccessKey = "DefaultEndpointsProtocol=https;AccountName=productstorageao;AccountKey=JIxeL8+FvIzEPkMnYwFM6I4JhnWpe/edw8u1EF0Z1V3HiaLdxDgQ2YzNmvV4d7kEyjxF9g1hs/1P+AStIuNJOw==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(clientAccessKey);
            var client = storageAccount.CreateCloudBlobClient();
            var imagesContainer = client.GetContainerReference("images");

            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');

            var blockBlob = imagesContainer.GetBlockBlobReference(fileName);
            var stream = file.OpenReadStream();
            await blockBlob.UploadFromStreamAsync(stream);
            product.Image = blockBlob.Uri;
            product.WarehouseId = product.WarehouseId;

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index), new { warehouseId = product.WarehouseId });
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'WarehouseManagementSystemContext.Product'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            var warehouseId = product.WarehouseId;
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { warehouseId = warehouseId });
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
