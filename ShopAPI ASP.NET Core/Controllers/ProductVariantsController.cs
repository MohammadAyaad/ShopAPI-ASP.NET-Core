﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Authorization;
using ShopAPI.Data;
using ShopAPI.Model.Products;
using static ShopAPI.Model.Moderation.Permissions;
using ShopAPI.Model.TokenComponents;

namespace ShopAPI.Controllers;

[Route("api/products")]
[ApiController]
public class ProductVariantsController : ControllerBase
{
    private readonly ShopDBContext _context;

    public ProductVariantsController(ShopDBContext context)
    {
        _context = context;
    }

    // PUT: api/ProductVariants/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{productId}/variant/{variantId}")]
    public async Task<IActionResult> PutProductVariant(
        [FromHeader(Name = "Authorization")] string authorization,
        int productId,
        int variantId,
        ProductVariant productVariant)
    {
        var result = AuthorizationService.AuthorizeAccess(authorization, _context, EDIT_PRODUCTS);

        (JCST userToken, string email, AccessToken access) = result.Value;

        if (userToken != null)
        {
            if (variantId != productVariant.Id || productId != productVariant.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(productVariant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductVariantExists(variantId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        return result.Result;
    }

    // POST: api/ProductVariants
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("{productId}/variants/add")]
    public async Task<ActionResult<ProductVariant>> PostProductVariant(
        [FromHeader(Name = "Authorization")] string authorization,
        ProductVariant productVariant)
    {
        var result = AuthorizationService.AuthorizeAccess(authorization, _context, EDIT_PRODUCTS);

        (JCST userToken, string email, AccessToken access) = result.Value;

        if (userToken != null)
        {
            _context.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductVariant", new { id = productVariant.Id }, productVariant);
        }

        return result.Result;
    }

    // DELETE: api/ProductVariants/5
    [HttpDelete("{productId}/variants/delete/{variantId}")]
    public async Task<IActionResult> DeleteProductVariant(
        [FromHeader(Name = "Authorization")] string authorization,
        int productId,
        int variantId)
    {
        var result = AuthorizationService.AuthorizeAccess(authorization, _context, EDIT_PRODUCTS);

        (JCST userToken, string email, AccessToken access) = result.Value;

        if (userToken != null)
        {
            var productVariant = _context.ProductVariants.FirstOrDefault(v => v.ProductId == productId && v.Id == variantId);
            if (productVariant == null)
            {
                return NotFound();
            }

            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        return result.Result;
    }

    private bool ProductVariantExists(int id)
    {
        return _context.ProductVariants.Any(e => e.Id == id);
    }
}
