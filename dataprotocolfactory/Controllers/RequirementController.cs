using application.Interfaces;
using domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace datamanager.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RequirementController : ControllerBase
    {
        private readonly IRequirement _requirement;  
        public RequirementController(IRequirement requirement) 
        { 
            _requirement=requirement;
        }

        [HttpGet]
        public async Task<IActionResult> GetRequirement(string idrequirement) 
        {
            try
            {
                var response =await _requirement.GetRequirement(idrequirement);
                return Ok(response);    
            }
            catch (Exception e)
            { 
                return BadRequest();   
            }
        }
    
    }
}
