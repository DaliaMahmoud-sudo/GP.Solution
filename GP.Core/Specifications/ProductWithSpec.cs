using GP.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP.Core.Specifications;

namespace GP.Core.Specifications
{
    public class ProductWithSpec : BaseSpecifications<Product>
    {
       

        
        
            public ProductWithSpec(ProductSpecParams Params)
                : base(P => Params.Name.IsNullOrEmpty() || P.Name == Params.Name)
            {
                
                if (!string.IsNullOrEmpty(Params.Sort))
                {
                    switch (Params.Sort)
                    {
                        case "PriceAsc":
                            AddOrderBy(P => P.Price);
                            break;
                        case "PriceDesc":
                            AddOrderByDesc(P => P.Price);
                            break;
                        default:
                            AddOrderBy(p => p.Name);
                            break;

                    }
                }

                ApplyPagination(Params.PageSize * (Params.PageIndex - 1), Params.PageSize);
            }
          
        }
    }

